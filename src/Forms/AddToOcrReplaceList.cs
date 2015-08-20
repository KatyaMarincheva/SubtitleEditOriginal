// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddToOcrReplaceList.cs" company="">
//   
// </copyright>
// <summary>
//   The add to ocr replace list.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Dictionaries;

    /// <summary>
    /// The add to ocr replace list.
    /// </summary>
    public partial class AddToOcrReplaceList : Form
    {
        /// <summary>
        /// The _three letter iso language name.
        /// </summary>
        private string _threeLetterIsoLanguageName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToOcrReplaceList"/> class.
        /// </summary>
        public AddToOcrReplaceList()
        {
            this.InitializeComponent();
            this.Text = Configuration.Settings.Language.AddToOcrReplaceList.Title;
            this.labelDescription.Text = Configuration.Settings.Language.AddToOcrReplaceList.Description;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets or sets the new source.
        /// </summary>
        public string NewSource { get; set; }

        /// <summary>
        /// Gets or sets the new target.
        /// </summary>
        public string NewTarget { get; set; }

        /// <summary>
        /// Gets the language string.
        /// </summary>
        public string LanguageString
        {
            get
            {
                string name = this.comboBoxDictionaries.SelectedItem.ToString();
                int start = name.LastIndexOf('[');
                int end = name.LastIndexOf(']');
                if (start >= 0 && end > start)
                {
                    start++;
                    name = name.Substring(start, end - start);
                    return name;
                }

                return null;
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
            string key = this.textBoxOcrFixKey.Text.Trim();
            string value = this.textBoxOcrFixValue.Text.Trim();
            if (key.Length == 0 || value.Length == 0 || key == value)
            {
                return;
            }

            var languageString = this.LanguageString;
            if (languageString == null)
            {
                return;
            }

            try
            {
                var ci = new CultureInfo(languageString.Replace('_', '-'));
                this._threeLetterIsoLanguageName = ci.ThreeLetterISOLanguageName;
            }
            catch (CultureNotFoundException exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            var ocrFixReplaceList = OcrFixReplaceList.FromLanguageId(this._threeLetterIsoLanguageName);
            ocrFixReplaceList.AddWordOrPartial(key, value);
            this.DialogResult = DialogResult.OK;
            this.NewSource = key;
            this.NewTarget = value;
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
        /// The add to ocr replace list_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AddToOcrReplaceList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="languageId">
        /// The language id.
        /// </param>
        /// <param name="hunspellName">
        /// The hunspell name.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        internal void Initialize(string languageId, string hunspellName, string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                this.textBoxOcrFixKey.Text = source;
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

            this._threeLetterIsoLanguageName = languageId;
        }
    }
}