using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchEnNxKey
{
    class TimeStamp
    {
        public static String GetTimestamp()
        {
            return DateTime.Now.ToString("yyyyMMdd_HHmmss");
        }
    }
}
