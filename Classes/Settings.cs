using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Microsoft.WindowsAPICodePack.Dialogs;

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
            YmlPath = "settings.yml";
        }
        public List<KeyValuePair<object, object>> Data;
        public string YmlPath { get; set; } = "";

        /// <summary>
        /// Creates a new Settings object from a yml file.
        /// </summary>
        /// <param name="path">(Optional) The path to the yml file.</param>
        public void Load(string path = "settings.yml")
        {
            YmlPath = Path.GetFullPath(path);
            var deserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
            Data = deserializer.Deserialize<IDictionary<object, object>>(File.ReadAllText(YmlPath)).ToList();
        }

        /// <summary>
        /// Get the value of a setting by name.
        /// </summary>
        /// <param name="key">The name of the setting to find the value of.</param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            return Data.Single(x => x.Key.ToString().ToLower().Equals(key.ToLower())).Value.ToString();
        }

        /// <summary>
        /// Set the value of a setting by name.
        /// </summary>
        /// <param name="key">The name of the setting to update the value of.</param>
        /// <param name="value">The new value of the setting.</param>
        public void SetValue(string key, string value)
        {
            int i = Data.IndexOf(Data.First(x => x.Key.ToString().ToLower().Equals(key.ToLower())));
            Data[i] = new KeyValuePair<object, object>(Data[i].Key, value);
        }

        /// <summary>
        /// Creates or overwrites a yml file from the Settings object if it's not null.
        /// </summary>
        /// <param name="path">(Optional) The path to the yml file.</param>
        public void Save(string path = "")
        {
            if (path != "")
                YmlPath = path;
            if (Data != null)
            {
                var dictionary = new Dictionary<string, string>();
                foreach (var setting in Data)
                    dictionary.Add(setting.Key.ToString(), setting.Value.ToString());

                var serializer = new SerializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();
                var yamlTxt = serializer.Serialize(dictionary);
                using (FileSys.WaitForFile(YmlPath)) { };
                File.WriteAllText(YmlPath, yamlTxt);
            }
        }

        /// <summary>
        /// Update Settings object with current values of each matching control from the form.
        /// </summary>
        /// <param name="form">Form to get settings values from.</param>
        public void UpdateSettings(Form form)
        {
            foreach (Control ctrl in form.Controls)
            {
                if (ctrl.GetType() == typeof(TextBox))
                {
                    var property = Data.GetType().GetProperty(ctrl.Tag.ToString());
                    property.SetValue(Data, ctrl.Text, null);
                }
            }

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

        /// <summary>
        /// Update form controls with current values of Settings object
        /// </summary>
        /// <param name="form">Form to apply settings values to.</param>
        public void UpdateForm(Form form)
        {
            foreach (TextBox txtBox in form.GetAllControls<TextBox>())
                if (Data.Any(x => x.Key.ToString().Equals(txtBox.Tag)))
                    txtBox.Text = Data.Single(x => x.Key.ToString().Equals(txtBox.Tag)).Value.ToString();
            foreach (ComboBox comboBox in form.GetAllControls<ComboBox>())
                if (Data.Any(x => x.Key.ToString().Equals(comboBox.Tag)))
                    comboBox.SelectedIndex = comboBox.Items.IndexOf(Data.Single(x => x.Key.ToString().Equals(comboBox.Tag)).Value);
        }

        /// <summary>
        /// Create form controls with current values of Settings object
        /// </summary>
        /// <param name="form">Form to apply settings values to.</param>
        public void CreateFormControls(Form form)
        {
            var tableLayoutPanel = form.GetAllControls<TableLayoutPanel>().First();
            int row = 0; // number of rows

            var formCtrlAttributeData = GetFormControlAttributes(form);
            
            foreach (var ctrlAttributes in formCtrlAttributeData)
            {
                // Set Control Name and Label
                string controlName = ctrlAttributes.Item1.ToString();
                string label = controlName;
                if (ctrlAttributes.Item2.Any(x => x.Item1.Equals("Label")))
                    label = ctrlAttributes.Item2.Single(x => x.Item1.Equals("Label")).Item2;

                // Create new row
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
                // Create new label and add to first column of row
                tableLayoutPanel.Controls.Add(new Label()
                {
                    Text = label,
                    Name = $"lbl_{controlName}",
                    Tag = controlName,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                    AutoSize = true
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
                                Width = 300,
                                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                                ForeColor = System.Drawing.Color.White
                            };
                            // Apply attributes
                            if (readOnly)
                                txtBox.ReadOnly = true;
                            if (multiline)
                                txtBox.Multiline = true;
                            if (clickEvent == "FolderPath_Click")
                                txtBox.Click += Forms.FolderPath_Click;
                            else if (clickEvent == "FilePath_Click")
                                txtBox.Click += Forms.FilePath_Click;
                            // Add textbox to second column of row
                            tableLayoutPanel.Controls.Add(txtBox, 1, row);
                            break;
                        case "ComboBox":
                            // Create new combobox
                            ComboBox comboBox = new ComboBox()
                            {
                                Tag = controlName,
                                Name = $"comboBox_{controlName}",
                                Width = 300,
                                Font = new System.Drawing.Font("Microsoft Sans Serif", 10F),
                                BackColor = System.Drawing.Color.FromArgb(20, 20, 20),
                                ForeColor = System.Drawing.Color.White
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
                            tableLayoutPanel.Controls.Add(comboBox, 1, row);
                            break;
                    }
                    row++;
                }
            }
        }

        private static List<Tuple<string, List<Tuple<string, string>>>> GetFormControlAttributes(Form form)
        {
            // Read form control YML file that corresponds to settings object
            Settings formSettings = new Settings();
            formSettings.Load(Path.Combine(Path.Combine(Exe.Directory(), "FormSettings"),
                $"{form.GetType().Name}Controls.yml"));

            List<Tuple<string, List<Tuple<string, string>>>> formCtrlAttributes = new List<Tuple<string, List<Tuple<string, string>>>>();

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

            return formCtrlAttributes;
        }
    }
}
