using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class Window
    {
        #region Win32Functions
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        static extern IntPtr GetMenu(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern int GetMenuItemCount(IntPtr hMenu);
        [DllImport("user32.dll")]
        static extern bool DrawMenuBar(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);

        const int GWL_STYLE = (-16);
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 2;
        private const int SW_NORMALE = 1;
        public static uint MF_BYPOSITION = 0x400;
        public static uint MF_REMOVE = 0x1000;
        const uint WS_VISIBLE = 0x10000000;
        #endregion

        /// <summary>
        /// Starts another process and nests the window inside a WinWinForms Control.
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="args"></param>
        /// <param name="control"></param>
        public static void Mount(string exePath, Control control, string args = "")
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = args;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                Exe.Processes.Add(new Tuple<string, IntPtr>(p.ProcessName, p.Handle));
                Thread.Sleep(1200);
                p.WaitForInputIdle();

                SetParent(p.MainWindowHandle, control.Handle);
                ShowWindow(p.MainWindowHandle, SW_MINIMIZE);
                SetForegroundWindow(p.MainWindowHandle);
                SetFocus(p.MainWindowHandle);
                MoveWindow(p.MainWindowHandle, 0, 0, control.Width, control.Height, true);
                IntPtr HMENU = GetMenu(p.MainWindowHandle);
                int count = GetMenuItemCount(HMENU);
                for (int i = 0; i < count; i++)
                    RemoveMenu(HMENU, 0, (MF_BYPOSITION | MF_REMOVE));
                DrawMenuBar(p.MainWindowHandle);
                SetWindowLong(p.MainWindowHandle, GWL_STYLE, WS_VISIBLE);
                ShowWindow(p.MainWindowHandle, SW_MAXIMIZE);
                SetForegroundWindow(p.MainWindowHandle);
                SetFocus(p.MainWindowHandle);
            }
        }

        
    }
}
