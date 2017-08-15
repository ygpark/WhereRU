using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouchEnNxKey
{
    /// <summary>
    /// 파일 끝부분에 데이터를 쓰고 읽는 클래스. 누구에게 파일을 보냈는지 확인하는 용도로 사용
    /// 파일 끝부분에 MAXBUFFSIZE byte 만큼의 데이터를 사용한다.
    /// </summary>
    class FileTag
    {
        private static int MAXBUFFSIZE = 1024;
        private static string magic = "TAG:";
        private static string Magic { get => magic; set => magic = value; }

        public static void Write(string tag)
        {
            string myFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            FileTag.Write(myFullPath, tag);
        }

        public static void Write(string filename, string tag)
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

        public static string Read()
        {
            string myFullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            return FileTag.Read(myFullPath);
        }

        public static string Read(string filename)
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
                    return "test";
            }
            catch (Exception)
            {
                return "test";
            }

            return tag.Replace(Magic, "");
        }
    }
}
