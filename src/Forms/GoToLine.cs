// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoToLine.cs" company="">
//   
// </copyright>
// <summary>
//   The go to line.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The go to line.
    /// </summary>
    public sealed partial class GoToLine : Form
    {
        /// <summary>
        /// The _line number.
        /// </summary>
        private int _lineNumber;

        /// <summary>
        /// The _max.
        /// </summary>
        private int _max;

        /// <summary>
        /// The _min.
        /// </summary>
        private int _min;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoToLine"/> class.
        /// </summary>
        public GoToLine()
        {
            this.InitializeComponent();
            this.Icon = Properties.Resources.SubtitleEditFormIcon;
            this.Text = Configuration.Settings.Language.GoToLine.Title;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return this._lineNumber;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        public void Initialize(int min, int max)
        {
            this._min = min;
            this._max = max;
            this.labelGoToLine.Text = string.Format(this.Text + " ({0} - {1})", min, max);
        }

        /// <summary>
        /// The form go to line_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormGoToLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The text box 1 key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBox1KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (int.TryParse(this.textBox1.Text, out this._lineNumber))
                {
                    if (this._lineNumber >= this._min && this._lineNumber <= this._max)
                    {
                        this.DialogResult = DialogResult.OK;
                    }
                }
            }
            else
            {
                if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 || e.KeyCode == Keys.D3 || e.KeyCode == Keys.D4 || e.KeyCode == Keys.D5 || e.KeyCode == Keys.D6 || e.KeyCode == Keys.D7 || e.KeyCode == Keys.D8 || e.KeyCode == Keys.D9 || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Back || e.KeyCode == Keys.Home || e.KeyCode == Keys.End || (e.KeyValue >= 96 && e.KeyValue <= 105))
                {
                    return;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    this.ButtonOkClick(null, null);
                }
                else if (e.KeyData == (Keys.Control | Keys.V) && Clipboard.GetText(TextDataFormat.UnicodeText).Length > 0)
                {
                    string p = Clipboard.GetText(TextDataFormat.UnicodeText);
                    int num;
                    if (!int.TryParse(p, out num))
                    {
                        e.SuppressKeyPress = true;
                    }
                }
                else if (e.Modifiers != Keys.Control && e.Modifiers != Keys.Alt)
                {
                    e.SuppressKeyPress = true;
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
            if (int.TryParse(this.textBox1.Text, out this._lineNumber))
            {
                if (this._lineNumber >= this._min && this._lineNumber <= this._max)
                {
                    this.DialogResult = DialogResult.OK;
                    return;
                }
            }

            MessageBox.Show(string.Format(Configuration.Settings.Language.GoToLine.XIsNotAValidNumber, this.textBox1.Text));
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
    }
}