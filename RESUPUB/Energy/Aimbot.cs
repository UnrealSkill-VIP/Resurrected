using RESUPUB.EngineObjects;
using RESUPUB.Helpers;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Energy
{
    class Aimbot
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        Stopwatch AimTime = new Stopwatch();
        Stopwatch PauseTime = new Stopwatch();

        public Entity theTarget;

        public bool isRunning = false;

        public async void Run()
        {
            if (!Canvas.myConfig.bAimbot)
                return;
            if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
            {
                if (!Convert.ToBoolean(GetAsyncKeyState(Canvas.myConfig.AimKey) & 0x8000))
                    return;
                var me = Manager.Me;
                if (me.WeaponID == WeaponType.Melee || me.WeaponID == WeaponType.Grenade || me.WeaponID == WeaponType.Unknown)
                    return;
                if (me.ViewAngle == Vector3.Zero)
                    return;
                if (Canvas.myConfig.bAimTime)
                {
                    if (!AimTime.IsRunning && !PauseTime.IsRunning)
                        AimTime.Start();
                    if (PauseTime.IsRunning && !AimTime.IsRunning)
                        if (PauseTime.ElapsedMilliseconds <= Canvas.myConfig.fAimPauseTime)
                            return;
                        else
                        {
                            PauseTime.Stop();
                            PauseTime.Reset();
                            AimTime.Start();
                        }
                }
                if (Canvas.myConfig.bDelay)
                    await PutTaskDelay();
                theTarget = GetBestTargetByFov(me);
                    //GetBestTarget(me);
                if (theTarget == null)
                    return;
                if (theTarget.Head == Vector3.Zero)
                    return;
                Vector3 newViewAngles = TheMaths.CalcAngle(me.ViewOffset + me.Position, new Vector3(theTarget.Head.X, theTarget.Head.Y, theTarget.Head.Z + 1.5f));
                if (Canvas.myConfig.useRCS)
                {
                    if (me.WeaponID != WeaponType.Sniper && me.WeaponID != WeaponType.Pistol)
                        newViewAngles = newViewAngles - me.PunchAngle * (float)Math.Round(Canvas.myConfig.RCSForce, 1);
                }
                if (Canvas.myConfig.smooth != 0)
                {
                    Vector3 smooth = me.ViewAngle - newViewAngles;
                    smooth = TheMaths.NormalizeAngles(smooth);
                    newViewAngles = me.ViewAngle - smooth / Canvas.myConfig.smooth;
                    //smooth *= myConfig.smooth / 100f;
                    //if (Math.Abs(smooth.Y) < 45)
                    //    newViewAngles = me.ViewAngle + smooth;
                }

                newViewAngles.Z = 0.0f;
                newViewAngles = TheMaths.ClampAngle(newViewAngles);
                me.WriteAngles(newViewAngles);

                if (Canvas.myConfig.bAimTime)
                {
                    if (AimTime.ElapsedMilliseconds >= Canvas.myConfig.fAimTime)
                    {
                        AimTime.Stop();
                        AimTime.Reset();
                        PauseTime.Start();
                    }
                }
            }
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(Canvas.myConfig.tsDelay);
        }


        public Entity DropTarget()
        {
            return null;
        }

        public Entity GetBestTargetByFov(Local me)
        {
            try
            {
                Dictionary<Entity, float> maybe = new Dictionary<Entity, float>();
                IReadOnlyList<Entity> copyList = Manager.Objects.Players.ToList();
                foreach (var ent in copyList)
                {
                    if (ent.Id < 64)
                        if (ent.ClsId == 35)
                            if (ent.IsAlive)
                                if (!ent.IsFriendly)
                                    if (Canvas.myConfig.bAimSpottedOnly)
                                    {
                                        if (ent.isSpotted != 0)
                                            if (ent.IsValid)
                                            {
                                                Vector3 newDawn;
                                                if (TheMaths.WorldToScreen(me.fViewMatrix, new Vector3(ent.Head.X, ent.Head.Y, ent.Head.Z + 1.5f), out newDawn, Canvas.a_myWidth, Canvas.a_myHeight, Canvas.a_MyLeft, Canvas.a_MyTop))
                                                {
                                                    Vector2 heads = new Vector2(newDawn.X, newDawn.Y);
                                                    float dist = Math.Abs((heads - Canvas.ScreenMid).Length());
                                                    if (dist < Canvas.myConfig.fov)
                                                    {
                                                        maybe.Add(ent, dist);
                                                    }
                                                }
                                            }
                                    }
                                    else
                                    {
                                        if (ent.IsValid)
                                        {
                                            Vector3 newDawn;
                                            if (TheMaths.WorldToScreen(me.fViewMatrix, new Vector3(ent.Head.X, ent.Head.Y, ent.Head.Z + 1.5f), out newDawn, Canvas.a_myWidth, Canvas.a_myHeight, Canvas.a_MyLeft, Canvas.a_MyTop))
                                            {
                                                
                                                Vector2 heads = new Vector2(newDawn.X, newDawn.Y);
                                                float dist = Math.Abs((heads - Canvas.ScreenMid).Length());
                                                if (dist < Canvas.myConfig.fov)
                                                {
                                                    maybe.Add(ent, dist);
                                                }
                                            }
                                        }
                                    }


                }
                var min = maybe.OrderBy(kvp => kvp.Value).First();
                return min.Key;
            }
            catch
            {
                return null;
            }
        }

        public Entity GetBestTarget(Local me)
        {
            try
            {
                IReadOnlyList<Entity> copyList = Manager.Objects.Players.ToList();
                foreach (var ent in copyList)
                {
                    if (ent.Id < 64)
                        if (ent.ClsId == 35)
                            if (ent.IsAlive)
                                if (!ent.IsFriendly)
                                    if (Canvas.myConfig.bAimSpottedOnly)
                                    {
                                        if (ent.isSpotted != 0)
                                            if (ent.IsValid)
                                            {
                                                Vector3 newDawn;
                                                if (TheMaths.WorldToScreen(me.fViewMatrix, new Vector3(ent.Head.X, ent.Head.Y, ent.Head.Z + 1.5f), out newDawn, Canvas.a_myWidth, Canvas.a_myHeight, Canvas.a_MyLeft, Canvas.a_MyTop))
                                                {
                                                    Vector2 heads = new Vector2(newDawn.X, newDawn.Y);
                                                    float dist = Math.Abs((heads - Canvas.ScreenMid).Length());
                                                    if (dist < Canvas.myConfig.fov)
                                                    {
                                                        return ent;
                                                    }
                                                }
                                            }
                                    }
                                    else
                                    {
                                        if (ent.IsValid)
                                        {
                                            Vector3 newDawn;
                                            if (TheMaths.WorldToScreen(me.fViewMatrix, new Vector3(ent.Head.X, ent.Head.Y, ent.Head.Z + 1.5f), out newDawn, Canvas.a_myWidth, Canvas.a_myHeight, Canvas.a_MyLeft, Canvas.a_MyTop))
                                            {
                                                Vector2 heads = new Vector2(newDawn.X, newDawn.Y);
                                                float dist = Math.Abs((heads - Canvas.ScreenMid).Length());
                                                if (dist < Canvas.myConfig.fov)
                                                {
                                                    return ent;
                                                }
                                            }
                                        }
                                    }


                }
                return null;
            } catch
            {
                return null;
            }

        }

    }
}
