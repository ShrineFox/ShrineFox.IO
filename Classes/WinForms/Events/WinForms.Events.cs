using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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

        public static List<string> FilePath_Click(string title = "Choose File...", bool multiSelect = false, 
            string[] filters = null)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = title;
            dialog.Multiselect = multiSelect;
            if (filters != null)
                foreach (var filter in filters)
                {
                    string[] filterParts = filter.Split('(');
                    dialog.Filters.Add(new CommonFileDialogFilter(filterParts[0].TrimEnd(), filterParts[1].TrimEnd(')')));
                }
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return new List<string>(dialog.FileNames);
            return new List<string>();
        }
    }
}
