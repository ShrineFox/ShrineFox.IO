using MetroSet_UI.Controls;
using MetroSet_UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using MetroSet_UI.Enums;
using System.Runtime.InteropServices;

namespace ShrineFox.IO
{
    public class SFControls
    {
        public static MetroSetTabControl SFTabControl()
        {
            var control = new MetroSetTabControl();
            // Layout
            control.Dock = DockStyle.Fill;
            control.SelectedIndex = 0;
            control.SizeMode = TabSizeMode.Fixed;
            // Theme
            control.SelectedTextColor = Color.White;
            control.UnselectedTextColor = Color.Gray;
            control.Style = Style.Dark;
            control.BackgroundColor = Color.FromArgb(30, 30, 30);
            control.Cursor = Cursors.Hand;
            control.TabStyle = TabStyle.Style1;
            // Animation
            control.UseAnimation = false;
            //control.AnimateEasingType = EasingType.Linear;
            //control.AnimateTime = 50;
            //control.Speed = 5;

            return control;
        }

        public static TabPage SFTabPage(string name = "tabPage", string text = "Tab Page", int tabIndex = 0)
        {
            var control = new TabPage();
            control.Name = name;
            // Layout
            control.Dock = DockStyle.Fill;
            control.TabIndex = tabIndex;
            // Theme
            control.BackColor = Color.FromArgb(20, 20, 20);
            control.Text = text;

            return control;
        }

        public static TreeView SFTreeView(string name = "treeView", int tabIndex = 0, TreeNodeMouseClickEventHandler clickEvent = null, TreeNodeMouseClickEventHandler doubleClickEvent = null)
        {
            var control = new TreeView();
            control.Name = name;
            // Layout
            control.Dock = DockStyle.Fill;
            control.BorderStyle = BorderStyle.None;
            control.TabIndex = tabIndex;
            // Theme
            control.BackColor = Color.FromArgb(20, 20, 20);
            control.ForeColor = Color.Silver;
            control.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            // Events
            if (clickEvent != null)
                control.NodeMouseClick += clickEvent;
            if (doubleClickEvent != null)
                control.NodeMouseDoubleClick += doubleClickEvent;

            return control;
        }

        public static Panel SFPanel(string name = "panel", int tabIndex = 0)
        {
            var control = new Panel();
            control.Name = name;
            // Layout
            control.Dock = DockStyle.Fill;
            control.TabIndex = tabIndex;
            // Theme
            control.BackColor = Color.FromArgb(20, 20, 20);

            return control;
        }
    }
}
