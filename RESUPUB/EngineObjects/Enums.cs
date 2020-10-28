using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.EngineObjects
{
        public enum BoneToString
        {
            Head = 0,
            LClavicle = 1,
            RClavicle = 2,
            LFoot = 3,
            RFoot = 4
        }
        public enum PlayerTeam
        {
            Neutral = 1,
            Terrorist = 2,
            CounterTerrorist = 3
        }

        public enum SignonState
        {
            None = 0, // Menu
            Challenge = 1,
            Connected = 2, // Server welcome message?
            New = 3, // nfi when this is used
            PreSpawn = 4, // Selecting team
            Spawn = 5, // Spawn protected
            Full = 6, // Can move, shoot, etc.
            ChangingLevel = 7
        }

        public enum WeaponType
        {
            Pistol, MachinePistol, AssaultRifle, ZoomRifle, MachineGun, Sniper, AutoSniper, Shotgun, Grenade, Melee, Special, Unknown
        }

        public enum PlayerState
        {
            Jump = 0, Stand = 1, Crouch = 7
        }

        public enum WeaponId
        {
            DEAGLE = 1,
            ELITE = 2,
            FIVESEVEN = 3,
            GLOCK = 4,
            AK47 = 7,
            AUG = 8,
            AWP = 9,
            FAMAS = 10,
            G3SG1 = 11,
            GALILAR = 13,
            M249 = 14,
            M4A1 = 16,
            MAC10 = 17,
            P90 = 19,
            UMP45 = 24,
            XM1014 = 25,
            BIZON = 26,
            MAG7 = 27,
            NEGEV = 28,
            SAWEDOFF = 29,
            TEC9 = 30,
            TASER = 31,
            HKP2000 = 32,
            MP7 = 33,
            MP9 = 34,
            NOVA = 35,
            P250 = 36,
            SCAR20 = 38,
            SG556 = 39,
            SSG08 = 40,
            KNIFE = 42,
            FLASHBANG = 43,
            HEGRENADE = 44,
            SMOKEGRENADE = 45,
            MOLOTOV = 46,
            DECOY = 47,
            INCGRENADE = 48,
            C4 = 49,
            KNIFE_T = 59,
            M4A1_SILENCER = 60,
            USP_SILENCER = 61,
            CZ75A = 63,
            REVOLVER = 64,
            KNIFE_BAYONET = 500,
            KNIFE_FLIP = 505,
            KNIFE_GUT = 506,
            KNIFE_KARAMBIT = 507,
            KNIFE_M9_BAYONET = 508,
            KNIFE_TACTICAL = 509,
            KNIFE_FALCHION = 512,
            KNIFE_BUTTERFLY = 515,
            WEAPON_KNIFE_PUSH = 516
        }
}
