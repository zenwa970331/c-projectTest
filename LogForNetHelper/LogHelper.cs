using log4net.Config;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Layout;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace LogHelper
{
    public class Log4netHelper
    {
        private static readonly object objLock = new object();

        private static Log4netHelper _logHelper = null;

        private log4net.ILog Logger;

        public static Log4netHelper LogerHelper
        {
            get 
            {
                if (_logHelper is null || _logHelper.Logger is null)
                {
                    lock (objLock)
                    {
                        if (_logHelper is null)
                        {
                            _logHelper = new Log4netHelper();
                            Setup();
                            _logHelper.Logger = LogManager.GetLogger("LoggerHelper");//改可输入#
                        }
                    }
                }
                return _logHelper;
            }
        }
        private Log4netHelper(){}
        //加头加尾
        public static void Setup(string filePath = @"D:/LogFile/")
        {
            RollingFileAppender appender = new RollingFileAppender();
            appender.Name = "RollingLogFileAppender";
            appender.File = filePath;
            appender.RollingStyle = RollingFileAppender.RollingMode.Composite;
            appender.MaxSizeRollBackups = 30;
            appender.MaximumFileSize = "2GB";
            appender.StaticLogFileName = false;
            appender.AppendToFile = true;
            appender.LockingModel = new FileAppender.MinimalLock();
            appender.DatePattern = @"yyyy/yyyy年MM月/yyyy年MM月dd日'.txt'";
            appender.CountDirection = 1;

            PatternLayout layout = new PatternLayout();
            layout.Header = "<==================================================================================================>";
            layout.ConversionPattern = "%n------------------------------------------------------------------------------------------------------" +
                                       "%n记录时间:[%date{yyyy年MM月dd日 HH时mm分ss秒ffff}] 线程ID:[%thread] 日志级别:[%level] %message%newline";
            layout.Footer = "<==================================================================================================>";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            BasicConfigurator.Configure(appender);
        }

        /// <summary>
        /// 写入日志，默认路径为-->D:/LogFile/
        /// </summary>
        /// <param name="message">自定义日志信息</param>
        /// <param name="logType">日志类型</param>
        /// <param name="ex">异常</param>
        /// <param name="name">函数名[使用默认值即可]</param>
        /// <param name="path">文件路径[使用默认值即可]</param>
        /// <param name="line">行数[使用默认值即可]</param>
        public static void WriteLog(string message,
                                    LogType logType = LogType.Info, 
                                    Exception ex = null,
                                    [CallerMemberName] string name = default,
                                    [CallerFilePath] string path = default,
                                    [CallerLineNumber] int line = default)
        {
            message = string.Format("\n文件:{0}   [{1}]{2}", path, line, name) + "\n日志描述:" + message;
            switch (logType)
            {
                case LogType.Info:
                    LogerHelper.Logger.Info(message, ex);
                    break;
                case LogType.Warn:
                    LogerHelper.Logger.Warn(message, ex);
                    break;
                case LogType.Fatal:
                    LogerHelper.Logger.Fatal(message, ex);
                    break;
                case LogType.Error:
                    LogerHelper.Logger.Error(message, ex);
                    break;
                case LogType.Debug:
                    LogerHelper.Logger.Debug(message, ex);
                    break;
                default:
                    break;
            }
        }
    }

    public enum LogType
    {
        Info = 0,
        Warn = 1,
        Fatal = 2,
        Error = 3,
        Debug = 4,
    }
}
