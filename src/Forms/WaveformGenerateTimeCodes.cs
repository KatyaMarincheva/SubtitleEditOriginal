// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaveformGenerateTimeCodes.cs" company="">
//   
// </copyright>
// <summary>
//   The waveform generate time codes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The waveform generate time codes.
    /// </summary>
    public partial class WaveformGenerateTimeCodes : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveformGenerateTimeCodes"/> class.
        /// </summary>
        public WaveformGenerateTimeCodes()
        {
            this.InitializeComponent();

            var l = Configuration.Settings.Language.WaveformGenerateTimeCodes;
            this.Text = l.Title;
            this.groupBoxStartFrom.Text = l.StartFrom;
            this.radioButtonStartFromPos.Text = l.CurrentVideoPosition;
            this.radioButtonStartFromStart.Text = l.Beginning;
            this.groupBoxDeleteLines.Text = l.DeleteLines;
            this.groupBoxDetectOptions.Text = l.DetectOptions;
            this.labelScanBlocksMs.Text = l.ScanBlocksOfMs;
            this.labelAbove1.Text = l.BlockAverageVolMin1;
            this.labelAbove2.Text = l.BlockAverageVolMin2;
            this.labelBelow1.Text = l.BlockAverageVolMax1;
            this.labelBelow2.Text = l.BlockAverageVolMax2;
            this.groupBoxOther.Text = l.Other;
            this.labelSplit1.Text = l.SplitLongLinesAt1;
            this.labelSplit2.Text = l.SplitLongLinesAt2;
            this.radioButtonDeleteAll.Text = Configuration.Settings.Language.General.All;
            this.radioButtonDeleteNone.Text = Configuration.Settings.Language.General.None;
            this.radioButtonForward.Text = l.FromCurrentVideoPosition;

            this.numericUpDownBlockSize.Left = this.labelScanBlocksMs.Left + this.labelScanBlocksMs.Width + 3;
            this.numericUpDownMinVol.Left = this.labelAbove1.Left + this.labelAbove1.Width + 3;
            this.labelAbove2.Left = this.numericUpDownMinVol.Left + this.numericUpDownMinVol.Width + 3;
            this.numericUpDownMaxVol.Left = this.labelBelow1.Left + this.labelBelow1.Width + 3;
            this.labelBelow2.Left = this.numericUpDownMaxVol.Left + this.numericUpDownMaxVol.Width + 3;
            this.numericUpDownDefaultMilliseconds.Left = this.labelSplit1.Left + this.labelSplit1.Width + 3;
            this.labelSplit2.Left = this.numericUpDownDefaultMilliseconds.Left + this.numericUpDownDefaultMilliseconds.Width + 3;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
        }

        /// <summary>
        /// Gets or sets a value indicating whether start from video position.
        /// </summary>
        public bool StartFromVideoPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether delete all.
        /// </summary>
        public bool DeleteAll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether delete forward.
        /// </summary>
        public bool DeleteForward { get; set; }

        /// <summary>
        /// Gets or sets the block size.
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// Gets or sets the volume minimum.
        /// </summary>
        public int VolumeMinimum { get; set; }

        /// <summary>
        /// Gets or sets the volume maximum.
        /// </summary>
        public int VolumeMaximum { get; set; }

        /// <summary>
        /// Gets or sets the default milliseconds.
        /// </summary>
        public int DefaultMilliseconds { get; set; }

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
            this.StartFromVideoPosition = this.radioButtonStartFromPos.Checked;
            this.DeleteAll = this.radioButtonDeleteAll.Checked;
            this.DeleteForward = this.radioButtonForward.Checked;
            this.BlockSize = (int)this.numericUpDownBlockSize.Value;
            this.VolumeMinimum = (int)this.numericUpDownMinVol.Value;
            this.VolumeMaximum = (int)this.numericUpDownMaxVol.Value;
            this.DefaultMilliseconds = (int)this.numericUpDownDefaultMilliseconds.Value;
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
    }
}