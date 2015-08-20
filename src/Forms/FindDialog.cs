// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindDialog.cs" company="">
//   
// </copyright>
// <summary>
//   The find dialog.
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
    /// The find dialog.
    /// </summary>
    public sealed partial class FindDialog : Form
    {
        /// <summary>
        /// The _reg ex.
        /// </summary>
        private Regex _regEx;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindDialog"/> class.
        /// </summary>
        public FindDialog()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.FindDialog.Title;
            this.buttonFind.Text = Configuration.Settings.Language.FindDialog.Find;
            this.radioButtonNormal.Text = Configuration.Settings.Language.FindDialog.Normal;
            this.radioButtonCaseSensitive.Text = Configuration.Settings.Language.FindDialog.CaseSensitive;
            this.radioButtonRegEx.Text = Configuration.Settings.Language.FindDialog.RegularExpression;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            if (this.Width < this.radioButtonRegEx.Right + 5)
            {
                this.Width = this.radioButtonRegEx.Right + 5;
            }

            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets or sets the find type.
        /// </summary>
        private FindType FindType
        {
            get
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

            set
            {
                if (value == FindType.CaseSensitive)
                {
                    this.radioButtonCaseSensitive.Checked = true;
                }

                if (value == FindType.Normal)
                {
                    this.radioButtonNormal.Checked = true;
                }

                if (value == FindType.RegEx)
                {
                    this.radioButtonRegEx.Checked = true;
                }
            }
        }

        /// <summary>
        /// Gets the find text.
        /// </summary>
        private string FindText
        {
            get
            {
                if (this.textBoxFind.Visible)
                {
                    return this.textBoxFind.Text;
                }

                return this.comboBoxFind.Text;
            }
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
            return new FindReplaceDialogHelper(this.FindType, this.FindText, this._regEx, string.Empty, 200, 300, startLineIndex);
        }

        /// <summary>
        /// The form find dialog_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormFindDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The button find_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonFind_Click(object sender, EventArgs e)
        {
            string searchText = this.FindText;
            this.textBoxFind.Text = searchText;
            this.comboBoxFind.Text = searchText;

            if (searchText.Length == 0)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (this.radioButtonNormal.Checked)
            {
                this.DialogResult = DialogResult.OK;
            }
            else if (this.radioButtonCaseSensitive.Checked)
            {
                this.DialogResult = DialogResult.OK;
            }
            else if (this.radioButtonRegEx.Checked)
            {
                try
                {
                    this._regEx = new Regex(this.FindText, RegexOptions.Compiled);
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        /// <summary>
        /// The text box find_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonFind_Click(null, null);
            }
        }

        /// <summary>
        /// The combo box find_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonFind_Click(null, null);
            }
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
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == this.radioButtonRegEx)
            {
                if (this.textBoxFind.Visible)
                {
                    this.comboBoxFind.ContextMenu = null;
                    this.textBoxFind.ContextMenu = FindReplaceDialogHelper.GetRegExContextMenu(this.textBoxFind);
                }
                else
                {
                    this.textBoxFind.ContextMenu = null;
                    this.comboBoxFind.ContextMenu = FindReplaceDialogHelper.GetRegExContextMenu(this.comboBoxFind);
                }
            }
            else
            {
                this.textBoxFind.ContextMenu = null;
                this.comboBoxFind.ContextMenu = null;
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
            if (Configuration.Settings.Tools.FindHistory.Count > 0)
            {
                this.textBoxFind.Visible = false;
                this.comboBoxFind.Visible = true;
                this.comboBoxFind.Text = selectedText;
                this.comboBoxFind.SelectAll();
                this.comboBoxFind.Items.Clear();
                for (int index = 0; index < Configuration.Settings.Tools.FindHistory.Count; index++)
                {
                    string s = Configuration.Settings.Tools.FindHistory[index];
                    this.comboBoxFind.Items.Add(s);
                }
            }
            else
            {
                this.comboBoxFind.Visible = false;
                this.textBoxFind.Visible = true;
                this.textBoxFind.Text = selectedText;
                this.textBoxFind.SelectAll();
            }

            if (findHelper != null)
            {
                this.FindType = findHelper.FindType;
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
    }
}