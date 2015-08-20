// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DurationsBridgeGaps.cs" company="">
//   
// </copyright>
// <summary>
//   The durations bridge gaps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The durations bridge gaps.
    /// </summary>
    public partial class DurationsBridgeGaps : PositionAndSizeForm
    {
        /// <summary>
        /// The _refresh timer.
        /// </summary>
        private readonly Timer _refreshTimer = new Timer();

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private readonly Subtitle _subtitle;

        /// <summary>
        /// The _dic.
        /// </summary>
        private Dictionary<string, string> _dic;

        /// <summary>
        /// The _fixed subtitle.
        /// </summary>
        private Subtitle _fixedSubtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationsBridgeGaps"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public DurationsBridgeGaps(Subtitle subtitle)
        {
            this.InitializeComponent();
            Utilities.FixLargeFonts(this, this.buttonOK);

            this.Text = Configuration.Settings.Language.DurationsBridgeGaps.Title;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            this.SubtitleListview1.ShowExtraColumn(Configuration.Settings.Language.DurationsBridgeGaps.GapToNext);
            this.SubtitleListview1.DisplayExtraFromExtra = true;
            this.SubtitleListview1.AutoSizeAllColumns(this);

            this.labelBridgePart1.Text = Configuration.Settings.Language.DurationsBridgeGaps.BridgeGapsSmallerThanXPart1;
            this.numericUpDownMaxMs.Left = this.labelBridgePart1.Left + this.labelBridgePart1.Width + 4;
            this.labelMilliseconds.Text = Configuration.Settings.Language.DurationsBridgeGaps.BridgeGapsSmallerThanXPart2;
            this.labelMilliseconds.Left = this.numericUpDownMaxMs.Left + this.numericUpDownMaxMs.Width + 4;
            this.labelMinMsBetweenLines.Text = Configuration.Settings.Language.DurationsBridgeGaps.MinMillisecondsBetweenLines;
            this.numericUpDownMinMsBetweenLines.Left = this.labelMinMsBetweenLines.Left + this.labelMinMsBetweenLines.Width + 4;
            this.radioButtonProlongEndTime.Text = Configuration.Settings.Language.DurationsBridgeGaps.ProlongEndTime;
            this.radioButtonDivideEven.Text = Configuration.Settings.Language.DurationsBridgeGaps.DivideEven;

            this._subtitle = subtitle;
            try
            {
                this.numericUpDownMaxMs.Value = Configuration.Settings.Tools.BridgeGapMilliseconds;
            }
            catch
            {
                this.numericUpDownMaxMs.Value = 100;
            }

            if (Configuration.Settings.General.MinimumMillisecondsBetweenLines >= 1 && Configuration.Settings.General.MinimumMillisecondsBetweenLines <= this.numericUpDownMinMsBetweenLines.Maximum)
            {
                this.numericUpDownMinMsBetweenLines.Value = Configuration.Settings.General.MinimumMillisecondsBetweenLines;
            }

            this._refreshTimer.Interval = 400;
            this._refreshTimer.Tick += this.RefreshTimerTick;
            this.GeneratePreview();
        }

        /// <summary>
        /// Gets the fixed subtitle.
        /// </summary>
        public Subtitle FixedSubtitle
        {
            get
            {
                return this._fixedSubtitle;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public override sealed string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
            }
        }

        /// <summary>
        /// The refresh timer tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RefreshTimerTick(object sender, EventArgs e)
        {
            this._refreshTimer.Stop();
            this.GeneratePreviewReal();
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
            Configuration.Settings.Tools.BridgeGapMilliseconds = (int)this.numericUpDownMaxMs.Value;
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
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            if (this._refreshTimer.Enabled)
            {
                this._refreshTimer.Stop();
                this._refreshTimer.Start();
            }
            else
            {
                this._refreshTimer.Start();
            }
        }

        /// <summary>
        /// The generate preview real.
        /// </summary>
        private void GeneratePreviewReal()
        {
            if (this._subtitle == null)
            {
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            this.SubtitleListview1.Items.Clear();
            this.SubtitleListview1.BeginUpdate();
            int count = 0;
            this._fixedSubtitle = new Subtitle(this._subtitle);
            this._dic = new Dictionary<string, string>();
            var fixedIndexes = new List<int>(this._fixedSubtitle.Paragraphs.Count);

            var minMsBetweenLines = (int)this.numericUpDownMinMsBetweenLines.Value;
            for (int i = 0; i < this._fixedSubtitle.Paragraphs.Count - 1; i++)
            {
                Paragraph cur = this._fixedSubtitle.Paragraphs[i];
                Paragraph next = this._fixedSubtitle.Paragraphs[i + 1];
                string before = null;
                var difMs = Math.Abs(cur.EndTime.TotalMilliseconds - next.StartTime.TotalMilliseconds);
                if (difMs < (double)this.numericUpDownMaxMs.Value && difMs > minMsBetweenLines && this.numericUpDownMaxMs.Value > minMsBetweenLines)
                {
                    before = string.Format("{0:0.000}", (next.StartTime.TotalMilliseconds - cur.EndTime.TotalMilliseconds) / TimeCode.BaseUnit);
                    if (this.radioButtonDivideEven.Checked && next.StartTime.TotalMilliseconds > cur.EndTime.TotalMilliseconds)
                    {
                        double half = (next.StartTime.TotalMilliseconds - cur.EndTime.TotalMilliseconds) / 2.0;
                        next.StartTime.TotalMilliseconds -= half;
                    }

                    cur.EndTime.TotalMilliseconds = next.StartTime.TotalMilliseconds - minMsBetweenLines;
                    fixedIndexes.Add(i);
                    fixedIndexes.Add(i + 1);
                    count++;
                }

                var msToNext = next.StartTime.TotalMilliseconds - cur.EndTime.TotalMilliseconds;
                if (msToNext < 2000)
                {
                    string info;
                    if (!string.IsNullOrEmpty(before))
                    {
                        info = string.Format("{0} => {1:0.000}", before, msToNext / TimeCode.BaseUnit);
                    }
                    else
                    {
                        info = string.Format("{0:0.000}", msToNext / TimeCode.BaseUnit);
                    }

                    this._dic.Add(cur.ID, info);
                }
            }

            this.SubtitleListview1.Fill(this._fixedSubtitle);
            for (int i = 0; i < this._fixedSubtitle.Paragraphs.Count - 1; i++)
            {
                Paragraph cur = this._fixedSubtitle.Paragraphs[i];
                if (this._dic != null && this._dic.ContainsKey(cur.ID))
                {
                    this.SubtitleListview1.SetExtraText(i, this._dic[cur.ID], this.SubtitleListview1.ForeColor);
                }

                this.SubtitleListview1.SetBackgroundColor(i, this.SubtitleListview1.BackColor);
            }

            foreach (var index in fixedIndexes)
            {
                this.SubtitleListview1.SetBackgroundColor(index, Color.Green);
            }

            this.SubtitleListview1.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.DurationsBridgeGaps.GapsBridgedX, count);

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The numeric up down max ms_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMaxMs_ValueChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The numeric up down min ms between lines_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMinMsBetweenLines_ValueChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The durations bridge gaps_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DurationsBridgeGaps_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}