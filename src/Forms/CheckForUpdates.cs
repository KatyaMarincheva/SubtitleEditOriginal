// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckForUpdates.cs" company="">
//   
// </copyright>
// <summary>
//   The check for updates.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Forms;

    /// <summary>
    /// The check for updates.
    /// </summary>
    public partial class CheckForUpdates : Form
    {
        /// <summary>
        /// The _main form.
        /// </summary>
        private Main _mainForm;

        /// <summary>
        /// The _perform check on shown.
        /// </summary>
        private bool _performCheckOnShown = true;

        /// <summary>
        /// The _seconds.
        /// </summary>
        private double _seconds = 0;

        /// <summary>
        /// The _updates helper.
        /// </summary>
        private CheckForUpdatesHelper _updatesHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckForUpdates"/> class.
        /// </summary>
        /// <param name="mainForm">
        /// The main form.
        /// </param>
        public CheckForUpdates(Main mainForm)
        {
            this.InitializeComponent();

            this._mainForm = mainForm;
            this.InitLanguage();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckForUpdates"/> class.
        /// </summary>
        /// <param name="mainForm">
        /// The main form.
        /// </param>
        /// <param name="checkForUpdatesHelper">
        /// The check for updates helper.
        /// </param>
        public CheckForUpdates(Main mainForm, CheckForUpdatesHelper checkForUpdatesHelper)
        {
            this.InitializeComponent();

            this._mainForm = mainForm;
            this._updatesHelper = checkForUpdatesHelper;
            this.InitLanguage();
            this.ShowAvailableUpdate(true);
            this._performCheckOnShown = false;
        }

        /// <summary>
        /// The init language.
        /// </summary>
        private void InitLanguage()
        {
            this.Text = Configuration.Settings.Language.CheckForUpdates.Title;
            this.labelStatus.Text = Configuration.Settings.Language.CheckForUpdates.CheckingForUpdates;
            this.buttonDownloadAndInstall.Text = Configuration.Settings.Language.CheckForUpdates.InstallUpdate;
            this.buttonDownloadAndInstall.Visible = false;
            this.textBoxChangeLog.Visible = false;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonOK.Visible = false;
            this.buttonDontCheckUpdates.Text = Configuration.Settings.Language.CheckForUpdates.NoUpdates;
            this.buttonDontCheckUpdates.Visible = false;

            this.Location = new System.Drawing.Point(this._mainForm.Location.X + (this._mainForm.Width / 2) - (this.Width / 2), this._mainForm.Location.Y + (this._mainForm.Height / 2) - (this.Height / 2) - 200);
        }

        /// <summary>
        /// The check for updates_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckForUpdates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The check for updates_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckForUpdates_Shown(object sender, EventArgs e)
        {
            if (!this._performCheckOnShown)
            {
                this.Activate();
                return;
            }

            this._updatesHelper = new CheckForUpdatesHelper();
            Application.DoEvents();
            this.Refresh();
            this._updatesHelper.CheckForUpdates();
            this.timerCheckForUpdates.Start();

            this.buttonOK.Focus();
            this.Activate();
        }

        /// <summary>
        /// The timer check for updates_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timerCheckForUpdates_Tick(object sender, EventArgs e)
        {
            if (this._seconds > 10)
            {
                this.timerCheckForUpdates.Stop();
                this.labelStatus.Text = string.Format(Configuration.Settings.Language.CheckForUpdates.CheckingForUpdatesFailedX, "Time out");
                this.buttonOK.Visible = true;
            }
            else if (this._updatesHelper.Error != null)
            {
                this.timerCheckForUpdates.Stop();
                this.labelStatus.Text = string.Format(Configuration.Settings.Language.CheckForUpdates.CheckingForUpdatesFailedX, this._updatesHelper.Error);
                this.buttonOK.Visible = true;
            }
            else if (this._updatesHelper.Done)
            {
                this.timerCheckForUpdates.Stop();
                if (this._updatesHelper.IsUpdateAvailable())
                {
                    this.ShowAvailableUpdate(false);
                }
                else
                {
                    this.labelStatus.Text = Configuration.Settings.Language.CheckForUpdates.CheckingForUpdatesNoneAvailable;
                    this.SetLargeSize();
                    this.textBoxChangeLog.Text = this._updatesHelper.LatestChangeLog;
                    this.textBoxChangeLog.Visible = true;
                    this.buttonOK.Visible = true;
                }
            }

            this._seconds += this.timerCheckForUpdates.Interval / TimeCode.BaseUnit;

            if (this.buttonDownloadAndInstall.Visible)
            {
                this.buttonDownloadAndInstall.Focus();
            }
            else if (this.buttonOK.Visible)
            {
                this.buttonOK.Focus();
            }
        }

        /// <summary>
        /// The set large size.
        /// </summary>
        private void SetLargeSize()
        {
            this.Height = 600;
            this.MinimumSize = new System.Drawing.Size(500, 400);
        }

        /// <summary>
        /// The show available update.
        /// </summary>
        /// <param name="fromAutoCheck">
        /// The from auto check.
        /// </param>
        private void ShowAvailableUpdate(bool fromAutoCheck)
        {
            this.SetLargeSize();
            this.textBoxChangeLog.Text = this._updatesHelper.LatestChangeLog;
            this.textBoxChangeLog.Visible = true;
            this.labelStatus.Text = Configuration.Settings.Language.CheckForUpdates.CheckingForUpdatesNewVersion;
            this.buttonDownloadAndInstall.Visible = true;
            this.buttonOK.Visible = true;
            if (Configuration.Settings.General.CheckForUpdates && fromAutoCheck)
            {
                this.buttonDontCheckUpdates.Visible = true;
            }
            else
            {
                this.buttonDontCheckUpdates.Visible = false;
                this.buttonDownloadAndInstall.Left = this.buttonOK.Left - 6 - this.buttonDownloadAndInstall.Width;
            }
        }

        /// <summary>
        /// The button download and install_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonDownloadAndInstall_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SubtitleEdit/subtitleedit/releases");
        }

        /// <summary>
        /// The button dont check updates_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonDontCheckUpdates_Click(object sender, EventArgs e)
        {
            Configuration.Settings.General.CheckForUpdates = false;
            this.DialogResult = DialogResult.Cancel;
        }
    }
}