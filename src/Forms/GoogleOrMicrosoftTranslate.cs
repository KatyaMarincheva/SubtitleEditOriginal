// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleOrMicrosoftTranslate.cs" company="">
//   
// </copyright>
// <summary>
//   The google or microsoft translate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic;

    /// <summary>
    /// The google or microsoft translate.
    /// </summary>
    public sealed partial class GoogleOrMicrosoftTranslate : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleOrMicrosoftTranslate"/> class.
        /// </summary>
        public GoogleOrMicrosoftTranslate()
        {
            this.InitializeComponent();
            using (var gt = new GoogleTranslate())
            {
                gt.FillComboWithGoogleLanguages(this.comboBoxFrom);
                gt.FillComboWithGoogleLanguages(this.comboBoxTo);
            }

            RemovedLanguagesNotInMicrosoftTranslate(this.comboBoxFrom);
            RemovedLanguagesNotInMicrosoftTranslate(this.comboBoxTo);

            this.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.Title;
            this.labelGoogleTranslate.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.GoogleTranslate;
            this.labelMicrosoftTranslate.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.MicrosoftTranslate;
            this.labelFrom.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.From;
            this.labelTo.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.To;
            this.labelSourceText.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.SourceText;
            this.buttonGoogle.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.GoogleTranslate;
            this.buttonMicrosoft.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.MicrosoftTranslate;
            this.buttonTranslate.Text = Configuration.Settings.Language.GoogleOrMicrosoftTranslate.Translate;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            Utilities.FixLargeFonts(this, this.buttonCancel);
        }

        /// <summary>
        /// Gets or sets the translated text.
        /// </summary>
        public string TranslatedText { get; set; }

        /// <summary>
        /// The removed languages not in microsoft translate.
        /// </summary>
        /// <param name="comboBox">
        /// The combo box.
        /// </param>
        private static void RemovedLanguagesNotInMicrosoftTranslate(ComboBox comboBox)
        {
            for (int i = comboBox.Items.Count - 1; i > 0; i--)
            {
                var item = (GoogleTranslate.ComboBoxItem)comboBox.Items[i];
                if (item.Value != FixMsLocale(item.Value))
                {
                    comboBox.Items.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// The initialize from language.
        /// </summary>
        /// <param name="defaultFromLanguage">
        /// The default from language.
        /// </param>
        /// <param name="defaultToLanguage">
        /// The default to language.
        /// </param>
        internal void InitializeFromLanguage(string defaultFromLanguage, string defaultToLanguage)
        {
            if (defaultFromLanguage == defaultToLanguage)
            {
                defaultToLanguage = "en";
            }

            int i = 0;
            this.comboBoxFrom.SelectedIndex = 0;
            foreach (GoogleTranslate.ComboBoxItem item in this.comboBoxFrom.Items)
            {
                if (item.Value == defaultFromLanguage)
                {
                    this.comboBoxFrom.SelectedIndex = i;
                    break;
                }

                i++;
            }

            i = 0;
            this.comboBoxTo.SelectedIndex = 0;
            foreach (GoogleTranslate.ComboBoxItem item in this.comboBoxTo.Items)
            {
                if (item.Value == defaultToLanguage)
                {
                    this.comboBoxTo.SelectedIndex = i;
                    break;
                }

                i++;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        internal void Initialize(Paragraph paragraph)
        {
            this.textBoxSourceText.Text = paragraph.Text;
        }

        /// <summary>
        /// The google or microsoft translate_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GoogleOrMicrosoftTranslate_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            this.Translate();
        }

        /// <summary>
        /// The translate.
        /// </summary>
        private void Translate()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                string from = (this.comboBoxFrom.SelectedItem as GoogleTranslate.ComboBoxItem).Value;
                string to = (this.comboBoxTo.SelectedItem as GoogleTranslate.ComboBoxItem).Value;
                string languagePair = from + "|" + to;

                this.buttonGoogle.Text = string.Empty;
                this.buttonGoogle.Text = GoogleTranslate.TranslateTextViaApi(this.textBoxSourceText.Text, languagePair);

                using (var gt = new GoogleTranslate())
                {
                    Subtitle subtitle = new Subtitle();
                    subtitle.Paragraphs.Add(new Paragraph(0, 0, this.textBoxSourceText.Text));
                    gt.Initialize(subtitle, string.Empty, false, System.Text.Encoding.UTF8);
                    from = FixMsLocale(from);
                    to = FixMsLocale(to);
                    gt.DoMicrosoftTranslate(from, to);
                    this.buttonMicrosoft.Text = gt.TranslatedSubtitle.Paragraphs[0].Text;
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// The fix ms locale.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string FixMsLocale(string from)
        {
            if ("ar bg zh-CHS zh-CHT cs da nl en et fi fr de el ht he hu id it ja ko lv lt no pl pt ro ru sk sl es sv th tr uk vi".Contains(from))
            {
                return from;
            }

            return "en";
        }

        /// <summary>
        /// The button translate_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonTranslate_Click(object sender, EventArgs e)
        {
            this.Translate();
        }

        /// <summary>
        /// The button google_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            this.TranslatedText = this.buttonGoogle.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The button microsoft_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonMicrosoft_Click(object sender, EventArgs e)
        {
            this.TranslatedText = this.buttonMicrosoft.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The google or microsoft translate_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GoogleOrMicrosoftTranslate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}