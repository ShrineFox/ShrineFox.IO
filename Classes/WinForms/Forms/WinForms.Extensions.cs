using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public static class WinFormsExtensions
    {
        // Return all controls of a certain type in a given control
        public static IEnumerable<T> GetAllControls<T>(this Control container) where T : Control
        {
            var controls = container.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls<T>(ctrl))
                                      .Concat(controls)
                                      .OfType<T>();
        }

        // Return all controls of a certain type in a given form
        public static IEnumerable<T> GetAllControls<T>(this Form form) where T : Control
        {
            var tlp = form.Controls.OfType<TableLayoutPanel>().FirstOrDefault();

            var controls = tlp.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls<T>(ctrl))
                                      .Concat(controls)
                                      .OfType<T>();
        }

        // Improve menustrip rendering for dark theme
        public static ToolStripProfessionalRenderer renderer =
            new ToolStripProfessionalRenderer(new MyColorTable(Color.FromArgb(255, 20, 20, 20)));
        public class MyColorTable : ProfessionalColorTable
        {
            private Color menuItemSelectedColor;
            public MyColorTable(Color color) : base()
            {
                menuItemSelectedColor = color;
            }
            public override Color MenuItemSelected
            {
                get { return menuItemSelectedColor; }
            }
        }

        // Allow toggling of TableLayoutPanel rows/columns
        public static void HideRows(this TableLayoutPanel panel, params int[] rowNumbers)
        {
            foreach (Control c in panel.Controls)
                if (rowNumbers.Contains(panel.GetRow(c)))
                    c.Visible = false;
        }
        public static void ShowRows(this TableLayoutPanel panel, params int[] rowNumbers)
        {
            foreach (Control c in panel.Controls)
                if (rowNumbers.Contains(panel.GetRow(c)))
                    c.Visible = true;
        }
        public static void HideColumns(this TableLayoutPanel panel, params int[] colNumbers)
        {
            foreach (Control c in panel.Controls)
                if (colNumbers.Contains(panel.GetColumn(c)))
                    c.Visible = false;
        }
        public static void ShowColumns(this TableLayoutPanel panel, params int[] colNumbers)
        {
            foreach (Control c in panel.Controls)
                if (colNumbers.Contains(panel.GetColumn(c)))
                    c.Visible = true;
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
