using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Helpers
{
    static class Generators
    {
        public static Random random = new Random();

        public static string GetRandomString(int length = 32, string palette = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
        {
            char[] chars = new char[length];
            random = new Random(random.Next(int.MinValue, int.MaxValue));
            for (int i = 0; i < length; i++)
                chars[i] = palette[random.Next(0, palette.Length)];
            return new string(chars);
        }

        public static void Log(string s, string file)
        {
            using (StreamWriter sw = new StreamWriter(file, true))
            {
                sw.WriteLine("[---" + DateTime.Now + "---]" + Environment.NewLine + s + Environment.NewLine + "[-------------------------]" + Environment.NewLine);
                sw.Close();
            }
        }
    }
}
