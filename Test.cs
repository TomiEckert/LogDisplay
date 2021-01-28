using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogDisplay.Configs;
using NUnit.Framework;

namespace LogDisplay {
    public class Test {
        private const string START_MESSAGE = "Application started";
        private const string CRITICAL_MESSAGE = "A critical error happened";
        private const string PATH = "log.txt";
        private const string SAVE_MESSAGE = "Log has been saved, well done";
        
        [SetUp]
        public void Setup() {
            Logger.Instance.Log(START_MESSAGE);
            Logger.Instance.Log(CRITICAL_MESSAGE, LogType.Critical);
        }
        
        [Test]
        public void TestMethod1() {
            

            var criticalLogs = Logger.Instance.GetAllLogs(LogType.Critical);
            var warningLogs = Logger.Instance.GetAllLogs(LogType.Warning);
            var infoLogs = Logger.Instance.GetLastNLogs(4, LogType.Info);
            var debugLogs = Logger.Instance.GetLastNLogs(12);
            
            Assert.AreEqual(1, criticalLogs.Length);
            Assert.AreEqual(1, warningLogs.Length);
            Assert.AreEqual(1, infoLogs.Length);
            Assert.AreEqual(2, debugLogs.Length);
            Assert.AreEqual(CRITICAL_MESSAGE, criticalLogs[0].Message);
            Assert.AreEqual(CRITICAL_MESSAGE, warningLogs[0].Message);
            Assert.AreEqual(CRITICAL_MESSAGE, infoLogs[0].Message);
            Assert.AreEqual(START_MESSAGE, debugLogs[0].Message);
            Assert.AreEqual(CRITICAL_MESSAGE, debugLogs[1].Message);
        }
        
        [Test]
        public void TestMethod2() {
            var monitor = new Monitor();
            var list = new List<string>();
            monitor.OutAction = OutActions.DefaultAction;
            monitor.OutAction = list.Add;
            monitor.UpdateDelay = 100;
            
            Assert.AreEqual(0, list.Count);
            monitor.StartAsync().Wait();
            Logger.Instance.Log(START_MESSAGE);
            Task.Delay(200).Wait();
            Assert.AreEqual(2, list.Count);
            StringAssert.Contains(START_MESSAGE, list[1]);
        }

        [Test]
        public void TestMethod3() {
            var config = new LogConfigBuilder()
                         .SaveLog()
                         .SetPath(PATH)
                         .Build();
            Logger.Instance.LoadConfig(config);
            
            Logger.Instance.Log(SAVE_MESSAGE);
            var text = File.ReadAllText(PATH);
            StringAssert.Contains(SAVE_MESSAGE, text);
        }
    }
}