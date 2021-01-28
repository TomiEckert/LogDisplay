using System;

namespace LogDisplay {
    public class ExportLogEntry {
        public TimeSpan Time { get; }
        public string Message { get; }
        public string Type { get; }
        public string Sender { get; }

        public ExportLogEntry(TimeSpan time, string message, string type, string sender) {
            Time = time;
            Message = message;
            Type = type;
            Sender = sender;
        }

        internal static ExportLogEntry From(LogEntry entry) {
            var sender = entry.Sender.DeclaringType?.FullName + "." + entry.Sender.Name;
            return new ExportLogEntry(entry.Time, entry.Message, entry.Type.ToString(), sender);
        }
    }
}