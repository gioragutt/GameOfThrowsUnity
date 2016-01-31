using System.Text;
using log4net;

namespace GotLoggingService
{
    public sealed partial class LoggerManager
    {
        private ILog Logger { get; }

        #region Info Methods

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Logger.InfoFormat(format, args);
        }

        #endregion

        #region Debug Methods

        public void Debug(string message)
        {
            Logger.Debug(message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Logger.DebugFormat(format, args);
        }

        #endregion

        #region Warn Methods

        public void Warn(string message)
        {
            Logger.Warn(message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Logger.WarnFormat(format, args);
        }

        #endregion

        #region Error Methods

        public void Error(string message)
        {
            Logger.Error(message);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Logger.ErrorFormat(format, args);
        }

        #endregion

        #region Fatal Methods

        public void Fatal(string message)
        {
            Logger.Fatal(message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Logger.FatalFormat(format, args);
        }

        #endregion

        #region Getters

        public static LoggerManager GetLogger(string name)
        {
            return new LoggerManager(name);
        }

        public static LoggerManager GetLogger(string name, LoggerManager parent)
        {
            return new LoggerManager(name, parent);
        }

        public static LoggerManager GetRootLogger()
        {
            return RootLooger;
        }

        #endregion
    }
    
    // Manage stuff here
    public sealed partial class LoggerManager
    {

        // Initializes root logger
        private LoggerManager()
        {
            log4net.Config.XmlConfigurator.Configure(); // Missing!!?
            Logger = LogManager.GetLogger(ROOT_LOGGER_NAME);
        }

        private LoggerManager(string name) : this(name, GetRootLogger())
        {
        }

        private LoggerManager(string name, LoggerManager parent)
        {
            Logger = GetILog(name, parent);
        }

        private const string ROOT_LOGGER_NAME = "root";

        private static readonly LoggerManager RootLooger = new LoggerManager();

        private static ILog GetILog(string name, LoggerManager parent)
        {
            return LogManager.GetLogger(GetLoggerName(name, parent.Logger.Logger.Name));
        }

        private static string GetLoggerName(string name, string parent = ROOT_LOGGER_NAME)
        {
            StringBuilder nameBuilder = new StringBuilder();
            nameBuilder.AppendFormat("{0}.{1}", LogManager.GetLogger(parent).Logger.Name, name);
            return nameBuilder.ToString();
        }
    }
}
