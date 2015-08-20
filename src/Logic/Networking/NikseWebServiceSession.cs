// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NikseWebServiceSession.cs" company="">
//   
// </copyright>
// <summary>
//   The nikse web service session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;
    using System.Threading;

    using Nikse.SubtitleEdit.SeNetworkService;

    using Timer = System.Windows.Forms.Timer;

    /// <summary>
    /// The nikse web service session.
    /// </summary>
    public class NikseWebServiceSession : IDisposable
    {
        /// <summary>
        /// The _se ws.
        /// </summary>
        private SeService _seWs;

        /// <summary>
        /// The _se ws last update.
        /// </summary>
        private DateTime _seWsLastUpdate = DateTime.Now.AddYears(-1);

        /// <summary>
        /// The _timer web service.
        /// </summary>
        private Timer _timerWebService;

        /// <summary>
        /// The chat log.
        /// </summary>
        public List<ChatEntry> ChatLog = new List<ChatEntry>();

        /// <summary>
        /// The file name.
        /// </summary>
        public string FileName;

        /// <summary>
        /// The last subtitle.
        /// </summary>
        public Subtitle LastSubtitle;

        /// <summary>
        /// The log.
        /// </summary>
        public StringBuilder Log;

        /// <summary>
        /// The original subtitle.
        /// </summary>
        public Subtitle OriginalSubtitle;

        /// <summary>
        /// The session id.
        /// </summary>
        public string SessionId;

        /// <summary>
        /// The subtitle.
        /// </summary>
        public Subtitle Subtitle;

        /// <summary>
        /// The update log.
        /// </summary>
        public List<UpdateLogEntry> UpdateLog = new List<UpdateLogEntry>();

        /// <summary>
        /// The user name.
        /// </summary>
        public string UserName;

        /// <summary>
        /// The users.
        /// </summary>
        public List<SeUser> Users;

        /// <summary>
        /// Initializes a new instance of the <see cref="NikseWebServiceSession"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="originalSubtitle">
        /// The original subtitle.
        /// </param>
        /// <param name="onUpdateTimerTick">
        /// The on update timer tick.
        /// </param>
        /// <param name="onUpdateUserLogEntries">
        /// The on update user log entries.
        /// </param>
        public NikseWebServiceSession(Subtitle subtitle, Subtitle originalSubtitle, EventHandler onUpdateTimerTick, EventHandler onUpdateUserLogEntries)
        {
            this.Subtitle = subtitle;
            this.OriginalSubtitle = originalSubtitle;
            this._timerWebService = new Timer();
            if (Configuration.Settings.NetworkSettings.PollIntervalSeconds < 1)
            {
                Configuration.Settings.NetworkSettings.PollIntervalSeconds = 1;
            }

            this._timerWebService.Interval = Configuration.Settings.NetworkSettings.PollIntervalSeconds * 1000;
            this._timerWebService.Tick += this.TimerWebServiceTick;
            this.Log = new StringBuilder();
            this.OnUpdateTimerTick = onUpdateTimerTick;
            this.OnUpdateUserLogEntries = onUpdateUserLogEntries;
        }

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        public SeUser CurrentUser { get; set; }

        /// <summary>
        /// Gets the web service url.
        /// </summary>
        public string WebServiceUrl
        {
            get
            {
                return this._seWs.Url;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The on update timer tick.
        /// </summary>
        public event EventHandler OnUpdateTimerTick;

        /// <summary>
        /// The on update user log entries.
        /// </summary>
        public event EventHandler OnUpdateUserLogEntries;

        /// <summary>
        /// The start server.
        /// </summary>
        /// <param name="webServiceUrl">
        /// The web service url.
        /// </param>
        /// <param name="sessionKey">
        /// The session key.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void StartServer(string webServiceUrl, string sessionKey, string userName, string fileName, out string message)
        {
            this.SessionId = sessionKey;
            this.UserName = userName;
            this.FileName = fileName;
            List<SeSequence> list = new List<SeSequence>();
            foreach (Paragraph p in this.Subtitle.Paragraphs)
            {
                list.Add(new SeSequence { StartMilliseconds = (int)p.StartTime.TotalMilliseconds, EndMilliseconds = (int)p.EndTime.TotalMilliseconds, Text = WebUtility.HtmlEncode(p.Text.Replace(Environment.NewLine, "<br />")) });
            }

            List<SeSequence> originalSubtitle = new List<SeSequence>();
            if (this.OriginalSubtitle != null)
            {
                foreach (Paragraph p in this.OriginalSubtitle.Paragraphs)
                {
                    originalSubtitle.Add(new SeSequence { StartMilliseconds = (int)p.StartTime.TotalMilliseconds, EndMilliseconds = (int)p.EndTime.TotalMilliseconds, Text = WebUtility.HtmlEncode(p.Text.Replace(Environment.NewLine, "<br />")) });
                }
            }

            this._seWs = new SeService();
            this._seWs.Url = webServiceUrl;
            this._seWs.Proxy = Utilities.GetProxy();
            SeUser user = this._seWs.Start(sessionKey, userName, list.ToArray(), originalSubtitle.ToArray(), fileName, out message);
            this.CurrentUser = user;
            this.Users = new List<SeUser>();
            this.Users.Add(user);
            if (message == "OK")
            {
                this._timerWebService.Start();
            }
        }

        /// <summary>
        /// The join.
        /// </summary>
        /// <param name="webServiceUrl">
        /// The web service url.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="sessionKey">
        /// The session key.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Join(string webServiceUrl, string userName, string sessionKey, out string message)
        {
            this.SessionId = sessionKey;
            this._seWs = new SeService();
            this._seWs.Url = webServiceUrl;
            this._seWs.Proxy = Utilities.GetProxy();
            this.Users = new List<SeUser>();
            SeUser[] users = this._seWs.Join(sessionKey, userName, out message);
            if (message != "OK")
            {
                return false;
            }

            string tempFileName;
            DateTime updateTime;
            this.Subtitle = new Subtitle();
            foreach (SeSequence sequence in this._seWs.GetSubtitle(sessionKey, out tempFileName, out updateTime))
            {
                this.Subtitle.Paragraphs.Add(new Paragraph(WebUtility.HtmlDecode(sequence.Text).Replace("<br />", Environment.NewLine), sequence.StartMilliseconds, sequence.EndMilliseconds));
            }

            this.FileName = tempFileName;

            this.OriginalSubtitle = new Subtitle();
            SeSequence[] sequences = this._seWs.GetOriginalSubtitle(sessionKey);
            if (sequences != null)
            {
                foreach (SeSequence sequence in sequences)
                {
                    this.OriginalSubtitle.Paragraphs.Add(new Paragraph(WebUtility.HtmlDecode(sequence.Text).Replace("<br />", Environment.NewLine), sequence.StartMilliseconds, sequence.EndMilliseconds));
                }
            }

            this.SessionId = sessionKey;
            this.CurrentUser = users[users.Length - 1]; // me
            foreach (SeUser user in users)
            {
                this.Users.Add(user);
            }

            this.ReloadFromWs();
            this._timerWebService.Start();
            return true;
        }

        /// <summary>
        /// The timer stop.
        /// </summary>
        public void TimerStop()
        {
            this._timerWebService.Stop();
        }

        /// <summary>
        /// The timer start.
        /// </summary>
        public void TimerStart()
        {
            this._timerWebService.Start();
        }

        /// <summary>
        /// The get updates.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="numberOfLines">
        /// The number of lines.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<SeUpdate> GetUpdates(out string message, out int numberOfLines)
        {
            List<SeUpdate> list = new List<SeUpdate>();
            DateTime newUpdateTime;
            SeUpdate[] updates = this._seWs.GetUpdates(this.SessionId, this.CurrentUser.UserName, this._seWsLastUpdate, out message, out newUpdateTime, out numberOfLines);
            if (updates != null)
            {
                foreach (SeUpdate update in updates)
                {
                    list.Add(update);
                }
            }

            this._seWsLastUpdate = newUpdateTime;
            return list;
        }

        /// <summary>
        /// The reload subtitle.
        /// </summary>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public Subtitle ReloadSubtitle()
        {
            this.Subtitle.Paragraphs.Clear();
            string tempFileName;
            DateTime updateTime;
            SeSequence[] sequences = this._seWs.GetSubtitle(this.SessionId, out tempFileName, out updateTime);
            this.FileName = tempFileName;
            this._seWsLastUpdate = updateTime;
            if (sequences != null)
            {
                foreach (SeSequence sequence in sequences)
                {
                    this.Subtitle.Paragraphs.Add(new Paragraph(WebUtility.HtmlDecode(sequence.Text).Replace("<br />", Environment.NewLine), sequence.StartMilliseconds, sequence.EndMilliseconds));
                }
            }

            return this.Subtitle;
        }

        /// <summary>
        /// The append to log.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public void AppendToLog(string text)
        {
            string timestamp = DateTime.Now.ToLongTimeString();
            this.Log.AppendLine(timestamp + ": " + text.TrimEnd().Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
        }

        /// <summary>
        /// The get log.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetLog()
        {
            return this.Log.ToString();
        }

        /// <summary>
        /// The send chat message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void SendChatMessage(string message)
        {
            this._seWs.SendMessage(this.SessionId, WebUtility.HtmlEncode(message.Replace(Environment.NewLine, "<br />")), this.CurrentUser);
        }

        /// <summary>
        /// The check for and submit updates.
        /// </summary>
        public void CheckForAndSubmitUpdates()
        {
            // check for changes in text/time codes (not insert/delete)
            if (this.LastSubtitle != null)
            {
                for (int i = 0; i < this.Subtitle.Paragraphs.Count; i++)
                {
                    Paragraph last = this.LastSubtitle.GetParagraphOrDefault(i);
                    Paragraph current = this.Subtitle.GetParagraphOrDefault(i);
                    if (last != null && current != null)
                    {
                        if (last.StartTime.TotalMilliseconds != current.StartTime.TotalMilliseconds || last.EndTime.TotalMilliseconds != current.EndTime.TotalMilliseconds || last.Text != current.Text)
                        {
                            this.UpdateLine(i, current);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The add to ws user log.
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="pos">
        /// The pos.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="updateUI">
        /// The update ui.
        /// </param>
        public void AddToWsUserLog(SeUser user, int pos, string action, bool updateUI)
        {
            for (int i = 0; i < this.UpdateLog.Count; i++)
            {
                if (this.UpdateLog[i].Index == pos)
                {
                    this.UpdateLog.RemoveAt(i);
                    break;
                }
            }

            this.UpdateLog.Add(new UpdateLogEntry(0, user.UserName, pos, action));
            if (updateUI && this.OnUpdateUserLogEntries != null)
            {
                this.OnUpdateUserLogEntries.Invoke(null, null);
            }
        }

        /// <summary>
        /// The update line.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        internal void UpdateLine(int index, Paragraph paragraph)
        {
            this._seWs.UpdateLine(this.SessionId, index, new SeSequence { StartMilliseconds = (int)paragraph.StartTime.TotalMilliseconds, EndMilliseconds = (int)paragraph.EndTime.TotalMilliseconds, Text = WebUtility.HtmlEncode(paragraph.Text.Replace(Environment.NewLine, "<br />")) }, this.CurrentUser);
            this.AddToWsUserLog(this.CurrentUser, index, "UPD", true);
        }

        /// <summary>
        /// The leave.
        /// </summary>
        internal void Leave()
        {
            try
            {
                this._seWs.Leave(this.SessionId, this.CurrentUser.UserName);
            }
            catch
            {
            }
        }

        /// <summary>
        /// The delete lines.
        /// </summary>
        /// <param name="indices">
        /// The indices.
        /// </param>
        internal void DeleteLines(List<int> indices)
        {
            this._seWs.DeleteLines(this.SessionId, indices.ToArray(), this.CurrentUser);
            StringBuilder sb = new StringBuilder();
            foreach (int index in indices)
            {
                sb.Append(index + ", ");
                this.AdjustUpdateLogToDelete(index);
                this.AppendToLog(string.Format(Configuration.Settings.Language.Main.NetworkDelete, this.CurrentUser.UserName, this.CurrentUser.Ip, index));
            }
        }

        /// <summary>
        /// The insert line.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="newParagraph">
        /// The new paragraph.
        /// </param>
        internal void InsertLine(int index, Paragraph newParagraph)
        {
            this._seWs.InsertLine(this.SessionId, index, (int)newParagraph.StartTime.TotalMilliseconds, (int)newParagraph.EndTime.TotalMilliseconds, newParagraph.Text, this.CurrentUser);
            this.AppendToLog(string.Format(Configuration.Settings.Language.Main.NetworkInsert, this.CurrentUser.UserName, this.CurrentUser.Ip, index, newParagraph.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString)));
        }

        /// <summary>
        /// The adjust update log to insert.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        internal void AdjustUpdateLogToInsert(int index)
        {
            foreach (UpdateLogEntry logEntry in this.UpdateLog)
            {
                if (logEntry.Index >= index)
                {
                    logEntry.Index++;
                }
            }
        }

        /// <summary>
        /// The adjust update log to delete.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        internal void AdjustUpdateLogToDelete(int index)
        {
            UpdateLogEntry removeThis = null;
            foreach (UpdateLogEntry logEntry in this.UpdateLog)
            {
                if (logEntry.Index == index)
                {
                    removeThis = logEntry;
                }
                else if (logEntry.Index > index)
                {
                    logEntry.Index--;
                }
            }

            if (removeThis != null)
            {
                this.UpdateLog.Remove(removeThis);
            }
        }

        /// <summary>
        /// The restart.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string Restart()
        {
            string message = string.Empty;
            int retries = 0;
            const int maxRetries = 10;
            while (retries < maxRetries)
            {
                try
                {
                    Thread.Sleep(200);
                    this.StartServer(this._seWs.Url, this.SessionId, this.UserName, this.FileName, out message);
                    retries = maxRetries;
                }
                catch
                {
                    Thread.Sleep(200);
                    retries++;
                }
            }

            if (message == "Session is already running")
            {
                return this.ReJoin();
            }

            return message;
        }

        /// <summary>
        /// The re join.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string ReJoin()
        {
            string message = string.Empty;
            int retries = 0;
            const int maxRetries = 10;
            while (retries < maxRetries)
            {
                try
                {
                    Thread.Sleep(200);
                    if (this.Join(this._seWs.Url, this.UserName, this.SessionId, out message))
                    {
                        message = "Reload";
                    }

                    retries = maxRetries;
                }
                catch
                {
                    Thread.Sleep(200);
                    retries++;
                }
            }

            return message;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._timerWebService != null)
                {
                    this._timerWebService.Dispose();
                    this._timerWebService = null;
                }

                if (this._seWs != null)
                {
                    this._seWs.Dispose();
                    this._seWs = null;
                }
            }
        }

        /// <summary>
        /// The timer web service tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TimerWebServiceTick(object sender, EventArgs e)
        {
            if (this.OnUpdateTimerTick != null)
            {
                this.OnUpdateTimerTick.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The reload from ws.
        /// </summary>
        private void ReloadFromWs()
        {
            if (this._seWs != null)
            {
                this.Subtitle = new Subtitle();
                SeSequence[] sequences = this._seWs.GetSubtitle(this.SessionId, out this.FileName, out this._seWsLastUpdate);
                foreach (SeSequence sequence in sequences)
                {
                    Paragraph p = new Paragraph(WebUtility.HtmlDecode(sequence.Text).Replace("<br />", Environment.NewLine), sequence.StartMilliseconds, sequence.EndMilliseconds);
                    this.Subtitle.Paragraphs.Add(p);
                }

                this.Subtitle.Renumber();
                this.LastSubtitle = new Subtitle(this.Subtitle);
            }
        }

        /// <summary>
        /// The chat entry.
        /// </summary>
        public class ChatEntry
        {
            /// <summary>
            /// Gets or sets the user.
            /// </summary>
            public SeUser User { get; set; }

            /// <summary>
            /// Gets or sets the message.
            /// </summary>
            public string Message { get; set; }
        }
    }
}