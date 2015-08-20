// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DvdSubRip.cs" company="">
//   
// </copyright>
// <summary>
//   The dvd sub rip.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.VobSub;

    /// <summary>
    /// The dvd sub rip.
    /// </summary>
    public sealed partial class DvdSubRip : Form
    {
        /// <summary>
        /// The _language.
        /// </summary>
        private readonly LanguageStructure.DvdSubRip _language;

        /// <summary>
        /// The _abort.
        /// </summary>
        private volatile bool _abort;

        /// <summary>
        /// The _accumulated presentation timestamp.
        /// </summary>
        private long _accumulatedPresentationTimestamp;

        /// <summary>
        /// The _last nav end pts.
        /// </summary>
        private long _lastNavEndPts;

        /// <summary>
        /// The _last presentation timestamp.
        /// </summary>
        private long _lastPresentationTimestamp;

        /// <summary>
        /// The _last vob presentation timestamp.
        /// </summary>
        private long _lastVobPresentationTimestamp;

        /// <summary>
        /// The _taskbar form handle.
        /// </summary>
        private IntPtr _taskbarFormHandle;

        /// <summary>
        /// The languages.
        /// </summary>
        public List<string> Languages;

        /// <summary>
        /// The merged vob sub packs.
        /// </summary>
        public List<VobSubMergedPack> MergedVobSubPacks;

        /// <summary>
        /// The palette.
        /// </summary>
        public List<Color> Palette;

        /// <summary>
        /// Initializes a new instance of the <see cref="DvdSubRip"/> class.
        /// </summary>
        /// <param name="taskbarFormHandle">
        /// The taskbar form handle.
        /// </param>
        public DvdSubRip(IntPtr taskbarFormHandle)
        {
            this.InitializeComponent();
            this._taskbarFormHandle = taskbarFormHandle;
            this.labelStatus.Text = string.Empty;
            this.buttonStartRipping.Enabled = false;

            this._language = Configuration.Settings.Language.DvdSubRip;
            this.Text = this._language.Title;
            this.groupBoxDvd.Text = this._language.DvdGroupTitle;
            this.labelIfoFile.Text = this._language.IfoFile;
            this.labelVobFiles.Text = this._language.VobFiles;
            this.groupBoxLanguages.Text = this._language.Languages;
            this.groupBoxPalNtsc.Text = this._language.PalNtsc;
            this.radioButtonPal.Text = this._language.Pal;
            this.radioButtonNtsc.Text = this._language.Ntsc;
            this.buttonStartRipping.Text = this._language.StartRipping;
            this.buttonAddVobFile.Text = this._language.Add;
            this.ButtonRemoveVob.Text = this._language.Remove;
            this.buttonClear.Text = this._language.Clear;
            this.ButtonMoveVobDown.Text = this._language.MoveDown;
            this.ButtonMoveVobUp.Text = this._language.MoveUp;

            if (System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "en")
            {
                this.radioButtonNtsc.Checked = true;
            }
            else
            {
                this.radioButtonPal.Checked = true;
            }

            Utilities.FixLargeFonts(this, this.buttonAddVobFile);
        }

        /// <summary>
        /// Gets the selected language.
        /// </summary>
        public string SelectedLanguage
        {
            get
            {
                if (this.comboBoxLanguages.SelectedIndex >= 0)
                {
                    return string.Format("{0} (0x{1:x})", this.comboBoxLanguages.Items[this.comboBoxLanguages.SelectedIndex], this.comboBoxLanguages.SelectedIndex + 32);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// The button open ifo click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOpenIfoClick(object sender, EventArgs e)
        {
            this.openFileDialog1.Multiselect = false;
            this.openFileDialog1.Filter = this._language.IfoFiles + "|*.IFO";
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK && File.Exists(this.openFileDialog1.FileName))
            {
                this.OpenIfoFile(this.openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// The open ifo file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void OpenIfoFile(string fileName)
        {
            this.Clear();
            this.textBoxIfoFileName.Text = fileName;

            // List vob files
            string path = Path.GetDirectoryName(fileName);
            string onlyFileName = Path.GetFileNameWithoutExtension(fileName);
            string searchPattern = onlyFileName.Substring(0, onlyFileName.Length - 1) + "?.VOB";
            this.listBoxVobFiles.Items.Clear();
            for (int i = 1; i < 30; i++)
            {
                string vobFileName = searchPattern.Replace("?", i.ToString(CultureInfo.InvariantCulture));
                if (File.Exists(Path.Combine(path, vobFileName)))
                {
                    this.listBoxVobFiles.Items.Add(Path.Combine(path, vobFileName));
                }
            }

            if (this.listBoxVobFiles.Items.Count == 0)
            {
                searchPattern = onlyFileName.Substring(0, onlyFileName.Length - 1) + "PGC_01_?.VOB";
                for (int i = 1; i < 30; i++)
                {
                    string vobFileName = searchPattern.Replace("?", i.ToString(CultureInfo.InvariantCulture));
                    if (File.Exists(Path.Combine(path, vobFileName)))
                    {
                        this.listBoxVobFiles.Items.Add(Path.Combine(path, vobFileName));
                    }
                }
            }

            using (var ifoParser = new IfoParser(fileName))
            {
                if (!string.IsNullOrEmpty(ifoParser.ErrorMessage))
                {
                    this.Clear();
                    MessageBox.Show(ifoParser.ErrorMessage);
                    return;
                }

                // List info
                this.labelIfoFile.Text = this._language.IfoFile + ": " + ifoParser.VideoTitleSetVobs.VideoStream.Resolution;
                bool isPal = ifoParser.VideoTitleSetVobs.VideoStream.Standard.Equals("PAL", StringComparison.OrdinalIgnoreCase);
                if (isPal)
                {
                    this.radioButtonPal.Checked = true;
                }
                else
                {
                    this.radioButtonNtsc.Checked = true;
                }

                // List languages
                this.comboBoxLanguages.Items.Clear();
                foreach (string s in ifoParser.VideoTitleSetVobs.GetAllLanguages())
                {
                    this.comboBoxLanguages.Items.Add(s);
                }

                if (this.comboBoxLanguages.Items.Count > 0)
                {
                    this.comboBoxLanguages.SelectedIndex = 0;
                }

                // Save palette (Color LookUp Table)
                if (ifoParser.VideoTitleSetProgramChainTable.ProgramChains.Count > 0)
                {
                    this.Palette = ifoParser.VideoTitleSetProgramChainTable.ProgramChains[0].ColorLookupTable;
                }

                ifoParser.Dispose();
            }

            this.buttonStartRipping.Enabled = this.listBoxVobFiles.Items.Count > 0;
        }

        /// <summary>
        /// The button start ripping click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStartRippingClick(object sender, EventArgs e)
        {
            if (this.buttonStartRipping.Text == this._language.Abort)
            {
                this._abort = true;
                this.buttonStartRipping.Text = this._language.StartRipping;
                return;
            }

            this._abort = false;
            this.buttonStartRipping.Text = this._language.Abort;
            this._lastPresentationTimestamp = 0;
            this._lastVobPresentationTimestamp = 0;
            this._lastNavEndPts = 0;
            this._accumulatedPresentationTimestamp = 0;

            this.progressBarRip.Visible = true;
            var ms = new MemoryStream();
            int i = 0;
            foreach (string vobFileName in this.listBoxVobFiles.Items)
            {
                i++;
                this.labelStatus.Text = string.Format(this._language.RippingVobFileXofYZ, Path.GetFileName(vobFileName), i, this.listBoxVobFiles.Items.Count);
                this.Refresh();
                Application.DoEvents();

                if (!this._abort)
                {
                    this.RipSubtitles(vobFileName, ms, i - 1); // Rip/demux subtitle vob packs
                }
            }

            this.progressBarRip.Visible = false;
            TaskbarList.SetProgressState(this._taskbarFormHandle, TaskbarButtonProgressFlags.NoProgress);
            this.buttonStartRipping.Enabled = false;
            if (this._abort)
            {
                this.labelStatus.Text = this._language.AbortedByUser;
                this.buttonStartRipping.Text = this._language.StartRipping;
                this.buttonStartRipping.Enabled = true;
                return;
            }

            this.labelStatus.Text = string.Format(this._language.ReadingSubtitleData);
            this.Refresh();
            Application.DoEvents();
            var vobSub = new VobSubParser(this.radioButtonPal.Checked);
            vobSub.Open(ms);
            ms.Close();
            this.labelStatus.Text = string.Empty;

            this.MergedVobSubPacks = vobSub.MergeVobSubPacks(); // Merge splitted-packs to whole-packs
            if (this.MergedVobSubPacks.Count == 0)
            {
                MessageBox.Show(Configuration.Settings.Language.Main.NoSubtitlesFound);
                this.buttonStartRipping.Text = this._language.StartRipping;
                this.buttonStartRipping.Enabled = true;
                return;
            }

            this.Languages = new List<string>();
            for (int k = 0; k < this.comboBoxLanguages.Items.Count; k++)
            {
                this.Languages.Add(this.comboBoxLanguages.Items[k].ToString());
            }

            this.buttonStartRipping.Text = this._language.StartRipping;
            this.buttonStartRipping.Enabled = true;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The rip subtitles.
        /// </summary>
        /// <param name="vobFileName">
        /// The vob file name.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="vobNumber">
        /// The vob number.
        /// </param>
        private void RipSubtitles(string vobFileName, MemoryStream stream, int vobNumber)
        {
            long firstNavStartPts = 0;

            using (var fs = FileUtil.RetryOpenRead(vobFileName))
            {
                var buffer = new byte[0x800];
                long position = 0;
                this.progressBarRip.Maximum = 100;
                this.progressBarRip.Value = 0;
                int lba = 0;
                long length = fs.Length;
                while (position < length && !this._abort)
                {
                    int bytesRead = 0;

                    // Reading and test for IO errors... and allow abort/retry/ignore
                    var tryAgain = true;
                    while (tryAgain && position < length)
                    {
                        tryAgain = false;
                        try
                        {
                            fs.Seek(position, SeekOrigin.Begin);
                            bytesRead = fs.Read(buffer, 0, 0x800);
                        }
                        catch (IOException exception)
                        {
                            var result = MessageBox.Show(string.Format("An error occured while reading file: {0}", exception.Message), string.Empty, MessageBoxButtons.AbortRetryIgnore);
                            if (result == DialogResult.Abort)
                            {
                                return;
                            }

                            if (result == DialogResult.Retry)
                            {
                                tryAgain = true;
                            }

                            if (result == DialogResult.Ignore)
                            {
                                position += 0x800;
                                tryAgain = true;
                            }
                        }
                    }

                    if (VobSubParser.IsMpeg2PackHeader(buffer))
                    {
                        var vsp = new VobSubPack(buffer, null);
                        if (IsSubtitlePack(buffer))
                        {
                            if (vsp.PacketizedElementaryStream.PresentationTimestamp.HasValue && this._accumulatedPresentationTimestamp != 0)
                            {
                                UpdatePresentationTimestamp(buffer, this._accumulatedPresentationTimestamp, vsp);
                            }

                            stream.Write(buffer, 0, 0x800);
                            if (bytesRead < 0x800)
                            {
                                stream.Write(Encoding.ASCII.GetBytes(new string(' ', 0x800 - bytesRead)), 0, 0x800 - bytesRead);
                            }
                        }
                        else if (IsPrivateStream2(buffer, 0x26))
                        {
                            if (Helper.GetEndian(buffer, 0x0026, 4) == 0x1bf && Helper.GetEndian(buffer, 0x0400, 4) == 0x1bf)
                            {
                                uint vobuSPtm = Helper.GetEndian(buffer, 0x0039, 4);
                                uint vobuEPtm = Helper.GetEndian(buffer, 0x003d, 4);

                                this._lastPresentationTimestamp = vobuEPtm;

                                if (firstNavStartPts == 0)
                                {
                                    firstNavStartPts = vobuSPtm;
                                    if (vobNumber == 0)
                                    {
                                        this._accumulatedPresentationTimestamp = -vobuSPtm;
                                    }
                                }

                                if (vobuSPtm + firstNavStartPts + this._accumulatedPresentationTimestamp < this._lastVobPresentationTimestamp)
                                {
                                    this._accumulatedPresentationTimestamp += this._lastNavEndPts - vobuSPtm;
                                }
                                else if (this._lastNavEndPts > vobuEPtm)
                                {
                                    this._accumulatedPresentationTimestamp += this._lastNavEndPts - vobuSPtm;
                                }

                                this._lastNavEndPts = vobuEPtm;
                            }
                        }
                    }

                    position += 0x800;

                    var progress = (int)((position * 100) / length);
                    if (progress != this.progressBarRip.Value)
                    {
                        this.progressBarRip.Value = progress;
                        TaskbarList.SetProgressValue(this._taskbarFormHandle, (vobNumber * 100) + this.progressBarRip.Value, this.progressBarRip.Maximum * this.listBoxVobFiles.Items.Count);
                        Application.DoEvents();
                    }

                    lba++;
                }
            }

            this._lastVobPresentationTimestamp = this._lastPresentationTimestamp;
        }

        /// <summary>
        /// Write the 5 PTS bytes to buffer
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="addPresentationTimestamp">
        /// The add Presentation Timestamp.
        /// </param>
        /// <param name="vsp">
        /// The vsp.
        /// </param>
        private static void UpdatePresentationTimestamp(byte[] buffer, long addPresentationTimestamp, VobSubPack vsp)
        {
            const int presentationTimestampIndex = 23;
            long newPts = addPresentationTimestamp + ((long)vsp.PacketizedElementaryStream.PresentationTimestamp.Value);

            var buffer5B = BitConverter.GetBytes((UInt64)newPts);
            if (BitConverter.IsLittleEndian)
            {
                buffer[presentationTimestampIndex + 4] = (byte)(buffer5B[0] << 1 | Helper.B00000001); // last 7 bits + '1'
                buffer[presentationTimestampIndex + 3] = (byte)((buffer5B[0] >> 7) + (buffer5B[1] << 1)); // the next 8 bits (1 from last byte, 7 from next)
                buffer[presentationTimestampIndex + 2] = (byte)((buffer5B[1] >> 6 | Helper.B00000001) + (buffer5B[2] << 2)); // the next 7 bits (1 from 2nd last byte, 6 from 3rd last byte)
                buffer[presentationTimestampIndex + 1] = (byte)((buffer5B[2] >> 6) + (buffer5B[3] << 2)); // the next 8 bits (2 from 3rd last byte, 6 from 2rd last byte)
                buffer[presentationTimestampIndex] = (byte)((buffer5B[3] >> 6 | Helper.B00000001) + (buffer5B[4] << 2));
            }
            else
            {
                buffer[presentationTimestampIndex + 4] = (byte)(buffer5B[7] << 1 | Helper.B00000001); // last 7 bits + '1'
                buffer[presentationTimestampIndex + 3] = (byte)((buffer5B[7] >> 7) + (buffer5B[6] << 1)); // the next 8 bits (1 from last byte, 7 from next)
                buffer[presentationTimestampIndex + 2] = (byte)((buffer5B[6] >> 6 | Helper.B00000001) + (buffer5B[5] << 2)); // the next 7 bits (1 from 2nd last byte, 6 from 3rd last byte)
                buffer[presentationTimestampIndex + 1] = (byte)((buffer5B[5] >> 6) + (buffer5B[4] << 2)); // the next 8 bits (2 from 3rd last byte, 6 from 2rd last byte)
                buffer[presentationTimestampIndex] = (byte)((buffer5B[4] >> 6 | Helper.B00000001) + (buffer5B[3] << 2));
            }

            if (vsp.PacketizedElementaryStream.PresentationTimestampDecodeTimestampFlags == Helper.B00000010)
            {
                buffer[presentationTimestampIndex] += Helper.B00100000;
            }
            else
            {
                buffer[presentationTimestampIndex] += Helper.B00110000;
            }
        }

        /// <summary>
        /// The is private stream 2.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsPrivateStream2(byte[] buffer, int index)
        {
            return buffer.Length >= index + 3 && buffer[index + 0] == 0 && buffer[index + 1] == 0 && buffer[index + 2] == 1 && buffer[index + 3] == 0xbf;
        }

        /// <summary>
        /// The is subtitle pack.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool IsSubtitlePack(byte[] buffer)
        {
            const int mpeg2HeaderLength = 14;
            if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 1 && buffer[3] == 0xba)
            {
                // 186 - MPEG-2 Pack Header
                if (buffer[mpeg2HeaderLength + 0] == 0 && buffer[mpeg2HeaderLength + 1] == 0 && buffer[mpeg2HeaderLength + 2] == 1 && buffer[mpeg2HeaderLength + 3] == 0xbd)
                {
                    // 189 - Private stream 1 (non MPEG audio, subpictures)
                    int pesHeaderDataLength = buffer[mpeg2HeaderLength + 8];
                    int streamId = buffer[mpeg2HeaderLength + 8 + 1 + pesHeaderDataLength];
                    if (streamId >= 0x20 && streamId <= 0x3f)
                    {
                        // Subtitle IDs allowed (or x3f to x40?)
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The button add vob file click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAddVobFileClick(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = this._language.VobFiles + "|*.VOB";
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Multiselect = true;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK && File.Exists(this.openFileDialog1.FileName))
            {
                foreach (var fileName in this.openFileDialog1.FileNames)
                {
                    this.listBoxVobFiles.Items.Add(fileName);
                }
            }

            this.buttonStartRipping.Enabled = this.listBoxVobFiles.Items.Count > 0;
        }

        /// <summary>
        /// The button move vob up_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonMoveVobUp_Click(object sender, EventArgs e)
        {
            if (this.listBoxVobFiles.SelectedIndex > 0)
            {
                int index = this.listBoxVobFiles.SelectedIndex;
                string old = this.listBoxVobFiles.Items[index].ToString();
                this.listBoxVobFiles.Items.RemoveAt(index);
                this.listBoxVobFiles.Items.Insert(index - 1, old);
                this.listBoxVobFiles.SelectedIndex = index - 1;
            }
        }

        /// <summary>
        /// The button move vob down_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonMoveVobDown_Click(object sender, EventArgs e)
        {
            if (this.listBoxVobFiles.SelectedIndex >= 0 && this.listBoxVobFiles.SelectedIndex < this.listBoxVobFiles.Items.Count - 1)
            {
                int index = this.listBoxVobFiles.SelectedIndex;
                string old = this.listBoxVobFiles.Items[index].ToString();
                this.listBoxVobFiles.Items.RemoveAt(index);
                this.listBoxVobFiles.Items.Insert(index + 1, old);
                this.listBoxVobFiles.SelectedIndex = index + 1;
            }
        }

        /// <summary>
        /// The button remove vob_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonRemoveVob_Click(object sender, EventArgs e)
        {
            if (this.listBoxVobFiles.SelectedIndex >= 0)
            {
                int index = this.listBoxVobFiles.SelectedIndex;
                this.listBoxVobFiles.Items.RemoveAt(index);
                if (index < this.listBoxVobFiles.Items.Count)
                {
                    this.listBoxVobFiles.SelectedIndex = index;
                }
                else if (index > 0)
                {
                    this.listBoxVobFiles.SelectedIndex = index - 1;
                }

                this.buttonStartRipping.Enabled = this.listBoxVobFiles.Items.Count > 0;

                if (this.listBoxVobFiles.Items.Count == 0)
                {
                    this.Clear();
                }
            }
        }

        /// <summary>
        /// The text box ifo file name drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxIfoFileNameDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// The text box ifo file name drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxIfoFileNameDragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length >= 1)
            {
                string fileName = files[0];

                var fi = new FileInfo(fileName);
                string ext = Path.GetExtension(fileName).ToLower();
                if (fi.Length < 1024 * 1024 * 2)
                {
                    // max 2 mb
                    if (ext == ".ifo")
                    {
                        this.OpenIfoFile(fileName);
                    }
                }
            }
        }

        /// <summary>
        /// The list box vob files drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxVobFilesDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// The list box vob files drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxVobFilesDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileName in files)
            {
                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".vob")
                {
                    this.listBoxVobFiles.Items.Add(fileName);
                }
            }

            this.buttonStartRipping.Enabled = this.listBoxVobFiles.Items.Count > 0;
        }

        /// <summary>
        /// The button clear click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonClearClick(object sender, EventArgs e)
        {
            this.Clear();
        }

        /// <summary>
        /// The clear.
        /// </summary>
        private void Clear()
        {
            this.textBoxIfoFileName.Text = string.Empty;
            this.listBoxVobFiles.Items.Clear();
            this.buttonStartRipping.Enabled = false;
            this.comboBoxLanguages.Items.Clear();
            this.labelIfoFile.Text = this._language.IfoFile;
        }

        /// <summary>
        /// The dvd sub rip_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DvdSubRip_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (this.buttonStartRipping.Text == this._language.Abort)
                {
                    this.ButtonStartRippingClick(sender, e);
                }
                else
                {
                    e.SuppressKeyPress = true;
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }
    }
}