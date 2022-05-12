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
    public class Output
    {
        /// <summary>
        /// The path of the file to output log text to.
        /// Must be set in order for text to be logged.
        /// </summary>
        public static string LogPath { get; set; } = "";

        /// <summary>
        /// Logs text with a timestamp to the directory specified by LogPath.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void Log(string text)
        {
            if (LogPath != "")
                File.AppendAllText(LogPath, $"\n[{DateTime.Now.ToString("MM/dd/yyyy HH:mm tt")}] text");
            Console.WriteLine(text);
        }
    }
}
