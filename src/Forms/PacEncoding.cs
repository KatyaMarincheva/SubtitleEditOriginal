// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PacEncoding.cs" company="">
//   
// </copyright>
// <summary>
//   The pac encoding.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Text;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.SubtitleFormats;

    /// <summary>
    /// The pac encoding.
    /// </summary>
    public sealed partial class PacEncoding : Form
    {
        /// <summary>
        /// The _preview buffer.
        /// </summary>
        private readonly byte[] _previewBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacEncoding"/> class.
        /// </summary>
        /// <param name="previewBuffer">
        /// The preview buffer.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public PacEncoding(byte[] previewBuffer, string fileName)
        {
            this.CodePageIndex = Configuration.Settings.General.LastPacCodePage;
            this.InitializeComponent();
            this.Text = System.IO.Path.GetFileName(fileName);
            this._previewBuffer = previewBuffer;
            if (this.CodePageIndex >= 0 && this.CodePageIndex < this.comboBoxCodePage.Items.Count)
            {
                this.comboBoxCodePage.SelectedIndex = this.CodePageIndex;
            }

            if (previewBuffer == null)
            {
                this.labelPreview.Visible = false;
                this.textBoxPreview.Visible = false;
                this.Height -= this.textBoxPreview.Height;
            }

            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets or sets the code page index.
        /// </summary>
        public int CodePageIndex { get; set; }

        /// <summary>
        /// The pac encoding_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PacEncoding_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The combo box code page_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void comboBoxCodePage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxCodePage.SelectedIndex >= 0)
            {
                this.CodePageIndex = this.comboBoxCodePage.SelectedIndex;
                if (this._previewBuffer != null)
                {
                    Encoding encoding = Pac.GetEncoding(this.CodePageIndex);
                    const int feIndex = 0;
                    const int endDelimiter = 0x00;
                    var sb = new StringBuilder();
                    int index = feIndex + 3;
                    while (index < this._previewBuffer.Length && this._previewBuffer[index] != endDelimiter)
                    {
                        if (this._previewBuffer[index] == 0xFE)
                        {
                            sb.AppendLine();
                            index += 2;
                        }
                        else if (this._previewBuffer[index] == 0xFF)
                        {
                            sb.Append(' ');
                        }
                        else if (this.CodePageIndex == Pac.CodePageLatin)
                        {
                            sb.Append(Pac.GetLatinString(encoding, this._previewBuffer, ref index));
                        }
                        else if (this.CodePageIndex == Pac.CodePageArabic)
                        {
                            sb.Append(Pac.GetArabicString(this._previewBuffer, ref index));
                        }
                        else if (this.CodePageIndex == Pac.CodePageHebrew)
                        {
                            sb.Append(Pac.GetHebrewString(this._previewBuffer, ref index));
                        }
                        else if (this.CodePageIndex == Pac.CodePageCyrillic)
                        {
                            sb.Append(Pac.GetCyrillicString(this._previewBuffer, ref index));
                        }
                        else
                        {
                            sb.Append(encoding.GetString(this._previewBuffer, index, 1));
                        }

                        index++;
                    }

                    if (this.CodePageIndex == Pac.CodePageArabic)
                    {
                        this.textBoxPreview.Text = Utilities.FixEnglishTextInRightToLeftLanguage(sb.ToString(), "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
                    }
                    else
                    {
                        this.textBoxPreview.Text = sb.ToString();
                    }
                }
            }
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
            this.DialogResult = DialogResult.OK;
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
    }
}