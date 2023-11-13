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
using MetroSet_UI.Forms;
using MetroSet_UI.Enums;

namespace ShrineFox.IO
{
    public class Theme
    {
        public static Style ThemeStyle { get; set; } = Style.Dark;
        public static Color DarkBG { get; set; } = Color.FromArgb(30, 30, 30);
        public static Color DarkText { get; set; } = Color.FromArgb(220, 220, 220);
        public static Color LightBG { get; set; } = Color.FromArgb(240, 240, 240);
        public static Color LightText { get; set; } = Color.FromArgb(20, 20, 20);

        public static void ApplyToForm(MetroSetForm form)
        {
            // Set MetroSetUI Style for form
            form.Style = ThemeStyle;

            // Set form's menustrip renderer
            SetMenuRenderer(form.MainMenuStrip);

            // Set forecolor and backcolor of each control in form
            SetFormControlColors(form);

        }

        private static void SetFormControlColors(MetroSetForm form)
        {
            foreach (var control in WinForms.EnumerateControls(form))
            {
                dynamic ctrl = control as dynamic;

                SetColorProperties(ctrl);

                if (ctrl.GetType() == typeof(MenuStrip) || ctrl.GetType() == typeof(ContextMenuStrip))
                    RecursivelySetColors(ctrl);
            }
        }

        private static void SetColorProperties(dynamic ctrl)
        {
            if (WinForms.PropertyExists(ctrl, "Style") && ctrl.GetType() != typeof(ProgressBar))
                ctrl.Style = ThemeStyle;

            if (ThemeStyle == Style.Dark)
            {
                if (WinForms.PropertyExists(ctrl, "ForeColor"))
                    ctrl.ForeColor = DarkText;
                if (WinForms.PropertyExists(ctrl, "BackColor"))
                    ctrl.BackColor = DarkBG;
                if (WinForms.PropertyExists(ctrl, "BackgroundColor"))
                    ctrl.BackgroundColor = DarkBG;
                if (WinForms.PropertyExists(ctrl, "BorderColor"))
                    ctrl.BackColor = DarkBG;
            }
            else
            {
                if (WinForms.PropertyExists(ctrl, "ForeColor"))
                    ctrl.ForeColor = LightText;
                if (WinForms.PropertyExists(ctrl, "BackColor"))
                    ctrl.BackColor = LightBG;
                if (WinForms.PropertyExists(ctrl, "BackgroundColor"))
                    ctrl.BackgroundColor = LightBG;
                if (WinForms.PropertyExists(ctrl, "BorderColor"))
                    ctrl.BackColor = LightBG;
            }
        }

        public static void SetMenuRenderer(dynamic ctrl)
        {
            if (ctrl == null)
                return;

            if (ThemeStyle == Style.Dark)
                ctrl.Renderer = new DarkMenuRenderer();
            else
                ctrl.Renderer = new ToolStripProfessionalRenderer();
        }

        public static void RecursivelySetColors(dynamic ctrl)
        {
            SetColorProperties(ctrl);

            if (ctrl.GetType() == typeof(MenuStrip))
                foreach (dynamic item in ctrl.Items)
                    RecursivelySetColors(item);
            else if (ctrl.GetType() == typeof(ContextMenuStrip))
            {
                SetMenuRenderer(ctrl);
                foreach (dynamic item in ctrl.Items)
                    RecursivelySetColors(item);
            }
            else if (ctrl.GetType() == typeof(ToolStripMenuItem) || ctrl.GetType() == typeof(ToolStripItem))
                foreach (dynamic item in ctrl.DropDownItems)
                {
                    RecursivelySetColors(item);
                }
        }

        public class DarkMenuRenderer : ToolStripProfessionalRenderer
        {
            public DarkMenuRenderer() : base(new CustomColors()) { }
        }
        public class CustomColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return DarkBG; }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return DarkBG; }
            }
            public override Color SeparatorDark
            {
                get { return DarkBG; }
            }
            public override Color SeparatorLight
            {
                get { return DarkBG; }
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color MenuStripGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color MenuStripGradientEnd
            {
                get { return DarkBG; }
            }

            public override Color ButtonCheckedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ButtonCheckedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ButtonCheckedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ButtonCheckedHighlight
            {
                get { return DarkBG; }
            }

            public override Color ButtonPressedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ButtonPressedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ButtonPressedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ButtonPressedHighlight
            {
                get { return DarkBG; }
            }

            public override Color ButtonSelectedGradientMiddle
            {
                get { return DarkBG; }
            }

            public override Color ButtonSelectedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ButtonSelectedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ButtonSelectedHighlight
            {
                get { return DarkBG; }
            }

            public override Color OverflowButtonGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color OverflowButtonGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color OverflowButtonGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color CheckBackground
            {
                get { return DarkBG; }
            }
            public override Color CheckPressedBackground
            {
                get { return DarkBG; }
            }
            public override Color CheckSelectedBackground
            {
                get { return DarkBG; }
            }
            public override Color ToolStripContentPanelGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ToolStripContentPanelGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ToolStripGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ToolStripGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ToolStripGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ToolStripPanelGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ToolStripPanelGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color GripDark
            {
                get { return DarkBG; }
            }
            public override Color GripLight
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginRevealedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginRevealedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginRevealedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color MenuItemPressedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color RaftingContainerGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color RaftingContainerGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color StatusStripGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color StatusStripGradientEnd
            {
                get { return DarkBG; }
            }
        }
    }
}