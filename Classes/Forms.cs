using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public static Form SettingsForm()
        {
            Form settingsForm = new Form() { };
            settingsForm.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            settingsForm.ForeColor = System.Drawing.Color.Silver;
            // Create TableLayoutPanel to separate form content and buttons
            TableLayoutPanel tlp_Main = new TableLayoutPanel() { BackColor = settingsForm.BackColor, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            // Add buttons to bottom of TableLayoutPanel
            TableLayoutPanel tlp_Buttons = new TableLayoutPanel() { BackColor = settingsForm.BackColor, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            Button btnCancel = new Button() { BackColor = settingsForm.BackColor, ForeColor = settingsForm.ForeColor, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, Dock = DockStyle.Fill };
            btnCancel.Text = "Cancel";
            tlp_Buttons.Controls.Add(btnCancel, 1, 0);
            Button btnSave = new Button() { Name = "btn_Save", BackColor = settingsForm.BackColor, ForeColor = settingsForm.ForeColor, FlatStyle = FlatStyle.Flat, Dock = DockStyle.Fill };
            btnSave.Text = "Save";
            tlp_Buttons.Controls.Add(btnSave, 2, 0);
            tlp_Main.Controls.Add(tlp_Buttons, 0, 1);
            // Create panel to hold content TableLayoutPanel
            Panel panel = new Panel() { BackColor = settingsForm.BackColor, Dock = DockStyle.Fill, AutoScroll = true, AutoSize = false };
            TableLayoutPanel tlp_Content = new TableLayoutPanel() { Name = "tlp_Content", BackColor = settingsForm.BackColor, Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, AutoScroll = false };
            tlp_Content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            tlp_Content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            // Add controls to form
            panel.Controls.Add(tlp_Content);
            tlp_Main.Controls.Add(panel);
            settingsForm.Controls.Add(tlp_Main);

            return settingsForm;
        }

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

    public class SFTextBox : TextBox
    {
        const int WM_NCPAINT = 0x85;
        const uint RDW_INVALIDATE = 0x1;
        const uint RDW_IUPDATENOW = 0x100;
        const uint RDW_FRAME = 0x400;
        [DllImport("user32.dll")]
        static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprc, IntPtr hrgn, uint flags);
        Color borderColor = Color.Blue;
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                    RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);
            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCPAINT && BorderColor != Color.Transparent)
            {
                var hdc = GetWindowDC(this.Handle);
                using (var g = Graphics.FromHdcInternal(hdc))
                { 
                    using (var p = new Pen(BorderColor)) g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1)); 
                    using (var b = new Pen(BackColor)) g.DrawRectangle(b, new Rectangle(1, 1, Width - 3, Height - 3)); 
                }
                ReleaseDC(this.Handle, hdc);
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                   RDW_FRAME | RDW_IUPDATENOW | RDW_INVALIDATE);
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
