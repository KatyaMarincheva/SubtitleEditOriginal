// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplyDurationLimits.cs" company="">
//   
// </copyright>
// <summary>
//   The apply duration limits.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The apply duration limits.
    /// </summary>
    public partial class ApplyDurationLimits : PositionAndSizeForm
    {
        /// <summary>
        /// The _refresh timer.
        /// </summary>
        private readonly Timer _refreshTimer = new Timer();

        /// <summary>
        /// The _only list fixes.
        /// </summary>
        private bool _onlyListFixes = true;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _total errors.
        /// </summary>
        private int _totalErrors;

        /// <summary>
        /// The _total fixes.
        /// </summary>
        private int _totalFixes;

        /// <summary>
        /// The _working.
        /// </summary>
        private Subtitle _working;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplyDurationLimits"/> class.
        /// </summary>
        public ApplyDurationLimits()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.ApplyDurationLimits.Title;
            this.labelMinDuration.Text = Configuration.Settings.Language.Settings.DurationMinimumMilliseconds;
            this.labelMaxDuration.Text = Configuration.Settings.Language.Settings.DurationMaximumMilliseconds;
            this.labelNote.Text = Configuration.Settings.Language.AdjustDisplayDuration.Note;
            this.numericUpDownDurationMin.Value = Configuration.Settings.General.SubtitleMinimumDisplayMilliseconds;
            this.numericUpDownDurationMax.Value = Configuration.Settings.General.SubtitleMaximumDisplayMilliseconds;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.subtitleListView1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.General.Before;
            this.listViewFixes.Columns[3].Text = Configuration.Settings.Language.General.After;
            this.FixLargeFonts();
            this._refreshTimer.Interval = 400;
            this._refreshTimer.Tick += this.RefreshTimerTick;
        }

        /// <summary>
        /// Gets the fixed subtitle.
        /// </summary>
        public Subtitle FixedSubtitle
        {
            get
            {
                return this._working;
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
        /// The fix large fonts.
        /// </summary>
        private void FixLargeFonts()
        {
            if (this.labelNote.Left + this.labelNote.Width + 5 > this.Width)
            {
                this.Width = this.labelNote.Left + this.labelNote.Width + 5;
            }

            Utilities.FixLargeFonts(this, this.buttonOK);
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
            this.GeneratePreview();
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
            this._totalFixes = 0;
            this._totalErrors = 0;
            this._onlyListFixes = true;

            this._working = new Subtitle(this._subtitle);
            this.listViewFixes.BeginUpdate();
            this.listViewFixes.Items.Clear();
            this.FixShortDisplayTimes();
            this.FixLongDisplayTimes();
            this.listViewFixes.EndUpdate();

            this.groupBoxFixesAvailable.Text = string.Format(Configuration.Settings.Language.ApplyDurationLimits.FixesAvailable, this._totalFixes);
            this.groupBoxUnfixable.Text = string.Format(Configuration.Settings.Language.ApplyDurationLimits.UnableToFix, this._totalErrors);
        }

        /// <summary>
        /// The add fix to list view.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="before">
        /// The before.
        /// </param>
        /// <param name="after">
        /// The after.
        /// </param>
        private void AddFixToListView(Paragraph p, string before, string after)
        {
            if (this._onlyListFixes)
            {
                var item = new ListViewItem(string.Empty) { Checked = true, Tag = p };
                item.SubItems.Add(p.Number.ToString());
                item.SubItems.Add(before.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
                item.SubItems.Add(after.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
                this.listViewFixes.Items.Add(item);
            }
        }

        /// <summary>
        /// The allow fix.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AllowFix(Paragraph p)
        {
            if (this._onlyListFixes)
            {
                return true;
            }

            string ln = p.Number.ToString();
            foreach (ListViewItem item in this.listViewFixes.Items)
            {
                if (item.SubItems[1].Text == ln)
                {
                    return item.Checked;
                }
            }

            return false;
        }

        /// <summary>
        /// The fix short display times.
        /// </summary>
        private void FixShortDisplayTimes()
        {
            Subtitle unfixables = new Subtitle();
            string fixAction = Configuration.Settings.Language.FixCommonErrors.FixShortDisplayTime;
            for (int i = 0; i < this._working.Paragraphs.Count; i++)
            {
                Paragraph p = this._working.Paragraphs[i];

                double minDisplayTime = (double)this.numericUpDownDurationMin.Value;

                // var minCharSecMs = Utilities.GetOptimalDisplayMilliseconds(p.Text, Configuration.Settings.General.SubtitleMaximumCharactersPerSeconds);
                // if (minCharSecMs > minDisplayTime)
                // minDisplayTime = minCharSecMs;
                double displayTime = p.Duration.TotalMilliseconds;
                if (displayTime < minDisplayTime)
                {
                    Paragraph next = this._working.GetParagraphOrDefault(i + 1);
                    if (next == null || (p.StartTime.TotalMilliseconds + minDisplayTime < next.StartTime.TotalMilliseconds) && this.AllowFix(p))
                    {
                        string before = p.StartTime.ToShortString() + " --> " + p.EndTime.ToShortString() + " - " + p.Duration.ToShortString();
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + minDisplayTime;

                        string after = p.StartTime.ToShortString() + " --> " + p.EndTime.ToShortString() + " - " + p.Duration.ToShortString();
                        this._totalFixes++;
                        this.AddFixToListView(p, before, after);
                    }
                    else
                    {
                        unfixables.Paragraphs.Add(new Paragraph(p));
                        this._totalErrors++;
                    }
                }
            }

            this.subtitleListView1.Fill(unfixables);
        }

        /// <summary>
        /// The fix long display times.
        /// </summary>
        public void FixLongDisplayTimes()
        {
            string fixAction = Configuration.Settings.Language.FixCommonErrors.FixLongDisplayTime;
            for (int i = 0; i < this._working.Paragraphs.Count; i++)
            {
                Paragraph p = this._working.Paragraphs[i];
                double displayTime = p.Duration.TotalMilliseconds;
                double maxDisplayTime = (double)this.numericUpDownDurationMax.Value;
                if (displayTime > maxDisplayTime && this.AllowFix(p))
                {
                    string before = p.StartTime.ToShortString() + " --> " + p.EndTime.ToShortString() + " - " + p.Duration.ToShortString();
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + maxDisplayTime;
                    string after = p.StartTime.ToShortString() + " --> " + p.EndTime.ToShortString() + " - " + p.Duration.ToShortString();
                    this._totalFixes++;
                    this.AddFixToListView(p, before, after);
                }
            }
        }

        /// <summary>
        /// The apply duration limits_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ApplyDurationLimits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The numeric up down duration min_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownDurationMin_ValueChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The numeric up down duration max_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownDurationMax_ValueChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
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
            this._onlyListFixes = false;
            this._working = new Subtitle(this._subtitle);
            this.FixShortDisplayTimes();
            this.FixLongDisplayTimes();
            this.DialogResult = DialogResult.OK;
        }
    }
}