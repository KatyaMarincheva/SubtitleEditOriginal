// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoBreakUnbreakLines.cs" company="">
//   
// </copyright>
// <summary>
//   The auto break unbreak lines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The auto break unbreak lines.
    /// </summary>
    public partial class AutoBreakUnbreakLines : PositionAndSizeForm
    {
        /// <summary>
        /// The _changes.
        /// </summary>
        private int _changes;

        /// <summary>
        /// The _fixed text.
        /// </summary>
        private Dictionary<string, string> _fixedText = new Dictionary<string, string>();

        /// <summary>
        /// The _mode auto balance.
        /// </summary>
        private bool _modeAutoBalance;

        /// <summary>
        /// The _not allowed fixes.
        /// </summary>
        private HashSet<string> _notAllowedFixes = new HashSet<string>();

        /// <summary>
        /// The _paragraphs.
        /// </summary>
        private List<Paragraph> _paragraphs;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoBreakUnbreakLines"/> class.
        /// </summary>
        public AutoBreakUnbreakLines()
        {
            this.InitializeComponent();
            this.groupBoxLinesFound.Text = string.Empty;
            this.listViewFixes.Columns[2].Width = 290;
            this.listViewFixes.Columns[3].Width = 290;

            this.listViewFixes.Columns[0].Text = Configuration.Settings.Language.General.Apply;
            this.listViewFixes.Columns[1].Text = Configuration.Settings.Language.General.LineNumber;
            this.listViewFixes.Columns[2].Text = Configuration.Settings.Language.General.Before;
            this.listViewFixes.Columns[3].Text = Configuration.Settings.Language.General.After;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the fixed text.
        /// </summary>
        public Dictionary<string, string> FixedText
        {
            get
            {
                return this._fixedText;
            }
        }

        /// <summary>
        /// Gets the changes.
        /// </summary>
        public int Changes
        {
            get
            {
                return this._changes;
            }
        }

        /// <summary>
        /// Gets the minimum length.
        /// </summary>
        public int MinimumLength
        {
            get
            {
                return int.Parse(this.comboBoxConditions.Items[this.comboBoxConditions.SelectedIndex].ToString());
            }
        }

        /// <summary>
        /// Gets the merge lines shorter than.
        /// </summary>
        public int MergeLinesShorterThan
        {
            get
            {
                if (Configuration.Settings.Tools.MergeLinesShorterThan > this.MinimumLength)
                {
                    return this.MinimumLength - 1;
                }

                return Configuration.Settings.Tools.MergeLinesShorterThan;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="autoBalance">
        /// The auto balance.
        /// </param>
        public void Initialize(Subtitle subtitle, bool autoBalance)
        {
            this._modeAutoBalance = autoBalance;
            this._paragraphs = new List<Paragraph>();

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                this._paragraphs.Add(p);
            }

            if (autoBalance)
            {
                this.labelCondition.Text = Configuration.Settings.Language.AutoBreakUnbreakLines.OnlyBreakLinesLongerThan;
                const int start = 10;
                const int max = 60;
                for (int i = start; i <= max; i++)
                {
                    this.comboBoxConditions.Items.Add(i.ToString(CultureInfo.InvariantCulture));
                }

                int index = Configuration.Settings.Tools.MergeLinesShorterThan - (start + 1);
                if (index > 0 && index < max)
                {
                    this.comboBoxConditions.SelectedIndex = index;
                }
                else
                {
                    this.comboBoxConditions.SelectedIndex = 30;
                }

                this.AutoBalance();
            }
            else
            {
                this.labelCondition.Text = Configuration.Settings.Language.AutoBreakUnbreakLines.OnlyUnbreakLinesLongerThan;
                for (int i = 5; i < 51; i++)
                {
                    this.comboBoxConditions.Items.Add(i.ToString(CultureInfo.InvariantCulture));
                }

                this.comboBoxConditions.SelectedIndex = 5;

                this.Unbreak();
            }

            this.comboBoxConditions.SelectedIndexChanged += this.ComboBoxConditionsSelectedIndexChanged;
        }

        /// <summary>
        /// The auto balance.
        /// </summary>
        private void AutoBalance()
        {
            this.listViewFixes.ItemChecked -= this.listViewFixes_ItemChecked;
            this._notAllowedFixes = new HashSet<string>();
            this._fixedText = new Dictionary<string, string>();
            int minLength = this.MinimumLength;
            this.Text = Configuration.Settings.Language.AutoBreakUnbreakLines.TitleAutoBreak;

            var sub = new Subtitle();
            foreach (Paragraph p in this._paragraphs)
            {
                sub.Paragraphs.Add(p);
            }

            var language = Utilities.AutoDetectGoogleLanguage(sub);

            this.listViewFixes.BeginUpdate();
            this.listViewFixes.Items.Clear();
            foreach (Paragraph p in this._paragraphs)
            {
                if (HtmlUtil.RemoveHtmlTags(p.Text, true).Length > minLength || p.Text.Contains(Environment.NewLine))
                {
                    var text = Utilities.AutoBreakLine(p.Text, 5, this.MergeLinesShorterThan, language);
                    if (text != p.Text)
                    {
                        this.AddToListView(p, text);
                        this._fixedText.Add(p.ID, text);
                        this._changes++;
                    }
                }
            }

            this.listViewFixes.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.AutoBreakUnbreakLines.LinesFoundX, this.listViewFixes.Items.Count);
            this.listViewFixes.ItemChecked += this.listViewFixes_ItemChecked;
        }

        /// <summary>
        /// The unbreak.
        /// </summary>
        private void Unbreak()
        {
            this.listViewFixes.ItemChecked -= this.listViewFixes_ItemChecked;
            this._notAllowedFixes = new HashSet<string>();
            this._fixedText = new Dictionary<string, string>();
            int minLength = int.Parse(this.comboBoxConditions.Items[this.comboBoxConditions.SelectedIndex].ToString());
            this.Text = Configuration.Settings.Language.AutoBreakUnbreakLines.TitleUnbreak;
            this.listViewFixes.BeginUpdate();
            this.listViewFixes.Items.Clear();
            foreach (Paragraph p in this._paragraphs)
            {
                if (p.Text != null && p.Text.Contains(Environment.NewLine) && HtmlUtil.RemoveHtmlTags(p.Text).Length > minLength)
                {
                    var text = Utilities.UnbreakLine(p.Text);
                    if (text != p.Text)
                    {
                        this.AddToListView(p, text);
                        this._fixedText.Add(p.ID, text);
                        this._changes++;
                    }
                }
            }

            this.listViewFixes.EndUpdate();
            this.groupBoxLinesFound.Text = string.Format(Configuration.Settings.Language.AutoBreakUnbreakLines.LinesFoundX, this.listViewFixes.Items.Count);
            this.listViewFixes.ItemChecked += this.listViewFixes_ItemChecked;
        }

        /// <summary>
        /// The auto break unbreak lines key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AutoBreakUnbreakLinesKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
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
            item.SubItems.Add(p.Number.ToString(CultureInfo.InvariantCulture));
            item.SubItems.Add(p.Text.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            item.SubItems.Add(newText.Replace(Environment.NewLine, Configuration.Settings.General.ListViewLineSeparatorString));
            this.listViewFixes.Items.Add(item);
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
            for (int i = this._paragraphs.Count - 1; i >= 0; i--)
            {
                var p = this._paragraphs[i];
                if (this._notAllowedFixes.Contains(p.ID))
                {
                    this._fixedText.Remove(p.ID);
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The combo box conditions selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxConditionsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._modeAutoBalance)
            {
                this.AutoBalance();
            }
            else
            {
                this.Unbreak();
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
            if (e.Item == null)
            {
                return;
            }

            var p = e.Item.Tag as Paragraph;
            if (p == null)
            {
                return;
            }

            if (e.Item.Checked)
            {
                this._notAllowedFixes.Remove(p.ID);
            }
            else
            {
                this._notAllowedFixes.Add(p.ID);
            }
        }

        /// <summary>
        /// The list view fixes_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void listViewFixes_Resize(object sender, EventArgs e)
        {
            var newWidth = (this.listViewFixes.Width - (this.listViewFixes.Columns[0].Width + this.listViewFixes.Columns[1].Width)) / 2;
            this.listViewFixes.Columns[3].Width = this.listViewFixes.Columns[2].Width = newWidth;
        }
    }
}