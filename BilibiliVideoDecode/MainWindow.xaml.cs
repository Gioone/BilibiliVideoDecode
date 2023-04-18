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
        // private VideoProcess _video;

        private string _openFilePath;

        private bool _isVideoSaved = true;

        public MainWindow()
        {
            
            void handler(object s, RoutedEventArgs e)
            {
                Loaded -= handler;
                NativeMethods.EnableBlur(this);
            }

            Loaded += handler;
            


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
           
            sw.Stop();
            mil = sw.ElapsedMilliseconds;

            if (mil < 500)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500 - mil));
            }
            progressView.Close();

            _openFilePath = ofd.FileName;
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

            if (_openFilePath is null)
            {
                _isVideoSaved = true;
                return;
            }

            int iFileName = _openFilePath.LastIndexOf('\\');
            string strFileName = _openFilePath.Substring(iFileName + 1);
            int iVideoExtensionsIndex = _openFilePath.LastIndexOf('.');
            string strFileExtensions = _openFilePath.Substring(iVideoExtensionsIndex + 1);

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

            string strSavedFilePath = sfd.FileName;

            if (File.Exists(strSavedFilePath))
            {
                // If file is existed (example: xxx.mp4), we rename it as like "xxx-1.mp4"
                int iExtensionIndex = strSavedFilePath.LastIndexOf('.');
                string strHeadFileName = strSavedFilePath.Substring(0, iExtensionIndex);
                strHeadFileName += "-1";
                string strTailFileName = strSavedFilePath.Substring(iExtensionIndex + 1);

                // We get a string like "c:\123.mp4".
                strSavedFilePath = strHeadFileName + "." + strTailFileName;
            }

            bool flag = await SaveVideo(strSavedFilePath);
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

        private async Task<bool> SaveVideo(string strSaveFileName)
        {
            using FileStream read = new FileStream(_openFilePath, FileMode.Open);

            using FileStream write = new FileStream(strSaveFileName, FileMode.Create);

            byte[] buffer = new byte[8192 * 10];

            read.Position = 3;
            int iReader = await read.ReadAsync(buffer, 0, buffer.Length);

            while (iReader != 0)
            {
                await write.WriteAsync(buffer, 0, iReader);
                iReader = await read.ReadAsync(buffer, 0, buffer.Length);
                write.Flush();
            }

            return true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
