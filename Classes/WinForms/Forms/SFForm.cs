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
using System.Drawing;

namespace ShrineFox.IO
{
    public partial class SFForm : MetroSet_UI.Forms.MetroSetForm
    {
        Config config;
        
        public SFForm(string formName = "", string formJson = "FormSettings\\MainForm.json", string userJson = "Saved\\MainUserData.json")
        {
            if (formName != "")
                Name = formName;
            else
                Name = Exe.Name();
            Text = Name;
            config = new Config(userJson, formJson);

            SetupForm();
        }

        private void SetupForm()
        {
            // Set up default form appearance
            SetupTheme();
            WinForms.SetDefaultIcon();
            SetTreeviewImages();
            SetupLogging();

            // Load user generated layout
            config.Load();
            if (config.FormSettings != null)
            {
                var formToken = config.FormSettings["Form"];
                SetupFormDetails(formToken);
                AddControls(formToken, this);
            }

            Output.Log("Form Loaded.");
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
            HeaderHeight = -50;
            ShowHeader = true;
            ShowLeftRect = false;
            SizeGripStyle = SizeGripStyle.Show;
        }

        private void SetupFormDetails(JToken form)
        {
            Name = form.Value<string>("Name");
            Text = form.Value<string>("Text");
            Width = Convert.ToInt32(form.Value<string>("Width"));
            Height = Convert.ToInt32(form.Value<string>("Height"));
            MinimumSize = new System.Drawing.Size(Convert.ToInt32(form.Value<string>("MinWidth")), 
                Convert.ToInt32(form.Value<string>("MinHeight")));
            MaximumSize = new System.Drawing.Size(Convert.ToInt32(form.Value<string>("MaxWidth")), 
                Convert.ToInt32(form.Value<string>("MaxHeight")));
        }

