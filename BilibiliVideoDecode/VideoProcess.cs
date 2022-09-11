using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilibiliVideoDecode
{
    public class VideoProcess
    {
        public string FilePath { get; set; }
        public byte[] VideoBytes { get; set; }

        public VideoProcess(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                VideoBytes = new byte[fs.Length];
                FilePath = filePath;
                fs.Read(VideoBytes, 0, (int)fs.Length);
            }

            /*
            if (VideoBytes.Length >= 3)
            {
                if (VideoBytes[0] == 0xFF)
                {
                    VideoBytes[0] = 0;
                }
                if (VideoBytes[1] == 0xFF)
                {
                    VideoBytes[1] = 0;
                }
                if (VideoBytes[2] == 0xFF)
                {
                    VideoBytes[2] = 0;
                }
            }
            */
        }

        /*
        public VideoProcess(byte[] videoBytes)
        {
            if (videoBytes.Length >= 3)
            {
                if (videoBytes[0] == 0xFF)
                {
                    videoBytes[0] = 0;
                }
                if (videoBytes[1] == 0xFF)
                {
                    videoBytes[1] = 0;
                }
                if (videoBytes[2] == 0xFF)
                {
                    videoBytes[2] = 0;
                }
            }
            VideoBytes = videoBytes;
        }
        */

        public async Task<bool> DecodeVideoAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    // If file is existed (example: xxx.mp4), we rename it as like "xxx-1.mp4"
                    int iExtensionIndex = filePath.LastIndexOf('.');
                    string strHeadFileName = filePath.Substring(0, iExtensionIndex);
                    strHeadFileName += "-1";
                    string strTailFileName = filePath.Substring(iExtensionIndex + 1);

                    // We get a string like "c:\123.mp4".
                    filePath = strHeadFileName + "." + strTailFileName;
                }

                using FileStream fs = new FileStream(filePath, FileMode.Create);

                // This is a Bilibili encryption video.
                if (VideoBytes.Length >= 3 && VideoBytes[0] == 0xFF && VideoBytes[1] == 0xFF && VideoBytes[2] == 0xFF)
                {
                    await fs.WriteAsync(VideoBytes, 3, VideoBytes.Length - 3);
                }
                // A normal video, we don't do anything, just save it.
                // In normal case, code doesn't execute here. But we should avoid exception.
                else
                {
                    await fs.WriteAsync(VideoBytes, 0, VideoBytes.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
