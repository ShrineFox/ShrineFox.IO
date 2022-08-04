using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class WinFormsEvents
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
