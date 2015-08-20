// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifySelection.cs" company="">
//   
// </copyright>
// <summary>
//   The modify selection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Controls;
    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The modify selection.
    /// </summary>
    public partial class ModifySelection : PositionAndSizeForm
    {
        /// <summary>
        /// The function contains.
        /// </summary>
        private const int FunctionContains = 0;

        /// <summary>
        /// The function starts with.
        /// </summary>
        private const int FunctionStartsWith = 1;

        /// <summary>
        /// The function ends with.
        /// </summary>
        private const int FunctionEndsWith = 2;

        /// <summary>
        /// The function not contains.
        /// </summary>
        private const int FunctionNotContains = 3;

        /// <summary>
        /// The function reg ex.
        /// </summary>
        private const int FunctionRegEx = 4;

        /// <summary>
        /// The function unequal.
        /// </summary>
        private const int FunctionUnequal = 5;

        /// <summary>
        /// The function equal.
        /// </summary>
        private const int FunctionEqual = 6;

        /// <summary>
        /// The _loading.
        /// </summary>
        private bool _loading;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _subtitle list view.
        /// </summary>
        private SubtitleListView _subtitleListView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifySelection"/> class.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="subtitleListView">
        /// The subtitle list view.
        /// </param>
        public ModifySelection(Subtitle subtitle, SubtitleListView subtitleListView)
        {
            this.InitializeComponent();
            this._loading = true;
            this._subtitle = subtitle;
            this._subtitleListView = subtitleListView;
            this.labelInfo.Text = string.Empty;
            this.comboBoxRule.SelectedIndex = 0;
            this.Text = Configuration.Settings.Language.ModifySelection.Title;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonApply.Text = Configuration.Settings.Language.General.Apply;
            this.groupBoxRule.Text = Configuration.Settings.Language.ModifySelection.Rule;
            this.groupBoxWhatToDo.Text = Configuration.Settings.Language.ModifySelection.DoWithMatches;
            this.checkBoxCaseSensitive.Text = Configuration.Settings.Language.ModifySelection.CaseSensitive;
            this.radioButtonNewSelection.Text = Configuration.Settings.Language.ModifySelection.MakeNewSelection;
            this.radioButtonAddToSelection.Text = Configuration.Settings.Language.ModifySelection.AddToCurrentSelection;
            this.radioButtonSubtractFromSelection.Text = Configuration.Settings.Language.ModifySelection.SubtractFromCurrentSelection;
            this.radioButtonIntersect.Text = Configuration.Settings.Language.ModifySelection.IntersectWithCurrentSelection;
            this.columnHeaderApply.Text = Configuration.Settings.Language.General.Apply;
            this.columnHeaderLine.Text = Configuration.Settings.Language.General.LineNumber;
            this.columnHeaderText.Text = Configuration.Settings.Language.General.Text;

            Utilities.FixLargeFonts(this, this.buttonOK);

            this.comboBoxRule.Items.Clear();
            this.comboBoxRule.Items.Add(Configuration.Settings.Language.ModifySelection.Contains);
            this.comboBoxRule.Items.Add(Configuration.Settings.Language.ModifySelection.StartsWith);
            this.comboBoxRule.Items.Add(Configuration.Settings.Language.ModifySelection.EndsWith);
            this.comboBoxRule.Items.Add(Configuration.Settings.Language.ModifySelection.NoContains);
            this.comboBoxRule.Items.Add(Configuration.Settings.Language.ModifySelection.RegEx);
            this.comboBoxRule.Items.Add(Configuration.Settings.Language.ModifySelection.UnequalLines);
            this.comboBoxRule.Items.Add(Configuration.Settings.Language.ModifySelection.EqualLines);

            this.checkBoxCaseSensitive.Checked = Configuration.Settings.Tools.ModifySelectionCaseSensitive;
            this.textBox1.Text = Configuration.Settings.Tools.ModifySelectionText;
            if (Configuration.Settings.Tools.ModifySelectionRule == "Starts with")
            {
                this.comboBoxRule.SelectedIndex = FunctionStartsWith;
            }
            else if (Configuration.Settings.Tools.ModifySelectionRule == "Ends with")
            {
                this.comboBoxRule.SelectedIndex = FunctionEndsWith;
            }
            else if (Configuration.Settings.Tools.ModifySelectionRule == "Not contains")
            {
                this.comboBoxRule.SelectedIndex = FunctionNotContains;
            }
            else if (Configuration.Settings.Tools.ModifySelectionRule == "RegEx")
            {
                this.comboBoxRule.SelectedIndex = FunctionRegEx;
            }
            else
            {
                this.comboBoxRule.SelectedIndex = 0;
            }

            this._loading = false;
            this.Preview();
        }

        /// <summary>
        /// The modify selection_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ModifySelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
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
            this.ApplySelection();
            this.DialogResult = DialogResult.OK;

            Configuration.Settings.Tools.ModifySelectionCaseSensitive = this.checkBoxCaseSensitive.Checked;
            Configuration.Settings.Tools.ModifySelectionText = this.textBox1.Text;
            if (this.comboBoxRule.SelectedIndex == FunctionContains)
            {
                Configuration.Settings.Tools.ModifySelectionRule = "Contains";
            }
            else if (this.comboBoxRule.SelectedIndex == FunctionStartsWith)
            {
                Configuration.Settings.Tools.ModifySelectionRule = "Starts with";
            }
            else if (this.comboBoxRule.SelectedIndex == FunctionEndsWith)
            {
                Configuration.Settings.Tools.ModifySelectionRule = "Ends with";
            }
            else if (this.comboBoxRule.SelectedIndex == FunctionNotContains)
            {
                Configuration.Settings.Tools.ModifySelectionRule = "Not contains";
            }
            else if (this.comboBoxRule.SelectedIndex == FunctionRegEx)
            {
                Configuration.Settings.Tools.ModifySelectionRule = "RegEx";
            }
        }

        /// <summary>
        /// The button apply_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonApply_Click(object sender, EventArgs e)
        {
            this.ApplySelection();
        }

        /// <summary>
        /// The add to list view.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        private void AddToListView(Paragraph p, int index)
        {
            var item = new ListViewItem(string.Empty) { Tag = index, Checked = true };
            var subItem = new ListViewItem.ListViewSubItem(item, p.Number.ToString());
            item.SubItems.Add(subItem);
            subItem = new ListViewItem.ListViewSubItem(item, p.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            item.SubItems.Add(subItem);

            this.listViewFixes.Items.Add(item);
        }

        /// <summary>
        /// The preview.
        /// </summary>
        private void Preview()
        {
            if (this._loading)
            {
                return;
            }

            Regex regEx = null;
            this.listViewFixes.BeginUpdate();
            this.listViewFixes.Items.Clear();
            string text = this.textBox1.Text;
            if (this.comboBoxRule.SelectedIndex != FunctionRegEx)
            {
                text = text.Replace("\\r\\n", Environment.NewLine);
            }

            for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
            {
                if ((this.radioButtonSubtractFromSelection.Checked || this.radioButtonIntersect.Checked) && this._subtitleListView.Items[i].Selected || !this.radioButtonSubtractFromSelection.Checked && !this.radioButtonIntersect.Checked)
                {
                    Paragraph p = this._subtitle.Paragraphs[i];
                    if (text.Length > 0)
                    {
                        if (this.comboBoxRule.SelectedIndex == FunctionContains)
                        {
                            // Contains
                            if (this.checkBoxCaseSensitive.Checked && p.Text.Contains(text, StringComparison.Ordinal) || !this.checkBoxCaseSensitive.Checked && p.Text.Contains(text, StringComparison.OrdinalIgnoreCase))
                            {
                                this.AddToListView(p, i);
                            }
                        }
                        else if (this.comboBoxRule.SelectedIndex == FunctionStartsWith)
                        {
                            // Starts with
                            if (this.checkBoxCaseSensitive.Checked && p.Text.StartsWith(text, StringComparison.Ordinal) || !this.checkBoxCaseSensitive.Checked && p.Text.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                            {
                                this.AddToListView(p, i);
                            }
                        }
                        else if (this.comboBoxRule.SelectedIndex == FunctionEndsWith)
                        {
                            // Ends with
                            if (this.checkBoxCaseSensitive.Checked && p.Text.EndsWith(text, StringComparison.Ordinal) || !this.checkBoxCaseSensitive.Checked && p.Text.EndsWith(text, StringComparison.OrdinalIgnoreCase))
                            {
                                this.AddToListView(p, i);
                            }
                        }
                        else if (this.comboBoxRule.SelectedIndex == FunctionNotContains)
                        {
                            // Not contains
                            if (this.checkBoxCaseSensitive.Checked && !p.Text.Contains(text, StringComparison.Ordinal) || !this.checkBoxCaseSensitive.Checked && !p.Text.Contains(text, StringComparison.OrdinalIgnoreCase))
                            {
                                this.AddToListView(p, i);
                            }
                        }
                        else if (this.comboBoxRule.SelectedIndex == FunctionRegEx)
                        {
                            // RegEx
                            this.labelInfo.Text = string.Empty;
                            if (regEx == null)
                            {
                                try
                                {
                                    regEx = new Regex(text, RegexOptions.Compiled);
                                }
                                catch (Exception e)
                                {
                                    this.labelInfo.Text = e.Message;
                                    break;
                                }
                            }

                            if (regEx.IsMatch(p.Text))
                            {
                                this.AddToListView(p, i);
                            }
                        }
                    }

                    if (this.comboBoxRule.SelectedIndex == FunctionUnequal)
                    {
                        // select unequal lines
                        if (i % 2 == 0)
                        {
                            this.AddToListView(p, i);
                        }
                    }
                    else if (this.comboBoxRule.SelectedIndex == FunctionEqual)
                    {
                        // select equal lines
                        if (i % 2 == 1)
                        {
                            this.AddToListView(p, i);
                        }
                    }
                }
            }

            this.listViewFixes.EndUpdate();
            this.groupBoxPreview.Text = string.Format(Configuration.Settings.Language.ModifySelection.MatchingLinesX, this.listViewFixes.Items.Count);
        }

        /// <summary>
        /// The apply selection.
        /// </summary>
        private void ApplySelection()
        {
            this._subtitleListView.BeginUpdate();
            if (this.radioButtonNewSelection.Checked || this.radioButtonIntersect.Checked)
            {
                this._subtitleListView.SelectNone();
            }

            if (this.radioButtonNewSelection.Checked || this.radioButtonAddToSelection.Checked || this.radioButtonIntersect.Checked)
            {
                foreach (ListViewItem item in this.listViewFixes.Items)
                {
                    if (item.Checked)
                    {
                        int index = Convert.ToInt32(item.Tag);
                        this._subtitleListView.Items[index].Selected = true;
                    }
                }
            }
            else if (this.radioButtonSubtractFromSelection.Checked)
            {
                foreach (ListViewItem item in this.listViewFixes.Items)
                {
                    if (item.Checked)
                    {
                        int index = Convert.ToInt32(item.Tag);
                        this._subtitleListView.Items[index].Selected = false;
                    }
                }
            }

            this._subtitleListView.EndUpdate();
        }

        /// <summary>
        /// The text box 1_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.Preview();
        }

        /// <summary>
        /// The combo box rule_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxRule.SelectedIndex == FunctionRegEx)
            {
                // regex
                this.textBox1.ContextMenu = FindReplaceDialogHelper.GetRegExContextMenu(this.textBox1);
                this.checkBoxCaseSensitive.Enabled = false;
            }
            else if (this.comboBoxRule.SelectedIndex == FunctionUnequal || this.comboBoxRule.SelectedIndex == FunctionEqual)
            {
                this.textBox1.ContextMenuStrip = null;
                this.checkBoxCaseSensitive.Enabled = false;
            }
            else
            {
                this.textBox1.ContextMenuStrip = null;
                this.checkBoxCaseSensitive.Enabled = true;
            }

            this.Preview();
        }

        /// <summary>
        /// The check box case sensitive_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            this.Preview();
        }

        /// <summary>
        /// The radio button_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.Preview();
        }
    }
}