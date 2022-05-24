using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ShrineFox.IO
{
    public class Settings
    {
        /// <summary>
        /// A dictionary of all the settings from a YML file.
        /// </summary>
        public Settings() 
        {
            Data = null;
            YmlPath = "";
        }
        private List<KeyValuePair<object, object>> Data;
        public string YmlPath { get; set; } = "";

        /// <summary>
        /// Creates a new Settings object from a yml file.
        /// </summary>
        /// <param name="path">(Optional) The path to the yml file.</param>
        public void Load(string path = "")
        {
            // Use input path if it's valid
            if (File.Exists(path))
                YmlPath = Path.GetFullPath(path);

            // Deserialize yml to Data object unless YmlPath doesn't exist
            if (File.Exists(YmlPath))
            {
                var deserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
                Data = deserializer.Deserialize<IDictionary<object, object>>(File.ReadAllText(YmlPath)).ToList();

                Output.Log($"Loaded settings file \"{YmlPath}\" and deserialized as Settings object.", ConsoleColor.Green);
            }
            else
                Output.Log($"[ERROR] Failed to load settings file: \"{YmlPath}\"", ConsoleColor.Red);
        }

        /// <summary>
        /// Get the value of a setting by name.
        /// </summary>
        /// <param name="key">The name of the setting to find the value of.</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            if (Data != null)
            {
                string value = Data.Single(x => x.Key.ToString().ToLower().Equals(key.ToLower())).Value.ToString();
                Output.Log($"Loaded value of \"{value}\" from key \"{key}\" in Settings object.", ConsoleColor.Green);

                return value;
            }
            else
                Output.Log($"[ERROR] Failed to load value of \"{key}\" from Settings object." +
                    $"\nSettings object was null. Last loaded from: \"{YmlPath}\"", ConsoleColor.Red);

            return "";
        }

        /// <summary>
        /// Set the value of a setting by name.
        /// </summary>
        /// <param name="key">The name of the setting to update the value of.</param>
        /// <param name="value">The new value of the setting.</param>
        public void SetValue(string key, string value)
        {
            if (Data != null)
            {
                int i = Data.IndexOf(Data.First(x => x.Key.ToString().ToLower().Equals(key.ToLower())));
                Data[i] = new KeyValuePair<object, object>(Data[i].Key, value);

                Output.Log($"Set value of \"{key}\" to \"{value}\" in Settings object.", ConsoleColor.Green);
            }
            else
                Output.Log($"[ERROR] Failed to set value of \"{key}\" to \"{value}\" in Settings object." +
                    $"\nSettings object was null. Last loaded from: \"{YmlPath}\"", ConsoleColor.Red);
        }

        /// <summary>
        /// Creates or overwrites a yml file from the Settings object if it's not null.
        /// </summary>
        /// <param name="path">(Optional) The path to the yml file.</param>
        public void Save(string path = "")
        {
            if (Data != null)
            {
                // Use input path if it's valid
                if (!string.IsNullOrEmpty(path))
                    YmlPath = Path.GetFullPath(path);

                var dictionary = new Dictionary<string, string>();
                foreach (var setting in Data)
                    dictionary.Add(setting.Key.ToString(), setting.Value.ToString());

                var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
                var yamlTxt = serializer.Serialize(dictionary);
                using (FileSys.WaitForFile(YmlPath)) { };
                File.WriteAllText(YmlPath, yamlTxt);

                Output.Log($"Saved settings to file: \"{YmlPath}\"", ConsoleColor.Green);
            }
            else
                Output.Log($"[ERROR] Failed to save settings to file: \"{YmlPath}\"" +
                    $"\nSettings object was null.", ConsoleColor.Red);
        }

        /// <summary>
        /// Update Settings object with current values of each matching control from the form.
        /// </summary>
        /// <param name="form">Form to get settings values from.</param>
        public void UpdateSettings(Form form)
        {
            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    foreach (TextBox txtBox in form.GetAllControls<TextBox>())
                        if (Data[i].Key.Equals(txtBox.Tag))
                            Data[i] = new KeyValuePair<object, object>(Data[i].Key, txtBox.Text);
                    foreach (ComboBox comboBox in form.GetAllControls<ComboBox>())
                        if (Data[i].Key.Equals(comboBox.Tag))
                            Data[i] = new KeyValuePair<object, object>(Data[i].Key, comboBox.SelectedItem.ToString());
                }
            }
            else
                Output.Log($"[ERROR] Failed to update Settings object with values from Form \"{form.Name}\"." +
                    $"\nSettings object was null. Last loaded from: \"{YmlPath}\"", ConsoleColor.Red);
        }

        /// <summary>
        /// Update form controls with current values of Settings object
        /// </summary>
        /// <param name="form">Form to apply settings values to.</param>
        public void UpdateForm(Form form)
        {
            if (Data != null)
            {
                foreach (TextBox txtBox in form.GetAllControls<TextBox>())
                    if (Data.Any(x => x.Key.ToString().Equals(txtBox.Tag)))
                        txtBox.Text = Data.Single(x => x.Key.ToString().Equals(txtBox.Tag)).Value.ToString();
                foreach (ComboBox comboBox in form.GetAllControls<ComboBox>())
                    if (Data.Any(x => x.Key.ToString().Equals(comboBox.Tag)))
                        comboBox.SelectedIndex = comboBox.Items.IndexOf(Data.Single(x => x.Key.ToString().Equals(comboBox.Tag)).Value);
            }
            else
                Output.Log($"[ERROR] Failed to update \"{form.Name}\" Form with values from Settings object." +
                    $"Settings object was null. Last loaded from: \"{YmlPath}\"", ConsoleColor.Red);
        }

        /// <summary>
        /// Create form with controls generated from Yml file.
        /// </summary>
        public Form Form(string formName = "settingsForm", string formText = "Settings", string formYmlPath = "", int width = 500, int height = 500)
        {
            Form settingsForm = new Form() { };
            settingsForm.Name = formName;
            settingsForm.Text = formText;
            settingsForm.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            settingsForm.ForeColor = System.Drawing.Color.Silver;
            settingsForm.Width = width;
            settingsForm.Height = height;
            // Create TableLayoutPanel to separate form content and buttons
            TableLayoutPanel tlp_Main = new TableLayoutPanel() { BackColor = settingsForm.BackColor, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 80));
            tlp_Main.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            // Add buttons to bottom of TableLayoutPanel
            TableLayoutPanel tlp_Buttons = new TableLayoutPanel() { BackColor = settingsForm.BackColor, Dock = DockStyle.Fill, Padding = new Padding(10) };
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            tlp_Buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            Button btnCancel = new Button() { BackColor = settingsForm.BackColor, ForeColor = settingsForm.ForeColor, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, Dock = DockStyle.Fill };
            btnCancel.Text = "Cancel";
            tlp_Buttons.Controls.Add(btnCancel, 1, 0);
            Button btnSave = new Button() { BackColor = settingsForm.BackColor, ForeColor = settingsForm.ForeColor, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, Dock = DockStyle.Fill };
            btnSave.Text = "Save";
            btnSave.DialogResult = DialogResult.OK;
            tlp_Buttons.Controls.Add(btnSave, 2, 0);
            tlp_Main.Controls.Add(tlp_Buttons, 0, 1);
            // Create panel to hold content TableLayoutPanel
            Panel panel = new Panel() { BackColor = settingsForm.BackColor, Dock = DockStyle.Fill, AutoScroll = true, AutoSize = false };
            TableLayoutPanel tlp_Content = new TableLayoutPanel() { BackColor = settingsForm.BackColor, Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, AutoScroll = false };
            tlp_Content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            tlp_Content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            int row = 0; // number of rows

            if (string.IsNullOrEmpty(formYmlPath))
                formYmlPath = Path.Combine(Path.Combine(Exe.Directory(), "FormSettings"), $"{formName}Controls.yml");

            foreach (var ctrlAttributes in GetFormControlAttributes(formYmlPath))
            {
                // Set Control Name and Label
                string controlName = ctrlAttributes.Item1.ToString();
                string label = controlName;
                if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("Label")))
                    label = ctrlAttributes.Item2.Single(x => x.Item1.Equals("Label")).Item2;

                // Create new label and add to first column of row
                tlp_Content.Controls.Add(new Label()
                {
                    Text = label,
                    Name = $"lbl_{controlName}",
                    Tag = controlName,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                    AutoSize = true,
                    Anchor = AnchorStyles.Left
                }, 0, row);

                // Create control
                if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("ControlType")))
                {
                    // Get attributes
                    string defaultValue = "";
                    if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("DefaultValue")))
                        defaultValue = ctrlAttributes.Item2.Single(x => x.Item1.Equals("DefaultValue")).Item2;
                    bool readOnly = false;
                    if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("ReadOnly")))
                        readOnly = Convert.ToBoolean(ctrlAttributes.Item2.Single(x => x.Item1.Equals("ReadOnly")).Item2);
                    bool multiline = false;
                    if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("MultiLine")))
                        multiline = Convert.ToBoolean(ctrlAttributes.Item2.Single(x => x.Item1.Equals("MultiLine")).Item2);
                    string clickEvent = "";
                    if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("ClickEvent")))
                        clickEvent = ctrlAttributes.Item2.Single(x => x.Item1.Equals("ClickEvent")).Item2;

                    // Get control type
                    switch (ctrlAttributes.Item2.Single(x => x.Item1.Equals("ControlType")).Item2)
                    {
                        case "TextBox":
                            // Create new textbox
                            TextBox txtBox = new TextBox()
                            {
                                Text = defaultValue,
                                Tag = controlName,
                                Name = $"txtBox_{controlName}",
                                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                                ForeColor = System.Drawing.Color.White,
                                BorderStyle = BorderStyle.FixedSingle,
                                Width = width - 50
                            };
                            // Apply attributes
                            if (readOnly)
                                txtBox.ReadOnly = true;
                            if (multiline)
                            {
                                txtBox.Multiline = true;
                                txtBox.Height = 100;
                            }
                            if (clickEvent == "FolderPath_Click")
                                txtBox.Click += Forms.FolderPath_Click;
                            else if (clickEvent == "FilePath_Click")
                                txtBox.Click += Forms.FilePath_Click;
                            // Add textbox to second column of row
                            tlp_Content.Controls.Add(txtBox, 1, row);
                            break;
                        case "ComboBox":
                            // Create new combobox
                            ComboBox comboBox = new ComboBox()
                            {
                                Tag = controlName,
                                Name = $"comboBox_{controlName}",
                                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                                ForeColor = System.Drawing.Color.White,
                                DropDownStyle = ComboBoxStyle.DropDownList,
                                Width = width - 50
                            };
                            // Add options
                            List<string> options = new List<string>();
                            if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("Options")))
                                options = ctrlAttributes.Item2.Single(x => x.Item1.Equals("Options")).Item2.Split('|').ToList();
                            foreach (var item in options)
                                comboBox.Items.Add(item);
                            // Select default option
                            if (defaultValue != "")
                                comboBox.SelectedIndex = comboBox.Items.IndexOf(defaultValue);
                            // Add textbox to second column of row
                            tlp_Content.Controls.Add(comboBox, 1, row);
                            break;
                    }
                    row++;
                }
            }

            // Add controls to form
            panel.Controls.Add(tlp_Content);
            tlp_Main.Controls.Add(panel);
            settingsForm.Controls.Add(tlp_Main);

            return settingsForm;
        }

        /// <summary>
        /// Creates a list of keys and values for each control of a dynamically generated form.
        /// </summary>
        /// <param name="yml">Path to the .yml to get values from.</param>
        /// <returns></returns>
        private static List<Tuple<string, List<Tuple<string, string>>>> GetFormControlAttributes(string ymlPath)
        {
            List<Tuple<string, List<Tuple<string, string>>>> formCtrlAttributes = new List<Tuple<string, List<Tuple<string, string>>>>();

            // Read form control YML file that corresponds to settings object
            Settings formSettings = new Settings();
            if (File.Exists(ymlPath))
            {
                formSettings.Load(ymlPath);

                foreach (var formSetting in formSettings.Data)
                {
                    string controlName = formSetting.Key.ToString();
                    List<Tuple<string, string>> formCtrlAttribute = new List<Tuple<string, string>>();
                    foreach (var attribute in (List<object>)formSetting.Value)
                    {
                        var attributeData = (List<object>)attribute;
                        string key = attributeData[0].ToString();
                        string value = attributeData[1].ToString();
                        // Create list of combobox options separated by | character
                        if (attributeData[0].ToString().Equals("Options"))
                        {
                            string options = "";
                            foreach (var item in (List<object>)attributeData[1])
                                options += $"{item.ToString()}|";
                            value = options.TrimEnd('|');
                        }

                        formCtrlAttribute.Add(new Tuple<string, string>(key, value));
                    }
                    formCtrlAttributes.Add(new Tuple<string, List<Tuple<string, string>>>(controlName, formCtrlAttribute));
                }
            }
            else
                Output.Log($"[ERROR] Failed to load Form data: \"{ymlPath}\"", ConsoleColor.Red);

            return formCtrlAttributes;
        }
    }
}
