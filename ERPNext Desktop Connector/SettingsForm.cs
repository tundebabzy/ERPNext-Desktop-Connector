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
            SetCancelButtonHandlers();
            SetSaveButtonHandlers();
        }

        private void SetCancelButtonHandlers()
        {
            CancelButton.Click += CloseForm;
        }

        private void CloseForm(object sender, EventArgs e)
        {
            Close();
        }

        private void SetSaveButtonHandlers()
        {
            SaveButton.Click += SaveSettings;
        }

        private void SaveSettings(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
