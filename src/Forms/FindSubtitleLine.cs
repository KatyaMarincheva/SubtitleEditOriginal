// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindSubtitleLine.cs" company="">
//   
// </copyright>
// <summary>
//   The find subtitle line.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The find subtitle line.
    /// </summary>
    public sealed partial class FindSubtitleLine : Form
    {
        /// <summary>
        /// The _main general go to next subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToNextSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle);

        /// <summary>
        /// The _main general go to prev subtitle.
        /// </summary>
        private readonly Keys _mainGeneralGoToPrevSubtitle = Utilities.GetKeys(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle);

        /// <summary>
        /// The _paragraphs.
        /// </summary>
        private List<Paragraph> _paragraphs = new List<Paragraph>();

        /// <summary>
        /// The _start find index.
        /// </summary>
        private int _startFindIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindSubtitleLine"/> class.
        /// </summary>
        public FindSubtitleLine()
        {
            this.InitializeComponent();
            this.Icon = Properties.Resources.SubtitleEditFormIcon;

            this.Text = Configuration.Settings.Language.FindSubtitleLine.Title;
            this.buttonFind.Text = Configuration.Settings.Language.FindSubtitleLine.Find;
            this.buttonFindNext.Text = Configuration.Settings.Language.FindSubtitleLine.FindNext;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.subtitleListView1.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.FixLargeFonts();
        }

        /// <summary>
        /// Gets the selected index.
        /// </summary>
        public int SelectedIndex { get; private set; }

        /// <summary>
        /// The fix large fonts.
        /// </summary>
        private void FixLargeFonts()
        {
            using (var graphics = this.CreateGraphics())
            {
                var textSize = graphics.MeasureString(this.buttonOK.Text, this.Font);
                if (textSize.Height > this.buttonOK.Height - 4)
                {
                    this.subtitleListView1.InitializeTimestampColumnWidths(this);
                    int newButtonHeight = (int)(textSize.Height + 7 + 0.5);
                    Utilities.SetButtonHeight(this, newButtonHeight, 1);
                }
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="paragraphs">
        /// The paragraphs.
        /// </param>
        /// <param name="appendTitle">
        /// The append title.
        /// </param>
        public void Initialize(List<Paragraph> paragraphs, string appendTitle)
        {
            this.Text += appendTitle;
            this._paragraphs = paragraphs;
            this.subtitleListView1.Fill(paragraphs);
            this._startFindIndex = -1;
        }

        /// <summary>
        /// The button find click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonFindClick(object sender, EventArgs e)
        {
            this._startFindIndex = -1;
            this.FindText();
        }

        /// <summary>
        /// The find text.
        /// </summary>
        private void FindText()
        {
            if (string.IsNullOrWhiteSpace(this.textBoxFindText.Text))
            {
                return;
            }

            for (var index = 0; index < this._paragraphs.Count; index++)
            {
                if (index > this._startFindIndex && this._paragraphs[index].Text.Contains(this.textBoxFindText.Text, StringComparison.OrdinalIgnoreCase))
                {
                    this.subtitleListView1.Items[index].Selected = true;
                    this.subtitleListView1.HideSelection = false;
                    this.subtitleListView1.Items[index].EnsureVisible();
                    this.subtitleListView1.Items[index].Focused = true;
                    this._startFindIndex = index;
                    return;
                }
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
            this.SelectedIndex = -1;
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
            if (this.subtitleListView1.SelectedItems.Count > 0)
            {
                this.SelectedIndex = this.subtitleListView1.SelectedItems[0].Index;
            }
            else
            {
                this.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// The text box find text key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxFindTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonFindClick(sender, null);
            }
        }

        /// <summary>
        /// The text box find text text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxFindTextTextChanged(object sender, EventArgs e)
        {
            this._startFindIndex = -1;
        }

        /// <summary>
        /// The button find next click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonFindNextClick(object sender, EventArgs e)
        {
            this.FindText();
        }

        /// <summary>
        /// The form find subtitle line_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormFindSubtitleLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                this.FindText();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (this._mainGeneralGoToNextSubtitle == e.KeyData || (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt))
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex++;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
            }
            else if (this._mainGeneralGoToPrevSubtitle == e.KeyData || (e.KeyCode == Keys.Up && e.Modifiers == Keys.Alt))
            {
                int selectedIndex = 0;
                if (this.subtitleListView1.SelectedItems.Count > 0)
                {
                    selectedIndex = this.subtitleListView1.SelectedItems[0].Index;
                    selectedIndex--;
                }

                this.subtitleListView1.SelectIndexAndEnsureVisible(selectedIndex);
            }
            else if (e.KeyCode == Keys.Home && e.Alt)
            {
                this.subtitleListView1.SelectIndexAndEnsureVisible(0);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.End && e.Alt)
            {
                this.subtitleListView1.SelectIndexAndEnsureVisible(this.subtitleListView1.Items.Count - 1);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.G && this.subtitleListView1.Items.Count > 1)
            {
                using (var goToLine = new GoToLine())
                {
                    goToLine.Initialize(1, this.subtitleListView1.Items.Count);
                    if (goToLine.ShowDialog(this) == DialogResult.OK)
                    {
                        this.subtitleListView1.SelectNone();
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].Selected = true;
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].EnsureVisible();
                        this.subtitleListView1.Items[goToLine.LineNumber - 1].Focused = true;
                    }
                }
            }
        }

        /// <summary>
        /// The form find subtitle line_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormFindSubtitleLine_Load(object sender, EventArgs e)
        {
            this.SetFocus();
        }

        /// <summary>
        /// The form find subtitle line_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormFindSubtitleLine_Shown(object sender, EventArgs e)
        {
            this.SetFocus();
        }

        /// <summary>
        /// The set focus.
        /// </summary>
        private void SetFocus()
        {
            if (this.textBoxFindText.CanFocus)
            {
                this.textBoxFindText.Focus();
            }
        }

        /// <summary>
        /// The subtitle list view 1 mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SubtitleListView1MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ButtonOkClick(null, null);
            this.Close();
        }
    }
}