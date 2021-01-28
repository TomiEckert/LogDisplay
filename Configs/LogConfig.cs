namespace LogDisplay.Configs {
    public class LogConfig {
        internal string Path { get; }
        internal bool IsLogSaved { get; }

        public LogConfig(string path, bool isLogSaved) {
            Path = path;
            IsLogSaved = isLogSaved;
        }
    }
}