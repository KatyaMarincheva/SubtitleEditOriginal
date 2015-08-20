// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OCRSpellCheck.cs" company="">
//   
// </copyright>
// <summary>
//   The ocr spell check.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The ocr spell check.
    /// </summary>
    public sealed partial class OcrSpellCheck : Form
    {
        /// <summary>
        /// The action.
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// The abort.
            /// </summary>
            Abort, 

            /// <summary>
            /// The add to user dictionary.
            /// </summary>
            AddToUserDictionary, 

            /// <summary>
            /// The add to names.
            /// </summary>
            AddToNames, 

            /// <summary>
            /// The allways use suggestion.
            /// </summary>
            AllwaysUseSuggestion, 

            /// <summary>
            /// The change and save.
            /// </summary>
            ChangeAndSave, 

            /// <summary>
            /// The change once.
            /// </summary>
            ChangeOnce, 

            /// <summary>
            /// The change whole text.
            /// </summary>
            ChangeWholeText, 

            /// <summary>
            /// The change all whole text.
            /// </summary>
            ChangeAllWholeText, 

            /// <summary>
            /// The skip all.
            /// </summary>
            SkipAll, 

            /// <summary>
            /// The skip whole text.
            /// </summary>
            SkipWholeText, 

            /// <summary>
            /// The skip once.
            /// </summary>
            SkipOnce, 

            /// <summary>
            /// The use suggestion.
            /// </summary>
            UseSuggestion, 
        }

        /// <summary>
        /// The _original word.
        /// </summary>
        private string _originalWord;

        /// <summary>
        /// Initializes a new instance of the <see cref="OcrSpellCheck"/> class.
        /// </summary>
        public OcrSpellCheck()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.SpellCheck.Title;
            this.buttonAddToDictionary.Text = Configuration.Settings.Language.SpellCheck.AddToUserDictionary;
            this.buttonChange.Text = Configuration.Settings.Language.SpellCheck.Change;
            this.buttonChangeAll.Text = Configuration.Settings.Language.SpellCheck.ChangeAll;
            this.buttonSkipAll.Text = Configuration.Settings.Language.SpellCheck.SkipAll;
            this.buttonSkipOnce.Text = Configuration.Settings.Language.SpellCheck.SkipOnce;
            this.buttonUseSuggestion.Text = Configuration.Settings.Language.SpellCheck.Use;
            this.buttonUseSuggestionAlways.Text = Configuration.Settings.Language.SpellCheck.UseAlways;
            this.buttonAbort.Text = Configuration.Settings.Language.SpellCheck.Abort;
            this.groupBoxEditWholeText.Text = Configuration.Settings.Language.SpellCheck.EditWholeText;
            this.buttonChangeWholeText.Text = Configuration.Settings.Language.SpellCheck.Change;
            this.buttonSkipText.Text = Configuration.Settings.Language.SpellCheck.SkipOnce;
            this.buttonEditWholeText.Text = Configuration.Settings.Language.SpellCheck.EditWholeText;
            this.buttonEditWord.Text = Configuration.Settings.Language.SpellCheck.EditWordOnly;
            this.groupBoxText.Text = Configuration.Settings.Language.General.Text;
            this.GroupBoxEditWord.Text = Configuration.Settings.Language.SpellCheck.WordNotFound;
            this.groupBoxSuggestions.Text = Configuration.Settings.Language.SpellCheck.Suggestions;
            this.groupBoxTextAsImage.Text = Configuration.Settings.Language.SpellCheck.ImageText;
            this.buttonAddToNames.Text = Configuration.Settings.Language.SpellCheck.AddToNamesAndIgnoreList;
            this.buttonChangeAllWholeText.Text = Configuration.Settings.Language.SpellCheck.ChangeAll;
            this.buttonGoogleIt.Text = Configuration.Settings.Language.Main.VideoControls.GoogleIt;
            Utilities.FixLargeFonts(this, this.buttonAddToNames);
        }

        /// <summary>
        /// Gets the action result.
        /// </summary>
        public Action ActionResult { get; private set; }

        /// <summary>
        /// Gets the word.
        /// </summary>
        public string Word { get; private set; }

        /// <summary>
        /// Gets the paragraph.
        /// </summary>
        public string Paragraph { get; private set; }

        /// <summary>
        /// Gets the original whole text.
        /// </summary>
        public string OriginalWholeText { get; private set; }

        /// <summary>
        /// The button abort click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAbortClick(object sender, EventArgs e)
        {
            this.ActionResult = Action.Abort;
            this.DialogResult = DialogResult.Abort;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="suggestions">
        /// The suggestions.
        /// </param>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <param name="bitmap">
        /// The bitmap.
        /// </param>
        internal void Initialize(string word, List<string> suggestions, string line, Bitmap bitmap)
        {
            this._originalWord = word;
            this.OriginalWholeText = line;
            this.pictureBoxText.Image = bitmap;
            this.textBoxWord.Text = word;
            this.richTextBoxParagraph.Text = line;
            this.textBoxWholeText.Text = line;
            this.listBoxSuggestions.Items.Clear();
            foreach (string suggestion in suggestions)
            {
                this.listBoxSuggestions.Items.Add(suggestion);
            }

            if (this.listBoxSuggestions.Items.Count > 0)
            {
                this.listBoxSuggestions.SelectedIndex = 0;
            }

            HighLightWord(this.richTextBoxParagraph, word);
            this.ButtonEditWordClick(null, null);
        }

        /// <summary>
        /// The high light word.
        /// </summary>
        /// <param name="richTextBoxParagraph">
        /// The rich text box paragraph.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        private static void HighLightWord(RichTextBox richTextBoxParagraph, string word)
        {
            if (word != null && richTextBoxParagraph.Text.Contains(word))
            {
                for (int i = 0; i < richTextBoxParagraph.Text.Length; i++)
                {
                    if (richTextBoxParagraph.Text.Substring(i).StartsWith(word))
                    {
                        bool startOk = i == 0;
                        if (!startOk)
                        {
                            startOk = (@" <>-""”“[]'‘`´¶()♪¿¡.…—!?,:;/" + Environment.NewLine).Contains(richTextBoxParagraph.Text[i - 1]);
                        }

                        if (startOk)
                        {
                            bool endOk = i + word.Length == richTextBoxParagraph.Text.Length;
                            if (!endOk)
                            {
                                endOk = (@" <>-""”“[]'‘`´¶()♪¿¡.…—!?,:;/" + Environment.NewLine).Contains(richTextBoxParagraph.Text[i + word.Length]);
                            }

                            if (endOk)
                            {
                                richTextBoxParagraph.SelectionStart = i + 1;
                                richTextBoxParagraph.SelectionLength = word.Length;
                                while (richTextBoxParagraph.SelectedText != word && richTextBoxParagraph.SelectionStart > 0)
                                {
                                    richTextBoxParagraph.SelectionStart = richTextBoxParagraph.SelectionStart - 1;
                                    richTextBoxParagraph.SelectionLength = word.Length;
                                }

                                if (richTextBoxParagraph.SelectedText == word)
                                {
                                    richTextBoxParagraph.SelectionColor = Color.Red;
                                }
                            }
                        }
                    }
                }

                richTextBoxParagraph.SelectionLength = 0;
                richTextBoxParagraph.SelectionStart = 0;
            }
        }

        /// <summary>
        /// The button edit whole text click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonEditWholeTextClick(object sender, EventArgs e)
        {
            this.groupBoxEditWholeText.BringToFront();
            this.groupBoxEditWholeText.Enabled = true;
            this.GroupBoxEditWord.SendToBack();
            this.GroupBoxEditWord.Enabled = false;
            this.buttonEditWord.Enabled = true;
            this.buttonEditWholeText.Enabled = false;
            this.textBoxWholeText.Focus();
        }

        /// <summary>
        /// The button edit word click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonEditWordClick(object sender, EventArgs e)
        {
            this.groupBoxEditWholeText.SendToBack();
            this.groupBoxEditWholeText.Enabled = false;
            this.GroupBoxEditWord.BringToFront();
            this.GroupBoxEditWord.Enabled = true;
            this.buttonEditWord.Enabled = false;
            this.buttonEditWholeText.Enabled = true;
            this.textBoxWord.Focus();
        }

        /// <summary>
        /// The button change all click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonChangeAllClick(object sender, EventArgs e)
        {
            if (this._originalWord == this.textBoxWord.Text.Trim())
            {
                MessageBox.Show("Word was not changed!");
                return;
            }

            this.Word = this.textBoxWord.Text;
            this.ActionResult = Action.ChangeAndSave;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button change click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonChangeClick(object sender, EventArgs e)
        {
            this.Word = this.textBoxWord.Text;
            this.ActionResult = Action.ChangeOnce;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button skip once click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSkipOnceClick(object sender, EventArgs e)
        {
            this.ActionResult = Action.SkipOnce;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button skip all click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSkipAllClick(object sender, EventArgs e)
        {
            this.Word = this.textBoxWord.Text;
            this.ActionResult = Action.SkipAll;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button use suggestion click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonUseSuggestionClick(object sender, EventArgs e)
        {
            if (this.listBoxSuggestions.SelectedIndex >= 0)
            {
                this.Word = this.listBoxSuggestions.SelectedItem.ToString();
                this.ActionResult = Action.UseSuggestion;
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The button use suggestion always click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonUseSuggestionAlwaysClick(object sender, EventArgs e)
        {
            if (this.listBoxSuggestions.SelectedIndex >= 0)
            {
                this.Word = this.listBoxSuggestions.Items[this.listBoxSuggestions.SelectedIndex].ToString();
                this.ActionResult = Action.AllwaysUseSuggestion;
                this.DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// The button change whole text click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonChangeWholeTextClick(object sender, EventArgs e)
        {
            this.Paragraph = this.textBoxWholeText.Text.Trim();
            this.ActionResult = Action.ChangeWholeText;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button change all whole text_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonChangeAllWholeText_Click(object sender, EventArgs e)
        {
            this.Paragraph = this.textBoxWholeText.Text.Trim();
            this.ActionResult = Action.ChangeAllWholeText;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button add to names click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAddToNamesClick(object sender, EventArgs e)
        {
            this.Word = this.textBoxWord.Text.Trim();
            this.ActionResult = Action.AddToNames;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button add to dictionary click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonAddToDictionaryClick(object sender, EventArgs e)
        {
            string s = this.textBoxWord.Text.Trim();
            if (s.Length == 0 || s.Contains(' '))
            {
                MessageBox.Show("Word should be one single word");
                return;
            }

            this.Word = s;
            if (this.Word.Length == 0)
            {
                this.ActionResult = Action.SkipOnce;
            }
            else
            {
                this.ActionResult = Action.AddToUserDictionary;
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button skip text click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ButtonSkipTextClick(object sender, EventArgs e)
        {
            this.Word = this.textBoxWord.Text;
            this.ActionResult = Action.SkipWholeText;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The text box word key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TextBoxWordKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.ButtonChangeClick(null, null);
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// The text box word_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxWord_TextChanged(object sender, EventArgs e)
        {
            this.buttonChange.Enabled = this.textBoxWord.Text != this._originalWord;
            this.buttonChangeAll.Enabled = this.buttonChange.Enabled;
        }

        /// <summary>
        /// The text box whole text_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void textBoxWholeText_TextChanged(object sender, EventArgs e)
        {
            this.buttonChangeWholeText.Enabled = this.textBoxWholeText.Text != this.OriginalWholeText;
            this.buttonChangeAllWholeText.Enabled = this.buttonChangeWholeText.Enabled;
        }

        /// <summary>
        /// The button google it_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGoogleIt_Click(object sender, EventArgs e)
        {
            string text = this.textBoxWord.Text;
            if (!string.IsNullOrWhiteSpace(text))
            {
                System.Diagnostics.Process.Start("https://www.google.com/search?q=" + Utilities.UrlEncode(text));
            }
        }

        /// <summary>
        /// The ocr spell check_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OcrSpellCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.G)
            {
                e.SuppressKeyPress = true;
                this.buttonGoogleIt_Click(null, null);
            }
        }
    }
}