// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkJoin.cs" company="">
//   
// </copyright>
// <summary>
//   The network join.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Net;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Networking;

    /// <summary>
    /// The network join.
    /// </summary>
    public partial class NetworkJoin : Form
    {
        /// <summary>
        /// The _network session.
        /// </summary>
        private NikseWebServiceSession _networkSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkJoin"/> class.
        /// </summary>
        public NetworkJoin()
        {
            this.InitializeComponent();
            this.labelStatus.Text = string.Empty;
            this.Text = Configuration.Settings.Language.NetworkJoin.Title;
            this.labelInfo.Text = Configuration.Settings.Language.NetworkJoin.Information;
            this.labelSessionKey.Text = Configuration.Settings.Language.General.SessionKey;
            this.labelUserName.Text = Configuration.Settings.Language.General.UserName;
            this.labelWebServiceUrl.Text = Configuration.Settings.Language.General.WebServiceUrl;
            this.buttonJoin.Text = Configuration.Settings.Language.NetworkJoin.Join;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="networkSession">
        /// The network session.
        /// </param>
        internal void Initialize(NikseWebServiceSession networkSession)
        {
            this._networkSession = networkSession;

            this.textBoxSessionKey.Text = Configuration.Settings.NetworkSettings.SessionKey;
            if (this.textBoxSessionKey.Text.Trim().Length < 2)
            {
                this.textBoxSessionKey.Text = Guid.NewGuid().ToString().Replace("-", string.Empty);
            }

            this.comboBoxWebServiceUrl.Text = Configuration.Settings.NetworkSettings.WebServiceUrl;
            this.textBoxUserName.Text = Configuration.Settings.NetworkSettings.UserName;
            if (this.textBoxUserName.Text.Trim().Length < 2)
            {
                this.textBoxUserName.Text = Dns.GetHostName();
            }
        }

        /// <summary>
        /// The button join_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonJoin_Click(object sender, EventArgs e)
        {
            Configuration.Settings.NetworkSettings.SessionKey = this.textBoxSessionKey.Text;
            Configuration.Settings.NetworkSettings.WebServiceUrl = this.comboBoxWebServiceUrl.Text;
            Configuration.Settings.NetworkSettings.UserName = this.textBoxUserName.Text;

            this.buttonJoin.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.textBoxUserName.Enabled = false;
            this.comboBoxWebServiceUrl.Enabled = false;
            this.labelStatus.Text = string.Format(Configuration.Settings.Language.NetworkStart.ConnectionTo, this.comboBoxWebServiceUrl.Text);
            this.Refresh();

            try
            {
                string message;
                this._networkSession.Join(this.comboBoxWebServiceUrl.Text, this.textBoxUserName.Text, this.textBoxSessionKey.Text, out message);
                if (message == "OK")
                {
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    if (message == "Session not found!")
                    {
                        MessageBox.Show(string.Format(Configuration.Settings.Language.Main.XNotFound, this.textBoxSessionKey.Text));
                    }
                    else if (message == "Username already in use!")
                    {
                        MessageBox.Show(string.Format(Configuration.Settings.Language.General.UserNameAlreadyInUse, this.textBoxSessionKey.Text));
                    }
                    else
                    {
                        MessageBox.Show(message);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            this.buttonJoin.Enabled = true;
            this.buttonCancel.Enabled = true;
            this.textBoxUserName.Enabled = true;
            this.comboBoxWebServiceUrl.Enabled = true;
            this.labelStatus.Text = string.Empty;
        }

        /// <summary>
        /// The button cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The network join_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NetworkJoin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#networking");
                e.SuppressKeyPress = true;
            }
        }
    }
}