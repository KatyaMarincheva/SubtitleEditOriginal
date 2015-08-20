// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportText.cs" company="">
//   
// </copyright>
// <summary>
//   The import text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The import text.
    /// </summary>
    public sealed partial class ImportText : Form
    {
        /// <summary>
        /// The _refresh timer.
        /// </summary>
        private readonly Timer _refreshTimer = new Timer();

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _video file name.
        /// </summary>
        private string _videoFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportText"/> class.
        /// </summary>
        public ImportText()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.ImportText.Title;
            this.groupBoxImportText.Text = Configuration.Settings.Language.ImportText.Title;
            this.buttonOpenText.Text = Configuration.Settings.Language.ImportText.OpenTextFile;
            this.groupBoxImportOptions.Text = Configuration.Settings.Language.ImportText.ImportOptions;
            this.groupBoxSplitting.Text = Configuration.Settings.Language.ImportText.Splitting;
            this.radioButtonAutoSplit.Text = Configuration.Settings.Language.ImportText.AutoSplitText;
            this.radioButtonLineMode.Text = Configuration.Settings.Language.ImportText.OneLineIsOneSubtitle;
            this.labelLineBreak.Left = this.radioButtonLineMode.Left + this.radioButtonLineMode.Width + 10;
            this.labelLineBreak.Text = Configuration.Settings.Language.ImportText.LineBreak;
            this.columnHeaderFName.Text = Configuration.Settings.Language.JoinSubtitles.FileName;
            this.columnHeaderSize.Text = Configuration.Settings.Language.General.Size;
            this.comboBoxLineBreak.Left = this.labelLineBreak.Left + this.labelLineBreak.Width + 3;
            this.comboBoxLineBreak.Width = this.groupBoxSplitting.Width - this.comboBoxLineBreak.Left - 5;
            this.checkBoxMultipleFiles.AutoSize = true;
            this.checkBoxMultipleFiles.Left = this.buttonOpenText.Left - this.checkBoxMultipleFiles.Width - 9;
            this.checkBoxMultipleFiles.Text = Configuration.Settings.Language.ImportText.OneSubtitleIsOneFile;
            this.listViewInputFiles.Visible = false;

            this.radioButtonSplitAtBlankLines.Text = Configuration.Settings.Language.ImportText.SplitAtBlankLines;
            this.checkBoxMergeShortLines.Text = Configuration.Settings.Language.ImportText.MergeShortLines;
            this.checkBoxRemoveEmptyLines.Text = Configuration.Settings.Language.ImportText.RemoveEmptyLines;
            this.checkBoxRemoveLinesWithoutLetters.Text = Configuration.Settings.Language.ImportText.RemoveLinesWithoutLetters;
            this.checkBoxGenerateTimeCodes.Text = Configuration.Settings.Language.ImportText.GenerateTimeCodes;
            this.checkBoxAutoBreak.Text = Configuration.Settings.Language.Settings.MainTextBoxAutoBreak;
            this.labelGapBetweenSubtitles.Text = Configuration.Settings.Language.ImportText.GapBetweenSubtitles;
            this.numericUpDownGapBetweenLines.Left = this.labelGapBetweenSubtitles.Left + this.labelGapBetweenSubtitles.Width + 3;
            this.groupBoxDuration.Text = Configuration.Settings.Language.General.Duration;
            this.radioButtonDurationAuto.Text = Configuration.Settings.Language.ImportText.Auto;
            this.radioButtonDurationFixed.Text = Configuration.Settings.Language.ImportText.Fixed;
            this.buttonRefresh.Text = Configuration.Settings.Language.ImportText.Refresh;
            this.groupBoxTimeCodes.Text = Configuration.Settings.Language.ImportText.TimeCodes;
            this.groupBoxImportResult.Text = Configuration.Settings.Language.General.Preview;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.SubtitleListview1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.SubtitleListview1);
            this.SubtitleListview1.AutoSizeAllColumns(this);

            if (string.IsNullOrEmpty(Configuration.Settings.Tools.ImportTextSplitting))
            {
                this.radioButtonAutoSplit.Checked = true;
            }
            else if (Configuration.Settings.Tools.ImportTextSplitting.Equals("blank lines", StringComparison.OrdinalIgnoreCase))
            {
                this.radioButtonSplitAtBlankLines.Checked = true;
            }
            else if (Configuration.Settings.Tools.ImportTextSplitting.Equals("line", StringComparison.OrdinalIgnoreCase))
            {
                this.radioButtonLineMode.Checked = true;
            }

            this.checkBoxMergeShortLines.Checked = Configuration.Settings.Tools.ImportTextMergeShortLines;
            this.comboBoxLineBreak.Text = Configuration.Settings.Tools.ImportTextLineBreak;

            this.numericUpDownDurationFixed.Enabled = this.radioButtonDurationFixed.Checked;
            Utilities.FixLargeFonts(this, this.buttonOK);
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
                return this._subtitle;
            }
        }

        /// <summary>
        /// Gets the video file name.
        /// </summary>
        public string VideoFileName
        {
            get
            {
                return this._videoFileName;
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
        /// The button open text click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOpenTextClick(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = this.buttonOpenText.Text;
            if (this.checkBoxMultipleFiles.Visible && this.checkBoxMultipleFiles.Checked)
            {
                this.openFileDialog1.Filter = Configuration.Settings.Language.ImportText.TextFiles + "|*.txt|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
            }
            else
            {
                this.openFileDialog1.Filter = Configuration.Settings.Language.ImportText.TextFiles + "|*.txt|Adobe Story|*.astx|" + Configuration.Settings.Language.General.AllFiles + "|*.*";
            }

            this.openFileDialog1.FileName = string.Empty;
            this.openFileDialog1.Multiselect = this.checkBoxMultipleFiles.Visible && this.checkBoxMultipleFiles.Checked;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (this.checkBoxMultipleFiles.Visible && this.checkBoxMultipleFiles.Checked)
                {
                    foreach (string fileName in this.openFileDialog1.FileNames)
                    {
                        this.AddInputFile(fileName);
                    }
                }
                else
                {
                    string ext = Path.GetExtension(this.openFileDialog1.FileName).ToLower();
                    if (ext == ".astx")
                    {
                        this.LoadAdobeStory(this.openFileDialog1.FileName);
                    }
                    else
                    {
                        this.LoadTextFile(this.openFileDialog1.FileName);
                    }
                }

                this.GeneratePreview();
            }
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            if (this.radioButtonSplitAtBlankLines.Checked || this.radioButtonLineMode.Checked)
            {
                this.checkBoxAutoBreak.Enabled = true;
            }
            else
            {
                this.checkBoxAutoBreak.Enabled = false;
            }

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
            this._subtitle = new Subtitle();
            if (this.checkBoxMultipleFiles.Visible && this.checkBoxMultipleFiles.Checked)
            {
                this.ImportMultipleFiles(this.listViewInputFiles.Items);
            }
            else if (this.radioButtonLineMode.Checked)
            {
                this.ImportLineMode(this.textBoxText.Lines);
            }
            else if (this.radioButtonAutoSplit.Checked)
            {
                this.ImportAutoSplit(this.textBoxText.Lines);
            }
            else
            {
                this.ImportSplitAtBlankLine(this.textBoxText.Lines);
            }

            if (this.checkBoxMergeShortLines.Checked)
            {
                this.MergeLinesWithContinuation();
            }

            this._subtitle.Renumber();
            if (this.checkBoxGenerateTimeCodes.Checked)
            {
                this.FixDurations();
                this.MakePseudoStartTime();
            }
            else
            {
                foreach (Paragraph p in this._subtitle.Paragraphs)
                {
                    p.StartTime.TotalMilliseconds = TimeCode.MaxTime.TotalMilliseconds;
                    p.EndTime.TotalMilliseconds = TimeCode.MaxTime.TotalMilliseconds;
                }
            }

            this.groupBoxImportResult.Text = string.Format(Configuration.Settings.Language.ImportText.PreviewLinesModifiedX, this._subtitle.Paragraphs.Count);
            this.SubtitleListview1.Fill(this._subtitle);
            this.SubtitleListview1.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The import multiple files.
        /// </summary>
        /// <param name="listViewItemCollection">
        /// The list view item collection.
        /// </param>
        private void ImportMultipleFiles(ListView.ListViewItemCollection listViewItemCollection)
        {
            foreach (ListViewItem item in listViewItemCollection)
            {
                string line;
                try
                {
                    line = File.ReadAllText(item.Text).Trim();
                }
                catch
                {
                    line = string.Empty;
                }

                line = line.Replace("|", Environment.NewLine);
                if (this.comboBoxLineBreak.Text.Length > 0)
                {
                    foreach (string splitter in this.comboBoxLineBreak.Text.Split(';'))
                    {
                        var tempSplitter = splitter.Trim();
                        if (tempSplitter.Length > 0)
                        {
                            line = line.Replace(tempSplitter, Environment.NewLine);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (!this.checkBoxRemoveEmptyLines.Checked)
                    {
                        this._subtitle.Paragraphs.Add(new Paragraph());
                    }
                }
                else if (!ContainsLetters(line))
                {
                    if (!this.checkBoxRemoveLinesWithoutLetters.Checked)
                    {
                        this._subtitle.Paragraphs.Add(new Paragraph(0, 0, line.Trim()));
                    }
                }
                else
                {
                    this._subtitle.Paragraphs.Add(new Paragraph(0, 0, line.Trim()));
                }
            }
        }

        /// <summary>
        /// The merge lines with continuation.
        /// </summary>
        private void MergeLinesWithContinuation()
        {
            var temp = new Subtitle();
            bool skipNext = false;
            for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
            {
                Paragraph p = this._subtitle.Paragraphs[i];
                if (!skipNext)
                {
                    Paragraph next = this._subtitle.GetParagraphOrDefault(i + 1);

                    bool merge = !(p.Text.Contains(Environment.NewLine) || next == null);

                    if (merge && (p.Text.TrimEnd().EndsWith('!') || p.Text.TrimEnd().EndsWith('.')))
                    {
                        var st = new StripableText(p.Text);
                        if (st.StrippedText.Length > 0 && Utilities.UppercaseLetters.Contains(st.StrippedText[0].ToString(CultureInfo.InvariantCulture)))
                        {
                            merge = false;
                        }
                    }

                    if (merge && (p.Text.Length >= Configuration.Settings.General.SubtitleLineMaximumLength - 5 || next.Text.Length >= Configuration.Settings.General.SubtitleLineMaximumLength - 5))
                    {
                        merge = false;
                    }

                    if (merge)
                    {
                        temp.Paragraphs.Add(new Paragraph { Text = p.Text + Environment.NewLine + next.Text });
                        skipNext = true;
                    }
                    else
                    {
                        temp.Paragraphs.Add(new Paragraph(p));
                    }
                }
                else
                {
                    skipNext = false;
                }
            }

            this._subtitle = temp;
        }

        /// <summary>
        /// The make pseudo start time.
        /// </summary>
        private void MakePseudoStartTime()
        {
            var millisecondsInterval = (double)this.numericUpDownGapBetweenLines.Value;
            double millisecondsIndex = millisecondsInterval;
            foreach (Paragraph p in this._subtitle.Paragraphs)
            {
                p.EndTime.TotalMilliseconds = millisecondsIndex + p.Duration.TotalMilliseconds;
                p.StartTime.TotalMilliseconds = millisecondsIndex;

                millisecondsIndex += p.Duration.TotalMilliseconds + millisecondsInterval;
            }
        }

        /// <summary>
        /// The fix durations.
        /// </summary>
        private void FixDurations()
        {
            foreach (Paragraph p in this._subtitle.Paragraphs)
            {
                if (p.Text.Length == 0)
                {
                    p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + 2000;
                }
                else
                {
                    if (this.radioButtonDurationAuto.Checked)
                    {
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(p.Text);
                    }
                    else
                    {
                        p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + ((double)this.numericUpDownDurationFixed.Value);
                    }
                }
            }
        }

        /// <summary>
        /// The import line mode.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        private void ImportLineMode(IEnumerable<string> lines)
        {
            foreach (string loopLine in lines)
            {
                // Replace user line break character with Environment.NewLine.
                string line = loopLine;
                if (this.comboBoxLineBreak.Text.Length > 0)
                {
                    foreach (string splitter in this.comboBoxLineBreak.Text.Split(';'))
                    {
                        var tempSplitter = splitter.Trim();
                        if (tempSplitter.Length > 0)
                        {
                            line = line.Replace(tempSplitter, Environment.NewLine);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (!this.checkBoxRemoveEmptyLines.Checked)
                    {
                        this._subtitle.Paragraphs.Add(new Paragraph());
                    }
                }
                else if (!ContainsLetters(line))
                {
                    if (!this.checkBoxRemoveLinesWithoutLetters.Checked)
                    {
                        this._subtitle.Paragraphs.Add(new Paragraph(0, 0, line.Trim()));
                    }
                }
                else
                {
                    string text = line.Trim();
                    if (this.checkBoxAutoBreak.Enabled && this.checkBoxAutoBreak.Checked)
                    {
                        text = Utilities.AutoBreakLine(text);
                    }

                    this._subtitle.Paragraphs.Add(new Paragraph(0, 0, text));
                }
            }
        }

        /// <summary>
        /// The import split at blank line.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        private void ImportSplitAtBlankLine(IEnumerable<string> lines)
        {
            var sb = new StringBuilder();
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (sb.Length > 0)
                    {
                        string text = sb.ToString().Trim();
                        if (this.checkBoxAutoBreak.Enabled && this.checkBoxAutoBreak.Checked)
                        {
                            text = Utilities.AutoBreakLine(text);
                        }

                        this._subtitle.Paragraphs.Add(new Paragraph { Text = text });
                    }

                    sb = new StringBuilder();
                }
                else if (!ContainsLetters(line))
                {
                    if (!this.checkBoxRemoveLinesWithoutLetters.Checked)
                    {
                        sb.AppendLine(line.Trim());
                    }
                }
                else
                {
                    sb.AppendLine(line.Trim());
                }
            }

            if (sb.Length > 0)
            {
                this.SplitSingle(sb);
            }
        }

        /// <summary>
        /// The can make three liner.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool CanMakeThreeLiner(out string text, string input)
        {
            text = string.Empty;
            if (input.Length < Configuration.Settings.General.SubtitleLineMaximumLength * 3 && input.Length > Configuration.Settings.General.SubtitleLineMaximumLength * 1.5)
            {
                var breaked = Utilities.AutoBreakLine(input).SplitToLines();
                if (breaked.Length == 2 && (breaked[0].Length > Configuration.Settings.General.SubtitleLineMaximumLength || breaked[1].Length > Configuration.Settings.General.SubtitleLineMaximumLength))
                {
                    var first = new StringBuilder();
                    var second = new StringBuilder();
                    var third = new StringBuilder();
                    foreach (string word in input.Replace(Environment.NewLine, " ").Replace("  ", " ").Split(' '))
                    {
                        if (first.Length + word.Length < Configuration.Settings.General.SubtitleLineMaximumLength)
                        {
                            first.Append(' ');
                            first.Append(word);
                        }
                        else if (second.Length + word.Length < Configuration.Settings.General.SubtitleLineMaximumLength)
                        {
                            second.Append(' ');
                            second.Append(word);
                        }
                        else
                        {
                            third.Append(' ');
                            third.Append(word);
                        }
                    }

                    if (first.Length <= Configuration.Settings.General.SubtitleLineMaximumLength && second.Length <= Configuration.Settings.General.SubtitleLineMaximumLength && third.Length <= Configuration.Settings.General.SubtitleLineMaximumLength && third.Length > 10)
                    {
                        if (second.Length > 15)
                        {
                            string ending = second.ToString().Substring(second.Length - 9);
                            int splitPos = -1;
                            if (ending.Contains(". "))
                            {
                                splitPos = ending.IndexOf(". ", StringComparison.Ordinal) + second.Length - 9;
                            }
                            else if (ending.Contains("! "))
                            {
                                splitPos = ending.IndexOf("! ", StringComparison.Ordinal) + second.Length - 9;
                            }
                            else if (ending.Contains(", "))
                            {
                                splitPos = ending.IndexOf(", ", StringComparison.Ordinal) + second.Length - 9;
                            }
                            else if (ending.Contains("? "))
                            {
                                splitPos = ending.IndexOf("? ", StringComparison.Ordinal) + second.Length - 9;
                            }

                            if (splitPos > 0)
                            {
                                text = Utilities.AutoBreakLine(first.ToString().Trim() + second.ToString().Substring(0, splitPos + 1)).Trim() + Environment.NewLine + (second.ToString().Substring(splitPos + 1) + third).Trim();
                                return true;
                            }
                        }

                        text = first + Environment.NewLine + second + Environment.NewLine + third;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The split single.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        private void SplitSingle(StringBuilder sb)
        {
            string t = sb.ToString().Trim();
            string[] tarr = t.Replace("\r\n", "\n").Split('\n');
            if (this.checkBoxMergeShortLines.Checked == false && tarr.Length == 3 && tarr[0].Length < Configuration.Settings.General.SubtitleLineMaximumLength && tarr[1].Length < Configuration.Settings.General.SubtitleLineMaximumLength && tarr[2].Length < Configuration.Settings.General.SubtitleLineMaximumLength)
            {
                this._subtitle.Paragraphs.Add(new Paragraph { Text = tarr[0] + Environment.NewLine + tarr[1] });
                return;
            }

            if (this.checkBoxMergeShortLines.Checked == false && tarr.Length == 2 && tarr[0].Length < Configuration.Settings.General.SubtitleLineMaximumLength && tarr[1].Length < Configuration.Settings.General.SubtitleLineMaximumLength)
            {
                this._subtitle.Paragraphs.Add(new Paragraph { Text = tarr[0] + Environment.NewLine + tarr[1] });
                return;
            }

            if (this.checkBoxMergeShortLines.Checked == false && tarr.Length == 1 && tarr[0].Length < Configuration.Settings.General.SubtitleLineMaximumLength)
            {
                this._subtitle.Paragraphs.Add(new Paragraph { Text = tarr[0].Trim() });
                return;
            }

            Paragraph p = null;
            string threeliner;
            if (CanMakeThreeLiner(out threeliner, sb.ToString()))
            {
                var parts = threeliner.SplitToLines();
                this._subtitle.Paragraphs.Add(new Paragraph { Text = parts[0] + Environment.NewLine + parts[1] });
                this._subtitle.Paragraphs.Add(new Paragraph { Text = parts[2].Trim() });
                return;
            }

            foreach (string text in Utilities.AutoBreakLineMoreThanTwoLines(sb.ToString(), Configuration.Settings.General.SubtitleLineMaximumLength, string.Empty).SplitToLines())
            {
                if (p == null)
                {
                    p = new Paragraph { Text = text };
                }
                else if (p.Text.Contains(Environment.NewLine))
                {
                    this._subtitle.Paragraphs.Add(p);
                    p = new Paragraph();
                    if (text.Length >= Configuration.Settings.General.SubtitleLineMaximumLength)
                    {
                        p.Text = Utilities.AutoBreakLine(text);
                    }
                    else
                    {
                        p.Text = text;
                    }
                }
                else
                {
                    if (this.checkBoxMergeShortLines.Checked || p.Text.Length > Configuration.Settings.General.SubtitleLineMaximumLength || text.Length > Configuration.Settings.General.SubtitleLineMaximumLength)
                    {
                        p.Text = Utilities.AutoBreakLine(p.Text + Environment.NewLine + text.Trim());
                    }
                    else
                    {
                        p.Text = p.Text + Environment.NewLine + text.Trim();
                    }
                }
            }

            if (p != null)
            {
                this._subtitle.Paragraphs.Add(p);
            }
        }

        /// <summary>
        /// The import auto split.
        /// </summary>
        /// <param name="textLines">
        /// The text lines.
        /// </param>
        private void ImportAutoSplit(IEnumerable<string> textLines)
        {
            var sb = new StringBuilder();
            foreach (string line in textLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (!this.checkBoxRemoveEmptyLines.Checked)
                    {
                        sb.AppendLine();
                    }
                }
                else if (!ContainsLetters(line.Trim()))
                {
                    if (!this.checkBoxRemoveLinesWithoutLetters.Checked)
                    {
                        sb.AppendLine(line);
                    }
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            string text = sb.ToString().Replace(Environment.NewLine, " ");

            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            text = text.Replace("!", "_@EXM_");
            text = text.Replace("?", "_@QST_");
            text = text.Replace(".", "_@PER_");

            string[] lines = text.Split('.');

            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("_@EXM_", "!");
                lines[i] = lines[i].Replace("_@QST_", "?");
                lines[i] = lines[i].Replace("_@PER_", ".");
            }

            var list = new List<string>();
            foreach (string s in lines)
            {
                this.AutoSplit(list, s);
            }

            this.ImportLineMode(list);
        }

        /// <summary>
        /// The auto split.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        private void AutoSplit(List<string> list, string line)
        {
            foreach (string split in Utilities.AutoBreakLine(line).SplitToLines())
            {
                if (split.Length <= Configuration.Settings.General.SubtitleLineMaximumLength)
                {
                    list.Add(split);
                }
                else if (split != line)
                {
                    this.AutoSplit(list, split);
                }
                else
                {
                    string s = split.Trim();
                    if (s.Length > Configuration.Settings.General.SubtitleLineMaximumLength)
                    {
                        s = s.Insert(split.Length / 2, Environment.NewLine);
                    }

                    list.Add(s);
                }
            }
        }

        /// <summary>
        /// The contains letters.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ContainsLetters(string line)
        {
            if (string.IsNullOrWhiteSpace(line.Replace("0", string.Empty).Replace("1", string.Empty).Replace("2", string.Empty).Replace("3", string.Empty).Replace("4", string.Empty).Replace("5", string.Empty).Replace("6", string.Empty).Replace("7", string.Empty).Replace("8", string.Empty).Replace("9", string.Empty).Replace(":", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("-", string.Empty).Replace(">", string.Empty)))
            {
                return false;
            }

            foreach (char ch in line)
            {
                if (!"\r\n\t .?\0".Contains(ch))
                {
                    return true;
                }
            }

            return false;
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
            if (this.SubtitleListview1.Items.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
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
        /// The check box remove lines without letters or numbers checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxRemoveLinesWithoutLettersOrNumbersCheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The check box remove empty lines checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxRemoveEmptyLinesCheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The radio button line mode checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonLineModeCheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();

            // textBoxLineBreak and its label are enabled if radioButtonLineMode is checked.
            this.comboBoxLineBreak.Enabled = this.radioButtonLineMode.Checked;
            this.labelLineBreak.Enabled = this.radioButtonLineMode.Checked;
        }

        /// <summary>
        /// The radio button auto split checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonAutoSplitCheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The text box text drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxTextDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// The text box text drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxTextDragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1)
            {
                this.LoadTextFile(files[0]);
            }
        }

        /// <summary>
        /// The load text file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void LoadTextFile(string fileName)
        {
            try
            {
                Encoding encoding = Utilities.GetEncodingFromFile(fileName);
                this.textBoxText.Text = File.ReadAllText(fileName, encoding);
                this.SetVideoFileName(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.GeneratePreview();
        }

        /// <summary>
        /// The load adobe story.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void LoadAdobeStory(string fileName)
        {
            try
            {
                var sb = new StringBuilder();
                var doc = new XmlDocument();
                doc.Load(fileName);
                foreach (XmlNode node in doc.DocumentElement.SelectNodes("//paragraph[@element='Dialog']"))
                {
                    // <paragraph objID="1:28" element="Dialog">
                    XmlNode textRun = node.SelectSingleNode("textRun"); // <textRun objID="1:259">Yeah...I suppose</textRun>
                    if (textRun != null)
                    {
                        sb.AppendLine(textRun.InnerText);
                    }
                }

                this.textBoxText.Text = sb.ToString();
                this.SetVideoFileName(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.radioButtonLineMode.Checked = true;
            this.checkBoxMergeShortLines.Checked = false;
            this.GeneratePreview();
        }

        /// <summary>
        /// The set video file name.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void SetVideoFileName(string fileName)
        {
            this._videoFileName = fileName.Substring(0, fileName.Length - Path.GetExtension(fileName).Length);
            if (this._videoFileName.EndsWith(".en"))
            {
                this._videoFileName = this._videoFileName.Remove(this._videoFileName.Length - 3);
            }

            if (File.Exists(this._videoFileName + ".avi"))
            {
                this._videoFileName += ".avi";
            }
            else if (File.Exists(this._videoFileName + ".mkv"))
            {
                this._videoFileName += ".mkv";
            }
            else
            {
                string[] files = Directory.GetFiles(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(this._videoFileName) + "*.avi");
                if (files.Length == 0)
                {
                    files = Directory.GetFiles(Path.GetDirectoryName(fileName), "*.avi");
                }

                if (files.Length == 0)
                {
                    files = Directory.GetFiles(Path.GetDirectoryName(fileName), "*.mkv");
                }

                if (files.Length > 0)
                {
                    this._videoFileName = files[0];
                }
            }
        }

        /// <summary>
        /// The button refresh click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonRefreshClick(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The radio button duration fixed checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonDurationFixedCheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDownDurationFixed.Enabled = this.radioButtonDurationFixed.Checked;
            this.GeneratePreview();
        }

        /// <summary>
        /// The check box merge short lines checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxMergeShortLinesCheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The import text key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ImportTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The text box text text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxTextTextChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The numeric up down duration fixed value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NumericUpDownDurationFixedValueChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The numeric up down gap between lines value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NumericUpDownGapBetweenLinesValueChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The radio button duration auto checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonDurationAutoCheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The radio button split at blank lines_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButtonSplitAtBlankLines_CheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The check box generate time codes_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxGenerateTimeCodes_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBoxTimeCodes.Enabled = this.checkBoxGenerateTimeCodes.Checked;
            this.GeneratePreview();
        }

        /// <summary>
        /// The import text_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ImportText_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.radioButtonSplitAtBlankLines.Checked)
            {
                Configuration.Settings.Tools.ImportTextSplitting = "blank lines";
            }
            else if (this.radioButtonLineMode.Checked)
            {
                Configuration.Settings.Tools.ImportTextSplitting = "line";
            }
            else
            {
                Configuration.Settings.Tools.ImportTextSplitting = "auto";
            }

            Configuration.Settings.Tools.ImportTextMergeShortLines = this.checkBoxMergeShortLines.Checked;
            Configuration.Settings.Tools.ImportTextLineBreak = this.comboBoxLineBreak.Text.Trim();
        }

        /// <summary>
        /// The check box multiple files_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxMultipleFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxMultipleFiles.Checked)
            {
                this.listViewInputFiles.Visible = true;
                this.textBoxText.Visible = false;
                this.buttonOpenText.Text = Configuration.Settings.Language.ImportText.OpenTextFiles;
                this.groupBoxSplitting.Enabled = false;
            }
            else
            {
                this.listViewInputFiles.Visible = false;
                this.textBoxText.Visible = true;
                this.buttonOpenText.Text = Configuration.Settings.Language.ImportText.OpenTextFile;
                this.groupBoxSplitting.Enabled = true;
            }

            this.GeneratePreview();
        }

        /// <summary>
        /// The list view input files_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewInputFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.All;
            }
        }

        /// <summary>
        /// The list view input files_ drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewInputFiles_DragDrop(object sender, DragEventArgs e)
        {
            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileName in fileNames)
            {
                this.AddInputFile(fileName);
            }

            this.GeneratePreview();
        }

        /// <summary>
        /// The add input file.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        private void AddInputFile(string fileName)
        {
            try
            {
                var fi = new FileInfo(fileName);
                var item = new ListViewItem(fileName);
                item.SubItems.Add(Utilities.FormatBytesToDisplayFileSize(fi.Length));
                if (fi.Length < 1024 * 1024)
                {
                    // max 1 mb
                    this.listViewInputFiles.Items.Add(item);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The check box auto break_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxAutoBreak_CheckedChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The combo box line break_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxLineBreak_TextChanged(object sender, EventArgs e)
        {
            this.GeneratePreview();
        }
    }
}