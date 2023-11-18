using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace blendLearn
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //x_move.Completed
        }

        private void fan_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(fan,"run", false);
        }

        private void fan_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(fan, "stop", false);
        }



        DoubleAnimation x_move = new DoubleAnimation()
        {
            Duration = new TimeSpan(0, 0, 0, 0, 50),
        };
        private void Run(UIElement element, double to)
        {
            lock (element)
            {
                x_move.To = to;
                element.BeginAnimation(Canvas.LeftProperty, x_move);
            }
        }
    }
}
