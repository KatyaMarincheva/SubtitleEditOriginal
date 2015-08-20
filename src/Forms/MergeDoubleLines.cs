// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeDoubleLines.cs" company="">
//   
// </copyright>
// <summary>
//   The merge double lines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The merge double lines.
    /// </summary>
    public partial class MergeDoubleLines : PositionAndSizeForm
    {
        /// <summary>
        /// The _merged subtitle.
        /// </summary>
        private Subtitle _mergedSubtitle;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The loading.
        /// </summary>
        private bool loading = true;

        /// <summary>
        /// The preview timer.
        /// </summary>
        private Timer previewTimer = new Timer();

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeDoubleLines"/> class.
        /// </summary>
        public MergeDoubleLines()
        {
            this.InitializeComponent();
            this.previewTimer.Tick += this.previewTimer_Tick;
            this.previewTimer.Interval = 250;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the number of merges.
        /// </summary>
        public int NumberOfMerges { get; private set; }

        /// <summary>
        /// Gets the merged subtitle.
        /// </summary>
        public Subtitle MergedSubtitle
        {
            get
            {
                return this._mergedSubtitle;
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
            if (subtitle.Paragraphs.Count > 0)
            {
                subtitle.Renumber(subtitle.Paragraphs[0].Number);
            }

            this.Text = Configuration.Settings.Language.MergeDoubleLines.Title;
            this.labelMaxMillisecondsBetweenLines.Text = Configuration.Settings.Language.MergeDoubleLines.MaxMillisecondsBetweenLines;
            this.checkBoxIncludeIncrementing.Text = Configuration.Settings.Language.MergeDoubleLines.IncludeIncrementing;
            this.numericUpDownMaxMillisecondsBetweenLines.Left = this.labelMaxMillisecondsBetweenLines.Left + this.labelMaxMillisecondsBetweenLines.Width + 3;
            this.checkBoxIncludeIncrementing.Left = this.numericUpDownMaxMillisecondsBetweenLines.Left + this.numericUpDownMaxMillisecondsBetweenLines.Width + 10;

            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.MergedShortLines.MergedText;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            this.SubtitleListview1.AutoSizeAllColumns(this);
            this.NumberOfMerges = 0;
            this._subtitle = subtitle;
            this.MergeDoubleLines_ResizeEnd(null, null);
        }

        /// <summary>
        /// The add to list view.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="lineNumbers">
        /// The line numbers.
        /// </param>
        /// <param name="newText">
        /// The new text.
        /// </param>
        private void AddToListView(Paragraph p, string lineNumbers, string newText)
        {
            var item = new ListViewItem(string.Empty) { Tag = p, Checked = true };

            var subItem = new ListViewItem.ListViewSubItem(item, lineNumbers.TrimEnd(','));
            item.SubItems.Add(subItem);
            subItem = new ListViewItem.ListViewSubItem(item, newText.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            item.SubItems.Add(subItem);

            this.listViewFixes.Items.Add(item);
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            if (this._subtitle == null)
            {
                return;
            }

            var mergedIndexes = new List<int>();

            this.NumberOfMerges = 0;
            this.SubtitleListview1.Items.Clear();
            this.SubtitleListview1.BeginUpdate();
            int count;
            this._mergedSubtitle = this.MergeLineswithSameTextInSubtitle(this._subtitle, mergedIndexes, out count, true, this.checkBoxIncludeIncrementing.Checked, true, (int)this.numericUpDownMaxMillisecondsBetweenLines.Value);
            this.NumberOfMerges = count;

            this.SubtitleListview1.Fill(this._subtitle);

            foreach (var index in mergedIndexes)
            {
                this.SubtitleListview1.SetBackgroundColor(index, Color.Green);
            }

            this.SubtitleListview1.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.MergedShortLines.NumberOfMergesX, this.NumberOfMerges);
        }

        /// <summary>
        /// The is fix allowed.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsFixAllowed(Paragraph p)
        {
            foreach (ListViewItem item in this.listViewFixes.Items)
            {
                string numbers = item.SubItems[1].Text;
                foreach (string number in numbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (number == p.Number.ToString())
                    {
                        return item.Checked;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The merge lineswith same text in subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="mergedIndexes">
        /// The merged indexes.
        /// </param>
        /// <param name="numberOfMerges">
        /// The number of merges.
        /// </param>
        /// <param name="clearFixes">
        /// The clear fixes.
        /// </param>
        /// <param name="fixIncrementing">
        /// The fix incrementing.
        /// </param>
        /// <param name="lineAfterNext">
        /// The line after next.
        /// </param>
        /// <param name="maxMsBetween">
        /// The max ms between.
        /// </param>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public Subtitle MergeLineswithSameTextInSubtitle(Subtitle subtitle, List<int> mergedIndexes, out int numberOfMerges, bool clearFixes, bool fixIncrementing, bool lineAfterNext, int maxMsBetween)
        {
            List<int> removed = new List<int>();
            if (!this.loading)
            {
                this.listViewFixes.ItemChecked -= this.listViewFixes_ItemChecked;
            }

            if (clearFixes)
            {
                this.listViewFixes.Items.Clear();
            }

            numberOfMerges = 0;
            var mergedSubtitle = new Subtitle();
            bool lastMerged = false;
            Paragraph p = null;
            var lineNumbers = new StringBuilder();
            for (int i = 1; i < subtitle.Paragraphs.Count; i++)
            {
                if (!lastMerged)
                {
                    p = new Paragraph(subtitle.GetParagraphOrDefault(i - 1));
                    mergedSubtitle.Paragraphs.Add(p);
                }

                Paragraph next = subtitle.GetParagraphOrDefault(i);
                Paragraph afterNext = subtitle.GetParagraphOrDefault(i + 1);
                if (next != null)
                {
                    if ((QualifiesForMerge(p, next, maxMsBetween) || (fixIncrementing && QualifiesForMergeIncrement(p, next, maxMsBetween))) && this.IsFixAllowed(p))
                    {
                        p.Text = next.Text;
                        p.EndTime = next.EndTime;
                        if (lastMerged)
                        {
                            lineNumbers.Append(next.Number);
                            lineNumbers.Append(',');
                        }
                        else
                        {
                            lineNumbers.Append(p.Number);
                            lineNumbers.Append(',');
                            lineNumbers.Append(next.Number);
                            lineNumbers.Append(',');
                        }

                        lastMerged = true;
                        removed.Add(i);
                        numberOfMerges++;
                        if (!mergedIndexes.Contains(i))
                        {
                            mergedIndexes.Add(i);
                        }

                        if (!mergedIndexes.Contains(i - 1))
                        {
                            mergedIndexes.Add(i - 1);
                        }
                    }
                    else if (lineAfterNext && QualifiesForMerge(p, afterNext, maxMsBetween) && p.Duration.TotalMilliseconds > afterNext.Duration.TotalMilliseconds && this.IsFixAllowed(p))
                    {
                        removed.Add(i + 2);
                        numberOfMerges++;
                        if (lastMerged)
                        {
                            lineNumbers.Append(afterNext.Number);
                            lineNumbers.Append(',');
                        }
                        else
                        {
                            lineNumbers.Append(p.Number);
                            lineNumbers.Append(',');
                            lineNumbers.Append(afterNext.Number);
                            lineNumbers.Append(',');
                        }

                        lastMerged = true;
                        if (!mergedIndexes.Contains(i))
                        {
                            mergedIndexes.Add(i);
                        }

                        if (!mergedIndexes.Contains(i - 1))
                        {
                            mergedIndexes.Add(i - 1);
                        }
                    }
                    else
                    {
                        lastMerged = false;
                    }
                }
                else
                {
                    lastMerged = false;
                }

                if (!removed.Contains(i) && lineNumbers.Length > 0 && clearFixes)
                {
                    this.AddToListView(p, lineNumbers.ToString(), p.Text);
                    lineNumbers = new StringBuilder();
                }
            }

            if (lineNumbers.Length > 0 && clearFixes && p != null)
            {
                this.AddToListView(p, lineNumbers.ToString(), p.Text);
            }

            if (!lastMerged)
            {
                mergedSubtitle.Paragraphs.Add(new Paragraph(subtitle.GetParagraphOrDefault(subtitle.Paragraphs.Count - 1)));
            }

            if (!this.loading)
            {
                this.listViewFixes.ItemChecked += this.listViewFixes_ItemChecked;
            }

            mergedSubtitle.Renumber();
            return mergedSubtitle;
        }

        /// <summary>
        /// The qualifies for merge.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <param name="maxMsBetween">
        /// The max ms between.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool QualifiesForMerge(Paragraph p, Paragraph next, int maxMsBetween)
        {
            if (p == null || next == null)
            {
                return false;
            }

            if (next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds > maxMsBetween)
            {
                return false;
            }

            if (p.Text != null && next.Text != null)
            {
                string s = HtmlUtil.RemoveHtmlTags(p.Text.Trim());
                string s2 = HtmlUtil.RemoveHtmlTags(next.Text.Trim());
                return s == s2;
            }

            return false;
        }

        /// <summary>
        /// The qualifies for merge increment.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <param name="maxMsBetween">
        /// The max ms between.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool QualifiesForMergeIncrement(Paragraph p, Paragraph next, int maxMsBetween)
        {
            if (p == null || next == null)
            {
                return false;
            }

            if (next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds > maxMsBetween)
            {
                return false;
            }

            if (p.Text != null && next.Text != null)
            {
                string s = HtmlUtil.RemoveHtmlTags(p.Text.Trim());
                string s2 = HtmlUtil.RemoveHtmlTags(next.Text.Trim());
                if (!string.IsNullOrEmpty(s) && s2.Length > 0 && s2.StartsWith(s))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The button cancel click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The list view fixes_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewFixes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewFixes.SelectedIndices.Count > 0)
            {
                int index = this.listViewFixes.SelectedIndices[0];
                ListViewItem item = this.listViewFixes.Items[index];
                string[] numbers = item.SubItems[1].Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string number in numbers)
                {
                    foreach (Paragraph p in this._subtitle.Paragraphs)
                    {
                        if (p.Number.ToString() == number)
                        {
                            index = this._subtitle.GetIndex(p);
                            this.SubtitleListview1.EnsureVisible(index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The list view fixes_ item checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewFixes_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (this.loading)
            {
                return;
            }

            var mergedIndexes = new List<int>();

            this.NumberOfMerges = 0;
            this.SubtitleListview1.Items.Clear();
            this.SubtitleListview1.BeginUpdate();
            int count;
            this._mergedSubtitle = this.MergeLineswithSameTextInSubtitle(this._subtitle, mergedIndexes, out count, false, this.checkBoxIncludeIncrementing.Checked, true, (int)this.numericUpDownMaxMillisecondsBetweenLines.Value);
            this.NumberOfMerges = count;
            this.SubtitleListview1.Fill(this._subtitle);
            foreach (var index in mergedIndexes)
            {
                this.SubtitleListview1.SetBackgroundColor(index, Color.Green);
            }

            this.SubtitleListview1.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.MergedShortLines.NumberOfMergesX, this.NumberOfMerges);
        }

        /// <summary>
        /// The merge double lines_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeDoubleLines_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The merge double lines_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeDoubleLines_Shown(object sender, EventArgs e)
        {
            this.GeneratePreview();
            this.listViewFixes.Focus();
            if (this.listViewFixes.Items.Count > 0)
            {
                this.listViewFixes.Items[0].Selected = true;
            }

            this.loading = false;
            this.listViewFixes.ItemChecked += this.listViewFixes_ItemChecked;
        }

        /// <summary>
        /// The check box fix incrementing_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxFixIncrementing_CheckedChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The numeric up down max milliseconds between lines_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numericUpDownMaxMillisecondsBetweenLines_ValueChanged(object sender, EventArgs e)
        {
            this.previewTimer.Stop();
            this.previewTimer.Start();
        }

        /// <summary>
        /// The preview timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void previewTimer_Tick(object sender, EventArgs e)
        {
            this.previewTimer.Stop();
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The merge double lines_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeDoubleLines_ResizeEnd(object sender, EventArgs e)
        {
            this.columnHeaderText.Width = -2;
        }
    }
}