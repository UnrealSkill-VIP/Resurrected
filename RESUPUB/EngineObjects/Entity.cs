using RESUPUB.EngineObjects.Mem;
using RESUPUB.Off;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Numerics;
using System.Collections;
using RESUPUB.Helpers;

namespace RESUPUB.EngineObjects
{
    public class Entity : Object
    {
        internal Entity(IntPtr baseAddress) : base(baseAddress)
        { }
        public int Id => ReadField<int>(oStatic.Index);
        public int ClsId => ReadClassId();
        public SharpDX.Vector3 Position => ReadField<SharpDX.Vector3>(oStatic.Position);
        public string Name => (IsAlive) ? ReadName((int)oStatic.RadarBaseName, this.Id, this.IsAlive).ToString() : "";
        public int GlowIndex => (IsAlive) ? ReadField<int>(oStatic.GlowIndex) : 0;
        public string ModelName => (IsAlive) ? ReadModelName((int)this.BaseAddress, this.IsAlive) : "";
        public int Health => (IsAlive) ? ReadField<int>(oStatic.Health) : 0;
        public int Armor => (IsAlive) ? ReadField<int>(oStatic.Armor) : 0;
        public WeaponId eWeaponIndex => (WeaponId)ReadWeaponId();
        public int Flags => ReadField<int>(oStatic.Flags);
        public bool IsDormant => ReadField<int>(oStatic.Dormant) == 1;
        public bool IsAlive => ReadField<byte>(oStatic.LifeState) == 0;
        public bool IsFriendly => Team == Manager.Me.Team;
        public PlayerTeam Team => (PlayerTeam)ReadField<int>(oStatic.Team);
        public float Distance => SharpDX.Vector3.Distance(Manager.Me.Position, Position);
        public int isSpotted => (IsAlive) ? ReadField<int>((int)oStatic.SpottedMask) : 0;
        public SharpDX.Vector3 Head => (IsAlive) ? GetBoneVec(this.BaseAddress, 6, this.IsAlive) : new SharpDX.Vector3(0, 0, 0);
        public int MatchmakingWins => iRankWins(this.Id);
        public int MatchmakingRank => iRanking(this.Id);
        public int i_Kills => iKills(this.Id);
    }
}
