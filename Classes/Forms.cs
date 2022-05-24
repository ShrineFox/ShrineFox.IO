using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public static class FormExtensions
    {
        public static IEnumerable<T> GetAllControls<T>(this Control container) where T : Control
        {
            var controls = container.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls<T>(ctrl))
                                      .Concat(controls)
                                      .OfType<T>();
        }

        public static IEnumerable<T> GetAllControls<T>(this Form form) where T : Control
        {
            var tlp = form.Controls.OfType<TableLayoutPanel>().FirstOrDefault();

            var controls = tlp.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls<T>(ctrl))
                                      .Concat(controls)
                                      .OfType<T>();
        }
    }

    public class Forms
    {
        public static void FolderPath_Click(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Choose File...";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtBox.Text = dialog.FileName;
            }
        }

        public static void FilePath_Click(object sender, EventArgs e)
        {
            TextBox txtBox = (TextBox)sender;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Choose File...";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtBox.Text = dialog.FileName;
            }
        }

        public static bool YesNoMsgBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            Output.Log($"{title}: {message}");
            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon) == DialogResult.Yes)
            {
                Output.Log($"{title}: {message}\n> Yes\n");
                return true;
            }
            else
            {
                Output.Log($"{title}: {message}\n> No\n");
                return false;
            }
        }

        public static bool OKCancelBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            Output.Log($"{title}: {message}");
            if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel, icon) == DialogResult.Yes)
            {
                Output.Log($"{title}: {message}\n> OK\n");
                return true;
            }
            else
            {
                Output.Log($"{title}: {message}\n> Cancel\n");
                return false;
            }
        }

        public static void OKMsgBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon);
            Output.Log($"{title}: {message}");
        }

        /// <summary>
        /// Sets the current form's icon to be the default icon for every form.
        /// </summary>
        public static void SetDefaultIcon()
        {
            var icon = Icon.ExtractAssociatedIcon(EntryAssemblyInfo.ExecutablePath);
            typeof(Form)
                .GetField("defaultIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .SetValue(null, icon);
        }

    }

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
