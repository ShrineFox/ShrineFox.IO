using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