        private void AddControls(JToken ctrlToken, dynamic parent)
        {
            foreach (var subCtrl in ctrlToken["Controls"])
            {
                var ctrl = subCtrl.Children().First().Value<JObject>();
                // Get name of parent control
                string parentName = GetJsonCtrlName(parent);
                // Get type of parent control
                Type parentType = parent.GetType();

                // Get name of control being created
                string ctrlName = GetJsonCtrlParent(ctrl).Name;
                // Get type of control being created
                Type type = GetJsonCtrlType(ctrl);
                // Create new control instance from type
                dynamic newCtrl = Exe.GetInstance(type);

                int row = -1;
                int column = -1;

                // Get a list of the control's Type properties
                PropertyInfo[] typeProperties = type.GetProperties();
                // For each property of the control in JSON...
                foreach (JProperty jsonProperty in ctrl.Properties())
                {
                    // If there's a Type property with the same name...
                    if (typeProperties.Any(x => x.Name.Equals(jsonProperty.Name)))
                    {
                        // Get the first JSON property that matches
                        var typeProperty = typeProperties.First(x => x.Name.Equals(jsonProperty.Name));

                        // Assign property value to control
                        switch (jsonProperty.Name)
                        {
                            case "Name":
                                typeProperty.SetValue(newCtrl, ctrlName);
                                break;
                            case "Margin":
                            case "Padding":
                                SetPadding(typeProperty, jsonProperty, newCtrl);
                                break;
                            case "MaximumSize":
                            case "MinimumSize":
                                SetSize(typeProperty, jsonProperty, newCtrl);
                                break;
                            case "Controls":
                                AddControls(ctrl, newCtrl);
                                break;
                            case "BackColor":
                            case "ForeColor":
                                typeProperty.SetValue(newCtrl, WinFormsExtensions.StringToColor(jsonProperty.Value.ToString()));
                                break;
                            default:
                                SetCtrlProperty(typeProperty, jsonProperty, newCtrl);
                                break;
                        }
                    }
                    else
                    {
                        // Assign certain properties to the control even if names don't match
                        switch (jsonProperty.Name)
                        {
                            case "Row":
                                row = GetPropValueAsInt(jsonProperty);
                                break;
                            case "Column":
                                column = GetPropValueAsInt(jsonProperty);
                                break;
                            case "Rows":
                                CreateRows(newCtrl, jsonProperty);
                                break;
                            case "Columns":
                                CreateColumns(newCtrl, jsonProperty);
                                break;
                            case "Events":
                                CreateEventHandlers(newCtrl, jsonProperty);
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Log info about controls being added
                string log = $"Adding {type.Name.Replace("System.Windows.Forms.", "")} \"{ctrlName}\"";
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
        }

        private void CreateEventHandlers(dynamic newCtrl, JProperty jsonProperty)
        {
            foreach (var token in jsonProperty.Value)
            {
                JProperty jprop = token.ToObject<JProperty>();
                Exe.BindEventToDynamicCtrl(newCtrl, jprop.Name, this, "ExecuteScript");
            }
        }

        private void ExecuteScript(object sender, EventArgs e)
        {
            string scriptName = "RunScript.txt";
            string scriptPath = Path.Combine(Exe.Directory(), $"FormSettings\\Scripts\\{scriptName}");
            //var scriptOptions = ScriptOptions.Default.AddReferences(typeof(MessageBox).Assembly).AddImports("System.Windows.Forms");
            //CSharpScript.EvaluateAsync(File.ReadAllText(scriptPath), scriptOptions);
        }

        private void SetSize(PropertyInfo typeProperty, JProperty jsonProperty, dynamic newCtrl)
        {
            var value = (JArray)jsonProperty.Value;
            int[] array = value.ToObject<int[]>();
            typeProperty.SetValue(newCtrl, new Size(array[0], array[1]));
        }

        private void SetPadding(PropertyInfo typeProperty, JProperty jsonProperty, dynamic newCtrl)
        {
            var value = (JArray)jsonProperty.Value;
            int[] array = value.ToObject<int[]>();
            typeProperty.SetValue(newCtrl, new Padding(array[0], array[1], array[2], array[3]));
        }

        private void SetCtrlProperty(PropertyInfo typeProperty, JProperty jsonProperty, dynamic newCtrl)
        {
            if (typeProperty.PropertyType == typeof(string))
                typeProperty.SetValue(newCtrl, jsonProperty.Value.ToString());
            else if (typeProperty.PropertyType == typeof(int))
                typeProperty.SetValue(newCtrl, Convert.ToInt32(jsonProperty.Value.ToString()));
            else if (typeProperty.PropertyType == typeof(float))
                typeProperty.SetValue(newCtrl, Convert.ToSingle(jsonProperty.Value.ToString()));
            else if (typeProperty.PropertyType == typeof(bool))
                typeProperty.SetValue(newCtrl, Convert.ToBoolean(jsonProperty.Value.ToString()));
            else if (typeProperty.PropertyType.IsEnum)
                typeProperty.SetValue(newCtrl, Enum.Parse(typeProperty.PropertyType, jsonProperty.Value.ToString().Split('.').Last()), null);
        }

        private void CreateColumns(dynamic ctrl, JProperty prop)
        {
            foreach (var value in (JArray)prop.Value)
            {
                ctrl.ColumnCount += 1;
                ctrl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, value.Value<int>()));
            }
        }

        private void CreateRows(dynamic ctrl, JProperty prop)
        {
            foreach (var value in (JArray)prop.Value)
            {
                ctrl.RowCount += 1;
                ctrl.RowStyles.Add(new RowStyle(SizeType.Percent, value.Value<int>()));
            }
        }

        private int GetPropValueAsInt(JProperty prop)
        {
            return Convert.ToInt32(prop.Value.ToString());
        }

        private JArray GetJToken(JProperty prop)
        {
            return (JArray)prop.Value;
        }

        private JProperty GetJsonCtrlParent(JObject ctrl)
        {
            return (JProperty)ctrl.Parent;
        }

        private Type GetJsonCtrlType(JObject ctrl)
        {
            return Exe.GetType(ctrl.Value<string>("ControlType"));
        }

        private string GetJsonCtrlName(dynamic obj)
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
                if (prop.Name == "Name")
                    return prop.GetValue(obj).ToString();
            return "";
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
