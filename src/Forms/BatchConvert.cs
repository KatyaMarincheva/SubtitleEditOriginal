// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchConvert.cs" company="">
//   
// </copyright>
// <summary>
//   The batch convert.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Forms.Styles;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.BluRaySup;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska;
    using Nikse.SubtitleEdit.Logic.Forms;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The batch convert.
    /// </summary>
    public sealed partial class BatchConvert : PositionAndSizeForm
    {
        /// <summary>
        /// The _all formats.
        /// </summary>
        private readonly IList<SubtitleFormat> _allFormats;

        /// <summary>
        /// The _change casing.
        /// </summary>
        private readonly ChangeCasing _changeCasing = new ChangeCasing();

        /// <summary>
        /// The _change casing names.
        /// </summary>
        private readonly ChangeCasingNames _changeCasingNames = new ChangeCasingNames();

        /// <summary>
        /// The _remove text for hearing impaired.
        /// </summary>
        private readonly RemoveTextForHI _removeTextForHearingImpaired;

        /// <summary>
        /// The _abort.
        /// </summary>
        private bool _abort;

        /// <summary>
        /// The _ass style.
        /// </summary>
        private string _assStyle;

        /// <summary>
        /// The _converted.
        /// </summary>
        private int _converted;

        /// <summary>
        /// The _converting.
        /// </summary>
        private bool _converting;

        /// <summary>
        /// The _count.
        /// </summary>
        private int _count;

        /// <summary>
        /// The _errors.
        /// </summary>
        private int _errors;

        /// <summary>
        /// The _ssa style.
        /// </summary>
        private string _ssaStyle;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchConvert"/> class.
        /// </summary>
        /// <param name="icon">
        /// The icon.
        /// </param>
        public BatchConvert(Icon icon)
        {
            this.InitializeComponent();
            this.Icon = (Icon)icon.Clone();

            this.progressBar1.Visible = false;
            this.labelStatus.Text = string.Empty;
            var l = Configuration.Settings.Language.BatchConvert;
            this.Text = l.Title;
            this.groupBoxInput.Text = l.Input;
            this.labelChooseInputFiles.Text = l.InputDescription;
            this.groupBoxOutput.Text = l.Output;
            this.labelChooseOutputFolder.Text = l.ChooseOutputFolder;
            this.checkBoxOverwrite.Text = l.OverwriteExistingFiles;
            this.labelOutputFormat.Text = Configuration.Settings.Language.Main.Controls.SubtitleFormat;
            this.labelEncoding.Text = Configuration.Settings.Language.Main.Controls.FileEncoding;
            this.buttonStyles.Text = l.Style;
            this.groupBoxConvertOptions.Text = l.ConvertOptions;
            this.checkBoxRemoveFormatting.Text = l.RemoveFormatting;
            this.checkBoxFixCasing.Text = l.RedoCasing;
            this.checkBoxRemoveTextForHI.Text = l.RemoveTextForHI;
            this.checkBoxOverwriteOriginalFiles.Text = l.OverwriteOriginalFiles;
            this.columnHeaderFName.Text = Configuration.Settings.Language.JoinSubtitles.FileName;
            this.columnHeaderFormat.Text = Configuration.Settings.Language.Main.Controls.SubtitleFormat;
            this.columnHeaderSize.Text = Configuration.Settings.Language.General.Size;
            this.columnHeaderStatus.Text = l.Status;
            this.linkLabelOpenOutputFolder.Text = Configuration.Settings.Language.Main.Menu.File.Open;
            this.buttonSearchFolder.Text = l.ScanFolder;
            this.buttonConvert.Text = l.Convert;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Ok;
            this.checkBoxScanFolderRecursive.Text = l.Recursive;
            this.checkBoxScanFolderRecursive.Left = this.buttonSearchFolder.Left - this.checkBoxScanFolderRecursive.Width - 5;

            this.groupBoxChangeFrameRate.Text = Configuration.Settings.Language.ChangeFrameRate.Title;
            this.groupBoxOffsetTimeCodes.Text = Configuration.Settings.Language.ShowEarlierLater.TitleAll;
            this.labelFromFrameRate.Text = Configuration.Settings.Language.ChangeFrameRate.FromFrameRate;
            this.labelToFrameRate.Text = Configuration.Settings.Language.ChangeFrameRate.ToFrameRate;
            this.labelHourMinSecMilliSecond.Text = Configuration.Settings.Language.General.HourMinutesSecondsMilliseconds;

            this.comboBoxFrameRateFrom.Left = this.labelFromFrameRate.Left + this.labelFromFrameRate.Width + 3;
            this.comboBoxFrameRateTo.Left = this.labelToFrameRate.Left + this.labelToFrameRate.Width + 3;
            if (this.comboBoxFrameRateFrom.Left > this.comboBoxFrameRateTo.Left)
            {
                this.comboBoxFrameRateTo.Left = this.comboBoxFrameRateFrom.Left;
            }
            else
            {
                this.comboBoxFrameRateFrom.Left = this.comboBoxFrameRateTo.Left;
            }

            this.comboBoxSubtitleFormats.Left = this.labelOutputFormat.Left + this.labelOutputFormat.Width + 3;
            this.comboBoxEncoding.Left = this.labelEncoding.Left + this.labelEncoding.Width + 3;
            if (this.comboBoxSubtitleFormats.Left > this.comboBoxEncoding.Left)
            {
                this.comboBoxEncoding.Left = this.comboBoxSubtitleFormats.Left;
            }
            else
            {
                this.comboBoxSubtitleFormats.Left = this.comboBoxEncoding.Left;
            }

            this.buttonStyles.Left = this.comboBoxSubtitleFormats.Left + this.comboBoxSubtitleFormats.Width + 5;

            this.timeUpDownAdjust.MaskedTextBox.Text = "000000000";

            this.comboBoxFrameRateFrom.Items.Add(23.976);
            this.comboBoxFrameRateFrom.Items.Add(24.0);
            this.comboBoxFrameRateFrom.Items.Add(25.0);
            this.comboBoxFrameRateFrom.Items.Add(29.97);

            this.comboBoxFrameRateTo.Items.Add(23.976);
            this.comboBoxFrameRateTo.Items.Add(24.0);
            this.comboBoxFrameRateTo.Items.Add(25.0);
            this.comboBoxFrameRateTo.Items.Add(29.97);

            Utilities.FixLargeFonts(this, this.buttonCancel);

            this._allFormats = new List<SubtitleFormat> { new Pac() };
            int selectedFormatIndex = 0;
            for (int index = 0; index < SubtitleFormat.AllSubtitleFormats.Count; index++)
            {
                var f = SubtitleFormat.AllSubtitleFormats[index];
                if (!f.IsVobSubIndexFile)
                {
                    this.comboBoxSubtitleFormats.Items.Add(f.Name);
                    this._allFormats.Add(f);
                    if (Configuration.Settings.Tools.BatchConvertFormat == f.Name)
                    {
                        selectedFormatIndex = index;
                    }
                }
            }

            this.comboBoxSubtitleFormats.SelectedIndex = selectedFormatIndex;
            this.comboBoxSubtitleFormats.Items.Add(l.PlainText);

            this.comboBoxEncoding.Items.Clear();
            int encodingSelectedIndex = 0;
            this.comboBoxEncoding.Items.Add(Encoding.UTF8.EncodingName);
            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.Name != Encoding.UTF8.BodyName && ei.CodePage >= 949 && !ei.DisplayName.Contains("EBCDIC") && ei.CodePage != 1047)
                {
                    this.comboBoxEncoding.Items.Add(ei.CodePage + ": " + ei.DisplayName);
                    if (ei.Name == Configuration.Settings.General.DefaultEncoding)
                    {
                        encodingSelectedIndex = this.comboBoxEncoding.Items.Count - 1;
                    }
                }
            }

            this.comboBoxEncoding.SelectedIndex = encodingSelectedIndex;

            if (string.IsNullOrEmpty(Configuration.Settings.Tools.BatchConvertOutputFolder) || !Directory.Exists(Configuration.Settings.Tools.BatchConvertOutputFolder))
            {
                this.textBoxOutputFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else
            {
                this.textBoxOutputFolder.Text = Configuration.Settings.Tools.BatchConvertOutputFolder;
            }

            this.checkBoxOverwrite.Checked = Configuration.Settings.Tools.BatchConvertOverwriteExisting;
            this.checkBoxOverwriteOriginalFiles.Checked = Configuration.Settings.Tools.BatchConvertOverwriteOriginal;
            this.checkBoxFixCasing.Checked = Configuration.Settings.Tools.BatchConvertFixCasing;
            this.checkBoxFixCommonErrors.Checked = Configuration.Settings.Tools.BatchConvertFixCommonErrors;
            this.checkBoxMultipleReplace.Checked = Configuration.Settings.Tools.BatchConvertMultipleReplace;
            this.checkBoxSplitLongLines.Checked = Configuration.Settings.Tools.BatchConvertSplitLongLines;
            this.checkBoxAutoBalance.Checked = Configuration.Settings.Tools.BatchConvertAutoBalance;
            this.checkBoxRemoveFormatting.Checked = Configuration.Settings.Tools.BatchConvertRemoveFormatting;
            this.checkBoxRemoveTextForHI.Checked = Configuration.Settings.Tools.BatchConvertRemoveTextForHI;
            this.checkBoxSetMinimumDisplayTimeBetweenSubs.Checked = Configuration.Settings.Tools.BatchConvertSetMinDisplayTimeBetweenSubtitles;
            this.buttonRemoveTextForHiSettings.Text = l.Settings;
            this.buttonFixCommonErrorSettings.Text = l.Settings;
            this.buttonMultipleReplaceSettings.Text = l.Settings;
            this.checkBoxFixCommonErrors.Text = Configuration.Settings.Language.FixCommonErrors.Title;
            this.checkBoxMultipleReplace.Text = Configuration.Settings.Language.MultipleReplace.Title;
            this.checkBoxAutoBalance.Text = l.AutoBalance;
            this.checkBoxSplitLongLines.Text = l.SplitLongLines;
            this.radioButtonShowEarlier.Text = Configuration.Settings.Language.ShowEarlierLater.ShowEarlier;
            this.radioButtonShowLater.Text = Configuration.Settings.Language.ShowEarlierLater.ShowLater;
            this.checkBoxSetMinimumDisplayTimeBetweenSubs.Text = l.SetMinMsBetweenSubtitles;

            this._removeTextForHearingImpaired = new Logic.Forms.RemoveTextForHI(new Logic.Forms.RemoveTextForHISettings());

            this.labelFilter.Text = l.Filter;
            this.comboBoxFilter.Items[0] = Configuration.Settings.Language.General.AllFiles;
            this.comboBoxFilter.Items[1] = l.FilterSrtNoUtf8BOM;
            this.comboBoxFilter.Items[2] = l.FilterMoreThanTwoLines;
            this.comboBoxFilter.Items[3] = l.FilterContains;
            this.comboBoxFilter.SelectedIndex = 0;
            this.comboBoxFilter.Left = this.labelFilter.Left + this.labelFilter.Width + 4;
            this.textBoxFilter.Left = this.comboBoxFilter.Left + this.comboBoxFilter.Width + 4;
        }

        /// <summary>
        /// The button choose folder_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonChooseFolder_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowNewFolderButton = true;
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxOutputFolder.Text = this.folderBrowserDialog1.SelectedPath;
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
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenSubtitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetOpenDialogFilter();
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
                foreach (ListViewItem lvi in this.listViewInputFiles.Items)
                {
                    if (lvi.Text.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }

                var fi = new FileInfo(fileName);
                var ext = fi.Extension;
                var item = new ListViewItem(fileName);
                item.SubItems.Add(Utilities.FormatBytesToDisplayFileSize(fi.Length));

                SubtitleFormat format = null;
                var sub = new Subtitle();
                if (fi.Length < 1024 * 1024)
                {
                    // max 1 mb
                    Encoding encoding;
                    format = sub.LoadSubtitle(fileName, out encoding, null);

                    if (format == null)
                    {
                        var ebu = new Ebu();
                        if (ebu.IsMine(null, fileName))
                        {
                            format = ebu;
                        }
                    }

                    if (format == null)
                    {
                        var pac = new Pac();
                        if (pac.IsMine(null, fileName))
                        {
                            format = pac;
                        }
                    }

                    if (format == null)
                    {
                        var cavena890 = new Cavena890();
                        if (cavena890.IsMine(null, fileName))
                        {
                            format = cavena890;
                        }
                    }

                    if (format == null)
                    {
                        var spt = new Spt();
                        if (spt.IsMine(null, fileName))
                        {
                            format = spt;
                        }
                    }

                    if (format == null)
                    {
                        var cheetahCaption = new CheetahCaption();
                        if (cheetahCaption.IsMine(null, fileName))
                        {
                            format = cheetahCaption;
                        }
                    }

                    if (format == null)
                    {
                        var chk = new Chk();
                        if (chk.IsMine(null, fileName))
                        {
                            format = chk;
                        }
                    }

                    if (format == null)
                    {
                        var ayato = new Ayato();
                        if (ayato.IsMine(null, fileName))
                        {
                            format = ayato;
                        }
                    }

                    if (format == null)
                    {
                        var capMakerPlus = new CapMakerPlus();
                        if (capMakerPlus.IsMine(null, fileName))
                        {
                            format = capMakerPlus;
                        }
                    }

                    if (format == null)
                    {
                        var captionate = new Captionate();
                        if (captionate.IsMine(null, fileName))
                        {
                            format = captionate;
                        }
                    }

                    if (format == null)
                    {
                        var ultech130 = new Ultech130();
                        if (ultech130.IsMine(null, fileName))
                        {
                            format = ultech130;
                        }
                    }

                    if (format == null)
                    {
                        var nciCaption = new NciCaption();
                        if (nciCaption.IsMine(null, fileName))
                        {
                            format = nciCaption;
                        }
                    }

                    if (format == null)
                    {
                        var avidStl = new AvidStl();
                        if (avidStl.IsMine(null, fileName))
                        {
                            format = avidStl;
                        }
                    }

                    if (format == null)
                    {
                        var asc = new TimeLineAscii();
                        if (asc.IsMine(null, fileName))
                        {
                            format = asc;
                        }
                    }

                    if (format == null)
                    {
                        var asc = new TimeLineFootageAscii();
                        if (asc.IsMine(null, fileName))
                        {
                            format = asc;
                        }
                    }
                }

                if (format == null)
                {
                    if (FileUtil.IsBluRaySup(fileName))
                    {
                        item.SubItems.Add("Blu-ray");
                    }
                    else if (FileUtil.IsVobSub(fileName))
                    {
                        item.SubItems.Add("VobSub");
                    }
                    else if (ext.Equals(".mkv", StringComparison.OrdinalIgnoreCase) || ext.Equals(".mks", StringComparison.OrdinalIgnoreCase))
                    {
                        int mkvCount = 0;
                        using (var matroska = new MatroskaFile(fileName))
                        {
                            if (matroska.IsValid)
                            {
                                foreach (var track in matroska.GetTracks(true))
                                {
                                    if (track.CodecId.Equals("S_VOBSUB", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // TODO: Convert from VobSub image based format!
                                    }
                                    else if (track.CodecId.Equals("S_HDMV/PGS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        // TODO: Convert from Blu-ray image based format!
                                    }
                                    else if (track.CodecId.Equals("S_TEXT/UTF8", StringComparison.OrdinalIgnoreCase) || track.CodecId.Equals("S_TEXT/SSA", StringComparison.OrdinalIgnoreCase) || track.CodecId.Equals("S_TEXT/ASS", StringComparison.OrdinalIgnoreCase))
                                    {
                                        mkvCount++;
                                    }
                                }
                            }
                        }

                        if (mkvCount > 0)
                        {
                            item.SubItems.Add("Matroska - " + mkvCount);
                        }
                        else
                        {
                            item.SubItems.Add(Configuration.Settings.Language.UnknownSubtitle.Title);
                        }
                    }
                    else
                    {
                        item.SubItems.Add(Configuration.Settings.Language.UnknownSubtitle.Title);
                    }
                }
                else
                {
                    item.SubItems.Add(format.Name);
                }

                item.SubItems.Add("-");

                this.listViewInputFiles.Items.Add(item);
            }
            catch
            {
            }
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
                if (FileUtil.IsDirectory(fileName))
                {
                    this.SearchFolder(fileName);
                }
                else
                {
                    this.AddInputFile(fileName);
                }
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
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The get current encoding.
        /// </summary>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        private Encoding GetCurrentEncoding()
        {
            if (this.comboBoxEncoding.Text == Encoding.UTF8.BodyName || this.comboBoxEncoding.Text == Encoding.UTF8.EncodingName || this.comboBoxEncoding.Text == "utf-8")
            {
                return Encoding.UTF8;
            }

            foreach (EncodingInfo ei in Encoding.GetEncodings())
            {
                if (ei.CodePage + ": " + ei.DisplayName == this.comboBoxEncoding.Text)
                {
                    return ei.GetEncoding();
                }
            }

            return Encoding.UTF8;
        }

        /// <summary>
        /// The get current subtitle format.
        /// </summary>
        /// <returns>
        /// The <see cref="SubtitleFormat"/>.
        /// </returns>
        private SubtitleFormat GetCurrentSubtitleFormat()
        {
            var format = Utilities.GetSubtitleFormatByFriendlyName(this.comboBoxSubtitleFormats.SelectedItem.ToString());
            return format ?? new SubRip();
        }

        /// <summary>
        /// The button convert_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonConvert_Click(object sender, EventArgs e)
        {
            if (this.listViewInputFiles.Items.Count == 0)
            {
                MessageBox.Show(Configuration.Settings.Language.BatchConvert.NothingToConvert);
                return;
            }

            if (!this.checkBoxOverwriteOriginalFiles.Checked)
            {
                if (this.textBoxOutputFolder.Text.Length < 2)
                {
                    MessageBox.Show(Configuration.Settings.Language.BatchConvert.PleaseChooseOutputFolder);
                    return;
                }

                if (!Directory.Exists(this.textBoxOutputFolder.Text))
                {
                    try
                    {
                        Directory.CreateDirectory(this.textBoxOutputFolder.Text);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        return;
                    }
                }
            }

            this._converting = true;
            this.buttonConvert.Enabled = false;
            this.buttonCancel.Enabled = false;
            this.progressBar1.Style = ProgressBarStyle.Blocks;
            this.progressBar1.Maximum = this.listViewInputFiles.Items.Count;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = this.progressBar1.Maximum > 2;
            string toFormat = this.comboBoxSubtitleFormats.Text;
            this.groupBoxOutput.Enabled = false;
            this.groupBoxConvertOptions.Enabled = false;
            this.buttonInputBrowse.Enabled = false;
            this.buttonSearchFolder.Enabled = false;
            this.comboBoxFilter.Enabled = false;
            this.textBoxFilter.Enabled = false;
            this._count = 0;
            this._converted = 0;
            this._errors = 0;
            this._abort = false;
            var worker1 = new BackgroundWorker();
            var worker2 = new BackgroundWorker();
            var worker3 = new BackgroundWorker();
            worker1.DoWork += DoThreadWork;
            worker1.RunWorkerCompleted += this.ThreadWorkerCompleted;
            worker2.DoWork += DoThreadWork;
            worker2.RunWorkerCompleted += this.ThreadWorkerCompleted;
            worker3.DoWork += DoThreadWork;
            worker3.RunWorkerCompleted += this.ThreadWorkerCompleted;
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
                ListViewItem item = this.listViewInputFiles.Items[index];
                string fileName = item.Text;
                try
                {
                    SubtitleFormat format = null;
                    var sub = new Subtitle();
                    var fi = new FileInfo(fileName);
                    if (fi.Length < 1024 * 1024)
                    {
                        // max 1 mb
                        Encoding encoding;
                        format = sub.LoadSubtitle(fileName, out encoding, null);
                        if (format == null)
                        {
                            var ebu = new Ebu();
                            if (ebu.IsMine(null, fileName))
                            {
                                ebu.LoadSubtitle(sub, null, fileName);
                                format = ebu;
                            }
                        }

                        if (format == null)
                        {
                            var pac = new Pac();
                            if (pac.IsMine(null, fileName))
                            {
                                pac.BatchMode = true;
                                pac.LoadSubtitle(sub, null, fileName);
                                format = pac;
                            }
                        }

                        if (format == null)
                        {
                            var cavena890 = new Cavena890();
                            if (cavena890.IsMine(null, fileName))
                            {
                                cavena890.LoadSubtitle(sub, null, fileName);
                                format = cavena890;
                            }
                        }

                        if (format == null)
                        {
                            var spt = new Spt();
                            if (spt.IsMine(null, fileName))
                            {
                                spt.LoadSubtitle(sub, null, fileName);
                                format = spt;
                            }
                        }

                        if (format == null)
                        {
                            var cheetahCaption = new CheetahCaption();
                            if (cheetahCaption.IsMine(null, fileName))
                            {
                                cheetahCaption.LoadSubtitle(sub, null, fileName);
                                format = cheetahCaption;
                            }
                        }

                        if (format == null)
                        {
                            var capMakerPlus = new CapMakerPlus();
                            if (capMakerPlus.IsMine(null, fileName))
                            {
                                capMakerPlus.LoadSubtitle(sub, null, fileName);
                                format = capMakerPlus;
                            }
                        }

                        if (format == null)
                        {
                            var captionate = new Captionate();
                            if (captionate.IsMine(null, fileName))
                            {
                                captionate.LoadSubtitle(sub, null, fileName);
                                format = captionate;
                            }
                        }

                        if (format == null)
                        {
                            var ultech130 = new Ultech130();
                            if (ultech130.IsMine(null, fileName))
                            {
                                ultech130.LoadSubtitle(sub, null, fileName);
                                format = ultech130;
                            }
                        }

                        if (format == null)
                        {
                            var nciCaption = new NciCaption();
                            if (nciCaption.IsMine(null, fileName))
                            {
                                nciCaption.LoadSubtitle(sub, null, fileName);
                                format = nciCaption;
                            }
                        }

                        if (format == null)
                        {
                            var avidStl = new AvidStl();
                            if (avidStl.IsMine(null, fileName))
                            {
                                avidStl.LoadSubtitle(sub, null, fileName);
                                format = avidStl;
                            }
                        }

                        if (format == null)
                        {
                            var elr = new ELRStudioClosedCaption();
                            if (elr.IsMine(null, fileName))
                            {
                                elr.LoadSubtitle(sub, null, fileName);
                                format = elr;
                            }
                        }

                        if (format != null && format.GetType() == typeof(MicroDvd))
                        {
                            if (sub != null && sub.Paragraphs.Count > 0 && sub.Paragraphs[0].Duration.TotalMilliseconds < 1001)
                            {
                                if (sub.Paragraphs[0].Text.StartsWith("29.", StringComparison.Ordinal) || sub.Paragraphs[0].Text.StartsWith("23.", StringComparison.Ordinal) || sub.Paragraphs[0].Text.StartsWith("29,", StringComparison.Ordinal) || sub.Paragraphs[0].Text.StartsWith("23,", StringComparison.Ordinal) || sub.Paragraphs[0].Text == "24" || sub.Paragraphs[0].Text == "25" || sub.Paragraphs[0].Text == "30" || sub.Paragraphs[0].Text == "60")
                                {
                                    sub.Paragraphs.RemoveAt(0);
                                }
                            }
                        }
                    }

                    var bluRaySubtitles = new List<BluRaySupParser.PcsData>();
                    bool isVobSub = false;
                    bool isMatroska = false;
                    if (format == null && fileName.EndsWith(".sup", StringComparison.OrdinalIgnoreCase) && FileUtil.IsBluRaySup(fileName))
                    {
                        var log = new StringBuilder();
                        bluRaySubtitles = BluRaySupParser.ParseBluRaySup(fileName, log);
                    }
                    else if (format == null && fileName.EndsWith(".sub", StringComparison.OrdinalIgnoreCase) && FileUtil.IsVobSub(fileName))
                    {
                        isVobSub = true;
                    }
                    else if (format == null && (fileName.EndsWith(".mkv", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".mks", StringComparison.OrdinalIgnoreCase)) && item.SubItems[2].Text.StartsWith("Matroska"))
                    {
                        isMatroska = true;
                    }

                    if (format == null && bluRaySubtitles.Count == 0 && !isVobSub && !isMatroska)
                    {
                        this.IncrementAndShowProgress();
                    }
                    else
                    {
                        if (isMatroska && (Path.GetExtension(fileName).Equals(".mkv", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(fileName).Equals(".mks", StringComparison.OrdinalIgnoreCase)))
                        {
                            using (var matroska = new MatroskaFile(fileName))
                            {
                                if (matroska.IsValid)
                                {
                                    foreach (var track in matroska.GetTracks(true))
                                    {
                                        if (track.CodecId.Equals("S_VOBSUB", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // TODO: Convert from VobSub image based format!
                                        }
                                        else if (track.CodecId.Equals("S_HDMV/PGS", StringComparison.OrdinalIgnoreCase))
                                        {
                                            // TODO: Convert from Blu-ray image based format!
                                        }
                                        else if (track.CodecId.Equals("S_TEXT/UTF8", StringComparison.OrdinalIgnoreCase) || track.CodecId.Equals("S_TEXT/SSA", StringComparison.OrdinalIgnoreCase) || track.CodecId.Equals("S_TEXT/ASS", StringComparison.OrdinalIgnoreCase))
                                        {
                                            var mkvSub = matroska.GetSubtitle(track.TrackNumber, null);
                                            Utilities.LoadMatroskaTextSubtitle(track, matroska, mkvSub, sub);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (bluRaySubtitles.Count > 0)
                        {
                            item.SubItems[3].Text = Configuration.Settings.Language.BatchConvert.Ocr;
                            using (var vobSubOcr = new VobSubOcr())
                            {
                                vobSubOcr.FileName = Path.GetFileName(fileName);
                                vobSubOcr.InitializeBatch(bluRaySubtitles, Configuration.Settings.VobSubOcr, fileName);
                                sub = vobSubOcr.SubtitleFromOcr;
                            }
                        }
                        else if (isVobSub)
                        {
                            item.SubItems[3].Text = Configuration.Settings.Language.BatchConvert.Ocr;
                            using (var vobSubOcr = new VobSubOcr())
                            {
                                vobSubOcr.InitializeBatch(fileName, Configuration.Settings.VobSubOcr);
                                sub = vobSubOcr.SubtitleFromOcr;
                            }
                        }

                        if (this.comboBoxSubtitleFormats.Text == AdvancedSubStationAlpha.NameOfFormat && this._assStyle != null)
                        {
                            sub.Header = this._assStyle;
                        }
                        else if (this.comboBoxSubtitleFormats.Text == SubStationAlpha.NameOfFormat && this._ssaStyle != null)
                        {
                            sub.Header = this._ssaStyle;
                        }

                        bool skip = this.CheckSkipFilter(fileName, format, sub);
                        if (skip)
                        {
                            item.SubItems[3].Text = Configuration.Settings.Language.BatchConvert.FilterSkipped;
                        }
                        else
                        {
                            foreach (Paragraph p in sub.Paragraphs)
                            {
                                if (this.checkBoxRemoveTextForHI.Checked)
                                {
                                    p.Text = this._removeTextForHearingImpaired.RemoveTextFromHearImpaired(p.Text);
                                }

                                if (this.checkBoxRemoveFormatting.Checked)
                                {
                                    p.Text = HtmlUtil.RemoveHtmlTags(p.Text, true);
                                }
                            }

                            sub.RemoveEmptyLines();
                            if (this.checkBoxFixCasing.Checked)
                            {
                                this._changeCasing.FixCasing(sub, Utilities.AutoDetectGoogleLanguage(sub));
                                this._changeCasingNames.Initialize(sub);
                                this._changeCasingNames.FixCasing();
                            }

                            double fromFrameRate;
                            double toFrameRate;
                            if (double.TryParse(this.comboBoxFrameRateFrom.Text.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out fromFrameRate) && double.TryParse(this.comboBoxFrameRateTo.Text.Replace(",", "."), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out toFrameRate))
                            {
                                sub.ChangeFrameRate(fromFrameRate, toFrameRate);
                            }

                            if (this.timeUpDownAdjust.TimeCode.TotalMilliseconds > 0.00001)
                            {
                                var totalMilliseconds = this.timeUpDownAdjust.TimeCode.TotalMilliseconds;
                                if (this.radioButtonShowEarlier.Checked)
                                {
                                    totalMilliseconds *= -1;
                                }

                                sub.AddTimeToAllParagraphs(TimeSpan.FromMilliseconds(totalMilliseconds));
                            }

                            while (worker1.IsBusy && worker2.IsBusy && worker3.IsBusy)
                            {
                                Application.DoEvents();
                                System.Threading.Thread.Sleep(100);
                            }

                            var parameter = new ThreadDoWorkParameter(this.checkBoxFixCommonErrors.Checked, this.checkBoxMultipleReplace.Checked, this.checkBoxSplitLongLines.Checked, this.checkBoxAutoBalance.Checked, this.checkBoxSetMinimumDisplayTimeBetweenSubs.Checked, item, sub, this.GetCurrentSubtitleFormat(), this.GetCurrentEncoding(), Configuration.Settings.Tools.BatchConvertLanguage, fileName, toFormat, format);
                            if (!worker1.IsBusy)
                            {
                                worker1.RunWorkerAsync(parameter);
                            }
                            else if (!worker2.IsBusy)
                            {
                                worker2.RunWorkerAsync(parameter);
                            }
                            else if (!worker3.IsBusy)
                            {
                                worker3.RunWorkerAsync(parameter);
                            }
                        }
                    }
                }
                catch
                {
                    this.IncrementAndShowProgress();
                }

                index++;
            }

            while (worker1.IsBusy || worker2.IsBusy || worker3.IsBusy)
            {
                try
                {
                    Application.DoEvents();
                }
                catch
                {
                }

                System.Threading.Thread.Sleep(100);
            }

            this._converting = false;
            this.labelStatus.Text = string.Empty;
            this.progressBar1.Visible = false;
            TaskbarList.SetProgressState(this.Handle, TaskbarButtonProgressFlags.NoProgress);
            this.buttonConvert.Enabled = true;
            this.buttonCancel.Enabled = true;
            this.groupBoxOutput.Enabled = true;
            this.groupBoxConvertOptions.Enabled = true;
            this.buttonInputBrowse.Enabled = true;
            this.buttonSearchFolder.Enabled = true;
            this.comboBoxFilter.Enabled = true;
            this.textBoxFilter.Enabled = true;
        }

        /// <summary>
        /// The check skip filter.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="sub">
        /// The sub.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CheckSkipFilter(string fileName, SubtitleFormat format, Subtitle sub)
        {
            bool skip = false;
            if (this.comboBoxFilter.SelectedIndex == 1)
            {
                if (format != null && format.GetType() == typeof(SubRip) && FileUtil.HasUtf8Bom(fileName))
                {
                    skip = true;
                }
            }
            else if (this.comboBoxFilter.SelectedIndex == 2)
            {
                foreach (Paragraph p in sub.Paragraphs)
                {
                    if (p.Text != null && Utilities.GetNumberOfLines(p.Text) > 2)
                    {
                        skip = true;
                        break;
                    }
                }
            }
            else if (this.comboBoxFilter.SelectedIndex == 3 && !string.IsNullOrWhiteSpace(this.textBoxFilter.Text))
            {
                skip = true;
                foreach (Paragraph p in sub.Paragraphs)
                {
                    if (p.Text != null && p.Text.Contains(this.textBoxFilter.Text, StringComparison.Ordinal))
                    {
                        skip = false;
                        break;
                    }
                }
            }

            return skip;
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
            this.labelStatus.Text = this.progressBar1.Value + " / " + this.progressBar1.Maximum;
        }

        /// <summary>
        /// The do thread work.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void DoThreadWork(object sender, DoWorkEventArgs e)
        {
            var p = (ThreadDoWorkParameter)e.Argument;
            if (p.FixCommonErrors)
            {
                try
                {
                    using (var fixCommonErrors = new FixCommonErrors())
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            fixCommonErrors.RunBatch(p.Subtitle, p.Format, p.Encoding, Configuration.Settings.Tools.BatchConvertLanguage);
                            p.Subtitle = fixCommonErrors.FixedSubtitle;
                        }
                    }
                }
                catch (Exception exception)
                {
                    p.Error = string.Format(Configuration.Settings.Language.BatchConvert.FixCommonErrorsErrorX, exception.Message);
                }
            }

            if (p.MultipleReplaceActive)
            {
                try
                {
                    using (var form = new MultipleReplace())
                    {
                        form.RunFromBatch(p.Subtitle);
                        p.Subtitle = form.FixedSubtitle;

                        foreach (int deleteIndex in form.DeleteIndices)
                        {
                            p.Subtitle.Paragraphs.RemoveAt(deleteIndex);
                        }

                        p.Subtitle.Renumber();
                    }
                }
                catch (Exception exception)
                {
                    p.Error = string.Format(Configuration.Settings.Language.BatchConvert.MultipleReplaceErrorX, exception.Message);
                }
            }

            if (p.SplitLongLinesActive)
            {
                try
                {
                    p.Subtitle = Logic.Forms.SplitLongLinesHelper.SplitLongLinesInSubtitle(p.Subtitle, Configuration.Settings.General.SubtitleLineMaximumLength * 2, Configuration.Settings.General.SubtitleLineMaximumLength);
                }
                catch (Exception exception)
                {
                    p.Error = string.Format(Configuration.Settings.Language.BatchConvert.AutoBalanceErrorX, exception.Message);
                }
            }

            if (p.AutoBalanceActive)
            {
                try
                {
                    foreach (var paragraph in p.Subtitle.Paragraphs)
                    {
                        paragraph.Text = Utilities.AutoBreakLine(paragraph.Text);
                    }
                }
                catch (Exception exception)
                {
                    p.Error = string.Format(Configuration.Settings.Language.BatchConvert.AutoBalanceErrorX, exception.Message);
                }
            }

            if (p.SetMinDisplayTimeBetweenSubtitles)
            {
                double minumumMillisecondsBetweenLines = Configuration.Settings.General.MinimumMillisecondsBetweenLines;
                for (int i = 0; i < p.Subtitle.Paragraphs.Count - 1; i++)
                {
                    Paragraph current = p.Subtitle.GetParagraphOrDefault(i);
                    Paragraph next = p.Subtitle.GetParagraphOrDefault(i + 1);
                    var gapsBetween = next.StartTime.TotalMilliseconds - current.EndTime.TotalMilliseconds;
                    if (gapsBetween < minumumMillisecondsBetweenLines && current.Duration.TotalMilliseconds > minumumMillisecondsBetweenLines)
                    {
                        current.EndTime.TotalMilliseconds -= minumumMillisecondsBetweenLines - gapsBetween;
                    }
                }
            }

            // always re-number
            p.Subtitle.Renumber();

            e.Result = p;
        }

        /// <summary>
        /// The thread worker completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ThreadWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var p = (ThreadDoWorkParameter)e.Result;
            if (p.Item.Index + 2 < this.listViewInputFiles.Items.Count)
            {
                this.listViewInputFiles.EnsureVisible(p.Item.Index + 2);
            }
            else
            {
                this.listViewInputFiles.EnsureVisible(p.Item.Index);
            }

            if (!string.IsNullOrEmpty(p.Error))
            {
                p.Item.SubItems[3].Text = p.Error;
            }
            else
            {
                if (p.SourceFormat == null)
                {
                    p.SourceFormat = new SubRip();
                }

                bool success;
                if (this.checkBoxOverwriteOriginalFiles.Checked)
                {
                    success = CommandLineConvert.BatchConvertSave(p.ToFormat, null, this.GetCurrentEncoding(), Path.GetDirectoryName(p.FileName), this._count, ref this._converted, ref this._errors, this._allFormats, p.FileName, p.Subtitle, p.SourceFormat, true, string.Empty, null);
                }
                else
                {
                    success = CommandLineConvert.BatchConvertSave(p.ToFormat, null, this.GetCurrentEncoding(), this.textBoxOutputFolder.Text, this._count, ref this._converted, ref this._errors, this._allFormats, p.FileName, p.Subtitle, p.SourceFormat, this.checkBoxOverwrite.Checked, string.Empty, null);
                }

                if (success)
                {
                    p.Item.SubItems[3].Text = Configuration.Settings.Language.BatchConvert.Converted;
                }
                else
                {
                    p.Item.SubItems[3].Text = Configuration.Settings.Language.BatchConvert.NotConverted;
                }

                this.IncrementAndShowProgress();
                if (this.progressBar1.Value == this.progressBar1.Maximum)
                {
                    this.labelStatus.Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// The combo box subtitle formats selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxSubtitleFormatsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxSubtitleFormats.Text == AdvancedSubStationAlpha.NameOfFormat || this.comboBoxSubtitleFormats.Text == SubStationAlpha.NameOfFormat)
            {
                this.buttonStyles.Visible = true;
            }
            else
            {
                this.buttonStyles.Visible = false;
            }

            this._assStyle = null;
            this._ssaStyle = null;
        }

        /// <summary>
        /// The button styles click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonStylesClick(object sender, EventArgs e)
        {
            SubStationAlphaStyles form = null;
            try
            {
                var assa = new AdvancedSubStationAlpha();
                if (this.comboBoxSubtitleFormats.Text == assa.Name)
                {
                    form = new SubStationAlphaStyles(new Subtitle(), assa);
                    form.MakeOnlyOneStyle();
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        this._assStyle = form.Header;
                    }
                }
                else
                {
                    var ssa = new SubStationAlpha();
                    if (this.comboBoxSubtitleFormats.Text == ssa.Name)
                    {
                        form = new SubStationAlphaStyles(new Subtitle(), ssa);
                        if (form.ShowDialog(this) == DialogResult.OK)
                        {
                            this._ssaStyle = form.Header;
                        }
                    }
                }
            }
            finally
            {
                if (form != null)
                {
                    form.Dispose();
                }
            }
        }

        /// <summary>
        /// The link label open output folder link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LinkLabelOpenOutputFolderLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Directory.Exists(this.textBoxOutputFolder.Text))
            {
                System.Diagnostics.Process.Start(this.textBoxOutputFolder.Text);
            }
            else
            {
                MessageBox.Show(string.Format(Configuration.Settings.Language.SplitSubtitle.FolderNotFoundX, this.textBoxOutputFolder.Text));
            }
        }

        /// <summary>
        /// The context menu strip files opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ContextMenuStripFilesOpening(object sender, CancelEventArgs e)
        {
            if (this.listViewInputFiles.Items.Count == 0 || this._converting)
            {
                e.Cancel = true;
                return;
            }

            this.removeToolStripMenuItem.Visible = this.listViewInputFiles.SelectedItems.Count > 0;
        }

        /// <summary>
        /// The remove all tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RemoveAllToolStripMenuItemClick(object sender, EventArgs e)
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
        /// The remove tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RemoveToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.RemoveSelectedFiles();
        }

        /// <summary>
        /// The list view input files key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListViewInputFilesKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.RemoveSelectedFiles();
            }
        }

        /// <summary>
        /// The button fix common error settings_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonFixCommonErrorSettings_Click(object sender, EventArgs e)
        {
            using (var form = new FixCommonErrors())
            {
                form.RunBatchSettings(new Subtitle(), this.GetCurrentSubtitleFormat(), this.GetCurrentEncoding(), Configuration.Settings.Tools.BatchConvertLanguage);
                form.ShowDialog(this);
                Configuration.Settings.Tools.BatchConvertLanguage = form.Language;
            }
        }

        /// <summary>
        /// The batch convert_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BatchConvert_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._converting)
            {
                e.Cancel = true;
                return;
            }

            Configuration.Settings.Tools.BatchConvertFixCasing = this.checkBoxFixCasing.Checked;
            Configuration.Settings.Tools.BatchConvertFixCommonErrors = this.checkBoxFixCommonErrors.Checked;
            Configuration.Settings.Tools.BatchConvertMultipleReplace = this.checkBoxMultipleReplace.Checked;
            Configuration.Settings.Tools.BatchConvertSplitLongLines = this.checkBoxSplitLongLines.Checked;
            Configuration.Settings.Tools.BatchConvertAutoBalance = this.checkBoxAutoBalance.Checked;
            Configuration.Settings.Tools.BatchConvertRemoveFormatting = this.checkBoxRemoveFormatting.Checked;
            Configuration.Settings.Tools.BatchConvertRemoveTextForHI = this.checkBoxRemoveTextForHI.Checked;
            Configuration.Settings.Tools.BatchConvertSetMinDisplayTimeBetweenSubtitles = this.checkBoxSetMinimumDisplayTimeBetweenSubs.Checked;
            Configuration.Settings.Tools.BatchConvertOutputFolder = this.textBoxOutputFolder.Text;
            Configuration.Settings.Tools.BatchConvertOverwriteExisting = this.checkBoxOverwrite.Checked;
            Configuration.Settings.Tools.BatchConvertOverwriteOriginal = this.checkBoxOverwriteOriginalFiles.Checked;
            Configuration.Settings.Tools.BatchConvertFormat = this.comboBoxSubtitleFormats.SelectedItem.ToString();
        }

        /// <summary>
        /// The button multiple replace settings_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonMultipleReplaceSettings_Click(object sender, EventArgs e)
        {
            using (var form = new MultipleReplace())
            {
                form.Initialize(new Subtitle());
                form.ShowDialog(this);
            }
        }

        /// <summary>
        /// The check box overwrite original files_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxOverwriteOriginalFiles_CheckedChanged(object sender, EventArgs e)
        {
            this.labelChooseOutputFolder.Enabled = !this.checkBoxOverwriteOriginalFiles.Checked;
            this.textBoxOutputFolder.Enabled = !this.checkBoxOverwriteOriginalFiles.Checked;
            this.checkBoxOverwrite.Enabled = !this.checkBoxOverwriteOriginalFiles.Checked;
            this.buttonChooseFolder.Enabled = !this.checkBoxOverwriteOriginalFiles.Checked;
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
                this.buttonConvert.Enabled = false;
                this.buttonCancel.Enabled = false;
                this.progressBar1.Style = ProgressBarStyle.Marquee;
                this.progressBar1.Visible = true;
                this.groupBoxOutput.Enabled = false;
                this.groupBoxConvertOptions.Enabled = false;
                this.buttonInputBrowse.Enabled = false;
                this.buttonSearchFolder.Enabled = false;
                this.labelStatus.Text = string.Format(Configuration.Settings.Language.BatchConvert.ScanningFolder, this.folderBrowserDialog1.SelectedPath);
                this._abort = false;

                this.SearchFolder(this.folderBrowserDialog1.SelectedPath);

                this.labelStatus.Text = string.Empty;
                this.buttonConvert.Enabled = true;
                this.buttonCancel.Enabled = true;
                this.progressBar1.Style = ProgressBarStyle.Continuous;
                this.progressBar1.Visible = true;
                this.groupBoxOutput.Enabled = true;
                this.groupBoxConvertOptions.Enabled = true;
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
                    string ext = Path.GetExtension(fileName).ToLowerInvariant();
                    if (ext != ".png" && ext != ".jpg" && ext != ".dll" && ext != ".exe" && ext != ".zip")
                    {
                        var fi = new FileInfo(fileName);
                        if (ext == ".sub" && FileUtil.IsVobSub(fileName))
                        {
                            this.AddFromSearch(fileName, fi, "VobSub");
                        }
                        else if (ext == ".sup" && FileUtil.IsBluRaySup(fileName))
                        {
                            this.AddFromSearch(fileName, fi, "Blu-ray");
                        }
                        else
                        {
                            if (fi.Length < 1024 * 1024)
                            {
                                // max 1 mb
                                Encoding encoding;
                                var sub = new Subtitle();
                                var format = sub.LoadSubtitle(fileName, out encoding, null);
                                if (format == null)
                                {
                                    var ebu = new Ebu();
                                    if (ebu.IsMine(null, fileName))
                                    {
                                        format = ebu;
                                    }
                                }

                                if (format == null)
                                {
                                    var pac = new Pac();
                                    if (pac.IsMine(null, fileName))
                                    {
                                        format = pac;
                                    }
                                }

                                if (format == null)
                                {
                                    var cavena890 = new Cavena890();
                                    if (cavena890.IsMine(null, fileName))
                                    {
                                        format = cavena890;
                                    }
                                }

                                if (format == null)
                                {
                                    var spt = new Spt();
                                    if (spt.IsMine(null, fileName))
                                    {
                                        format = spt;
                                    }
                                }

                                if (format == null)
                                {
                                    var cheetahCaption = new CheetahCaption();
                                    if (cheetahCaption.IsMine(null, fileName))
                                    {
                                        format = cheetahCaption;
                                    }
                                }

                                if (format == null)
                                {
                                    var capMakerPlus = new CapMakerPlus();
                                    if (capMakerPlus.IsMine(null, fileName))
                                    {
                                        format = capMakerPlus;
                                    }
                                }

                                if (format == null)
                                {
                                    var captionate = new Captionate();
                                    if (captionate.IsMine(null, fileName))
                                    {
                                        format = captionate;
                                    }
                                }

                                if (format == null)
                                {
                                    var ultech130 = new Ultech130();
                                    if (ultech130.IsMine(null, fileName))
                                    {
                                        format = ultech130;
                                    }
                                }

                                if (format == null)
                                {
                                    var nciCaption = new NciCaption();
                                    if (nciCaption.IsMine(null, fileName))
                                    {
                                        format = nciCaption;
                                    }
                                }

                                if (format == null)
                                {
                                    var avidStl = new AvidStl();
                                    if (avidStl.IsMine(null, fileName))
                                    {
                                        format = avidStl;
                                    }
                                }

                                if (format != null)
                                {
                                    this.AddFromSearch(fileName, fi, format.Name);
                                }
                            }
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
        /// The batch convert_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BatchConvert_KeyDown(object sender, KeyEventArgs e)
        {
            if (!this._converting)
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyData == (Keys.Control | Keys.O))
                {
                    // Open file/s
                    this.buttonInputBrowse_Click(null, EventArgs.Empty);
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this._abort = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The button remove text for hi settings_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRemoveTextForHiSettings_Click(object sender, EventArgs e)
        {
            using (var form = new FormRemoveTextForHearImpaired())
            {
                form.InitializeSettingsOnly();
                form.ShowDialog(this);
            }
        }

        /// <summary>
        /// The combo box filter_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxFilter.Visible = this.comboBoxFilter.SelectedIndex == 3;
        }

        /// <summary>
        /// The thread do work parameter.
        /// </summary>
        public class ThreadDoWorkParameter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ThreadDoWorkParameter"/> class.
            /// </summary>
            /// <param name="fixCommonErrors">
            /// The fix common errors.
            /// </param>
            /// <param name="multipleReplace">
            /// The multiple replace.
            /// </param>
            /// <param name="splitLongLinesActive">
            /// The split long lines active.
            /// </param>
            /// <param name="autoBalance">
            /// The auto balance.
            /// </param>
            /// <param name="setMinDisplayTimeBetweenSubtitles">
            /// The set min display time between subtitles.
            /// </param>
            /// <param name="item">
            /// The item.
            /// </param>
            /// <param name="subtitle">
            /// The subtitle.
            /// </param>
            /// <param name="format">
            /// The format.
            /// </param>
            /// <param name="encoding">
            /// The encoding.
            /// </param>
            /// <param name="language">
            /// The language.
            /// </param>
            /// <param name="fileName">
            /// The file name.
            /// </param>
            /// <param name="toFormat">
            /// The to format.
            /// </param>
            /// <param name="sourceFormat">
            /// The source format.
            /// </param>
            public ThreadDoWorkParameter(bool fixCommonErrors, bool multipleReplace, bool splitLongLinesActive, bool autoBalance, bool setMinDisplayTimeBetweenSubtitles, ListViewItem item, Subtitle subtitle, SubtitleFormat format, Encoding encoding, string language, string fileName, string toFormat, SubtitleFormat sourceFormat)
            {
                this.FixCommonErrors = fixCommonErrors;
                this.MultipleReplaceActive = multipleReplace;
                this.SplitLongLinesActive = splitLongLinesActive;
                this.AutoBalanceActive = autoBalance;
                this.SetMinDisplayTimeBetweenSubtitles = setMinDisplayTimeBetweenSubtitles;
                this.Item = item;
                this.Subtitle = subtitle;
                this.Format = format;
                this.Encoding = encoding;
                this.Language = language;
                this.FileName = fileName;
                this.ToFormat = toFormat;
                this.SourceFormat = sourceFormat;
            }

            /// <summary>
            /// Gets or sets a value indicating whether fix common errors.
            /// </summary>
            public bool FixCommonErrors { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether multiple replace active.
            /// </summary>
            public bool MultipleReplaceActive { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether split long lines active.
            /// </summary>
            public bool SplitLongLinesActive { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether auto balance active.
            /// </summary>
            public bool AutoBalanceActive { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether set min display time between subtitles.
            /// </summary>
            public bool SetMinDisplayTimeBetweenSubtitles { get; set; }

            /// <summary>
            /// Gets or sets the item.
            /// </summary>
            public ListViewItem Item { get; set; }

            /// <summary>
            /// Gets or sets the subtitle.
            /// </summary>
            public Subtitle Subtitle { get; set; }

            /// <summary>
            /// Gets or sets the format.
            /// </summary>
            public SubtitleFormat Format { get; set; }

            /// <summary>
            /// Gets or sets the encoding.
            /// </summary>
            public Encoding Encoding { get; set; }

            /// <summary>
            /// Gets or sets the language.
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// Gets or sets the error.
            /// </summary>
            public string Error { get; set; }

            /// <summary>
            /// Gets or sets the file name.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or sets the to format.
            /// </summary>
            public string ToFormat { get; set; }

            /// <summary>
            /// Gets or sets the source format.
            /// </summary>
            public SubtitleFormat SourceFormat { get; set; }
        }
    }
}