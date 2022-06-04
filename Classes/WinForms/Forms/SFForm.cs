using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Remoting;

namespace ShrineFox.IO
{
    public partial class SFForm : MetroSet_UI.Forms.MetroSetForm
    {
        Config config;

        public SFForm(string formName = "", string formJson = "", string userJson = "")
        {
            // Set Form Name
            if (formName != "")
                Name = formName;
            else
                Name = Exe.Name();
            Text = Name;

            // Attempt to load config
            config = new Config(userJson, formJson);

            SetupForm();
        }

        private void SetupForm()
        {
            SetupTheme();
            WinForms.SetDefaultIcon();
            SetTreeviewImages();
            SetupLogging();

            config.Load();
            SetupLayout();
        }

        private void SetupTheme()
        {
            // Make MetroSetUI look presentable
            Style = MetroSet_UI.Enums.Style.Dark;
            Padding = new Padding(0);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundColor = System.Drawing.Color.FromArgb(30, 30, 30);
            BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            DropShadowEffect = false;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new System.Drawing.Size(640, 360);
            Opacity = 0D;
            HeaderHeight = -90;
            ShowHeader = true;
            ShowLeftRect = false;
            SizeGripStyle = SizeGripStyle.Show;
        }

        private void SetupLayout()
        {
            SetupMainControls();

            if (config.FormSettings != null)
            {
                SetupFormDetails();
                SetupDynamicControls();
            }

            //SetupToolstripRenderer();
        }

        private void SetupFormDetails()
        {
            Name = config.FormSettings["Form"].Value<string>("Name");
            Text = config.FormSettings["Form"].Value<string>("Text");
            Width = config.FormSettings["Form"].Value<int>("Width");
            Height = config.FormSettings["Form"].Value<int>("Height");
        }

        private void SetupDynamicControls()
        {
            foreach (JProperty ctrl in config.FormSettings["Form"]["Controls"])
                AddControls(ctrl, this);
        }

        private void AddControls(JProperty ctrls, dynamic parent)
        {
            foreach (JObject ctrl in ctrls)
                AddControl(ctrl, parent);
        }

        private void AddControl(JObject ctrl, dynamic parent)
        {
            // Get name of parent control
            string parentName = "";
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(parent))
                if (prop.Name == "Name")
                    parentName = prop.GetValue(parent);
            // Get type of parent control
            // Type parentType = ((ObjectHandle)parent).Unwrap().GetType();

            // Get name of control being created
            var parentNode = (JProperty)ctrl.Parent;
            string ctrlName = parentNode.Name;
            // Get type of control being created
            string typeName = ctrl.Value<string>("ControlType");
            Type type = Exe.GetType(typeName);
            // Create new control instance from type
            dynamic newCtrl = Exe.GetInstance(type);
            int row = -1;
            int column = -1;

            // For each property of control in json...
            foreach (JProperty prop in ctrl.Properties())
            {
                // Set Rows/Columns that this control has
                if (prop.Name == "Rows" && type == typeof(TableLayoutPanel))
                    foreach (var value in (JArray)prop.Value)
                        newCtrl.RowStyles.Add(new RowStyle(SizeType.Percent, value.Value<int>()));
                else if (prop.Name == "Columns" && type == typeof(TableLayoutPanel))
                    foreach (var value in (JArray)prop.Value)
                        newCtrl.RowStyles.Add(new ColumnStyle(SizeType.Percent, value.Value<int>()));
                // Set Row/Column of parent control that this control gets added to
                if (prop.Name == "Row")
                    row = Convert.ToInt32(prop.Value.ToString());
                if (prop.Name == "Column")
                    column = Convert.ToInt32(prop.Value.ToString());

                // For each property of control type...
                foreach (var typeProp in type.GetProperties())
                {
                    // Set Name
                    if (typeProp.Name == "Name")
                        typeProp.SetValue(newCtrl, ctrlName);
                    // If the names match...
                    if (typeProp.Name == prop.Name)
                    {
                        if (typeProp.Name == "Controls")
                            foreach (JProperty nestedCtrl in ctrl["Controls"])
                                AddControls(nestedCtrl, newCtrl);
                        else
                        {
                            if (typeProp.PropertyType == typeof(string))
                                typeProp.SetValue(newCtrl, prop.Value.ToString());
                            else if (typeProp.PropertyType == typeof(int))
                                typeProp.SetValue(newCtrl, Convert.ToInt32(prop.Value.ToString()));
                            else if (typeProp.PropertyType == typeof(bool))
                                typeProp.SetValue(newCtrl, Convert.ToBoolean(prop.Value.ToString()));
                        }
                    }
                    // Set Theme Stuff
                    if (typeProp.Name == "BackColor")
                        typeProp.SetValue(newCtrl, BackColor);
                    if (typeProp.Name == "ForeColor")
                        typeProp.SetValue(newCtrl, ForeColor);
                }
            }

            // Log info about controls being added
            string log = $"Adding {typeName.Replace("System.Windows.Forms.", "")} \"{ctrlName}\"" +
                $"\n                        to Control \"{parentName}\"";
            if (row != -1 || column != -1)
                log += $" (column {column}, row {row})";
            Output.VerboseLog(log, ConsoleColor.Yellow);

            // Add to parent object depending on type
            if (type == typeof(ToolStripMenuItem))
                parent.Items.Add(newCtrl);
            else if (row != -1 || column != -1)
                parent.Controls.Add(newCtrl, column, row);
            else
                parent.Controls.Add(newCtrl);
        }

        private void SetupMainControls()
        {
            // Create main TableLayoutPanel
            TableLayoutPanel tlp_Main = new TableLayoutPanel() { Name = "tlp_Main", BackColor = BackColor, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Create panel to hold Content TableLayoutPanel
            Panel panel = new Panel() { BackColor = BackColor, Dock = DockStyle.Fill, AutoScroll = true, AutoSize = false };
            TableLayoutPanel tlp_Content = new TableLayoutPanel() { Name = "tlp_Content", BackColor = BackColor, Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, AutoScroll = false };
            tlp_Content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tlp_Content.RowStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Add controls to form
            panel.Controls.Add(tlp_Content);
            tlp_Main.Controls.Add(panel);
            Controls.Add(tlp_Main);
        }

        public void SetupToolstripRenderer()
        {
            ToolStripManager.Renderer = WinFormsExtensions.renderer;
            MenuStrip menuStrip = WinForms.GetControl(this, "menuStrip_Main");
            menuStrip.Renderer = WinFormsExtensions.renderer;
        }

        private void SetTreeviewImages()
        {
            string iconPath = Path.Combine(Exe.Directory(), "Icons");

            TreeViewBuilder.SetIcon(Path.Combine(iconPath, "page_white.png"), ".file");
            TreeViewBuilder.SetIcon(Path.Combine(iconPath, "folder.png"), ".folder");
        }

        private void SetupLogging()
        {
            if (!Output.HasMainWindow())
                Output.AllocConsole();

            Output.LogPath = "log.txt";
            //Output.LogControl = WinForms.GetControl(this, "richTextBox_OutputLog");
            #if DEBUG
                Output.VerboseLogging = true;
            #endif

            Output.VerboseLog("Program started.", ConsoleColor.Gray);
            Output.Log("Create a new project or load an existing one to get started.", ConsoleColor.Green);
            Output.Log("Log test.");
        }
    }
}
