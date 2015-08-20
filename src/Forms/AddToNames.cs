// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddToNames.cs" company="">
//   
// </copyright>
// <summary>
//   The add to names list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Dictionaries;

    /// <summary>
    /// The add to names list.
    /// </summary>
    public sealed partial class AddToNamesList : PositionAndSizeForm
    {
        /// <summary>
        /// The _language.
        /// </summary>
        private LanguageStructure.Main _language;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToNamesList"/> class.
        /// </summary>
        public AddToNamesList()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.AddToNames.Title;
            this.labelDescription.Text = Configuration.Settings.Language.AddToNames.Description;
            this.labelLanguage.Text = Configuration.Settings.Language.SpellCheck.Language;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.FixLargeFonts();
        }

        /// <summary>
        /// Gets the new name.
        /// </summary>
        public string NewName { get; private set; }

        /// <summary>
        /// The fix large fonts.
        /// </summary>
        private void FixLargeFonts()
        {
            if (this.labelDescription.Left + this.labelDescription.Width + 5 > this.Width)
            {
                this.Width = this.labelDescription.Left + this.labelDescription.Width + 5;
            }

            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public void Initialize(Subtitle subtitle, string text)
        {
            this._subtitle = subtitle;

            if (!string.IsNullOrEmpty(text))
            {
                this.textBoxAddName.Text = text.Trim().TrimEnd('.').TrimEnd('!').TrimEnd('?');
                if (this.textBoxAddName.Text.Length > 1)
                {
                    this.textBoxAddName.Text = char.ToUpper(this.textBoxAddName.Text[0]) + this.textBoxAddName.Text.Substring(1);
                }
            }

            this.comboBoxDictionaries.Items.Clear();
            string languageName = Utilities.AutoDetectLanguageName(Configuration.Settings.General.SpellCheckLanguage, this._subtitle);
            foreach (string name in Utilities.GetDictionaryLanguages())
            {
                this.comboBoxDictionaries.Items.Add(name);
                if (name.Contains("[" + languageName + "]"))
                {
                    this.comboBoxDictionaries.SelectedIndex = this.comboBoxDictionaries.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="hunspellName">
        /// The hunspell name.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        internal void Initialize(Subtitle subtitle, string hunspellName, string text)
        {
            this._subtitle = subtitle;

            if (!string.IsNullOrEmpty(text))
            {
                this.textBoxAddName.Text = text.Trim().TrimEnd('.').TrimEnd('!').TrimEnd('?');
                if (this.textBoxAddName.Text.Length > 1)
                {
                    this.textBoxAddName.Text = char.ToUpper(this.textBoxAddName.Text[0]) + this.textBoxAddName.Text.Substring(1);
                }
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
            if (string.IsNullOrWhiteSpace(this.textBoxAddName.Text))
            {
                return;
            }

            this.NewName = this.textBoxAddName.Text.Trim();
            string languageName = null;
            this._language = Configuration.Settings.Language.Main;

            if (!string.IsNullOrEmpty(Configuration.Settings.General.SpellCheckLanguage))
            {
                languageName = Configuration.Settings.General.SpellCheckLanguage;
            }
            else
            {
                List<string> list = Utilities.GetDictionaryLanguages();
                if (list.Count > 0)
                {
                    string name = list[0];
                    int start = name.LastIndexOf('[');
                    int end = name.LastIndexOf(']');
                    if (start > 0 && end > start)
                    {
                        start++;
                        name = name.Substring(start, end - start);
                        languageName = name;
                    }
                    else
                    {
                        MessageBox.Show(string.Format(this._language.InvalidLanguageNameX, name));
                        return;
                    }
                }
            }

            languageName = Utilities.AutoDetectLanguageName(languageName, this._subtitle);
            if (this.comboBoxDictionaries.Items.Count > 0)
            {
                string name = this.comboBoxDictionaries.SelectedItem.ToString();
                int start = name.LastIndexOf('[');
                int end = name.LastIndexOf(']');
                if (start >= 0 && end > start)
                {
                    start++;
                    name = name.Substring(start, end - start);
                    languageName = name;
                }
            }

            if (string.IsNullOrEmpty(languageName))
            {
                languageName = "en_US";
            }

            var namesList = new NamesList(Configuration.DictionariesFolder, languageName, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
            if (namesList.Add(this.textBoxAddName.Text))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}