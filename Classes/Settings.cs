using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            FormSettings = null;
            YmlPath = "";
        }
        public List<KeyValuePair<object, object>> Data;
        public Settings FormSettings;
        public string YmlPath { get; set; } = "";

        public void Initialize(string formName)
        {
            // Look for form config yml
            string formYml = Path.Combine(Path.Combine(Exe.Directory(), "FormSettings"), $"{formName}Controls.yml");

            // Read YML and add results to main Settings object
            if (File.Exists(formYml))
            {
                Settings formSettings = new Settings();
                formSettings.Load(formYml);
                FormSettings = formSettings;
            }
        }

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
                    foreach (SFTextBox txtBox in form.GetAllControls<SFTextBox>())
                        if (Data[i].Key.Equals(txtBox.Tag))
                        {
                            if (ValidateSetting(txtBox.Tag.ToString(), txtBox.Text))
                                Data[i] = new KeyValuePair<object, object>(Data[i].Key, txtBox.Text);
                            else
                                txtBox.BorderColor = Color.Red;
                        }
                            
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
        /// Check if settings form has valid values.
        /// </summary>
        /// <param name="form">Form to get settings values from.</param>
        public bool ValidateSettings(Form form)
        {
            foreach (SFTextBox txtBox in form.GetAllControls<SFTextBox>())
            {
                if (ValidateSetting(txtBox.Tag.ToString(), txtBox.Text))
                {
                    txtBox.BorderColor = Color.Red;
                    return false;
                }
                else
                {
                    txtBox.BorderColor = Color.Green;
                }
            }
            return true;
        }

        /// <summary>
        /// Update form controls with current values of Settings object
        /// </summary>
        /// <param name="form">Form to apply settings values to.</param>
        public void UpdateForm(Form form)
        {
            if (Data != null)
            {
                foreach (SFTextBox txtBox in form.GetAllControls<SFTextBox>())
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
        /// Update form controls with current values of Settings object
        /// </summary>
        public bool ValidateSetting(string key, string value)
        {
            if (FormSettings != null)
            {
                var formCtrl = FormSettings.Data.Single(x => x.Key.Equals(key));

                if (formCtrl.Key.ToString() == "Required" && formCtrl.Value.ToString() == "true")
                {
                    if (string.IsNullOrEmpty(value))
                        return false;
                }
                if (formCtrl.Key.ToString() == "AlphanumericOnly" && formCtrl.Value.ToString() == "true")
                {
                    if (!Regex.IsMatch(value, "^[a-zA-Z0-9-_ .]*$"))
                        return false;
                }
            }
            else
            {
                Output.Log($"[ERROR] Failed to check attributes of FormSettings object." +
                    $"FormSettings object was null.", ConsoleColor.Red);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create form with controls generated from Yml file.
        /// </summary>
        public Form Form()
        {
            var attributes = FormControlAttributes();
            // Create base settings form
            Form settingsForm = Forms.SettingsForm();
            // Get Content Table Layout Panel object
            TableLayoutPanel tlp_Content = (TableLayoutPanel)settingsForm.Controls.Find("tlp_Content", true).Single();
            // Get Save Button object
            Button btn_Save = (Button)settingsForm.Controls.Find("btn_Save", true).Single();
            btn_Save.Click += SaveButton_Clicked;

            int row = 0; // number of rows

            // Create a form control for each setting
            foreach (var attribute in attributes)
            {
                // Get Form attributes
                if (attribute.Item1.Equals("Form"))
                {
                    settingsForm.Name = attribute.Item2.Single(x => x.Item1.Equals("Name")).Item2;
                    settingsForm.Text = attribute.Item2.Single(x => x.Item1.Equals("Text")).Item2;
                    settingsForm.Width = Convert.ToInt32(attribute.Item2.Single(x => x.Item1.Equals("Width")).Item2);
                    settingsForm.Height = Convert.ToInt32(attribute.Item2.Single(x => x.Item1.Equals("Height")).Item2);
                }
                else
                {
                    // Set Control Name and Label
                    string controlName = attribute.Item1.ToString();
                    string label = controlName;

                    // Create form control
                    if (attribute.Item2.Any(x => x.Item1.Equals("ControlType")))
                    {
                        // Get attributes
                        string defaultValue = "";
                        if (attribute.Item2.Any(x => x.Item1.Equals("DefaultValue")))
                            defaultValue = attribute.Item2.Single(x => x.Item1.Equals("DefaultValue")).Item2;
                        bool readOnly = false;
                        if (attribute.Item2.Any(x => x.Item1.Equals("ReadOnly")))
                            readOnly = Convert.ToBoolean(attribute.Item2.Single(x => x.Item1.Equals("ReadOnly")).Item2);
                        bool multiline = false;
                        if (attribute.Item2.Any(x => x.Item1.Equals("MultiLine")))
                            multiline = Convert.ToBoolean(attribute.Item2.Single(x => x.Item1.Equals("MultiLine")).Item2);
                        bool required = false;
                        if (attribute.Item2.Any(x => x.Item1.Equals("Required")))
                            required = Convert.ToBoolean(attribute.Item2.Single(x => x.Item1.Equals("Required")).Item2);
                        string clickEvent = "";
                        if (attribute.Item2.Any(x => x.Item1.Equals("ClickEvent")))
                            clickEvent = attribute.Item2.Single(x => x.Item1.Equals("ClickEvent")).Item2;

                        // Get label text associated with form control
                        if (attribute.Item2.Any(x => x.Item1.Equals("Label")))
                            label = attribute.Item2.Single(x => x.Item1.Equals("Label")).Item2;
                        if (required)
                            label += "*";

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

                        // Get control type
                        switch (attribute.Item2.Single(x => x.Item1.Equals("ControlType")).Item2)
                        {
                            case "TextBox":
                                // Create new textbox
                                SFTextBox txtBox = new SFTextBox()
                                {
                                    Text = defaultValue,
                                    Tag = controlName,
                                    Name = $"txtBox_{controlName}",
                                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                    BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                                    ForeColor = System.Drawing.Color.White,
                                    BorderStyle = BorderStyle.FixedSingle,
                                    BorderColor = Color.White,
                                    Width = settingsForm.Width - 50
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
                                    Width = settingsForm.Width - 50
                                };
                                // Add options
                                List<string> options = new List<string>();
                                if (attribute.Item2.Any(x => x.Item1.Equals("Options")))
                                    options = attribute.Item2.Single(x => x.Item1.Equals("Options")).Item2.Split('|').ToList();
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
            }

            // Fill default values with current settings values
            UpdateForm(settingsForm);

            return settingsForm;
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Form form = btn.FindForm();

            //if (ValidateSettings(form))
            {
                form.DialogResult = DialogResult.OK;
                UpdateSettings(form);
            }
        }

        /// <summary>
        /// Creates a list of keys and values for each control of a dynamically generated form.
        /// </summary>
        /// <param name="yml">Path to the .yml to get values from.</param>
        /// <returns></returns>
        private List<Tuple<string, List<Tuple<string, string>>>> FormControlAttributes()
        {
            // Createss a blank list of pairs of control names and their keys/values
            List<Tuple<string, List<Tuple<string, string>>>> formCtrlAttributes = new List<Tuple<string, List<Tuple<string, string>>>>();
            // Creates a blank Data object for default values
            List<KeyValuePair<object, object>> data = new List<KeyValuePair<object, object>>();

            foreach (var formSetting in FormSettings.Data)
            {
                string controlName = formSetting.Key.ToString();
                List<Tuple<string, string>> formCtrlAttribute = new List<Tuple<string, string>>();
                foreach (var attribute in (List<object>)formSetting.Value)
                {
                    var attributeData = (List<object>)attribute;
                    string key = attributeData[0].ToString();
                    string value = attributeData[1].ToString();

                    // Add to default values list
                    if (key == "DefaultValue")
                        data.Add(new KeyValuePair<object, object>(controlName, value));

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
            // Set Data object to default values if null or empty
            if (Data == null || Data.Count == 0)
                Data = data;

            return formCtrlAttributes;
        }
    }
}
