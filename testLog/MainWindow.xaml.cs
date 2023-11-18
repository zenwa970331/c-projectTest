using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using log4net;
using LogHelper;

namespace testLog
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        string strTime = string.Empty;


        [Obsolete]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            stopwatch.Reset();
            stopwatch.Start();
            string strLog = String.Format("[Thread ID:{0}]{1}", AppDomain.GetCurrentThreadId(), " 耗时：" + strTime + "位置:" + AuxiliaryMessage());
            //for (int i = 0; i < 10000; i++)
            //{
            //    Log4netHelper.WriteLog(i.ToString(),LogType.Info,new Exception("kkk",new Exception("ggg")));
            //}
            Parallel.For(0, 10000, new ParallelOptions() { MaxDegreeOfParallelism = 2 },
            (int i) =>
            {
                Log4netHelper.WriteLog(i.ToString(), LogType.Info, new Exception("kkk", new Exception("ggg")));
            });
            stopwatch.Stop();
            strTime = stopwatch.ElapsedTicks.ToString();
            MessageBox.Show(strTime);
        }
        private void WriteLog1(string exp = null, string loggerName = "InfoLogger")
        {
            //新建的一个静态类
            LogObject.Log(loggerName).Error(exp);
        }

        static public string AuxiliaryMessage([CallerMemberName] string name = default,
                                              [CallerFilePath] string path = default,
                                              [CallerLineNumber] int line = default)
        {
            return string.Format("函数:{0},  文件{1},  第{2}行", name, path, line);
        }
    }
    public static class LogObject
    {

        public static ILog Log(string LoggerName)
        {
            //log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            return LogManager.GetLogger(LoggerName);
        }
    }
}
