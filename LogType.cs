using System;

namespace LogDisplay {
    [Flags]
    public enum LogType {
        Critical = 1,
        Warning = 2,
        Info = 4,
        Debug = 8
    }
}