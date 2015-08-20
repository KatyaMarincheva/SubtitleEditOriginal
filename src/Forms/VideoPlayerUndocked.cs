// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlayerUndocked.cs" company="">
//   
// </copyright>
// <summary>
//   The video player undocked.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Controls;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The video player undocked.
    /// </summary>
    public partial class VideoPlayerUndocked : PositionAndSizeForm
    {
        /// <summary>
        /// The _main form.
        /// </summary>
        private Main _mainForm = null;

        /// <summary>
        /// The _redock keys.
        /// </summary>
        private Keys _redockKeys;

        /// <summary>
        /// The _video player container.
        /// </summary>
        private VideoPlayerContainer _videoPlayerContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoPlayerUndocked"/> class.
        /// </summary>
        /// <param name="main">
        /// The main.
        /// </param>
        /// <param name="videoPlayerContainer">
        /// The video player container.
        /// </param>
        public VideoPlayerUndocked(Main main, VideoPlayerContainer videoPlayerContainer)
        {
            this.InitializeComponent();
            this._mainForm = main;
            this.Icon = (Icon)this._mainForm.Icon.Clone();
            this._videoPlayerContainer = videoPlayerContainer;
            this._redockKeys = Utilities.GetKeys(Configuration.Settings.Shortcuts.MainVideoToggleVideoControls);
            this.RedockOnFullscreenEnd = false;
            videoPlayerContainer.TextBox.MouseMove += this.VideoPlayerUndocked_MouseMove;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoPlayerUndocked"/> class.
        /// </summary>
        public VideoPlayerUndocked()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether redock on fullscreen end.
        /// </summary>
        public bool RedockOnFullscreenEnd { get; set; }

        /// <summary>
        /// Gets the panel container.
        /// </summary>
        public Panel PanelContainer
        {
            get
            {
                return this.panelContainer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is fullscreen.
        /// </summary>
        internal bool IsFullscreen
        {
            get
            {
                return this.WindowState == FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// The video player undocked_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoPlayerUndocked_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.RedockOnFullscreenEnd)
            {
                this._mainForm.RedockVideoControlsToolStripMenuItemClick(null, null);
            }
            else if (e.CloseReason == CloseReason.UserClosing && this.panelContainer.Controls.Count > 0)
            {
                var control = this.panelContainer.Controls[0];
                if (control is Panel)
                {
                    this.panelContainer.Controls.Clear();
                    this._mainForm.ReDockVideoPlayer(control);
                    this._mainForm.SetVideoPlayerToggleOff();
                }
            }
        }

        /// <summary>
        /// The video player undocked_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoPlayerUndocked_KeyDown(object sender, KeyEventArgs e)
        {
            this.VideoPlayerUndocked_MouseMove(null, null);

            if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
            {
                this._videoPlayerContainer.TogglePlayPause();
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Enter)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    e.SuppressKeyPress = true;
                    this.NoFullscreen();
                }
                else if (this.WindowState == FormWindowState.Normal)
                {
                    this.GoFullscreen();
                }

                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Escape && this.WindowState == FormWindowState.Maximized)
            {
                e.SuppressKeyPress = true;
                this.NoFullscreen();
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.A)
            {
                if (this._videoPlayerContainer.VideoWidth > 0 && this._videoPlayerContainer.VideoHeight > 0)
                {
                    int wDiff = this._videoPlayerContainer.VideoWidth - this._videoPlayerContainer.PanelPlayer.Width;
                    int hDiff = this._videoPlayerContainer.VideoHeight - this._videoPlayerContainer.PanelPlayer.Height;
                    this.Width += wDiff;
                    this.Height += hDiff;
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt && this.WindowState == FormWindowState.Maximized)
            {
                this._mainForm.GotoPrevSubPosFromvideoPos();
                e.Handled = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Down && this.WindowState == FormWindowState.Maximized)
            {
                this._mainForm.GotoNextSubPosFromVideoPos();
                e.Handled = true;
            }
            else if (this._redockKeys == e.KeyData)
            {
                this._mainForm.RedockVideoControlsToolStripMenuItemClick(null, null);
                e.SuppressKeyPress = true;
            }
            else
            {
                this._mainForm.MainKeyDown(sender, e);
            }
        }

        /// <summary>
        /// The video player undocked_ mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoPlayerUndocked_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.timer1.Enabled)
            {
                this.timer1.Stop();
            }

            this._videoPlayerContainer.ShowControls();
            this.timer1.Start();
        }

        /// <summary>
        /// The timer 1_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            if (this.WindowState == FormWindowState.Maximized)
            {
                this._videoPlayerContainer.HideControls();
            }
        }

        /// <summary>
        /// The go fullscreen.
        /// </summary>
        internal void GoFullscreen()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this._videoPlayerContainer.FontSizeFactor = 1.5F;
            this._videoPlayerContainer.SetSubtitleFont();
            this._videoPlayerContainer.SubtitleText = string.Empty;
            this._videoPlayerContainer.ShowFullScreenControls();
            this.timer1.Start();
        }

        /// <summary>
        /// The no fullscreen.
        /// </summary>
        internal void NoFullscreen()
        {
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.WindowState = FormWindowState.Normal;
            this._videoPlayerContainer.FontSizeFactor = 1.0F;
            this._videoPlayerContainer.SetSubtitleFont();
            this._videoPlayerContainer.SubtitleText = string.Empty;
            this._videoPlayerContainer.ShowFullscreenButton = Configuration.Settings.General.VideoPlayerShowFullscreenButton;
            this._videoPlayerContainer.ShowNonFullScreenControls();
            if (this.RedockOnFullscreenEnd)
            {
                this.Close();
            }
        }

        /// <summary>
        /// The video player undocked_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoPlayerUndocked_Shown(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }
}