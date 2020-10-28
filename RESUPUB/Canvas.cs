using SharpDX;
using SharpDX.Direct2D1;
using System.Drawing.Drawing2D;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using SharpDX.DXGI;
using SharpDX.DirectWrite;
using FontFactory = SharpDX.DirectWrite.Factory;
using System.IO;
using Microsoft.Win32.SafeHandles;
using RESUPUB.Helpers;
using WindowScrape;
using WindowScrape.Types;
using System.Collections.Generic;
using Newtonsoft.Json;
using RESUPUB.Static;
using RESUPUB.EngineObjects;
using RESUPUB.Energy;

namespace RESUPUB
{
    public partial class Canvas : Form
    {

        #region "allocConsole"
        [DllImport("kernel32.dll",
    EntryPoint = "GetStdHandle",
    SetLastError = true,
    CharSet = CharSet.Auto,
    CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;
        #endregion
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        #region Variables
        private Margins marg;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        public bool canTrig;

        float[] ViewMatrix = new float[16];

        public static SharpDX.Color pColor;

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private static SharpDX.Direct2D1.Factory factory = new SharpDX.Direct2D1.Factory();
        private static FontFactory fontFactory = new FontFactory();

        public Thread trig;
        public Thread t3;

        public static Vector2 ScreenMid = new Vector2(0, 0);

        //this is used to specify the boundaries of the transparent area
        internal struct Margins
        {
            public int Left, Right, Top, Bottom;
        }

        #region "SetWindowLong"

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]

        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]

        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]

        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const int GWL_EXSTYLE = -20;

        public const int WS_EX_LAYERED = 0x80000;

        public const int WS_EX_TRANSPARENT = 0x20;

        public const int LWA_ALPHA = 0x2;

        public const int LWA_COLORKEY = 0x1;

        private WindowRenderTarget device;
        private HwndRenderTargetProperties renderProperties;
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        #endregion
        private int MyWidth = 0;
        private int MyHeight = 0;
        RECT my = new RECT();
        //private int weirdo, weirda, weirde, weirdi;

        [DllImport("dwmapi.dll")]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        public static cConfig myConfig = new cConfig();

        public Hashtable _players = new Hashtable();
        #endregion

        public static int CurrentMenu = 1;//1 = Main, 2 = ESP, 3 = Misc

        public static float stepHop = 0.3f;
        public static bool showMenu = true;

        public static int CurrentOption = 0;
        public static int CurrentY = 0;
        public static int menuTimer = 0;


        public static float a_myWidth, a_myHeight, a_MyLeft, a_MyTop;

        public bool cancelH = false;

        public HwndObject csgoWindow;

        public static string[] MMRanks = {
            "-",
            "Silver I",
            "Silver II",
            "Silver III",
            "Silver IV",
            "Silver Elite",
            "Silver Elite Master",

            "Gold Nova I",
            "Gold Nova II",
            "Gold Nova III",
            "Gold Nova Master",
            "Master Guardian I",
            "Master Guardian II",

            "Master Guardian Elite",
            "Distinguished Master Guardian",
            "Legendary Eagle",
            "Legendary Eagle Master",
            "Supreme Master First Class",
            "The Global Elite" };

        public HwndObject getCsgoWindow()
        {
            foreach (HwndObject item in HwndObject.GetWindows())
            {
                if (item.Text.Contains("Counter-Strike: Global Offensive") || item.Title.Contains("Counter-Strike: Global Offensive"))
                {
                    return item;
                }

            }
            return new HwndObject((IntPtr)null);
        }

