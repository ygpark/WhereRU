using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sign
{
    class Program
    {
        private static string magic = "TAG:";
        private static int MAXBUFFSIZE = 1024;

        public static string Magic { get => magic; set => magic = value; }

        static void Main(string[] args)
        {
            if(args.Length == 0) {
                string help = "사용법: ";
                help += "   (읽기) sign.exe -r <파일명>";
                help += "   (쓰기) sign.exe -w <파일명> <태그>";
                Console.WriteLine(help);
                return;
            }

            if (args.Length == 2 && args[0] == "-r")
            {
                string fname = args[1];
                string tag = ReadTag(fname);
                Console.Write(tag);
            }
            else if(args.Length == 3 && args[0] == "-w")
            {
                string fname = args[1];
                string tag = args[2];
                WriteTag(fname, tag);
            }
        }
        static void WriteTag(string filename, string tag)
        {
            using (var fileStream = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.None))
            using (var bw = new BinaryWriter(fileStream))
            {
                byte[] data = new byte[MAXBUFFSIZE];
                byte[] aa = System.Text.Encoding.UTF8.GetBytes(Magic + tag);
                if (aa.Length > data.Length)
                    throw new Exception("태그 사이즈가 너무 깁니다. Magic + tag 사이즈는 " + MAXBUFFSIZE + " 이내여야 합니다.");

                aa.CopyTo(data, 0);

                bw.Write(data);
            }
        }

        static string ReadTag(string filename)
        {
            string tag = ""; //TAG:문자열
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

            return tag.Replace(Magic, "");
        }
    }
}
