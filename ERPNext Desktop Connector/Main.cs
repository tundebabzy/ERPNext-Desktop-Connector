using ERPNext_Desktop_Connector.Events;
using System;
using System.Windows.Forms;

namespace ERPNext_Desktop_Connector
{
    public partial class Main : Form
    {
        private Connector Connector;
        private bool Started = false;
        // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls?view=netframeworkdesktop-4.8
        private delegate void ThreadSafeDelegate(string text);
        private bool Exit = false;
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
            Connector.LoggedInStateChange += ChangeLoggedInStatus;
        }

        private void ChangeLoggedInStatus(object sender, EventDataArgs e)
        {
            ChangeInformationText(e.Text);
        }

        private void ChangeInformationText(string text)
        {
            if (InformationLabel.InvokeRequired)
            {
                var del = new ThreadSafeDelegate(ChangeInformationText);
                InformationLabel.Invoke(del, new object[] { text });
            } else
            {
                InformationLabel.Text = text;
            }
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
            ExitMenuItem.Enabled = true;
        }

        private void SetButtonsForStartState(object sender, EventArgs e)
        {
            syncButton.Enabled = false;
            stopButton.Enabled = true;
            SettingsMenuItem.Enabled = false;
            ExitMenuItem.Enabled = false;
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

        private void SyncButton_Click(object sender, EventArgs e)
        {
            Connector.OnStart();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Connector.OnStop();
        }

        private void ViewLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var settingsForm = new SettingsForm())
            {
                settingsForm.FormClosing += SaveApplicationSettings;
                settingsForm.ShowDialog();
            }

        }

        private void SaveApplicationSettings(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void AboutMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var aboutForm = new About())
            {
                aboutForm.ShowDialog();
            }
        }

        private void ExitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Exit = true;
            Close();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Exit)
            {
                Hide();
                NotifyIcon.Visible = true;
                ShowInTaskbar = false;
                e.Cancel = true;
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            NotifyIcon.Visible = false;
        }
    }
}
