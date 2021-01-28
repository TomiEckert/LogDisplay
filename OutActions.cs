using System;

namespace LogDisplay {
    public static class OutActions {
        public static void ConsoleWriteAction(string message) => Console.Write(message);
        public static void DefaultAction(string message) => ConsoleWriteAction(message);
        public static void NoAction(string message) { }
    }
}