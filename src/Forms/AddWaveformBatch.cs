// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddWaveformBatch.cs" company="">
//   
// </copyright>
// <summary>
//   The add waveform batch.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska;

    /// <summary>
    /// The add waveform batch.
    /// </summary>
    public sealed partial class AddWaveformBatch : PositionAndSizeForm
    {
        /// <summary>
        /// The _abort.
        /// </summary>
        private bool _abort;

        /// <summary>
        /// The _converting.
        /// </summary>
        private bool _converting;

        /// <summary>
        /// The _delay in milliseconds.
        /// </summary>
        private int _delayInMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddWaveformBatch"/> class.
        /// </summary>
        public AddWaveformBatch()
        {
            this.InitializeComponent();
            this.labelProgress.Text = string.Empty;
            this.labelInfo.Text = string.Empty;
            this.progressBar1.Visible = false;

            this.Text = Configuration.Settings.Language.AddWaveformBatch.Title;
            this.buttonRipWave.Text = Configuration.Settings.Language.AddWaveform.GenerateWaveformData;
            this.buttonDone.Text = Configuration.Settings.Language.General.Ok;

            var l = Configuration.Settings.Language.BatchConvert;
            this.groupBoxInput.Text = l.Input;
            this.labelChooseInputFiles.Text = l.InputDescription;
            this.columnHeaderFName.Text = Configuration.Settings.Language.JoinSubtitles.FileName;
            this.columnHeaderFormat.Text = Configuration.Settings.Language.Main.Controls.SubtitleFormat;
            this.columnHeaderSize.Text = Configuration.Settings.Language.General.Size;
            this.columnHeaderStatus.Text = l.Status;
            this.buttonSearchFolder.Text = l.ScanFolder;
            this.checkBoxScanFolderRecursive.Text = l.Recursive;
            this.checkBoxScanFolderRecursive.Left = this.buttonSearchFolder.Left - this.checkBoxScanFolderRecursive.Width - 5;
        }

        /// <summary>
        /// The remove tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RemoveSelectedFiles();
        }

        /// <summary>
        /// The remove all tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listViewInputFiles.Items.Clear();
        }

        /// <summary>
        /// The remove selected files.
        /// </summary>
        private void RemoveSelectedFiles()
        {
            if (this._converting)
            {
                return;
            }

            for (int i = this.listViewInputFiles.SelectedIndices.Count - 1; i >= 0; i--)
            {
                this.listViewInputFiles.Items.RemoveAt(this.listViewInputFiles.SelectedIndices[i]);
            }
        }

        /// <summary>
        /// The button input browse_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonInputBrowse_Click(object sender, EventArgs e)
        {
            this.buttonInputBrowse.Enabled = false;
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenVideoFile;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetVideoFileFilter(true);
            this.openFileDialog1.Multiselect = true;
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                foreach (string fileName in this.openFileDialog1.FileNames)
                {
                    this.AddInputFile(fileName);
                }
            }

            this.buttonInputBrowse.Enabled = true;
        }

        /// <summary>
        /// The add input file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void AddInputFile(string fileName)
        {
            try
            {
                var ext = Path.GetExtension(fileName).ToLowerInvariant();
                var excludedExtensions = new List<string> { ".srt", ".txt", ".exe", ".ass", ".sub", ".jpg", ".png", ".zip", ".rar" };
                if (string.IsNullOrEmpty(ext) || excludedExtensions.Contains(ext))
                {
                    return;
                }

                foreach (ListViewItem lvi in this.listViewInputFiles.Items)
                {
                    if (lvi.Text.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }

                var fi = new FileInfo(fileName);
                var item = new ListViewItem(fileName);
                item.SubItems.Add(Utilities.FormatBytesToDisplayFileSize(fi.Length));

                item.SubItems.Add(Path.GetExtension(fileName));
                item.SubItems.Add("-");

                this.listViewInputFiles.Items.Add(item);
            }
            catch
            {
                // Ignore errors
            }
        }

        /// <summary>
        /// The button search folder_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSearchFolder_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.listViewInputFiles.BeginUpdate();
                this.buttonRipWave.Enabled = false;
                this.progressBar1.Style = ProgressBarStyle.Marquee;
                this.progressBar1.Visible = true;
                this.buttonInputBrowse.Enabled = false;
                this.buttonSearchFolder.Enabled = false;
                this._abort = false;

                this.SearchFolder(this.folderBrowserDialog1.SelectedPath);

                this.buttonRipWave.Enabled = true;
                this.progressBar1.Style = ProgressBarStyle.Continuous;
                this.progressBar1.Visible = true;
                this.buttonInputBrowse.Enabled = true;
                this.buttonSearchFolder.Enabled = true;
                this.listViewInputFiles.EndUpdate();
            }
        }

        /// <summary>
        /// The search folder.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        private void SearchFolder(string path)
        {
            foreach (string fileName in Directory.GetFiles(path))
            {
                try
                {
                    string ext = Path.GetExtension(fileName).ToLower();
                    if (Utilities.GetMovieFileExtensions().Contains(ext))
                    {
                        var fi = new FileInfo(fileName);
                        if (ext == ".mkv" && FileUtil.IsVobSub(fileName))
                        {
                            this.AddFromSearch(fileName, fi, "Matroska");
                        }
                        else
                        {
                            this.AddFromSearch(fileName, fi, ext.Remove(0, 1));
                        }

                        this.progressBar1.Refresh();
                        Application.DoEvents();
                        if (this._abort)
                        {
                            return;
                        }
                    }
                }
                catch
                {
                }
            }

            if (this.checkBoxScanFolderRecursive.Checked)
            {
                foreach (string directory in Directory.GetDirectories(path))
                {
                    if (directory != "." && directory != "..")
                    {
                        this.SearchFolder(directory);
                    }

                    if (this._abort)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// The add from search.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="fi">
        /// The fi.
        /// </param>
        /// <param name="nameOfFormat">
        /// The name of format.
        /// </param>
        private void AddFromSearch(string fileName, FileInfo fi, string nameOfFormat)
        {
            var item = new ListViewItem(fileName);
            item.SubItems.Add(Utilities.FormatBytesToDisplayFileSize(fi.Length));
            item.SubItems.Add(nameOfFormat);
            item.SubItems.Add("-");
            this.listViewInputFiles.Items.Add(item);
        }

        /// <summary>
        /// The button done click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonDoneClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
            if (this.listViewInputFiles.Items.Count == 0)
            {
                MessageBox.Show(Configuration.Settings.Language.BatchConvert.NothingToConvert);
                return;
            }

            this._converting = true;
            this.buttonRipWave.Enabled = false;
            this.progressBar1.Style = ProgressBarStyle.Blocks;
            this.progressBar1.Maximum = this.listViewInputFiles.Items.Count;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = this.progressBar1.Maximum > 2;
            this.buttonInputBrowse.Enabled = false;
            this.buttonSearchFolder.Enabled = false;
            this._abort = false;
            this.listViewInputFiles.BeginUpdate();
            foreach (ListViewItem item in this.listViewInputFiles.Items)
            {
                item.SubItems[3].Text = "-";
            }

            this.listViewInputFiles.EndUpdate();
            this.Refresh();
            int index = 0;
            while (index < this.listViewInputFiles.Items.Count && this._abort == false)
            {
                var item = this.listViewInputFiles.Items[index];
                item.SubItems[3].Text = Configuration.Settings.Language.AddWaveformBatch.ExtractingAudio;
                string fileName = item.Text;
                try
                {
                    string targetFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
                    Process process;
                    try
                    {
                        string encoderName;
                        process = AddWaveform.GetCommandLineProcess(fileName, -1, targetFile, Configuration.Settings.General.VlcWaveTranscodeSettings, out encoderName);
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
                    while (!process.HasExited && !this._abort)
                    {
                        Application.DoEvents();
                    }

                    // check for delay in matroska files
                    var audioTrackNames = new List<string>();
                    var mkvAudioTrackNumbers = new Dictionary<int, int>();
                    if (fileName.ToLower().EndsWith(".mkv", StringComparison.OrdinalIgnoreCase))
                    {
                        MatroskaFile matroska = null;
                        try
                        {
                            matroska = new MatroskaFile(fileName);

                            if (matroska.IsValid)
                            {
                                foreach (var track in matroska.GetTracks())
                                {
                                    if (track.IsAudio)
                                    {
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

                                if (mkvAudioTrackNumbers.Count > 0)
                                {
                                    this._delayInMilliseconds = (int)matroska.GetTrackStartTime(mkvAudioTrackNumbers[0]);
                                }
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

                    item.SubItems[3].Text = Configuration.Settings.Language.AddWaveformBatch.Calculating;
                    this.MakeWaveformAndSpectrogram(fileName, targetFile, this._delayInMilliseconds);

                    // cleanup
                    try
                    {
                        File.Delete(targetFile);
                    }
                    catch
                    {
                        // don't show error about unsuccessful delete
                    }

                    this.IncrementAndShowProgress();

                    item.SubItems[3].Text = Configuration.Settings.Language.AddWaveformBatch.Done;
                }
                catch
                {
                    this.IncrementAndShowProgress();

                    item.SubItems[3].Text = Configuration.Settings.Language.AddWaveformBatch.Error;
                }

                index++;
            }

            this._converting = false;
            this.labelProgress.Text = string.Empty;
            this.labelInfo.Text = string.Empty;
            this.progressBar1.Visible = false;
            TaskbarList.SetProgressState(this.Handle, TaskbarButtonProgressFlags.NoProgress);
            this.buttonRipWave.Enabled = true;
            this.buttonInputBrowse.Enabled = true;
            this.buttonSearchFolder.Enabled = true;
        }

        /// <summary>
        /// The make waveform and spectrogram.
        /// </summary>
        /// <param name="videoFileName">
        /// The video file name.
        /// </param>
        /// <param name="targetFile">
        /// The target file.
        /// </param>
        /// <param name="delayInMilliseconds">
        /// The delay in milliseconds.
        /// </param>
        private void MakeWaveformAndSpectrogram(string videoFileName, string targetFile, int delayInMilliseconds)
        {
            var waveFile = new WavePeakGenerator(targetFile);

            int sampleRate = Configuration.Settings.VideoControls.WaveformMinimumSampleRate; // Normally 128
            while (waveFile.Header.SampleRate % sampleRate != 0 && sampleRate < 5000)
            {
                sampleRate++; // old sample-rate / new sample-rate must have rest = 0
            }

            waveFile.GeneratePeakSamples(sampleRate, delayInMilliseconds); // samples per second - SampleRate

            // if (Configuration.Settings.VideoControls.GenerateSpectrogram)
            // {
            // //Directory.CreateDirectory(_spectrogramDirectory);
            // //SpectrogramBitmaps = waveFile.GenerateFourierData(256, _spectrogramDirectory, delayInMilliseconds); // image height = nfft / 2
            // }
            waveFile.WritePeakSamples(Main.GetPeakWaveFileName(videoFileName));
            waveFile.Close();
        }

        /// <summary>
        /// The increment and show progress.
        /// </summary>
        private void IncrementAndShowProgress()
        {
            if (this.progressBar1.Value < this.progressBar1.Maximum)
            {
                this.progressBar1.Value++;
            }

            TaskbarList.SetProgressValue(this.Handle, this.progressBar1.Value, this.progressBar1.Maximum);
            this.labelProgress.Text = this.progressBar1.Value + " / " + this.progressBar1.Maximum;
        }

        /// <summary>
        /// The add waveform batch_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AddWaveformBatch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this._abort = true;
            }
        }

        /// <summary>
        /// The context menu strip files_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void contextMenuStripFiles_Opening(object sender, CancelEventArgs e)
        {
            if (this.listViewInputFiles.Items.Count == 0)
            {
                e.Cancel = true;
            }

            this.removeToolStripMenuItem.Visible = this.listViewInputFiles.SelectedItems.Count > 0;
        }

        /// <summary>
        /// The list view input files_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewInputFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (this._converting)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// The list view input files_ drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewInputFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (this._converting)
            {
                return;
            }

            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileName in fileNames)
            {
                this.AddInputFile(fileName);
            }
        }
    }
}