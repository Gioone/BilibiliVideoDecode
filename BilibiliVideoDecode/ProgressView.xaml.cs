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

namespace BilibiliVideoDecode
{
    /// <summary>
    /// ProgressView.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressView : Window
    {


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ProgressView), new PropertyMetadata("Title"));


        public ProgressView(double width, string title)
        {
            // this.Width = width;
            Title = title;

            
            void handler(object s, RoutedEventArgs e)
            {
                Loaded -= handler;
                NativeMethods.EnableBlur(this, NativeMethods.AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT);
            }

            Loaded += handler;
            
            InitializeComponent();
        }
    }
}
