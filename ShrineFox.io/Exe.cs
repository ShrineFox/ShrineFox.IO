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

namespace ShrineFox.io
{
    public class Exe
    {
        /// <summary>
        /// Runs an exe and outputs text to the console.
        /// Also logs to text file if Output.LogPath is set.
        /// Waits for process to exit before continuing.
        /// </summary>
        /// <param name="exePath">Path to the .exe to execute.</param>
        /// <param name="args">Additional arguments for the exe.</param>
        public static void Run_WaitForExit(string exePath, string args = "")
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = args;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                // Set event handler
                p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                // Start the process
                p.EnableRaisingEvents = true;
                p.Exited += ProcessEnded;
                p.Start();
                // Start the asynchronous read
                p.BeginOutputReadLine();
                p.WaitForExit();
                p.Close();
                p.Dispose();
            }
        }

        /// <summary>
        /// Launches an .exe and continues immediately.
        /// </summary>
        /// <param name="exePath">Path to the .exe to execute.</param>
        /// <param name="args">Additional arguments for the exe.</param>
        public static void Run(string exePath, string args = "")
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = args;
                p.Start();
            }
        }

        /// <summary>
        /// Closes all processes with a specific name.
        /// </summary>
        /// <param name="procName">The name of the process to close.</param>
        public static void CloseProcess(string procName)
        {
            try
            {
                foreach (Process p in Process.GetProcessesByName(procName))
                    p.Kill();
            }
            catch { }
        }

        /// <summary>
        /// Log output to the console and a specific text file (if specified by Output.LogPath).
        /// </summary>
        private static void OutputHandler(object sender, DataReceivedEventArgs e)
        {
            Output.Log(e.Data);
        }

        /// <summary>
        /// Log error if commandline process ends unexpectedly.
        /// </summary>
        private static void ProcessEnded(object sender, EventArgs e)
        {
            var process = sender as Process;
            try
            {
                using (StreamReader sr = process.StandardError)
                {
                    string error = sr.ReadToEnd();
                    Console.WriteLine(error);
                }
            }
            catch { }
        }
    }
}
