using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using LogDisplay.Configs;
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable MemberCanBePrivate.Global

namespace LogDisplay {
    public class Logger {
        private readonly object _fileLock = new object();
        
        public static readonly Logger Instance = new Logger();
        
        internal delegate void NewLogEntryEventHandler(LogEntry logEntry);
        internal event NewLogEntryEventHandler NewLogEntry;
        
        private readonly object _logLock = new object();
        private List<LogEntry> Logs { get; }
        private Stopwatch Timer { get; }
        private string Path { get; set; }
        private bool IsLogSaved { get; set; }
        private bool _isSubscribed;

        public Logger() {
            Logs = new List<LogEntry>();
            Timer = new Stopwatch();
            Timer.Start();
        }

        public void LoadConfig(LogConfig config) {
            Path = config.Path;
            IsLogSaved = config.IsLogSaved;
            if (_isSubscribed) NewLogEntry -= Save;
            if (!IsLogSaved) return;
            NewLogEntry += Save;
            _isSubscribed = true;
        }

        public void Log(string message) {
            var log = new LogEntry(Timer.Elapsed, message, LogType.Debug, GetSender());
            lock(_logLock) Logs.Add(log);
            NewLogEntry?.Invoke(log);
        }
        public void Log(string message, LogType type) {
            var log = new LogEntry(Timer.Elapsed, message, type, GetSender());
            lock(_logLock) Logs.Add(log);
            NewLogEntry?.Invoke(log);
        }

        public ExportLogEntry[] GetAllLogs(LogType type = LogType.Debug) {
            ExportLogEntry[] logs;
            var filter = (LogType) ((int) type * 2 - 1);
            lock (_logLock) logs = Logs
                                   .Where(x=>filter.HasFlag(x.Type))
                                   .Select(ExportLogEntry.From)
                                   .ToArray();

            return logs;
        }

        public ExportLogEntry[] GetLastNLogs(int n, LogType type = LogType.Debug) {
            ExportLogEntry[] logs;
            var filter = (LogType) ((int) type * 2 - 1);
            lock (_logLock) logs = Logs
                                   .Where(x=>filter.HasFlag(x.Type))
                                   .Skip(Logs.Count - n)
                                   .Select(ExportLogEntry.From)
                                   .ToArray();
            return logs;
        }

        private MethodInfo GetSender() {
            return (MethodInfo)new StackTrace().GetFrame(2)?.GetMethod();
        }

        private void Save(LogEntry entry) {
            lock (_fileLock) {
                var sw = File.AppendText(Path);
                sw.WriteLine(entry.GetMessage());
                sw.Close();
            }
        }
    }
}