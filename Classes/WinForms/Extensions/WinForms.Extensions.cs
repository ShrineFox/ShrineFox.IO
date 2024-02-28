using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

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
        public static Color StringToColor(string colorStr)
        {
            TypeConverter cc = TypeDescriptor.GetConverter(typeof(Color));
            var result = (Color)cc.ConvertFromString(colorStr);
            return result;
        }
    }

    [ToolboxBitmap(typeof(System.Windows.Forms.TabControl))]
    public class TabControl : Dotnetrix.Controls.TabControl
    {
        private bool m_HideTabs = false;

        [DefaultValue(false)]
        [RefreshProperties(RefreshProperties.All)]
        public bool HideTabs
        {
            get { return m_HideTabs; }
            set
            {
                if (m_HideTabs == value) return;
                m_HideTabs = value;
                if (value == true) this.Multiline = true;
                this.UpdateStyles();
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public new bool Multiline
        {
            get
            {
                if (this.HideTabs) return true;
                return base.Multiline;
            }
            set
            {
                if (this.HideTabs)
                    base.Multiline = true;
                else
                    base.Multiline = value;
            }
        }

        public override System.Drawing.Rectangle DisplayRectangle
        {
            get
            {
                if (this.HideTabs)
                    return new Rectangle(0, 0, Width, Height);
                else
                {
                    int tabStripHeight, itemHeight;

                    if (this.Alignment <= TabAlignment.Bottom)
                        itemHeight = this.ItemSize.Height;
                    else
                        itemHeight = this.ItemSize.Width;

                    if (this.Appearance == TabAppearance.Normal)
                        tabStripHeight = 5 + (itemHeight * this.RowCount);
                    else
                        tabStripHeight = (3 + itemHeight) * this.RowCount;

                    switch (this.Alignment)
                    {
                        case TabAlignment.Bottom:
                            return new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
                        case TabAlignment.Left:
                            return new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
                        case TabAlignment.Right:
                            return new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
                        default:
                            return new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
                    }
                }
            }
        }
   
    }

    // https://stackoverflow.com/a/58658119
    static public class SyncUIHelper
    {
        static public Thread MainThread { get; private set; }

        // Must be called from Program.Main
        static public void Initialize()
        {
            MainThread = Thread.CurrentThread;
        }

        static public void SyncUI(this Control control, Action action, bool wait = true)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));
            if (!Thread.CurrentThread.IsAlive) throw new ThreadStateException();
            Exception exception = null;
            Semaphore semaphore = null;
            Action processAction = () =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            };
            Action processActionWait = () =>
            {
                processAction();
                semaphore?.Release();
            };
            if (control.InvokeRequired && Thread.CurrentThread != MainThread)
            {
                if (wait) semaphore = new Semaphore(0, 1);
                control.BeginInvoke(wait ? processActionWait : processAction);
                semaphore?.WaitOne();
            }
            else
                processAction();
            if (exception != null)
                throw exception;
        }

    }
}