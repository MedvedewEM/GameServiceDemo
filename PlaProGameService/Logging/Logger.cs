namespace PlaProGameService.Logging
{
    using System;
    using NLog.Web;

    public class Logger: ILogger
    {
        private readonly NLog.Logger logger;

        public Logger()
        {
            logger = NLogBuilder.ConfigureNLog("nlog.config").GetLogger("GameServiceLogger");
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }

        public void Warn(string message)
        {
            logger.Warn(message);
        }

        public void Error(string message, Exception exception)
        {
            logger.Error(exception, message);
        }
    }
}
