using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Static
{
    public static class sTimer
    {
        private static readonly Stopwatch _sw = Stopwatch.StartNew();

        public static TimeSpan GetTime()
        {
            return _sw.Elapsed;
        }
    }
}
