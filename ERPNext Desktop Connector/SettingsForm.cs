using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ERPNext_Desktop_Connector
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            ToggleTimePickerState(sender as CheckBox);
        }

        private void ToggleTimePickerState(CheckBox sender)
        {
            this.StartTimePicker.Enabled = sender.Checked;
            this.EndTimePicker.Enabled = sender.Checked;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            TokenText.Enabled = true;
            SecretText.Enabled = true;
            ServerUrlText.Enabled = true;
            AppIdText.Enabled = true;
            EditButton.Enabled = false;
        }
    }
}
