// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkStart.cs" company="">
//   
// </copyright>
// <summary>
//   The network start.
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
    /// The network start.
    /// </summary>
    public sealed partial class NetworkStart : PositionAndSizeForm
    {
        /// <summary>
        /// The _file name.
        /// </summary>
        private string _fileName;

        /// <summary>
        /// The _network session.
        /// </summary>
        private NikseWebServiceSession _networkSession;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkStart"/> class.
        /// </summary>
        public NetworkStart()
        {
            this.InitializeComponent();
            this.labelStatus.Text = string.Empty;
            this.Text = Configuration.Settings.Language.NetworkStart.Title;
            this.labelInfo.Text = Configuration.Settings.Language.NetworkStart.Information;
            this.labelSessionKey.Text = Configuration.Settings.Language.General.SessionKey;
            this.labelUserName.Text = Configuration.Settings.Language.General.UserName;
            this.labelWebServiceUrl.Text = Configuration.Settings.Language.General.WebServiceUrl;
            this.buttonStart.Text = Configuration.Settings.Language.NetworkStart.Start;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="networkSession">
        /// The network session.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        internal void Initialize(NikseWebServiceSession networkSession, string fileName)
        {
            this._networkSession = networkSession;
            this._fileName = fileName;

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
        /// The button start_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonStart_Click(object sender, EventArgs e)
        {
            Configuration.Settings.NetworkSettings.SessionKey = this.textBoxSessionKey.Text;
            Configuration.Settings.NetworkSettings.WebServiceUrl = this.comboBoxWebServiceUrl.Text;
            Configuration.Settings.NetworkSettings.UserName = this.textBoxUserName.Text;

            this.buttonStart.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.textBoxSessionKey.Enabled = false;
            this.textBoxUserName.Enabled = false;
            this.comboBoxWebServiceUrl.Enabled = false;
            this.labelStatus.Text = string.Format(Configuration.Settings.Language.NetworkStart.ConnectionTo, this.comboBoxWebServiceUrl.Text);
            this.Refresh();

            try
            {
                string message;
                this._networkSession.StartServer(this.comboBoxWebServiceUrl.Text, this.textBoxSessionKey.Text, this.textBoxUserName.Text, this._fileName, out message);
                if (message != "OK")
                {
                    MessageBox.Show(message);
                }
                else
                {
                    this.DialogResult = DialogResult.OK;
                    return;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

            this.buttonStart.Enabled = true;
            this.buttonCancel.Enabled = true;
            this.textBoxSessionKey.Enabled = false;
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
        /// The network new_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NetworkNew_KeyDown(object sender, KeyEventArgs e)
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