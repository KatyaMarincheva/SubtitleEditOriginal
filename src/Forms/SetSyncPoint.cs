// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetSyncPoint.cs" company="">
//   
// </copyright>
// <summary>
//   The set sync point.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.VideoPlayers;
    using Nikse.SubtitleEdit.Logic.VideoPlayers.Interfaces;

    /// <summary>
    /// The set sync point.
    /// </summary>
    public sealed partial class SetSyncPoint : Form
    {
        /// <summary>
        /// The _main general go to next subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToNextSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle);

        /// <summary>
        /// The _main general go to prev subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToPrevSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle);

        /// <summary>
        /// The _audio track number.
        /// </summary>
        private int _audioTrackNumber = -1;

        /// <summary>
        /// The _go back position.
        /// </summary>
        private double _goBackPosition;

        /// <summary>
        /// The _guess.
        /// </summary>
        private TimeSpan _guess;

        /// <summary>
        /// The _last position.
        /// </summary>
        private double _lastPosition;

        /// <summary>
        /// The _stop position.
        /// </summary>
        private double _stopPosition = -1.0;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _subtitle file name.
        /// </summary>
        private string _subtitleFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetSyncPoint"/> class.
        /// </summary>
        public SetSyncPoint()
        {
            this.InitializeComponent();

            this.groupBoxSyncPointTimeCode.Text = Configuration.Settings.Language.SetSyncPoint.SyncPointTimeCode;
            this.buttonThreeSecondsBack.Text = Configuration.Settings.Language.SetSyncPoint.ThreeSecondsBack;
            this.buttonHalfASecondBack.Text = Configuration.Settings.Language.SetSyncPoint.HalfASecondBack;
            this.buttonVerify.Text = string.Format(Configuration.Settings.Language.VisualSync.PlayXSecondsAndBack, Configuration.Settings.Tools.VerifyPlaySeconds);
            this.buttonHalfASecondAhead.Text = Configuration.Settings.Language.SetSyncPoint.HalfASecondForward;
            this.buttonThreeSecondsAhead.Text = Configuration.Settings.Language.SetSyncPoint.ThreeSecondsForward;
            this.buttonOpenMovie.Text = Configuration.Settings.Language.General.OpenVideoFile;
            this.buttonSetSyncPoint.Text = Configuration.Settings.Language.PointSync.SetSyncPoint;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.subtitleListView1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.subtitleListView1);
            this.subtitleListView1.AutoSizeAllColumns(this);
            this.buttonFindTextEnd.Text = Configuration.Settings.Language.VisualSync.FindText;
            Utilities.FixLargeFonts(this, this.buttonSetSyncPoint);
        }

        /// <summary>
        /// Gets the video file name.
        /// </summary>
        public string VideoFileName { get; private set; }

        /// <summary>
        /// Gets the synchronization point.
        /// </summary>
        public TimeSpan SynchronizationPoint
        {
            get
            {
                return this.timeUpDownLine.TimeCode.TimeSpan;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="subtitleFileName">
        /// The subtitle file name.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="audioTrackNumber">
        /// The audio track number.
        /// </param>
        public void Initialize(Subtitle subtitle, string subtitleFileName, int index, string videoFileName, int audioTrackNumber)
        {
            this._subtitleFileName = subtitleFileName;
            this._subtitle = subtitle;
            this._audioTrackNumber = audioTrackNumber;
            this.subtitleListView1.Fill(subtitle);
            this._guess = subtitle.Paragraphs[index].StartTime.TimeSpan;
            this.subtitleListView1.Items[index].Selected = true;
            this.Text = string.Format(Configuration.Settings.Language.SetSyncPoint.Title, subtitle.Paragraphs[index].Number + ": " + subtitle.Paragraphs[index]);
            this.labelSubtitle.Text = string.Empty;
            this.labelVideoFileName.Text = Configuration.Settings.Language.General.NoVideoLoaded;

            this.timeUpDownLine.TimeCode = subtitle.Paragraphs[index].StartTime;

            if (!string.IsNullOrEmpty(videoFileName) && File.Exists(videoFileName))
            {
                this.OpenVideo(videoFileName);
            }
            else if (!string.IsNullOrEmpty(subtitleFileName))
            {
                this.TryToFindAndOpenVideoFile(Path.GetDirectoryName(subtitleFileName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(subtitleFileName));
            }
        }

        /// <summary>
        /// The try to find and open video file.
        /// </summary>
        /// <param name="fileNameNoExtension">
        /// The file name no extension.
        /// </param>
        private void TryToFindAndOpenVideoFile(string fileNameNoExtension)
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
                this.TryToFindAndOpenVideoFile(fileNameNoExtension);
            }
        }

        /// <summary>
        /// The button open movie_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOpenMovie_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenVideoFileTitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetVideoFileFilter(false);
            if (File.Exists(this._subtitleFileName))
            {
                var videoFileExt = Utilities.GetVideoFileFilter(false);
                foreach (var file in Directory.GetFiles(Path.GetDirectoryName(this._subtitleFileName)))
                {
                    if (file.EndsWith(".nfo", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".srt", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".sub", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (videoFileExt.Contains(Path.GetExtension(file)))
                    {
                        this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(file);
                        break;
                    }
                }
            }

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this._audioTrackNumber = -1;
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(this.openFileDialog1.FileName);
                this.OpenVideo(this.openFileDialog1.FileName);
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
                var fi = new FileInfo(fileName);
                if (fi.Length < 1000)
                {
                    return;
                }

                this.labelVideoFileName.Text = fileName;
                this.VideoFileName = fileName;
                if (this.videoPlayerContainer1.VideoPlayer != null)
                {
                    this.videoPlayerContainer1.Pause();
                    this.videoPlayerContainer1.VideoPlayer.DisposeVideoPlayer();
                }

                VideoInfo videoInfo = Utilities.GetVideoInfo(fileName);

                Utilities.InitializeVideoPlayerAndContainer(fileName, videoInfo, this.videoPlayerContainer1, this.VideoStartLoaded, this.VideoStartEnded);
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
            this.videoPlayerContainer1.Pause();
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
            this.timer1.Start();

            this.videoPlayerContainer1.Pause();

            if (this._guess.TotalMilliseconds > 0 && this._guess.TotalMilliseconds / TimeCode.BaseUnit < this.videoPlayerContainer1.VideoPlayer.Duration)
            {
                this.videoPlayerContainer1.VideoPlayer.CurrentPosition = this._guess.TotalMilliseconds / TimeCode.BaseUnit;
                this.videoPlayerContainer1.RefreshProgressBar();
            }

            if (this._audioTrackNumber >= 0 && this.videoPlayerContainer1.VideoPlayer is LibVlcDynamic)
            {
                var libVlc = (LibVlcDynamic)this.videoPlayerContainer1.VideoPlayer;
                libVlc.AudioTrackNumber = this._audioTrackNumber;
            }
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
            if (this.videoPlayerContainer1 != null)
            {
                double pos;

                if (this._stopPosition >= 0 && this.videoPlayerContainer1.CurrentPosition > this._stopPosition)
                {
                    this.videoPlayerContainer1.Pause();
                    this.videoPlayerContainer1.CurrentPosition = this._goBackPosition;
                    this._stopPosition = -1;
                }

                if (!this.videoPlayerContainer1.IsPaused)
                {
                    this.videoPlayerContainer1.RefreshProgressBar();
                    pos = this.videoPlayerContainer1.CurrentPosition;
                }
                else
                {
                    pos = this.videoPlayerContainer1.CurrentPosition;
                }

                if (pos != this._lastPosition)
                {
                    Utilities.ShowSubtitle(this._subtitle.Paragraphs, this.videoPlayerContainer1);
                    this.timeUpDownLine.TimeCode = TimeCode.FromSeconds(pos);
                    this._lastPosition = pos;
                }
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
            this.DialogResult = DialogResult.OK;
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
        /// The get time_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GetTime_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp(string.Empty);
            }
            else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                this.videoPlayerContainer1.Pause();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.P && e.Control)
            {
                this.videoPlayerContainer1.VideoPlayer.Pause();
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Left)
            {
                this.GoBackSeconds(0.5, this.videoPlayerContainer1.VideoPlayer);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Right)
            {
                this.GoBackSeconds(-0.5, this.videoPlayerContainer1.VideoPlayer);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Left)
            {
                this.GoBackSeconds(0.1, this.videoPlayerContainer1.VideoPlayer);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Right)
            {
                this.GoBackSeconds(-0.1, this.videoPlayerContainer1.VideoPlayer);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#sync");
                e.SuppressKeyPress = true;
            }
            else if (this._mainGeneralGoToNextSubtitle == e.KeyData || (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt))
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex++;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
                e.SuppressKeyPress = true;
            }
            else if (this._mainGeneralGoToPrevSubtitle == e.KeyData || (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt))
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex--;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The get time_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GetTime_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timer1.Stop();

            if (this.videoPlayerContainer1 != null)
            {
                this.videoPlayerContainer1.Pause();
            }
        }

        /// <summary>
        /// The get time_ form closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GetTime_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.videoPlayerContainer1.VideoPlayer != null)
            {
                // && videoPlayerContainer1.VideoPlayer.GetType() == typeof(QuartsPlayer))
                this.videoPlayerContainer1.VideoPlayer.DisposeVideoPlayer();
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
        private void GoBackSeconds(double seconds, IVideoPlayer mediaPlayer)
        {
            if (mediaPlayer != null)
            {
                if (mediaPlayer.CurrentPosition > seconds)
                {
                    mediaPlayer.CurrentPosition -= seconds;
                }
                else
                {
                    mediaPlayer.CurrentPosition = 0;
                }

                this.videoPlayerContainer1.RefreshProgressBar();
            }
        }

        /// <summary>
        /// The button start half a second back_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonStartHalfASecondBack_Click(object sender, EventArgs e)
        {
            this.GoBackSeconds(0.5, this.videoPlayerContainer1.VideoPlayer);
        }

        /// <summary>
        /// The button start three seconds back_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonStartThreeSecondsBack_Click(object sender, EventArgs e)
        {
            this.GoBackSeconds(3, this.videoPlayerContainer1.VideoPlayer);
        }

        /// <summary>
        /// The button start three seconds ahead_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonStartThreeSecondsAhead_Click(object sender, EventArgs e)
        {
            this.GoBackSeconds(-3.0, this.videoPlayerContainer1.VideoPlayer);
        }

        /// <summary>
        /// The button start half a second ahead_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonStartHalfASecondAhead_Click(object sender, EventArgs e)
        {
            this.GoBackSeconds(-0.5, this.videoPlayerContainer1.VideoPlayer);
        }

        /// <summary>
        /// The button start verify_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonStartVerify_Click(object sender, EventArgs e)
        {
            if (this.videoPlayerContainer1 != null && this.videoPlayerContainer1.VideoPlayer != null)
            {
                this._goBackPosition = this.videoPlayerContainer1.CurrentPosition;
                this._stopPosition = this._goBackPosition + Configuration.Settings.Tools.VerifyPlaySeconds;
                this.videoPlayerContainer1.Play();
            }
        }

        /// <summary>
        /// The get time load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GetTimeLoad(object sender, EventArgs e)
        {
            if (this.subtitleListView1.SelectedItems.Count == 1)
            {
                this.subtitleListView1.SelectIndexAndEnsureVisible(this.subtitleListView1.SelectedItems[0].Index);
            }
        }

        /// <summary>
        /// The subtitle list view 1 mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView1MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.subtitleListView1.SelectedItems.Count == 1)
            {
                int index = this.subtitleListView1.SelectedItems[0].Index;

                this.videoPlayerContainer1.Pause();
                this.videoPlayerContainer1.CurrentPosition = this._subtitle.Paragraphs[index].StartTime.TotalMilliseconds / TimeCode.BaseUnit;
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
                findSubtitle.Initialize(this._subtitle.Paragraphs, string.Empty);
                findSubtitle.ShowDialog();
                if (findSubtitle.SelectedIndex >= 0)
                {
                    this.subtitleListView1.SelectIndexAndEnsureVisible(findSubtitle.SelectedIndex);
                }
            }
        }
    }
}