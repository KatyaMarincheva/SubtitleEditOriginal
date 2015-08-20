// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddWaveform.cs" company="">
//   
// </copyright>
// <summary>
//   The add waveform.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4;

    /// <summary>
    /// The add waveform.
    /// </summary>
    public sealed partial class AddWaveform : Form
    {
        /// <summary>
        /// The retry encode parameters.
        /// </summary>
        private const string RetryEncodeParameters = "acodec=s16l";

        /// <summary>
        /// The _audio track number.
        /// </summary>
        private int _audioTrackNumber = -1;

        /// <summary>
        /// The _cancel.
        /// </summary>
        private bool _cancel;

        /// <summary>
        /// The _delay in milliseconds.
        /// </summary>
        private int _delayInMilliseconds;

        /// <summary>
        /// The _encode paramters.
        /// </summary>
        private string _encodeParamters;

        /// <summary>
        /// The _spectrogram directory.
        /// </summary>
        private string _spectrogramDirectory;

        /// <summary>
        /// The _wav file name.
        /// </summary>
        private string _wavFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddWaveform"/> class.
        /// </summary>
        public AddWaveform()
        {
            this.InitializeComponent();
            this.labelProgress.Text = string.Empty;
            this.buttonCancel.Visible = false;
            this.labelInfo.Text = string.Empty;
        }

        /// <summary>
        /// Gets the source video file name.
        /// </summary>
        public string SourceVideoFileName { get; private set; }

        /// <summary>
        /// Gets the spectrogram bitmaps.
        /// </summary>
        public List<Bitmap> SpectrogramBitmaps { get; private set; }

        /// <summary>
        /// Gets the wave peak.
        /// </summary>
        public WavePeakGenerator WavePeak { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="videoFile">
        /// The video file.
        /// </param>
        /// <param name="spectrogramDirectory">
        /// The spectrogram directory.
        /// </param>
        /// <param name="audioTrackNumber">
        /// The audio track number.
        /// </param>
        public void Initialize(string videoFile, string spectrogramDirectory, int audioTrackNumber)
        {
            this._audioTrackNumber = audioTrackNumber;
            if (this._audioTrackNumber < 0)
            {
                this._audioTrackNumber = 0;
            }

            this.Text = Configuration.Settings.Language.AddWaveform.Title;
            this.buttonRipWave.Text = Configuration.Settings.Language.AddWaveform.GenerateWaveformData;
            this.labelPleaseWait.Text = Configuration.Settings.Language.AddWaveform.PleaseWait;
            this.labelVideoFileName.Text = videoFile;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.labelSourcevideoFile.Text = Configuration.Settings.Language.AddWaveform.SourceVideoFile;
            this._spectrogramDirectory = spectrogramDirectory;
            this._encodeParamters = Configuration.Settings.General.VlcWaveTranscodeSettings;
        }

        /// <summary>
        /// The get command line process.
        /// </summary>
        /// <param name="inputVideoFile">
        /// The input video file.
        /// </param>
        /// <param name="audioTrackNumber">
        /// The audio track number.
        /// </param>
        /// <param name="outWaveFile">
        /// The out wave file.
        /// </param>
        /// <param name="encodeParamters">
        /// The encode paramters.
        /// </param>
        /// <param name="encoderName">
        /// The encoder name.
        /// </param>
        /// <returns>
        /// The <see cref="Process"/>.
        /// </returns>
        /// <exception cref="DllNotFoundException">
        /// </exception>
        public static Process GetCommandLineProcess(string inputVideoFile, int audioTrackNumber, string outWaveFile, string encodeParamters, out string encoderName)
        {
            encoderName = "VLC";
            string parameters = "\"" + inputVideoFile + "\" -I dummy -vvv --no-sout-video --audio-track=" + audioTrackNumber + " --sout=\"#transcode{acodec=s16l,channels=1,ab=128}:std{access=file,mux=wav,dst=" + outWaveFile + "}\" vlc://quit";
            string exeFilePath;
            if (Configuration.IsRunningOnLinux() || Configuration.IsRunningOnMac())
            {
                exeFilePath = "cvlc";
                parameters = "-vvv --no-sout-video --audio-track=" + audioTrackNumber + " --sout '#transcode{" + encodeParamters + "}:std{mux=wav,access=file,dst=" + outWaveFile + "}' \"" + inputVideoFile + "\" vlc://quit";
            }
            else
            {
                // windows
                exeFilePath = Logic.VideoPlayers.LibVlcDynamic.GetVlcPath("vlc.exe");
                if (!File.Exists(exeFilePath))
                {
                    if (Configuration.Settings.General.UseFFmpegForWaveExtraction && File.Exists(Configuration.Settings.General.FFmpegLocation))
                    {
                        // We will run FFmpeg
                    }
                    else
                    {
                        throw new DllNotFoundException("NO_VLC");
                    }
                }
            }

            if (Configuration.Settings.General.UseFFmpegForWaveExtraction && File.Exists(Configuration.Settings.General.FFmpegLocation))
            {
                encoderName = "FFmpeg";
                const string fFmpegWaveTranscodeSettings = "-i \"{0}\" -vn -ar 24000 -ac 2 -ab 128 -vol 448 -f wav \"{1}\"";

                // -i indicates the input
                // -vn means no video ouput
                // -ar 44100 indicates the sampling frequency.
                // -ab indicates the bit rate (in this example 160kb/s)
                // -vol 448 will boot volume... 256 is normal
                // -ac 2 means 2 channels
                exeFilePath = Configuration.Settings.General.FFmpegLocation;
                parameters = string.Format(fFmpegWaveTranscodeSettings, inputVideoFile, outWaveFile);
            }

            return new Process { StartInfo = new ProcessStartInfo(exeFilePath, parameters) { WindowStyle = ProcessWindowStyle.Hidden } };
        }

        /// <summary>
        /// The button rip wave_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRipWave_Click(object sender, EventArgs e)
        {
            this.buttonRipWave.Enabled = false;
            this._cancel = false;
            this.SourceVideoFileName = this.labelVideoFileName.Text;
            string targetFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
            string targetDriveLetter = null;
            if (!(Configuration.IsRunningOnLinux() || Configuration.IsRunningOnMac()))
            {
                var root = Path.GetPathRoot(targetFile);
                if (root.Length > 1 && root[1] == ':')
                {
                    targetDriveLetter = root.Remove(1);
                }
            }

            this.labelPleaseWait.Visible = true;
            string encoderName;
            Process process;
            try
            {
                process = GetCommandLineProcess(this.SourceVideoFileName, this._audioTrackNumber, targetFile, this._encodeParamters, out encoderName);
                this.labelInfo.Text = encoderName;
            }
            catch (DllNotFoundException)
            {
                if (MessageBox.Show(Configuration.Settings.Language.AddWaveform.VlcMediaPlayerNotFound + Environment.NewLine + Environment.NewLine + Configuration.Settings.Language.AddWaveform.GoToVlcMediaPlayerHomePage, Configuration.Settings.Language.AddWaveform.VlcMediaPlayerNotFoundTitle, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start("http://www.videolan.org/");
                }

                this.buttonRipWave.Enabled = true;
                return;
            }

            process.Start();
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.Visible = true;
            double seconds = 0;
            this.buttonCancel.Visible = true;
            try
            {
                process.PriorityClass = ProcessPriorityClass.Normal;
            }
            catch
            {
            }

            while (!process.HasExited)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
                seconds += 0.1;
                if (seconds < 60)
                {
                    this.labelProgress.Text = string.Format(Configuration.Settings.Language.AddWaveform.ExtractingSeconds, seconds);
                }
                else
                {
                    this.labelProgress.Text = string.Format(Configuration.Settings.Language.AddWaveform.ExtractingMinutes, (int)(seconds / 60), (int)(seconds % 60));
                }

                this.Refresh();
                if (this._cancel)
                {
                    process.Kill();
                    this.progressBar1.Visible = false;
                    this.labelPleaseWait.Visible = false;
                    this.buttonRipWave.Enabled = true;
                    this.buttonCancel.Visible = false;
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }

                if (targetDriveLetter != null && seconds > 1 && Convert.ToInt32(seconds) % 10 == 0)
                {
                    try
                    {
                        var drive = new DriveInfo(targetDriveLetter);
                        if (drive.IsReady)
                        {
                            if (drive.AvailableFreeSpace < 50 * 1000000)
                            {
                                // 50 mb
                                this.labelInfo.ForeColor = Color.Red;
                                this.labelInfo.Text = Configuration.Settings.Language.AddWaveform.LowDiskSpace;
                            }
                            else if (this.labelInfo.ForeColor == Color.Red)
                            {
                                this.labelInfo.Text = string.Format(Configuration.Settings.Language.AddWaveform.FreeDiskSpace, Utilities.FormatBytesToDisplayFileSize(drive.AvailableFreeSpace));
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            this.buttonCancel.Visible = false;
            this.progressBar1.Visible = false;
            this.progressBar1.Style = ProgressBarStyle.Blocks;
            process.Dispose();

            var targetFileInfo = new FileInfo(targetFile);
            if (!targetFileInfo.Exists)
            {
                if (this._encodeParamters != RetryEncodeParameters)
                {
                    this._encodeParamters = RetryEncodeParameters;
                    this.buttonRipWave_Click(null, null);
                    return;
                }

                MessageBox.Show(string.Format(Configuration.Settings.Language.AddWaveform.WaveFileNotFound, IntPtr.Size * 8, process.StartInfo.FileName, process.StartInfo.Arguments));

                this.labelPleaseWait.Visible = false;
                this.labelProgress.Text = string.Empty;
                this.buttonRipWave.Enabled = true;
                return;
            }

            if (targetFileInfo.Length <= 200)
            {
                MessageBox.Show(string.Format(Configuration.Settings.Language.AddWaveform.WaveFileMalformed, encoderName, process.StartInfo.FileName, process.StartInfo.Arguments));

                this.labelPleaseWait.Visible = false;
                this.labelProgress.Text = string.Empty;
                this.buttonRipWave.Enabled = true;
                return;
            }

            this.ReadWaveFile(targetFile, this._delayInMilliseconds);
            this.labelProgress.Text = string.Empty;
            File.Delete(targetFile);
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The read wave file.
        /// </summary>
        /// <param name="targetFile">
        /// The target file.
        /// </param>
        /// <param name="delayInMilliseconds">
        /// The delay in milliseconds.
        /// </param>
        private void ReadWaveFile(string targetFile, int delayInMilliseconds)
        {
            this.labelProgress.Text = Configuration.Settings.Language.AddWaveform.GeneratingPeakFile;
            this.Refresh();

            var waveFile = new WavePeakGenerator(targetFile);

            int sampleRate = Configuration.Settings.VideoControls.WaveformMinimumSampleRate; // Normally 128
            while (waveFile.Header.SampleRate % sampleRate != 0 && sampleRate < 5000)
            {
                sampleRate++; // old sample-rate / new sample-rate must have rest = 0
            }

            waveFile.GeneratePeakSamples(sampleRate, delayInMilliseconds); // samples per second - SampleRate

            if (Configuration.Settings.VideoControls.GenerateSpectrogram)
            {
                this.labelProgress.Text = Configuration.Settings.Language.AddWaveform.GeneratingSpectrogram;
                this.Refresh();
                Directory.CreateDirectory(this._spectrogramDirectory);
                this.SpectrogramBitmaps = waveFile.GenerateFourierData(256, this._spectrogramDirectory, delayInMilliseconds); // image height = nfft / 2
            }

            this.WavePeak = waveFile;
            waveFile.Close();

            this.labelPleaseWait.Visible = false;
        }

        /// <summary>
        /// The add ware form_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AddWareForm_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            var audioTrackNames = new List<string>();
            var mkvAudioTrackNumbers = new Dictionary<int, int>();
            int numberOfAudioTracks = 0;
            if (this.labelVideoFileName.Text.Length > 1 && File.Exists(this.labelVideoFileName.Text))
            {
                if (this.labelVideoFileName.Text.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase))
                { // Choose for number of audio tracks in matroska files
                    MatroskaFile matroska = null;
                    try
                    {
                        matroska = new MatroskaFile(this.labelVideoFileName.Text);
                        if (matroska.IsValid)
                        {
                            foreach (var track in matroska.GetTracks())
                            {
                                if (track.IsAudio)
                                {
                                    numberOfAudioTracks++;
                                    if (track.CodecId != null && track.Language != null)
                                    {
                                        audioTrackNames.Add("#" + track.TrackNumber + ": " + track.CodecId.Replace("\0", string.Empty) + " - " + track.Language.Replace("\0", string.Empty));
                                    }
                                    else
                                    {
                                        audioTrackNames.Add("#" + track.TrackNumber);
                                    }

                                    mkvAudioTrackNumbers.Add(mkvAudioTrackNumbers.Count, track.TrackNumber);
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (matroska != null)
                        {
                            matroska.Dispose();
                        }
                    }
                }
                else if (this.labelVideoFileName.Text.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) || this.labelVideoFileName.Text.EndsWith(".m4v", StringComparison.OrdinalIgnoreCase))
                { // Choose for number of audio tracks in mp4 files
                    try
                    {
                        var mp4 = new MP4Parser(this.labelVideoFileName.Text);
                        var tracks = mp4.GetAudioTracks();
                        int i = 0;
                        foreach (var track in tracks)
                        {
                            i++;
                            if (track.Name != null && track.Mdia != null && track.Mdia.Mdhd != null && track.Mdia.Mdhd.LanguageString != null)
                            {
                                audioTrackNames.Add(i + ":  " + track.Name + " - " + track.Mdia.Mdhd.LanguageString);
                            }
                            else if (track.Name != null)
                            {
                                audioTrackNames.Add(i + ":  " + track.Name);
                            }
                            else
                            {
                                audioTrackNames.Add(i.ToString(CultureInfo.InvariantCulture));
                            }
                        }

                        numberOfAudioTracks = tracks.Count;
                    }
                    catch
                    {
                    }
                }

                if (Configuration.Settings.General.UseFFmpegForWaveExtraction)
                { // don't know how to extract audio number x via FFmpeg...
                    numberOfAudioTracks = 1;
                    this._audioTrackNumber = 0;
                }

                // Choose audio track
                if (numberOfAudioTracks > 1)
                {
                    using (var form = new ChooseAudioTrack(audioTrackNames, this._audioTrackNumber))
                    {
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            this._audioTrackNumber = form.SelectedTrack;
                        }
                        else
                        {
                            this.DialogResult = DialogResult.Cancel;
                            return;
                        }
                    }
                }

                // check for delay in matroska files
                if (this.labelVideoFileName.Text.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase))
                {
                    MatroskaFile matroska = null;
                    try
                    {
                        matroska = new MatroskaFile(this.labelVideoFileName.Text);
                        if (matroska.IsValid)
                        {
                            this._delayInMilliseconds = (int)matroska.GetTrackStartTime(mkvAudioTrackNumbers[this._audioTrackNumber]);
                        }
                    }
                    catch
                    {
                        this._delayInMilliseconds = 0;
                    }
                    finally
                    {
                        if (matroska != null)
                        {
                            matroska.Dispose();
                        }
                    }
                }

                this.buttonRipWave_Click(null, null);
            }
            else if (this._wavFileName != null)
            {
                this.FixWaveOnly();
            }
        }

        /// <summary>
        /// The add ware form_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AddWareForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#waveform");
                e.SuppressKeyPress = true;
            }
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
            this._cancel = true;
        }

        /// <summary>
        /// The initialize via wave file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="spectrogramFolder">
        /// The spectrogram folder.
        /// </param>
        internal void InitializeViaWaveFile(string fileName, string spectrogramFolder)
        {
            this._wavFileName = fileName;
            this._spectrogramDirectory = spectrogramFolder;
        }

        /// <summary>
        /// The fix wave only.
        /// </summary>
        private void FixWaveOnly()
        {
            this.Text = Configuration.Settings.Language.AddWaveform.Title;
            this.buttonRipWave.Text = Configuration.Settings.Language.AddWaveform.GenerateWaveformData;
            this.labelPleaseWait.Text = Configuration.Settings.Language.AddWaveform.PleaseWait;
            this.labelVideoFileName.Text = string.Empty;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonRipWave.Enabled = false;
            this._cancel = false;
            this.buttonCancel.Visible = false;
            this.progressBar1.Visible = false;
            this.progressBar1.Style = ProgressBarStyle.Blocks;

            this.labelProgress.Text = Configuration.Settings.Language.AddWaveform.GeneratingPeakFile;
            this.Refresh();
            this.labelPleaseWait.Visible = false;
            try
            {
                this.ReadWaveFile(this._wavFileName, this._delayInMilliseconds);
                this.labelProgress.Text = string.Empty;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.StackTrace);
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}