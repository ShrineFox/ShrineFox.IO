using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class WinFormsEvents
    {
        public static string FolderPath_Click(string title = "Choose Folder...")
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = title;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return dialog.FileName;
            return "";
        }

        public static string FilePath_Click(string title = "Choose File...")
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = title;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return dialog.FileName;
            return "";
        }
    }
}
