using RedRain;
using RESUPUB.EngineObjects;
using RESUPUB.Off;
using RESUPUB.Static;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RESUPUB
{
    public static class Manager
    {
        private static bool _isAttached;

        /// <summary>
        ///     Gets the memory instance of the process Orion is currently attached to.
        /// </summary>
        public static NativeMemory Memory { get; private set; }

        /// <summary>
        ///     Gets the local player.
        /// </summary>
        public static Local Me => Objects.LocalPlayer;

        /// <summary>
        ///     Gets the current object manager.
        /// </summary>
        public static ObjManager Objects { get; private set; }

        /// <summary>
        ///     Gets the current instance of the game client.
        /// </summary>
        public static Game gClient { get; private set; }

        public static IntPtr ClientBase { get; private set; }
        public static IntPtr EngineBase { get; private set; }
        public static IntPtr RadarPointer { get; private set; }
        public static IntPtr EnginePointer { get; private set; }
        public static IntPtr GlowPointer { get; set; }
        public static IntPtr ResourcePointer { get; set; }

        public static int GlowObjCnt { get; private set; }

        /// <summary>
        ///     Initializes Orion by attaching to the specified CSGO process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="isInjected">if set to <c>true</c> [is injected].</param>
        public static void Attach(Process process, bool isInjected = false)
        {
            if (_isAttached)
                return;

            // We won't require the injector for now - we're completely passive.
            if (isInjected)
                Memory = new LocalProcessMemory(process);
            else
            {
                Memory = new ExternalProcessMemory(process);
            }

            ClientBase = Memory.GetModule("client.dll").BaseAddress;
            EngineBase = Memory.GetModule("engine.dll").BaseAddress;

            Objects = new ObjManager(ClientBase + (int)oBase.EntityList, 128);

            EnginePointer = Memory.Read<IntPtr>(EngineBase + (int)oBase.EnginePtr);

            GlowPointer = Memory.Read<IntPtr>(ClientBase + (int)oBase.GlowObject);
            if (EnginePointer == IntPtr.Zero)
                throw new Exception("Couldn't find Engine Ptr - are you sure your offsets are up to date?");

            Console.WriteLine("GlowPointer is: " + GlowPointer.ToString());
            gClient = new Game(EnginePointer);

            IntPtr myint = Memory.Read<IntPtr>(ClientBase + (int)oBase.RadarBase);
            RadarPointer = Memory.Read<IntPtr>(myint + 0x50);
            ResourcePointer = Memory.Read<IntPtr>(ClientBase + (int)oBase.PlayerResource);
            _isAttached = true;
        }
    }


}
