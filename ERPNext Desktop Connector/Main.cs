using ERPNext_Desktop_Connector.Events;
using System;
using System.Windows.Forms;

namespace ERPNext_Desktop_Connector
{
    public partial class Main : Form
    {
        private Connector Connector;
        private bool Started = false;
        public Main()
        {
            InitializeComponent();
            InitializeConnector();
            EnableButtons();
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            Connector.ConnectorStarted += SetButtonsForStartState;
            Connector.ConnectorStopped += SetButtonsForStopState;
            Connector.ConnectorInformation += ChangeStatus;
            Connector.PeachtreeInformation += ChangeStatus;
        }

        private void ChangeStatus(object sender, EventDataArgs e)
        {
            toolStripStatusLabel.Text = e.Text;
        }

        private void SetButtonsForStopState(object sender, EventArgs e)
        {
            syncButton.Enabled = true;
            stopButton.Enabled = false;
            SettingsMenuItem.Enabled = true;
        }

        private void SetButtonsForStartState(object sender, EventArgs e)
        {
            syncButton.Enabled = false;
            stopButton.Enabled = true;
            SettingsMenuItem.Enabled = false;
        }

        private void EnableButtons()
        {
            stopButton.Enabled = Started;
            syncButton.Enabled = !Started;
            SettingsMenuItem.Enabled = !Started;
        }

        private void InitializeConnector()
        {
            Connector = new Connector();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void syncButton_Click(object sender, EventArgs e)
        {
            Connector.OnStart();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Connector.OnStop();
        }

        private void generalStateLabel_Click(object sender, EventArgs e)
        {

        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void ViewLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }

        }
    }
}
