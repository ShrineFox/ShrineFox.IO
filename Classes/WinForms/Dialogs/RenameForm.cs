using DarkUI.Forms;
using System;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public partial class RenameForm : DarkForm
    {
        public string RenameText
        {
            get;
            private set;
        }

        public RenameForm(string text)
        {
            InitializeComponent();
            this.txt_NewName.Text = text;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            RenameText = txt_NewName.Text;
            this.DialogResult = DialogResult.OK;
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}