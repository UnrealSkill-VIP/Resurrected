using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Off
{
    public enum oStatic
    {
        // Entity
        Position = 0x134,
        Team = 0xF0,
        Armor = 0xA9E8,//0xA8F8,
        Health = 0xFC,
        Name = 0x150,
        Dormant = 0xE9,
        Index = 0x64,
        Flags = 0x100,
        LifeState = 0x25B,
        CrosshairId = 0xAA44,//A954,//0xA950,
        HasBomb = 0x161C,
        //LocalPlayer - ActiveWeapon
        LocalActiveWeapon = 0x2EE8,
        //BaseCombatWeapon - WeaponID
        WIP = 0x32E0,
        //Bones
        Bonez = 0x2698,

        //Radar Stuff
        RadarBasePointer = 0x50,
        RadarBaseName = 0x204,
        RadarSize = 0x1E0,

        // GameClient
        LocalPlayerIndex = 0x178,
        GameState = 0x100,

        //GlowIndex
        GlowIndex = 0xA310,

        // EntityList/ObjectManager
        EntitySize = 0x10,

        Spotted = 0x939,
        SpottedMask = 0x97C,

        //ViewAngle shit
        ViewAngleX = 0x4D0C,
        LocalViewOffset = 0x104,
        //Punchangle? 0x3024 or 0x3018
        Punch = 0x3018,

        ICompRank = 0x1A44,
        ICompWins = 0x1B48,

        IShotsFired = 0xA2B0,
        IAccount = 0xA8E8,
        //return Manager.Memory.Read<int>(preIndex + (int)oStatic.AttributeManager + (int)oStatic.Item + (int)oStatic.ItemDefIndex);
        AttributeManager = 0x2D70,
        Item = 0x2DB0,
        ItemDefIndex = 0x2F80,

        //Making dem esp boxes sexe
        vecMins = 0x320,
        vecMaxs = 0x32C,
        CoordFrame = 0x440
    }
}
