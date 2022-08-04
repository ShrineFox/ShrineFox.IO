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
    public class WinFormsControls
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
            control.UseAnimation = true;
            control.AnimateEasingType = EasingType.Linear;
            control.AnimateTime = 200;
            control.Speed = 5;

            return control;
        }

        public static ContextMenuStrip SFContextMenuStrip(TreeNode treeNode)
        {
            ContextMenuStrip rightClickMenu = new ContextMenuStrip();
            ToolStripMenuItem[] newMenuItems = new ToolStripMenuItem[] 
            {
                new ToolStripMenuItem() { Name = "rightClickMenu_New", Text = "New", Tag = "All" },
                new ToolStripMenuItem() { Name = "rightClickMenu_Replace", Text = "Open With...", Tag = "File" },
                new ToolStripMenuItem() { Name = "rightClickMenu_Replace", Text = "Replace", Tag = "File" },
                new ToolStripMenuItem() { Name = "rightClickMenu_Remove", Text = "Remove", Tag = "All" },
                new ToolStripMenuItem() { Name = "rightClickMenu_ShowInExplorer", Text = "Show in Explorer", Tag = "All" },
                new ToolStripMenuItem() { Name = "rightClickMenu_Copy", Text = "Copy", Tag = "All" },
                new ToolStripMenuItem() { Name = "rightClickMenu_Rename", Text = "Rename", Tag = "All" },
                new ToolStripMenuItem() { Name = "rightClickMenu_Expand", Text = "Expand", Tag = "Folder" },
                new ToolStripMenuItem() { Name = "rightClickMenu_Collapse", Text = "Collapse", Tag = "Folder" },
            };
            rightClickMenu.Items.AddRange(newMenuItems);

            // TODO: Hide visibility of options specific to a certain treeview
            // if (treeNode.TreeView.Name == "treeView_Files")

            return rightClickMenu;
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

        public static ContextMenuStrip SFContextMenuStrip(string name = "contextMenuStrip")
        {
            var control = new ContextMenuStrip();
            control.Name = name;
            // Theme
            control.BackColor = Color.FromArgb(30, 30, 30);
            control.ImageScalingSize = new System.Drawing.Size(20, 20);

            return control;
        }

        public static ToolStripMenuItem SFToolStripMenuItem(string name = "toolStripItem", string text = "", EventHandler clickEvent = null)
        {
            var control = new ToolStripMenuItem();
            control.Name = name;
            if (text != "")
                control.Text += text;
            if (clickEvent != null)
                control.Click += clickEvent;
            // Theme
            control.ForeColor = System.Drawing.Color.Silver;
            //control.AutoSize = false;
            control.BackColor = Color.FromArgb(30, 30, 30);
            control.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;

            return control;
        }
    }
}
