// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkLogAndInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The network log and info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Networking;

    /// <summary>
    /// The network log and info.
    /// </summary>
    public sealed partial class NetworkLogAndInfo : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkLogAndInfo"/> class.
        /// </summary>
        public NetworkLogAndInfo()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.NetworkLogAndInfo.Title;
            this.labelSessionKey.Text = Configuration.Settings.Language.General.SessionKey;
            this.labelUserName.Text = Configuration.Settings.Language.General.UserName;
            this.labelWebServiceUrl.Text = Configuration.Settings.Language.General.WebServiceUrl;
            this.labelLog.Text = Configuration.Settings.Language.NetworkLogAndInfo.Log;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="_networkSession">
        /// The _network session.
        /// </param>
        internal void Initialize(NikseWebServiceSession _networkSession)
        {
            this.textBoxSessionKey.Text = _networkSession.SessionId;
            this.textBoxUserName.Text = _networkSession.CurrentUser.UserName;
            this.textBoxWebServiceUrl.Text = _networkSession.WebServiceUrl;
            this.textBoxLog.Text = _networkSession.GetLog();
        }

        /// <summary>
        /// The button o k_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The network log and info_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NetworkLogAndInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}