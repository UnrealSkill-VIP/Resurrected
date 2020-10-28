using System;
using RESUPUB.Off;
using RESUPUB.Helpers;

namespace RESUPUB.EngineObjects
{
    public class Local : Entity
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalPlayer" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        internal Local(IntPtr baseAddress) : base(baseAddress)
        {
        }

        /// <summary>
        ///     Gets the view matrix of the local player.
        /// </summary>
        /// <value>
        ///     The view matrix.
        /// </value>
        public int WeaponIndex => ReadField<int>(0, false) - 64 - 1;

        public Matrix4x4 fViewMatrix => ReadMatrix(oBase.ViewMatrix);
        
        /// <summary>
        ///     Gets the player ID for the player currently under the player's crosshair, and 0 if none.
        /// </summary>
        public int CrosshairId => ReadField<int>(oStatic.CrosshairId);

        /// <summary>
        ///     Gets the target the local player is currently aiming at, or null if none.
        /// </summary>
        /// 
        public WeaponType WeaponID => GetWeaponType(ReadField<int>(Manager.Me.BaseAddress, this.Id));

        public SharpDX.Vector3 ViewAngle => ReadAngle((int)oStatic.ViewAngleX);

        public SharpDX.Vector3 ViewOffset => ReadField<SharpDX.Vector3>((int)oStatic.LocalViewOffset);

        public SharpDX.Vector3 PunchAngle => ReadField<SharpDX.Vector3>((int)oStatic.Punch);

        public SharpDX.Vector3 VelocityVec => ReadField<SharpDX.Vector3>((0x110));

        public int OnGround => ReadField<int>((int)oBase.ForceJump);

        public int ShotsFired => ReadField<int>((int)oStatic.IShotsFired);

        //public int myFlag => ReadField<int>((int)oStatic.Flags);

        public Entity Target
        {
            get
            {
                // Store this in a local variable - the crosshair ID will get updated *very* frequently, 
                // to the point where we can't be sure that by the time we make a call to FirstOrDefault, it'll
                // still be "valid" according to the check before it. (Value can change on a per-frame basis)
                // This way, at least we'll be sure that for the execution of this function, we maintain the same value.
                var id = CrosshairId;

                if (CrosshairId <= 0)
                    return null;

                return Manager.Objects.GetPlayerById(id);
            }
        }
    }
}
