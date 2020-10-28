using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RESUPUB
{
    public class cConfig
    {
        /* AIMBOT */
        public bool bAimbot
        {
            get; set;
        }

        public bool bAimSpottedOnly
        {
            get; set;
        }

        public float fov
        {
            get; set;
        }
        public float smooth
        {
            get; set;
        }
        public int AimKey
        {
            get; set;
        }

        public bool bAimTime
        {
            get; set;
        }
        public float fAimTime
        {
            get; set;
        }
        public float fAimPauseTime
        {
            get; set;
        }

        public bool bDelay
        {
            get; set;
        }

        public TimeSpan tsDelay
        {
            get; set;
        }

        /* TRIGGERBOT */
        public bool bTrigger
        {
            get; set;
        }

        /* ESP */
        public bool bESP
        {
            get; set;
        }
        public bool drawAllies
        {
            get; set;
        }
        public bool drawName
        {
            get; set;
        }
        public bool drawWeapon
        {
            get; set;
        }
        public bool drawHP
        {
            get; set;
        }
        public bool drawKV
        {
            get; set;
        }

        public bool drawHead
        {
            get; set;
        }

        public bool drawDistance
        {
            get; set;
        }

        public bool drawMMWins
        {
            get; set;
        }

        public bool drawMMRank
        {
            get; set;
        }

        public bool bGlow
        {
            get; set;
        }

        //Misc
        public bool useRCS
        {
            get; set;
        }

        public bool useRCSPistol
        {
            get; set;
        }

        public bool useRCSSniper
        {
            get; set;
        }

        public float RCSForce
        {
            get; set;
        }

        public bool bBunnyhop
        {
            get; set;
        }

        public bool bDrawDebug
        {
            get; set;
        }

        public SharpDX.Color ColorTSpotted
        {
            get; set;
        }

        public SharpDX.Color ColorT
        {
            get; set;
        }

        public SharpDX.Color ColorCTSpotted
        {
            get; set;
        }

        public SharpDX.Color ColorCT
        {
            get; set;
        }

        public bool enableOutline { get; set; }
    }
}
