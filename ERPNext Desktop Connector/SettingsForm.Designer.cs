namespace ERPNext_Desktop_Connector
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.EndTimePicker = new System.Windows.Forms.DateTimePicker();
            this.StartTimePicker = new System.Windows.Forms.DateTimePicker();
            this.PollingIntervalTextBox = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TokenText = new System.Windows.Forms.TextBox();
            this.EditButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.AppIdText = new System.Windows.Forms.TextBox();
            this.ServerUrlText = new System.Windows.Forms.TextBox();
            this.SecretText = new System.Windows.Forms.TextBox();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Polling Interval (mins)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Sync Start Time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Application ID";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Secret Key";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Sync Stop Time";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "ERPNext Server URL";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.EndTimePicker);
            this.groupBox2.Controls.Add(this.StartTimePicker);
            this.groupBox2.Controls.Add(this.PollingIntervalTextBox);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(13, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(376, 229);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connector Options";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = global::ERPNext_Desktop_Connector.Properties.Settings.Default.AutomaticSync;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "AutomaticSync", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.Location = new System.Drawing.Point(9, 132);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(153, 17);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Automatic Synchronizatioin";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // EndTimePicker
            // 
            this.EndTimePicker.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "SyncStopTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.EndTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.EndTimePicker.Location = new System.Drawing.Point(152, 95);
            this.EndTimePicker.Name = "EndTimePicker";
            this.EndTimePicker.Size = new System.Drawing.Size(200, 20);
            this.EndTimePicker.TabIndex = 15;
            this.EndTimePicker.Value = global::ERPNext_Desktop_Connector.Properties.Settings.Default.SyncStopTime;
            // 
            // StartTimePicker
            // 
            this.StartTimePicker.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "SyncStartTime", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.StartTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.StartTimePicker.Location = new System.Drawing.Point(152, 60);
            this.StartTimePicker.Name = "StartTimePicker";
            this.StartTimePicker.Size = new System.Drawing.Size(200, 20);
            this.StartTimePicker.TabIndex = 14;
            this.StartTimePicker.Value = global::ERPNext_Desktop_Connector.Properties.Settings.Default.SyncStartTime;
            // 
            // PollingIntervalTextBox
            // 
            this.PollingIntervalTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "PollingInterval", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.PollingIntervalTextBox.Location = new System.Drawing.Point(152, 28);
            this.PollingIntervalTextBox.Name = "PollingIntervalTextBox";
            this.PollingIntervalTextBox.Size = new System.Drawing.Size(100, 20);
            this.PollingIntervalTextBox.TabIndex = 1;
            this.PollingIntervalTextBox.Text = global::ERPNext_Desktop_Connector.Properties.Settings.Default.PollingInterval;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TokenText);
            this.groupBox3.Controls.Add(this.EditButton);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.AppIdText);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.ServerUrlText);
            this.groupBox3.Controls.Add(this.SecretText);
            this.groupBox3.Location = new System.Drawing.Point(395, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(393, 229);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Advanced";
            // 
            // TokenText
            // 
            this.TokenText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "ApiToken", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.TokenText.Enabled = false;
            this.TokenText.Location = new System.Drawing.Point(126, 142);
            this.TokenText.Name = "TokenText";
            this.TokenText.Size = new System.Drawing.Size(249, 20);
            this.TokenText.TabIndex = 18;
            this.TokenText.Text = global::ERPNext_Desktop_Connector.Properties.Settings.Default.ApiToken;
            // 
            // EditButton
            // 
            this.EditButton.Location = new System.Drawing.Point(126, 200);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(75, 23);
            this.EditButton.TabIndex = 20;
            this.EditButton.Text = "Edit";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Token";
            // 
            // AppIdText
            // 
            this.AppIdText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "ApplicationId", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.AppIdText.Enabled = false;
            this.AppIdText.Location = new System.Drawing.Point(126, 28);
            this.AppIdText.Name = "AppIdText";
            this.AppIdText.Size = new System.Drawing.Size(249, 20);
            this.AppIdText.TabIndex = 16;
            this.AppIdText.Text = global::ERPNext_Desktop_Connector.Properties.Settings.Default.ApplicationId;
            // 
            // ServerUrlText
            // 
            this.ServerUrlText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "ServerAddress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ServerUrlText.Enabled = false;
            this.ServerUrlText.Location = new System.Drawing.Point(126, 67);
            this.ServerUrlText.Name = "ServerUrlText";
            this.ServerUrlText.Size = new System.Drawing.Size(249, 20);
            this.ServerUrlText.TabIndex = 15;
            this.ServerUrlText.Text = global::ERPNext_Desktop_Connector.Properties.Settings.Default.ServerAddress;
            // 
            // SecretText
            // 
            this.SecretText.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ERPNext_Desktop_Connector.Properties.Settings.Default, "SecretToken", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.SecretText.Enabled = false;
            this.SecretText.Location = new System.Drawing.Point(126, 109);
            this.SecretText.Name = "SecretText";
            this.SecretText.Size = new System.Drawing.Size(249, 20);
            this.SecretText.TabIndex = 11;
            this.SecretText.Text = global::ERPNext_Desktop_Connector.Properties.Settings.Default.SecretToken;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 295);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PollingIntervalTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox SecretText;
        private System.Windows.Forms.TextBox ServerUrlText;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker EndTimePicker;
        private System.Windows.Forms.DateTimePicker StartTimePicker;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox AppIdText;
        private System.Windows.Forms.Button EditButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.TextBox TokenText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}