        public Stopwatch performance = new Stopwatch();



        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }
        #region settings
        ///* AIMBOT */
        //public bool bTobmia = true; // IS the aimbot enabled?
        //public float fov = 5; //Size of the FOV in pixel
        //public float smooth = 100; //Smoothing dem Angles
        //public MouseButtons mousebtn = MouseButtons.Left;
        ///* TRIGGERBOT */
        //public bool bReggirt = false;

        ///* ESP */
        //public bool bPSE = true; // Draw ESP
        //public bool drawAllies = false; // Draw Allies, or not.
        //public bool drawName = true;
        //public bool drawHPKV = true;
        //public bool drawHead = true;
        #endregion
        private SharpDX.Direct2D1.Bitmap SDXBitmapFromSysBitmap(WindowRenderTarget device, System.Drawing.Bitmap bitmap)
        {
            var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapProperties = new BitmapProperties(new PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
            var size = new Size2(bitmap.Width, bitmap.Height);

            // Transform pixels from BGRA to RGBA
            int stride = bitmap.Width * sizeof(int);
            using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
            {
                // Lock System.Drawing.Bitmap
                var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                // Convert all pixels 
                for (int y = 0; y < bitmap.Height; y++)
                {
                    int offset = bitmapData.Stride * y;
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        // Not optimized 
                        byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        int rgba = R | (G << 8) | (B << 16) | (A << 24);
                        tempStream.Write(rgba);
                    }

                }
                bitmap.UnlockBits(bitmapData);
                tempStream.Position = 0;

                return new SharpDX.Direct2D1.Bitmap(device, size, tempStream, stride, bitmapProperties);
            }
        }
        public void makenewCFG()
        {
            if (File.Exists(Application.StartupPath + @"\config.cfg"))
            {
                File.Copy(Application.StartupPath + @"\config.cfg", Application.StartupPath + @"\config.cfg.bak");
                File.Delete(Application.StartupPath + @"\config.cfg");
            }
            myConfig.bAimbot = true; // IS the aimbot enabled?
            myConfig.bAimSpottedOnly = true;
            myConfig.bAimTime = false;
            myConfig.fAimTime = 200;
            myConfig.fAimPauseTime = 1000;
            myConfig.fov = 50; //Size of the FOV in pixel
            myConfig.smooth = 0; //Smoothing dem Angles
            myConfig.AimKey = 0x1;
            myConfig.bTrigger = false;
            myConfig.bESP = true; // Draw ESP
            myConfig.bGlow = true;
            myConfig.drawAllies = false; // Draw Allies, or not.
            myConfig.drawName = true;
            myConfig.drawHP = true;
            myConfig.drawKV = true;
            myConfig.drawHead = true;
            myConfig.drawDistance = true;
            myConfig.drawMMRank = true;
            myConfig.drawMMWins = true;
            myConfig.useRCS = true;
            myConfig.useRCSPistol = false;
            myConfig.useRCSSniper = false;
            myConfig.bBunnyhop = true;
            myConfig.RCSForce = 1.5f;
            myConfig.bDrawDebug = false;
            myConfig.drawWeapon = true;
            myConfig.bDelay = true;
            myConfig.tsDelay = new TimeSpan(5000);

            myConfig.ColorCT = SharpDX.Color.Blue;
            myConfig.ColorCTSpotted = SharpDX.Color.ForestGreen;

            myConfig.ColorT = SharpDX.Color.Red;
            myConfig.ColorTSpotted = SharpDX.Color.Yellow;

            File.WriteAllText(Application.StartupPath + @"\config.cfg", JsonConvert.SerializeObject(myConfig, Formatting.Indented));
            myConfig = JsonConvert.DeserializeObject<cConfig>(File.ReadAllText(Application.StartupPath + @"\config.cfg"));
        }

        #region "hotkeystuff"
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        const int WM_HOTKEY = 0x0312;

        private void CreateMenuKeys()
        {
            RegisterHotKey(this.Handle, 2, 0, (int)Keys.Up);
            RegisterHotKey(this.Handle, 3, 0, (int)Keys.Down);
            RegisterHotKey(this.Handle, 4, 0, (int)Keys.Right);
            RegisterHotKey(this.Handle, 5, 0, (int)Keys.Left);
        }
        private void DestroyMenuKeys()
        {
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
            UnregisterHotKey(this.Handle, 4);
            UnregisterHotKey(this.Handle, 5);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY && (int)m.WParam == 1)
                if (showMenu)
                {
                    showMenu = false;
                    DestroyMenuKeys();
                }
                else
                {
                    showMenu = true;
                    CreateMenuKeys();
                }


            //myConfig = JsonConvert.DeserializeObject<cConfig>(File.ReadAllText(Application.StartupPath + @"\config.cfg"));
            //Arrow UP
            if (m.Msg == WM_HOTKEY && (int)m.WParam == 2)
                if (showMenu)
                        CurrentOption -= 1;
            //Arrow Down
            if (m.Msg == WM_HOTKEY && (int)m.WParam == 3)
                if (showMenu)
                    CurrentOption += 1;

            if (m.Msg == WM_HOTKEY && (int)m.WParam == 4)
            {
                //Main Menu
                if (CurrentMenu == 1)
                {
                    if (CurrentOption == 3)
                        myConfig.bAimbot = (myConfig.bAimbot) ? false : true;
                    if (CurrentOption == 4)
                    {
                        myConfig.fov += 5f;
                        if (myConfig.fov > 360f)
                        {
                            myConfig.fov = 5;
                        }
                    }

                    if (CurrentOption == 5)
                    {
                        myConfig.smooth += 1;
                        if (myConfig.smooth >= 30)
                            myConfig.smooth = 0;
                    }
                    if (CurrentOption == 6)
                        myConfig.bAimTime = (myConfig.bAimTime) ? false : true;
                    if (CurrentOption == 7)
                        if (myConfig.bAimTime)
                            myConfig.fAimTime += 1;
                    if (CurrentOption == 8)
                        if (myConfig.bAimTime)
                            myConfig.fAimPauseTime += 1;
                    if (CurrentOption == 11)
                        myConfig.bESP = (myConfig.bESP) ? false : true;
                    if (CurrentOption == 12)
                        myConfig.drawAllies = (myConfig.drawAllies) ? false : true;
                    if (CurrentOption == 13)
                        myConfig.drawName = (myConfig.drawName) ? false : true;
                    if (CurrentOption == 14)
                        myConfig.drawHP = (myConfig.drawHP) ? false : true;
                    if (CurrentOption == 15)
                        myConfig.drawKV = (myConfig.drawKV) ? false : true;
                    if (CurrentOption == 16)
                        myConfig.drawWeapon = (myConfig.drawWeapon) ? false : true;
                    if (CurrentOption == 17)
                        myConfig.drawDistance = (myConfig.drawDistance) ? false : true;
                    if (CurrentOption == 18)
                        myConfig.drawHead = (myConfig.drawHead) ? false : true;
                    if (CurrentOption == 19)
                    {
                        myConfig.bGlow = (myConfig.bGlow) ? false : true;
                        if (myConfig.bGlow == false)
                            if (t_updGobj.IsAlive)
                                t_updGobj.Abort();
                    }

                    if (CurrentOption == 22)
                        myConfig.useRCS = (myConfig.useRCS) ? false : true;
                    if (CurrentOption == 23)
                        myConfig.RCSForce += 0.1f;
                    if (CurrentOption == 24)
                        myConfig.useRCSPistol = (myConfig.useRCSPistol) ? false : true;
                    if (CurrentOption == 25)
                        myConfig.useRCSSniper = (myConfig.useRCSSniper) ? false : true;
                    if (CurrentOption == 28)
                        myConfig.bTrigger = (myConfig.bTrigger) ? false : true;
                    if (CurrentOption == 29)
                        myConfig.bBunnyhop = (myConfig.bBunnyhop) ? false : true;
                    if (CurrentOption == 30)
                        myConfig.bDrawDebug = (myConfig.bDrawDebug) ? false : true;
                    if (CurrentOption == 31)
                        myConfig.bAimSpottedOnly = (myConfig.bAimSpottedOnly) ? false : true;
                    if (CurrentOption == 33)
                        File.WriteAllText(Application.StartupPath + @"\config.cfg", JsonConvert.SerializeObject(myConfig));
                    if (CurrentOption == 34)
                        myConfig.enableOutline = (myConfig.enableOutline) ? false : true;
                }
            }
            if (m.Msg == WM_HOTKEY && (int)m.WParam == 5)
            {
                //Main Menu
                if (CurrentMenu == 1)
                {
                    if (CurrentOption == 3)
                        myConfig.bAimbot = (myConfig.bAimbot) ? false : true;
                    if (CurrentOption == 4)
                    {
                        myConfig.fov -= 5;
                        if (myConfig.fov <= 0)
                            myConfig.fov = 360f;
                    }


                    if (CurrentOption == 5)
                    {
                        myConfig.smooth -= 1;
                        if (myConfig.smooth == -1)
                            myConfig.smooth = 30;
                    }

                    if (CurrentOption == 6)
                        myConfig.bAimTime = (myConfig.bAimTime) ? false : true;
                    if (CurrentOption == 7)
                        if (myConfig.bAimTime)
                            myConfig.fAimTime -= 1;
                    if (CurrentOption == 8)
                        if (myConfig.bAimTime)
                            myConfig.fAimPauseTime -= 1;
                    if (CurrentOption == 11)
                        myConfig.bESP = (myConfig.bESP) ? false : true;
                    if (CurrentOption == 12)
                        myConfig.drawAllies = (myConfig.drawAllies) ? false : true;
                    if (CurrentOption == 13)
                        myConfig.drawName = (myConfig.drawName) ? false : true;
                    if (CurrentOption == 14)
                        myConfig.drawHP = (myConfig.drawHP) ? false : true;
                    if (CurrentOption == 15)
                        myConfig.drawKV = (myConfig.drawKV) ? false : true;
                    if (CurrentOption == 16)
                        myConfig.drawWeapon = (myConfig.drawWeapon) ? false : true;
                    if (CurrentOption == 17)
                        myConfig.drawDistance = (myConfig.drawDistance) ? false : true;
                    if (CurrentOption == 18)
                        myConfig.drawHead = (myConfig.drawHead) ? false : true;
                    if (CurrentOption == 19)
                    {
                        myConfig.bGlow = (myConfig.bGlow) ? false : true;
                        if (myConfig.bGlow == false)
                            if (t_updGobj.IsAlive)
                                t_updGobj.Abort();
                    }
                    if (CurrentOption == 22)
                        myConfig.useRCS = (myConfig.useRCS) ? false : true;
                    if (CurrentOption == 23)
                        myConfig.RCSForce -= 0.1f;
                    if (CurrentOption == 24)
                        myConfig.useRCSPistol = (myConfig.useRCSPistol) ? false : true;
                    if (CurrentOption == 25)
                        myConfig.useRCSSniper = (myConfig.useRCSSniper) ? false : true;
                    if (CurrentOption == 28)
                        myConfig.bTrigger = (myConfig.bTrigger) ? false : true;
                    if (CurrentOption == 29)
                        myConfig.bBunnyhop = (myConfig.bBunnyhop) ? false : true;
                    if (CurrentOption == 30)
                        myConfig.bDrawDebug = (myConfig.bDrawDebug) ? false : true;
                    if (CurrentOption == 31)
                        myConfig.bAimSpottedOnly = (myConfig.bAimSpottedOnly) ? false : true;
                    if (CurrentOption == 33)
                        File.WriteAllText(Application.StartupPath + @"\config.cfg", JsonConvert.SerializeObject(myConfig, Formatting.Indented));
                    if (CurrentOption == 34)
                        myConfig.enableOutline = (myConfig.enableOutline) ? false : true;
                }
            }

            base.WndProc(ref m);
        }
        private void DoMenuStuff()
        {
            if (!showMenu)
                return;
            if (CurrentOption < 3)
                CurrentOption = 2;
            if (stepHop < 0.05f)
                stepHop = 0.05f;
            if (stepHop > 5f)
                stepHop = 5f;

            int maxOptions = 0;
            if (CurrentMenu == 1)
                maxOptions = 35;
            if (CurrentMenu == 2)
                maxOptions = 10;
            if (CurrentMenu == 3)
                maxOptions = 8;
            if (CurrentMenu == 4)
                maxOptions = 11;
            if (CurrentOption == maxOptions)
                CurrentOption = maxOptions - 1;
            CurrentY = CurrentOption * 13;
            //DrawText(string.Format("CurrentMenu={0} CurrentY={1} MaxOtions={2} CurrentOption={3}", CurrentMenu, CurrentY, maxOptions, CurrentOption), SharpDX.Color.White, 11, new Vector3(100, 100, 100));
            menuTimer -= 1;
            string curMenu = "";
            if (CurrentMenu == 1)
                curMenu = "Main";
            if (CurrentMenu == 2)
                curMenu = "ESP";
            if (CurrentMenu == 3)
                curMenu = "Misc";
            if (CurrentMenu == 4)
                curMenu = "Settings";

            int TextX = 15;
            if (curMenu == "Main")
            {
                //DrawRectangle(device, SharpDX.Color.Blue, 0, 0, 200, 110);
                //DrawText(curMenu + " ", SharpDX.Color.White, 10, new Vector3(1, 0, 0));
                FillRectangle(device, SharpDX.Color.Black, TextX - 10, 26, 150, 13 * 33);
                DrawRectangle(device, SharpDX.Color.RoyalBlue, TextX - 11, 25, 151, (13 * 33) +1);
                DrawText(">", SharpDX.Color.Red, 10, new Vector3(TextX - 10, CurrentY, 0));
                DrawText("[Aimbot]", SharpDX.Color.White, 10, new Vector3(TextX, 26, 0));
                DrawText("Enable: " + myConfig.bAimbot, SharpDX.Color.White, 10, new Vector3(TextX, 39, 0));
                DrawText("FOV: " + myConfig.fov.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 52, 0));
                DrawText("Smooth: " + myConfig.smooth.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 65, 0));
                DrawText("AimTime: " + myConfig.bAimTime.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 78, 0));
                DrawText("AimTime: " + myConfig.fAimTime.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 91, 0));
                DrawText("AimPauseTime: " + myConfig.fAimPauseTime.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 104, 0));
                DrawText("[ESP]", SharpDX.Color.White, 10, new Vector3(TextX, 130, 0));
                DrawText("Draw Box: " + myConfig.bESP.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 143, 0));
                DrawText("Draw Allies: " + myConfig.drawAllies.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 156, 0));
                DrawText("Draw Name: " + myConfig.drawName.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 169, 0));
                DrawText("Draw Health: " + myConfig.drawHP.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 182, 0));
                DrawText("Draw Kevlar: " + myConfig.drawKV.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 195, 0));
                DrawText("Draw Weapon: " + myConfig.drawWeapon.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 208, 0));
                DrawText("Draw Distance: " + myConfig.drawDistance.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 221, 0));
                DrawText("Draw Head: " + myConfig.drawHead.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 234, 0));
                DrawText("Glow: " + myConfig.bGlow.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 247, 0));
                DrawText("[RCS]", SharpDX.Color.White, 10, new Vector3(TextX, 273, 0));
                DrawText("Enable: " + myConfig.useRCS.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 286, 0));
                DrawText("Force: " + Math.Round(myConfig.RCSForce, 1).ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 299, 0));
                DrawText("Pistol RCS: " + myConfig.useRCSPistol.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 312, 0));
                DrawText("Sniper RCS: " + myConfig.useRCSSniper.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 325, 0));
                DrawText("[MISC]", SharpDX.Color.White, 10, new Vector3(TextX, 351, 0));
                DrawText("Triggerbot: " + myConfig.bTrigger.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 365, 0));
                DrawText("Bunnyhop: " + myConfig.bBunnyhop.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 377, 0));
                DrawText("Draw Debug: " + myConfig.bDrawDebug.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 390, 0));
                DrawText("Spotted: " + myConfig.bAimSpottedOnly.ToString(), SharpDX.Color.White, 10, new Vector3(TextX, 403, 0));
                DrawText("Save Config", SharpDX.Color.DeepSkyBlue, 10, new Vector3(TextX, 429, 0));
                DrawText("Enable Outline", SharpDX.Color.IndianRed, 10, new Vector3(TextX, 442, 0));
                //DrawShadowText("Hack ->", new Point(TextX, 26), Color.DeepSkyBlue);



            }
            if (curMenu == "ESP")
            {
                //DrawTransparentBox(0, 0, 200, 135, 248, Color.Black);
                //DrawShadowText(curMenu + " ", new Point(1, 0), Color.White);
                //DrawShadowText(">", new Point(0, CurrentY), Color.Red);

                //DrawShadowText("<- Back", new Point(TextX, 26), Color.DeepSkyBlue);

                //if (showPlayers)
                //    DrawShadowText("Show Players", new Point(TextX, 52), Color.Green);
                //else
                //    DrawShadowText("Show Players", new Point(TextX, 52), Color.Red);

                //if (TriggerHack)
                //    DrawShadowText("Trigger Hack", new Point(TextX, 65), Color.Green);
                //else
                //    DrawShadowText("Trigger Hack", new Point(TextX, 65), Color.Red);

            }
        }
        #endregion

        public void ReportBack()
        {

        }

        public void v_AllocConsole()
        {
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }

        public void RegisterHotKeys()
        {
            RegisterHotKey(this.Handle, 1, 0, (int)Keys.Insert);

        }
        private System.Drawing.Icon CreateIcon()
        {
            Icon ico = null;
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(32, 32))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Random ran = new Random((int)Environment.TickCount);
                    g.CopyFromScreen(ran.Next(0, 512), ran.Next(0, 512), 0, 0, new Size(128, 128), CopyPixelOperation.SourceCopy);
                }
                ico = Icon.FromHandle(bmp.GetHicon());
            }
            return ico;
        }

        public static void Log(string s, string file)
        {
            using (StreamWriter sw = new StreamWriter(file, true))
            {
                sw.WriteLine("[---" + DateTime.Now + "---]" + Environment.NewLine + s + Environment.NewLine + "[-------------------------]" + Environment.NewLine);
                sw.Close();
            }
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
        public Canvas()
        {
            InitializeComponent();
            //Log("Initializing form.", "debug.log");
            this.FormClosing += Canvas_Closing;
            this.Name = Generators.GetRandomString(32);
            this.Text = Generators.GetRandomString(32);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Icon = CreateIcon();
            this.DoubleBuffered = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            RegisterHotKeys();
            time.Elapsed += new System.Timers.ElapsedEventHandler(tima_Tick);
            time.Interval = 50;

            v_AllocConsole();
            //Log("Creating config.", "debug.log");
            if (File.Exists(Application.StartupPath + @"\config.cfg"))
            {
                try
                {
                    myConfig = JsonConvert.DeserializeObject<cConfig>(File.ReadAllText(Application.StartupPath + @"\config.cfg"));
                }
                catch { makenewCFG(); }


            }
            else
            {
                makenewCFG();
            }
            //Log("Attaching to process.", "debug.log");
            //this.TransparencyKey = System.Drawing.Color.Black;
            while (true)
            {
                if (IsProcessOpen("csgo"))
                {
                    break;
                }
                Thread.Sleep(100);
            }
            Thread.Sleep(7500);
            var proc = Process.GetProcessesByName("csgo")[0];
            //Log("Attached.", "debug.log");
            //Log("Setting vars.", "debug.log");
            //if(mypenis.)
            csgoWindow = getCsgoWindow();
            GetWindowRect(proc.MainWindowHandle, out my);
            MyWidth = my.Right - my.Left + 1;
            MyHeight = my.Bottom - my.Top + 1;
            System.Drawing.Point newloc = csgoWindow.Location;
            Console.WriteLine("X: " + newloc.X.ToString() + " Y: " + newloc.Y.ToString());
            Size thesize = csgoWindow.Size;
            //MyWidth = thesize.Width;//my.Right - my.Left;
            //MyHeight = thesize.Height;// my.Bottom - my.Top;
            Console.WriteLine("{0} {1}", MyWidth, MyHeight);
            ScreenMid = new Vector2(MyWidth / 2, (MyHeight / 2) - 5);
            //this.Height = MyHeight;
            //this.Width = MyWidth;
            //this.Location = newloc;
            //this.Location = new System.Drawing.Point(my.Top, (my.Top + my.Right) - my.Bottom);
            //weirda = my.Top;
            // weirde = (my.Top + my.Right) - my.Bottom;
            //
            //Log("Vars set.", "debug.log");
            //Log("Initializing device.", "debug.log");
            renderProperties = new HwndRenderTargetProperties()
            {
                Hwnd = this.Handle,
                PixelSize = new Size2(MyWidth, MyHeight),
                
                PresentOptions = PresentOptions.None
            };
            device = new WindowRenderTarget(factory,
                new RenderTargetProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied)), renderProperties);
            device.TextAntialiasMode = TextAntialiasMode.Cleartype;
            //Log("Device initialized.", "debug.log");
            //Log("Creating threads.", "debug.log");
            t3 = new Thread(new ThreadStart(Updater));
            //device.Resize(new Size2(MyWidth, MyHeight));

            //Make the window's border completely transparant
            SetWindowLong(this.Handle, GWL_EXSTYLE,
                    (IntPtr)(GetWindowLong(this.Handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT));

            //Set the Alpha on the Whole Window to 255(solid)
            SetLayeredWindowAttributes(this.Handle, 0, 255, LWA_ALPHA);

            SetWindowPos(this.Handle, new IntPtr(-1), 0, 0, 0, 0, TOPMOST_FLAGS);
            //Init DirectX
            //This initializes the DirectX device. It needs to be done once.
            //The alpha channel in the backbuffer is critical.
            OnResize(null);
            this.Paint += Canvas_Paint;
            //Log("Threads created.", "debug.log");
            //Log("Starting Updater loop.", "debug.log");
            Manager.Attach(proc);
            //MessageBox.Show("Starting thread on: " + MyWidth.ToString() + " - " +MyHeight.ToString());
            t3.Start();


            SetForegroundWindow(csgoWindow.Hwnd);
            CreateMenuKeys();
            //Log("Started.", "debug.log");
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);
        void Canvas_Paint(object sender, PaintEventArgs e)
        {

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            ////Create a margin (the whole form)
            marg.Left = 0;
            marg.Top = 0;
            marg.Right = this.Width;
            marg.Bottom = this.Height;

            ////Expand the Aero Glass Effect Border to the WHOLE form.
            //// since we have already had the border invisible we now
            //// have a completely invisible window - apart from the DirectX
            //// renders NOT in black.
            DwmExtendFrameIntoClientArea(this.Handle, ref marg);

        }

        private void Canvas_Closing(object sender, EventArgs e)
        {
            UnregisterHotKey(this.Handle, 1);
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
            UnregisterHotKey(this.Handle, 4);
            UnregisterHotKey(this.Handle, 5);
        }

        private void Canvas_Load(object sender, EventArgs e)
        {


        }

        private void GarbageCollector()
        {
            while (true)
            {
                GC.Collect(); Thread.Sleep(750);
            }

        }

        private void CheckForAbortion()
        {
            while (true)
            {
                if (!IsProcessOpen("csgo"))
                {
                    cancelH = true;
                    //UnregisterHotKey(this.Handle, 1);
                    break;
                }
                Thread.Sleep(1000);
            }

        }

        private void disposer()
        {
            device.EndDraw();
            Environment.Exit(0);
        }
        private void bhop()
        {
            Random myRandom = new Random();
            while (true)
            {
                if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
                {
                    if (Convert.ToBoolean(GetAsyncKeyState(0x20) & 0x8000) && myConfig.bBunnyhop)
                    {
                        if (Manager.Me.VelocityVec != Vector3.Zero)
                        {
                            int myass = Manager.Memory.Read<int>(Manager.Objects.LocalPlayer.BaseAddress + 0x100) & (1 << 0);
                            if (myass == 1)
                            {
                                Manager.Memory.Write(Manager.ClientBase + (int)Off.oBase.ForceJump, 5);
                                Thread.Sleep(20);
                                Manager.Memory.Write(Manager.ClientBase + (int)Off.oBase.ForceJump, 4);
                            }
                        }
                    }
                    Thread.Sleep(myRandom.Next(0, 20));
                }
                else { Thread.Sleep(1000); }


            }

        }

        private void debugOverlay(EngineObjects.Local me)
        {
            DrawText(string.Format("Current Weapon: {0}", me.WeaponID.ToString()), SharpDX.Color.Red, 10, new Vector3(1, 610, 0));
            DrawText(string.Format("Flag: {0}", me.Flags.ToString()), SharpDX.Color.Red, 10, new Vector3(1, 630, 0));
            DrawText(string.Format("Shots: {0}", me.ShotsFired.ToString()), SharpDX.Color.Red, 10, new Vector3(1, 640, 0));
            DrawText(string.Format("Current Punch - X{0} - Y{1} - Z{2}", me.PunchAngle.X, me.PunchAngle.Y, me.PunchAngle.Z), SharpDX.Color.Red, 10, new Vector3(1, 650, 100), 500);
            DrawText(string.Format("Current Angles - X{0} - Y{1} - Z{2}", me.ViewAngle.X, me.ViewAngle.Y, me.ViewAngle.Z), SharpDX.Color.DarkGray, 10, new Vector3(1, 660, 100), 500);
            DrawText(string.Format("Current OffAngles - X{0} - Y{1} - Z{2}", me.ViewOffset.X, me.ViewOffset.Y, me.ViewOffset.Z), SharpDX.Color.DarkGray, 10, new Vector3(1, 670, 100), 500);
            DrawText(string.Format("Current Velocity - X{0} - Y{1} - Z{2}", me.VelocityVec.X, me.VelocityVec.Y, me.VelocityVec.Z), SharpDX.Color.DarkGray, 10, new Vector3(1, 680, 100), 500);
            DrawText(me.Position.ToString(), SharpDX.Color.DarkGray, 10, new Vector3(1, 690, 100), 500);
        }

        Thread t_updResPTR;
        private void UpdateRESPTR()
        {
            while (true)
            {
                if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
                {
                    Manager.ResourcePointer = Manager.Memory.Read<IntPtr>(Manager.ClientBase + (int)Off.oBase.PlayerResource);
                }
                Thread.Sleep(1000);
            }
        }

        Thread t_updGobj;
        private void UpdateGObj()
        {
            while (myConfig.bGlow)
            {
                try
                {
                    if (t_updGobj.ThreadState != System.Threading.ThreadState.Stopped)
                        if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
                        {
                            Manager.GlowPointer = Manager.Memory.Read<IntPtr>(Manager.ClientBase + (int)Off.oBase.GlowObject);
                            Thread.Sleep(1000);
                        }
                }
                catch { Thread.ResetAbort(); }

            }
        }

        private Aimbot aimbot = new Aimbot();
        private RecoilControlSystem rcs = new RecoilControlSystem();
        private Entity theBomb;

        private static TextFormat crText = new TextFormat(fontFactory, "Arial", 9);

        public int GloObjCnt;
        public int curkills = 0;
        private void Updater()
        {
            //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Properties.Resources.logo);
            //logoBitmap = SDXBitmapFromSysBitmap(device, bmp);
            //Thread.Sleep(5000);
            Thread gcLect = new Thread(GarbageCollector);
            gcLect.Start();
            Thread mypenis = new Thread(() => CheckForAbortion());
            mypenis.Start();

            Thread bhopper = new Thread(() => bhop());
            bhopper.Start();
            trig = new Thread(() => tobreggirT());
            t_updGobj = new Thread(() => UpdateGObj());
            t_updResPTR = new Thread(() => UpdateRESPTR());
            t_updResPTR.Start();

            decimal myHalf = Math.Round((decimal)((MyWidth / 2) - 110f));
            decimal myHeightd = Math.Round((decimal)(MyHeight - 56));
            a_myHeight = Height;
            a_myWidth = Width;
            a_MyTop = Top;
            a_MyLeft = Left;


            while (true)
            {
                performance.Start();
                device.BeginDraw();
                device.Clear(SharpDX.Color.Transparent);
                //device.DrawTextLayout()
                //
                // MyHeight - copyright.Height
                DrawText("ResurrectedPUB External for CS:GO", SharpDX.Color.White, 10, new Vector3(5, 5, 0), 400);
                //DrawText(string.Format("Glow Thread State={0}", t_updGobj.ThreadState), SharpDX.Color.White, 12, new Vector3(400, 400, 0));

                DoMenuStuff();
                if (cancelH)
                {
                    //submit last session to ensure data.
                    if (Session.CheckSession())
                    {
                        CSession SsnHndlr = Session.ConstructSessionObject();
                        Session.StopSession();
                        string objc = JsonConvert.SerializeObject(SsnHndlr);
                        PipeClient.Send("SESSION:" + objc);
                    }
                    disposer();
                    return;
                }
                Manager.Objects.Update();
                canTrig = true;
                if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
                {
                    var me = Manager.Me;
                    if (myConfig.bDrawDebug)
                        debugOverlay(me);
                    curkills = me.i_Kills;
                    if (me.WeaponID == WeaponType.Sniper)
                        DrawText("•", SharpDX.Color.Red, 10, new Vector3(ScreenMid.X - 1.5f, ScreenMid.Y, 0));
                    aimbot.Run();
                    if (!myConfig.bAimbot)
                    {
                        if (myConfig.useRCS)
                            rcs.Run(me);

                    }
                    foreach (var player in Manager.Objects.Players)
                    {
                        if (player.Id < 64)
                        {
                            if (myConfig.bESP)
                                PSE(me, player);

                            if (myConfig.bGlow)
                            {
                                if (!t_updGobj.IsAlive)
                                {
                                    t_updGobj = new Thread(() => UpdateGObj());
                                    t_updGobj.Start();
                                }

                                Glow(player, me);
                            }

                            if (myConfig.bTrigger)
                                if (!trig.IsAlive)
                                    trig.Start();
                        }
                        if (player.ClsId == 29)
                        {
                            if (player.Position != Vector3.Zero)
                            {
                                Vector3 PPOS;
                                if (TheMaths.WorldToScreen(me.fViewMatrix, new SharpDX.Vector3(player.Position.X, player.Position.Y, player.Position.Z), out PPOS, Width, Height, Left, Top))
                                {
                                    DrawEntBox(PPOS, PPOS, 20);
                                }
                            }
                        }
                        if (player.ClsId == 105) //Planted C4
                        {
                            theBomb = player;
                            if (player.Position != Vector3.Zero)
                            {
                                Vector3 PPOS;
                                if (TheMaths.WorldToScreen(me.fViewMatrix, new SharpDX.Vector3(player.Position.X, player.Position.Y, player.Position.Z), out PPOS, Width, Height, Left, Top))
                                {
                                    DrawText(countDown, SharpDX.Color.Wheat, 11, PPOS);
                                    DrawEntBox(PPOS, PPOS, 20);
                                }
                            }
                        }
                    }
                    if (theBomb != null)
                        if (theBomb.IsValid)
                            try
                            {
                                int getBombTimer = Manager.Memory.Read<int>(theBomb.BaseAddress + 0x2970);
                                if (getBombTimer != 0)
                                {
                                    //DrawText("Bomb Ticking: " + getBombTimer.ToString(), SharpDX.Color.Red, 12, new Vector3(1, 400, 0));

                                    float bombLength = Manager.Memory.Read<float>(theBomb.BaseAddress + 0x2980);
                                    //DrawText("Timer Length: " + bombLength.ToString(), SharpDX.Color.Red, 12, new Vector3(1, 420, 0));
                                    if (!isTickingBomb)
                                    {
                                        bombExplode = DateTime.Now.AddSeconds(bombLength + 1);
                                        isTickingBomb = true;
                                        time.Start();
                                    }


                                    //DrawText("Bomb Exploding in: " + countDown, SharpDX.Color.White, 12, new Vector3(1, 440, 0));
                                }
                                else
                                    theBomb = null;

                            }
                            catch { }
                }
                if (Manager.gClient.State == SignonState.None || Manager.gClient.State == SignonState.New)
                {
                    if (Session.CheckSession())
                    {
                        CSession SsnHndlr = Session.ConstructSessionObject();
                        Session.StopSession();
                        string objc = JsonConvert.SerializeObject(SsnHndlr);
                        PipeClient.Send("SESSION:" + objc);
                    }
                }
                else if (Manager.gClient.State == SignonState.Full)
                {
                    if (!Session.CheckSession())
                    {
                        Session.StartSession(s_GetMapName());
                    }
                    Session.UpdateKills(curkills);
                }
                if (myConfig.bDrawDebug)
                    DrawText(string.Format("Loop Performance: {0}", performance.ElapsedMilliseconds.ToString() + "ms"), SharpDX.Color.Red, 10, new Vector3(1, 600, 0));

                performance.Reset();
                canTrig = false;
                device.EndDraw();
            }

        }





        DateTime bombExplode = new DateTime();
        bool isTickingBomb = false;
        System.Timers.Timer time = new System.Timers.Timer();
        static string countDown = "00:00";

        private void tima_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimeSpan leftTime = bombExplode.Subtract(DateTime.Now);
            if (leftTime.TotalSeconds < 0 || theBomb == null)
            {
                isTickingBomb = false;
                time.Stop();
            }
            else
                countDown = leftTime.Seconds.ToString("00") + ":" +
         (leftTime.Milliseconds / 10).ToString("00");
        }

        public string s_GetMapName()
        {
            return Manager.Memory.ReadString(Manager.EnginePointer + 0x284, Encoding.UTF8);
        }
        //public void DrawBones(EngineObjects.Local me, Vector3 PPOS, EngineObjects.Entity player)
        //{
        //    float precalctwo = PPOS.Y - my.Top;
        //    List<Vector3> vMatrixBones = new List<Vector3>();
        //    foreach (DictionaryEntry bone in player.Bones)
        //    {
        //        Vector3 bbone;
        //        if (TheMaths.WorldToScreen(me.fViewMatrix, (SharpDX.Vector3)bone.Value, out bbone, Width, Height, Left, Top))
        //        {
        //            vMatrixBones.Add(bbone);
        //            device.DrawText(bone.Key.ToString(), new TextFormat(fontFactory, "Arial", 10), new SharpDX.Rectangle((int)bbone.X, (int)bbone.Y, 20, 10), new SolidColorBrush(device, SharpDX.Color.White));
        //        }
        //    }
        //}

        public void tobreggirT()
        {
            while (myConfig.bTrigger)
            {
                if (canTrig)
                {
                    if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
                    {
                        var t = Manager.Me.Target;
                        if (t != null)
                            if (Manager.Me.Team != t.Team)
                                mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

                    }
                }

                Thread.Sleep(5);
            }

        }
        SharpDX.Color entClr = SharpDX.Color.Silver;
        public void DrawEntBox(Vector3 PPOS, Vector3 theHead, float height)
        {
            DrawLine(PPOS.X - (height / 4), (theHead.Y - 5), PPOS.X - (height / 4), PPOS.Y, 1.5f, entClr);
            DrawLine(PPOS.X + (height / 4), (theHead.Y - 5), PPOS.X + (height / 4), PPOS.Y, 1.5f, entClr);
            DrawLine(PPOS.X - (height / 4), (theHead.Y - 5), PPOS.X + (height / 4), (theHead.Y - 5), 1.5f, entClr);
            DrawLine(PPOS.X - (height / 4), PPOS.Y, PPOS.X + (height / 4), PPOS.Y, 1.5f, entClr);
        }

        public void DrawBox(Vector3 PPOS, Vector3 theHead, float height)
        {
            DrawLine(PPOS.X - (height / 4), (theHead.Y - 5), PPOS.X - (height / 4), PPOS.Y, 1.5f, pColor);
            DrawLine(PPOS.X + (height / 4), (theHead.Y - 5), PPOS.X + (height / 4), PPOS.Y, 1.5f, pColor);
            DrawLine(PPOS.X - (height / 4), (theHead.Y - 5), PPOS.X + (height / 4), (theHead.Y - 5), 1.5f, pColor);
            DrawLine(PPOS.X - (height / 4), PPOS.Y, PPOS.X + (height / 4), PPOS.Y, 1.5f, pColor);
        }
        public float height;

        private static TextFormat pseTxtFormat = new TextFormat(fontFactory, "Arial", 10);

        public void PSE(EngineObjects.Local me, EngineObjects.Entity player)
        {
            if (player.ClsId != 35)
                return;
            if (!myConfig.drawAllies)
                if (player.IsFriendly)
                    return;
            if (player.Id != me.Id)
                if (!player.IsDormant)
                    if (player.Health > 0)
                    {
                        Vector3 PPOS;
                        if (TheMaths.WorldToScreen(me.fViewMatrix, player.Position, out PPOS, Width, Height, Left, Top))
                        {
                            if (player.Team == EngineObjects.PlayerTeam.CounterTerrorist)
                            {
                                if (player.isSpotted == 1)
                                {
                                    pColor = myConfig.ColorCTSpotted;
                                }
                                else
                                {
                                    pColor = myConfig.ColorCT;
                                }
                            }
                            else if (player.Team == EngineObjects.PlayerTeam.Terrorist)
                            {
                                if (player.isSpotted == 1)
                                {
                                    pColor = myConfig.ColorTSpotted;
                                }
                                else
                                {
                                    pColor = myConfig.ColorT;
                                }
                            }

                            Vector3 theHead = new Vector3(0, 0, 0);
                            if (TheMaths.WorldToScreen(me.fViewMatrix, player.Head, out theHead, Width, Height, Left, Top))
                            {
                                if (myConfig.drawHead)
                                {
                                    DrawText("•", pColor, 10, theHead);
                                }
                                DrawBox(PPOS, theHead, (theHead.Y - 5) - PPOS.Y);
                                height = (theHead.Y - 5) - PPOS.Y;
                            }


                            if (myConfig.drawName)
                            {
                                SharpDX.RectangleF nameSize = MeasureString(pseTxtFormat, player.Name);
                                //Orignal:
                                //DrawText(player.Name, SharpDX.Color.White, 10, new Vector3(PPOS.X - nameSize.Width / 2, PPOS.Y + (height / 2) - (nameSize.Height), PPOS.Z));
                                DrawText(player.Name, SharpDX.Color.White, 10, new Vector3((PPOS.X - (height / 4)) + 5, PPOS.Y + (height / 2) - (nameSize.Height / 2), PPOS.Z));
                            }
                            if (myConfig.drawHP)
                            {
                                string playerHealth = player.Health.ToString() + "HP";                          
                                SharpDX.RectangleF healthSize = MeasureString(pseTxtFormat, playerHealth);
                                DrawText(playerHealth, SharpDX.Color.White, 10, new Vector3(PPOS.X + (height / 4) - (healthSize.Width + 5), PPOS.Y + (height / 2) - (healthSize.Height / 2), PPOS.Z));
                               
                            }
                            if(myConfig.drawKV)
                            {
                                string playerArmor = player.Armor == 0 ? "No Kevlar" : player.Armor.ToString() + "KV";
                                SharpDX.RectangleF kevSize = MeasureString(pseTxtFormat, playerArmor);
                                DrawText(playerArmor, SharpDX.Color.White, 10, new Vector3(PPOS.X + (height / 4) - (kevSize.Width + 5), PPOS.Y + (height / 2) + 10 - (kevSize.Height / 2), PPOS.Z));
                            }
                            if (myConfig.drawWeapon)
                            {
                                SharpDX.RectangleF wSize = MeasureString(pseTxtFormat, player.eWeaponIndex.ToString());
                                DrawText(player.eWeaponIndex.ToString(), SharpDX.Color.White, 10, new Vector3(PPOS.X - wSize.Width / 2, PPOS.Y + height - wSize.Height, PPOS.Z));
                            }
                            if (myConfig.drawDistance)
                            {
                                SharpDX.RectangleF dsize = MeasureString(pseTxtFormat, Math.Round(TheMaths.DistanceToOtherEntityInMetres(me, player)).ToString() + "M");
                                DrawText(Math.Round(TheMaths.DistanceToOtherEntityInMetres(me, player)).ToString() + "M", SharpDX.Color.White, 10, new Vector3(PPOS.X - dsize.Width / 2, PPOS.Y, PPOS.Z));
                            }
                            //SharpDX.RectangleF rminvec = MeasureString(new TextFormat(fontFactory, "Arial", 10), Math.Round(TheMaths.DistanceToOtherEntityInMetres(me, player)).ToString() + "M");
                            //DrawText(player.vecMins.ToString(), pColor, 10, new Vector3(PPOS.X, PPOS.Y - 15, PPOS.Z));
                            //SharpDX.RectangleF rmaxvec = MeasureString(new TextFormat(fontFactory, "Arial", 10), Math.Round(TheMaths.DistanceToOtherEntityInMetres(me, player)).ToString() + "M");
                            //DrawText(player.vecMaxs.ToString(), pColor, 10, new Vector3(PPOS.X - rmaxvec.Width / 2, PPOS.Y - 25, PPOS.Z));
                        }
                    }
        }



        public void DrawLine(float x1, float y1, float x2, float y2, float w, SharpDX.Color color)
        {
            using (SolidColorBrush scb = new SolidColorBrush(device, color))
            {
                device.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), scb, w);
            }
            //device.DrawLine(new Vector2(x1 + 1.5f, y1 + 1.5f), new Vector2(x2 - 1.5f, y2 + 1.5f), new SolidColorBrush(device, SharpDX.Color.Black));

            //device.DrawLine(new Vector2(x1 - 1.5f, y1 - 1.5f), new Vector2(x2 - 1.5f, y2 - 1.5f), new SolidColorBrush(device, SharpDX.Color.Black));
        }

        public static SharpDX.RectangleF MeasureString(TextFormat format, string text, float maxWidth, float maxHeight)
        {
            SharpDX.RectangleF rect;
            using (TextLayout layout = new TextLayout(fontFactory, text, format, maxWidth, maxHeight))
            {
                rect = new SharpDX.RectangleF(0, 0, layout.Metrics.Width, layout.Metrics.Height);
            }
            return rect;
        }
        public static SharpDX.RectangleF MeasureString(TextFormat format, string text)
        {
            return MeasureString(format, text, float.MaxValue, float.MaxValue);
        }

        SharpDX.Color newclr;
        public void Glow(Entity player, Local me)
        {
            if (player.ClsId != 35)
                return;
            if (player.Team == me.Team)
                if (!myConfig.drawAllies)
                    return;
            if (player.IsDormant || !player.IsAlive)
                return;

            if (player.Team == EngineObjects.PlayerTeam.CounterTerrorist)
            {
                if (player.isSpotted == 1)
                {
                    newclr = myConfig.ColorCTSpotted;
                }
                else
                {
                    newclr = myConfig.ColorCT;
                }
            }
            else if (player.Team == EngineObjects.PlayerTeam.Terrorist)
            {
                if (player.isSpotted == 1)
                {
                    newclr = myConfig.ColorTSpotted;
                }
                else
                {
                    newclr = myConfig.ColorT;
                }
            }
            try
            {
                int idx = player.GlowIndex;
                if (idx == 0 || idx >= 64)
                    return;
                Manager.Memory.Write((Manager.GlowPointer + ((player.GlowIndex * 0x38) + 0x04)), (float)(newclr.R / 255f));
                Manager.Memory.Write((Manager.GlowPointer + ((player.GlowIndex * 0x38) + 0x08)), (float)(newclr.G / 255f));
                Manager.Memory.Write((Manager.GlowPointer + ((player.GlowIndex * 0x38) + 0x0C)), (float)(newclr.B / 255f));
                Manager.Memory.Write((Manager.GlowPointer + ((player.GlowIndex * 0x38) + 0x10)), (float)(newclr.A / 255f));
                Manager.Memory.Write((Manager.GlowPointer + ((player.GlowIndex * 0x38) + 0x24)), true);
                Manager.Memory.Write((Manager.GlowPointer + ((player.GlowIndex * 0x38) + 0x25)), false);
            }
            catch { Console.WriteLine("Glow broke :("); }



        }
        public static GlowObjDef defs = new GlowObjDef();
        public void WriteGlowObject(GlowObjDef def, int index)
        {
            byte[] data = def.GetBytes();
            byte[] writeData = new byte[GlowObjDef.GetSize() - 14];
            Array.Copy(data, 4, writeData, 0, writeData.Length);
            Manager.Memory.WriteBytes(Manager.GlowPointer + GlowObjDef.GetSize() * index, writeData);
            //WinAPI.WriteMemory(scanner.Process.Handle, glowAddr + GlowObjectDefinition.GetSize() * index + 4, writeData, writeData.Length);
        }
        public void DrawEsp(List<Vector3> bones, SharpDX.Color clr)
        {
            if (bones.Count <= 3)
                return;
            try
            {
                using (SolidColorBrush scb = new SolidColorBrush(device, clr))
                {
                    device.DrawLine(
                        new Vector2(bones[0].X, bones[0].Y),
                        new Vector2(bones[1].X, bones[1].Y),
                        scb,
                        1.5f
                        );
                    device.DrawLine(
                        new Vector2(bones[0].X, bones[0].Y),
                        new Vector2(bones[2].X, bones[2].Y),
                        scb,
                        1.5f
                        );
                    device.DrawLine(
                        new Vector2(bones[1].X, bones[1].Y),
                        new Vector2(bones[3].X, bones[3].Y),
                        scb,
                        1.5f
                        );
                    device.DrawLine(
                        new Vector2(bones[3].X, bones[3].Y),
                        new Vector2(bones[2].X, bones[2].Y),
                        scb,
                        1.5f
                        );
                }

            }
            catch { };


        }

        public void DrawText(string text, SharpDX.Color color, int size, Vector3 theVecs, int width = 200, int height = 20)
        {
            // device.DrawText(text, new TextFormat(fontFactory, "Arial Bold", size), new SharpDX.Rectangle((int)theVecs.X, (int)theVecs.Y, width, height), new SolidColorBrush(device, SharpDX.Color.DarkBlue));
            //TextFormat txtform = ;
            if(myConfig.enableOutline)
                using (OutlineTextRender textRenderer = new OutlineTextRender(device, new SolidColorBrush(device, SharpDX.Color.Black)))
                {
                    textRenderer.strokeText(text, theVecs.X, theVecs.Y, width, SharpDX.Color.Black, size, width);
                }

            using (TextFormat txtform = new TextFormat(fontFactory,
                "Tahoma",
                FontWeight.Normal,
                SharpDX.DirectWrite.FontStyle.Normal,
                FontStretch.Normal,
                size))
            {
                using (SolidColorBrush clrBrush = new SolidColorBrush(device, color))
                {
                    device.DrawText(text, txtform, new SharpDX.Rectangle((int)theVecs.X, (int)theVecs.Y, width, height), clrBrush);
                }
            }
        }
        private bool BoneToScreen(Matrix4x4 _ViewMatrix, Vector3 _Position, out Vector3 _Result)
        {
            _Result = new Vector3(0, 0, 0);

            _Result.X = (_ViewMatrix.M11 * _Position.X) + (_ViewMatrix.M12 * _Position.Y) + (_ViewMatrix.M13 * _Position.Z) + _ViewMatrix.M14;
            _Result.Y = (_ViewMatrix.M21 * _Position.X) + (_ViewMatrix.M22 * _Position.Y) + (_ViewMatrix.M23 * _Position.Z) + _ViewMatrix.M24;
            _Result.Z = (_ViewMatrix.M41 * _Position.X) + (_ViewMatrix.M42 * _Position.Y) + (_ViewMatrix.M43 * _Position.Z) + _ViewMatrix.M44;

            float invw = 1.0f / _Result.Z;
            _Result.X *= invw;
            _Result.Y *= invw;

            float x = Width / 2;
            float y = Height / 2;

            x += 0.5f * _Result.X * Width + 0.5f;
            y -= 0.5f * _Result.Y * Height + 0.5f;

            _Result.X = x + Left;
            _Result.Y = y + Top;
            //Console.WriteLine("Last - X: " + _Result.X.ToString() + " - " + _Result.Y.ToString());
            return true;
        }

        protected void FillRectangle(WindowRenderTarget device, SharpDX.Color Color, float x, float y, float width, float height)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, Color))
            {
                device.FillRectangle(new SharpDX.RectangleF(x, y, width, height), brush);
            }
        }
        protected void DrawRectangle(WindowRenderTarget device, SharpDX.Color Color, float x, float y, float width, float height, float strokeWidth = 2f, float outlineThickness = 0.5f)
        {
            using (SolidColorBrush brush = new SolidColorBrush(device, Color))
            {
                device.DrawRectangle(new SharpDX.RectangleF(x, y, width, height), brush, strokeWidth);
            }
        }

    }

}

