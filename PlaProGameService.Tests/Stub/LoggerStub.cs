namespace PlaProGameService.Tests.Stub
{
    using System;
    using Logging;

    public class LoggerStub: ILogger
    {
        public void Debug(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }

        public void Error(string message, Exception exception)
        {
        }
    }
}
