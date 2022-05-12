using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShrineFox.IO
{
    public class Python
    {
        /// <summary>
        /// List of potential registry keys that might have Python install paths
        /// </summary>
        private static string[] PossibleInstallLocations = new string[3] 
        {
                @"HKLM\SOFTWARE\Python\PythonCore\",
                @"HKCU\SOFTWARE\Python\PythonCore\",
                @"HKLM\SOFTWARE\Wow6432Node\Python\PythonCore\"
        };

        /// <summary>
        /// Dictionary of Python installs found using GetInstalls().
        /// Item 1: Version Number (e.x. "2.7.0")
        /// Item 2: Path (e.x. "C:\Python27")
        /// </summary>
        public static Dictionary<string, string> FoundLocations = new Dictionary<string, string>();

        /// <summary>
        /// Update FoundLocations with the path of every Python install within a specified range.
        /// </summary>
        /// <param name="requiredVersion">The lowest required Python version. e.x. "0.0.1"</param>
        /// <param name="maxVersion">The highest allowed Python version. e.x. "999.999.999"</param>
        /// <param name="requiredScripts">A list of .exe names required to be found in the Python install's Scripts directory. e.x. "byml_to_yml.exe"</param>
        /// <returns></returns>
        public static void GetInstalls(string requiredVersion = "", string maxVersion = "", List<string> requiredScripts = null)
        {
            foreach (string possibleLocation in PossibleInstallLocations)
            {
                try
                {
                    string regKey = possibleLocation.Substring(0, 4), actualPath = possibleLocation.Substring(5);
                    RegistryKey theKey = (regKey == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser);
                    RegistryKey theValue = theKey.OpenSubKey(actualPath);

                    foreach (var v in theValue.GetSubKeyNames())
                    {
                        RegistryKey productKey = theValue.OpenSubKey(v);
                        if (productKey != null)
                        {
                            try
                            {
                                string pythonExePath = productKey.OpenSubKey("InstallPath").GetValue("ExecutablePath").ToString();
                                if (pythonExePath != null && pythonExePath != "")
                                {
                                    if (requiredScripts != null && !requiredScripts.Any(x => !File.Exists(Path.Combine(Path.GetDirectoryName(pythonExePath), Path.Combine("Scripts", x)))))
                                        FoundLocations.Add(v.ToString(), pythonExePath);
                                }
                                else
                                    FoundLocations.Add(v.ToString(), pythonExePath);
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            if (FoundLocations.Count > 0)
            {
                System.Version desiredVersion = new System.Version(requiredVersion == "" ? "0.0.1" : requiredVersion),
                    maxPVersion = new System.Version(maxVersion == "" ? "999.999.999" : maxVersion);

                string highestVersion = "", highestVersionPath = "";

                foreach (KeyValuePair<string, string> pVersion in FoundLocations)
                {
                    //Require 64 bit
                    if (!pVersion.Value.Contains("-32"))
                    {
                        int index = pVersion.Key.IndexOf("-"); //For x-32 and x-64 in version numbers
                        string formattedVersion = index > 0 ? pVersion.Key.Substring(0, index) : pVersion.Key;

                        System.Version thisVersion = new System.Version(formattedVersion);
                        int comparison = desiredVersion.CompareTo(thisVersion),
                            maxComparison = maxPVersion.CompareTo(thisVersion);

                        if (comparison <= 0)
                        {
                            //Version is greater or equal
                            if (maxComparison >= 0)
                            {
                                desiredVersion = thisVersion;

                                highestVersion = pVersion.Key;
                                highestVersionPath = pVersion.Value;
                            }
                        }
                    }
                }
            }

            return;
        }
    }
}
