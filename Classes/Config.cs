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
    public class Config
    {
        /// <summary>
        /// Serialized object from a JSON file.
        /// </summary>
        public Config(string userDataPath = "", string formSettingsPath = "") 
        {
            UserData = null;
            UserDataPath = userDataPath;
            FormSettings = null;
            FormSettingsPath = formSettingsPath;
        }
        public JObject UserData;
        public JObject FormSettings;
        public string UserDataPath { get; set; } = "";
        public string FormSettingsPath { get; set; } = "";

        // Try to get userdata and formsettings from json files
        public void Load()
        {
            FormSettingsPath = Path.GetFullPath(FormSettingsPath);
            if (File.Exists(FormSettingsPath))
            {
                FormSettings = Json.Deserialize(FormSettingsPath);
                Output.Log($"Loaded form settings from \"{FormSettingsPath}\"", ConsoleColor.Green);
            }
            else
                Output.Log($"Failed to load \"{FormSettingsPath}\", file doesn't exist.");

            UserDataPath = Path.GetFullPath(UserDataPath);
            if (File.Exists(UserDataPath))
            {
                UserData = Json.Deserialize(UserDataPath);
                Output.Log($"Loaded user data from \"{(UserDataPath)}\"", ConsoleColor.Green);
            }
            else
                Output.VerboseLog($"Failed to load \"{UserDataPath}\", file doesn't exist.");
        }

        // Overwrite userdata and formsettings with current values
        public void Save()
        {
            if (FormSettings != null && !string.IsNullOrEmpty(FormSettingsPath))
                Json.Serialize(FormSettings, FormSettingsPath);
            else
                Output.Log($"Failed to save to \"{FormSettingsPath}\", FormSettings were null.");

            if (UserData != null && !string.IsNullOrEmpty(UserDataPath))
                Json.Serialize(UserData, UserDataPath);
            else
                Output.Log($"Failed to save to \"{UserDataPath}\", UserData was null.");

        }

        /// <summary>
        /// Update Settings object with current values of each matching control from the form.
        /// </summary>
        /// <param name="form">Form to get settings values from.</param>
        public void UpdateSettings(Form form)
        {
            if (UserData != null)
            {
                foreach (TextBox txtBox in form.GetAllControls<TextBox>())
                    if (UserData.TryGetValue(txtBox.Tag.ToString(), out var value))
                        UserData[txtBox.Tag.ToString()] = txtBox.Text;

                foreach (ComboBox comboBox in form.GetAllControls<ComboBox>())
                    if (UserData.TryGetValue(comboBox.Tag.ToString(), out var value))
                        UserData[comboBox.Tag.ToString()] = comboBox.SelectedItem.ToString();
            }
            else
                Output.Log($"[ERROR] Failed to update UserData with values from Form \"{form.Name}\". " +
                    $"UserData was null. Last loaded from: \"{UserDataPath}\"", ConsoleColor.Red);
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
                Output.Log($"[ERROR] Failed to check attributes of FormSettings. " +
                    $"FormSettings was null.", ConsoleColor.Red);
                return false;
            }
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
