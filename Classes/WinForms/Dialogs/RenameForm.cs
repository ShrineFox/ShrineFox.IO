using MetroSet_UI.Forms;
using System;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public partial class RenameForm : MetroSetForm
    {
        public string RenameText
        {
            get;
            private set;
        }

        public RenameForm(string title, string text)
        {
            InitializeComponent();
            Theme.ApplyToForm(this);

            this.Text = title;
            this.txt_RenameText.Text = text;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            RenameText = txt_RenameText.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}