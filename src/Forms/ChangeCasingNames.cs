// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeCasingNames.cs" company="">
//   
// </copyright>
// <summary>
//   The change casing names.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Dictionaries;

    /// <summary>
    /// The change casing names.
    /// </summary>
    public sealed partial class ChangeCasingNames : Form
    {
        /// <summary>
        /// The _used names.
        /// </summary>
        private readonly List<string> _usedNames = new List<string>();

        /// <summary>
        /// The _no of lines changed.
        /// </summary>
        private int _noOfLinesChanged;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeCasingNames"/> class.
        /// </summary>
        public ChangeCasingNames()
        {
            this.InitializeComponent();
            this.labelXLinesSelected.Text = string.Empty;
            this.Text = Configuration.Settings.Language.ChangeCasingNames.Title;
            this.groupBoxNames.Text = string.Empty;
            this.listViewNames.Columns[0].Text = Configuration.Settings.Language.ChangeCasingNames.Enabled;
            this.listViewNames.Columns[1].Text = Configuration.Settings.Language.ChangeCasingNames.Name;
            this.groupBoxLinesFound.Text = string.Empty;
            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.General.Before;
            this.listViewFixes.Columns[3].Text = Configuration.Settings.Language.General.After;

            this.buttonSelectAll.Text = Configuration.Settings.Language.FixCommonErrors.SelectAll;
            this.buttonInverseSelection.Text = Configuration.Settings.Language.FixCommonErrors.InverseSelection;

            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.listViewFixes.Resize += delegate
                {
                    var width = (listViewFixes.Width - (listViewFixes.Columns[0].Width + listViewFixes.Columns[1].Width)) / 2;
                    listViewFixes.Columns[2].Width = width;
                    listViewFixes.Columns[3].Width = width;
                };
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the lines changed.
        /// </summary>
        public int LinesChanged
        {
            get
            {
                return this._noOfLinesChanged;
            }
        }

        /// <summary>
        /// The change casing names_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChangeCasingNames_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The add to list view names.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        private void AddToListViewNames(string name)
        {
            var item = new ListViewItem(string.Empty) { Checked = true };
            item.SubItems.Add(name);
            this.listViewNames.Items.Add(item);
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

            this.FindAllNames();
            this.GeneratePreview();
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            this.Cursor = Cursors.WaitCursor;
            this.listViewFixes.BeginUpdate();
            this.listViewFixes.Items.Clear();
            foreach (Paragraph p in this._subtitle.Paragraphs)
            {
                string text = p.Text;
                foreach (ListViewItem item in this.listViewNames.Items)
                {
                    string name = item.SubItems[1].Text;

                    string textNoTags = HtmlUtil.RemoveHtmlTags(text);
                    if (textNoTags != textNoTags.ToUpper())
                    {
                        if (item.Checked && text != null && text.Contains(name, StringComparison.OrdinalIgnoreCase) && name.Length > 1 && name != name.ToLower())
                        {
                            var st = new StripableText(text);
                            st.FixCasing(new List<string> { name }, true, false, false, string.Empty);
                            text = st.MergedString;
                        }
                    }
                }

                if (text != p.Text)
                {
                    this.AddToPreviewListView(p, text);
                }
            }

            this.listViewFixes.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.ChangeCasingNames.LinesFoundX, this.listViewFixes.Items.Count);
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// The add to preview list view.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="newText">
        /// The new text.
        /// </param>
        private void AddToPreviewListView(Paragraph p, string newText)
        {
            var item = new ListViewItem(string.Empty) { Tag = p, Checked = true };
            item.SubItems.Add(p.Number.ToString(CultureInfo.InvariantCulture));
            item.SubItems.Add(p.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            item.SubItems.Add(newText.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            this.listViewFixes.Items.Add(item);
        }

        /// <summary>
        /// The find all names.
        /// </summary>
        private void FindAllNames()
        {
            string language = Utilities.AutoDetectLanguageName("en_US", this._subtitle);
            if (string.IsNullOrEmpty(language))
            {
                language = "en_US";
            }

            var namesList = new NamesList(Configuration.DictionariesFolder, language, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);

            // Will contains both one word names and multi names
            var namesEtcList = namesList.GetAllNames();

            if (language.StartsWith("en", StringComparison.Ordinal))
            {
                namesEtcList.Remove("Black");
                namesEtcList.Remove("Bill");
                namesEtcList.Remove("Long");
                namesEtcList.Remove("Don");
            }

            var sb = new StringBuilder();
            foreach (Paragraph p in this._subtitle.Paragraphs)
            {
                sb.AppendLine(p.Text);
            }

            string text = HtmlUtil.RemoveHtmlTags(sb.ToString());
            string textToLower = text.ToLower();
            foreach (string name in namesEtcList)
            {
                int startIndex = textToLower.IndexOf(name.ToLower(), StringComparison.Ordinal);
                if (startIndex >= 0)
                {
                    while (startIndex >= 0 && startIndex < text.Length && textToLower.Substring(startIndex).Contains(name.ToLower()) && name.Length > 1 && name != name.ToLower())
                    {
                        bool startOk = (startIndex == 0) || (text[startIndex - 1] == ' ') || (text[startIndex - 1] == '-') || (text[startIndex - 1] == '"') || (text[startIndex - 1] == '\'') || (text[startIndex - 1] == '>') || Environment.NewLine.EndsWith(text[startIndex - 1].ToString(CultureInfo.InvariantCulture));

                        if (startOk)
                        {
                            int end = startIndex + name.Length;
                            bool endOk = end <= text.Length;
                            if (endOk)
                            {
                                endOk = end == text.Length || (@" ,.!?:;')-<""" + Environment.NewLine).Contains(text[end]);
                            }

                            if (endOk && text.Substring(startIndex, name.Length) != name)
                            {
                                // do not add names where casing already is correct
                                if (!this._usedNames.Contains(name))
                                {
                                    this._usedNames.Add(name);
                                    this.AddToListViewNames(name);
                                    break; // break while
                                }
                            }
                        }

                        startIndex = textToLower.IndexOf(name.ToLower(), startIndex + 2, StringComparison.Ordinal);
                    }
                }
            }

            this.groupBoxNames.Text = string.Format(Configuration.Settings.Language.ChangeCasingNames.NamesFoundInSubtitleX, this.listViewNames.Items.Count);
        }

        /// <summary>
        /// The list view names selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListViewNamesSelectedIndexChanged(object sender, EventArgs e)
        {
            this.labelXLinesSelected.Text = string.Empty;
            if (this.listViewNames.SelectedItems.Count != 1)
            {
                return;
            }

            string name = this.listViewNames.SelectedItems[0].SubItems[1].Text;
            this.listViewFixes.BeginUpdate();
            foreach (ListViewItem item in this.listViewFixes.Items)
            {
                item.Selected = false;

                string text = item.SubItems[2].Text.Replace(Configuration.Settings.General.ListViewLineSeparatorString, Environment.NewLine);

                string lower = text.ToLower();
                if (lower.Contains(name.ToLower()) && name.Length > 1 && name != name.ToLower())
                {
                    int start = lower.IndexOf(name.ToLower(), StringComparison.Ordinal);
                    if (start >= 0)
                    {
                        bool startOk = (start == 0) || (lower[start - 1] == ' ') || (lower[start - 1] == '-') || (lower[start - 1] == '"') || lower[start - 1] == '\'' || lower[start - 1] == '>' || Environment.NewLine.EndsWith(lower[start - 1]);

                        if (startOk)
                        {
                            int end = start + name.Length;
                            bool endOk = end <= lower.Length;
                            if (endOk)
                            {
                                endOk = end == lower.Length || (@" ,.!?:;')<-""" + Environment.NewLine).Contains(lower[end]);
                            }

                            item.Selected = endOk;
                        }
                    }
                }
            }

            this.listViewFixes.EndUpdate();

            if (this.listViewFixes.SelectedItems.Count > 0)
            {
                this.listViewFixes.EnsureVisible(this.listViewFixes.SelectedItems[0].Index);
            }
        }

        /// <summary>
        /// The list view names item checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListViewNamesItemChecked(object sender, ItemCheckedEventArgs e)
        {
            this.GeneratePreview();
        }

        /// <summary>
        /// The change casing names_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ChangeCasingNames_Shown(object sender, EventArgs e)
        {
            this.listViewNames.ItemChecked += this.ListViewNamesItemChecked;
        }

        /// <summary>
        /// The fix casing.
        /// </summary>
        internal void FixCasing()
        {
            foreach (ListViewItem item in this.listViewFixes.Items)
            {
                if (item.Checked)
                {
                    this._noOfLinesChanged++;
                    var p = item.Tag as Paragraph;
                    if (p != null)
                    {
                        p.Text = item.SubItems[3].Text.Replace(Configuration.Settings.General.ListViewLineSeparatorString, Environment.NewLine);
                    }
                }
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
            if (this.listViewFixes.SelectedItems.Count > 1)
            {
                this.labelXLinesSelected.Text = string.Format(Configuration.Settings.Language.Main.XLinesSelected, this.listViewFixes.SelectedItems.Count);
            }
            else
            {
                this.labelXLinesSelected.Text = string.Empty;
            }
        }

        /// <summary>
        /// The button select all_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            this.DoSelection(true);
        }

        /// <summary>
        /// The button inverse selection_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonInverseSelection_Click(object sender, EventArgs e)
        {
            this.DoSelection(false);
        }

        /// <summary>
        /// The do selection.
        /// </summary>
        /// <param name="selectAll">
        /// The select all.
        /// </param>
        private void DoSelection(bool selectAll)
        {
            this.listViewNames.ItemChecked -= this.ListViewNamesItemChecked;
            this.listViewNames.BeginUpdate();
            foreach (ListViewItem item in this.listViewNames.Items)
            {
                if (selectAll)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = !item.Checked;
                }
            }

            this.listViewNames.EndUpdate();
            this.listViewNames.ItemChecked += this.ListViewNamesItemChecked;
            this.GeneratePreview();
        }
    }
}