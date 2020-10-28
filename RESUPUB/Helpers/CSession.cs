using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Helpers
{
    public class CSession
    {
        // MatchID is random generated or maybe just 0 and increase by new added matches by one.
        public int i_MatchID { get; set; }
        // Display the Kills instead of headshots because we love to cheat. ;)
        public int i_Kills { get; set; }
        //Because collecting map info is awesome...
        public string s_Map { get; set; }
        // And we need a date so lets save this here too.
        public DateTime theTime { get; set; }
    }
}
