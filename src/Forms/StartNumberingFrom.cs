// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartNumberingFrom.cs" company="">
//   
// </copyright>
// <summary>
//   The start numbering from.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The start numbering from.
    /// </summary>
    public sealed partial class StartNumberingFrom : PositionAndSizeForm
    {
        /// <summary>
        /// The _start from number.
        /// </summary>
        private int _startFromNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartNumberingFrom"/> class.
        /// </summary>
        public StartNumberingFrom()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.StartNumberingFrom.Title;
            this.label1.Text = Configuration.Settings.Language.StartNumberingFrom.StartFromNumber;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the start from number.
        /// </summary>
        public int StartFromNumber
        {
            get
            {
                return this._startFromNumber;
            }
        }

        /// <summary>
        /// The form start numbering from_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormStartNumberingFrom_KeyDown(object sender, KeyEventArgs e)
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
                this.ButtonOkClick(null, null);
            }
            else
            {
                if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 || e.KeyCode == Keys.D3 || e.KeyCode == Keys.D4 || e.KeyCode == Keys.D5 || e.KeyCode == Keys.D6 || e.KeyCode == Keys.D7 || e.KeyCode == Keys.D8 || e.KeyCode == Keys.D9 || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Back || e.KeyCode == Keys.Home || e.KeyCode == Keys.End || (e.KeyValue >= 96 && e.KeyValue <= 105))
                {
                    return;
                }
                else if (e.KeyData == (Keys.Control | Keys.V) && Clipboard.GetText(TextDataFormat.UnicodeText).Length > 0)
                {
                    var p = Clipboard.GetText(TextDataFormat.UnicodeText);
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
            if (int.TryParse(this.textBox1.Text, out this._startFromNumber))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(Configuration.Settings.Language.StartNumberingFrom.PleaseEnterAValidNumber);
                this.textBox1.Focus();
                this.textBox1.SelectAll();
            }
        }
    }
}