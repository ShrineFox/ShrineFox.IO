using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public class WinFormsmDialogs
    {
        public static bool YesNoMsgBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            Output.Log($"{title}: {message}");
            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, icon) == DialogResult.Yes)
            {
                Output.Log($"{title}: {message}\n> Yes\n");
                return true;
            }
            else
            {
                Output.Log($"{title}: {message}\n> No\n");
                return false;
            }
        }

        public static bool OKCancelBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Warning)
        {
            Output.Log($"{title}: {message}");
            if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel, icon) == DialogResult.Yes)
            {
                Output.Log($"{title}: {message}\n> OK\n");
                return true;
            }
            else
            {
                Output.Log($"{title}: {message}\n> Cancel\n");
                return false;
            }
        }

        public static void OKMsgBox(string title, string message, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
            Output.Log($"{title}: {message}");
        }
    }
}
