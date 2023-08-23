using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class WinFormsDialogs
    {
        /// <summary>
        /// Shows a message box. Returns a positive selection as true, otherwise false.
        /// </summary>
        /// <param name="title">Title of the message box.</param>
        /// <param name="message">Message displayed within window.</param>
        /// <param name="buttons">The type of buttons to display in the box.</param>
        /// <param name="icon">Image displayed alongside message.</param>
        /// <returns></returns>
        public static bool ShowMessageBox(string title, string message, MessageBoxButtons buttons = MessageBoxButtons.OKCancel, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            var result = MessageBox.Show(message, title, buttons, icon);
            if (result == DialogResult.Yes || result == DialogResult.OK || result == DialogResult.Retry)
                return true;

            return false;
        }

        /// <summary>
        /// Prompts a user to select a folder, then returns the full path of the selected file.
        /// Returned string will be empty if selection is cancelled.
        /// </summary>
        /// <param name="title">The title of the folder picker window.</param>
        /// <returns></returns>
        public static string SelectFolder(string title = "Choose Folder...")
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = title;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                return dialog.FileName;
            return "";
        }

        /// <summary>
        /// Prompts a user to select file(s), then returns a list of selected paths.
        /// Returned list will be empty if selection is cancelled.
        /// </summary>
        /// <param name="title">The title of the file picker window.</param>
        /// <param name="multiSelect">Whether the user can pick multiple files.</param>
        /// <param name="filters">List of allowed file extensions i.e. "Executable (.exe)"</param>
        /// <param name="save">If true, the confirm button will say Save. Otherwise, Open.</param>
        /// <returns></returns>
        public static List<string> SelectFile(string title = "Choose File...", bool multiSelect = false,
            string[] filters = null, bool save = false)
        {
            if (save)
            {
                CommonSaveFileDialog dialog = new CommonSaveFileDialog();

                dialog.Title = title;
                if (filters != null)
                    foreach (var filter in filters)
                    {
                        string[] filterParts = filter.Split('(');
                        dialog.Filters.Add(new CommonFileDialogFilter(filterParts[0].TrimEnd(), filterParts[1].TrimEnd(')')));
                    }
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    return new List<string>() { dialog.FileName };
            }
            else
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
            }

            return new List<string>() { "" };
        }
    }
}
