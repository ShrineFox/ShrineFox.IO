using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public partial class SettingsForm : SFForm
    {
        public SettingsForm()
        {
            SetupSettingsForm();
        }

        private void SetupSettingsForm()
        {
            SetupOutsideControls();
        }

        private void SetupOutsideControls()
        {

        }

        private EventHandler Content_Clicked()
        {
            throw new NotImplementedException();
            // Create context menu for control being clicked on
            // i.e. "Add Cell Left," "Remove Cell," "Add Control" ...
        }
    }
}
