using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB
{
    public static class PipeClient
    {
        public static void Send(string SendStr)
        {
            int hwnd;

            //Get a handle for the Calculator Application main window
            hwnd = Win32.FindWindow(null, "ResurrectedPUB");
            if (hwnd != 0)
                SendArgs((IntPtr)hwnd, SendStr);
        }
        public static bool SendArgs(IntPtr targetHWnd, string args)
        {
            Win32.CopyDataStruct cds = new Win32.CopyDataStruct();
            try
            {
                cds.cbData = (args.Length + 1) * 2;
                cds.lpData = Win32.LocalAlloc(0x40, cds.cbData);
                Marshal.Copy(args.ToCharArray(), 0, cds.lpData, args.Length);
                cds.dwData = (IntPtr)1;
                Win32.SendMessage(targetHWnd, Win32.WM_COPYDATA, IntPtr.Zero, ref cds);
            }
            finally
            {
                cds.Dispose();
            }

            return true;
        }
    }
}
