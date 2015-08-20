// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MergeTextWithSameTimeCodes.cs" company="">
//   
// </copyright>
// <summary>
//   The merge text with same time codes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The merge text with same time codes.
    /// </summary>
    public partial class MergeTextWithSameTimeCodes : Form
    {
        /// <summary>
        /// The _preview timer.
        /// </summary>
        private readonly Timer _previewTimer = new Timer();

        /// <summary>
        /// The _is fix allowed list.
        /// </summary>
        private Dictionary<int, bool> _isFixAllowedList = new Dictionary<int, bool>();

        /// <summary>
        /// The _language.
        /// </summary>
        private string _language;

        /// <summary>
        /// The _loading.
        /// </summary>
        private bool _loading = true;

        /// <summary>
        /// The _merged subtitle.
        /// </summary>
        private Subtitle _mergedSubtitle;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeTextWithSameTimeCodes"/> class.
        /// </summary>
        public MergeTextWithSameTimeCodes()
        {
            this.InitializeComponent();

            this._previewTimer.Tick += this.previewTimer_Tick;
            this._previewTimer.Interval = 250;

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

            this.Text = Configuration.Settings.Language.MergeTextWithSameTimeCodes.Title;
            this.labelMaxDifferenceMS.Text = Configuration.Settings.Language.MergeTextWithSameTimeCodes.MaxDifferenceMilliseconds;
            this.checkBoxAutoBreakOn.Text = Configuration.Settings.Language.MergeTextWithSameTimeCodes.ReBreakLines;
            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.MergeTextWithSameTimeCodes.MergedText;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            this.SubtitleListview1.AutoSizeAllColumns(this);
            this.NumberOfMerges = 0;
            this._subtitle = subtitle;
            this.MergeTextWithSameTimeCodes_ResizeEnd(null, null);
            this._language = Utilities.AutoDetectGoogleLanguage(subtitle);
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
            this._previewTimer.Stop();
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
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

            foreach (string number in lineNumbers.TrimEnd(',').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int key = Convert.ToInt32(number);
                if (!this._isFixAllowedList.ContainsKey(key))
                {
                    this._isFixAllowedList.Add(key, true);
                }
            }
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
            this._mergedSubtitle = this.MergeLinesWithSameTimeCodes(this._subtitle, mergedIndexes, out count, true, this.checkBoxAutoBreakOn.Checked, (int)this.numericUpDownMaxMillisecondsBetweenLines.Value);
            this.NumberOfMerges = count;

            this.SubtitleListview1.Fill(this._subtitle);
            foreach (var index in mergedIndexes)
            {
                this.SubtitleListview1.SetBackgroundColor(index, Color.Green);
            }

            this.SubtitleListview1.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.MergeTextWithSameTimeCodes.NumberOfMergesX, this.NumberOfMerges);
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
            if (this._isFixAllowedList.ContainsKey(p.Number))
            {
                return this._isFixAllowedList[p.Number];
            }

            return true;
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
            if (this._loading)
            {
                return;
            }

            foreach (string number in e.Item.SubItems[1].Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int no = Convert.ToInt32(number);
                if (this._isFixAllowedList.ContainsKey(no))
                {
                    this._isFixAllowedList[no] = e.Item.Checked;
                }
            }

            var mergedIndexes = new List<int>();

            this.NumberOfMerges = 0;
            this.Cursor = Cursors.WaitCursor;
            this.SubtitleListview1.Items.Clear();
            this.SubtitleListview1.BeginUpdate();
            int count;
            this._mergedSubtitle = this.MergeLinesWithSameTimeCodes(this._subtitle, mergedIndexes, out count, false, this.checkBoxAutoBreakOn.Checked, (int)this.numericUpDownMaxMillisecondsBetweenLines.Value);
            this.NumberOfMerges = count;
            this.SubtitleListview1.Fill(this._subtitle);
            foreach (var index in mergedIndexes)
            {
                this.SubtitleListview1.SetBackgroundColor(index, Color.Green);
            }

            this.SubtitleListview1.EndUpdate();
            this.Cursor = Cursors.Default;
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.MergeTextWithSameTimeCodes.NumberOfMergesX, this.NumberOfMerges);
        }

        /// <summary>
        /// The merge lines with same time codes.
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
        /// <param name="reBreak">
        /// The re break.
        /// </param>
        /// <param name="maxMsBetween">
        /// The max ms between.
        /// </param>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public Subtitle MergeLinesWithSameTimeCodes(Subtitle subtitle, List<int> mergedIndexes, out int numberOfMerges, bool clearFixes, bool reBreak, int maxMsBetween)
        {
            this.listViewFixes.BeginUpdate();
            var removed = new List<int>();
            if (!this._loading)
            {
                this.listViewFixes.ItemChecked -= this.listViewFixes_ItemChecked;
            }

            if (clearFixes)
            {
                this.listViewFixes.Items.Clear();
                this._isFixAllowedList = new Dictionary<int, bool>();
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
                if (next != null)
                {
                    if (QualifiesForMerge(p, next, maxMsBetween) && this.IsFixAllowed(p))
                    {
                        if (p.Text.StartsWith("<i>", StringComparison.Ordinal) && p.Text.EndsWith("</i>", StringComparison.Ordinal) && next.Text.StartsWith("<i>", StringComparison.Ordinal) && next.Text.EndsWith("</i>", StringComparison.Ordinal))
                        {
                            p.Text = p.Text.Remove(p.Text.Length - 4) + Environment.NewLine + next.Text.Remove(0, 3);
                        }
                        else
                        {
                            p.Text = p.Text + Environment.NewLine + next.Text;
                        }

                        if (reBreak)
                        {
                            p.Text = Utilities.AutoBreakLine(p.Text, this._language);
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

                        if (!("," + lineNumbers).Contains("," + p.Number + ","))
                        {
                            lineNumbers.Append(p.Number);
                            lineNumbers.Append(',');
                        }

                        if (!("," + lineNumbers).Contains("," + next.Number + ","))
                        {
                            lineNumbers.Append(next.Number);
                            lineNumbers.Append(',');
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

            this.listViewFixes.EndUpdate();
            if (!this._loading)
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

            return Math.Abs(next.StartTime.TotalMilliseconds - p.StartTime.TotalMilliseconds) <= maxMsBetween && Math.Abs(next.EndTime.TotalMilliseconds - p.EndTime.TotalMilliseconds) <= maxMsBetween;
        }

        /// <summary>
        /// The merge text with same time codes_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeTextWithSameTimeCodes_KeyDown(object sender, KeyEventArgs e)
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
        /// The merge text with same time codes_ resize end.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeTextWithSameTimeCodes_ResizeEnd(object sender, EventArgs e)
        {
            this.columnHeaderText.Width = -2;
        }

        /// <summary>
        /// The merge text with same time codes_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MergeTextWithSameTimeCodes_Shown(object sender, EventArgs e)
        {
            this.GeneratePreview();
            this.listViewFixes.Focus();
            if (this.listViewFixes.Items.Count > 0)
            {
                this.listViewFixes.Items[0].Selected = true;
            }

            this._loading = false;
            this.listViewFixes.ItemChecked += this.listViewFixes_ItemChecked;
        }

        /// <summary>
        /// The check box auto break on_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxAutoBreakOn_CheckedChanged(object sender, EventArgs e)
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
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }
    }
}