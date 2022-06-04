using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    public partial class FormCreator : SFForm
    {
        public FormCreator()
        {
            SetupCreatorForm();
        }

        private void SetupCreatorForm()
        {
            SetupOutsideControls();
        }

        private void SetupOutsideControls()
        {
            BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            ForeColor = System.Drawing.Color.Silver;

            // Create TableLayoutPanel to separate form content and buttons
            TableLayoutPanel tlp_Main = new TableLayoutPanel() { BackColor = BackColor, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            // Add buttons to bottom of TableLayoutPanel
            TableLayoutPanel tlp_Buttons = new TableLayoutPanel() { BackColor = BackColor, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            Button btnCancel = new Button() { Name = "btn_Cancel", BackColor = BackColor, ForeColor = ForeColor, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, Dock = DockStyle.Fill };
            btnCancel.Text = "Cancel";
            tlp_Buttons.Controls.Add(btnCancel, 1, 0);
            Button btnSave = new Button() { Name = "btn_Save", BackColor = BackColor, ForeColor = ForeColor, FlatStyle = FlatStyle.Flat, Dock = DockStyle.Fill };
            btnSave.Text = "Save";
            tlp_Buttons.Controls.Add(btnSave, 2, 0);
            tlp_Main.Controls.Add(tlp_Buttons, 0, 1);

            // Create panel to hold Content TableLayoutPanel
            Panel panel = new Panel() { BackColor = System.Drawing.Color.FromArgb(0, 122, 204), Dock = DockStyle.Fill, AutoScroll = true, AutoSize = false, BorderStyle = BorderStyle.Fixed3D };
            TableLayoutPanel tlp_Content = new TableLayoutPanel() { Name = "tlp_Content", BackColor = BackColor, Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, AutoScroll = false, Padding = new Padding(2, 20, 2, 2) };
            tlp_Content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlp_Content.RowStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            // Context Menu When Right Clicking the Content TableLayoutPanel
            tlp_Content.Click += Content_Clicked();

            // Add controls to form
            panel.Controls.Add(tlp_Content);
            tlp_Main.Controls.Add(panel);
            Controls.Add(tlp_Main);
        }

        private EventHandler Content_Clicked()
        {
            throw new NotImplementedException();
            // Create context menu for control being clicked on
            // i.e. "Add Cell Left," "Remove Cell," "Add Control" ...
        }
    }
}
