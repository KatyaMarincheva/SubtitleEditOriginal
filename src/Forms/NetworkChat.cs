// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkChat.cs" company="">
//   
// </copyright>
// <summary>
//   The network chat.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Networking;
    using Nikse.SubtitleEdit.SeNetworkService;

    /// <summary>
    /// The network chat.
    /// </summary>
    public sealed partial class NetworkChat : Form
    {
        /// <summary>
        /// The _network session.
        /// </summary>
        private NikseWebServiceSession _networkSession;

        /// <summary>
        /// The break chars.
        /// </summary>
        private string breakChars = "\".!?,)([]<>:;♪{}-/#*| ¿¡" + Environment.NewLine + "\t";

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkChat"/> class.
        /// </summary>
        public NetworkChat()
        {
            this.InitializeComponent();
            this.buttonSendChat.Text = Configuration.Settings.Language.NetworkChat.Send;
            this.listViewUsers.Columns[0].Text = Configuration.Settings.Language.General.UserName;
            this.listViewUsers.Columns[1].Text = Configuration.Settings.Language.General.IP;
            this.listViewChat.Columns[0].Text = Configuration.Settings.Language.General.UserName;
            this.listViewChat.Columns[1].Text = Configuration.Settings.Language.General.Text;
        }

        /// <summary>
        /// Gets a value indicating whether show without activation.
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="networkSession">
        /// The network session.
        /// </param>
        internal void Initialize(NikseWebServiceSession networkSession)
        {
            this._networkSession = networkSession;
            this.Text = Configuration.Settings.Language.NetworkChat.Title + " - " + this._networkSession.CurrentUser.UserName;

            this.listViewUsers.Items.Clear();
            foreach (var user in this._networkSession.Users)
            {
                this.AddUser(user);
            }

            this.listViewChat.Items.Clear();
            foreach (var message in this._networkSession.ChatLog)
            {
                this.AddChatMessage(message.User, message.Message);
                this.listViewChat.EnsureVisible(this.listViewChat.Items.Count - 1);
            }
        }

        /// <summary>
        /// The button send chat_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSendChat_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.textBoxChat.Text))
            {
                this._networkSession.SendChatMessage(this.textBoxChat.Text.Trim());
                this.AddChatMessage(this._networkSession.CurrentUser, this.textBoxChat.Text.Trim());
                this.listViewChat.EnsureVisible(this.listViewChat.Items.Count - 1);
                this._networkSession.ChatLog.Add(new NikseWebServiceSession.ChatEntry { User = this._networkSession.CurrentUser, Message = this.textBoxChat.Text.Trim() });
            }

            this.textBoxChat.Text = string.Empty;
            this.textBoxChat.Focus();
        }

        /// <summary>
        /// The add chat message.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void AddChatMessage(SeUser user, string message)
        {
            ListViewItem item = new ListViewItem(user.UserName);
            item.Tag = this._networkSession.CurrentUser;
            item.ForeColor = Utilities.GetColorFromUserName(user.UserName);
            item.ImageIndex = Utilities.GetNumber0To7FromUserName(user.UserName);
            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, message));
            this.listViewChat.Items.Add(item);
        }

        /// <summary>
        /// The text box chat_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.None)
            {
                e.SuppressKeyPress = true;
                this.buttonSendChat_Click(null, null);
            }
            else
            {
                if (e.KeyData == (Keys.Control | Keys.A))
                {
                    this.textBoxChat.SelectAll();
                    e.SuppressKeyPress = true;
                }

                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Back)
                {
                    int index = this.textBoxChat.SelectionStart;
                    if (this.textBoxChat.SelectionLength == 0)
                    {
                        var s = this.textBoxChat.Text;
                        int deleteFrom = index - 1;

                        if (deleteFrom > 0 && deleteFrom < s.Length)
                        {
                            if (s[deleteFrom] == ' ')
                            {
                                deleteFrom--;
                            }

                            while (deleteFrom > 0 && !this.breakChars.Contains(s[deleteFrom]))
                            {
                                deleteFrom--;
                            }

                            if (deleteFrom == index - 1)
                            {
                                while (deleteFrom > 0 && this.breakChars.Replace(" ", string.Empty).Contains(s[deleteFrom - 1]))
                                {
                                    deleteFrom--;
                                }
                            }

                            if (s[deleteFrom] == ' ')
                            {
                                deleteFrom++;
                            }

                            this.textBoxChat.Text = s.Remove(deleteFrom, index - deleteFrom);
                            this.textBoxChat.SelectionStart = deleteFrom;
                        }
                    }

                    e.SuppressKeyPress = true;
                }
            }
        }

        /// <summary>
        /// The add user.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        internal void AddUser(SeUser user)
        {
            ListViewItem item = new ListViewItem(user.UserName);
            item.Tag = user;
            item.ForeColor = Utilities.GetColorFromUserName(user.UserName);
            if (DateTime.Now.Month == 12 && DateTime.Now.Day >= 23 && DateTime.Now.Day <= 25)
            {
                item.ImageIndex = 7;
            }
            else
            {
                item.ImageIndex = Utilities.GetNumber0To7FromUserName(user.UserName);
            }

            item.SubItems.Add(new ListViewItem.ListViewSubItem(item, user.Ip));
            this.listViewUsers.Items.Add(item);
        }

        /// <summary>
        /// The remove user.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        internal void RemoveUser(SeUser user)
        {
            ListViewItem removeItem = null;
            foreach (ListViewItem item in this.listViewUsers.Items)
            {
                if ((item.Tag as SeNetworkService.SeUser).UserName == user.UserName)
                {
                    removeItem = item;
                }
            }

            if (removeItem != null)
            {
                this.listViewUsers.Items.Remove(removeItem);
            }
        }
    }
}