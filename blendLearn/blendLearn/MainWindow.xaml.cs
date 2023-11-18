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
        }

        private void fan_KeyDown(object sender, KeyEventArgs e)
        {
            VisualStateManager.GoToState(fan, "run", false);
        }
        bool a = false;
        private void fan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (a == false)
            {
                _ = VisualStateManager.GoToState(fan, "run", false);
                a = true;
            }
            else
            {
                _ = VisualStateManager.GoToState(fan, "stop", false);
                a = false;
            }
            
        }
    }
}
