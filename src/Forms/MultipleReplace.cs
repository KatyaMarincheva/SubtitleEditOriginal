// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleReplace.cs" company="">
//   
// </copyright>
// <summary>
//   The multiple replace.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The multiple replace.
    /// </summary>
    public sealed partial class MultipleReplace : PositionAndSizeForm
    {
        /// <summary>
        /// The search type normal.
        /// </summary>
        public const string SearchTypeNormal = "Normal";

        /// <summary>
        /// The search type case sensitive.
        /// </summary>
        public const string SearchTypeCaseSensitive = "CaseSensitive";

        /// <summary>
        /// The search type regular expression.
        /// </summary>
        public const string SearchTypeRegularExpression = "RegularExpression";

        /// <summary>
        /// The _compiled reg ex list.
        /// </summary>
        private readonly Dictionary<string, Regex> _compiledRegExList = new Dictionary<string, Regex>();

        /// <summary>
        /// The _old multiple search and replace list.
        /// </summary>
        private readonly List<MultipleSearchAndReplaceSetting> _oldMultipleSearchAndReplaceList = new List<MultipleSearchAndReplaceSetting>();

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleReplace"/> class.
        /// </summary>
        public MultipleReplace()
        {
            this.InitializeComponent();

            this.openFileDialog1.FileName = string.Empty;
            this.saveFileDialog1.FileName = string.Empty;

            this.textBoxReplace.ContextMenu = FindReplaceDialogHelper.GetReplaceTextContextMenu(this.textBoxReplace);
            this.buttonUpdate.Enabled = false;

            this.Text = Configuration.Settings.Language.MultipleReplace.Title;
            this.labelFindWhat.Text = Configuration.Settings.Language.MultipleReplace.FindWhat;
            this.labelReplaceWith.Text = Configuration.Settings.Language.MultipleReplace.ReplaceWith;
            this.radioButtonNormal.Text = Configuration.Settings.Language.MultipleReplace.Normal;
            this.radioButtonRegEx.Text = Configuration.Settings.Language.MultipleReplace.RegularExpression;
            this.radioButtonCaseSensitive.Text = Configuration.Settings.Language.MultipleReplace.CaseSensitive;
            this.buttonAdd.Text = Configuration.Settings.Language.MultipleReplace.Add;
            this.buttonUpdate.Text = Configuration.Settings.Language.MultipleReplace.Update;
            this.listViewReplaceList.Columns[0].Text = Configuration.Settings.Language.MultipleReplace.Enabled;
            this.listViewReplaceList.Columns[1].Text = Configuration.Settings.Language.MultipleReplace.FindWhat;
            this.listViewReplaceList.Columns[2].Text = Configuration.Settings.Language.MultipleReplace.ReplaceWith;
            this.listViewReplaceList.Columns[3].Text = Configuration.Settings.Language.MultipleReplace.SearchType;
            this.groupBoxLinesFound.Text = string.Empty;
            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.General.Before;
            this.listViewFixes.Columns[3].Text = Configuration.Settings.Language.General.After;
            this.deleteToolStripMenuItem.Text = Configuration.Settings.Language.MultipleReplace.Delete;
            this.buttonRemoveAll.Text = Configuration.Settings.Language.MultipleReplace.RemoveAll;
            this.buttonImport.Text = Configuration.Settings.Language.MultipleReplace.Import;
            this.buttonExport.Text = Configuration.Settings.Language.MultipleReplace.Export;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonReplacesSelectAll.Text = Configuration.Settings.Language.FixCommonErrors.SelectAll;
            this.buttonReplacesInverseSelection.Text = Configuration.Settings.Language.FixCommonErrors.InverseSelection;
            Utilities.FixLargeFonts(this, this.buttonOK);
            this.splitContainer1.Panel1MinSize = 200;
            this.splitContainer1.Panel2MinSize = 200;

            this.moveUpToolStripMenuItem.Text = Configuration.Settings.Language.DvdSubRip.MoveUp;
            this.moveDownToolStripMenuItem.Text = Configuration.Settings.Language.DvdSubRip.MoveDown;
            this.moveTopToolStripMenuItem.Text = Configuration.Settings.Language.MultipleReplace.MoveToTop;
            this.moveBottomToolStripMenuItem.Text = Configuration.Settings.Language.MultipleReplace.MoveToBottom;

            this.radioButtonCaseSensitive.Left = this.radioButtonNormal.Left + this.radioButtonNormal.Width + 40;
            this.radioButtonRegEx.Left = this.radioButtonCaseSensitive.Left + this.radioButtonCaseSensitive.Width + 40;
        }

        /// <summary>
        /// Gets the delete indices.
        /// </summary>
        public List<int> DeleteIndices { get; private set; }

        /// <summary>
        /// Gets the fixed subtitle.
        /// </summary>
        public Subtitle FixedSubtitle { get; private set; }

        /// <summary>
        /// Gets the fix count.
        /// </summary>
        public int FixCount { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public void Initialize(Subtitle subtitle)
        {
            if (subtitle == null)
            {
                throw new ArgumentNullException("subtitle");
            }

            this._subtitle = subtitle;
            foreach (var item in Configuration.Settings.MultipleSearchAndReplaceList)
            {
                this.AddToReplaceListView(item.Enabled, item.FindWhat, item.ReplaceWith, EnglishSearchTypeToLocal(item.SearchType));
                this._oldMultipleSearchAndReplaceList.Add(item);
            }

            if (subtitle.Paragraphs == null || subtitle.Paragraphs.Count == 0)
            {
                this.groupBoxLinesFound.Enabled = false;
            }
        }

        /// <summary>
        /// The multiple replace_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MultipleReplace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.buttonCancel_Click(null, null);
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#multiple_replace");
            }
        }

        /// <summary>
        /// The radio button checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButtonCheckedChanged(object sender, EventArgs e)
        {
            if (sender == this.radioButtonRegEx)
            {
                this.textBoxFind.ContextMenu = FindReplaceDialogHelper.GetRegExContextMenu(this.textBoxFind);
            }
            else
            {
                this.textBoxFind.ContextMenu = null;
            }
        }

        /// <summary>
        /// The button add click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAddClick(object sender, EventArgs e)
        {
            if (this.textBoxFind.Text.Length > 0)
            {
                string searchType = SearchTypeNormal;
                if (this.radioButtonCaseSensitive.Checked)
                {
                    searchType = SearchTypeCaseSensitive;
                }
                else if (this.radioButtonRegEx.Checked)
                {
                    searchType = SearchTypeRegularExpression;
                    if (!Utilities.IsValidRegex(this.textBoxFind.Text))
                    {
                        MessageBox.Show(Configuration.Settings.Language.General.RegularExpressionIsNotValid);
                        this.textBoxFind.Select();
                        return;
                    }
                }

                this.AddToReplaceListView(true, this.textBoxFind.Text, this.textBoxReplace.Text, EnglishSearchTypeToLocal(searchType));
                this.textBoxFind.Text = string.Empty;
                this.textBoxReplace.Text = string.Empty;
                this.GeneratePreview();
                this.textBoxFind.Select();
                this.SaveReplaceList(false);
            }
        }

        /// <summary>
        /// The run from batch.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        internal void RunFromBatch(Subtitle subtitle)
        {
            this.Initialize(subtitle);
            this.GeneratePreview();
        }

        /// <summary>
        /// The generate preview.
        /// </summary>
        private void GeneratePreview()
        {
            this.Cursor = Cursors.WaitCursor;
            this.FixedSubtitle = new Subtitle(this._subtitle);
            this.DeleteIndices = new List<int>();
            this.FixCount = 0;
            this.listViewFixes.BeginUpdate();
            this.listViewFixes.Items.Clear();
            var replaceExpressions = new HashSet<ReplaceExpression>();
            foreach (ListViewItem item in this.listViewReplaceList.Items)
            {
                if (item.Checked)
                {
                    string findWhat = item.SubItems[1].Text;
                    if (!string.IsNullOrWhiteSpace(findWhat))
                    {
                        string replaceWith = item.SubItems[2].Text.Replace(@"\n", Environment.NewLine);
                        string searchType = item.SubItems[3].Text;
                        var mpi = new ReplaceExpression(findWhat, replaceWith, searchType);
                        replaceExpressions.Add(mpi);
                        if (mpi.SearchType == ReplaceExpression.SearchRegEx && !this._compiledRegExList.ContainsKey(findWhat))
                        {
                            this._compiledRegExList.Add(findWhat, new Regex(findWhat, RegexOptions.Compiled | RegexOptions.Multiline));
                        }
                    }
                }
            }

            foreach (Paragraph p in this._subtitle.Paragraphs)
            {
                bool hit = false;
                string newText = p.Text;
                foreach (ReplaceExpression item in replaceExpressions)
                {
                    if (item.SearchType == ReplaceExpression.SearchCaseSensitive)
                    {
                        if (newText.Contains(item.FindWhat))
                        {
                            hit = true;
                            newText = newText.Replace(item.FindWhat, item.ReplaceWith);
                        }
                    }
                    else if (item.SearchType == ReplaceExpression.SearchRegEx)
                    {
                        Regex r = this._compiledRegExList[item.FindWhat];
                        if (r.IsMatch(newText))
                        {
                            hit = true;
                            newText = r.Replace(newText, item.ReplaceWith);
                        }
                    }
                    else
                    {
                        int index = newText.IndexOf(item.FindWhat, StringComparison.OrdinalIgnoreCase);
                        if (index >= 0)
                        {
                            hit = true;
                            do
                            {
                                newText = newText.Remove(index, item.FindWhat.Length).Insert(index, item.ReplaceWith);
                                index = newText.IndexOf(item.FindWhat, index + item.ReplaceWith.Length, StringComparison.OrdinalIgnoreCase);
                            }
                            while (index >= 0);
                        }
                    }
                }

                if (hit && newText != p.Text)
                {
                    this.FixCount++;
                    this.AddToPreviewListView(p, newText);
                    int index = this._subtitle.GetIndex(p);
                    this.FixedSubtitle.Paragraphs[index].Text = newText;
                    if (!string.IsNullOrWhiteSpace(p.Text) && (string.IsNullOrWhiteSpace(newText) || string.IsNullOrWhiteSpace(HtmlUtil.RemoveHtmlTags(newText, true))))
                    {
                        this.DeleteIndices.Add(index);
                    }
                }
            }

            this.listViewFixes.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.MultipleReplace.LinesFoundX, this.FixCount);
            this.Cursor = Cursors.Default;
            this.DeleteIndices.Reverse();
        }

        /// <summary>
        /// The add to replace list view.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        /// <param name="findWhat">
        /// The find what.
        /// </param>
        /// <param name="replaceWith">
        /// The replace with.
        /// </param>
        /// <param name="searchType">
        /// The search type.
        /// </param>
        private void AddToReplaceListView(bool enabled, string findWhat, string replaceWith, string searchType)
        {
            var item = new ListViewItem(string.Empty) { Checked = enabled };

            var subItem = new ListViewItem.ListViewSubItem(item, findWhat);
            item.SubItems.Add(subItem);
            subItem = new ListViewItem.ListViewSubItem(item, replaceWith);
            item.SubItems.Add(subItem);
            subItem = new ListViewItem.ListViewSubItem(item, searchType);
            item.SubItems.Add(subItem);

            this.listViewReplaceList.Items.Add(item);
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
        /// The local search type to english.
        /// </summary>
        /// <param name="searchType">
        /// The search type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string LocalSearchTypeToEnglish(string searchType)
        {
            if (searchType == Configuration.Settings.Language.MultipleReplace.RegularExpression)
            {
                return SearchTypeRegularExpression;
            }

            if (searchType == Configuration.Settings.Language.MultipleReplace.CaseSensitive)
            {
                return SearchTypeCaseSensitive;
            }

            return SearchTypeNormal;
        }

        /// <summary>
        /// The english search type to local.
        /// </summary>
        /// <param name="searchType">
        /// The search type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string EnglishSearchTypeToLocal(string searchType)
        {
            if (searchType == SearchTypeRegularExpression)
            {
                return Configuration.Settings.Language.MultipleReplace.RegularExpression;
            }

            if (searchType == SearchTypeCaseSensitive)
            {
                return Configuration.Settings.Language.MultipleReplace.CaseSensitive;
            }

            return Configuration.Settings.Language.MultipleReplace.Normal;
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
            this.ResetUncheckLines();
            this.SaveReplaceList(true);
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The save replace list.
        /// </summary>
        /// <param name="saveToDisk">
        /// The save to disk.
        /// </param>
        private void SaveReplaceList(bool saveToDisk)
        {
            Configuration.Settings.MultipleSearchAndReplaceList = new List<MultipleSearchAndReplaceSetting>();
            foreach (ListViewItem item in this.listViewReplaceList.Items)
            {
                Configuration.Settings.MultipleSearchAndReplaceList.Add(new MultipleSearchAndReplaceSetting { Enabled = item.Checked, FindWhat = item.SubItems[1].Text, ReplaceWith = item.SubItems[2].Text, SearchType = LocalSearchTypeToEnglish(item.SubItems[3].Text) });
            }

            if (saveToDisk)
            {
                Configuration.Settings.Save();
            }
        }

        /// <summary>
        /// The reset uncheck lines.
        /// </summary>
        private void ResetUncheckLines()
        {
            foreach (ListViewItem item in this.listViewFixes.Items)
            {
                if (!item.Checked)
                {
                    int index = this._subtitle.GetIndex(item.Tag as Paragraph);
                    this.FixedSubtitle.Paragraphs[index].Text = this._subtitle.Paragraphs[index].Text;
                }
            }
        }

        /// <summary>
        /// The list view replace list item checked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListViewReplaceListItemChecked(object sender, ItemCheckedEventArgs e)
        {
            this.GeneratePreview();
            this.SaveReplaceList(false);
        }

        /// <summary>
        /// The delete tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.listViewReplaceList.Items.Count < 1 || this.listViewReplaceList.SelectedItems.Count < 1)
            {
                return;
            }

            for (int i = this.listViewReplaceList.Items.Count - 1; i >= 0; i--)
            {
                ListViewItem item = this.listViewReplaceList.Items[i];
                if (item.Selected)
                {
                    item.Remove();
                }
            }

            this.GeneratePreview();
            this.SaveReplaceList(false);
        }

        /// <summary>
        /// The list view replace list key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListViewReplaceListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                this.DeleteToolStripMenuItemClick(null, null);
            }

            if (this.listViewReplaceList.SelectedItems.Count == 1)
            {
                if (e.KeyCode == Keys.Up && e.Control && !e.Alt && !e.Shift)
                {
                    this.moveUpToolStripMenuItem_Click(sender, e);
                }

                if (e.KeyCode == Keys.Down && e.Control && !e.Alt && !e.Shift)
                {
                    this.moveDownToolStripMenuItem_Click(sender, e);
                }

                if (e.KeyData == (Keys.Control | Keys.Home))
                {
                    this.moveTopToolStripMenuItem_Click(sender, e);
                }
                else if (e.KeyData == (Keys.Control | Keys.End))
                {
                    this.moveBottomToolStripMenuItem_Click(sender, e);
                }
            }
        }

        /// <summary>
        /// The button update click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonUpdateClick(object sender, EventArgs e)
        {
            if (this.listViewReplaceList.SelectedItems.Count != 1)
            {
                return;
            }

            if (this.textBoxFind.Text.Length > 0)
            {
                string searchType = SearchTypeNormal;
                if (this.radioButtonCaseSensitive.Checked)
                {
                    searchType = SearchTypeCaseSensitive;
                }
                else if (this.radioButtonRegEx.Checked)
                {
                    searchType = SearchTypeRegularExpression;
                    if (!Utilities.IsValidRegex(this.textBoxFind.Text))
                    {
                        MessageBox.Show(Configuration.Settings.Language.General.RegularExpressionIsNotValid);
                        this.textBoxFind.Select();
                        return;
                    }
                }

                var item = this.listViewReplaceList.SelectedItems[0];
                item.SubItems[1].Text = this.textBoxFind.Text;
                item.SubItems[2].Text = this.textBoxReplace.Text;
                item.SubItems[3].Text = EnglishSearchTypeToLocal(searchType);

                this.GeneratePreview();
                this.textBoxFind.Select();
                this.SaveReplaceList(false);
            }
        }

        /// <summary>
        /// The list view replace list selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListViewReplaceListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewReplaceList.SelectedItems.Count == 1)
            {
                this.buttonUpdate.Enabled = true;
                this.textBoxFind.Text = this.listViewReplaceList.SelectedItems[0].SubItems[1].Text;
                this.textBoxReplace.Text = this.listViewReplaceList.SelectedItems[0].SubItems[2].Text;
                string searchType = LocalSearchTypeToEnglish(this.listViewReplaceList.SelectedItems[0].SubItems[3].Text);
                if (searchType == SearchTypeRegularExpression)
                {
                    this.radioButtonRegEx.Checked = true;
                }
                else if (searchType == SearchTypeCaseSensitive)
                {
                    this.radioButtonCaseSensitive.Checked = true;
                }
                else
                {
                    this.radioButtonNormal.Checked = true;
                }
            }
            else
            {
                this.buttonUpdate.Enabled = false;
            }
        }

        /// <summary>
        /// The text box replace key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxReplaceKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonAddClick(null, null);
            }
        }

        /// <summary>
        /// The button replaces select all_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonReplacesSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listViewFixes.Items)
            {
                item.Checked = true;
            }
        }

        /// <summary>
        /// The button replaces inverse selection_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonReplacesInverseSelection_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listViewFixes.Items)
            {
                item.Checked = !item.Checked;
            }
        }

        /// <summary>
        /// The context menu strip 1_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            this.moveUpToolStripMenuItem.Visible = this.listViewReplaceList.Items.Count > 1 && this.listViewReplaceList.SelectedItems.Count == 1;
            this.moveDownToolStripMenuItem.Visible = this.listViewReplaceList.Items.Count > 1 && this.listViewReplaceList.SelectedItems.Count == 1;
            this.moveTopToolStripMenuItem.Visible = this.listViewReplaceList.Items.Count > 1 && this.listViewReplaceList.SelectedItems.Count == 1;
            this.moveBottomToolStripMenuItem.Visible = this.listViewReplaceList.Items.Count > 1 && this.listViewReplaceList.SelectedItems.Count == 1;
        }

        /// <summary>
        /// The move up tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = this.listViewReplaceList.SelectedIndices[0];
            if (index == 0)
            {
                return;
            }

            this.SwapReplaceList(index, index - 1);
        }

        /// <summary>
        /// The swap replace list.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="index2">
        /// The index 2.
        /// </param>
        private void SwapReplaceList(int index, int index2)
        {
            bool enabled = this.listViewReplaceList.Items[index].Checked;
            string findWhat = this.listViewReplaceList.Items[index].SubItems[1].Text;
            string replaceWith = this.listViewReplaceList.Items[index].SubItems[2].Text;
            string searchType = this.listViewReplaceList.Items[index].SubItems[3].Text;

            this.listViewReplaceList.Items[index].Checked = this.listViewReplaceList.Items[index2].Checked;
            this.listViewReplaceList.Items[index].SubItems[1].Text = this.listViewReplaceList.Items[index2].SubItems[1].Text;
            this.listViewReplaceList.Items[index].SubItems[2].Text = this.listViewReplaceList.Items[index2].SubItems[2].Text;
            this.listViewReplaceList.Items[index].SubItems[3].Text = this.listViewReplaceList.Items[index2].SubItems[3].Text;

            this.listViewReplaceList.Items[index2].Checked = enabled;
            this.listViewReplaceList.Items[index2].SubItems[1].Text = findWhat;
            this.listViewReplaceList.Items[index2].SubItems[2].Text = replaceWith;
            this.listViewReplaceList.Items[index2].SubItems[3].Text = searchType;

            this.listViewReplaceList.Items[index].Selected = false;
            this.listViewReplaceList.Items[index2].Selected = true;
            this.SaveReplaceList(false);
            this.GeneratePreview();
        }

        /// <summary>
        /// The move down tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = this.listViewReplaceList.SelectedIndices[0];
            if (index == this.listViewReplaceList.Items.Count - 1)
            {
                return;
            }

            this.SwapReplaceList(index, index + 1);
        }

        /// <summary>
        /// The move top tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void moveTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = this.listViewReplaceList.SelectedIndices[0];
            if (index == 0)
            {
                return;
            }

            var item = this.listViewReplaceList.Items[index];
            this.listViewReplaceList.Items.RemoveAt(index);
            this.listViewReplaceList.Items.Insert(0, item);
            this.GeneratePreview();
            this.SaveReplaceList(false);
        }

        /// <summary>
        /// The move bottom tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void moveBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = this.listViewReplaceList.SelectedIndices[0];
            int bottomIndex = this.listViewReplaceList.Items.Count - 1;
            if (index == bottomIndex)
            {
                return;
            }

            var item = this.listViewReplaceList.Items[index];
            this.listViewReplaceList.Items.RemoveAt(index);
            this.listViewReplaceList.Items.Add(item);
            this.GeneratePreview();
            this.SaveReplaceList(false);
        }

        /// <summary>
        /// The export click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ExportClick(object sender, EventArgs e)
        {
            if (this.listViewReplaceList.Items.Count == 0)
            {
                return;
            }

            this.saveFileDialog1.Title = Configuration.Settings.Language.MultipleReplace.ExportRulesTitle;
            this.saveFileDialog1.Filter = Configuration.Settings.Language.MultipleReplace.Rules + "|*.template";
            if (this.saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                this.SaveReplaceList(false);

                var textWriter = new XmlTextWriter(this.saveFileDialog1.FileName, null) { Formatting = Formatting.Indented };
                textWriter.WriteStartDocument();
                textWriter.WriteStartElement("Settings", string.Empty);
                textWriter.WriteStartElement("MultipleSearchAndReplaceList", string.Empty);
                foreach (var item in Configuration.Settings.MultipleSearchAndReplaceList)
                {
                    textWriter.WriteStartElement("MultipleSearchAndReplaceItem", string.Empty);
                    textWriter.WriteElementString("Enabled", item.Enabled.ToString());
                    textWriter.WriteElementString("FindWhat", item.FindWhat);
                    textWriter.WriteElementString("ReplaceWith", item.ReplaceWith);
                    textWriter.WriteElementString("SearchType", item.SearchType);
                    textWriter.WriteEndElement();
                }

                textWriter.WriteEndElement();
                textWriter.WriteEndElement();
                textWriter.WriteEndDocument();
                textWriter.Close();
            }
        }

        /// <summary>
        /// The button open_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = Configuration.Settings.Language.MultipleReplace.ImportRulesTitle;
            this.openFileDialog1.Filter = Configuration.Settings.Language.MultipleReplace.Rules + "|*.template";
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var doc = new XmlDocument();
                try
                {
                    doc.Load(this.openFileDialog1.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                    return;
                }

                foreach (XmlNode listNode in doc.DocumentElement.SelectNodes("MultipleSearchAndReplaceList/MultipleSearchAndReplaceItem"))
                {
                    var item = new MultipleSearchAndReplaceSetting();
                    var subNode = listNode.SelectSingleNode("Enabled");
                    if (subNode != null)
                    {
                        item.Enabled = Convert.ToBoolean(subNode.InnerText);
                    }

                    subNode = listNode.SelectSingleNode("FindWhat");
                    if (subNode != null)
                    {
                        item.FindWhat = subNode.InnerText;
                    }

                    subNode = listNode.SelectSingleNode("ReplaceWith");
                    if (subNode != null)
                    {
                        item.ReplaceWith = subNode.InnerText;
                    }

                    subNode = listNode.SelectSingleNode("SearchType");
                    if (subNode != null)
                    {
                        item.SearchType = subNode.InnerText;
                    }

                    Configuration.Settings.MultipleSearchAndReplaceList.Add(item);
                }

                this.listViewReplaceList.ItemChecked -= this.ListViewReplaceListItemChecked;
                this.listViewReplaceList.BeginUpdate();
                this.listViewReplaceList.Items.Clear();
                foreach (var item in Configuration.Settings.MultipleSearchAndReplaceList)
                {
                    this.AddToReplaceListView(item.Enabled, item.FindWhat, item.ReplaceWith, EnglishSearchTypeToLocal(item.SearchType));
                }

                this.GeneratePreview();
                this.listViewReplaceList.ItemChecked += this.ListViewReplaceListItemChecked;
                this.listViewReplaceList.EndUpdate();
            }
        }

        /// <summary>
        /// The button remove all_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonRemoveAll_Click(object sender, EventArgs e)
        {
            this.listViewReplaceList.Items.Clear();
            Configuration.Settings.MultipleSearchAndReplaceList.Clear();
        }

        /// <summary>
        /// The multiple replace_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MultipleReplace_Shown(object sender, EventArgs e)
        {
            this.listViewReplaceList.ItemChecked += this.ListViewReplaceListItemChecked;
            this.GeneratePreview();
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
            Configuration.Settings.MultipleSearchAndReplaceList.Clear();
            foreach (var item in this._oldMultipleSearchAndReplaceList)
            {
                Configuration.Settings.MultipleSearchAndReplaceList.Add(item);
            }

            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The replace expression.
        /// </summary>
        internal class ReplaceExpression
        {
            /// <summary>
            /// The search normal.
            /// </summary>
            internal const int SearchNormal = 0;

            /// <summary>
            /// The search reg ex.
            /// </summary>
            internal const int SearchRegEx = 1;

            /// <summary>
            /// The search case sensitive.
            /// </summary>
            internal const int SearchCaseSensitive = 2;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReplaceExpression"/> class.
            /// </summary>
            /// <param name="findWhat">
            /// The find what.
            /// </param>
            /// <param name="replaceWith">
            /// The replace with.
            /// </param>
            /// <param name="searchType">
            /// The search type.
            /// </param>
            internal ReplaceExpression(string findWhat, string replaceWith, string searchType)
            {
                this.FindWhat = findWhat;
                this.ReplaceWith = replaceWith;
                if (string.CompareOrdinal(searchType, Configuration.Settings.Language.MultipleReplace.RegularExpression) == 0)
                {
                    this.SearchType = SearchRegEx;
                }
                else if (string.CompareOrdinal(searchType, Configuration.Settings.Language.MultipleReplace.CaseSensitive) == 0)
                {
                    this.SearchType = SearchCaseSensitive;
                }
            }

            /// <summary>
            /// Gets or sets the find what.
            /// </summary>
            internal string FindWhat { get; set; }

            /// <summary>
            /// Gets or sets the replace with.
            /// </summary>
            internal string ReplaceWith { get; set; }

            /// <summary>
            /// Gets or sets the search type.
            /// </summary>
            internal int SearchType { get; set; }
        }
    }
}