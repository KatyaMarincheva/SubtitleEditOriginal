// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Compare.cs" company="">
//   
// </copyright>
// <summary>
//   The compare.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Controls;
    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The compare.
    /// </summary>
    public sealed partial class Compare : Form
    {
        /// <summary>
        /// The _back difference color.
        /// </summary>
        private readonly Color _backDifferenceColor = Color.FromArgb(255, 90, 90);

        /// <summary>
        /// The _foreground difference color.
        /// </summary>
        private readonly Color _foregroundDifferenceColor = Color.FromArgb(225, 0, 0);

        /// <summary>
        /// The _main general go to next subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToNextSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle);

        /// <summary>
        /// The _main general go to prev subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToPrevSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle);

        /// <summary>
        /// The _differences.
        /// </summary>
        private List<int> _differences;

        /// <summary>
        /// The _language 1.
        /// </summary>
        private string _language1;

        /// <summary>
        /// The _subtitle 1.
        /// </summary>
        private Subtitle _subtitle1;

        /// <summary>
        /// The _subtitle 2.
        /// </summary>
        private Subtitle _subtitle2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Compare"/> class.
        /// </summary>
        public Compare()
        {
            this.InitializeComponent();

            this.labelSubtitle2.Text = string.Empty;
            this.Text = Configuration.Settings.Language.CompareSubtitles.Title;
            this.buttonPreviousDifference.Text = Configuration.Settings.Language.CompareSubtitles.PreviousDifference;
            this.buttonNextDifference.Text = Configuration.Settings.Language.CompareSubtitles.NextDifference;
            this.checkBoxShowOnlyDifferences.Text = Configuration.Settings.Language.CompareSubtitles.ShowOnlyDifferences;
            this.checkBoxIgnoreLineBreaks.Text = Configuration.Settings.Language.CompareSubtitles.IgnoreLineBreaks;
            this.checkBoxOnlyListDifferencesInText.Text = Configuration.Settings.Language.CompareSubtitles.OnlyLookForDifferencesInText;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.copyTextToolStripMenuItem.Text = Configuration.Settings.Language.Main.Menu.ContextMenu.Copy;
            this.copyTextToolStripMenuItem1.Text = Configuration.Settings.Language.Main.Menu.ContextMenu.Copy;
            this.subtitleListView1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.subtitleListView2.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.subtitleListView1);
            Utilities.InitializeSubtitleFont(this.subtitleListView2);
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle1">
        /// The subtitle 1.
        /// </param>
        /// <param name="subtitleFileName1">
        /// The subtitle file name 1.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public void Initialize(Subtitle subtitle1, string subtitleFileName1, string title)
        {
            this.subtitleListView1.UseSyntaxColoring = false;
            this.subtitleListView2.UseSyntaxColoring = false;

            this.Compare_Resize(null, null);
            this.labelStatus.Text = string.Empty;
            this._subtitle1 = subtitle1;
            this.labelSubtitle1.Text = subtitleFileName1;
            if (string.IsNullOrEmpty(subtitleFileName1))
            {
                this.labelSubtitle1.Text = title;
            }

            this.subtitleListView1.Fill(subtitle1);

            if (!string.IsNullOrEmpty(subtitleFileName1))
            {
                try
                {
                    this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(subtitleFileName1);
                }
                catch
                {
                }
            }

            this.openFileDialog1.Filter = Utilities.GetOpenDialogFilter();
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
            this.subtitleListView2.SelectIndexAndEnsureVisible(0);
            this._language1 = Utilities.AutoDetectGoogleLanguage(this._subtitle1);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle1">
        /// The subtitle 1.
        /// </param>
        /// <param name="subtitleFileName1">
        /// The subtitle file name 1.
        /// </param>
        /// <param name="subtitle2">
        /// The subtitle 2.
        /// </param>
        /// <param name="subtitleFileName2">
        /// The subtitle file name 2.
        /// </param>
        public void Initialize(Subtitle subtitle1, string subtitleFileName1, Subtitle subtitle2, string subtitleFileName2)
        {
            this.Compare_Resize(null, null);
            this.labelStatus.Text = string.Empty;
            this._subtitle1 = subtitle1;
            this.labelSubtitle1.Text = subtitleFileName1;

            this._subtitle2 = subtitle2;
            this.labelSubtitle2.Text = subtitleFileName2;

            this._language1 = Utilities.AutoDetectGoogleLanguage(this._subtitle1);
            this.CompareSubtitles();

            if (string.IsNullOrEmpty(subtitleFileName1))
            {
                this.openFileDialog1.InitialDirectory = Path.GetDirectoryName(subtitleFileName1);
            }

            this.openFileDialog1.Filter = Utilities.GetOpenDialogFilter();
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
            this.subtitleListView2.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The button open subtitle 1 click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOpenSubtitle1Click(object sender, EventArgs e)
        {
            this.openFileDialog1.FileName = string.Empty;

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (FileUtil.IsVobSub(this.openFileDialog1.FileName) || FileUtil.IsBluRaySup(this.openFileDialog1.FileName))
                {
                    MessageBox.Show(Configuration.Settings.Language.CompareSubtitles.CannotCompareWithImageBasedSubtitles);
                    return;
                }

                this._subtitle1 = new Subtitle();
                Encoding encoding;
                var format = this._subtitle1.LoadSubtitle(this.openFileDialog1.FileName, out encoding, null);
                if (format == null)
                {
                    var pac = new Pac();
                    if (pac.IsMine(null, this.openFileDialog1.FileName))
                    {
                        pac.BatchMode = true;
                        pac.LoadSubtitle(this._subtitle1, null, this.openFileDialog1.FileName);
                        format = pac;
                    }
                }

                if (format == null)
                {
                    var cavena890 = new Cavena890();
                    if (cavena890.IsMine(null, this.openFileDialog1.FileName))
                    {
                        cavena890.LoadSubtitle(this._subtitle1, null, this.openFileDialog1.FileName);
                    }
                }

                if (format == null)
                {
                    var spt = new Spt();
                    if (spt.IsMine(null, this.openFileDialog1.FileName))
                    {
                        spt.LoadSubtitle(this._subtitle1, null, this.openFileDialog1.FileName);
                    }
                }

                if (format == null)
                {
                    var cheetahCaption = new CheetahCaption();
                    if (cheetahCaption.IsMine(null, this.openFileDialog1.FileName))
                    {
                        cheetahCaption.LoadSubtitle(this._subtitle1, null, this.openFileDialog1.FileName);
                    }
                }

                if (format == null)
                {
                    var chk = new Chk();
                    if (chk.IsMine(null, this.openFileDialog1.FileName))
                    {
                        chk.LoadSubtitle(this._subtitle1, null, this.openFileDialog1.FileName);
                    }
                }

                if (format == null)
                {
                    var asc = new TimeLineAscii();
                    if (asc.IsMine(null, this.openFileDialog1.FileName))
                    {
                        asc.LoadSubtitle(this._subtitle1, null, this.openFileDialog1.FileName);
                    }
                }

                if (format == null)
                {
                    var asc = new TimeLineFootageAscii();
                    if (asc.IsMine(null, this.openFileDialog1.FileName))
                    {
                        asc.LoadSubtitle(this._subtitle1, null, this.openFileDialog1.FileName);
                    }
                }

                this.subtitleListView1.Fill(this._subtitle1);
                this.subtitleListView1.SelectIndexAndEnsureVisible(0);
                this.subtitleListView2.SelectIndexAndEnsureVisible(0);
                this.labelSubtitle1.Text = this.openFileDialog1.FileName;
                this._language1 = Utilities.AutoDetectGoogleLanguage(this._subtitle1);
                if (this._subtitle1.Paragraphs.Count > 0)
                {
                    this.CompareSubtitles();
                }
            }
        }

        /// <summary>
        /// The button open subtitle 2 click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonOpenSubtitle2Click(object sender, EventArgs e)
        {
            this.openFileDialog1.FileName = string.Empty;

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (FileUtil.IsVobSub(this.openFileDialog1.FileName) || FileUtil.IsBluRaySup(this.openFileDialog1.FileName))
                {
                    MessageBox.Show(Configuration.Settings.Language.CompareSubtitles.CannotCompareWithImageBasedSubtitles);
                    return;
                }

                this._subtitle2 = new Subtitle();
                Encoding encoding;
                var format = this._subtitle2.LoadSubtitle(this.openFileDialog1.FileName, out encoding, null);
                if (format == null)
                {
                    var pac = new Pac();
                    if (pac.IsMine(null, this.openFileDialog1.FileName))
                    {
                        pac.BatchMode = true;
                        pac.LoadSubtitle(this._subtitle2, null, this.openFileDialog1.FileName);
                        format = pac;
                    }
                }

                if (format == null)
                {
                    var cavena890 = new Cavena890();
                    if (cavena890.IsMine(null, this.openFileDialog1.FileName))
                    {
                        cavena890.LoadSubtitle(this._subtitle2, null, this.openFileDialog1.FileName);
                    }
                }

                this.subtitleListView2.Fill(this._subtitle2);
                this.subtitleListView1.SelectIndexAndEnsureVisible(0);
                this.subtitleListView2.SelectIndexAndEnsureVisible(0);
                this.labelSubtitle2.Text = this.openFileDialog1.FileName;
                if (this._subtitle2.Paragraphs.Count > 0)
                {
                    this.CompareSubtitles();
                }
            }
        }

        /// <summary>
        /// The compare subtitles.
        /// </summary>
        private void CompareSubtitles()
        {
            this.timer1.Stop();
            var sub1 = new Subtitle(this._subtitle1);
            var sub2 = new Subtitle(this._subtitle2);

            int index = 0;
            Paragraph p1 = sub1.GetParagraphOrDefault(index);
            Paragraph p2 = sub2.GetParagraphOrDefault(index);
            int max = sub1.Paragraphs.Count;
            if (max < sub2.Paragraphs.Count)
            {
                max = sub2.Paragraphs.Count;
            }

            while (index < max)
            {
                if (p1 != null && p2 != null)
                {
                    if (p1.ToString() == p2.ToString())
                    {
                    }
                    else
                    {
                        if (GetColumnsEqualExceptNumber(p1, p2) == 0)
                        {
                            int oldIndex = index;
                            for (int i = 1; oldIndex + i < max; i++)
                            {
                                if (GetColumnsEqualExceptNumber(sub1.GetParagraphOrDefault(index + i), p2) > 1)
                                {
                                    for (int j = 0; j < i; j++)
                                    {
                                        sub2.Paragraphs.Insert(index, new Paragraph());
                                        index++;
                                    }

                                    break;
                                }

                                if (GetColumnsEqualExceptNumber(p1, sub2.GetParagraphOrDefault(index + i)) > 1)
                                {
                                    for (int j = 0; j < i; j++)
                                    {
                                        sub1.Paragraphs.Insert(index, new Paragraph());
                                        index++;
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                index++;
                p1 = sub1.GetParagraphOrDefault(index);
                p2 = sub2.GetParagraphOrDefault(index);
            }

            this.subtitleListView1.Fill(sub1);
            this.subtitleListView2.Fill(sub2);

            // coloring + differences index list
            this._differences = new List<int>();
            index = 0;
            p1 = sub1.GetParagraphOrDefault(index);
            p2 = sub2.GetParagraphOrDefault(index);
            int totalWords = 0;
            int wordsChanged = 0;
            if (this.checkBoxOnlyListDifferencesInText.Checked)
            {
                while (index < sub1.Paragraphs.Count || index < sub2.Paragraphs.Count)
                {
                    if (p1 != null && p2 != null)
                    {
                        Utilities.GetTotalAndChangedWords(p1.Text, p2.Text, ref totalWords, ref wordsChanged, this.checkBoxIgnoreLineBreaks.Checked, this.GetBreakToLetter());
                        if (this.FixWhitespace(p1.ToString()) == this.FixWhitespace(p2.ToString()) && p1.Number == p2.Number)
                        { // no differences
                        }
                        else if (p1.ToString() == new Paragraph().ToString())
                        {
                            this._differences.Add(index);
                            this.subtitleListView1.ColorOut(index, Color.Salmon);
                        }
                        else if (p2.ToString() == new Paragraph().ToString())
                        {
                            this._differences.Add(index);
                            this.subtitleListView2.ColorOut(index, Color.Salmon);
                        }
                        else if (this.FixWhitespace(p1.Text) != this.FixWhitespace(p2.Text))
                        {
                            this._differences.Add(index);
                            this.subtitleListView1.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexText);
                        }
                    }
                    else
                    {
                        if (p1 != null && p1.Text != null)
                        {
                            totalWords += Utilities.SplitForChangedCalc(p1.Text, this.checkBoxIgnoreLineBreaks.Checked, this.GetBreakToLetter()).Length;
                        }
                        else if (p2 != null && p2.Text != null)
                        {
                            totalWords += Utilities.SplitForChangedCalc(p2.Text, this.checkBoxIgnoreLineBreaks.Checked, this.GetBreakToLetter()).Length;
                        }

                        this._differences.Add(index);
                    }

                    index++;
                    p1 = sub1.GetParagraphOrDefault(index);
                    p2 = sub2.GetParagraphOrDefault(index);
                }
            }
            else
            {
                while (index < sub1.Paragraphs.Count || index < sub2.Paragraphs.Count)
                {
                    if (p1 != null && p2 != null)
                    {
                        Utilities.GetTotalAndChangedWords(p1.Text, p2.Text, ref totalWords, ref wordsChanged, this.checkBoxIgnoreLineBreaks.Checked, this.GetBreakToLetter());
                        if (this.FixWhitespace(p1.ToString()) != this.FixWhitespace(p2.ToString()))
                        {
                            this._differences.Add(index);
                        }

                        if (this.FixWhitespace(p1.ToString()) == this.FixWhitespace(p2.ToString()) && p1.Number == p2.Number)
                        { // no differences
                        }
                        else if (p1.ToString() == new Paragraph().ToString())
                        {
                            this.subtitleListView1.ColorOut(index, Color.Salmon);
                        }
                        else if (p2.ToString() == new Paragraph().ToString())
                        {
                            this.subtitleListView2.ColorOut(index, Color.Salmon);
                        }
                        else
                        {
                            int columnsAlike = GetColumnsEqualExceptNumber(p1, p2);
                            if (columnsAlike > 0)
                            {
                                if (p1.StartTime.TotalMilliseconds != p2.StartTime.TotalMilliseconds)
                                {
                                    this.subtitleListView1.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexStart);
                                    this.subtitleListView2.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexStart);
                                }

                                if (p1.EndTime.TotalMilliseconds != p2.EndTime.TotalMilliseconds)
                                {
                                    this.subtitleListView1.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexEnd);
                                    this.subtitleListView2.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexEnd);
                                }

                                if (p1.Duration.TotalMilliseconds != p2.Duration.TotalMilliseconds)
                                {
                                    this.subtitleListView1.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexDuration);
                                    this.subtitleListView2.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexDuration);
                                }

                                if (this.FixWhitespace(p1.Text.Trim()) != this.FixWhitespace(p2.Text.Trim()))
                                {
                                    this.subtitleListView1.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexText);
                                    this.subtitleListView2.SetBackgroundColor(index, Color.LightGreen, SubtitleListView.ColumnIndexText);
                                }

                                if (p1.Number != p2.Number)
                                {
                                    this.subtitleListView1.SetBackgroundColor(index, Color.LightYellow, SubtitleListView.ColumnIndexNumber);
                                    this.subtitleListView2.SetBackgroundColor(index, Color.LightYellow, SubtitleListView.ColumnIndexNumber);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (p1 != null && p1.Text != null)
                        {
                            totalWords += Utilities.SplitForChangedCalc(p1.Text, this.checkBoxIgnoreLineBreaks.Checked, this.GetBreakToLetter()).Length;
                        }
                        else if (p2 != null && p2.Text != null)
                        {
                            totalWords += Utilities.SplitForChangedCalc(p2.Text, this.checkBoxIgnoreLineBreaks.Checked, this.GetBreakToLetter()).Length;
                        }

                        this._differences.Add(index);
                    }

                    index++;
                    p1 = sub1.GetParagraphOrDefault(index);
                    p2 = sub2.GetParagraphOrDefault(index);
                }
            }

            this.UpdatePreviousAndNextButtons();

            if (max == this._differences.Count)
            {
                this.labelStatus.Text = Configuration.Settings.Language.CompareSubtitles.SubtitlesNotAlike;
                this.labelStatus.Font = new Font(this.labelStatus.Font.FontFamily, this.labelStatus.Font.Size, FontStyle.Bold);
            }
            else
            {
                if (wordsChanged != totalWords && wordsChanged > 0)
                {
                    string formatString = Configuration.Settings.Language.CompareSubtitles.XNumberOfDifferenceAndPercentChanged;
                    if (this.GetBreakToLetter())
                    {
                        formatString = Configuration.Settings.Language.CompareSubtitles.XNumberOfDifferenceAndPercentLettersChanged;
                    }

                    this.labelStatus.Text = string.Format(formatString, this._differences.Count, wordsChanged * 100 / totalWords);
                }
                else
                {
                    this.labelStatus.Text = string.Format(Configuration.Settings.Language.CompareSubtitles.XNumberOfDifference, this._differences.Count);
                }

                this.labelStatus.Font = new Font(this.labelStatus.Font.FontFamily, this.labelStatus.Font.Size);
            }

            if (this.checkBoxShowOnlyDifferences.Checked)
            { // Remove all lines with no difference
                this.subtitleListView1.BeginUpdate();
                this.subtitleListView2.BeginUpdate();
                if (max != this._differences.Count)
                {
                    for (index = Math.Max(this.subtitleListView1.Items.Count, this.subtitleListView2.Items.Count); index >= 0; index--)
                    {
                        if (!this._differences.Contains(index))
                        {
                            if (this.subtitleListView1.Items.Count > index)
                            {
                                this.subtitleListView1.Items.RemoveAt(index);
                            }

                            if (this.subtitleListView2.Items.Count > index)
                            {
                                this.subtitleListView2.Items.RemoveAt(index);
                            }
                        }
                    }
                }

                this.subtitleListView1.EndUpdate();
                this.subtitleListView2.EndUpdate();
                this._differences = new List<int>();
                for (index = 0; index < Math.Max(this.subtitleListView1.Items.Count, this.subtitleListView2.Items.Count); index++)
                {
                    this._differences.Add(index);
                }
            }

            this.timer1.Start();
            this.subtitleListView1.FirstVisibleIndex = -1;
            this.subtitleListView1.SelectIndexAndEnsureVisible(0);
        }

        /// <summary>
        /// The get break to letter.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool GetBreakToLetter()
        {
            if (this._language1 != null && (this._language1 == "jp" || this._language1 == "zh"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The fix whitespace.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FixWhitespace(string p)
        {
            if (this.checkBoxIgnoreLineBreaks.Checked)
            {
                p = p.Replace(Environment.NewLine, " ");
                while (p.Contains("  "))
                {
                    p = p.Replace("  ", " ");
                }
            }

            return p;
        }

        /// <summary>
        /// The get columns equal except number.
        /// </summary>
        /// <param name="p1">
        /// The p 1.
        /// </param>
        /// <param name="p2">
        /// The p 2.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetColumnsEqualExceptNumber(Paragraph p1, Paragraph p2)
        {
            if (p1 == null || p2 == null)
            {
                return 0;
            }

            int columnsEqual = 0;
            if (p1.StartTime.TotalMilliseconds == p2.StartTime.TotalMilliseconds)
            {
                columnsEqual++;
            }

            if (p1.EndTime.TotalMilliseconds == p2.EndTime.TotalMilliseconds)
            {
                columnsEqual++;
            }

            if (p1.Duration.TotalMilliseconds == p2.Duration.TotalMilliseconds)
            {
                columnsEqual++;
            }

            if (p1.Text.Trim() == p2.Text.Trim())
            {
                columnsEqual++;
            }

            return columnsEqual;
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
            this.Close();
        }

        /// <summary>
        /// The compare_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Compare_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.Enter && this.buttonNextDifference.Enabled)
            {
                this.ButtonNextDifferenceClick(null, null);
            }
            else if (e.KeyCode == Keys.Right && this.buttonNextDifference.Enabled)
            {
                this.ButtonNextDifferenceClick(null, null);
            }
            else if (e.KeyCode == Keys.Left && this.buttonPreviousDifference.Enabled)
            {
                this.ButtonPreviousDifferenceClick(null, null);
            }
            else if (this._mainGeneralGoToNextSubtitle == e.KeyData || (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt))
            {
                SubtitleListView lv = this.subtitleListView1;
                if (this.subtitleListView2.Focused)
                {
                    lv = this.subtitleListView2;
                }

                int selectedIndex = 0;
                if (lv.SelectedItems.Count > 0)
                {
                    selectedIndex = lv.SelectedItems[0].Index;
                    selectedIndex++;
                }

                lv.SelectIndexAndEnsureVisible(selectedIndex);
            }
            else if (this._mainGeneralGoToPrevSubtitle == e.KeyData || (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt))
            {
                SubtitleListView lv = this.subtitleListView1;
                if (this.subtitleListView2.Focused)
                {
                    lv = this.subtitleListView2;
                }

                int selectedIndex = 0;
                if (lv.SelectedItems.Count > 0)
                {
                    selectedIndex = lv.SelectedItems[0].Index;
                    selectedIndex--;
                }

                lv.SelectIndexAndEnsureVisible(selectedIndex);
            }
        }

        /// <summary>
        /// The subtitle list view 1 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView1SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdatePreviousAndNextButtons();
            this.ShowText();
        }

        /// <summary>
        /// The show text.
        /// </summary>
        private void ShowText()
        {
            string text1 = string.Empty;
            string text2 = string.Empty;

            if (this.subtitleListView1.SelectedItems.Count == 1)
            {
                text1 = this.subtitleListView1.GetText(this.subtitleListView1.SelectedItems[0].Index);

                if (this.subtitleListView2.Items.Count > this.subtitleListView1.SelectedItems[0].Index)
                {
                    text2 = this.subtitleListView2.GetText(this.subtitleListView1.SelectedItems[0].Index);
                }
            }

            this.richTextBox1.Text = text1;
            this.richTextBox2.Text = text2;

            // show diff
            if (string.IsNullOrWhiteSpace(text1) || string.IsNullOrWhiteSpace(text2) || text1 == text2)
            {
                return;
            }

            this.ShowTextDifference();
        }

        /// <summary>
        /// The show text difference.
        /// </summary>
        private void ShowTextDifference()
        {
            // from start
            int minLength = Math.Min(this.richTextBox1.Text.Length, this.richTextBox2.Text.Length);
            int startCharactersOk = 0;
            for (int i = 0; i < minLength; i++)
            {
                if (this.richTextBox1.Text[i] == this.richTextBox2.Text[i])
                {
                    startCharactersOk++;
                }
                else
                {
                    if (this.richTextBox1.Text.Length > i + 4 && this.richTextBox2.Text.Length > i + 4 && this.richTextBox1.Text[i + 1] == this.richTextBox2.Text[i + 1] && this.richTextBox1.Text[i + 2] == this.richTextBox2.Text[i + 2] && this.richTextBox1.Text[i + 3] == this.richTextBox2.Text[i + 3] && this.richTextBox1.Text[i + 4] == this.richTextBox2.Text[i + 4])
                    {
                        startCharactersOk++;

                        this.richTextBox1.SelectionStart = i;
                        this.richTextBox1.SelectionLength = 1;
                        this.richTextBox1.SelectionColor = this._foregroundDifferenceColor;
                        if (string.IsNullOrWhiteSpace(this.richTextBox1.SelectedText))
                        {
                            this.richTextBox1.SelectionBackColor = this._backDifferenceColor;
                        }

                        this.richTextBox2.SelectionStart = i;
                        this.richTextBox2.SelectionLength = 1;
                        this.richTextBox2.SelectionColor = this._foregroundDifferenceColor;
                        if (string.IsNullOrWhiteSpace(this.richTextBox2.SelectedText))
                        {
                            this.richTextBox2.SelectionBackColor = this._backDifferenceColor;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            int maxLength = Math.Max(this.richTextBox1.Text.Length, this.richTextBox2.Text.Length);
            for (int i = startCharactersOk; i <= maxLength; i++)
            {
                if (i < this.richTextBox1.Text.Length)
                {
                    this.richTextBox1.SelectionStart = i;
                    this.richTextBox1.SelectionLength = 1;
                    this.richTextBox1.SelectionBackColor = this._backDifferenceColor;
                    if (string.IsNullOrWhiteSpace(this.richTextBox1.SelectedText))
                    {
                        this.richTextBox1.SelectionBackColor = this._backDifferenceColor;
                    }
                }

                if (i < this.richTextBox2.Text.Length)
                {
                    this.richTextBox2.SelectionStart = i;
                    this.richTextBox2.SelectionLength = 1;
                    this.richTextBox2.SelectionColor = this._foregroundDifferenceColor;
                    if (string.IsNullOrWhiteSpace(this.richTextBox2.SelectedText))
                    {
                        this.richTextBox2.SelectionBackColor = this._backDifferenceColor;
                    }
                }
            }

            // from end
            for (int i = 1; i < minLength; i++)
            {
                if (this.richTextBox1.Text[this.richTextBox1.Text.Length - i] == this.richTextBox2.Text[this.richTextBox2.Text.Length - i])
                {
                    this.richTextBox1.SelectionStart = this.richTextBox1.Text.Length - i;
                    this.richTextBox1.SelectionLength = 1;
                    this.richTextBox1.SelectionColor = Color.Black;
                    this.richTextBox1.SelectionBackColor = this.richTextBox1.BackColor;

                    this.richTextBox2.SelectionStart = this.richTextBox2.Text.Length - i;
                    this.richTextBox2.SelectionLength = 1;
                    this.richTextBox2.SelectionColor = Color.Black;
                    this.richTextBox2.SelectionBackColor = this.richTextBox1.BackColor;
                }
                else
                {
                    break;
                }
            }

            // special situation - equal, but one has more chars
            if (this.richTextBox1.Text.Length > this.richTextBox2.Text.Length)
            {
                if (this.richTextBox1.Text.StartsWith(this.richTextBox2.Text))
                {
                    this.richTextBox1.SelectionStart = this.richTextBox2.Text.Length;
                    this.richTextBox1.SelectionLength = this.richTextBox1.Text.Length - this.richTextBox2.Text.Length;
                    this.richTextBox1.SelectionBackColor = this._backDifferenceColor;
                }
            }
            else if (this.richTextBox2.Text.Length > this.richTextBox1.Text.Length)
            {
                if (this.richTextBox2.Text.StartsWith(this.richTextBox1.Text))
                {
                    this.richTextBox2.SelectionStart = this.richTextBox1.Text.Length;
                    this.richTextBox2.SelectionLength = this.richTextBox2.Text.Length - this.richTextBox1.Text.Length;
                    this.richTextBox2.SelectionColor = this._foregroundDifferenceColor;
                }
            }
        }

        /// <summary>
        /// The update previous and next buttons.
        /// </summary>
        private void UpdatePreviousAndNextButtons()
        {
            if (this.subtitleListView1.Items.Count > 0 && this.subtitleListView2.Items.Count > 0 && this._differences != null && this._differences.Count > 0)
            {
                if (this.subtitleListView1.SelectedItems.Count == 0)
                {
                    this.buttonPreviousDifference.Enabled = false;
                    this.buttonNextDifference.Enabled = true;
                }
                else
                {
                    int index = this.subtitleListView1.SelectedItems[0].Index;
                    this.buttonPreviousDifference.Enabled = this._differences[0] < index;
                    this.buttonNextDifference.Enabled = this._differences[this._differences.Count - 1] > index;
                }
            }
            else
            {
                this.buttonPreviousDifference.Enabled = false;
                this.buttonNextDifference.Enabled = false;
            }
        }

        /// <summary>
        /// The subtitle list view 2 selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView2SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdatePreviousAndNextButtons();
        }

        /// <summary>
        /// The compare_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Compare_Resize(object sender, EventArgs e)
        {
            this.subtitleListView1.Width = (this.Width / 2) - 20;

            this.subtitleListView2.Left = (this.Width / 2) - 3;
            this.subtitleListView2.Width = (this.Width / 2) - 20;
            this.labelSubtitle2.Left = this.subtitleListView2.Left;
            this.buttonOpenSubtitle2.Left = this.subtitleListView2.Left;

            this.subtitleListView1.Height = this.Height - (this.subtitleListView1.Top + 140);
            this.subtitleListView2.Height = this.Height - (this.subtitleListView2.Top + 140);

            this.richTextBox1.Width = this.subtitleListView1.Width;
            this.richTextBox2.Width = this.subtitleListView2.Width;
            this.richTextBox2.Left = this.subtitleListView2.Left;
        }

        /// <summary>
        /// The button previous difference click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonPreviousDifferenceClick(object sender, EventArgs e)
        {
            if (this._differences != null && this._differences.Count > 0)
            {
                if (this.subtitleListView1.SelectedItems.Count == 0)
                {
                    this.subtitleListView1.SelectIndexAndEnsureVisible(this._differences[0]);
                }
                else
                {
                    for (int i = this.subtitleListView1.SelectedItems[0].Index - 1; i >= 0; i--)
                    {
                        if (this._differences.Contains(i))
                        {
                            this.subtitleListView1.SelectIndexAndEnsureVisible(i - 2);
                            this.subtitleListView1.SelectIndexAndEnsureVisible(i + 2);
                            this.subtitleListView1.SelectIndexAndEnsureVisible(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The button next difference click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonNextDifferenceClick(object sender, EventArgs e)
        {
            if (this._differences != null && this._differences.Count > 0)
            {
                if (this.subtitleListView1.SelectedItems.Count == 0)
                {
                    this.subtitleListView1.SelectIndexAndEnsureVisible(this._differences[0]);
                }
                else
                {
                    for (int i = this.subtitleListView1.SelectedItems[0].Index + 1; i < this.subtitleListView1.Items.Count; i++)
                    {
                        if (this._differences.Contains(i))
                        {
                            this.subtitleListView1.SelectIndexAndEnsureVisible(i - 2);
                            this.subtitleListView1.SelectIndexAndEnsureVisible(i + 2);
                            this.subtitleListView1.SelectIndexAndEnsureVisible(i);
                            this.subtitleListView1.Focus();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The timer 1 tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Timer1Tick(object sender, EventArgs e)
        {
            char activeListView;
            var p = this.PointToClient(MousePosition);
            if (p.X >= this.subtitleListView1.Left && p.X <= this.subtitleListView1.Left + this.subtitleListView1.Width + 2)
            {
                activeListView = 'L';
            }
            else if (p.X >= this.subtitleListView2.Left && p.X <= this.subtitleListView2.Left + this.subtitleListView2.Width + 2)
            {
                activeListView = 'R';
            }
            else
            {
                return;
            }

            if (this.subtitleListView1.SelectedItems.Count > 0 && activeListView == 'L')
            {
                if (this.subtitleListView2.SelectedItems.Count > 0 && this.subtitleListView1.SelectedItems[0].Index == this.subtitleListView2.SelectedItems[0].Index)
                {
                    if (this.subtitleListView1.TopItem.Index != this.subtitleListView2.TopItem.Index && this.subtitleListView2.Items.Count > this.subtitleListView1.TopItem.Index)
                    {
                        this.subtitleListView2.TopItem = this.subtitleListView2.Items[this.subtitleListView1.TopItem.Index];
                    }

                    return;
                }

                this.subtitleListView2.SelectedIndexChanged -= this.SubtitleListView2SelectedIndexChanged;
                this.subtitleListView2.SelectIndexAndEnsureVisible(this.subtitleListView1.SelectedItems[0].Index);
                if (this.subtitleListView2.TopItem != null && this.subtitleListView1.TopItem.Index != this.subtitleListView2.TopItem.Index && this.subtitleListView2.Items.Count > this.subtitleListView1.TopItem.Index)
                {
                    this.subtitleListView2.TopItem = this.subtitleListView2.Items[this.subtitleListView1.TopItem.Index];
                }

                this.subtitleListView2.SelectedIndexChanged += this.SubtitleListView2SelectedIndexChanged;
            }
            else if (this.subtitleListView2.SelectedItems.Count > 0 && activeListView == 'R')
            {
                if (this.subtitleListView1.SelectedItems.Count > 0 && this.subtitleListView2.SelectedItems[0].Index == this.subtitleListView1.SelectedItems[0].Index)
                {
                    if (this.subtitleListView2.TopItem.Index != this.subtitleListView1.TopItem.Index && this.subtitleListView1.Items.Count > this.subtitleListView2.TopItem.Index)
                    {
                        this.subtitleListView1.TopItem = this.subtitleListView1.Items[this.subtitleListView2.TopItem.Index];
                    }

                    return;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(this.subtitleListView2.SelectedItems[0].Index);
                if (this.subtitleListView2.TopItem.Index != this.subtitleListView1.TopItem.Index && this.subtitleListView1.Items.Count > this.subtitleListView2.TopItem.Index)
                {
                    this.subtitleListView1.TopItem = this.subtitleListView1.Items[this.subtitleListView2.TopItem.Index];
                }
            }
        }

        /// <summary>
        /// The compare_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Compare_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.timer1.Stop();
        }

        /// <summary>
        /// The check box show only differences_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxShowOnlyDifferences_CheckedChanged(object sender, EventArgs e)
        {
            this.CompareSubtitles();
        }

        /// <summary>
        /// The check box only list differences in text_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxOnlyListDifferencesInText_CheckedChanged(object sender, EventArgs e)
        {
            this.CompareSubtitles();
        }

        /// <summary>
        /// The check box ignore line breaks_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxIgnoreLineBreaks_CheckedChanged(object sender, EventArgs e)
        {
            this.CompareSubtitles();
        }

        /// <summary>
        /// The label subtitle 1_ mouse hover.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void labelSubtitle1_MouseHover(object sender, EventArgs e)
        {
            this.ShowTip(this.labelSubtitle1);
        }

        /// <summary>
        /// The label subtitle 2_ mouse hover.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void labelSubtitle2_MouseHover(object sender, EventArgs e)
        {
            this.ShowTip(this.labelSubtitle2);
        }

        /// <summary>
        /// The show tip.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        private void ShowTip(Control control)
        {
            string sub1Path = control.Text;
            if (!string.IsNullOrEmpty(sub1Path))
            {
                this.toolTip1.Show(Path.GetFileName(sub1Path), control);
            }
        }

        /// <summary>
        /// The subtitle list view 1_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListView1_DragEnter(object sender, DragEventArgs e)
        {
            VerifyDragEnter(e);
        }

        /// <summary>
        /// The subtitle list view 2_ drag enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListView2_DragEnter(object sender, DragEventArgs e)
        {
            VerifyDragEnter(e);
        }

        /// <summary>
        /// The verify drag enter.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void VerifyDragEnter(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// The subtitle list view 1_ drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListView1_DragDrop(object sender, DragEventArgs e)
        {
            this.VerifyDragDrop(sender as ListView, e);
        }

        /// <summary>
        /// The subtitle list view 2_ drag drop.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListView2_DragDrop(object sender, DragEventArgs e)
        {
            this.VerifyDragDrop(sender as ListView, e);
        }

        /// <summary>
        /// The verify drag drop.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VerifyDragDrop(ListView listView, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files.Length > 1)
            {
                MessageBox.Show(Configuration.Settings.Language.Main.DropOnlyOneFile, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string filePath = files[0];
            var listExt = new List<string>();
            foreach (var s in Utilities.GetOpenDialogFilter().Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (s.EndsWith(';'))
                {
                    listExt.Add(s.Trim(';'));
                }
            }

            if (!listExt.Contains(Path.GetExtension(filePath)))
            {
                return;
            }

            if (FileUtil.IsVobSub(filePath) || FileUtil.IsBluRaySup(filePath))
            {
                MessageBox.Show(Configuration.Settings.Language.CompareSubtitles.CannotCompareWithImageBasedSubtitles);
                return;
            }

            Encoding encoding;
            if (listView.Name == "subtitleListView1")
            {
                this._subtitle1 = new Subtitle();
                this._subtitle1.LoadSubtitle(filePath, out encoding, null);
                this.subtitleListView1.Fill(this._subtitle1);
                this.subtitleListView1.SelectIndexAndEnsureVisible(0);
                this.subtitleListView2.SelectIndexAndEnsureVisible(0);
                this.labelSubtitle1.Text = filePath;
                this._language1 = Utilities.AutoDetectGoogleLanguage(this._subtitle1);
                if (this._subtitle1.Paragraphs.Count > 0)
                {
                    this.CompareSubtitles();
                }
            }
            else
            {
                this._subtitle2 = new Subtitle();
                this._subtitle2.LoadSubtitle(filePath, out encoding, null);
                this.subtitleListView2.Fill(this._subtitle2);
                this.subtitleListView1.SelectIndexAndEnsureVisible(0);
                this.subtitleListView2.SelectIndexAndEnsureVisible(0);
                this.labelSubtitle2.Text = filePath;
                if (this._subtitle2.Paragraphs.Count > 0)
                {
                    this.CompareSubtitles();
                }
            }
        }

        /// <summary>
        /// The copy text tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void copyTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyTextToClipboard(this.richTextBox1);
        }

        /// <summary>
        /// The copy text tool strip menu item 1_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void copyTextToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyTextToClipboard(this.richTextBox2);
        }

        /// <summary>
        /// The copy text to clipboard.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private static void CopyTextToClipboard(RichTextBox sender)
        {
            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                return;
            }

            if (sender.SelectedText.Length > 0)
            {
                Clipboard.SetText(sender.SelectedText);
                return;
            }

            Clipboard.SetText(sender.Text);
        }
    }
}