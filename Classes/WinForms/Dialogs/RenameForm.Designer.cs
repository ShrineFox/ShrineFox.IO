using MetroSet_UI.Controls;
using MetroSet_UI.Forms;
using System.Windows.Forms;

namespace ShrineFox.IO
{
    partial class RenameForm : MetroSetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tlp_Main = new TableLayoutPanel();
            tlp_Buttons = new TableLayoutPanel();
            btn_Cancel = new Button();
            btn_Save = new Button();
            tlp_ProjectName = new TableLayoutPanel();
            txt_NewName = new TextBox();
            lbl_ProjectName = new MetroSetLabel();
            tlp_Fields = new TableLayoutPanel();
            darkTextBox1 = new TextBox();
            MetroSetLabel1 = new MetroSetLabel();
            this.tlp_Main.SuspendLayout();
            tlp_Buttons.SuspendLayout();
            tlp_ProjectName.SuspendLayout();
            tlp_Fields.SuspendLayout();
            // 
            // tlp_Main
            // 
            this.tlp_Main.ColumnCount = 1;
            this.tlp_Main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tlp_Main.Controls.Add(tlp_Fields, 0, 0);
            this.tlp_Main.Controls.Add(tlp_Buttons, 0, 1);
            this.tlp_Main.Dock = DockStyle.Fill;
            this.tlp_Main.Margin = new Padding(2);
            this.tlp_Main.Name = "tlp_Main";
            this.tlp_Main.RowCount = 2;
            this.tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 65F));
            this.tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
            this.tlp_Main.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            this.tlp_Main.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            this.tlp_Main.TabIndex = 28;
            // 
            // tlp_Buttons
            // 
            tlp_Buttons.ColumnCount = 3;
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlp_Buttons.Controls.Add(btn_Cancel, 1, 0);
            tlp_Buttons.Controls.Add(btn_Save, 2, 0);
            tlp_Buttons.Dock = DockStyle.Fill;
            tlp_Buttons.Margin = new Padding(2);
            tlp_Buttons.Name = "tlp_Buttons";
            tlp_Buttons.RowCount = 1;
            tlp_Buttons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlp_Buttons.TabIndex = 34;
            // 
            // btn_Cancel
            // 
            btn_Cancel.DialogResult = DialogResult.Cancel;
            btn_Cancel.Dock = DockStyle.Fill;
            btn_Cancel.Margin = new Padding(2);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Padding = new Padding(3, 4, 3, 4);
            btn_Cancel.TabIndex = 0;
            btn_Cancel.Text = "Cancel";
            btn_Cancel.Click += Cancel_Click;
            // 
            // btn_Save
            // 
            btn_Save.Dock = DockStyle.Fill;
            btn_Save.Margin = new Padding(2);
            btn_Save.Name = "btn_Save";
            btn_Save.Padding = new Padding(3, 4, 3, 4);
            btn_Save.TabIndex = 1;
            btn_Save.Text = "Save";
            btn_Save.Click += Save_Click;
            // 
            // tlp_ProjectName
            // 
            tlp_ProjectName.ColumnCount = 2;
            tlp_ProjectName.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlp_ProjectName.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tlp_ProjectName.Controls.Add(txt_NewName, 1, 0);
            tlp_ProjectName.Dock = DockStyle.Fill;
            tlp_ProjectName.Margin = new Padding(2);
            tlp_ProjectName.Name = "tlp_ProjectName";
            tlp_ProjectName.RowCount = 1;
            tlp_ProjectName.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlp_ProjectName.TabIndex = 0;
            // 
            // txt_NewName
            // 
            txt_NewName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txt_NewName.BorderStyle = BorderStyle.FixedSingle;
            txt_NewName.Margin = new Padding(2);
            txt_NewName.Name = "txt_NewName";
            txt_NewName.TabIndex = 26;
            // 
            // lbl_ProjectName
            // 
            lbl_ProjectName.Anchor = AnchorStyles.Right;
            lbl_ProjectName.Margin = new Padding(2, 0, 2, 0);
            lbl_ProjectName.Name = "lbl_ProjectName";
            lbl_ProjectName.TabIndex = 25;
            lbl_ProjectName.Text = "New Name:";
            // 
            // tlp_Fields
            // 
            tlp_Fields.ColumnCount = 2;
            tlp_Fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlp_Fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tlp_Fields.Controls.Add(darkTextBox1, 1, 0);
            tlp_Fields.Controls.Add(MetroSetLabel1, 0, 0);
            tlp_Fields.Dock = DockStyle.Fill;
            tlp_Fields.Margin = new Padding(2);
            tlp_Fields.Name = "tlp_Fields";
            tlp_Fields.RowCount = 1;
            tlp_Fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlp_Fields.TabIndex = 35;
            // 
            // darkTextBox1
            // 
            darkTextBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            darkTextBox1.BorderStyle = BorderStyle.FixedSingle;
            darkTextBox1.Margin = new Padding(2);
            darkTextBox1.Name = "darkTextBox1";
            darkTextBox1.TabIndex = 26;
            // 
            // MetroSetLabel1
            // 
            MetroSetLabel1.Anchor = AnchorStyles.Right;
            MetroSetLabel1.Margin = new Padding(2, 0, 2, 0);
            MetroSetLabel1.Name = "MetroSetLabel1";
            MetroSetLabel1.TabIndex = 25;
            MetroSetLabel1.Text = "New Name:";
            //
            // RenameForm
            //
            this.Name = "Rename";
            this.Text = "Set Name";
        }

        #endregion
        private TableLayoutPanel tlp_Main;
        private TableLayoutPanel tlp_Settings;
        private TableLayoutPanel tlp_Buttons;
        private Button btn_Cancel;
        private Button btn_Save;
        private TableLayoutPanel tlp_Fields;
        private TextBox darkTextBox1;
        private MetroSetLabel MetroSetLabel1;
        private TableLayoutPanel tlp_ProjectName;
        private TextBox txt_NewName;
        private MetroSetLabel lbl_ProjectName;
    }
}