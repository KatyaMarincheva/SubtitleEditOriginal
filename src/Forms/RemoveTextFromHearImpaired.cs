// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveTextFromHearImpaired.cs" company="">
//   
// </copyright>
// <summary>
//   The form remove text for hear impaired.
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
    using Nikse.SubtitleEdit.Logic.Forms;

    /// <summary>
    /// The form remove text for hear impaired.
    /// </summary>
    public sealed partial class FormRemoveTextForHearImpaired : PositionAndSizeForm
    {
        /// <summary>
        /// The _language.
        /// </summary>
        private readonly LanguageStructure.RemoveTextFromHearImpaired _language;

        /// <summary>
        /// The _remove text for hi lib.
        /// </summary>
        private readonly RemoveTextForHI _removeTextForHiLib;

        /// <summary>
        /// The _fixes.
        /// </summary>
        private Dictionary<Paragraph, string> _fixes;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormRemoveTextForHearImpaired"/> class.
        /// </summary>
        public FormRemoveTextForHearImpaired()
        {
            this.InitializeComponent();

            this._removeTextForHiLib = new RemoveTextForHI(this.GetSettings());

            this.checkBoxRemoveTextBetweenSquares.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenBrackets;
            this.checkBoxRemoveTextBetweenParentheses.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenParentheses;
            this.checkBoxRemoveTextBetweenBrackets.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCurlyBrackets;
            this.checkBoxRemoveTextBetweenQuestionMarks.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenQuestionMarks;
            this.checkBoxRemoveTextBetweenCustomTags.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustom;
            this.checkBoxOnlyIfInSeparateLine.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenOnlySeperateLines;
            this.checkBoxRemoveTextBeforeColon.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColon;
            this.checkBoxRemoveTextBeforeColonOnlyUppercase.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyIfUppercase;
            this.checkBoxColonSeparateLine.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyOnSeparateLine;
            this.checkBoxRemoveInterjections.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveInterjections;
            this.checkBoxRemoveWhereContains.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContains;
            this.checkBoxRemoveIfAllUppercase.Checked = Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfAllUppercase;

            this._language = Configuration.Settings.Language.RemoveTextFromHearImpaired;
            this.Text = this._language.Title;
            this.groupBoxRemoveTextConditions.Text = this._language.RemoveTextConditions;
            this.labelAnd.Text = this._language.And;
            this.labelRemoveTextBetween.Text = this._language.RemoveTextBetween;
            this.checkBoxRemoveTextBeforeColon.Text = this._language.RemoveTextBeforeColon;
            this.checkBoxRemoveTextBeforeColonOnlyUppercase.Text = this._language.OnlyIfTextIsUppercase;
            this.checkBoxOnlyIfInSeparateLine.Text = this._language.OnlyIfInSeparateLine;
            this.checkBoxColonSeparateLine.Text = this._language.OnlyIfInSeparateLine;
            this.checkBoxRemoveTextBetweenBrackets.Text = this._language.Brackets;
            this.checkBoxRemoveTextBetweenParentheses.Text = this._language.Parentheses;
            this.checkBoxRemoveTextBetweenQuestionMarks.Text = this._language.QuestionMarks;
            this.checkBoxRemoveTextBetweenSquares.Text = this._language.SquareBrackets;
            this.checkBoxRemoveWhereContains.Text = this._language.RemoveTextIfContains;
            this.checkBoxRemoveIfAllUppercase.Text = this._language.RemoveTextIfAllUppercase;
            this.checkBoxRemoveInterjections.Text = this._language.RemoveInterjections;
            this.buttonEditInterjections.Text = this._language.EditInterjections;
            this.buttonEditInterjections.Left = this.checkBoxRemoveInterjections.Left + this.checkBoxRemoveInterjections.Width;
            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.General.Before;
            this.listViewFixes.Columns[3].Text = Configuration.Settings.Language.General.After;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
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
            if (Environment.OSVersion.Version.Major < 6)
            {
                // 6 == Vista/Win2008Server/Win7
                const string unicodeFontName = Utilities.WinXP2KUnicodeFontName;
                float fontSize = this.comboBoxCustomStart.Font.Size;
                this.comboBoxCustomStart.Font = new Font(unicodeFontName, fontSize);
                this.comboBoxCustomEnd.Font = new Font(unicodeFontName, fontSize);
                this.comboBoxRemoveIfTextContains.Font = new Font(unicodeFontName, fontSize);
            }

            this.comboBoxRemoveIfTextContains.Left = this.checkBoxRemoveWhereContains.Left + this.checkBoxRemoveWhereContains.Width;

            this._subtitle = subtitle;
            this.GeneratePreview();
        }

        /// <summary>
        /// The initialize settings only.
        /// </summary>
        public void InitializeSettingsOnly()
        {
            this.comboBoxRemoveIfTextContains.Left = this.checkBoxRemoveWhereContains.Left + this.checkBoxRemoveWhereContains.Width;
            this.groupBoxLinesFound.Visible = false;
            int h = this.groupBoxRemoveTextConditions.Top + this.groupBoxRemoveTextConditions.Height + this.buttonOK.Height + 50;
            this.MinimumSize = new Size(this.MinimumSize.Width, h);
            this.Height = h;
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

            this._removeTextForHiLib.Settings = this.GetSettings();
            this._removeTextForHiLib.Warnings = new List<int>();
            this.listViewFixes.BeginUpdate();
            this.listViewFixes.Items.Clear();
            int count = 0;
            this._fixes = new Dictionary<Paragraph, string>();
            for (int index = 0; index < this._subtitle.Paragraphs.Count; index++)
            {
                Paragraph p = this._subtitle.Paragraphs[index];
                this._removeTextForHiLib.WarningIndex = index - 1;
                string newText = this._removeTextForHiLib.RemoveTextFromHearImpaired(p.Text);
                if (p.Text.Replace(" ", string.Empty) != newText.Replace(" ", string.Empty))
                {
                    count++;
                    this.AddToListView(p, newText);
                    this._fixes.Add(p, newText);
                }
            }

            this.listViewFixes.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(this._language.LinesFoundX, count);
        }

        /// <summary>
        /// The add to list view.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="newText">
        /// The new text.
        /// </param>
        private void AddToListView(Paragraph p, string newText)
        {
            var item = new ListViewItem(string.Empty) { Tag = p, Checked = true };
            if (this._removeTextForHiLib.Warnings != null && this._removeTextForHiLib.Warnings.Contains(this._removeTextForHiLib.WarningIndex))
            {
                item.UseItemStyleForSubItems = true;
                item.BackColor = Color.PeachPuff;
            }

            item.SubItems.Add(p.Number.ToString(CultureInfo.InvariantCulture));
            item.SubItems.Add(p.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            item.SubItems.Add(newText.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            this.listViewFixes.Items.Add(item);
        }

        /// <summary>
        /// The form remove text for hear impaired_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormRemoveTextForHearImpaired_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#remove_text_for_hi");
            }
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
        /// The remove text from hear impaired.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int RemoveTextFromHearImpaired()
        {
            int count = 0;
            for (int i = this.listViewFixes.Items.Count - 1; i >= 0; i--)
            {
                var item = this.listViewFixes.Items[i];
                if (item.Checked)
                {
                    var p = (Paragraph)item.Tag;
                    string newText = this._fixes[p];
                    if (string.IsNullOrWhiteSpace(newText))
                    {
                        this._subtitle.Paragraphs.Remove(p);
                    }
                    else
                    {
                        p.Text = newText;
                    }

                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// The check box remove text between checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxRemoveTextBetweenCheckedChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The check box remove interjections_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxRemoveInterjections_CheckedChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The button edit interjections_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonEditInterjections_Click(object sender, EventArgs e)
        {
            using (var editInterjections = new Interjections())
            {
                editInterjections.Initialize(Configuration.Settings.Tools.Interjections);
                if (editInterjections.ShowDialog(this) == DialogResult.OK)
                {
                    Configuration.Settings.Tools.Interjections = editInterjections.GetInterjectionsSemiColonSeperatedString();
                    this._removeTextForHiLib.ResetInterjections();
                    if (this.checkBoxRemoveInterjections.Checked)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        this.GeneratePreview();
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        /// <summary>
        /// The form remove text for hear impaired_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormRemoveTextForHearImpaired_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenBrackets = this.checkBoxRemoveTextBetweenSquares.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenParentheses = this.checkBoxRemoveTextBetweenParentheses.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCurlyBrackets = this.checkBoxRemoveTextBetweenBrackets.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenQuestionMarks = this.checkBoxRemoveTextBetweenQuestionMarks.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustom = this.checkBoxRemoveTextBetweenCustomTags.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomBefore = this.comboBoxCustomStart.Text;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomAfter = this.comboBoxCustomEnd.Text;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenOnlySeperateLines = this.checkBoxOnlyIfInSeparateLine.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColon = this.checkBoxRemoveTextBeforeColon.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyIfUppercase = this.checkBoxRemoveTextBeforeColonOnlyUppercase.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyOnSeparateLine = this.checkBoxColonSeparateLine.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveInterjections = this.checkBoxRemoveInterjections.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContains = this.checkBoxRemoveWhereContains.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfAllUppercase = this.checkBoxRemoveIfAllUppercase.Checked;
            Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContainsText = this.comboBoxRemoveIfTextContains.Text;
        }

        /// <summary>
        /// The form remove text for hear impaired_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormRemoveTextForHearImpaired_Resize(object sender, EventArgs e)
        {
            int availableWidth = (this.listViewFixes.Width - (this.columnHeaderApply.Width + this.columnHeaderLine.Width + 20)) / 2;

            this.columnHeaderBefore.Width = availableWidth;
            this.columnHeaderAfter.Width = availableWidth;
        }

        /// <summary>
        /// The check box remove text before colon_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxRemoveTextBeforeColon_CheckedChanged(object sender, EventArgs e)
        {
            this.checkBoxRemoveTextBeforeColonOnlyUppercase.Enabled = this.checkBoxRemoveTextBeforeColon.Checked;
            this.checkBoxColonSeparateLine.Enabled = this.checkBoxRemoveTextBeforeColon.Checked;
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The check box remove if all uppercase_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxRemoveIfAllUppercase_CheckedChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.GeneratePreview();
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The get settings.
        /// </summary>
        /// <returns>
        /// The <see cref="RemoveTextForHISettings"/>.
        /// </returns>
        public RemoveTextForHISettings GetSettings()
        {
            var settings = new RemoveTextForHISettings { OnlyIfInSeparateLine = this.checkBoxOnlyIfInSeparateLine.Checked, RemoveIfAllUppercase = this.checkBoxRemoveIfAllUppercase.Checked, RemoveTextBeforeColon = this.checkBoxRemoveTextBeforeColon.Checked, RemoveTextBeforeColonOnlyUppercase = this.checkBoxRemoveTextBeforeColonOnlyUppercase.Checked, ColonSeparateLine = this.checkBoxColonSeparateLine.Checked, RemoveWhereContains = this.checkBoxRemoveWhereContains.Checked, RemoveIfTextContains = new List<string>(), RemoveTextBetweenCustomTags = this.checkBoxRemoveTextBetweenCustomTags.Checked, RemoveInterjections = this.checkBoxRemoveInterjections.Checked, RemoveTextBetweenSquares = this.checkBoxRemoveTextBetweenSquares.Checked, RemoveTextBetweenBrackets = this.checkBoxRemoveTextBetweenBrackets.Checked, RemoveTextBetweenQuestionMarks = this.checkBoxRemoveTextBetweenQuestionMarks.Checked, RemoveTextBetweenParentheses = this.checkBoxRemoveTextBetweenParentheses.Checked, CustomStart = this.comboBoxCustomStart.Text, CustomEnd = this.comboBoxCustomEnd.Text };
            foreach (string item in this.comboBoxRemoveIfTextContains.Text.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                settings.RemoveIfTextContains.Add(item.Trim());
            }

            return settings;
        }

        /// <summary>
        /// The form remove text for hear impaired_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormRemoveTextForHearImpaired_Load(object sender, EventArgs e)
        {
            // only works when used from "Form Load"
            this.comboBoxCustomStart.Text = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomBefore;
            this.comboBoxCustomEnd.Text = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomAfter;
            this.comboBoxRemoveIfTextContains.Text = Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContainsText;
        }
    }
}