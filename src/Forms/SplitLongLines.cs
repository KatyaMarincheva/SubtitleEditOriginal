// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitLongLines.cs" company="">
//   
// </copyright>
// <summary>
//   The split long lines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Forms;

    /// <summary>
    /// The split long lines.
    /// </summary>
    public sealed partial class SplitLongLines : PositionAndSizeForm
    {
        /// <summary>
        /// The _splitted subtitle.
        /// </summary>
        private Subtitle _splittedSubtitle;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="SplitLongLines"/> class.
        /// </summary>
        public SplitLongLines()
        {
            this.InitializeComponent();
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the number of splits.
        /// </summary>
        public int NumberOfSplits { get; private set; }

        /// <summary>
        /// Gets the splitted subtitle.
        /// </summary>
        public Subtitle SplittedSubtitle
        {
            get
            {
                return this._splittedSubtitle;
            }
        }

        /// <summary>
        /// The split long lines_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SplitLongLines_KeyDown(object sender, KeyEventArgs e)
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

            this.Text = Configuration.Settings.Language.SplitLongLines.Title;
            this.labelSingleLineMaxLength.Text = Configuration.Settings.Language.SplitLongLines.SingleLineMaximumLength;
            this.labelLineMaxLength.Text = Configuration.Settings.Language.SplitLongLines.LineMaximumLength;
            this.labelLineContinuationBeginEnd.Text = Configuration.Settings.Language.SplitLongLines.LineContinuationBeginEndStrings;

            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.General.Text;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            this.SubtitleListview1.AutoSizeAllColumns(this);
            this.NumberOfSplits = 0;
            this.numericUpDownSingleLineMaxCharacters.Value = Configuration.Settings.General.SubtitleLineMaximumLength;
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

            var subItem = new ListViewItem.ListViewSubItem(item, lineNumbers);
            item.SubItems.Add(subItem);
            subItem = new ListViewItem.ListViewSubItem(item, newText.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            item.SubItems.Add(subItem);

            this.listViewFixes.Items.Add(item);
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
            this.GeneratePreview(false);
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        /// <param name="clearFixes">
        /// The clear fixes.
        /// </param>
        private void GeneratePreview(bool clearFixes)
        {
            if (this._subtitle == null)
            {
                return;
            }

            var splittedIndexes = new List<int>();
            var autoBreakedIndexes = new List<int>();

            this.NumberOfSplits = 0;
            this.SubtitleListview1.Items.Clear();
            this.SubtitleListview1.BeginUpdate();
            int count;
            this._splittedSubtitle = this.SplitLongLinesInSubtitle(this._subtitle, splittedIndexes, autoBreakedIndexes, out count, (int)this.numericUpDownLineMaxCharacters.Value, (int)this.numericUpDownSingleLineMaxCharacters.Value, clearFixes);
            this.NumberOfSplits = count;

            this.SubtitleListview1.Fill(this._splittedSubtitle);

            foreach (var index in splittedIndexes)
            {
                this.SubtitleListview1.SetBackgroundColor(index, Color.Green);
            }

            foreach (var index in autoBreakedIndexes)
            {
                this.SubtitleListview1.SetBackgroundColor(index, Color.LightGreen);
            }

            this.SubtitleListview1.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.SplitLongLines.NumberOfSplits, this.NumberOfSplits);
            this.UpdateLongestLinesInfo(this._splittedSubtitle);
        }

        /// <summary>
        /// The update longest lines info.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        private void UpdateLongestLinesInfo(Subtitle subtitle)
        {
            int maxLength = -1;
            int maxLengthIndex = -1;
            int singleLineMaxLength = -1;
            int singleLineMaxLengthIndex = -1;
            int i = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                string s = HtmlUtil.RemoveHtmlTags(p.Text, true);
                if (s.Length > maxLength)
                {
                    maxLength = s.Length;
                    maxLengthIndex = i;
                }

                var arr = s.SplitToLines();
                foreach (string line in arr)
                {
                    if (line.Length > singleLineMaxLengthIndex)
                    {
                        singleLineMaxLength = line.Length;
                        singleLineMaxLengthIndex = i;
                    }
                }

                i++;
            }

            this.labelMaxSingleLineLengthIs.Text = string.Format(Configuration.Settings.Language.SplitLongLines.LongestSingleLineIsXAtY, singleLineMaxLength, singleLineMaxLengthIndex + 1);
            this.labelMaxSingleLineLengthIs.Tag = singleLineMaxLengthIndex.ToString(CultureInfo.InvariantCulture);
            this.labelMaxLineLengthIs.Text = string.Format(Configuration.Settings.Language.SplitLongLines.LongestLineIsXAtY, maxLength, maxLengthIndex + 1);
            this.labelMaxLineLengthIs.Tag = maxLengthIndex.ToString(CultureInfo.InvariantCulture);
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
                if ((item.Tag as Paragraph) == p)
                {
                    return item.Checked;
                }
            }

            return true;
        }

        /// <summary>
        /// The split long lines in subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="splittedIndexes">
        /// The splitted indexes.
        /// </param>
        /// <param name="autoBreakedIndexes">
        /// The auto breaked indexes.
        /// </param>
        /// <param name="numberOfSplits">
        /// The number of splits.
        /// </param>
        /// <param name="totalLineMaxCharacters">
        /// The total line max characters.
        /// </param>
        /// <param name="singleLineMaxCharacters">
        /// The single line max characters.
        /// </param>
        /// <param name="clearFixes">
        /// The clear fixes.
        /// </param>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public Subtitle SplitLongLinesInSubtitle(Subtitle subtitle, List<int> splittedIndexes, List<int> autoBreakedIndexes, out int numberOfSplits, int totalLineMaxCharacters, int singleLineMaxCharacters, bool clearFixes)
        {
            this.listViewFixes.ItemChecked -= this.listViewFixes_ItemChecked;
            if (clearFixes)
            {
                this.listViewFixes.Items.Clear();
            }

            numberOfSplits = 0;
            string language = Utilities.AutoDetectGoogleLanguage(subtitle);
            var splittedSubtitle = new Subtitle();
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                bool added = false;
                var p = subtitle.GetParagraphOrDefault(i);
                if (p != null && p.Text != null)
                {
                    string oldText = HtmlUtil.RemoveHtmlTags(p.Text);
                    if (SplitLongLinesHelper.QualifiesForSplit(p.Text, singleLineMaxCharacters, totalLineMaxCharacters) && this.IsFixAllowed(p))
                    {
                        bool isDialog = false;
                        string dialogText = string.Empty;
                        if (p.Text.Contains('-'))
                        {
                            dialogText = Utilities.AutoBreakLine(p.Text, 5, 1, language);

                            var tempText = p.Text.Replace(Environment.NewLine, " ").Replace("  ", " ");
                            if (Utilities.CountTagInText(tempText, '-') == 2 && (p.Text.StartsWith('-') || p.Text.StartsWith("<i>-")))
                            {
                                int idx = tempText.IndexOf(". -", StringComparison.Ordinal);
                                if (idx < 1)
                                {
                                    idx = tempText.IndexOf("! -", StringComparison.Ordinal);
                                }

                                if (idx < 1)
                                {
                                    idx = tempText.IndexOf("? -", StringComparison.Ordinal);
                                }

                                if (idx > 1)
                                {
                                    dialogText = tempText.Remove(idx + 1, 1).Insert(idx + 1, Environment.NewLine);
                                }
                            }

                            var arr = dialogText.SplitToLines();
                            if (arr.Length == 2 && (arr[0].StartsWith('-') || arr[0].StartsWith("<i>-")) && (arr[1].StartsWith('-') || arr[1].StartsWith("<i>-")))
                            {
                                isDialog = true;
                            }
                        }

                        if (!isDialog && !SplitLongLinesHelper.QualifiesForSplit(Utilities.AutoBreakLine(p.Text, language), singleLineMaxCharacters, totalLineMaxCharacters))
                        {
                            var newParagraph = new Paragraph(p) { Text = Utilities.AutoBreakLine(p.Text, language) };
                            if (clearFixes)
                            {
                                this.AddToListView(p, (splittedSubtitle.Paragraphs.Count + 1).ToString(CultureInfo.InvariantCulture), oldText);
                            }

                            autoBreakedIndexes.Add(splittedSubtitle.Paragraphs.Count);
                            splittedSubtitle.Paragraphs.Add(newParagraph);
                            added = true;
                            numberOfSplits++;
                        }
                        else
                        {
                            string text = Utilities.AutoBreakLine(p.Text, language);
                            if (isDialog)
                            {
                                text = dialogText;
                            }

                            if (text.Contains(Environment.NewLine))
                            {
                                var arr = text.SplitToLines();
                                if (arr.Length == 2)
                                {
                                    int spacing1 = Configuration.Settings.General.MinimumMillisecondsBetweenLines / 2;
                                    int spacing2 = Configuration.Settings.General.MinimumMillisecondsBetweenLines / 2;
                                    if (Configuration.Settings.General.MinimumMillisecondsBetweenLines % 2 == 1)
                                    {
                                        spacing2++;
                                    }

                                    var newParagraph1 = new Paragraph(p);
                                    var newParagraph2 = new Paragraph(p);
                                    newParagraph1.Text = Utilities.AutoBreakLine(arr[0], language);

                                    double middle = p.StartTime.TotalMilliseconds + (p.Duration.TotalMilliseconds / 2);
                                    if (!string.IsNullOrWhiteSpace(HtmlUtil.RemoveHtmlTags(oldText)))
                                    {
                                        var startFactor = (double)HtmlUtil.RemoveHtmlTags(newParagraph1.Text).Length / HtmlUtil.RemoveHtmlTags(oldText).Length;
                                        if (startFactor < 0.25)
                                        {
                                            startFactor = 0.25;
                                        }

                                        if (startFactor > 0.75)
                                        {
                                            startFactor = 0.75;
                                        }

                                        middle = p.StartTime.TotalMilliseconds + (p.Duration.TotalMilliseconds * startFactor);
                                    }

                                    newParagraph1.EndTime.TotalMilliseconds = middle - spacing1;
                                    newParagraph2.Text = Utilities.AutoBreakLine(arr[1], language);
                                    newParagraph2.StartTime.TotalMilliseconds = newParagraph1.EndTime.TotalMilliseconds + spacing2;

                                    if (clearFixes)
                                    {
                                        this.AddToListView(p, (splittedSubtitle.Paragraphs.Count + 1).ToString(CultureInfo.InvariantCulture), oldText);
                                    }

                                    splittedIndexes.Add(splittedSubtitle.Paragraphs.Count);
                                    splittedIndexes.Add(splittedSubtitle.Paragraphs.Count + 1);

                                    string p1 = HtmlUtil.RemoveHtmlTags(newParagraph1.Text).TrimEnd();
                                    if (p1.EndsWith('.') || p1.EndsWith('!') || p1.EndsWith('?') || p1.EndsWith(':') || p1.EndsWith(')') || p1.EndsWith(']') || p1.EndsWith('♪'))
                                    {
                                        if (newParagraph1.Text.StartsWith('-') && newParagraph2.Text.StartsWith('-'))
                                        {
                                            newParagraph1.Text = newParagraph1.Text.Remove(0, 1).Trim();
                                            newParagraph2.Text = newParagraph2.Text.Remove(0, 1).Trim();
                                        }
                                        else if (newParagraph1.Text.StartsWith("<i>-") && newParagraph2.Text.StartsWith('-'))
                                        {
                                            newParagraph1.Text = newParagraph1.Text.Remove(3, 1).Trim();
                                            if (newParagraph1.Text.StartsWith("<i> "))
                                            {
                                                newParagraph1.Text = newParagraph1.Text.Remove(3, 1).Trim();
                                            }

                                            newParagraph2.Text = newParagraph2.Text.Remove(0, 1).Trim();
                                        }
                                    }
                                    else
                                    {
                                        bool endsWithComma = newParagraph1.Text.EndsWith(',') || newParagraph1.Text.EndsWith(",</i>");

                                        string post = string.Empty;
                                        if (newParagraph1.Text.EndsWith("</i>"))
                                        {
                                            post = "</i>";
                                            newParagraph1.Text = newParagraph1.Text.Remove(newParagraph1.Text.Length - post.Length);
                                        }

                                        if (endsWithComma)
                                        {
                                            newParagraph1.Text += post;
                                        }
                                        else
                                        {
                                            newParagraph1.Text += this.comboBoxLineContinuationEnd.Text.TrimEnd() + post;
                                        }

                                        string pre = string.Empty;
                                        if (newParagraph2.Text.StartsWith("<i>"))
                                        {
                                            pre = "<i>";
                                            newParagraph2.Text = newParagraph2.Text.Remove(0, pre.Length);
                                        }

                                        if (endsWithComma)
                                        {
                                            newParagraph2.Text = pre + newParagraph2.Text;
                                        }
                                        else
                                        {
                                            newParagraph2.Text = pre + this.comboBoxLineContinuationBegin.Text + newParagraph2.Text;
                                        }
                                    }

                                    var italicStart1 = newParagraph1.Text.IndexOf("<i>", StringComparison.Ordinal);
                                    if (italicStart1 >= 0 && italicStart1 < 10 && newParagraph1.Text.IndexOf("</i>", StringComparison.Ordinal) < 0 && newParagraph2.Text.Contains("</i>") && newParagraph2.Text.IndexOf("<i>", StringComparison.Ordinal) < 0)
                                    {
                                        newParagraph1.Text += "</i>";
                                        newParagraph2.Text = "<i>" + newParagraph2.Text;
                                    }

                                    splittedSubtitle.Paragraphs.Add(newParagraph1);
                                    splittedSubtitle.Paragraphs.Add(newParagraph2);
                                    added = true;
                                    numberOfSplits++;
                                }
                            }
                        }
                    }

                    if (!added)
                    {
                        splittedSubtitle.Paragraphs.Add(new Paragraph(p));
                    }
                }
            }

            this.listViewFixes.ItemChecked += this.listViewFixes_ItemChecked;
            splittedSubtitle.Renumber();
            return splittedSubtitle;
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
            this.GeneratePreview(true);
            this.Cursor = Cursors.Default;
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
                index = int.Parse(item.SubItems[1].Text) - 1;
                this.SubtitleListview1.SelectIndexAndEnsureVisible(index);
            }
        }

        /// <summary>
        /// The split long lines_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SplitLongLines_Shown(object sender, EventArgs e)
        {
            this.GeneratePreview(true);
            this.listViewFixes.Focus();
            if (this.listViewFixes.Items.Count > 0)
            {
                this.listViewFixes.Items[0].Selected = true;
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
        /// The label max single line length is_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void labelMaxSingleLineLengthIs_Click(object sender, EventArgs e)
        {
            int index = int.Parse(this.labelMaxSingleLineLengthIs.Tag.ToString());
            this.SubtitleListview1.SelectIndexAndEnsureVisible(index);
        }

        /// <summary>
        /// The label max line length is_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void labelMaxLineLengthIs_Click(object sender, EventArgs e)
        {
            int index = int.Parse(this.labelMaxLineLengthIs.Tag.ToString());
            this.SubtitleListview1.SelectIndexAndEnsureVisible(index);
        }

        /// <summary>
        /// The continuation begin end changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ContinuationBeginEndChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            int index = -1;
            if (this.listViewFixes.SelectedItems.Count > 0)
            {
                index = this.listViewFixes.SelectedItems[0].Index;
            }

            this.GeneratePreview(true);
            if (index >= 0)
            {
                this.listViewFixes.Items[index].Selected = true;
            }

            this.Cursor = Cursors.Default;
        }
    }
}