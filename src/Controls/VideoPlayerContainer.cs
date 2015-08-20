// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoPlayerContainer.cs" company="">
//   
// </copyright>
// <summary>
//   The video player container.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Controls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Controls.Interfaces;
    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Interfaces;
    using Nikse.SubtitleEdit.Logic.VideoPlayers.Interfaces;

    /// <summary>
    /// The video player container.
    /// </summary>
    public sealed class VideoPlayerContainer : Panel, IVideoPlayerContainer
    {
        /// <summary>
        /// The subtitles height.
        /// </summary>
        private const int SubtitlesHeight = 57;

        /// <summary>
        /// The _background color.
        /// </summary>
        private readonly Color _backgroundColor = Color.FromArgb(18, 18, 18);

        /// <summary>
        /// The _label time code.
        /// </summary>
        private readonly Label _labelTimeCode = new Label();

        /// <summary>
        /// The _label video player name.
        /// </summary>
        private readonly Label _labelVideoPlayerName = new Label();

        /// <summary>
        /// The _picture box fullscreen.
        /// </summary>
        private readonly PictureBox _pictureBoxFullscreen = new PictureBox();

        /// <summary>
        /// The _picture box fullscreen down.
        /// </summary>
        private readonly PictureBox _pictureBoxFullscreenDown = new PictureBox();

        /// <summary>
        /// The _picture box fullscreen over.
        /// </summary>
        private readonly PictureBox _pictureBoxFullscreenOver = new PictureBox();

        /// <summary>
        /// The _picture box mute.
        /// </summary>
        private readonly PictureBox _pictureBoxMute = new PictureBox();

        /// <summary>
        /// The _picture box mute down.
        /// </summary>
        private readonly PictureBox _pictureBoxMuteDown = new PictureBox();

        /// <summary>
        /// The _picture box mute over.
        /// </summary>
        private readonly PictureBox _pictureBoxMuteOver = new PictureBox();

        /// <summary>
        /// The _picture box pause.
        /// </summary>
        private readonly PictureBox _pictureBoxPause = new PictureBox();

        /// <summary>
        /// The _picture box pause down.
        /// </summary>
        private readonly PictureBox _pictureBoxPauseDown = new PictureBox();

        /// <summary>
        /// The _picture box pause over.
        /// </summary>
        private readonly PictureBox _pictureBoxPauseOver = new PictureBox();

        /// <summary>
        /// The _picture box progress bar.
        /// </summary>
        private readonly PictureBox _pictureBoxProgressBar = new PictureBox();

        /// <summary>
        /// The _picture box progressbar background.
        /// </summary>
        private readonly PictureBox _pictureBoxProgressbarBackground = new PictureBox();

        /// <summary>
        /// The _picture box stop.
        /// </summary>
        private readonly PictureBox _pictureBoxStop = new PictureBox();

        /// <summary>
        /// The _picture box stop down.
        /// </summary>
        private readonly PictureBox _pictureBoxStopDown = new PictureBox();

        /// <summary>
        /// The _picture box stop over.
        /// </summary>
        private readonly PictureBox _pictureBoxStopOver = new PictureBox();

        /// <summary>
        /// The _picture box volume bar.
        /// </summary>
        private readonly PictureBox _pictureBoxVolumeBar = new PictureBox();

        /// <summary>
        /// The _picture box volume bar background.
        /// </summary>
        private readonly PictureBox _pictureBoxVolumeBarBackground = new PictureBox();

        /// <summary>
        /// The _resources.
        /// </summary>
        private readonly ComponentResourceManager _resources;

        /// <summary>
        /// The _controls height.
        /// </summary>
        private int _controlsHeight = 47;

        /// <summary>
        /// The _is muted.
        /// </summary>
        private bool _isMuted;

        /// <summary>
        /// The _mute old volume.
        /// </summary>
        private double? _muteOldVolume;

        /// <summary>
        /// The _panelcontrols.
        /// </summary>
        private Panel _panelcontrols;

        /// <summary>
        /// The _panel subtitle.
        /// </summary>
        private Panel _panelSubtitle;

        /// <summary>
        /// The _picture box background.
        /// </summary>
        private PictureBox _pictureBoxBackground;

        /// <summary>
        /// The _picture box fast forward.
        /// </summary>
        private PictureBox _pictureBoxFastForward;

        /// <summary>
        /// The _picture box fast forward down.
        /// </summary>
        private PictureBox _pictureBoxFastForwardDown;

        /// <summary>
        /// The _picture box fast forward over.
        /// </summary>
        private PictureBox _pictureBoxFastForwardOver;

        /// <summary>
        /// The _picture box play.
        /// </summary>
        private PictureBox _pictureBoxPlay;

        /// <summary>
        /// The _picture box play down.
        /// </summary>
        private PictureBox _pictureBoxPlayDown;

        /// <summary>
        /// The _picture box play over.
        /// </summary>
        private PictureBox _pictureBoxPlayOver;

        /// <summary>
        /// The _picture box reverse.
        /// </summary>
        private PictureBox _pictureBoxReverse;

        /// <summary>
        /// The _picture box reverse down.
        /// </summary>
        private PictureBox _pictureBoxReverseDown;

        /// <summary>
        /// The _picture box reverse over.
        /// </summary>
        private PictureBox _pictureBoxReverseOver;

        /// <summary>
        /// The _subtitle text.
        /// </summary>
        private string _subtitleText = string.Empty;

        /// <summary>
        /// The _subtitle text box.
        /// </summary>
        private RichTextBoxViewOnly _subtitleTextBox;

        /// <summary>
        /// The _video player.
        /// </summary>
        private IVideoPlayer _videoPlayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoPlayerContainer"/> class.
        /// </summary>
        public VideoPlayerContainer()
        {
            this.FontSizeFactor = 1.0F;
            this.BorderStyle = BorderStyle.None;
            this._resources = new System.ComponentModel.ComponentResourceManager(typeof(VideoPlayerContainer));
            this.BackColor = this._backgroundColor;
            this.Controls.Add(this.MakePlayerPanel());
            this.Controls.Add(this.MakeSubtitlesPanel());
            this.Controls.Add(this.MakeControlsPanel());
            this._panelcontrols.BringToFront();

            this.HideAllPlayImages();
            this.HideAllPauseImages();
            this._pictureBoxPlay.Visible = true;
            this._pictureBoxPlay.BringToFront();

            this.HideAllStopImages();
            this._pictureBoxStop.Visible = true;

            this.HideAllFullscreenImages();
            this._pictureBoxFullscreen.Visible = true;

            this.HideAllMuteImages();
            this._pictureBoxMute.Visible = true;

            this.HideAllReverseImages();
            this._pictureBoxReverse.Visible = true;

            this.HideAllFastForwardImages();
            this._pictureBoxFastForward.Visible = true;

            this.VideoPlayerContainerResize(this, null);
            this.Resize += this.VideoPlayerContainerResize;

            this._pictureBoxProgressBar.Width = 0;

            this.PanelPlayer.MouseDown += this.PanelPlayerMouseDown;
        }

        /// <summary>
        /// The on button clicked.
        /// </summary>
        public event EventHandler OnButtonClicked;

        /// <summary>
        /// Gets the panel player.
        /// </summary>
        public Panel PanelPlayer { get; private set; }

        /// <summary>
        /// Gets or sets the font size factor.
        /// </summary>
        public float FontSizeFactor { get; set; }

        /// <summary>
        /// Gets or sets the video player.
        /// </summary>
        public IVideoPlayer VideoPlayer
        {
            get
            {
                return this._videoPlayer;
            }

            set
            {
                this._videoPlayer = value;
                if (this._videoPlayer != null)
                {
                    this.SetPlayerName(this._videoPlayer.PlayerName);
                }
            }
        }

        /// <summary>
        /// Gets the text box.
        /// </summary>
        public RichTextBoxViewOnly TextBox
        {
            get
            {
                return this._subtitleTextBox;
            }
        }

        /// <summary>
        /// Gets or sets the video width.
        /// </summary>
        public int VideoWidth { get; set; }

        /// <summary>
        /// Gets or sets the video height.
        /// </summary>
        public int VideoHeight { get; set; }

        /// <summary>
        /// Gets or sets the text right to left.
        /// </summary>
        public RightToLeft TextRightToLeft
        {
            get
            {
                return this._subtitleTextBox.RightToLeft;
            }

            set
            {
                this._subtitleTextBox.RightToLeft = value;
                this._subtitleTextBox.SelectAll();
                this._subtitleTextBox.SelectionAlignment = HorizontalAlignment.Center;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show stop button.
        /// </summary>
        public bool ShowStopButton
        {
            get
            {
                return this._pictureBoxStop.Visible || this._pictureBoxStopOver.Visible || this._pictureBoxStopDown.Visible;
            }

            set
            {
                if (value)
                {
                    this._pictureBoxStop.Visible = true;
                }
                else
                {
                    this.HideAllStopImages();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show mute button.
        /// </summary>
        public bool ShowMuteButton
        {
            get
            {
                return this._pictureBoxMute.Visible || this._pictureBoxMuteOver.Visible || this._pictureBoxMuteDown.Visible;
            }

            set
            {
                if (value)
                {
                    this._pictureBoxMute.Visible = true;
                }
                else
                {
                    this.HideAllMuteImages();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show fullscreen button.
        /// </summary>
        public bool ShowFullscreenButton
        {
            get
            {
                return this._pictureBoxFullscreen.Visible || this._pictureBoxFullscreenOver.Visible || this._pictureBoxFullscreenDown.Visible;
            }

            set
            {
                if (value)
                {
                    this._pictureBoxFullscreen.Visible = true;
                }
                else
                {
                    this.HideAllFullscreenImages();
                }
            }
        }

        /// <summary>
        /// The enable mouse wheel step.
        /// </summary>
        public void EnableMouseWheelStep()
        {
            this.AddMouseWheelEvent(this);
        }

        /// <summary>
        /// The set player name.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        public void SetPlayerName(string s)
        {
            this._labelVideoPlayerName.Text = s;
            this._labelVideoPlayerName.Left = this.Width - this._labelVideoPlayerName.Width - 3;
        }

        /// <summary>
        /// The reset time label.
        /// </summary>
        public void ResetTimeLabel()
        {
            this._labelTimeCode.Text = string.Empty;
        }

        /// <summary>
        /// The add mouse wheel event.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        public void AddMouseWheelEvent(Control control)
        {
            control.MouseWheel += this.ControlMouseWheel;
            foreach (Control ctrl in control.Controls)
            {
                this.AddMouseWheelEvent(ctrl);
            }
        }

        /// <summary>
        /// The control mouse wheel.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void ControlMouseWheel(object sender, MouseEventArgs e)
        {
            int delta = e.Delta;
            double newPosition = this.CurrentPosition - (delta / 256.0);
            if (newPosition < 0)
            {
                newPosition = 0;
            }
            else if (newPosition > this.Duration)
            {
                newPosition = this.Duration;
            }

            this.CurrentPosition = newPosition;
        }

        /// <summary>
        /// The make subtitles panel.
        /// </summary>
        /// <returns>
        /// The <see cref="Control"/>.
        /// </returns>
        public Control MakeSubtitlesPanel()
        {
            this._panelSubtitle = new Panel { BackColor = this._backgroundColor, Left = 0, Top = 0, Height = SubtitlesHeight + 1 };
            this._subtitleTextBox = new RichTextBoxViewOnly();
            this._panelSubtitle.Controls.Add(this._subtitleTextBox);
            this._subtitleTextBox.BackColor = this._backgroundColor;
            this._subtitleTextBox.ForeColor = Color.White;
            this._subtitleTextBox.Dock = DockStyle.Fill;
            this.SetSubtitleFont();
            this._subtitleTextBox.MouseClick += this.SubtitleTextBoxMouseClick;
            return this._panelSubtitle;
        }

        /// <summary>
        /// The set subtitle font.
        /// </summary>
        public void SetSubtitleFont()
        {
            var gs = Configuration.Settings.General;
            if (string.IsNullOrEmpty(gs.SubtitleFontName))
            {
                gs.SubtitleFontName = "Tahoma";
            }

            if (gs.VideoPlayerPreviewFontBold)
            {
                this._subtitleTextBox.Font = new Font(gs.SubtitleFontName, gs.VideoPlayerPreviewFontSize * this.FontSizeFactor, FontStyle.Bold);
            }
            else
            {
                this._subtitleTextBox.Font = new Font(gs.SubtitleFontName, gs.VideoPlayerPreviewFontSize * this.FontSizeFactor, FontStyle.Regular);
            }

            this.SubtitleText = this._subtitleText;
        }

        /// <summary>
        /// The subtitle text box mouse click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void SubtitleTextBoxMouseClick(object sender, MouseEventArgs e)
        {
            this.TogglePlayPause();
        }

        /// <summary>
        /// Gets the last paragraph.
        /// </summary>
        public IParagraph LastParagraph { get; private set; }

        /// <summary>
        /// The set subtitle text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        public void SetSubtitleText(string text, IParagraph p)
        {
            this.SubtitleText = text;
            this.LastParagraph = p;
        }

        /// <summary>
        /// Gets or sets the subtitle text.
        /// </summary>
        public string SubtitleText
        {
            get
            {
                return this._subtitleText;
            }

            set
            {
                this._subtitleText = value;

                bool alignLeft = this._subtitleText.StartsWith("{\\a1}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\a5}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\a9}", StringComparison.Ordinal) || // sub station alpha
                                 this._subtitleText.StartsWith("{\\an1}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\an4}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\an7}", StringComparison.Ordinal); // advanced sub station alpha

                bool alignRight = this._subtitleText.StartsWith("{\\a3}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\a7}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\a11}", StringComparison.Ordinal) || // sub station alpha
                                  this._subtitleText.StartsWith("{\\an3}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\an6}", StringComparison.Ordinal) || this._subtitleText.StartsWith("{\\an9}", StringComparison.Ordinal); // advanced sub station alpha

                // remove styles for display text (except italic)
                string text = RemoveSubStationAlphaFormatting(this._subtitleText);
                text = HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagBold, HtmlUtil.TagUnderline);

                // display italic
                var sb = new StringBuilder();
                int i = 0;
                bool isItalic = false;
                bool isFontColor = false;
                int italicBegin = 0;
                int fontColorBegin = 0;
                this._subtitleTextBox.Text = string.Empty;
                int letterCount = 0;
                var italicLookups = new System.Collections.Generic.Dictionary<int, int>();
                var fontColorLookups = new System.Collections.Generic.Dictionary<Point, Color>();
                Color fontColor = Color.White;
                while (i < text.Length)
                {
                    if (text.Substring(i).StartsWith("<i>", StringComparison.OrdinalIgnoreCase))
                    {
                        this._subtitleTextBox.AppendText(sb.ToString());
                        sb = new StringBuilder();
                        isItalic = true;
                        italicBegin = letterCount;
                        i += 2;
                    }
                    else if (text.Substring(i).StartsWith("</i>", StringComparison.OrdinalIgnoreCase) && isItalic)
                    {
                        italicLookups.Add(italicBegin, this._subtitleTextBox.Text.Length + sb.ToString().Length - italicBegin);
                        this._subtitleTextBox.AppendText(sb.ToString());
                        sb = new StringBuilder();
                        isItalic = false;
                        i += 3;
                    }
                    else if (text.Substring(i).StartsWith("<font ", StringComparison.OrdinalIgnoreCase))
                    {
                        string s = text.Substring(i);
                        bool fontFound = false;
                        int end = s.IndexOf('>');
                        if (end > 0)
                        {
                            string f = s.Substring(0, end);
                            int colorStart = f.IndexOf(" color=", StringComparison.Ordinal);
                            if (colorStart > 0)
                            {
                                int colorEnd = f.IndexOf('"', colorStart + " color=".Length + 1);
                                if (colorEnd > 0)
                                {
                                    s = f.Substring(colorStart, colorEnd - colorStart);
                                    s = s.Remove(0, " color=".Length);
                                    s = s.Trim('"');
                                    s = s.Trim('\'');
                                    try
                                    {
                                        fontColor = ColorTranslator.FromHtml(s);
                                        fontFound = true;
                                    }
                                    catch
                                    {
                                        fontFound = false;
                                        if (s.Length > 0)
                                        {
                                            try
                                            {
                                                fontColor = ColorTranslator.FromHtml("#" + s);
                                                fontFound = true;
                                            }
                                            catch
                                            {
                                                fontFound = false;
                                            }
                                        }
                                    }
                                }
                            }

                            i += end;
                        }

                        // fontIndices.Push(_subtitleTextBox.Text.Length);
                        if (fontFound)
                        {
                            this._subtitleTextBox.AppendText(sb.ToString());
                            sb = new StringBuilder();
                            isFontColor = true;
                            fontColorBegin = letterCount;
                        }
                    }
                    else if (text.Substring(i).StartsWith("</font>", StringComparison.OrdinalIgnoreCase) && isFontColor)
                    {
                        fontColorLookups.Add(new Point(fontColorBegin, this._subtitleTextBox.Text.Length + sb.ToString().Length - fontColorBegin), fontColor);
                        this._subtitleTextBox.AppendText(sb.ToString());
                        sb = new StringBuilder();
                        isFontColor = false;
                        i += 6;
                    }
                    else if (text[i] == '\n')
                    {
                        // RichTextBox only count NewLine as one character!
                        sb.Append(text[i]);
                    }
                    else
                    {
                        sb.Append(text[i]);
                        letterCount++;
                    }

                    i++;
                }

                this._subtitleTextBox.Text += sb.ToString();
                this._subtitleTextBox.SelectAll();

                if (alignLeft)
                {
                    this._subtitleTextBox.SelectionAlignment = HorizontalAlignment.Left;
                }
                else if (alignRight)
                {
                    this._subtitleTextBox.SelectionAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    this._subtitleTextBox.SelectionAlignment = HorizontalAlignment.Center;
                }

                this._subtitleTextBox.DeselectAll();
                foreach (var entry in italicLookups)
                {
                    Font currentFont = this._subtitleTextBox.SelectionFont;
                    FontStyle newFontStyle = FontStyle.Italic | FontStyle.Bold;
                    if (!Configuration.Settings.General.VideoPlayerPreviewFontBold)
                    {
                        newFontStyle = FontStyle.Italic;
                    }

                    this._subtitleTextBox.SelectionStart = entry.Key;
                    this._subtitleTextBox.SelectionLength = entry.Value;
                    this._subtitleTextBox.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                    this._subtitleTextBox.DeselectAll();
                }

                foreach (var entry in fontColorLookups)
                {
                    this._subtitleTextBox.SelectionStart = entry.Key.X;
                    this._subtitleTextBox.SelectionLength = entry.Key.Y;
                    this._subtitleTextBox.SelectionColor = entry.Value;
                    this._subtitleTextBox.DeselectAll();
                }
            }
        }

        /// <summary>
        /// The panel player mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PanelPlayerMouseDown(object sender, MouseEventArgs e)
        {
            this.TogglePlayPause();
        }

        /// <summary>
        /// The initialize volume.
        /// </summary>
        /// <param name="defaultVolume">
        /// The default volume.
        /// </param>
        public void InitializeVolume(double defaultVolume)
        {
            int maxVolume = this._pictureBoxVolumeBarBackground.Width - 18;
            this._pictureBoxVolumeBar.Width = (int)(maxVolume * defaultVolume / 100.0);
        }

        /// <summary>
        /// The make player panel.
        /// </summary>
        /// <returns>
        /// The <see cref="Control"/>.
        /// </returns>
        public Control MakePlayerPanel()
        {
            this.PanelPlayer = new Panel { BackColor = this._backgroundColor, Left = 0, Top = 0 };
            return this.PanelPlayer;
        }

        /// <summary>
        /// The hide controls.
        /// </summary>
        public void HideControls()
        {
            if (this._panelcontrols.Visible)
            {
                this._panelSubtitle.Height = this._panelSubtitle.Height + this._controlsHeight;
                this._panelcontrols.Visible = false;
            }
        }

        /// <summary>
        /// The show controls.
        /// </summary>
        public void ShowControls()
        {
            if (!this._panelcontrols.Visible)
            {
                this._panelcontrols.Visible = true;
                this._panelSubtitle.Height = this._panelSubtitle.Height - this._controlsHeight;
            }
        }

        /// <summary>
        /// The make controls panel.
        /// </summary>
        /// <returns>
        /// The <see cref="Control"/>.
        /// </returns>
        public Control MakeControlsPanel()
        {
            this._panelcontrols = new Panel { Left = 0, Height = this._controlsHeight };

            this._pictureBoxBackground = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxBar.Image"), Location = new Point(0, 0), Name = "_pictureBoxBackground", Size = new Size(200, 45), SizeMode = PictureBoxSizeMode.StretchImage, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxBackground);

            this._pictureBoxPlay = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxPlay.Image"), Location = new Point(22, 126 - 113), Name = "_pictureBoxPlay", Size = new Size(29, 29), SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._pictureBoxPlay.MouseEnter += this.PictureBoxPlayMouseEnter;
            this._panelcontrols.Controls.Add(this._pictureBoxPlay);

            this._pictureBoxPlayDown = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxPlayDown.Image"), Location = new Point(22, 127 - 113), Name = "_pictureBoxPlayDown", Size = new Size(29, 29), SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxPlayDown);

            this._pictureBoxPlayOver = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxPlayOver.Image"), Location = new Point(23, 126 - 113), Name = "_pictureBoxPlayOver", Size = new Size(29, 29), SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._pictureBoxPlayOver.MouseLeave += this.PictureBoxPlayOverMouseLeave;
            this._pictureBoxPlayOver.MouseDown += this.PictureBoxPlayOverMouseDown;
            this._pictureBoxPlayOver.MouseUp += this.PictureBoxPlayOverMouseUp;
            this._panelcontrols.Controls.Add(this._pictureBoxPlayOver);

            this._pictureBoxPause.Image = (Image)this._resources.GetObject("pictureBoxPause.Image");
            this._pictureBoxPause.Location = new Point(23, 126 - 113);
            this._pictureBoxPause.Name = "_pictureBoxPause";
            this._pictureBoxPause.Size = new Size(29, 29);
            this._pictureBoxPause.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxPause.TabStop = false;
            this._pictureBoxPause.MouseEnter += this.PictureBoxPauseMouseEnter;
            this._panelcontrols.Controls.Add(this._pictureBoxPause);

            this._pictureBoxPauseDown.Image = (Image)this._resources.GetObject("pictureBoxPauseDown.Image");
            this._pictureBoxPauseDown.Location = new Point(22, 127 - 113);
            this._pictureBoxPauseDown.Name = "_pictureBoxPauseDown";
            this._pictureBoxPauseDown.Size = new Size(29, 29);
            this._pictureBoxPauseDown.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxPauseDown.TabStop = false;
            this._panelcontrols.Controls.Add(this._pictureBoxPauseDown);

            this._pictureBoxPauseOver.Image = (Image)this._resources.GetObject("pictureBoxPauseOver.Image");
            this._pictureBoxPauseOver.Location = new Point(22, 127 - 113);
            this._pictureBoxPauseOver.Name = "_pictureBoxPauseOver";
            this._pictureBoxPauseOver.Size = new Size(29, 29);
            this._pictureBoxPauseOver.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxPauseOver.TabStop = false;
            this._pictureBoxPauseOver.MouseLeave += this.PictureBoxPauseOverMouseLeave;
            this._pictureBoxPauseOver.MouseDown += this.PictureBoxPauseOverMouseDown;
            this._pictureBoxPauseOver.MouseUp += this.PictureBoxPauseOverMouseUp;
            this._panelcontrols.Controls.Add(this._pictureBoxPauseOver);

            this._pictureBoxStop.Image = (Image)this._resources.GetObject("pictureBoxStop.Image");
            this._pictureBoxStop.Location = new Point(52, 130 - 113);
            this._pictureBoxStop.Name = "_pictureBoxStop";
            this._pictureBoxStop.Size = new Size(20, 20);
            this._pictureBoxStop.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxStop.TabStop = false;
            this._pictureBoxStop.MouseEnter += this.PictureBoxStopMouseEnter;
            this._panelcontrols.Controls.Add(this._pictureBoxStop);

            this._pictureBoxStopDown.Image = (Image)this._resources.GetObject("pictureBoxStopDown.Image");
            this._pictureBoxStopDown.Location = new Point(52, 130 - 113);
            this._pictureBoxStopDown.Name = "_pictureBoxStopDown";
            this._pictureBoxStopDown.Size = new Size(20, 20);
            this._pictureBoxStopDown.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxStopDown.TabStop = false;
            this._panelcontrols.Controls.Add(this._pictureBoxStopDown);

            this._pictureBoxStopOver.Image = (Image)this._resources.GetObject("pictureBoxStopOver.Image");
            this._pictureBoxStopOver.Location = new Point(52, 130 - 113);
            this._pictureBoxStopOver.Name = "_pictureBoxStopOver";
            this._pictureBoxStopOver.Size = new Size(20, 20);
            this._pictureBoxStopOver.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxStopOver.TabStop = false;
            this._pictureBoxStopOver.MouseLeave += this.PictureBoxStopOverMouseLeave;
            this._pictureBoxStopOver.MouseDown += this.PictureBoxStopOverMouseDown;
            this._pictureBoxStopOver.MouseUp += this.PictureBoxStopOverMouseUp;
            this._panelcontrols.Controls.Add(this._pictureBoxStopOver);

            this._pictureBoxFullscreen.Image = (Image)this._resources.GetObject("pictureBoxFS.Image");
            this._pictureBoxFullscreen.Location = new Point(95, 130 - 113);
            this._pictureBoxFullscreen.Name = "_pictureBoxFullscreen";
            this._pictureBoxFullscreen.Size = new Size(20, 20);
            this._pictureBoxFullscreen.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxFullscreen.TabStop = false;
            this._pictureBoxFullscreen.MouseEnter += this.PictureBoxFullscreenMouseEnter;
            this._panelcontrols.Controls.Add(this._pictureBoxFullscreen);

            this._pictureBoxFullscreenDown.Image = (Image)this._resources.GetObject("pictureBoxFSDown.Image");
            this._pictureBoxFullscreenDown.Location = new Point(95, 130 - 113);
            this._pictureBoxFullscreenDown.Name = "_pictureBoxFullscreenDown";
            this._pictureBoxFullscreenDown.Size = new Size(20, 20);
            this._pictureBoxFullscreenDown.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxFullscreenDown.TabStop = false;
            this._panelcontrols.Controls.Add(this._pictureBoxFullscreenDown);

            this._pictureBoxFullscreenOver.Image = (Image)this._resources.GetObject("pictureBoxFSOver.Image");
            this._pictureBoxFullscreenOver.Location = new Point(95, 130 - 113);
            this._pictureBoxFullscreenOver.Name = "_pictureBoxFullscreenOver";
            this._pictureBoxFullscreenOver.Size = new Size(20, 20);
            this._pictureBoxFullscreenOver.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxFullscreenOver.TabStop = false;
            this._pictureBoxFullscreenOver.MouseLeave += this.PictureBoxFullscreenOverMouseLeave;
            this._pictureBoxFullscreenOver.MouseDown += this.PictureBoxFullscreenOverMouseDown;
            this._pictureBoxFullscreenOver.MouseUp += this.PictureBoxFullscreenOverMouseUp;
            this._panelcontrols.Controls.Add(this._pictureBoxFullscreenOver);

            this._pictureBoxProgressbarBackground.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this._pictureBoxProgressbarBackground.BackColor = Color.Transparent;
            this._pictureBoxProgressbarBackground.Image = (Image)this._resources.GetObject("pictureBoxProgressbarBackground.Image");
            this._pictureBoxProgressbarBackground.Location = new Point(43, 114 - 113);
            this._pictureBoxProgressbarBackground.Margin = new Padding(0);
            this._pictureBoxProgressbarBackground.Name = "_pictureBoxProgressbarBackground";
            this._pictureBoxProgressbarBackground.Size = new Size(531, 12);
            this._pictureBoxProgressbarBackground.SizeMode = PictureBoxSizeMode.StretchImage;
            this._pictureBoxProgressbarBackground.TabStop = false;
            this._pictureBoxProgressbarBackground.MouseDown += this.PictureBoxProgressbarBackgroundMouseDown;
            this._panelcontrols.Controls.Add(this._pictureBoxProgressbarBackground);

            this._pictureBoxProgressBar.Image = (Image)this._resources.GetObject("pictureBoxProgressBar.Image");
            this._pictureBoxProgressBar.Location = new Point(47, 118 - 113);
            this._pictureBoxProgressBar.Name = "_pictureBoxProgressBar";
            this._pictureBoxProgressBar.Size = new Size(318, 4);
            this._pictureBoxProgressBar.SizeMode = PictureBoxSizeMode.StretchImage;
            this._pictureBoxProgressBar.TabStop = false;
            this._pictureBoxProgressBar.MouseDown += this.PictureBoxProgressBarMouseDown;
            this._panelcontrols.Controls.Add(this._pictureBoxProgressBar);
            this._pictureBoxProgressBar.BringToFront();

            this._pictureBoxMute.Image = (Image)this._resources.GetObject("pictureBoxMute.Image");
            this._pictureBoxMute.Location = new Point(75, 131 - 113);
            this._pictureBoxMute.Name = "_pictureBoxMute";
            this._pictureBoxMute.Size = new Size(19, 19);
            this._pictureBoxMute.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxMute.TabStop = false;
            this._pictureBoxMute.MouseEnter += this.PictureBoxMuteMouseEnter;
            this._panelcontrols.Controls.Add(this._pictureBoxMute);

            this._pictureBoxMuteDown.Image = (Image)this._resources.GetObject("pictureBoxMuteDown.Image");
            this._pictureBoxMuteDown.Location = new Point(75, 131 - 113);
            this._pictureBoxMuteDown.Name = "_pictureBoxMuteDown";
            this._pictureBoxMuteDown.Size = new Size(19, 19);
            this._pictureBoxMuteDown.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxMuteDown.TabStop = false;
            this._pictureBoxMuteDown.Click += this.PictureBoxMuteDownClick;
            this._panelcontrols.Controls.Add(this._pictureBoxMuteDown);

            this._pictureBoxMuteOver.Image = (Image)this._resources.GetObject("pictureBoxMuteOver.Image");
            this._pictureBoxMuteOver.Location = new Point(75, 131 - 113);
            this._pictureBoxMuteOver.Name = "_pictureBoxMuteOver";
            this._pictureBoxMuteOver.Size = new Size(19, 19);
            this._pictureBoxMuteOver.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxMuteOver.TabStop = false;
            this._pictureBoxMuteOver.MouseLeave += this.PictureBoxMuteOverMouseLeave;
            this._pictureBoxMuteOver.MouseDown += this.PictureBoxMuteOverMouseDown;
            this._pictureBoxMuteOver.MouseUp += this.PictureBoxMuteOverMouseUp;
            this._panelcontrols.Controls.Add(this._pictureBoxMuteOver);

            this._pictureBoxVolumeBarBackground.Image = (Image)this._resources.GetObject("pictureBoxVolumeBarBackground.Image");
            this._pictureBoxVolumeBarBackground.Location = new Point(111, 135 - 113);
            this._pictureBoxVolumeBarBackground.Name = "_pictureBoxVolumeBarBackground";
            this._pictureBoxVolumeBarBackground.Size = new Size(82, 13);
            this._pictureBoxVolumeBarBackground.SizeMode = PictureBoxSizeMode.AutoSize;
            this._pictureBoxVolumeBarBackground.TabStop = false;
            this._pictureBoxVolumeBarBackground.MouseDown += this.PictureBoxVolumeBarBackgroundMouseDown;
            this._panelcontrols.Controls.Add(this._pictureBoxVolumeBarBackground);

            this._pictureBoxVolumeBar.Image = (Image)this._resources.GetObject("pictureBoxVolumeBar.Image");
            this._pictureBoxVolumeBar.Location = new Point(120, 139 - 113);
            this._pictureBoxVolumeBar.Name = "_pictureBoxVolumeBar";
            this._pictureBoxVolumeBar.Size = new Size(48, 4);
            this._pictureBoxVolumeBar.SizeMode = PictureBoxSizeMode.StretchImage;
            this._pictureBoxVolumeBar.TabStop = false;
            this._pictureBoxVolumeBar.MouseDown += this.PictureBoxVolumeBarMouseDown;
            this._panelcontrols.Controls.Add(this._pictureBoxVolumeBar);
            this._pictureBoxVolumeBar.BringToFront();

            this._pictureBoxReverse = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxReverse.Image"), Location = new Point(28, 3), Name = "_pictureBoxReverse", Size = new Size(16, 8), SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxReverse);
            this._pictureBoxReverse.MouseEnter += this.PictureBoxReverseMouseEnter;

            this._pictureBoxReverseOver = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxReverseMouseOver.Image"), Location = this._pictureBoxReverse.Location, Name = "_pictureBoxReverseOver", Size = this._pictureBoxReverse.Size, SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxReverseOver);
            this._pictureBoxReverseOver.MouseLeave += this.PictureBoxReverseOverMouseLeave;
            this._pictureBoxReverseOver.MouseDown += this.PictureBoxReverseOverMouseDown;
            this._pictureBoxReverseOver.MouseUp += this.PictureBoxReverseOverMouseUp;

            this._pictureBoxReverseDown = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxReverseMouseDown.Image"), Location = this._pictureBoxReverse.Location, Name = "_pictureBoxReverseOver", Size = this._pictureBoxReverse.Size, SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxReverseDown);

            this._pictureBoxFastForward = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxFastForward.Image"), Location = new Point(571, 1), Name = "_pictureBoxFastForward", Size = new Size(17, 13), SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxFastForward);
            this._pictureBoxFastForward.MouseEnter += this.PictureBoxFastForwardMouseEnter;

            this._pictureBoxFastForwardOver = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxFastForwardMouseOver.Image"), Location = this._pictureBoxFastForward.Location, Name = "_pictureBoxFastForwardOver", Size = this._pictureBoxFastForward.Size, SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxFastForwardOver);
            this._pictureBoxFastForwardOver.MouseLeave += this.PictureBoxFastForwardOverMouseLeave;
            this._pictureBoxFastForwardOver.MouseDown += this.PictureBoxFastForwardOverMouseDown;
            this._pictureBoxFastForwardOver.MouseUp += this.PictureBoxFastForwardOverMouseUp;

            this._pictureBoxFastForwardDown = new PictureBox { Image = (Image)this._resources.GetObject("pictureBoxFastForwardMouseDown.Image"), Location = this._pictureBoxFastForward.Location, Name = "_pictureBoxFastForwardDown", Size = this._pictureBoxFastForward.Size, SizeMode = PictureBoxSizeMode.AutoSize, TabStop = false };
            this._panelcontrols.Controls.Add(this._pictureBoxFastForwardDown);

            this._labelTimeCode.Location = new Point(280, 28);
            this._labelTimeCode.ForeColor = Color.Gray;
            this._labelTimeCode.Font = new Font(this._labelTimeCode.Font.FontFamily, 7);
            this._labelTimeCode.AutoSize = true;
            this._panelcontrols.Controls.Add(this._labelTimeCode);

            this._labelVideoPlayerName.Location = new Point(280, 17);
            this._labelVideoPlayerName.ForeColor = Color.Gray;
            this._labelVideoPlayerName.BackColor = Color.FromArgb(67, 75, 93);
            this._labelVideoPlayerName.AutoSize = true;
            this._labelVideoPlayerName.Font = new Font(this._labelTimeCode.Font.FontFamily, 5);

            this._panelcontrols.Controls.Add(this._labelVideoPlayerName);

            this._pictureBoxBackground.SendToBack();
            this._pictureBoxFastForward.BringToFront();
            this._pictureBoxFastForwardDown.BringToFront();
            this._pictureBoxFastForwardOver.BringToFront();
            this._pictureBoxPlay.BringToFront();

            this._panelcontrols.BackColor = this._backgroundColor;
            this._pictureBoxPlay.BringToFront();
            this._pictureBoxPlayDown.BringToFront();
            this._pictureBoxPlayOver.BringToFront();
            this._labelTimeCode.BringToFront();
            return this._panelcontrols;
        }

        /// <summary>
        /// The video player container resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void VideoPlayerContainerResize(object sender, EventArgs e)
        {
            this._controlsHeight = this._pictureBoxBackground.Height;
            this.PanelPlayer.Height = this.Height - (this._controlsHeight + SubtitlesHeight);
            this.PanelPlayer.Width = this.Width;
            if (this._videoPlayer != null)
            {
                this._videoPlayer.Resize(this.PanelPlayer.Width, this.PanelPlayer.Height);
            }

            this._panelSubtitle.Top = this.Height - (this._controlsHeight + SubtitlesHeight);
            this._panelSubtitle.Width = this.Width;

            this._panelcontrols.Top = this.Height - this._controlsHeight + 2;
            this._panelcontrols.Width = this.Width;
            this._pictureBoxBackground.Width = this.Width;
            this._pictureBoxProgressbarBackground.Width = this.Width - (this._pictureBoxProgressbarBackground.Left * 2);
            this._pictureBoxFastForward.Left = this.Width - 48;
            this._pictureBoxFastForwardDown.Left = this._pictureBoxFastForward.Left;
            this._pictureBoxFastForwardOver.Left = this._pictureBoxFastForward.Left;

            this._labelTimeCode.Left = this.Width - 170;
            this._labelVideoPlayerName.Left = this.Width - this._labelVideoPlayerName.Width - 3;
        }

        /// <summary>
        /// The remove sub station alpha formatting.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveSubStationAlphaFormatting(string s)
        {
            int indexOfBegin = s.IndexOf('{');
            while (indexOfBegin >= 0 && s.IndexOf('}') > indexOfBegin)
            {
                int indexOfEnd = s.IndexOf('}');
                s = s.Remove(indexOfBegin, (indexOfEnd - indexOfBegin) + 1);
                indexOfBegin = s.IndexOf('{');
            }

            return s;
        }

        #region PlayPauseButtons

        /// <summary>
        /// The refresh play pause buttons.
        /// </summary>
        public void RefreshPlayPauseButtons()
        {
            if (this.VideoPlayer != null)
            {
                if (this.VideoPlayer.IsPlaying)
                {
                    if (!this._pictureBoxPause.Visible && !this._pictureBoxPauseDown.Visible && !this._pictureBoxPauseOver.Visible)
                    {
                        this.HideAllPauseImages();
                        this.HideAllPlayImages();
                        this._pictureBoxPause.Visible = true;
                        this._pictureBoxPause.BringToFront();
                    }
                }
                else
                {
                    if (!this._pictureBoxPlay.Visible && !this._pictureBoxPlayOver.Visible && !this._pictureBoxPlayDown.Visible)
                    {
                        this.HideAllPauseImages();
                        this.HideAllPlayImages();
                        this._pictureBoxPlay.Visible = true;
                        this._pictureBoxPlay.BringToFront();
                    }
                }
            }
        }

        /// <summary>
        /// The hide all play images.
        /// </summary>
        public void HideAllPlayImages()
        {
            this._pictureBoxPlayOver.Visible = false;
            this._pictureBoxPlayDown.Visible = false;
            this._pictureBoxPlay.Visible = false;
        }

        /// <summary>
        /// The picture box play mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPlayMouseEnter(object sender, EventArgs e)
        {
            if (this._pictureBoxPlay.Visible)
            {
                this.HideAllPlayImages();
                this._pictureBoxPlayOver.Visible = true;
                this._pictureBoxPlayOver.BringToFront();
            }
        }

        /// <summary>
        /// The picture box play over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPlayOverMouseLeave(object sender, EventArgs e)
        {
            if (this._pictureBoxPlayOver.Visible)
            {
                this.HideAllPlayImages();
                this._pictureBoxPlay.Visible = true;
                this._pictureBoxPlay.BringToFront();
            }
        }

        /// <summary>
        /// The picture box play over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPlayOverMouseDown(object sender, MouseEventArgs e)
        {
            this.HideAllPlayImages();
            this._pictureBoxPlayDown.Visible = true;
            this._pictureBoxPlayDown.BringToFront();
            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The picture box play over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPlayOverMouseUp(object sender, MouseEventArgs e)
        {
            this.HideAllPlayImages();
            this._pictureBoxPause.Visible = true;
            this._pictureBoxPause.BringToFront();
            this.Play();
        }

        /// <summary>
        /// The hide all pause images.
        /// </summary>
        public void HideAllPauseImages()
        {
            this._pictureBoxPauseOver.Visible = false;
            this._pictureBoxPauseDown.Visible = false;
            this._pictureBoxPause.Visible = false;
        }

        /// <summary>
        /// The picture box pause mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPauseMouseEnter(object sender, EventArgs e)
        {
            if (this._pictureBoxPause.Visible)
            {
                this.HideAllPauseImages();
                this._pictureBoxPauseOver.Visible = true;
            }
        }

        /// <summary>
        /// The picture box pause over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPauseOverMouseLeave(object sender, EventArgs e)
        {
            if (this._pictureBoxPauseOver.Visible)
            {
                this.HideAllPauseImages();
                this._pictureBoxPause.Visible = true;
                this._pictureBoxPause.BringToFront();
            }
        }

        /// <summary>
        /// The picture box pause over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPauseOverMouseDown(object sender, MouseEventArgs e)
        {
            if (this._pictureBoxPauseOver.Visible)
            {
                this.HideAllPauseImages();
                this._pictureBoxPauseDown.Visible = true;
            }

            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The picture box pause over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxPauseOverMouseUp(object sender, MouseEventArgs e)
        {
            this.HideAllPauseImages();
            this._pictureBoxPlay.Visible = true;
            this.Pause();
        }

        #endregion PlayPauseButtons

        #region StopButtons

        /// <summary>
        /// The hide all stop images.
        /// </summary>
        public void HideAllStopImages()
        {
            this._pictureBoxStopOver.Visible = false;
            this._pictureBoxStopDown.Visible = false;
            this._pictureBoxStop.Visible = false;
        }

        /// <summary>
        /// The picture box stop mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxStopMouseEnter(object sender, EventArgs e)
        {
            this.HideAllStopImages();
            this._pictureBoxStopOver.Visible = true;
        }

        /// <summary>
        /// The picture box stop over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxStopOverMouseLeave(object sender, EventArgs e)
        {
            if (this._pictureBoxStopOver.Visible)
            {
                this.HideAllStopImages();
                this._pictureBoxStop.Visible = true;
            }
        }

        /// <summary>
        /// The picture box stop over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxStopOverMouseDown(object sender, MouseEventArgs e)
        {
            if (this._pictureBoxStopOver.Visible)
            {
                this.HideAllStopImages();
                this._pictureBoxStopDown.Visible = true;
            }

            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The picture box stop over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxStopOverMouseUp(object sender, MouseEventArgs e)
        {
            this.HideAllStopImages();
            this._pictureBoxStop.Visible = true;
            this.Stop();
        }

        #endregion StopButtons

        #region FullscreenButtons

        /// <summary>
        /// The hide all fullscreen images.
        /// </summary>
        public void HideAllFullscreenImages()
        {
            this._pictureBoxFullscreenOver.Visible = false;
            this._pictureBoxFullscreenDown.Visible = false;
            this._pictureBoxFullscreen.Visible = false;
        }

        /// <summary>
        /// The show full screen controls.
        /// </summary>
        public void ShowFullScreenControls()
        {
            this._pictureBoxFullscreen.Image = (Image)this._resources.GetObject("pictureBoxNoFS.Image");
            this._pictureBoxFullscreenDown.Image = (Image)this._resources.GetObject("pictureBoxNoFSDown.Image");
            this._pictureBoxFullscreenOver.Image = (Image)this._resources.GetObject("pictureBoxNoFSOver.Image");
        }

        /// <summary>
        /// The show non full screen controls.
        /// </summary>
        public void ShowNonFullScreenControls()
        {
            this._pictureBoxFullscreen.Image = (Image)this._resources.GetObject("pictureBoxFS.Image");
            this._pictureBoxFullscreenDown.Image = (Image)this._resources.GetObject("pictureBoxFSDown.Image");
            this._pictureBoxFullscreenOver.Image = (Image)this._resources.GetObject("pictureBoxFSOver.Image");
        }

        /// <summary>
        /// The picture box fullscreen mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFullscreenMouseEnter(object sender, EventArgs e)
        {
            this.HideAllFullscreenImages();
            this._pictureBoxFullscreenOver.Visible = true;
        }

        /// <summary>
        /// The picture box fullscreen over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFullscreenOverMouseLeave(object sender, EventArgs e)
        {
            if (this._pictureBoxFullscreenOver.Visible)
            {
                this.HideAllFullscreenImages();
                this._pictureBoxFullscreen.Visible = true;
            }
        }

        /// <summary>
        /// The picture box fullscreen over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFullscreenOverMouseDown(object sender, MouseEventArgs e)
        {
            this.HideAllFullscreenImages();
            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The picture box fullscreen over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFullscreenOverMouseUp(object sender, MouseEventArgs e)
        {
            this.HideAllFullscreenImages();
            this._pictureBoxFullscreen.Visible = true;
        }

        #endregion FullscreenButtons

        #region Mute buttons

        /// <summary>
        /// The hide all mute images.
        /// </summary>
        public void HideAllMuteImages()
        {
            this._pictureBoxMuteOver.Visible = false;
            this._pictureBoxMuteDown.Visible = false;
            this._pictureBoxMute.Visible = false;
        }

        /// <summary>
        /// The picture box mute mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxMuteMouseEnter(object sender, EventArgs e)
        {
            this.HideAllMuteImages();
            if (this.Mute)
            {
                this._pictureBoxMuteDown.Visible = true;
            }
            else
            {
                this._pictureBoxMuteOver.Visible = true;
            }
        }

        /// <summary>
        /// The picture box mute over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxMuteOverMouseLeave(object sender, EventArgs e)
        {
            if (this._pictureBoxMuteOver.Visible)
            {
                this.HideAllMuteImages();
                this._pictureBoxMute.Visible = true;
            }
        }

        /// <summary>
        /// The picture box mute over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxMuteOverMouseDown(object sender, MouseEventArgs e)
        {
            if (this._pictureBoxMuteOver.Visible)
            {
                this.HideAllMuteImages();
                this._pictureBoxMuteDown.Visible = true;
            }

            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The picture box mute over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxMuteOverMouseUp(object sender, MouseEventArgs e)
        {
            this.HideAllMuteImages();
            this.Mute = true;
            this._pictureBoxMuteDown.Visible = true;
        }

        /// <summary>
        /// The picture box mute down click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxMuteDownClick(object sender, EventArgs e)
        {
            this.Mute = false;
            this.HideAllMuteImages();
            this._pictureBoxMute.Visible = true;
            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        #endregion Mute buttons

        #region Reverse buttons

        /// <summary>
        /// The hide all reverse images.
        /// </summary>
        public void HideAllReverseImages()
        {
            this._pictureBoxReverseOver.Visible = false;
            this._pictureBoxReverseDown.Visible = false;
            this._pictureBoxReverse.Visible = false;
        }

        /// <summary>
        /// The picture box reverse mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxReverseMouseEnter(object sender, EventArgs e)
        {
            this.HideAllReverseImages();
            this._pictureBoxReverseOver.Visible = true;
        }

        /// <summary>
        /// The picture box reverse over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxReverseOverMouseLeave(object sender, EventArgs e)
        {
            this.HideAllReverseImages();
            this._pictureBoxReverse.Visible = true;
        }

        /// <summary>
        /// The picture box reverse over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxReverseOverMouseDown(object sender, MouseEventArgs e)
        {
            this.HideAllReverseImages();
            this._pictureBoxReverseDown.Visible = true;
            if (this.VideoPlayer != null)
            {
                var newPosition = this.CurrentPosition - 3.0;
                if (newPosition < 0)
                {
                    newPosition = 0;
                }

                this.CurrentPosition = newPosition;
            }
        }

        /// <summary>
        /// The picture box reverse over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxReverseOverMouseUp(object sender, MouseEventArgs e)
        {
            this.HideAllReverseImages();
            this._pictureBoxReverse.Visible = true;
        }

        #endregion Reverse buttons

        #region Fast forward buttons

        /// <summary>
        /// The hide all fast forward images.
        /// </summary>
        public void HideAllFastForwardImages()
        {
            this._pictureBoxFastForwardOver.Visible = false;
            this._pictureBoxFastForwardDown.Visible = false;
            this._pictureBoxFastForward.Visible = false;
        }

        /// <summary>
        /// The picture box fast forward mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFastForwardMouseEnter(object sender, EventArgs e)
        {
            this.HideAllFastForwardImages();
            this._pictureBoxFastForwardOver.Visible = true;
        }

        /// <summary>
        /// The picture box fast forward over mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFastForwardOverMouseLeave(object sender, EventArgs e)
        {
            this.HideAllFastForwardImages();
            this._pictureBoxFastForward.Visible = true;
        }

        /// <summary>
        /// The picture box fast forward over mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFastForwardOverMouseDown(object sender, MouseEventArgs e)
        {
            this.HideAllFastForwardImages();
            this._pictureBoxFastForwardDown.Visible = true;

            if (this.VideoPlayer != null)
            {
                var newPosition = this.CurrentPosition + 3.0;
                if (newPosition < 0)
                {
                    newPosition = 0;
                }

                this.CurrentPosition = newPosition;
            }
        }

        /// <summary>
        /// The picture box fast forward over mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxFastForwardOverMouseUp(object sender, MouseEventArgs e)
        {
            this.HideAllFastForwardImages();
            this._pictureBoxFastForward.Visible = true;
        }

        #endregion Fast forward buttons

        #region Progress bars

        /// <summary>
        /// The set progress bar position.
        /// </summary>
        /// <param name="mouseX">
        /// The mouse x.
        /// </param>
        public void SetProgressBarPosition(int mouseX)
        {
            int max = this._pictureBoxProgressbarBackground.Width - 9;
            if (mouseX > max)
            {
                mouseX = max;
            }

            double percent = (mouseX * 100.0) / max;
            this._pictureBoxProgressBar.Width = (int)(max * percent / 100.0);

            double pos = percent * (this.Duration - this.Offset) / 100.0;
            this.CurrentPosition = pos + this.Offset;
        }

        /// <summary>
        /// The picture box progressbar background mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxProgressbarBackgroundMouseDown(object sender, MouseEventArgs e)
        {
            this.SetProgressBarPosition(e.X - 4);
            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The picture box progress bar mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxProgressBarMouseDown(object sender, MouseEventArgs e)
        {
            this.SetProgressBarPosition(e.X + 2);
            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The refresh progress bar.
        /// </summary>
        public void RefreshProgressBar()
        {
            if (this.VideoPlayer == null)
            {
                this._pictureBoxProgressBar.Width = 0;
            }
            else
            {
                int max = this._pictureBoxProgressbarBackground.Width - 9;
                double percent = (this.VideoPlayer.CurrentPosition * 100.0) / this.VideoPlayer.Duration;
                this._pictureBoxProgressBar.Width = (int)(max * percent / 100.0);

                if (Convert.ToInt64(this.Duration) == 0)
                {
                    return;
                }

                var pos = this.CurrentPosition;
                if (pos > 1000000)
                {
                    pos = 0;
                }

                var span = TimeCode.FromSeconds(pos);
                var dur = TimeCode.FromSeconds(this.Duration);

                if (Configuration.Settings != null && Configuration.Settings.General.UseTimeFormatHHMMSSFF)
                {
                    this._labelTimeCode.Text = string.Format("{0} / {1}", span.ToHHMMSSFF(), dur.ToHHMMSSFF());
                }
                else
                {
                    this._labelTimeCode.Text = string.Format("{0:00}:{1:00}:{2:00},{3:000} / {4:00}:{5:00}:{6:00},{7:000}", span.Hours, span.Minutes, span.Seconds, span.Milliseconds, dur.Hours, dur.Minutes, dur.Seconds, dur.Milliseconds);
                }

                this.RefreshPlayPauseButtons();
            }
        }

        /// <summary>
        /// The set volume bar position.
        /// </summary>
        /// <param name="mouseX">
        /// The mouse x.
        /// </param>
        public void SetVolumeBarPosition(int mouseX)
        {
            int max = this._pictureBoxVolumeBarBackground.Width - 18;
            if (mouseX > max)
            {
                mouseX = max;
            }

            double percent = (mouseX * 100.0) / max;
            this._pictureBoxVolumeBar.Width = (int)(max * percent / 100.0);
            if (this._videoPlayer != null)
            {
                this._videoPlayer.Volume = (int)percent;
            }

            Configuration.Settings.General.VideoPlayerDefaultVolume = (int)percent;
        }

        /// <summary>
        /// The picture box volume bar background mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxVolumeBarBackgroundMouseDown(object sender, MouseEventArgs e)
        {
            this.SetVolumeBarPosition(e.X - 6);
            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The picture box volume bar mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void PictureBoxVolumeBarMouseDown(object sender, MouseEventArgs e)
        {
            this.SetVolumeBarPosition(e.X + 2);
            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(sender, e);
            }
        }

        /// <summary>
        /// The refresh volume bar.
        /// </summary>
        public void RefreshVolumeBar()
        {
            if (this.VideoPlayer == null)
            {
                this._pictureBoxVolumeBar.Width = 0;
            }
            else
            {
                int max = this._pictureBoxVolumeBarBackground.Width - 18;
                this._pictureBoxVolumeBar.Width = (int)(max * this.Volume / 100.0);
            }
        }

        #endregion Progress bars

        #region VideoPlayer functions

        /// <summary>
        /// The play.
        /// </summary>
        public void Play()
        {
            if (this.VideoPlayer != null)
            {
                this.VideoPlayer.Play();
                this.HideAllPlayImages();
                this._pictureBoxPause.Visible = true;
                this._pictureBoxPause.BringToFront();
                this.RefreshProgressBar();
            }

            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(null, null);
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            if (this.VideoPlayer != null)
            {
                this.VideoPlayer.Pause();
                this.CurrentPosition = this.Offset;
                this.HideAllPauseImages();
                this._pictureBoxPlay.Visible = true;
                this.RefreshProgressBar();
            }

            if (this.OnButtonClicked != null)
            {
                this.OnButtonClicked.Invoke(null, null);
            }
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            if (this.VideoPlayer != null)
            {
                this.VideoPlayer.Pause();
                this.HideAllPauseImages();
                this._pictureBoxPlay.Visible = true;
                this.RefreshProgressBar();
            }
        }

        /// <summary>
        /// The toggle play pause.
        /// </summary>
        public void TogglePlayPause()
        {
            if (this.VideoPlayer != null)
            {
                if (this.VideoPlayer.IsPaused)
                {
                    this.Play();
                }
                else
                {
                    this.Pause();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether is paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                if (this.VideoPlayer != null)
                {
                    return this.VideoPlayer.IsPaused;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public double Volume
        {
            get
            {
                if (this.VideoPlayer != null)
                {
                    return this.VideoPlayer.Volume;
                }

                return 0;
            }

            set
            {
                if (this.VideoPlayer != null)
                {
                    if (value > 0)
                    {
                        this._muteOldVolume = null;
                    }

                    if (value > 100)
                    {
                        this.VideoPlayer.Volume = 100;
                    }
                    else if (value < 0)
                    {
                        this.VideoPlayer.Volume = 0;
                    }
                    else
                    {
                        this.VideoPlayer.Volume = (int)value;
                    }

                    this.RefreshVolumeBar();
                }
            }
        }

        /// <summary>
        /// Video offset in seconds
        /// </summary>
        public double Offset { get; set; }

        /// <summary>
        /// Current position in seconds
        /// </summary>
        public double CurrentPosition
        {
            get
            {
                if (this.VideoPlayer != null)
                {
                    return this.VideoPlayer.CurrentPosition + this.Offset;
                }

                return 0;
            }

            set
            {
                if (this.VideoPlayer != null)
                {
                    this.VideoPlayer.CurrentPosition = value - this.Offset;
                }
            }
        }

        /// <summary>
        /// Total duration in seconds
        /// </summary>
        public double Duration
        {
            get
            {
                if (this.VideoPlayer != null)
                {
                    return this.VideoPlayer.Duration + this.Offset;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mute.
        /// </summary>
        public bool Mute
        {
            get
            {
                if (this.VideoPlayer != null)
                {
                    return this._isMuted;
                }

                return false;
            }

            set
            {
                if (this.VideoPlayer != null)
                {
                    if (value == false && this._muteOldVolume != null)
                    {
                        this.Volume = this._muteOldVolume.Value;
                    }
                    else if (value)
                    {
                        this._muteOldVolume = this.Volume;
                        this.Volume = 0;
                    }

                    this._isMuted = value;
                }
            }
        }

        #endregion VideoPlayer functions
    }
}