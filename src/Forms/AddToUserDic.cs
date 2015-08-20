// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddToUserDic.cs" company="">
//   
// </copyright>
// <summary>
//   The add to user dic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The add to user dic.
    /// </summary>
    public partial class AddToUserDic : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddToUserDic"/> class.
        /// </summary>
        public AddToUserDic()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.AddToUserDictionary.Title;
            this.labelDescription.Text = Configuration.Settings.Language.AddToUserDictionary.Description;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the new word.
        /// </summary>
        public string NewWord { get; private set; }

        /// <summary>
        /// The add to user dic_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AddToUserDic_KeyDown(object sender, KeyEventArgs e)
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
            this.NewWord = this.textBoxAddName.Text.Trim().ToLower();
            if (this.NewWord.Length == 0)
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            string language = this.comboBoxDictionaries.Text;
            if (language.IndexOf('[') > 0)
            {
                language = language.Substring(language.IndexOf('[')).TrimStart('[');
            }

            if (language.IndexOf(']') > 0)
            {
                language = language.Substring(0, language.IndexOf(']'));
            }

            var userWordList = new List<string>();

            Utilities.LoadUserWordList(userWordList, language);
            if (!string.IsNullOrEmpty(language) && this.NewWord.Length > 0 && !userWordList.Contains(this.NewWord))
            {
                Utilities.AddToUserDictionary(this.NewWord, language);
                this.DialogResult = DialogResult.OK;
                return;
            }

            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="hunspellName">
        /// The hunspell name.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        internal void Initialize(string hunspellName, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                this.textBoxAddName.Text = text.Trim().TrimEnd('.', '!', '?');
            }

            this.comboBoxDictionaries.Items.Clear();
            foreach (string name in Utilities.GetDictionaryLanguages())
            {
                this.comboBoxDictionaries.Items.Add(name);
                if (hunspellName != null && name.Equals(hunspellName, StringComparison.OrdinalIgnoreCase))
                {
                    this.comboBoxDictionaries.SelectedIndex = this.comboBoxDictionaries.Items.Count - 1;
                }
            }
        }
    }
}