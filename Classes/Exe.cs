using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Permissions;
using System.Security;
using System.Text.RegularExpressions;
using System.Text;

namespace ShrineFox.IO
{
    public class Exe
    {
        /// <summary>
        /// Returns the path to the program's executable.
        /// </summary>
        /// <returns></returns>
        public static string FullPath()
        {
            return Process.GetCurrentProcess().MainModule.FileName;
        }

        /// <summary>
        /// Returns the path to the program's executable directory.
        /// </summary>
        /// <returns></returns>
        public static string Directory()
        {
            return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        /// <summary>
        /// Returns the name of the program being executed.
        /// </summary>
        /// <returns></returns>
        public static string Name()
        {
            return Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.ModuleName);
        }

        /// <summary>
        /// Returns the assembly specified by name.
        /// </summary>
        /// <returns></returns>
        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        public static Type GetType(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Reverse()
                .Select(assembly => assembly.GetType(name))
                .FirstOrDefault(t => t != null) ??
                AppDomain.CurrentDomain.GetAssemblies()
                .Reverse()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name.Contains(name));
        }

        public static Type GetEnumType(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Reverse()
                .Select(assembly => assembly.GetType(name))
                .FirstOrDefault(t => t != null && t.IsEnum) ??
                AppDomain.CurrentDomain.GetAssemblies()
                .Reverse()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name.Contains(name));
        }

        public static dynamic GetInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static dynamic GetClassInstance(Type type)
        {
            // first, create a handle instead of the actual object
            ObjectHandle classInstanceHandle = Activator.CreateInstance(type.Assembly.FullName, type.FullName);
            // unwrap the real slim-shady
            object classInstance = classInstanceHandle.Unwrap();
            // re-map the type to that of the object we retrieved
            type = classInstance.GetType();

            return Activator.CreateInstance(type);
        }

        public static void InvokeMethod(string assemblyName, string namespaceName, string typeName, string methodName, object[] args = null)
        {
            Type calledType = Type.GetType(namespaceName + "." + typeName + "," + assemblyName);
            calledType.InvokeMember(methodName, 
                BindingFlags.InvokeMethod | BindingFlags.Public |BindingFlags.Static,
                null, null, args);
        }

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
        /// <param name="waitForExit">(Optional) Whether to halt code execution until process is complete. True by default.</param>
        /// <param name="workingDir">(Optional) The directory to execute from. Uses exePath directory if not specified.</param>
        public static void Run(string exePath, string args = "", bool waitForExit = true, string workingDir = "", 
            bool hideWindow = true, bool redirectStdOut = false)
        {
            using (Process p = new Process())
            {
                // Set working directory to exe dir if not specified
                if (workingDir != "")
                    p.StartInfo.WorkingDirectory = workingDir;
                else
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(exePath);

                // Set exe path and args, hide window and redirect output
                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = args;
                p.StartInfo.CreateNoWindow = hideWindow;
                p.StartInfo.UseShellExecute = !redirectStdOut;
                p.StartInfo.RedirectStandardOutput = redirectStdOut;
                p.StartInfo.RedirectStandardError = redirectStdOut;
                p.StartInfo.StandardOutputEncoding = Encoding.Unicode;
                p.StartInfo.StandardErrorEncoding = Encoding.Unicode;
                p.EnableRaisingEvents = true;
                p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                p.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                p.Exited += ProcessEnded;
                p.Start();
                Exe.Processes.Add(new Tuple<string, IntPtr>(p.ProcessName, p.Handle));
                p.BeginOutputReadLine();
                //Output.Log(p.StandardOutput.ReadToEnd());

                if (waitForExit)
                    p.WaitForExit();

                RemoveHandleFromProcList(p.Handle);

                p.Close();
                p.Dispose();
            }
        }

        /// <summary>
        /// Closes all processes with a specific name.
        /// </summary>
        /// <param name="procName">The name of the process to close.</param>
        /// <param name="closeAll">(Optional) Close all processes with this name regardless of whether it was launched by this program.</param>
        public static void CloseProcess(string procName, bool closeAll = false)
        {
            if (!closeAll)
            {
                foreach (var handle in Processes.Where(x => x.Item1.Equals(procName)))
                {
                    CloseProcess(handle.Item2);
                }
            }
            else
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

    // Return the path of the currently executing program
    public static class EntryAssemblyInfo
    {
        private static string _executablePath;

        public static string ExecutablePath
        {
            get
            {
                if (_executablePath == null)
                {
                    PermissionSet permissionSets = new PermissionSet(PermissionState.None);
                    permissionSets.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
                    permissionSets.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
                    permissionSets.Assert();

                    string uriString = null;
                    var entryAssembly = Assembly.GetEntryAssembly();

                    if (entryAssembly == null)
                        uriString = Process.GetCurrentProcess().MainModule.FileName;
                    else
                        uriString = entryAssembly.CodeBase;

                    PermissionSet.RevertAssert();

                    if (string.IsNullOrWhiteSpace(uriString))
                        throw new Exception("Can not Get EntryAssembly or Process MainModule FileName");
                    else
                    {
                        var uri = new Uri(uriString);
                        if (uri.IsFile)
                            _executablePath = string.Concat(uri.LocalPath, Uri.UnescapeDataString(uri.Fragment));
                        else
                            _executablePath = uri.ToString();
                    }
                }

                return _executablePath;
            }
        }
    }
}
