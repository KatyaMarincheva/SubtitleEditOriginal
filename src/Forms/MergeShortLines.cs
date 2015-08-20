// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeShortLines.cs" company="">
//   
// </copyright>
// <summary>
//   The merge short lines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The merge short lines.
    /// </summary>
    public partial class MergeShortLines : PositionAndSizeForm
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
        /// Initializes a new instance of the <see cref="MergeShortLines"/> class.
        /// </summary>
        public MergeShortLines()
        {
            this.InitializeComponent();
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
        /// The merge short lines_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeShortLines_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
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

            this.Text = Configuration.Settings.Language.MergedShortLines.Title;
            this.labelMaxCharacters.Text = Configuration.Settings.Language.MergedShortLines.MaximumCharacters;
            this.labelMaxMillisecondsBetweenLines.Text = Configuration.Settings.Language.MergedShortLines.MaximumMillisecondsBetween;

            this.checkBoxOnlyContinuationLines.Text = Configuration.Settings.Language.MergedShortLines.OnlyMergeContinuationLines;

            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.MergedShortLines.MergedText;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            this.SubtitleListview1.AutoSizeAllColumns(this);
            this.NumberOfMerges = 0;
            this.numericUpDownMaxCharacters.Value = Configuration.Settings.General.SubtitleLineMaximumLength;
            this._subtitle = subtitle;
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
            this._mergedSubtitle = this.MergeShortLinesInSubtitle(this._subtitle, mergedIndexes, out count, (double)this.numericUpDownMaxMillisecondsBetweenLines.Value, (int)this.numericUpDownMaxCharacters.Value, true);
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
                    if (number == p.Number.ToString(CultureInfo.InvariantCulture))
                    {
                        return item.Checked;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// The merge short lines in subtitle.
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
        /// <param name="maxMillisecondsBetweenLines">
        /// The max milliseconds between lines.
        /// </param>
        /// <param name="maxCharacters">
        /// The max characters.
        /// </param>
        /// <param name="clearFixes">
        /// The clear fixes.
        /// </param>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public Subtitle MergeShortLinesInSubtitle(Subtitle subtitle, List<int> mergedIndexes, out int numberOfMerges, double maxMillisecondsBetweenLines, int maxCharacters, bool clearFixes)
        {
            this.listViewFixes.ItemChecked -= this.listViewFixes_ItemChecked;
            if (clearFixes)
            {
                this.listViewFixes.Items.Clear();
            }

            numberOfMerges = 0;
            string language = Utilities.AutoDetectGoogleLanguage(subtitle);
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
                if (next != null)
                {
                    if (this.QualifiesForMerge(p, next, maxMillisecondsBetweenLines, maxCharacters) && this.IsFixAllowed(p))
                    {
                        if (GetStartTag(p.Text) == GetStartTag(next.Text) && GetEndTag(p.Text) == GetEndTag(next.Text))
                        {
                            string s1 = p.Text.Trim();
                            s1 = s1.Substring(0, s1.Length - GetEndTag(s1).Length);
                            string s2 = next.Text.Trim();
                            s2 = s2.Substring(GetStartTag(s2).Length);
                            p.Text = Utilities.AutoBreakLine(s1 + Environment.NewLine + s2, language);
                        }
                        else
                        {
                            p.Text = Utilities.AutoBreakLine(p.Text + Environment.NewLine + next.Text, language);
                        }

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
                    else
                    {
                        lastMerged = false;
                    }
                }
                else
                {
                    lastMerged = false;
                }

                if (!lastMerged && lineNumbers.Length > 0 && clearFixes)
                {
                    this.AddToListView(p, lineNumbers.ToString(), p.Text);
                    lineNumbers = new StringBuilder();
                }
            }

            if (!lastMerged)
            {
                mergedSubtitle.Paragraphs.Add(new Paragraph(subtitle.GetParagraphOrDefault(subtitle.Paragraphs.Count - 1)));
            }

            this.listViewFixes.ItemChecked += this.listViewFixes_ItemChecked;
            return mergedSubtitle;
        }

        /// <summary>
        /// The get end tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetEndTag(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = text.Trim();
            if (!text.EndsWith('>'))
            {
                return string.Empty;
            }

            string endTag = string.Empty;
            int start = text.LastIndexOf("</", StringComparison.Ordinal);
            if (start > 0 && start >= text.Length - 8)
            {
                endTag = text.Substring(start);
            }

            return endTag;
        }

        /// <summary>
        /// The get start tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetStartTag(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            text = text.Trim();
            if (!text.StartsWith('<'))
            {
                return string.Empty;
            }

            string startTag = string.Empty;
            int end = text.IndexOf('>');
            if (end > 0 && end < 25)
            {
                startTag = text.Substring(0, end + 1);
            }

            return startTag;
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
        /// <param name="maximumMillisecondsBetweenLines">
        /// The maximum milliseconds between lines.
        /// </param>
        /// <param name="maximumTotalLength">
        /// The maximum total length.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool QualifiesForMerge(Paragraph p, Paragraph next, double maximumMillisecondsBetweenLines, int maximumTotalLength)
        {
            if (p != null && p.Text != null && next != null && next.Text != null)
            {
                string s = HtmlUtil.RemoveHtmlTags(p.Text.Trim());

                if (p.Text.Length + next.Text.Length < maximumTotalLength && next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds < maximumMillisecondsBetweenLines)
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        return true;
                    }

                    bool isLineContinuation = s.EndsWith(',') || s.EndsWith('-') || s.EndsWith("...", StringComparison.Ordinal) || Utilities.AllLettersAndNumbers.Contains(s.Substring(s.Length - 1));

                    if (!this.checkBoxOnlyContinuationLines.Checked)
                    {
                        return true;
                    }

                    if (isLineContinuation)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The numeric up down max characters value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NumericUpDownMaxCharactersValueChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The numeric up down max milliseconds between lines value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NumericUpDownMaxMillisecondsBetweenLinesValueChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
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
                        if (p.Number.ToString(CultureInfo.InvariantCulture) == number)
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
            var mergedIndexes = new List<int>();

            this.NumberOfMerges = 0;
            this.SubtitleListview1.Items.Clear();
            this.SubtitleListview1.BeginUpdate();
            int count;
            this._mergedSubtitle = this.MergeShortLinesInSubtitle(this._subtitle, mergedIndexes, out count, (double)this.numericUpDownMaxMillisecondsBetweenLines.Value, (int)this.numericUpDownMaxCharacters.Value, false);
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
        /// The merge short lines_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeShortLines_Shown(object sender, EventArgs e)
        {
            this.GeneratePreview();
            this.listViewFixes.Focus();
            if (this.listViewFixes.Items.Count > 0)
            {
                this.listViewFixes.Items[0].Selected = true;
            }
        }

        /// <summary>
        /// The check box only continuation lines_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxOnlyContinuationLines_CheckedChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }
    }
}