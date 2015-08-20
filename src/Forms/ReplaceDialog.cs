// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReplaceDialog.cs" company="">
//   
// </copyright>
// <summary>
//   The replace dialog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Enums;

    /// <summary>
    /// The replace dialog.
    /// </summary>
    public sealed partial class ReplaceDialog : Form
    {
        /// <summary>
        /// The _left.
        /// </summary>
        private int _left;

        /// <summary>
        /// The _reg ex.
        /// </summary>
        private Regex _regEx;

        /// <summary>
        /// The _top.
        /// </summary>
        private int _top;

        /// <summary>
        /// The _user action.
        /// </summary>
        private bool _userAction = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceDialog"/> class.
        /// </summary>
        public ReplaceDialog()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.ReplaceDialog.Title;
            this.labelFindWhat.Text = Configuration.Settings.Language.ReplaceDialog.FindWhat;
            this.radioButtonNormal.Text = Configuration.Settings.Language.ReplaceDialog.Normal;
            this.radioButtonCaseSensitive.Text = Configuration.Settings.Language.ReplaceDialog.CaseSensitive;
            this.radioButtonRegEx.Text = Configuration.Settings.Language.ReplaceDialog.RegularExpression;
            this.labelReplaceWith.Text = Configuration.Settings.Language.ReplaceDialog.ReplaceWith;
            this.buttonFind.Text = Configuration.Settings.Language.ReplaceDialog.Find;
            this.buttonReplace.Text = Configuration.Settings.Language.ReplaceDialog.Replace;
            this.buttonReplaceAll.Text = Configuration.Settings.Language.ReplaceDialog.ReplaceAll;

            if (this.Width < this.radioButtonRegEx.Right + 5)
            {
                this.Width = this.radioButtonRegEx.Right + 5;
            }

            Utilities.FixLargeFonts(this, this.buttonReplace);
        }

        /// <summary>
        /// Gets or sets a value indicating whether replace all.
        /// </summary>
        public bool ReplaceAll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether find only.
        /// </summary>
        public bool FindOnly { get; set; }

        /// <summary>
        /// The get find type.
        /// </summary>
        /// <returns>
        /// The <see cref="FindType"/>.
        /// </returns>
        public FindType GetFindType()
        {
            if (this.radioButtonNormal.Checked)
            {
                return FindType.Normal;
            }

            if (this.radioButtonCaseSensitive.Checked)
            {
                return FindType.CaseSensitive;
            }

            return FindType.RegEx;
        }

        /// <summary>
        /// The get find dialog helper.
        /// </summary>
        /// <param name="startLineIndex">
        /// The start line index.
        /// </param>
        /// <returns>
        /// The <see cref="FindReplaceDialogHelper"/>.
        /// </returns>
        public FindReplaceDialogHelper GetFindDialogHelper(int startLineIndex)
        {
            return new FindReplaceDialogHelper(this.GetFindType(), this.textBoxFind.Text, this._regEx, this.textBoxReplace.Text, this._left, this._top, startLineIndex);
        }

        /// <summary>
        /// The form replace dialog_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormReplaceDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="selectedText">
        /// The selected text.
        /// </param>
        /// <param name="findHelper">
        /// The find helper.
        /// </param>
        internal void Initialize(string selectedText, FindReplaceDialogHelper findHelper)
        {
            this.textBoxFind.Text = selectedText;

            // if we are searching for the same thing, then keep the replace text the same.
            if (selectedText == findHelper.FindText)
            {
                this.textBoxReplace.Text = findHelper.ReplaceText;
            }

            this.textBoxFind.SelectAll();
            this._left = findHelper.WindowPositionLeft;
            this._top = findHelper.WindowPositionTop;

            if (findHelper.FindType == FindType.RegEx)
            {
                this.radioButtonRegEx.Checked = true;
            }
            else if (findHelper.FindType == FindType.CaseSensitive)
            {
                this.radioButtonCaseSensitive.Checked = true;
            }
            else
            {
                this.radioButtonNormal.Checked = true;
            }
        }

        /// <summary>
        /// The button replace click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonReplaceClick(object sender, EventArgs e)
        {
            this.ReplaceAll = false;
            this.FindOnly = false;

            this.Validate(this.textBoxFind.Text);
        }

        /// <summary>
        /// The button replace all click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonReplaceAllClick(object sender, EventArgs e)
        {
            this.ReplaceAll = true;
            this.FindOnly = false;

            this.Validate(this.textBoxFind.Text);
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="searchText">
        /// The search text.
        /// </param>
        private void Validate(string searchText)
        {
            if (searchText.Length == 0)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (this.radioButtonNormal.Checked)
            {
                this.DialogResult = DialogResult.OK;
                this._userAction = true;
            }
            else if (this.radioButtonCaseSensitive.Checked)
            {
                this.DialogResult = DialogResult.OK;
                this._userAction = true;
            }
            else if (this.radioButtonRegEx.Checked)
            {
                try
                {
                    this._regEx = new Regex(this.textBoxFind.Text, RegexOptions.Compiled);
                    this.DialogResult = DialogResult.OK;
                    this._userAction = true;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
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
            this.ReplaceAll = false;
            this.FindOnly = true;

            this.Validate(this.textBoxFind.Text);
        }

        /// <summary>
        /// The form replace dialog_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormReplaceDialog_Shown(object sender, EventArgs e)
        {
            if (this._left > 0 && this._top > 0)
            {
                this.Left = this._left;
                this.Top = this._top;
            }
            else
            {
                this._left = this.Left;
                this._top = this.Top;
            }
        }

        /// <summary>
        /// The form replace dialog_ move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormReplaceDialog_Move(object sender, EventArgs e)
        {
            if (this.Left > 0 && this.Top > 0)
            {
                this._left = this.Left;
                this._top = this.Top;
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
        /// The text box find key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxFindKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonFindClick(null, null);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The set icon.
        /// </summary>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        internal void SetIcon(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                IntPtr Hicon = bitmap.GetHicon();
                this.Icon = Icon.FromHandle(Hicon);
            }
        }

        /// <summary>
        /// The replace dialog_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ReplaceDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this._userAction)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}