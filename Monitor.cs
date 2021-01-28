using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ReSharper disable MemberCanBePrivate.Global

namespace LogDisplay {
    public class Monitor {
        private readonly object _outLock = new object();
        private readonly object _foreLock = new object();
        private readonly object _backLock = new object();
        private readonly object _monitorLock = new object();
        private readonly object _queueLock = new object();
        
        private readonly Queue<LogEntry> _queue;
        
        private Action<string> _outAction;
        private Action<LogColor> _foreColorSetAction;
        private Action<LogColor> _backColorSetAction;

        private bool _isMonitoring;
        private Task _monitoringThread;

        public bool IsBackColorChanging { get; set; }
        public Action<string> OutAction {
            get => _outAction;
            set {
                lock (_outLock) _outAction = value;
            }
        }
        public Action<LogColor> ForeColorSetAction {
            get => _foreColorSetAction;
            set {
                lock (_foreLock) _foreColorSetAction = value;
            }
        }
        public Action<LogColor> BackColorSetAction {
            get => _backColorSetAction;
            set {
                lock (_backLock) _backColorSetAction = value;
            }
        }
        public int UpdateDelay { get; set; }

        public Monitor() {
            _outAction = OutActions.DefaultAction;
            _queue = new Queue<LogEntry>();
            UpdateDelay = 500;
            Logger.Instance.NewLogEntry += entry => {
                lock (_queueLock)
                    _queue.Enqueue(entry);
            };
        }
        
        public Task StartAsync() {
            Logger.Instance.Log("Monitor started.", LogType.Info);
            lock(_monitorLock) _isMonitoring = true;
            _monitoringThread = MonitorLogsAsync();
            return Task.CompletedTask;
        }
        public async Task StopAsync() {
            _isMonitoring = false;
            await _monitoringThread;
        }

        private async Task MonitorLogsAsync() {
            bool running;
            lock (_monitorLock) running = _isMonitoring;
            
            while (running) {
                int queueCount;
                lock (_queueLock) queueCount = _queue.Count;
                if (queueCount != 0)
                    await DisplayEntryAsync();

                await Task.Delay(UpdateDelay);
                lock (_monitorLock) running = _isMonitoring;
            }
        }

        private Task DisplayEntryAsync() {
            LogEntry entry;
            lock (_queue) entry = _queue.Dequeue();

            var message = entry.GetMessage();
            var color = entry.GetColor();
            
            if (BackColorSetAction != null && IsBackColorChanging)
                lock (_backLock) BackColorSetAction(color);
            else if (ForeColorSetAction != null && !IsBackColorChanging)
                lock (_foreLock) ForeColorSetAction(color);

            lock (_outLock) OutAction(message);
            
            return Task.CompletedTask;
        }
    }
}