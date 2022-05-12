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
    public class Exe
    {
        /// <summary>
        /// List of processes that were started by the program, and their handles.
        /// </summary>
        public static List<Tuple<string, IntPtr>> Processes { get; set; } = new List<Tuple<string, IntPtr>>();

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
                Exe.Processes.Add(new Tuple<string, IntPtr>(p.ProcessName, p.Handle));
                // Start the asynchronous read
                p.BeginOutputReadLine();
                p.WaitForExit();
                p.Close();
                RemoveHandleFromProcList(p.Handle);
                p.Dispose();
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
                {
                    p.Kill();
                    RemoveHandleFromProcList(p.Handle);
                } 
            }
            catch { }
        }

        /// <summary>
        /// Closes all processes with a specific handle.
        /// </summary>
        /// <param name="handle">The handle of the process to close.</param>
        public static void CloseProcess(IntPtr handle)
        {
            try
            {
                if (handle != new IntPtr())
                    foreach (Process p in Process.GetProcesses().Where(p => p.Handle.Equals(handle)))
                    {
                        p.Kill();
                        RemoveHandleFromProcList(handle);
                    }
            }
            catch { }
        }

        /// <summary>
        /// Replaces Process list with a new list of only currently running handles.
        /// </summary>
        /// <param name="handle">The handle to remove from the list.</param>
        private static void RemoveHandleFromProcList(IntPtr handle)
        {
            UpdateProcessList();

            if (Processes.Any(x => x.Item2.Equals(handle)))
                Processes.Remove(Processes.First(x => x.Item2.Equals(handle)));
        }

        /// <summary>
        /// Update Exe.Processes to only include processes that are currently running.
        /// </summary>
        public static void UpdateProcessList()
        {
            List<Tuple<string, IntPtr>> newProcList = new List<Tuple<string, IntPtr>>();
            foreach (var p in Processes)
                if (Process.GetProcessesByName(p.Item1).Any(x => x.Handle.Equals(p.Item2)))
                    newProcList.Add(p);
            Processes = newProcList;
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
                    RemoveHandleFromProcList(process.Handle);
                }
            }
            catch { }
        }
    }
}
