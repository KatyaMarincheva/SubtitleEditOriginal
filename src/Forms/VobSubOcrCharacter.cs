// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubOcrCharacter.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub ocr character.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The vob sub ocr character.
    /// </summary>
    public sealed partial class VobSubOcrCharacter : Form
    {
        /// <summary>
        /// The _additions.
        /// </summary>
        private List<VobSubOcr.ImageCompareAddition> _additions;

        /// <summary>
        /// The _vob sub form.
        /// </summary>
        private VobSubOcr _vobSubForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubOcrCharacter"/> class.
        /// </summary>
        public VobSubOcrCharacter()
        {
            this.InitializeComponent();

            var language = Configuration.Settings.Language.VobSubOcrCharacter;
            this.Text = language.Title;
            this.labelSubtitleImage.Text = language.SubtitleImage;
            this.buttonExpandSelection.Text = language.ExpandSelection;
            this.buttonShrinkSelection.Text = language.ShrinkSelection;
            this.labelCharacters.Text = language.Characters;
            this.labelCharactersAsText.Text = language.CharactersAsText;
            this.checkBoxItalic.Text = language.Italic;
            this.labelItalicOn.Text = language.Italic.Replace("&", string.Empty);
            this.labelItalicOn.Visible = false;
            this.buttonAbort.Text = language.Abort;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = language.Skip;
            this.nordicToolStripMenuItem.Text = language.Nordic;
            this.spanishToolStripMenuItem.Text = language.Spanish;
            this.germanToolStripMenuItem.Text = language.German;
            this.checkBoxAutoSubmitOfFirstChar.Text = language.AutoSubmitOnFirstChar;

            this.dataGridView1.Rows.Add("♪", "á", "é", "í", "ó", "ö", "ő", "ú", "ü", "ű");
            this.dataGridView1.Rows.Add("♫", "Á", "É", "Í", "Ó", "Ö", "Ő", "Ú", "Ü", "Ű");

            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets the manual recognized characters.
        /// </summary>
        public string ManualRecognizedCharacters
        {
            get
            {
                return this.textBoxCharacters.Text;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is italic.
        /// </summary>
        public bool IsItalic
        {
            get
            {
                return this.checkBoxItalic.Checked;
            }
        }

        /// <summary>
        /// Gets the form position.
        /// </summary>
        public Point FormPosition
        {
            get
            {
                return new Point(this.Left, this.Top);
            }
        }

        /// <summary>
        /// Gets a value indicating whether expand selection.
        /// </summary>
        public bool ExpandSelection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether shrink selection.
        /// </summary>
        public bool ShrinkSelection { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="vobSubImage">
        /// The vob sub image.
        /// </param>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="italicChecked">
        /// The italic checked.
        /// </param>
        /// <param name="showShrink">
        /// The show shrink.
        /// </param>
        /// <param name="bestGuess">
        /// The best guess.
        /// </param>
        /// <param name="additions">
        /// The additions.
        /// </param>
        /// <param name="vobSubForm">
        /// The vob sub form.
        /// </param>
        internal void Initialize(Bitmap vobSubImage, ImageSplitterItem character, Point position, bool italicChecked, bool showShrink, VobSubOcr.CompareMatch bestGuess, List<VobSubOcr.ImageCompareAddition> additions, VobSubOcr vobSubForm)
        {
            this.ShrinkSelection = false;
            this.ExpandSelection = false;

            this.textBoxCharacters.Text = string.Empty;
            if (bestGuess != null)
            {
                this.buttonGuess.Visible = false; // hm... not too useful :(
                this.buttonGuess.Text = bestGuess.Text;
            }
            else
            {
                this.buttonGuess.Visible = false;
            }

            this._vobSubForm = vobSubForm;
            this._additions = additions;

            this.buttonShrinkSelection.Visible = showShrink;

            this.checkBoxItalic.Checked = italicChecked;
            if (position.X != -1 && position.Y != -1)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Left = position.X;
                this.Top = position.Y;
            }

            this.pictureBoxSubtitleImage.Image = vobSubImage;
            this.pictureBoxCharacter.Image = character.NikseBitmap.GetBitmap();

            if (this._additions.Count > 0)
            {
                var last = this._additions[this._additions.Count - 1];
                this.buttonLastEdit.Visible = true;
                if (last.Italic)
                {
                    this.buttonLastEdit.Font = new Font(this.buttonLastEdit.Font.FontFamily, this.buttonLastEdit.Font.Size, FontStyle.Italic);
                }
                else
                {
                    this.buttonLastEdit.Font = new Font(this.buttonLastEdit.Font.FontFamily, this.buttonLastEdit.Font.Size);
                }

                this.pictureBoxLastEdit.Visible = true;
                this.pictureBoxLastEdit.Image = last.Image.GetBitmap();
                this.buttonLastEdit.Text = string.Format(Configuration.Settings.Language.VobSubOcrCharacter.EditLastX, last.Text);
                this.pictureBoxLastEdit.Top = this.buttonLastEdit.Top - last.Image.Height + this.buttonLastEdit.Height;
            }
            else
            {
                this.buttonLastEdit.Visible = false;
                this.pictureBoxLastEdit.Visible = false;
            }

            Bitmap org = (Bitmap)vobSubImage.Clone();
            Bitmap bm = new Bitmap(org.Width, org.Height);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(org, 0, 0, org.Width, org.Height);
            g.DrawRectangle(Pens.Red, character.X, character.Y, character.NikseBitmap.Width, character.NikseBitmap.Height - 1);
            g.Dispose();
            this.pictureBoxSubtitleImage.Image = bm;

            this.pictureBoxCharacter.Top = this.labelCharacters.Top + 16;
            this.pictureBoxLastEdit.Left = this.buttonLastEdit.Left + this.buttonLastEdit.Width + 5;
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
        /// The text box characters key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxCharactersKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The check box italic checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxItalicCheckedChanged(object sender, EventArgs e)
        {
            this.textBoxCharacters.Focus();

            if (this.checkBoxItalic.Checked)
            {
                this.labelCharactersAsText.Font = new Font(this.labelCharactersAsText.Font.FontFamily, this.labelCharactersAsText.Font.Size, FontStyle.Italic);
                this.textBoxCharacters.Font = new Font(this.textBoxCharacters.Font.FontFamily, this.textBoxCharacters.Font.Size, FontStyle.Italic);
                this.labelItalicOn.Visible = true;
            }
            else
            {
                this.labelCharactersAsText.Font = new Font(this.labelCharactersAsText.Font.FontFamily, this.labelCharactersAsText.Font.Size);
                this.textBoxCharacters.Font = new Font(this.textBoxCharacters.Font.FontFamily, this.textBoxCharacters.Font.Size);
                this.labelItalicOn.Visible = false;
            }
        }

        /// <summary>
        /// The button expand selection click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonExpandSelectionClick(object sender, EventArgs e)
        {
            this.ExpandSelection = true;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button shrink selection click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonShrinkSelectionClick(object sender, EventArgs e)
        {
            this.ShrinkSelection = true;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button last edit_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonLastEdit_Click(object sender, EventArgs e)
        {
            if (this._additions.Count > 0)
            {
                var last = this._additions[this._additions.Count - 1];
                var result = this._vobSubForm.EditImageCompareCharacters(last.Name, last.Text);
                if (result == DialogResult.OK)
                {
                    this._additions.RemoveAt(this._additions.Count - 1);
                    this._vobSubForm.StartOcrFromDelayed();
                    this.DialogResult = DialogResult.Abort;
                }
            }
        }

        /// <summary>
        /// The button guess_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGuess_Click(object sender, EventArgs e)
        {
            this.textBoxCharacters.Text = this.buttonGuess.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The insert language character.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void InsertLanguageCharacter(object sender, EventArgs e)
        {
            this.textBoxCharacters.Text = this.textBoxCharacters.Text.Insert(this.textBoxCharacters.SelectionStart, (sender as ToolStripMenuItem).Text);
        }

        /// <summary>
        /// The text box characters_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxCharacters_TextChanged(object sender, EventArgs e)
        {
            if (this.checkBoxAutoSubmitOfFirstChar.Checked && this.textBoxCharacters.Text.Length > 0)
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The vob sub ocr character_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubOcrCharacter_Shown(object sender, EventArgs e)
        {
            this.textBoxCharacters.Focus();
        }

        /// <summary>
        /// The check box auto submit of first char_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void checkBoxAutoSubmitOfFirstChar_CheckedChanged(object sender, EventArgs e)
        {
            this.Focus();
            this.textBoxCharacters.Focus();
            this.textBoxCharacters.Focus();
            Application.DoEvents();
            this.textBoxCharacters.Focus();
            this.textBoxCharacters.Focus();
        }

        /// <summary>
        /// The vob sub ocr character_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void VobSubOcrCharacter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Left && this.buttonShrinkSelection.Visible)
            {
                this.ButtonShrinkSelectionClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.Right && this.buttonExpandSelection.Visible)
            {
                this.ButtonExpandSelectionClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Subtract && this.buttonShrinkSelection.Visible)
            {
                this.ButtonShrinkSelectionClick(null, null);
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.Add && this.buttonExpandSelection.Visible)
            {
                this.ButtonExpandSelectionClick(null, null);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The data grid view 1_ cell content click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.textBoxCharacters.Text = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        }
    }
}