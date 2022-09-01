using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Drawing;
using System.Linq;
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
