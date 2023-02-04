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
        public static bool YesNoMsgBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            Output.VerboseLog($"[MessageBox] {title}: {message}");
            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon) == DialogResult.Yes)
            {
                Output.VerboseLog("[Result] Yes\n");
                return true;
            }
            else
            {
                Output.VerboseLog("[Result] No\n");
                return false;
            }
        }

        public static bool OKCancelBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            Output.VerboseLog($"[MessageBox] {title}: {message}");
            if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel, icon) == DialogResult.Yes)
            {
                Output.VerboseLog("[Result] OK\n");
                return true;
            }
            else
            {
                Output.VerboseLog("[Result] Cancel\n");
                return false;
            }
        }

        public static void OKMsgBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            Output.VerboseLog($"[MessageBox] {title}: {message}");
            if (MessageBox.Show(message, title, MessageBoxButtons.OK, icon) == DialogResult.OK)
                Output.VerboseLog("[Result] OK\n");
        }
    }
}
