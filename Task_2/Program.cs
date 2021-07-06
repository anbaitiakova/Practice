using System;
using System.IO;

namespace Task_2
{
    enum Sevirity
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical,
        Severity
    }
    
    class Program
    { 
        public sealed class Logger : IDisposable
        {
            private readonly StreamWriter _logWriter;
            public Logger(string filePath)
            {
                _logWriter = new StreamWriter(filePath);
            }

            public void Log(string MessageForLog, Sevirity sevirity)
            {
                _logWriter.WriteLine($"[{DateTime.Now:G}][{sevirity}]: {MessageForLog}");
            }
            
            public void Dispose()
            {
                _logWriter.Dispose();
                GC.SuppressFinalize(this);
            }

            ~Logger()
            {
                _logWriter.Dispose();
            }
            
        }
        
        
        static void Main(string[] args)
        {
            using (Logger logger = new Logger(args[0]))
            {
                logger.Log("A lot of beer:)", Sevirity.Information);
                logger.Log("Lab about logger", Sevirity.Error);
                logger.Log("pupupu:3", Sevirity.Critical);
                logger.Log("Something intersting", Sevirity.Debug);
                logger.Log("Hello!", Sevirity.Trace);
            }
        }
    }
}