// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeSpeedInPercent.cs" company="">
//   
// </copyright>
// <summary>
//   The change speed in percent.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The change speed in percent.
    /// </summary>
    public sealed partial class ChangeSpeedInPercent : PositionAndSizeForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeSpeedInPercent"/> class.
        /// </summary>
        /// <param name="numberOfSelectedLines">
        /// The number of selected lines.
        /// </param>
        public ChangeSpeedInPercent(int numberOfSelectedLines)
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.ChangeSpeedInPercent.Title;
            this.groupBoxInfo.Text = Configuration.Settings.Language.ChangeSpeedInPercent.Info;
            this.radioButtonAllLines.Text = Configuration.Settings.Language.ShowEarlierLater.AllLines;
            this.radioButtonSelectedLinesOnly.Text = Configuration.Settings.Language.ShowEarlierLater.SelectedLinesOnly;
            this.radioButtonSpeedCustom.Text = Configuration.Settings.Language.ChangeSpeedInPercent.Custom;
            this.radioButtonSpeedFromDropFrame.Text = Configuration.Settings.Language.ChangeSpeedInPercent.FromDropFrame;
            this.radioButtonToDropFrame.Text = Configuration.Settings.Language.ChangeSpeedInPercent.ToDropFrame;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            if (string.IsNullOrEmpty(Configuration.Settings.Language.ChangeSpeedInPercent.ToDropFrame))
            {
                this.radioButtonSpeedCustom.Visible = false;
                this.radioButtonSpeedFromDropFrame.Visible = false;
                this.radioButtonToDropFrame.Visible = false;
            }

            Utilities.FixLargeFonts(this, this.buttonOK);

            if (numberOfSelectedLines > 1)
            {
                this.radioButtonSelectedLinesOnly.Checked = true;
            }
            else
            {
                this.radioButtonAllLines.Checked = true;
            }
        }

        /// <summary>
        /// Gets the adjust factor.
        /// </summary>
        public double AdjustFactor { get; private set; }

        /// <summary>
        /// Gets a value indicating whether adjust all lines.
        /// </summary>
        public bool AdjustAllLines { get; private set; }

        /// <summary>
        /// The change speed in percent_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChangeSpeedInPercent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#sync");
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The adjust all paragraphs.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public Subtitle AdjustAllParagraphs(Subtitle subtitle)
        {
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                this.AdjustParagraph(p);
            }

            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = subtitle.Paragraphs[i];
                Paragraph next = subtitle.GetParagraphOrDefault(i + 1);
                if (next != null)
                {
                    if (p.EndTime.TotalMilliseconds >= next.StartTime.TotalMilliseconds)
                    {
                        p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - 1;
                    }
                }
            }

            return subtitle;
        }

        /// <summary>
        /// The adjust paragraph.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        public void AdjustParagraph(Paragraph p)
        {
            p.StartTime.TotalMilliseconds = p.StartTime.TotalMilliseconds * this.AdjustFactor;
            p.EndTime.TotalMilliseconds = p.EndTime.TotalMilliseconds * this.AdjustFactor;
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
            this.AdjustFactor = Convert.ToDouble(this.numericUpDownPercent.Value) / 100.0;
            this.AdjustAllLines = this.radioButtonAllLines.Checked;
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
        /// The radio button speed custom_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonSpeedCustom_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownPercent.Enabled = true;
        }

        /// <summary>
        /// The radio button speed from drop frame_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonSpeedFromDropFrame_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownPercent.Value = Convert.ToDecimal(099.98887);
            this.numericUpDownPercent.Enabled = false;
        }

        /// <summary>
        /// The radio button to drop frame_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonToDropFrame_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownPercent.Value = Convert.ToDecimal(100.1001001);
            this.numericUpDownPercent.Enabled = false;
        }
    }
}