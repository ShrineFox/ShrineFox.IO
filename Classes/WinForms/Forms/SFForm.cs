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
        string mainFormJson = "";

        public SFForm(string formName = "", 
            string formJson = "Form\\MainForm.json",
            string userJson = "Form\\MainUserData.json")
        {
            if (formName != "")
                Name = formName;
            else
                Name = Exe.Name();
            Text = Name;
            mainFormJson = formJson;
            config = new Config(userJson, mainFormJson);

            SetupForm();
        }

        private void SetupForm()
        {
            // Set up default form appearance
            SetupLog();
            SetupTheme();
            WinForms.SetDefaultIcon();
            SetTreeviewImages();

            // Load user generated layout
            config.Load();
            if (config.FormSettings != null)
            {
                var formToken = config.FormSettings["Form"];
                SetupFormDetails(formToken);
                AddControls(formToken, this);
            }

            Output.Log("Form Loaded.");

            LoadSavedData();
        }

        public void LoadSavedData()
        {
            if (config.UserData != null)
            {
                foreach (JToken subCtrl in config.UserData["Controls"])
                {
                    string ctrlName = GetJsonCtrlName(subCtrl);
                    var ctrl = subCtrl.Children().First().Value<JObject>();
                    Type type = GetJsonCtrlType(ctrl);
                    // For each property of the control in JSON...
                    foreach (JProperty jsonProperty in ctrl.Properties())
                    {
                        // If there's a Type property with the same name...
                        var typeProperties = type.GetProperties();
                        if (typeProperties.Any(x => x.Name.Equals(jsonProperty.Name)))
                        {
                            // Get the first JSON property that matches
                            var typeProperty = typeProperties.First(x => x.Name.Equals(jsonProperty.Name));
                            // Assign property value to control
                            dynamic formCtrl = WinForms.GetControl(this, ctrlName);
                            SetCtrlProperty(typeProperty, jsonProperty, formCtrl);
                            Output.VerboseLog($"Set \"{ctrlName}\" {jsonProperty.Name} to \"{jsonProperty.Value}\" " +
                                $"from {Path.GetFileName(config.UserDataPath)}", ConsoleColor.Yellow);
                        }
                    }
                }
                Output.Log("User Data Loaded.");
            }
        }

        public void SaveData()
        {
            if (!string.IsNullOrEmpty(config.UserDataPath))
            {
                string json = "{\n\t\"Controls\": {\n";
                foreach (var ctrl in WinForms.EnumerateControls(this))
                {
                    if (ctrl.Text != null)
                    {
                        json = json + $"\t\t\"{ctrl.Name}\": {{" +
                            $"\n\t\t\t\"ControlType\": \"{ctrl.GetType().FullName}\"," +
                            $"\n\t\t\t\"Text\": \"{ctrl.Text}\"" +
                            "\n\t\t},\n";
                        Output.VerboseLog($"Saved \"{ctrl.Name}\" Text as \"{ctrl.Text}\" " +
                                $"to {Path.GetFileName(config.UserDataPath)}", ConsoleColor.Yellow);
                    }
                }
                json = json + "\t}\n}";
                File.WriteAllText(config.UserDataPath, json);
                Output.Log("User Data Saved.");
            }
        }

        private static void SetupLog()
        {
            Output.Logging = true;
            Output.LogToFile = true;
            #if DEBUG
                if (!Output.HasMainWindow())
                    Output.AllocConsole();
                Output.VerboseLogging = true;
            #endif
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
            foreach (JProperty property in ctrlToken.Children<JProperty>())
            {
                if (property.Name.Equals("Controls") || property.Name.Equals("DropDownItems"))
                {
                    AddSubControls(property, parent);
                }
            }
        }

        private void AddSubControls(JProperty controls, dynamic parent)
        {
            foreach (var propertyList in controls.Children())
            {
                foreach (JProperty property in propertyList)
                {
                    if (property.Value.ToString().Contains("ControlType"))
                    {
                        AddControl(property, parent);
                    }
                    else if (property.Name.ToLower().StartsWith("json_"))
                    {
                        // Parse referenced json files and add controls to parent
                        string value = property.Children().First().Value<string>();
                        if (value.ToLower().EndsWith(".json"))
                        {
                            string jsonPath = Path.Combine(Exe.Directory(),
                                Path.Combine(Path.GetDirectoryName(mainFormJson),
                                value));
                            var jObj = Json.Deserialize(jsonPath);
                            var subCtrl = (JToken)jObj;
                            Output.Log($"Loading Controls from \"{value}\"");
                            AddControls(subCtrl, parent);
                        }
                    }
                }
            }
        }

        private void AddControl(JToken subCtrl, dynamic parent)
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
            // Set Name of Control
            var nameProperty = typeProperties.First(x => x.Name.Equals("Name"));
            nameProperty.SetValue(newCtrl, ctrlName);
            
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
                        case "DropDownItems":
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
            string log = $"{type.Name.Replace("System.Windows.Forms.", "")} \"{ctrlName}\"" +
                $"\n\tadded to {parentType.FullName.Replace("System.Windows.Forms.", "")} \"{parent.Name}\"";
            if (row != -1 || column != -1)
                log += $" (column {column}, row {row})";
            Output.VerboseLog(log, ConsoleColor.Yellow);

            // Add to parent object depending on type
            if (parentType == typeof(ToolStripMenuItem))
                parent.DropDownItems.Add(newCtrl);
            else if (type == typeof(ToolStripMenuItem))
                parent.Items.Add(newCtrl);
            else if (row != -1 || column != -1)
                parent.Controls.Add(newCtrl, column, row);
            else
                parent.Controls.Add(newCtrl);
        }

        private void CreateEventHandlers(dynamic newCtrl, JProperty jsonProperty)
        {
            foreach (var token in jsonProperty.Value)
            {
                JProperty jprop = token.ToObject<JProperty>();
                string assemblyName = Assembly.GetEntryAssembly().GetName().ToString();
                List<object> args = new List<object>();
                var jarray = (JArray)jprop.Value;
                foreach (var value in jarray.Last())
                {
                    args.Add(value.Value<string>());
                }
                string[] valueParts = jarray.First().Value<string>().Split('.');
                string namespaceName = valueParts[0];
                string className = valueParts[1];
                string methodName = valueParts[2];
                Action<Object, EventArgs> action = (o, ea) => 
                            Exe.InvokeMethod(assemblyName, namespaceName, className, methodName, args.ToArray());
                EventHandler eHandler = action.Invoke;

                switch (jprop.Name)
                {
                    case "Click":
                        newCtrl.Click += eHandler;
                        break;
                }
            }
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
    }
}
