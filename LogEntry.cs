using System;
using System.Reflection;

namespace LogDisplay {
    internal class LogEntry {
        internal TimeSpan Time { get; }
        internal string Message { get; }
        internal LogType Type { get; }
        internal MethodInfo Sender { get; }

        internal LogEntry(TimeSpan time, string message, LogType type, MethodInfo sender) {
            Time = time;
            Message = message;
            Type = type;
            Sender = sender;
        }

        internal string GetMessage() {
            var time = "[" + Time.ToString(@"hh\:mm\:ss") + "] ";
            var type = "[" + Type + "] ";
            var sender = "[" + Sender.DeclaringType?.FullName + "." + Sender.Name + "] ";
            return time + type + sender + Message;
        }

        internal LogColor GetColor() {
            return Type switch {
                LogType.Critical => LogColor.Red,
                LogType.Warning => LogColor.Yellow,
                LogType.Info => LogColor.Black,
                LogType.Debug => LogColor.Blue,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}