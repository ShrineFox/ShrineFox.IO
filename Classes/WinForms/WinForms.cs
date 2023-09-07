using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class WinForms
    {
        public static dynamic GetControl(Form form, string controlName)
        {
            var ctrls = form.Controls.Find(controlName, true);
            if (ctrls.Length < 1)
                MessageBox.Show($"Form does not contain control named \"{controlName}\"", "Control Not Found");
            return ctrls.Single();
        }

        public static dynamic GetControl(Control control, string controlName)
        {
            return GetControl(control.FindForm(), controlName);
        }

        public static List<Control> EnumerateControls(Form form)
        {
            List<Control> controls = new List<Control>();
            foreach (Control control in form.Controls)
            {
                controls.Add(control);
                if (control.Controls != null)
                    controls.AddRange(EnumerateChildren(control));
            }
            return controls;
        }

        public static List<Control> EnumerateChildren(Control root)
        {
            List<Control> controls = new List<Control>();
            foreach (Control control in root.Controls)
            {
                controls.Add(control);
                if (control.Controls != null)
                    controls.AddRange(EnumerateChildren(control));
            }
            return controls;
        }

        public static void SetControlValue(Control control, string controlText)
        {
            if (control.GetType() == typeof(TextBox))
            {
                var ctrl = (TextBox)control;
                ctrl.Text = controlText;
            }
            else if (control.GetType() == typeof(ComboBox))
            {
                var ctrl = (ComboBox)control;
                ctrl.SelectedIndex = ctrl.Items.IndexOf(controlText);
            }
        }

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
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
}
