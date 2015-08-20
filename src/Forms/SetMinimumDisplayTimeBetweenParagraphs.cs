// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetMinimumDisplayTimeBetweenParagraphs.cs" company="">
//   
// </copyright>
// <summary>
//   The set minimum display time between paragraphs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The set minimum display time between paragraphs.
    /// </summary>
    public partial class SetMinimumDisplayTimeBetweenParagraphs : PositionAndSizeForm
    {
        /// <summary>
        /// The _fixed subtitle.
        /// </summary>
        private Subtitle _fixedSubtitle;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetMinimumDisplayTimeBetweenParagraphs"/> class.
        /// </summary>
        public SetMinimumDisplayTimeBetweenParagraphs()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.SetMinimumDisplayTimeBetweenParagraphs.Title;
            this.labelMaxMillisecondsBetweenLines.Text = Configuration.Settings.Language.SetMinimumDisplayTimeBetweenParagraphs.MinimumMillisecondsBetweenParagraphs;
            this.checkBoxShowOnlyChangedLines.Text = Configuration.Settings.Language.SetMinimumDisplayTimeBetweenParagraphs.ShowOnlyModifiedLines;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.SubtitleListview1.InitializeTimestampColumnWidths(this);
            Utilities.FixLargeFonts(this, this.buttonOK);

            this.groupBoxFrameInfo.Text = Configuration.Settings.Language.SetMinimumDisplayTimeBetweenParagraphs.FrameInfo;
            this.comboBoxFrameRate.Items.Add(23.976.ToString());
            this.comboBoxFrameRate.Items.Add(24.0.ToString());
            this.comboBoxFrameRate.Items.Add(25.0.ToString());
            this.comboBoxFrameRate.Items.Add(29.97.ToString());
            this.comboBoxFrameRate.Items.Add(30.0.ToString());
            this.comboBoxFrameRate.Items.Add(59.94.ToString());
            if (Math.Abs(Configuration.Settings.General.CurrentFrameRate - 23.976) < 0.1)
            {
                this.comboBoxFrameRate.SelectedIndex = 0;
            }
            else if (Math.Abs(Configuration.Settings.General.CurrentFrameRate - 24) < 0.1)
            {
                this.comboBoxFrameRate.SelectedIndex = 1;
            }
            else if (Math.Abs(Configuration.Settings.General.CurrentFrameRate - 25) < 0.1)
            {
                this.comboBoxFrameRate.SelectedIndex = 2;
            }
            else if (Math.Abs(Configuration.Settings.General.CurrentFrameRate - 29.97) < 0.01)
            {
                this.comboBoxFrameRate.SelectedIndex = 3;
            }
            else if (Math.Abs(Configuration.Settings.General.CurrentFrameRate - 30) < 0.1)
            {
                this.comboBoxFrameRate.SelectedIndex = 4;
            }
            else if (Math.Abs(Configuration.Settings.General.CurrentFrameRate - 59.94) < 0.1)
            {
                this.comboBoxFrameRate.SelectedIndex = 5;
            }
            else
            {
                this.comboBoxFrameRate.SelectedIndex = 3;
            }
        }

        /// <summary>
        /// Gets the fix count.
        /// </summary>
        public int FixCount { get; private set; }

        /// <summary>
        /// Gets the fixed subtitle.
        /// </summary>
        public Subtitle FixedSubtitle
        {
            get
            {
                return this._fixedSubtitle;
            }

            private set
            {
                this._fixedSubtitle = value;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public void Initialize(Subtitle subtitle)
        {
            this._subtitle = subtitle;
            this.numericUpDownMinMillisecondsBetweenLines.Value = Configuration.Settings.General.MinimumMillisecondsBetweenLines != 0 ? Configuration.Settings.General.MinimumMillisecondsBetweenLines : 1;

            // GeneratePreview();
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            List<int> fixes = new List<int>();
            if (this._subtitle == null)
            {
                return;
            }

            this.FixedSubtitle = new Subtitle(this._subtitle);
            var onlyFixedSubtitle = new Subtitle();

            double minumumMillisecondsBetweenLines = (double)this.numericUpDownMinMillisecondsBetweenLines.Value;
            for (int i = 0; i < this.FixedSubtitle.Paragraphs.Count - 1; i++)
            {
                Paragraph p = this.FixedSubtitle.GetParagraphOrDefault(i);
                Paragraph next = this.FixedSubtitle.GetParagraphOrDefault(i + 1);
                if (next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds < minumumMillisecondsBetweenLines)
                {
                    p.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - minumumMillisecondsBetweenLines;
                    fixes.Add(i);
                    onlyFixedSubtitle.Paragraphs.Add(new Paragraph(p));
                }
            }

            this.SubtitleListview1.BeginUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.SetMinimumDisplayTimeBetweenParagraphs.PreviewLinesModifiedX, fixes.Count);
            if (this.checkBoxShowOnlyChangedLines.Checked)
            {
                this.SubtitleListview1.Fill(onlyFixedSubtitle);
            }
            else
            {
                this.SubtitleListview1.Fill(this.FixedSubtitle);
                foreach (int index in fixes)
                {
                    this.SubtitleListview1.SetBackgroundColor(index, Color.Silver);
                }
            }

            this.SubtitleListview1.EndUpdate();
            this.FixCount = fixes.Count;
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
        /// The set minimal display time difference_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SetMinimalDisplayTimeDifference_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The numeric up down min milliseconds between lines_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMinMillisecondsBetweenLines_ValueChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
            Configuration.Settings.General.MinimumMillisecondsBetweenLines = (int)this.numericUpDownMinMillisecondsBetweenLines.Value;
        }

        /// <summary>
        /// The check box show only changed lines_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxShowOnlyChangedLines_CheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The numeric up down min milliseconds between lines_ key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMinMillisecondsBetweenLines_KeyUp(object sender, KeyEventArgs e)
        {
            this.numericUpDownMinMillisecondsBetweenLines.ValueChanged -= this.numericUpDownMinMillisecondsBetweenLines_ValueChanged;
            this.GeneratePreview();
            this.numericUpDownMinMillisecondsBetweenLines.ValueChanged += this.numericUpDownMinMillisecondsBetweenLines_ValueChanged;
            Configuration.Settings.General.MinimumMillisecondsBetweenLines = (int)this.numericUpDownMinMillisecondsBetweenLines.Value;
        }

        /// <summary>
        /// The combo box frame rate_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFrameRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            double frameRate;
            if (!double.TryParse(this.comboBoxFrameRate.Text.Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out frameRate))
            {
                frameRate = 25.0;
            }

            long ms = (long)Math.Round(1000 / frameRate);
            this.labelOneFrameIsXMS.Text = string.Format(Configuration.Settings.Language.SetMinimumDisplayTimeBetweenParagraphs.OneFrameXisYMilliseconds, frameRate, ms);
        }

        /// <summary>
        /// The combo box frame rate_ key up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxFrameRate_KeyUp(object sender, KeyEventArgs e)
        {
            this.comboBoxFrameRate_SelectedIndexChanged(sender, e);
        }
    }
}