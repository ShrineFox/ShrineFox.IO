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
            this.tlp_Main = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_NewText = new System.Windows.Forms.Label();
            this.txt_RenameText = new System.Windows.Forms.TextBox();
            this.tlp_Buttons = new System.Windows.Forms.TableLayoutPanel();
            this.btn_OK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tlp_Main.SuspendLayout();
            this.tlp_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlp_Main
            // 
            this.tlp_Main.ColumnCount = 2;
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.01047F));
            this.tlp_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.98953F));
            this.tlp_Main.Controls.Add(this.lbl_NewText, 0, 0);
            this.tlp_Main.Controls.Add(this.txt_RenameText, 1, 0);
            this.tlp_Main.Controls.Add(this.tlp_Buttons, 1, 1);
            this.tlp_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Main.Location = new System.Drawing.Point(2, 0);
            this.tlp_Main.Name = "tlp_Main";
            this.tlp_Main.RowCount = 2;
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Main.Size = new System.Drawing.Size(478, 151);
            this.tlp_Main.TabIndex = 0;
            // 
            // lbl_NewText
            // 
            this.lbl_NewText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_NewText.AutoSize = true;
            this.lbl_NewText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lbl_NewText.Location = new System.Drawing.Point(46, 27);
            this.lbl_NewText.Name = "lbl_NewText";
            this.lbl_NewText.Size = new System.Drawing.Size(84, 20);
            this.lbl_NewText.TabIndex = 0;
            this.lbl_NewText.Text = "New Text:";
            // 
            // txt_RenameText
            // 
            this.txt_RenameText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_RenameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_RenameText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txt_RenameText.Location = new System.Drawing.Point(136, 24);
            this.txt_RenameText.Name = "txt_RenameText";
            this.txt_RenameText.Size = new System.Drawing.Size(339, 26);
            this.txt_RenameText.TabIndex = 1;
            this.txt_RenameText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Txt_KeyDown);
            // 
            // tlp_Buttons
            // 
            this.tlp_Buttons.ColumnCount = 2;
            this.tlp_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Buttons.Controls.Add(this.btn_OK, 1, 0);
            this.tlp_Buttons.Controls.Add(this.button1, 0, 0);
            this.tlp_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlp_Buttons.Location = new System.Drawing.Point(136, 78);
            this.tlp_Buttons.Name = "tlp_Buttons";
            this.tlp_Buttons.RowCount = 1;
            this.tlp_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp_Buttons.Size = new System.Drawing.Size(339, 70);
            this.tlp_Buttons.TabIndex = 2;
            // 
            // btn_OK
            // 
            this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_OK.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_OK.Location = new System.Drawing.Point(172, 3);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(164, 64);
            this.btn_OK.TabIndex = 1;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.Save_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(163, 64);
            this.button1.TabIndex = 0;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // RenameForm
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(482, 153);
            this.Controls.Add(this.tlp_Main);
            this.DropShadowEffect = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.HeaderHeight = -40;
            this.MinimumSize = new System.Drawing.Size(500, 200);
            this.Name = "RenameForm";
            this.Opacity = 0.99D;
            this.Padding = new System.Windows.Forms.Padding(2, 0, 2, 2);
            this.ShowHeader = true;
            this.ShowLeftRect = false;
            this.ShowTitle = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Style = MetroSet_UI.Enums.Style.Dark;
            this.Text = "SET NAME";
            this.ThemeName = "MetroDark";
            this.tlp_Main.ResumeLayout(false);
            this.tlp_Main.PerformLayout();
            this.tlp_Buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tlp_Main;
        private Label lbl_NewText;
        private TextBox txt_RenameText;
        private TableLayoutPanel tlp_Buttons;
        private Button btn_OK;
        private Button button1;
    }
}