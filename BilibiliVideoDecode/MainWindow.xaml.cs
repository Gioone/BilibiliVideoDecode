using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using HD = HandyControl.Controls;

namespace BilibiliVideoDecode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private VideoProcess _video;

        private bool _isVideoSaved = true;

        public MainWindow()
        {
            /*
            void handler(object s, RoutedEventArgs e)
            {
                Loaded -= handler;
                NativeMethods.EnableBlur(this);
            }

            Loaded += handler;
            */


            InitializeComponent();

            

        }

        private async void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "视频文件(*.mov, *.mp4, *.flv, *.avi, *.mpg, *.asf, *.wmv, *.3gp, *.webm) | *.mov; *.mp4; *.flv, *.avi; *.mpg; *.asf; *.wmv; *.3gp; *.webm"
            };
            if (!(ofd.ShowDialog() ?? false)) { return; }
            if (string.IsNullOrEmpty(ofd.FileName))
            {
                return;
            }



            TextFilePath.Text = ofd.FileName;

            long mil = 0;

            Stopwatch sw = Stopwatch.StartNew();
            System.Windows.Window progressView = new ProgressView(ActualWidth, "请稍等，我们正在打开该视频文件...");
            progressView.Height = 60;

            progressView.Show();
            _video = new VideoProcess(ofd.FileName);
            sw.Stop();
            mil = sw.ElapsedMilliseconds;

            if (mil < 500)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500 - mil));
            }
            progressView.Close();
            
        }

        private async void BtnDecode_Click(object sender, RoutedEventArgs e)
        {
            if (!_isVideoSaved) return;

            _isVideoSaved = false;
            if (string.IsNullOrEmpty(TextFilePath.Text))
            {
                HD.MessageBox.Show("您需要首先打开一个要解密的视频!", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                _isVideoSaved = true;
                return;
            }

            if (_video is null)
            {
                _isVideoSaved = true;
                return;
            }

            int iFileName = _video.FilePath.LastIndexOf('\\');
            string strFileName = _video.FilePath.Substring(iFileName + 1);
            int iVideoExtensionsIndex = _video.FilePath.LastIndexOf('.');
            string strFileExtensions = _video.FilePath.Substring(iVideoExtensionsIndex + 1);

            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "选择要保存的路径",
                FileName = strFileName,
                Filter = $"视频文件 (*.{strFileExtensions}) | *.{strFileExtensions}"
            };

            if (!(sfd.ShowDialog() ?? false))
            {
                _isVideoSaved = true;
                return;
            }

            long mil = 0;

            Stopwatch sw = Stopwatch.StartNew();
            ProgressView progressView = new ProgressView(ActualWidth, title: $"请稍等，我们正在保存视频到指定的目录: {sfd.FileName}");
            progressView.Height = 60;
            progressView.Show();
            
            bool flag = await _video.DecodeVideoAsync(sfd.FileName);
            sw.Stop();
            mil = sw.ElapsedMilliseconds;

            if (mil < 500)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500 - mil));
            }

            progressView.Close();

            // We don't judge file is exists or not. Because DecodeVideoAsync method will process it.
            if (flag)
            {
                if (HD.MessageBox.Show($"文件保存成功, 文件保存在 {sfd.FileName}，打开吗?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Process process = new Process();
                    ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
                    psi.Arguments = sfd.FileName;
                    process.StartInfo = psi;

                    try
                    {
                        process.Start();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        process?.Close();

                    }
                }
            }
            _isVideoSaved = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            DependencyObject dp = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this); i++)
            {
                dp = VisualTreeHelper.GetChild(this, i);

                if (dp is Grid)
                {
                    break;
                }
            }
            */


            /*
            IEnumerable enumerable = LogicalTreeHelper.GetChildren(this);
            foreach (var item in enumerable)
            {

            }
            */

            // ElementBlur.SetIsEnabled(this, true);
        }
    }
}
