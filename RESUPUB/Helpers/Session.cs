using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Helpers
{
    public static class Session
    {
        public static bool b_IsStarted { get; private set; }
        public static int i_CurKills { get; private set; }
        public static string s_CurMap { get; private set; }
        public static DateTime dt_Started { get; private set; }

        public static void StartSession(string mapname)
        {
            if (!b_IsStarted)
            {
                b_IsStarted = true;
                s_CurMap = mapname;
                dt_Started = DateTime.Now;
            }
        }

        public static void StopSession()
        {
            if(b_IsStarted)
            {
                b_IsStarted = false;
                i_CurKills = 0;
            }
        }

        public static bool CheckSession() { return b_IsStarted; }

        public static void UpdateKills (int kills) { i_CurKills = kills; }
            
        public static int CheckKills() { return i_CurKills; }

        public static DateTime CheckStarted() { return dt_Started; }

        public static string CheckMap() { return s_CurMap; }

        public static CSession ConstructSessionObject()
        {
            CSession thisSession = new CSession();
            thisSession.i_Kills = CheckKills();
            thisSession.s_Map = CheckMap();
            thisSession.theTime = CheckStarted();
            thisSession.i_MatchID = Convert.ToInt32(Helpers.Generators.GetRandomString(8, "0123456789"));
            return thisSession;
        }
    }
}
