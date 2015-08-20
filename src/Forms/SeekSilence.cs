// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeekSilence.cs" company="">
//   
// </copyright>
// <summary>
//   The seek silence.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The seek silence.
    /// </summary>
    public partial class SeekSilence : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeekSilence"/> class.
        /// </summary>
        public SeekSilence()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.SeekSilence.Title;
            this.groupBoxSearchDirection.Text = Configuration.Settings.Language.SeekSilence.SearchDirection;
            this.radioButtonForward.Text = Configuration.Settings.Language.SeekSilence.Forward;
            this.radioButtonBack.Text = Configuration.Settings.Language.SeekSilence.Back;
            this.labelDuration.Text = Configuration.Settings.Language.SeekSilence.LengthInSeconds;
            this.labelVolumeBelow.Text = Configuration.Settings.Language.SeekSilence.MaxVolume;
            this.numericUpDownSeconds.Value = (decimal)Configuration.Settings.VideoControls.WaveformSeeksSilenceDurationSeconds;
            this.numericUpDownVolume.Value = Configuration.Settings.VideoControls.WaveformSeeksSilenceMaxVolume;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets or sets a value indicating whether seek forward.
        /// </summary>
        public bool SeekForward { get; set; }

        /// <summary>
        /// Gets or sets the seconds duration.
        /// </summary>
        public double SecondsDuration { get; set; }

        /// <summary>
        /// Gets or sets the volume below.
        /// </summary>
        public int VolumeBelow { get; set; }

        /// <summary>
        /// The seek silence_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SeekSilence_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
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
            this.SeekForward = this.radioButtonForward.Checked;
            this.SecondsDuration = (double)this.numericUpDownSeconds.Value;
            this.VolumeBelow = (int)this.numericUpDownVolume.Value;
            Configuration.Settings.VideoControls.WaveformSeeksSilenceDurationSeconds = this.SecondsDuration;
            Configuration.Settings.VideoControls.WaveformSeeksSilenceMaxVolume = this.VolumeBelow;
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