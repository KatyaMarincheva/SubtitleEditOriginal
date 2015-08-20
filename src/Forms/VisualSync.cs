// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualSync.cs" company="">
//   
// </copyright>
// <summary>
//   The visual sync.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Controls;
    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.VideoPlayers;

    /// <summary>
    /// The visual sync.
    /// </summary>
    public sealed partial class VisualSync : PositionAndSizeForm
    {
        /// <summary>
        /// The _language.
        /// </summary>
        private readonly LanguageStructure.VisualSync _language;

        /// <summary>
        /// The _language general.
        /// </summary>
        private readonly LanguageStructure.General _languageGeneral;

        /// <summary>
        /// The _timer hide sync label.
        /// </summary>
        private readonly Timer _timerHideSyncLabel = new Timer();

        /// <summary>
        /// The _end go back position.
        /// </summary>
        private double _endGoBackPosition;

        /// <summary>
        /// The _end stop position.
        /// </summary>
        private double _endStopPosition = -1.0;

        /// <summary>
        /// The _frame rate changed.
        /// </summary>
        private bool _frameRateChanged;

        /// <summary>
        /// The _is start scene active.
        /// </summary>
        private bool _isStartSceneActive;

        /// <summary>
        /// The _old frame rate.
        /// </summary>
        private double _oldFrameRate;

        /// <summary>
        /// The _original subtitle.
        /// </summary>
        private Subtitle _originalSubtitle;

        /// <summary>
        /// The _paragraphs.
        /// </summary>
        private List<Paragraph> _paragraphs;

        /// <summary>
        /// The _start go back position.
        /// </summary>
        private double _startGoBackPosition;

        /// <summary>
        /// The _start stop position.
        /// </summary>
        private double _startStopPosition = -1.0;

        /// <summary>
        /// The _subtitle file name.
        /// </summary>
        private string _subtitleFileName;

        /// <summary>
        /// The _video info.
        /// </summary>
        private VideoInfo _videoInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualSync"/> class.
        /// </summary>
        public VisualSync()
        {
            this.InitializeComponent();

            this.openFileDialog1.InitialDirectory = string.Empty;

            this.MediaPlayerStart.InitializeVolume(Configuration.Settings.General.VideoPlayerDefaultVolume);
            this.MediaPlayerEnd.InitializeVolume(Configuration.Settings.General.VideoPlayerDefaultVolume);

            this.labelSyncDone.Text = string.Empty;
            this._language = Configuration.Settings.Language.VisualSync;
            this._languageGeneral = Configuration.Settings.Language.General;
            this.Text = this._language.Title;
            this.buttonOpenMovie.Text = this._languageGeneral.OpenVideoFile;
            this.groupBoxMovieInfo.Text = this._languageGeneral.VideoInformation;
            this.labelVideoInfo.Text = this._languageGeneral.NoVideoLoaded;
            this.groupBoxStartScene.Text = this._language.StartScene;
            this.groupBoxEndScene.Text = this._language.EndScene;
            this.buttonStartThreeSecondsBack.Text = this._language.ThreeSecondsBack;
            this.buttonThreeSecondsBack.Text = this._language.ThreeSecondsBack;
            this.buttonStartHalfASecondBack.Text = this._language.HalfASecondBack;
            this.buttonEndHalfASecondBack.Text = this._language.HalfASecondBack;
            this.buttonStartVerify.Text = string.Format(this._language.PlayXSecondsAndBack, Configuration.Settings.Tools.VerifyPlaySeconds);
            this.buttonEndVerify.Text = this.buttonStartVerify.Text;
            this.buttonGotoStartSubtitlePosition.Text = this._language.GoToSubPosition;
            this.buttonGotoEndSubtitlePosition.Text = this._language.GoToSubPosition;
            this.buttonFindTextStart.Text = this._language.FindText;
            this.buttonFindTextEnd.Text = this._language.FindText;
            this.buttonSync.Text = this._language.Synchronize;
            this.buttonOK.Text = this._languageGeneral.Ok;
            this.buttonCancel.Text = this._languageGeneral.Cancel;
            this.labelTip.Text = this._language.Tip;
            Utilities.FixLargeFonts(this, this.buttonCancel);
            this._timerHideSyncLabel.Tick += this.timerHideSyncLabel_Tick;
            this._timerHideSyncLabel.Interval = 1000;
        }

        /// <summary>
        /// Gets or sets the video file name.
        /// </summary>
        public string VideoFileName { get; set; }

        /// <summary>
        /// Gets or sets the audio track number.
        /// </summary>
        public int AudioTrackNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ok pressed.
        /// </summary>
        public bool OkPressed { get; set; }

        /// <summary>
        /// Gets a value indicating whether frame rate changed.
        /// </summary>
        public bool FrameRateChanged
        {
            get
            {
                return this._frameRateChanged;
            }
        }

        /// <summary>
        /// Gets the frame rate.
        /// </summary>
        public double FrameRate
        {
            get
            {
                if (this._videoInfo == null)
                {
                    return 0;
                }

                return this._videoInfo.FramesPerSecond;
            }
        }

        /// <summary>
        /// Gets the paragraphs.
        /// </summary>
        public List<Paragraph> Paragraphs
        {
            get
            {
                return this._paragraphs;
            }
        }

        /// <summary>
        /// The timer hide sync label_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timerHideSyncLabel_Tick(object sender, EventArgs e)
        {
            this.labelSyncDone.Text = string.Empty;
        }

        /// <summary>
        /// The goto subtitle position.
        /// </summary>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        private void GotoSubtitlePosition(VideoPlayerContainer mediaPlayer)
        {
            int index;
            if (mediaPlayer == this.MediaPlayerStart)
            {
                index = this.comboBoxStartTexts.SelectedIndex;
            }
            else
            {
                index = this.comboBoxEndTexts.SelectedIndex;
            }

            mediaPlayer.Pause();
            if (index != -1)
            {
                double indexPositionInSeconds = this._paragraphs[index].StartTime.TotalMilliseconds / TimeCode.BaseUnit;

                if (indexPositionInSeconds > mediaPlayer.Duration)
                {
                    indexPositionInSeconds = mediaPlayer.Duration - (2 * 60);
                }

                if (indexPositionInSeconds < 0)
                {
                    indexPositionInSeconds = 0;
                }

                mediaPlayer.CurrentPosition = indexPositionInSeconds;
                mediaPlayer.RefreshProgressBar();
            }
        }

        /// <summary>
        /// The open video.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void OpenVideo(string fileName)
        {
            if (File.Exists(fileName))
            {
                this.timer1.Stop();
                this.timerProgressBarRefresh.Stop();

                this.VideoFileName = fileName;

                var fi = new FileInfo(fileName);
                if (fi.Length < 1000)
                {
                    return;
                }

                if (this.MediaPlayerStart.VideoPlayer != null)
                {
                    this.MediaPlayerStart.Pause();
                    this.MediaPlayerStart.VideoPlayer.DisposeVideoPlayer();
                }

                if (this.MediaPlayerEnd.VideoPlayer != null)
                {
                    this.MediaPlayerEnd.Pause();
                    this.MediaPlayerEnd.VideoPlayer.DisposeVideoPlayer();
                }

                VideoInfo videoInfo = this.ShowVideoInfo(fileName);

                // be sure to match frames with movie
                if (this._originalSubtitle.WasLoadedWithFrameNumbers)
                {
                    // frame based subtitles like MicroDVD
                    if (Math.Abs(this._videoInfo.FramesPerSecond - this._oldFrameRate) > 0.02)
                    {
                        this._originalSubtitle.CalculateTimeCodesFromFrameNumbers(this._videoInfo.FramesPerSecond);
                        this.LoadAndShowOriginalSubtitle();
                        this._frameRateChanged = true;
                    }
                }

                Utilities.InitializeVideoPlayerAndContainer(fileName, videoInfo, this.MediaPlayerStart, this.VideoStartLoaded, this.VideoStartEnded);
            }
        }

        /// <summary>
        /// The video start ended.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoStartEnded(object sender, EventArgs e)
        {
            this.MediaPlayerStart.Pause();
        }

        /// <summary>
        /// The video start loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoStartLoaded(object sender, EventArgs e)
        {
            this.MediaPlayerStart.Pause();
            this.GotoSubtitlePosition(this.MediaPlayerStart);

            this._startGoBackPosition = this.MediaPlayerStart.CurrentPosition;
            this._startStopPosition = this._startGoBackPosition + 0.1;
            this.MediaPlayerStart.Play();

            if (this.MediaPlayerStart.VideoPlayer.GetType() == typeof(LibVlcDynamic))
            {
                this.MediaPlayerEnd.VideoPlayer = (this.MediaPlayerStart.VideoPlayer as LibVlcDynamic).MakeSecondMediaPlayer(this.MediaPlayerEnd.PanelPlayer, this.VideoFileName, this.VideoEndLoaded, this.VideoEndEnded);
            }
            else
            {
                Utilities.InitializeVideoPlayerAndContainer(this.MediaPlayerStart.VideoPlayer.VideoFileName, this._videoInfo, this.MediaPlayerEnd, this.VideoEndLoaded, this.VideoEndEnded);
            }

            this.timer1.Start();
            this.timerProgressBarRefresh.Start();

            if (this.AudioTrackNumber >= 0 && this.MediaPlayerStart.VideoPlayer is LibVlcDynamic)
            {
                var libVlc = (LibVlcDynamic)this.MediaPlayerStart.VideoPlayer;
                libVlc.AudioTrackNumber = this.AudioTrackNumber;
            }
        }

        /// <summary>
        /// The video end ended.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoEndEnded(object sender, EventArgs e)
        {
            this.MediaPlayerEnd.Pause();
        }

        /// <summary>
        /// The video end loaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VideoEndLoaded(object sender, EventArgs e)
        {
            this.MediaPlayerEnd.Pause();
            this.GotoSubtitlePosition(this.MediaPlayerEnd);

            this._endGoBackPosition = this.MediaPlayerEnd.CurrentPosition;
            this._endStopPosition = this._endGoBackPosition + 0.1;
            this.MediaPlayerEnd.Play();

            if (this.AudioTrackNumber >= 0 && this.MediaPlayerEnd.VideoPlayer is LibVlcDynamic)
            {
                var libVlc = (LibVlcDynamic)this.MediaPlayerEnd.VideoPlayer;
                libVlc.AudioTrackNumber = this.AudioTrackNumber;
            }
        }

        /// <summary>
        /// The show video info.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="VideoInfo"/>.
        /// </returns>
        private VideoInfo ShowVideoInfo(string fileName)
        {
            this._videoInfo = Utilities.GetVideoInfo(fileName);
            var info = new FileInfo(fileName);
            long fileSizeInBytes = info.Length;

            this.labelVideoInfo.Text = string.Format(this._languageGeneral.FileNameXAndSize, fileName, Utilities.FormatBytesToDisplayFileSize(fileSizeInBytes)) + Environment.NewLine + string.Format(this._languageGeneral.ResolutionX, +this._videoInfo.Width + "x" + this._videoInfo.Height) + "    ";
            if (this._videoInfo.FramesPerSecond > 5 && this._videoInfo.FramesPerSecond < 200)
            {
                this.labelVideoInfo.Text += string.Format(this._languageGeneral.FrameRateX + "        ", this._videoInfo.FramesPerSecond);
            }

            if (this._videoInfo.TotalFrames > 10)
            {
                this.labelVideoInfo.Text += string.Format(this._languageGeneral.TotalFramesX + "         ", (int)this._videoInfo.TotalFrames);
            }

            if (!string.IsNullOrEmpty(this._videoInfo.VideoCodec))
            {
                this.labelVideoInfo.Text += string.Format(this._languageGeneral.VideoEncodingX, this._videoInfo.VideoCodec) + "        ";
            }

            return this._videoInfo;
        }

        /// <summary>
        /// The timer 1 tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            if (this.MediaPlayerStart != null)
            {
                if (!this.MediaPlayerStart.IsPaused)
                {
                    this.MediaPlayerStart.RefreshProgressBar();
                    if (this._startStopPosition >= 0 && this.MediaPlayerStart.CurrentPosition > this._startStopPosition)
                    {
                        this.MediaPlayerStart.Pause();
                        this.MediaPlayerStart.CurrentPosition = this._startGoBackPosition;
                        this._startStopPosition = -1;
                    }

                    Utilities.ShowSubtitle(this._paragraphs, this.MediaPlayerStart);
                }

                if (!this.MediaPlayerEnd.IsPaused)
                {
                    this.MediaPlayerEnd.RefreshProgressBar();
                    if (this._endStopPosition >= 0 && this.MediaPlayerEnd.CurrentPosition > this._endStopPosition)
                    {
                        this.MediaPlayerEnd.Pause();
                        this.MediaPlayerEnd.CurrentPosition = this._endGoBackPosition;
                        this._endStopPosition = -1;
                    }

                    Utilities.ShowSubtitle(this._paragraphs, this.MediaPlayerEnd);
                }
            }
        }

        /// <summary>
        /// The form visual sync_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormVisualSync_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._timerHideSyncLabel.Stop();
            this.labelSyncDone.Text = string.Empty;
            this.timer1.Stop();
            this.timerProgressBarRefresh.Stop();
            if (this.MediaPlayerStart != null)
            {
                this.MediaPlayerStart.Pause();
            }

            if (this.MediaPlayerEnd != null)
            {
                this.MediaPlayerEnd.Pause();
            }

            bool change = false;
            for (int i = 0; i < this._paragraphs.Count; i++)
            {
                if (this._paragraphs[i].ToString() != this._originalSubtitle.Paragraphs[i].ToString())
                {
                    change = true;
                    break;
                }
            }

            if (!change)
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            DialogResult dr;
            if (this.DialogResult == DialogResult.OK)
            {
                dr = DialogResult.Yes;
            }
            else
            {
                dr = MessageBox.Show(this._language.KeepChangesMessage, this._language.KeepChangesTitle, MessageBoxButtons.YesNoCancel);
            }

            if (dr == DialogResult.Cancel)
            {
                e.Cancel = true;
                this.timer1.Start();
                this.timerProgressBarRefresh.Start();
            }
            else if (dr == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="frameRate">
        /// The frame rate.
        /// </param>
        internal void Initialize(Bitmap bitmap, Subtitle subtitle, string fileName, string title, double frameRate)
        {
            if (bitmap != null)
            {
                IntPtr Hicon = bitmap.GetHicon();
                this.Icon = Icon.FromHandle(Hicon);
            }

            this._originalSubtitle = subtitle;
            this._oldFrameRate = frameRate;
            this._subtitleFileName = fileName;
            this.Text = title;
        }

        /// <summary>
        /// The load and show original subtitle.
        /// </summary>
        private void LoadAndShowOriginalSubtitle()
        {
            this._paragraphs = new List<Paragraph>();
            foreach (Paragraph p in this._originalSubtitle.Paragraphs)
            {
                this._paragraphs.Add(new Paragraph(p));
            }

            this.FillStartAndEndTexts();

            if (this.comboBoxStartTexts.Items.Count > Configuration.Settings.Tools.StartSceneIndex)
            {
                this.comboBoxStartTexts.SelectedIndex = Configuration.Settings.Tools.StartSceneIndex;
            }

            if (this.comboBoxEndTexts.Items.Count > Configuration.Settings.Tools.EndSceneIndex)
            {
                this.comboBoxEndTexts.SelectedIndex = this.comboBoxEndTexts.Items.Count - (Configuration.Settings.Tools.EndSceneIndex + 1);
            }
        }

        /// <summary>
        /// The fill start and end texts.
        /// </summary>
        private void FillStartAndEndTexts()
        {
            this.comboBoxStartTexts.BeginUpdate();
            this.comboBoxEndTexts.BeginUpdate();
            this.comboBoxStartTexts.Items.Clear();
            this.comboBoxEndTexts.Items.Clear();
            foreach (Paragraph p in this._paragraphs)
            {
                string s = p.StartTime + " - " + p.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString);
                this.comboBoxStartTexts.Items.Add(s);
                this.comboBoxEndTexts.Items.Add(s);
            }

            this.comboBoxStartTexts.EndUpdate();
            this.comboBoxEndTexts.EndUpdate();
        }

        /// <summary>
        /// The try to find and open movie file.
        /// </summary>
        /// <param name="fileNameNoExtension">
        /// The file name no extension.
        /// </param>
        private void TryToFindAndOpenMovieFile(string fileNameNoExtension)
        {
            string movieFileName = null;

            foreach (string extension in Utilities.GetMovieFileExtensions())
            {
                movieFileName = fileNameNoExtension + extension;
                if (File.Exists(movieFileName))
                {
                    break;
                }
            }

            if (movieFileName != null && File.Exists(movieFileName))
            {
                this.OpenVideo(movieFileName);
            }
            else if (fileNameNoExtension.Contains('.'))
            {
                fileNameNoExtension = fileNameNoExtension.Substring(0, fileNameNoExtension.LastIndexOf('.'));
                this.TryToFindAndOpenMovieFile(fileNameNoExtension);
            }
        }

        /// <summary>
        /// The button goto start subtitle position click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonGotoStartSubtitlePositionClick(object sender, EventArgs e)
        {
            this.GotoSubtitlePosition(this.MediaPlayerStart);
        }

        /// <summary>
        /// The button goto end subtitle position click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonGotoEndSubtitlePositionClick(object sender, EventArgs e)
        {
            this.GotoSubtitlePosition(this.MediaPlayerEnd);
        }

        /// <summary>
        /// The button sync click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSyncClick(object sender, EventArgs e)
        {
            double startPos = this.MediaPlayerStart.CurrentPosition;
            double endPos = this.MediaPlayerEnd.CurrentPosition;
            if (endPos > startPos)
            {
                double subStart = this._paragraphs[this.comboBoxStartTexts.SelectedIndex].StartTime.TotalMilliseconds / TimeCode.BaseUnit;
                double subEnd = this._paragraphs[this.comboBoxEndTexts.SelectedIndex].StartTime.TotalMilliseconds / TimeCode.BaseUnit;

                double subDiff = subEnd - subStart;
                double realDiff = endPos - startPos;

                // speed factor
                double factor = realDiff / subDiff;

                // adjust to starting position
                double adjust = startPos - subStart * factor;

                foreach (Paragraph p in this._paragraphs)
                {
                    p.Adjust(factor, adjust);
                }

                // fix overlapping time codes
                using (var formFix = new FixCommonErrors())
                {
                    var tmpSubtitle = new Subtitle { WasLoadedWithFrameNumbers = this._originalSubtitle.WasLoadedWithFrameNumbers };
                    foreach (Paragraph p in this._paragraphs)
                    {
                        tmpSubtitle.Paragraphs.Add(new Paragraph(p));
                    }

                    formFix.Initialize(tmpSubtitle, tmpSubtitle.OriginalFormat, System.Text.Encoding.UTF8);
                    formFix.FixOverlappingDisplayTimes();
                    this._paragraphs.Clear();
                    foreach (Paragraph p in formFix.FixedSubtitle.Paragraphs)
                    {
                        this._paragraphs.Add(new Paragraph(p));
                    }
                }

                // update comboboxes
                int startSaveIdx = this.comboBoxStartTexts.SelectedIndex;
                int endSaveIdx = this.comboBoxEndTexts.SelectedIndex;
                this.FillStartAndEndTexts();
                this.comboBoxStartTexts.SelectedIndex = startSaveIdx;
                this.comboBoxEndTexts.SelectedIndex = endSaveIdx;

                this.labelSyncDone.Text = this._language.SynchronizationDone;
                this._timerHideSyncLabel.Start();
            }
            else
            {
                MessageBox.Show(this._language.StartSceneMustComeBeforeEndScene, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// The go back seconds.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <param name="mediaPlayer">
        /// The media player.
        /// </param>
        private void GoBackSeconds(double seconds, VideoPlayerContainer mediaPlayer)
        {
            if (mediaPlayer.CurrentPosition > seconds)
            {
                mediaPlayer.CurrentPosition -= seconds;
            }
            else
            {
                mediaPlayer.CurrentPosition = 0;
            }

            Utilities.ShowSubtitle(this._paragraphs, mediaPlayer);
        }

        /// <summary>
        /// The button start half a second back click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStartHalfASecondBackClick(object sender, EventArgs e)
        {
            this.GoBackSeconds(0.5, this.MediaPlayerStart);
        }

        /// <summary>
        /// The button start three seconds back click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStartThreeSecondsBackClick(object sender, EventArgs e)
        {
            this.GoBackSeconds(3.0, this.MediaPlayerStart);
        }

        /// <summary>
        /// The button end half a second back click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonEndHalfASecondBackClick(object sender, EventArgs e)
        {
            this.GoBackSeconds(0.5, this.MediaPlayerEnd);
        }

        /// <summary>
        /// The button three seconds back click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonThreeSecondsBackClick(object sender, EventArgs e)
        {
            this.GoBackSeconds(3.0, this.MediaPlayerEnd);
        }

        /// <summary>
        /// The button open movie click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOpenMovieClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.openFileDialog1.InitialDirectory) && !string.IsNullOrEmpty(this._subtitleFileName))
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this._subtitleFileName);
            }

            this.openFileDialog1.Title = this._languageGeneral.OpenVideoFileTitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetVideoFileFilter(true);
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.AudioTrackNumber = -1;
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this.openFileDialog1.FileName);
                this.OpenVideo(this.openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// The size wmp.
        /// </summary>
        private void SizeWmp()
        {
            this.MediaPlayerStart.Height = this.panelControlsStart.Top - (this.MediaPlayerStart.Top + 2);
            this.MediaPlayerEnd.Height = this.MediaPlayerStart.Height;
            this.MediaPlayerEnd.RefreshProgressBar();
        }

        /// <summary>
        /// The form visual sync_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormVisualSync_Resize(object sender, EventArgs e)
        {
            int halfWidth = this.Width / 2;
            this.groupBoxStartScene.Width = halfWidth - 18;
            this.MediaPlayerStart.Width = this.groupBoxStartScene.Width - 12;
            this.panelControlsStart.Width = this.MediaPlayerStart.Width;
            this.groupBoxEndScene.Left = halfWidth + 3;
            this.groupBoxEndScene.Width = halfWidth - 18;
            this.MediaPlayerEnd.Width = this.groupBoxEndScene.Width - 12;
            this.SizeWmp();
            this.panelControlsEnd.Width = this.MediaPlayerEnd.Width;
            this.groupBoxStartScene.Height = this.Height - this.groupBoxEndScene.Top - 90;
            this.groupBoxEndScene.Height = this.Height - this.groupBoxEndScene.Top - 90;
            this.SizeWmp();
        }

        /// <summary>
        /// The button find text start click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonFindTextStartClick(object sender, EventArgs e)
        {
            using (var findSubtitle = new FindSubtitleLine())
            {
                findSubtitle.Initialize(this._paragraphs, " " + "(" + this._language.StartScene.ToLower() + ")");
                findSubtitle.ShowDialog();
                if (findSubtitle.SelectedIndex >= 0)
                {
                    this.comboBoxStartTexts.SelectedIndex = findSubtitle.SelectedIndex;
                }
            }
        }

        /// <summary>
        /// The button find text end click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonFindTextEndClick(object sender, EventArgs e)
        {
            using (var findSubtitle = new FindSubtitleLine())
            {
                findSubtitle.Initialize(this._paragraphs, " " + "(" + this._language.EndScene.ToLower() + ")");
                findSubtitle.ShowDialog();
                if (findSubtitle.SelectedIndex >= 0)
                {
                    this.comboBoxEndTexts.SelectedIndex = findSubtitle.SelectedIndex;
                }
            }
        }

        /// <summary>
        /// The highlight start scene.
        /// </summary>
        private void HighlightStartScene()
        {
            this._isStartSceneActive = true;
            this.panelControlsStart.BorderStyle = BorderStyle.FixedSingle;
            this.panelControlsEnd.BorderStyle = BorderStyle.None;
        }

        /// <summary>
        /// The highlight end scene.
        /// </summary>
        private void HighlightEndScene()
        {
            this._isStartSceneActive = false;
            this.panelControlsEnd.BorderStyle = BorderStyle.FixedSingle;
            this.panelControlsStart.BorderStyle = BorderStyle.None;
        }

        /// <summary>
        /// The group box start scene enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GroupBoxStartSceneEnter(object sender, EventArgs e)
        {
            this.HighlightStartScene();
        }

        /// <summary>
        /// The group box end scene enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GroupBoxEndSceneEnter(object sender, EventArgs e)
        {
            this.HighlightEndScene();
        }

        /// <summary>
        /// The visual sync_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VisualSync_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#visual_sync");
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                this.ButtonOpenMovieClick(null, null);
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
            {
                if (this._isStartSceneActive)
                {
                    this.ButtonFindTextStartClick(null, null);
                }
                else
                {
                    this.ButtonFindTextEndClick(null, null);
                }
            }
            else if (this.MediaPlayerStart != null && this.MediaPlayerEnd != null)
            {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
                {
                    if (this._isStartSceneActive)
                    {
                        this._startStopPosition = -1;
                        if (!this.MediaPlayerStart.IsPaused)
                        {
                            this.MediaPlayerStart.Pause();
                        }
                    }
                    else
                    {
                        this._endStopPosition = -1;
                        if (!this.MediaPlayerEnd.IsPaused)
                        {
                            this.MediaPlayerEnd.Pause();
                        }
                    }
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
                {
                    if (this._isStartSceneActive)
                    {
                        this._startStopPosition = -1;
                        this.MediaPlayerStart.TogglePlayPause();
                    }
                    else
                    {
                        this._endStopPosition = -1;
                        this.MediaPlayerStart.TogglePlayPause();
                    }
                }
                else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Left)
                {
                    if (this._isStartSceneActive)
                    {
                        this.GoBackSeconds(0.5, this.MediaPlayerStart);
                    }
                    else
                    {
                        this.GoBackSeconds(0.5, this.MediaPlayerEnd);
                    }

                    e.SuppressKeyPress = true;
                }
                else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Right)
                {
                    if (this._isStartSceneActive)
                    {
                        this.GoBackSeconds(-0.5, this.MediaPlayerStart);
                    }
                    else
                    {
                        this.GoBackSeconds(-0.5, this.MediaPlayerEnd);
                    }

                    e.SuppressKeyPress = true;
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Left)
                {
                    if (this._isStartSceneActive)
                    {
                        this.GoBackSeconds(0.1, this.MediaPlayerStart);
                    }
                    else
                    {
                        this.GoBackSeconds(0.1, this.MediaPlayerEnd);
                    }

                    e.SuppressKeyPress = true;
                }
                else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Right)
                {
                    if (this._isStartSceneActive)
                    {
                        this.GoBackSeconds(-0.1, this.MediaPlayerStart);
                    }
                    else
                    {
                        this.GoBackSeconds(-0.1, this.MediaPlayerEnd);
                    }

                    e.SuppressKeyPress = true;
                }
                else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Right)
                {
                    if (this._isStartSceneActive)
                    {
                        this.GoBackSeconds(1.0, this.MediaPlayerStart);
                    }
                    else
                    {
                        this.GoBackSeconds(1.0, this.MediaPlayerEnd);
                    }

                    e.SuppressKeyPress = true;
                }
                else if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.Left)
                {
                    if (this._isStartSceneActive)
                    {
                        this.GoBackSeconds(-1.0, this.MediaPlayerStart);
                    }
                    else
                    {
                        this.GoBackSeconds(-1.0, this.MediaPlayerEnd);
                    }

                    e.SuppressKeyPress = true;
                }
                else if (e.Modifiers == Keys.None && e.KeyCode == Keys.Space)
                {
                    if (this._isStartSceneActive)
                    {
                        this.MediaPlayerStart.TogglePlayPause();
                    }
                    else
                    {
                        this.MediaPlayerEnd.TogglePlayPause();
                    }

                    e.SuppressKeyPress = true;
                }
            }
        }

        /// <summary>
        /// The visual sync_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VisualSync_Shown(object sender, EventArgs e)
        {
            this.comboBoxStartTexts.Focus();
        }

        /// <summary>
        /// The button start verify click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStartVerifyClick(object sender, EventArgs e)
        {
            if (this.MediaPlayerStart != null && this.MediaPlayerStart.VideoPlayer != null)
            {
                this._startGoBackPosition = this.MediaPlayerStart.CurrentPosition;
                this._startStopPosition = this._startGoBackPosition + Configuration.Settings.Tools.VerifyPlaySeconds;
                this.MediaPlayerStart.Play();
            }
        }

        /// <summary>
        /// The button end verify click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonEndVerifyClick(object sender, EventArgs e)
        {
            if (this.MediaPlayerEnd != null && this.MediaPlayerEnd.VideoPlayer != null)
            {
                this._endGoBackPosition = this.MediaPlayerEnd.CurrentPosition;
                this._endStopPosition = this._endGoBackPosition + Configuration.Settings.Tools.VerifyPlaySeconds;
                this.MediaPlayerEnd.Play();
            }
        }

        /// <summary>
        /// The visual sync_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VisualSync_Load(object sender, EventArgs e)
        {
            this.LoadAndShowOriginalSubtitle();
            if (!string.IsNullOrEmpty(this.VideoFileName) && File.Exists(this.VideoFileName))
            {
                this.OpenVideo(this.VideoFileName);
            }
            else if (!string.IsNullOrEmpty(this._subtitleFileName))
            {
                this.TryToFindAndOpenMovieFile(Path.GetDirectoryName(this._subtitleFileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(this._subtitleFileName));
            }

            this.FormVisualSync_Resize(null, null);
        }

        /// <summary>
        /// The media player start_ on button clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerStart_OnButtonClicked(object sender, EventArgs e)
        {
            if (!this._isStartSceneActive)
            {
                this.HighlightStartScene();
            }
        }

        /// <summary>
        /// The media player end_ on button clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerEnd_OnButtonClicked(object sender, EventArgs e)
        {
            if (this._isStartSceneActive)
            {
                this.HighlightEndScene();
            }
        }

        /// <summary>
        /// The visual sync_ form closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VisualSync_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.MediaPlayerStart.VideoPlayer != null)
            {
                // && MediaPlayerStart.VideoPlayer.GetType() == typeof(QuartsPlayer))
                this.MediaPlayerStart.VideoPlayer.Pause();
                this.MediaPlayerStart.VideoPlayer.DisposeVideoPlayer();
            }

            if (this.MediaPlayerEnd.VideoPlayer != null)
            {
                // && MediaPlayerEnd.VideoPlayer.GetType() == typeof(QuartsPlayer))
                this.MediaPlayerEnd.VideoPlayer.Pause();
                this.MediaPlayerEnd.VideoPlayer.DisposeVideoPlayer();
            }
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
            this.OkPressed = true;
        }

        /// <summary>
        /// The timer progress bar refresh_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void timerProgressBarRefresh_Tick(object sender, EventArgs e)
        {
            if (this.MediaPlayerStart.VideoPlayer != null)
            {
                // && MediaPlayerStart.VideoPlayer.GetType() == typeof(QuartsPlayer))
                this.MediaPlayerStart.RefreshProgressBar();
            }

            if (this.MediaPlayerEnd.VideoPlayer != null)
            {
                // && MediaPlayerEnd.VideoPlayer.GetType() == typeof(QuartsPlayer))
                this.MediaPlayerEnd.RefreshProgressBar();
            }
        }
    }
}