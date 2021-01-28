namespace LogDisplay.Configs {
    public class LogConfigBuilder {
        private string Path { get; set; }
        private bool IsLogSaved { get; set; }
        
        public LogConfigBuilder() {
            Path = "log.txt";
            IsLogSaved = false;
        }

        public LogConfigBuilder SetPath(string path) {
            Path = path;
            return this;
        }
        
        public LogConfigBuilder SaveLog() {
            IsLogSaved = true;
            return this;
        }

        public LogConfig Build() {
            return new LogConfig(Path, IsLogSaved);
        }
    }
}