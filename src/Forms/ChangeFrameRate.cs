// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeFrameRate.cs" company="">
//   
// </copyright>
// <summary>
//   The change frame rate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The change frame rate.
    /// </summary>
    public sealed partial class ChangeFrameRate : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeFrameRate"/> class.
        /// </summary>
        public ChangeFrameRate()
        {
            this.InitializeComponent();

            this.comboBoxFrameRateFrom.Items.Add(23.976);
            this.comboBoxFrameRateFrom.Items.Add(24.0);
            this.comboBoxFrameRateFrom.Items.Add(25.0);
            this.comboBoxFrameRateFrom.Items.Add(29.97);

            this.comboBoxFrameRateTo.Items.Add(23.976);
            this.comboBoxFrameRateTo.Items.Add(24.0);
            this.comboBoxFrameRateTo.Items.Add(25.0);
            this.comboBoxFrameRateTo.Items.Add(29.97);

            LanguageStructure.ChangeFrameRate language = Configuration.Settings.Language.ChangeFrameRate;
            this.Text = language.Title;
            this.labelInfo.Text = language.ConvertFrameRateOfSubtitle;
            this.labelFromFrameRate.Text = language.FromFrameRate;
            this.labelToFrameRate.Text = language.ToFrameRate;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the old frame rate.
        /// </summary>
        public double OldFrameRate
        {
            get
            {
                return double.Parse(this.comboBoxFrameRateFrom.Text);
            }
        }

        /// <summary>
        /// Gets the new frame rate.
        /// </summary>
        public double NewFrameRate
        {
            get
            {
                return double.Parse(this.comboBoxFrameRateTo.Text);
            }
        }

        /// <summary>
        /// The form change frame rate_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormChangeFrameRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="fromFrameRate">
        /// The from frame rate.
        /// </param>
        public void Initialize(string fromFrameRate)
        {
            this.comboBoxFrameRateFrom.Text = fromFrameRate;
        }

        /// <summary>
        /// The get frame rate from video file.
        /// </summary>
        /// <param name="oldFrameRate">
        /// The old frame rate.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFrameRateFromVideoFile(string oldFrameRate)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.General.OpenVideoFileTitle;
            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Filter = Utilities.GetVideoFileFilter(false);
            this.openFileDialog1.FileName = string.Empty;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VideoInfo info = Utilities.GetVideoInfo(this.openFileDialog1.FileName);
                if (info != null && info.Success)
                {
                    return info.FramesPerSecond.ToString();
                }
            }

            return oldFrameRate;
        }

        /// <summary>
        /// The button get frame rate from click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonGetFrameRateFromClick(object sender, EventArgs e)
        {
            this.comboBoxFrameRateFrom.Text = this.GetFrameRateFromVideoFile(this.comboBoxFrameRateFrom.Text);
        }

        /// <summary>
        /// The button get frame rate to click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonGetFrameRateToClick(object sender, EventArgs e)
        {
            this.comboBoxFrameRateTo.Text = this.GetFrameRateFromVideoFile(this.comboBoxFrameRateTo.Text);
        }

        /// <summary>
        /// The button ok click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOkClick(object sender, EventArgs e)
        {
            double d;
            if (double.TryParse(this.comboBoxFrameRateFrom.Text, out d) && double.TryParse(this.comboBoxFrameRateTo.Text, out d))
            {
                this.DialogResult = DialogResult.OK;
            }
            else if (this.comboBoxFrameRateFrom.Text.Trim() == this.comboBoxFrameRateTo.Text.Trim())
            {
                MessageBox.Show(Configuration.Settings.Language.ChangeFrameRate.FrameRateNotChanged);
            }
            else
            {
                MessageBox.Show(Configuration.Settings.Language.ChangeFrameRate.FrameRateNotCorrect);
            }
        }
    }
}