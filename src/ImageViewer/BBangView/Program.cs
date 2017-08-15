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
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //정보 가져오기
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form1();
            if (Consistent.isInTemp() == true)
            {
                form.WindowState = FormWindowState.Minimized;
                form.ShowInTaskbar = false;
            }
            Application.Run(form);
        }
    }
}
