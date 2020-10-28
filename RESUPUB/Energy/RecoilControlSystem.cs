using RESUPUB.EngineObjects;
using RESUPUB.Helpers;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Energy
{
    class RecoilControlSystem
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        public Vector3 oldPunch;
        public Vector3 curPunch;
        public void Run(EngineObjects.Local me)
        {
            if (me.ShotsFired < 1)
            {
                oldPunch = Vector3.Zero;
                curPunch = Vector3.Zero;
                return;
            }

            if (!Canvas.myConfig.useRCSPistol)
                if (me.WeaponID == WeaponType.Pistol)
                    return;
            if (!Canvas.myConfig.useRCSSniper)
                if (me.WeaponID == WeaponType.Sniper)
                    return;
            if (me.PunchAngle == Vector3.Zero)
                return;
            if (me.PunchAngle == oldPunch)
                return;

            //oldAngle = me.ViewAngle;

            if (Convert.ToBoolean(GetAsyncKeyState(Canvas.myConfig.AimKey) & 0x8000))
            {
                curPunch = me.PunchAngle - oldPunch;
                if (curPunch != Vector3.Zero)
                {
                    Vector3 newViewAngles = me.ViewAngle - curPunch * (float)Math.Round(Canvas.myConfig.RCSForce, 1);
                    newViewAngles = TheMaths.ClampAngle(newViewAngles);
                    newViewAngles.Z = 0f;
                    Manager.Objects.WriteAngles(newViewAngles);
                }
                oldPunch = me.PunchAngle;
            }
            else
            {
                oldPunch = Vector3.Zero;
                curPunch = Vector3.Zero;
            }
            //return new Vector3(0, 0, 0);
        }

    }
}
