﻿using ShrineFox.IO.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class Output
    {
        /// <summary>
        /// Whether to collect log info.
        /// </summary>
        public static bool Logging { get; set; } = false;

        /// <summary>
        /// Whether to append log info to a .txt file.
        /// </summary>
        public static bool LogToFile { get; set; } = false;

        /// <summary>
        /// The path of the file to output log text to.
        /// </summary>
        public static string LogPath { get; set; } = "log.txt";

        /// <summary>
        /// Whether to show extra info in the log.
        /// </summary>
        public static bool VerboseLogging { get; set; } = false;

        /// <summary>
        /// The color to use for console/form control log text when not manually specified.
        /// </summary>
        public static ConsoleColor DefaultColor { get; set; } = ConsoleColor.White;

        /// <summary>
        /// The form control to output text to.
        /// Must be set in order for log text to appear in form.
        /// </summary>
        public static RichTextBox LogControl { get; set; } = null;

        /// <summary>
        /// Logs text with a timestamp to the directory specified by LogPath.
        /// </summary>
        /// <param name="text">The text to log.</param>
        /// <param name="color">The color of the text in the console/form. Will use DefaultColor if not specified.</param>
        public static void Log(string text, ConsoleColor color = new ConsoleColor(), string path = null)
        {
            Console.OutputEncoding = Encoding.Unicode;
            if (!Logging)
                return;

            if (path == null)
                path = LogPath;

            // Add timestamp before text
            string logText = $"\n[{DateTime.Now.ToString("MM/dd/yyyy HH:mm tt")}] {text}";

            // Output to txt file
            if (LogToFile)
                File.AppendAllText(path, logText);

            // Set the color to the default color unless one is specified
            if (color == new ConsoleColor())
                color = DefaultColor;

            // Append text to form control if specified
            if (LogControl != null)
            {
                if (LogControl.InvokeRequired)
                {
                    LogControl.BeginInvoke(new Action(() =>
                    {
                        LogControl.SuspendLayout();
                        //LogControl.SelectionStart = LogControl.TextLength;
                        LogControl.SelectionLength = 0;
                        LogControl.SelectionColor = FromColor(color);
                        LogControl.AppendText(logText);
                        LogControl.ScrollToCaret();
                        LogControl.ResumeLayout();
                        LogControl.Refresh();
                    }));
                }
                else
                {
                    LogControl.SuspendLayout();
                    //LogControl.SelectionStart = LogControl.TextLength;
                    LogControl.SelectionLength = 0;
                    LogControl.SelectionColor = FromColor(color);
                    LogControl.AppendText(logText);
                    LogControl.ScrollToCaret();
                    LogControl.ResumeLayout();
                    LogControl.Refresh();
                }
            }

            if (color != new ConsoleColor())
                Console.ForegroundColor = color;
            Console.Write(logText);
            Console.ResetColor();
        }

        /// <summary>
        /// If VerboseLogging is set to true, logs text with a timestamp to the directory specified by LogPath.
        /// </summary>
        /// <param name="text">The text to log.</param>
        /// <param name="color">The color of the text in the console/form.</param>
        public static void VerboseLog(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            if (VerboseLogging)
                Log(text, color);
        }

        // Convert ConsoleColor to Color
        private static Color FromColor(ConsoleColor c)
        {
            int cInt = (int)c;

            int brightnessCoefficient = ((cInt & 8) > 0) ? 2 : 1;
            int r = ((cInt & 4) > 0) ? 64 * brightnessCoefficient : 0;
            int g = ((cInt & 2) > 0) ? 64 * brightnessCoefficient : 0;
            int b = ((cInt & 1) > 0) ? 64 * brightnessCoefficient : 0;

            return Color.FromArgb(r, g, b);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();

        public static bool HasMainWindow()
        {
            return (Process.GetCurrentProcess().MainWindowHandle != IntPtr.Zero);
        }
    }
}
