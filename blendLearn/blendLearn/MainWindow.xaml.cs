using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            this.DataContext = this;
        }

        private void fan_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(fan,"run", false);
        }

        private void fan_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(fan, "stop", false);
        }
        #region 动画测试1

        private bool hasNew = false;
        private bool isRunning = false;

        private double _xSize;

        public double XSize
        {
            get { return _xSize; }
            set
            {
                _xSize = value;
                hasNew = true;
                //Run(bn_move, value + 0.02);
                Run2(bn_move, value);
            }
        }
        private double _rSize;

        public double RSize
        {
            get { return _rSize; }
            set
            {
                _rSize = value;
                hasNew = true;
                //Run(bn_move, value + 0.02);
                Run3(bn_move, value);
            }
        }

        DoubleAnimation x_move = new DoubleAnimation()
        {
            Duration = new TimeSpan(0, 0, 0, 0, 100),
        };
        DoubleAnimation r_move = new DoubleAnimation()
        {
            Duration = new TimeSpan(0, 0, 0, 0, 100),
        };
        RotateTransform rotate = new RotateTransform();
        
        private void Run(UIElement element, double to)
        {
            if (Math.Abs(to - XSize) > 0.01 && hasNew && !isRunning)
            {
                isRunning = true;
                lock (element)
                {
                    hasNew = false;
                    x_move.To = to;
                    element.BeginAnimation(Canvas.LeftProperty, x_move);

                    EventHandler eventHandler = null;
                    x_move.Completed += eventHandler = (s, e) =>
                    {
                        x_move.Completed -= eventHandler;
                        Run(bn_move, XSize);
                    };
                }
                isRunning = false;
            }
        }
        private void Run2(UIElement element, double to)
        {
            x_move.To = to;
            element.BeginAnimation(Canvas.LeftProperty, x_move);
        }
        private void Run3(UIElement element, double to)
        {
            r_move.To = to;
            rotate.BeginAnimation(RotateTransform.AngleProperty, r_move);
        }
        #endregion

        #region 动画测试2

        void test()
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation yd5 = new DoubleAnimation(0, 200, new Duration(TimeSpan.FromSeconds(3)));//浮点动画定义了开始值和起始值
            bn_move.RenderTransform = new TranslateTransform();//在二维x-y坐标系统内平移(移动)对象
            yd5.RepeatBehavior = RepeatBehavior.Forever;//设置循环播放
            yd5.AutoReverse = true;//设置可以进行反转
            Storyboard.SetTarget(yd5, bn_move);//绑定动画为这个按钮执行的浮点动画
            Storyboard.SetTargetProperty(yd5, new PropertyPath("RenderTransform.X"));//依赖的属性


            DoubleAnimation yd3 = new DoubleAnimation(0, 200, new Duration(TimeSpan.FromSeconds(3)));//浮点动画定义了开始值和起始值
            bn_move.RenderTransform = new TranslateTransform();//在二维x-y坐标系统内平移(移动)对象
            yd3.RepeatBehavior = RepeatBehavior.Forever;//设置循环播放
            yd3.AutoReverse = true;//设置可以进行反转
            Storyboard.SetTarget(yd3, bn_move);//绑定动画为这个按钮执行的浮点动画
            Storyboard.SetTargetProperty(yd3, new PropertyPath("RenderTransform.Y"));//依赖的属性
            sb.Children.Add(yd3);//向故事板中加入此浮点动画
            sb.Children.Add(yd5);//向故事板中加入此浮点动画
            sb.Begin();//播放此动画
        }

        #endregion

        #region 动画测试3

        void test2()
        {
            RotateTransform rotate = new RotateTransform();
            TranslateTransform translate = new TranslateTransform();

            TransformGroup group = new TransformGroup();
            group.Children.Add(rotate);
            group.Children.Add(translate);

            bn_move.RenderTransform = group;
            bn_move.RenderTransformOrigin = new Point(0, 0);
            EasingFunctionBase easingFunction = new PowerEase()
            {
                EasingMode = EasingMode.EaseInOut,
                Power = 2,
            };
            DoubleAnimation translateAnimation = new DoubleAnimation()
            {
                To = 100,
                EasingFunction = easingFunction,
                Duration = new TimeSpan(0, 0, 0, 1),
                //AutoReverse = true,
            };
            DoubleAnimation angleAnimation = new DoubleAnimation()
            {
                To = 90,
                EasingFunction = easingFunction,
                Duration = new TimeSpan(0, 0, 0, 1),
                //AutoReverse = true,
            };
            translate.BeginAnimation(TranslateTransform.XProperty, translateAnimation);
            rotate.BeginAnimation(RotateTransform.AngleProperty, angleAnimation);
        }

        #endregion
        private void bn_move_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //test();
            test2();
        }

        private void bn_move_Loaded(object sender, RoutedEventArgs e)
        {
            bn_move.RenderTransform = rotate;
            bn_move.RenderTransformOrigin = new Point(0.5, 0.5);
        }
    }
}
