using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

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
            jsonPath = "";
        }
        public JObject Data;
        public JObject FormSettings;
        public string jsonPath { get; set; } = "";

        public void Initialize(string formName)
        {
            // Look for form config file
            string formJson = Path.Combine(Path.Combine(Exe.Directory(), "FormSettings"), $"{formName}Controls.json");

            // Read file and add to main Settings object
            if (File.Exists(formJson))
                FormSettings = JObject.Parse(File.ReadAllText(formJson));
        }

        /// <summary>
        /// Creates a new Settings object from a json file.
        /// </summary>
        /// <param name="path">(Optional) The path to the json file.</param>
        public void Load(string path = "")
        {
            // Use input path if it's valid
            if (File.Exists(path))
                jsonPath = Path.GetFullPath(path);

            // Deserialize to Data object unless path doesn't exist
            if (File.Exists(jsonPath))
            {
                Data = Json.Deserialize(jsonPath);
                Output.Log($"Loaded settings file: \"{jsonPath}\"", ConsoleColor.Green);
            }
            else
                Output.Log($"[ERROR] Failed to load settings file: \"{jsonPath}\"", ConsoleColor.Red);
        }

        /// <summary>
        /// Creates or overwrites a json file from the Settings object if it's not null.
        /// </summary>
        /// <param name="path">(Optional) The path to the json file.</param>
        public void Save(string path = "")
        {
            if (Data != null)
            {
                // Use input path if it's valid
                if (!string.IsNullOrEmpty(path))
                    jsonPath = Path.GetFullPath(path);

                Directory.CreateDirectory(Path.GetDirectoryName(jsonPath));
                using (FileSys.WaitForFile(jsonPath)) { };
                File.WriteAllText(jsonPath, Data.ToString());

                Output.Log($"Saved settings to file: \"{jsonPath}\"", ConsoleColor.Green);
            }
            else
                Output.Log($"[ERROR] Failed to save settings to file: \"{jsonPath}\"" +
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
                foreach (TextBox txtBox in form.GetAllControls<TextBox>())
                    if (Data.TryGetValue(txtBox.Tag.ToString(), out var value))
                        Data[txtBox.Tag.ToString()] = txtBox.Text;

                foreach (ComboBox comboBox in form.GetAllControls<ComboBox>())
                    if (Data.TryGetValue(comboBox.Tag.ToString(), out var value))
                        Data[comboBox.Tag.ToString()] = comboBox.SelectedItem.ToString();
            }
            else
                Output.Log($"[ERROR] Failed to update Settings object with values from Form \"{form.Name}\"." +
                    $"\nSettings object was null. Last loaded from: \"{jsonPath}\"", ConsoleColor.Red);
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
                    if (Data.TryGetValue(txtBox.Tag.ToString(), out var value))
                        txtBox.Text = value.ToString();

                foreach (ComboBox comboBox in form.GetAllControls<ComboBox>())
                    if (Data.TryGetValue(comboBox.Tag.ToString(), out var value))
                        comboBox.SelectedIndex = comboBox.Items.IndexOf(value.ToString());            }
            else
                Output.Log($"[ERROR] Failed to update \"{form.Name}\" Form with values from Settings object." +
                    $"Settings object was null. Last loaded from: \"{jsonPath}\"", ConsoleColor.Red);
        }

        /// <summary>
        /// Check if settings form has valid values.
        /// </summary>
        /// <param name="form">Form to get settings values from.</param>
        public bool ValidateSettings(Form form)
        {
            Graphics g = form.CreateGraphics();
            foreach (TextBox txtBox in form.GetAllControls<TextBox>())
            {
                if (!ValidateSetting(txtBox.Tag.ToString(), txtBox.Text))
                {
                    txtBox.ForeColor = Color.Red;
                    return false;
                }
                else
                    txtBox.ForeColor = Color.Green;
            }
            Output.VerboseLog($"Successfully validated settings.", ConsoleColor.Green);
            return true;
        }

        /// <summary>
        /// Update form controls with current values of Settings object
        /// </summary>
        public bool ValidateSetting(string controlName, string controlValue)
        {
            if (FormSettings != null)
            {
                var ctrls = FormSettings["Form"]["Controls"];
                
                string requiredString = ctrls[controlName].GetValue<string>("Required") ?? "false";
                bool required = Convert.ToBoolean(requiredString);

                if (required && string.IsNullOrEmpty(controlValue))
                {
                    Output.VerboseLog($"Required field \"{controlName}\" has null or empty value: \"{controlValue}\"", ConsoleColor.Red);
                    return false;
                }

                string alphanumericString = ctrls[controlName].GetValue<string>("AlphanumericOnly") ?? "false";
                bool alphanumeric = Convert.ToBoolean(alphanumericString);

                if (alphanumeric && !Regex.IsMatch(controlValue, "^[a-zA-Z0-9-_ .]*$"))
                {
                    Output.VerboseLog($"Alphanumeric-only field \"{controlName}\" has invalid value: \"{controlValue}\"", ConsoleColor.Red);
                    return false;
                }

                Output.VerboseLog($"Field \"{controlName}\" has valid value: \"{controlValue}\"", ConsoleColor.Green);
                return true;
            }
            else
            {
                Output.Log($"[ERROR] Failed to check attributes of FormSettings object." +
                    $"FormSettings object was null.", ConsoleColor.Red);
                return false;
            }
        }

        /// <summary>
        /// Create form with controls generated from Yml file.
        /// </summary>
        public Form Form()
        {
            Form settingsForm = Forms.SettingsForm();

            TableLayoutPanel tlp_Content = (TableLayoutPanel)settingsForm.Controls.Find("tlp_Content", true).Single();
            Button btn_Save = (Button)settingsForm.Controls.Find("btn_Save", true).Single();
            btn_Save.Click += SaveButton_Clicked;

            // Initialize settings object with default form values
            JObject defaultSettings = new JObject();

            int row = 0;

            // Create a form control for each setting
            foreach (JProperty property in (JToken)FormSettings)
            {
                string name = property.Name;
                JToken value = property.Value;

                if (name == "Form")
                {
                    settingsForm.Name = value.Value<string>("Name");
                    settingsForm.Text = value["Text"].Value<string>();
                    settingsForm.Width = Convert.ToInt32(value["Width"].Value<string>());
                    settingsForm.Height = Convert.ToInt32(value["Height"].Value<string>());

                    foreach (JProperty ctrl in value["Controls"].Children().ToList())
                    {
                        string label = ctrl.Name;
                        value = ctrl.Value;

                        string ctrlType = value["ControlType"].Value<string>();
                        if (ctrlType != "")
                        {
                            label = value.GetValue<string>("Label") ?? ctrl.Name;
                            string defaultValue = value.GetValue<string>("DefaultValue");
                            bool readOnly = value.GetValue<bool>("ReadOnly");
                            bool multiline = value.GetValue<bool>("MultiLine");
                            bool required = value.GetValue<bool>("Required");
                            string clickEvent = value.GetValue<string>("ClickEvent");
                            JArray options = value.GetValue<JArray>("Options");

                            // Add default control value to settings object
                            defaultSettings.Add(ctrl.Name, defaultValue ?? "");

                            if (required)
                                label += "*";

                            // Create new label and add to first column of row
                            tlp_Content.Controls.Add(new Label()
                            {
                                Text = label,
                                Name = $"lbl_{ctrl.Name}",
                                Tag = ctrl.Name,
                                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                AutoSize = true,
                                Anchor = AnchorStyles.Left
                            }, 0, row);

                            // Get control type
                            switch (ctrlType)
                            {
                                case "TextBox":
                                    // Create new textbox
                                    TextBox txtBox = new TextBox()
                                    {
                                        Text = defaultValue,
                                        Tag = ctrl.Name,
                                        Name = $"txtBox_{ctrl.Name}",
                                        Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                        BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                                        ForeColor = System.Drawing.Color.White,
                                        BorderStyle = BorderStyle.FixedSingle,
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
                                        Tag = ctrl.Name,
                                        Name = $"comboBox_{ctrl.Name}",
                                        Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                        BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                                        ForeColor = System.Drawing.Color.White,
                                        DropDownStyle = ComboBoxStyle.DropDownList,
                                        Width = settingsForm.Width - 50
                                    };
                                    foreach (var item in options)
                                        comboBox.Items.Add(item.ToString());
                                    // Select default option on form
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
            }

            // Set settings object data to default values at first
            if (Data == null || Data.Count == 0)
                Data = defaultSettings;

            // Fill default values with current settings values
            UpdateForm(settingsForm);

            return settingsForm;
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Form form = btn.FindForm();

            if (ValidateSettings(form))
            {
                UpdateSettings(form);
                form.DialogResult = DialogResult.OK;
            }
        }
    }
}
