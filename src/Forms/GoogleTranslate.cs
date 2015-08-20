// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GoogleTranslate.cs" company="">
//   
// </copyright>
// <summary>
//   The google translate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Services.Protocols;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.MicrosoftTranslationService;

    /// <summary>
    /// The google translate.
    /// </summary>
    public sealed partial class GoogleTranslate : PositionAndSizeForm
    {
        /// <summary>
        /// The splitter string.
        /// </summary>
        private const string SplitterString = " == ";

        /// <summary>
        /// The newline string.
        /// </summary>
        private const string NewlineString = " __ ";

        /// <summary>
        /// The _break translation.
        /// </summary>
        private bool _breakTranslation;

        /// <summary>
        /// The _formatting types.
        /// </summary>
        private FormattingType[] _formattingTypes;

        /// <summary>
        /// The _google api not working.
        /// </summary>
        private bool _googleApiNotWorking;

        /// <summary>
        /// The _google translate.
        /// </summary>
        private bool _googleTranslate = true;

        /// <summary>
        /// The _microsoft translation service.
        /// </summary>
        private SoapService _microsoftTranslationService;

        /// <summary>
        /// The _screen scraping encoding.
        /// </summary>
        private Encoding _screenScrapingEncoding;

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _translated subtitle.
        /// </summary>
        private Subtitle _translatedSubtitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleTranslate"/> class.
        /// </summary>
        public GoogleTranslate()
        {
            this.InitializeComponent();

            this.Text = Configuration.Settings.Language.GoogleTranslate.Title;
            this.labelFrom.Text = Configuration.Settings.Language.GoogleTranslate.From;
            this.labelTo.Text = Configuration.Settings.Language.GoogleTranslate.To;
            this.buttonTranslate.Text = Configuration.Settings.Language.GoogleTranslate.Translate;
            this.labelPleaseWait.Text = Configuration.Settings.Language.GoogleTranslate.PleaseWait;
            this.linkLabelPoweredByGoogleTranslate.Text = Configuration.Settings.Language.GoogleTranslate.PoweredByGoogleTranslate;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;

            this.subtitleListViewFrom.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            this.subtitleListViewTo.InitializeLanguage(Configuration.Settings.Language.General, Configuration.Settings);
            Utilities.InitializeSubtitleFont(this.subtitleListViewFrom);
            Utilities.InitializeSubtitleFont(this.subtitleListViewTo);
            this.subtitleListViewFrom.AutoSizeAllColumns(this);
            this.subtitleListViewTo.AutoSizeAllColumns(this);
            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// Gets the screen scraping encoding.
        /// </summary>
        public Encoding ScreenScrapingEncoding
        {
            get
            {
                return this._screenScrapingEncoding;
            }
        }

        /// <summary>
        /// Gets the translated subtitle.
        /// </summary>
        public Subtitle TranslatedSubtitle
        {
            get
            {
                return this._translatedSubtitle;
            }
        }

        /// <summary>
        /// Gets the ms translation service client.
        /// </summary>
        private SoapService MsTranslationServiceClient
        {
            get
            {
                if (this._microsoftTranslationService == null)
                {
                    this._microsoftTranslationService = new MicrosoftTranslationService.SoapService { Proxy = Utilities.GetProxy() };
                }

                return this._microsoftTranslationService;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="googleTranslate">
        /// The google translate.
        /// </param>
        /// <param name="encoding">
        /// The encoding.
        /// </param>
        internal void Initialize(Subtitle subtitle, string title, bool googleTranslate, Encoding encoding)
        {
            if (title != null)
            {
                this.Text = title;
            }

            this._googleTranslate = googleTranslate;
            if (!this._googleTranslate)
            {
                this.linkLabelPoweredByGoogleTranslate.Text = Configuration.Settings.Language.GoogleTranslate.PoweredByMicrosoftTranslate;
            }

            this.labelPleaseWait.Visible = false;
            this.progressBar1.Visible = false;
            this._subtitle = subtitle;
            this._translatedSubtitle = new Subtitle(subtitle);

            string defaultFromLanguage = Utilities.AutoDetectGoogleLanguage(encoding); // Guess language via encoding
            if (string.IsNullOrEmpty(defaultFromLanguage))
            {
                defaultFromLanguage = Utilities.AutoDetectGoogleLanguage(subtitle); // Guess language based on subtitle contents
            }

            this.FillComboWithLanguages(this.comboBoxFrom);
            int i = 0;
            foreach (ComboBoxItem item in this.comboBoxFrom.Items)
            {
                if (item.Value == defaultFromLanguage)
                {
                    this.comboBoxFrom.SelectedIndex = i;
                    break;
                }

                i++;
            }

            this.FillComboWithLanguages(this.comboBoxTo);
            i = 0;
            string uiCultureTargetLanguage = Configuration.Settings.Tools.GoogleTranslateLastTargetLanguage;
            if (uiCultureTargetLanguage == defaultFromLanguage)
            {
                foreach (string s in Utilities.GetDictionaryLanguages())
                {
                    string temp = s.Replace("[", string.Empty).Replace("]", string.Empty);
                    if (temp.Length > 4)
                    {
                        temp = temp.Substring(temp.Length - 5, 2).ToLower();

                        if (temp != uiCultureTargetLanguage)
                        {
                            uiCultureTargetLanguage = temp;
                            break;
                        }
                    }
                }
            }

            this.comboBoxTo.SelectedIndex = 0;
            foreach (ComboBoxItem item in this.comboBoxTo.Items)
            {
                if (item.Value == uiCultureTargetLanguage)
                {
                    this.comboBoxTo.SelectedIndex = i;
                    break;
                }

                i++;
            }

            this.subtitleListViewFrom.Fill(subtitle);
            this.GoogleTranslate_Resize(null, null);

            this._googleApiNotWorking = !Configuration.Settings.Tools.UseGooleApiPaidService; // google has closed their free api service :(
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
            if (this.buttonTranslate.Text == Configuration.Settings.Language.General.Cancel)
            {
                this.buttonTranslate.Enabled = false;
                this._breakTranslation = true;
                this.buttonOK.Enabled = true;
                this.buttonCancel.Enabled = true;
                return;
            }

            // empty all texts
            foreach (Paragraph p in this._translatedSubtitle.Paragraphs)
            {
                p.Text = string.Empty;
            }

            if (!this._googleTranslate)
            {
                string from = (this.comboBoxFrom.SelectedItem as ComboBoxItem).Value;
                string to = (this.comboBoxTo.SelectedItem as ComboBoxItem).Value;
                this.DoMicrosoftTranslate(from, to);
                return;
            }

            this._formattingTypes = new FormattingType[this._subtitle.Paragraphs.Count];

            this.buttonOK.Enabled = false;
            this.buttonCancel.Enabled = false;
            this._breakTranslation = false;
            this.buttonTranslate.Text = Configuration.Settings.Language.General.Cancel;
            const int textMaxSize = 1000;
            Cursor.Current = Cursors.WaitCursor;
            this.progressBar1.Maximum = this._subtitle.Paragraphs.Count;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = true;
            this.labelPleaseWait.Visible = true;
            int start = 0;
            try
            {
                var sb = new StringBuilder();
                int index = 0;
                for (int i = 0; i < this._subtitle.Paragraphs.Count; i++)
                {
                    Paragraph p = this._subtitle.Paragraphs[i];
                    string text = p.Text.Trim();
                    if (text.StartsWith("<i>", StringComparison.Ordinal) && text.EndsWith("</i>", StringComparison.Ordinal) && text.Contains("</i>" + Environment.NewLine + "<i>") && Utilities.GetNumberOfLines(text) == 2 && Utilities.CountTagInText(text, "<i>") == 1)
                    {
                        this._formattingTypes[i] = FormattingType.ItalicTwoLines;
                        text = HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagItalic);
                    }
                    else if (text.StartsWith("<i>", StringComparison.Ordinal) && text.EndsWith("</i>", StringComparison.Ordinal) && Utilities.CountTagInText(text, "<i>") == 1)
                    {
                        this._formattingTypes[i] = FormattingType.Italic;
                        text = text.Substring(3, text.Length - 7);
                    }
                    else
                    {
                        this._formattingTypes[i] = FormattingType.None;
                    }

                    text = string.Format("{1} {0} |", text, SplitterString);
                    if (Utilities.UrlEncode(sb + text).Length >= textMaxSize)
                    {
                        this.FillTranslatedText(this.DoTranslate(sb.ToString()), start, index - 1);
                        sb = new StringBuilder();
                        this.progressBar1.Refresh();
                        Application.DoEvents();
                        start = index;
                    }

                    sb.Append(text);
                    index++;
                    this.progressBar1.Value = index;
                    if (this._breakTranslation)
                    {
                        break;
                    }
                }

                if (sb.Length > 0)
                {
                    this.FillTranslatedText(this.DoTranslate(sb.ToString()), start, index - 1);
                }
            }
            catch (WebException webException)
            {
                MessageBox.Show(webException.Source + ": " + webException.Message);
            }
            finally
            {
                this.labelPleaseWait.Visible = false;
                this.progressBar1.Visible = false;
                Cursor.Current = Cursors.Default;
                this.buttonTranslate.Text = Configuration.Settings.Language.GoogleTranslate.Translate;
                this.buttonTranslate.Enabled = true;
                this.buttonOK.Enabled = true;
                this.buttonCancel.Enabled = true;

                Configuration.Settings.Tools.GoogleTranslateLastTargetLanguage = (this.comboBoxTo.SelectedItem as ComboBoxItem).Value;
            }
        }

        /// <summary>
        /// The fill translated text.
        /// </summary>
        /// <param name="translatedText">
        /// The translated text.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        private void FillTranslatedText(string translatedText, int start, int end)
        {
            int index = start;
            foreach (string s in translatedText.Split(new[] { "|" }, StringSplitOptions.None))
            {
                if (index < this._translatedSubtitle.Paragraphs.Count)
                {
                    string cleanText = s.Replace("</p>", string.Empty).Trim();
                    int indexOfP = cleanText.IndexOf(SplitterString.Trim(), StringComparison.Ordinal);
                    if (indexOfP >= 0 && indexOfP < 4)
                    {
                        cleanText = cleanText.Remove(0, indexOfP);
                    }

                    cleanText = cleanText.Replace(SplitterString.Trim(), string.Empty).Trim();
                    if (cleanText.Contains('\n') && !cleanText.Contains('\r'))
                    {
                        cleanText = cleanText.Replace("\n", Environment.NewLine);
                    }

                    cleanText = cleanText.Replace(" ...", "...");
                    cleanText = cleanText.Replace(NewlineString.Trim(), Environment.NewLine);
                    cleanText = cleanText.Replace("<br />", Environment.NewLine);
                    cleanText = cleanText.Replace("<br/>", Environment.NewLine);
                    cleanText = cleanText.Replace("<br />", Environment.NewLine);
                    cleanText = cleanText.Replace(Environment.NewLine + " ", Environment.NewLine);
                    cleanText = cleanText.Replace(" " + Environment.NewLine, Environment.NewLine);
                    cleanText = cleanText.Replace("<I>", "<i>");
                    cleanText = cleanText.Replace("< I>", "<i>");
                    cleanText = cleanText.Replace("</ i>", "</i>");
                    cleanText = cleanText.Replace("</ I>", "</i>");
                    cleanText = cleanText.Replace("</I>", "</i>");
                    cleanText = cleanText.Replace("< i >", "<i>");
                    if (cleanText.StartsWith("<i> ", StringComparison.Ordinal))
                    {
                        cleanText = cleanText.Remove(3, 1);
                    }

                    if (cleanText.EndsWith(" </i>", StringComparison.Ordinal))
                    {
                        cleanText = cleanText.Remove(cleanText.Length - 5, 1);
                    }

                    cleanText = cleanText.Replace(Environment.NewLine + "<i> ", Environment.NewLine + "<i>");
                    cleanText = cleanText.Replace(" </i>" + Environment.NewLine, "</i>" + Environment.NewLine);

                    if (this._formattingTypes[index] == FormattingType.ItalicTwoLines || this._formattingTypes[index] == FormattingType.Italic)
                    {
                        this._translatedSubtitle.Paragraphs[index].Text = "<i>" + cleanText + "</i>";
                    }
                    else
                    {
                        this._translatedSubtitle.Paragraphs[index].Text = cleanText;
                    }
                }

                index++;
            }

            this.subtitleListViewTo.Fill(this._translatedSubtitle);
            this.subtitleListViewTo.SelectIndexAndEnsureVisible(end);
        }

        /// <summary>
        /// The do translate.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string DoTranslate(string input)
        {
            string languagePair = (this.comboBoxFrom.SelectedItem as ComboBoxItem).Value + "|" + (this.comboBoxTo.SelectedItem as ComboBoxItem).Value;
            bool romanji = languagePair.EndsWith("|romanji", StringComparison.InvariantCulture);
            if (romanji)
            {
                languagePair = (this.comboBoxFrom.SelectedItem as ComboBoxItem).Value + "|ja";
            }

            input = this.PreTranslate(input.TrimEnd('|').Trim());

            string result = null;
            if (!this._googleApiNotWorking)
            {
                try
                {
                    result = TranslateTextViaApi(input, languagePair);
                }
                catch
                {
                    this._googleApiNotWorking = true;
                    result = string.Empty;
                }
            }

            // fallback to screen scraping
            if (string.IsNullOrEmpty(result))
            {
                if (this._screenScrapingEncoding == null)
                {
                    this._screenScrapingEncoding = GetScreenScrapingEncoding(languagePair);
                }

                result = TranslateTextViaScreenScraping(input, languagePair, this._screenScrapingEncoding, romanji);
                this._googleApiNotWorking = true;
            }

            return this.PostTranslate(result);
        }

        /// <summary>
        /// The translate text via api.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="languagePair">
        /// The language pair.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string TranslateTextViaApi(string input, string languagePair)
        {
            // string googleApiKey = "ABQIAAAA4j5cWwa3lDH0RkZceh7PjBTDmNAghl5kWSyuukQ0wtoJG8nFBxRPlalq-gAvbeCXMCkmrysqjXV1Gw";
            string googleApiKey = Configuration.Settings.Tools.GoogleApiKey;

            input = input.Replace(Environment.NewLine, NewlineString);
            input = input.Replace("'", "&apos;");

            // create the web request to the Google Translate REST interface

            // API V 1.0
            var uri = new Uri("http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&q=" + Utilities.UrlEncode(input) + "&langpair=" + languagePair + "&key=" + googleApiKey);

            // API V 2.0 ?
            // string[] arr = languagePair.Split('|');
            // string from = arr[0];
            // string to = arr[1];
            // string url = String.Format("https://www.googleapis.com/language/translate/v2?key={3}&q={0}&source={1}&target={2}", HttpUtility.UrlEncode(input), from, to, googleApiKey);
            var request = WebRequest.Create(uri);
            request.Proxy = Utilities.GetProxy();
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();

            var indexOfTranslatedText = content.IndexOf("{\"translatedText\":", StringComparison.Ordinal);
            if (indexOfTranslatedText >= 0)
            {
                var start = indexOfTranslatedText + 19;
                int end = content.IndexOf("\"}", start, StringComparison.Ordinal);
                string translatedText = content.Substring(start, end - start);
                string test = translatedText.Replace("\\u003c", "<");
                test = test.Replace("\\u003e", ">");
                test = test.Replace("\\u0026#39;", "'");
                test = test.Replace("\\u0026amp;", "&");
                test = test.Replace("\\u0026quot;", "\"");
                test = test.Replace("\\u0026apos;", "'");
                test = test.Replace("\\u0026lt;", "<");
                test = test.Replace("\\u0026gt;", ">");
                test = test.Replace("\\u003d", "=");
                test = test.Replace("\\u200b", string.Empty);
                test = test.Replace("\\\"", "\"");
                test = test.Replace(" <br/>", Environment.NewLine);
                test = test.Replace("<br/>", Environment.NewLine);
                test = RemovePStyleParameters(test);
                return test;
            }

            return string.Empty;
        }

        /// <summary>
        /// The remove p style parameters.
        /// </summary>
        /// <param name="test">
        /// The test.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string RemovePStyleParameters(string test)
        {
            var startPosition = test.IndexOf("<p style", StringComparison.Ordinal);
            while (startPosition >= 0)
            {
                var endPosition = test.IndexOf('>', startPosition + 8);
                if (endPosition > 0)
                {
                    return test.Remove(startPosition + 2, endPosition - startPosition - 2);
                }
            }

            return test;
        }

        /// <summary>
        /// The get screen scraping encoding.
        /// </summary>
        /// <param name="languagePair">
        /// The language pair.
        /// </param>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        public static Encoding GetScreenScrapingEncoding(string languagePair)
        {
            try
            {
                string url = string.Format("https://translate.google.com/?hl=en&eotf=1&sl={0}&tl={1}&q={2}", languagePair.Substring(0, 2), languagePair.Substring(3), "123 456");
                var result = Utilities.DownloadString(url).ToLower();
                int idx = result.IndexOf("charset", StringComparison.Ordinal);
                int end = result.IndexOf('"', idx + 8);
                string charset = result.Substring(idx, end - idx).Replace("charset=", string.Empty);
                return Encoding.GetEncoding(charset); // "koi8-r");
            }
            catch
            {
                return Encoding.Default;
            }
        }

        /// <summary>
        /// Translate Text using Google Translate API's
        /// Google URL - https://www.google.com/translate_t?hl=en&amp;ie=UTF8&amp;text={0}&amp;langpair={1}
        /// </summary>
        /// <param name="input">
        /// Input string
        /// </param>
        /// <param name="languagePair">
        /// 2 letter Language Pair, delimited by "|".
        /// E.g. "ar|en" language pair means to translate from Arabic to English
        /// </param>
        /// <param name="encoding">
        /// Encoding to use when downloading text
        /// </param>
        /// <param name="romanji">
        /// Get Romanjii text (made during Japanese) but in a separate div tag
        /// </param>
        /// <returns>
        /// Translated to String
        /// </returns>
        public static string TranslateTextViaScreenScraping(string input, string languagePair, Encoding encoding, bool romanji)
        {
            input = input.Replace(Environment.NewLine, NewlineString);

            // input = input.Replace("'", "&apos;");

            // string url = String.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", HttpUtility.UrlEncode(input), languagePair);
            string url = string.Format("https://translate.google.com/?hl=en&eotf=1&sl={0}&tl={1}&q={2}", languagePair.Substring(0, 2), languagePair.Substring(3), Utilities.UrlEncode(input));
            var result = Utilities.DownloadString(url, encoding);

            var sb = new StringBuilder();
            if (romanji)
            {
                int startIndex = result.IndexOf("<div id=res-translit", StringComparison.Ordinal);
                if (startIndex > 0)
                {
                    startIndex = result.IndexOf('>', startIndex);
                    if (startIndex > 0)
                    {
                        startIndex++;
                        int endIndex = result.IndexOf("</div>", startIndex, StringComparison.Ordinal);
                        string translatedText = result.Substring(startIndex, endIndex - startIndex);
                        string test = WebUtility.HtmlDecode(translatedText);
                        test = test.Replace("= =", SplitterString).Replace("  ", " ");
                        test = test.Replace("_ _", NewlineString).Replace("  ", " ");
                        sb.Append(test);
                    }
                }
            }
            else
            {
                int startIndex = result.IndexOf("<span id=result_box", StringComparison.Ordinal);
                if (startIndex > 0)
                {
                    startIndex = result.IndexOf("<span title=", startIndex, StringComparison.Ordinal);
                    while (startIndex > 0)
                    {
                        startIndex = result.IndexOf('>', startIndex);
                        if (startIndex > 0)
                        {
                            startIndex++;
                            int endIndex = result.IndexOf("</span>", startIndex, StringComparison.Ordinal);
                            string translatedText = result.Substring(startIndex, endIndex - startIndex);
                            string test = WebUtility.HtmlDecode(translatedText);
                            sb.Append(test);
                            startIndex = result.IndexOf("<span title=", startIndex, StringComparison.Ordinal);
                        }
                    }
                }
            }

            string res = sb.ToString();
            res = res.Replace(NewlineString, Environment.NewLine);
            res = res.Replace("<BR>", Environment.NewLine);
            res = res.Replace("<BR />", Environment.NewLine);
            res = res.Replace("<BR/>", Environment.NewLine);
            res = res.Replace("< br />", Environment.NewLine);
            res = res.Replace("< br / >", Environment.NewLine);
            res = res.Replace("<br / >", Environment.NewLine);
            res = res.Replace(" <br/>", Environment.NewLine);
            res = res.Replace(" <br/>", Environment.NewLine);
            res = res.Replace("<br/>", Environment.NewLine);
            res = res.Replace("<br />", Environment.NewLine);
            res = res.Replace("  ", " ").Trim();
            res = res.Replace(Environment.NewLine + " ", Environment.NewLine);
            res = res.Replace(Environment.NewLine + " ", Environment.NewLine);
            res = res.Replace(" " + Environment.NewLine, Environment.NewLine);
            res = res.Replace(" " + Environment.NewLine, Environment.NewLine).Trim();
            int end = res.LastIndexOf("<p>", StringComparison.Ordinal);
            if (end > 0)
            {
                res = res.Substring(0, end);
            }

            return res;
        }

        /// <summary>
        /// The fill combo with languages.
        /// </summary>
        /// <param name="comboBox">
        /// The combo box.
        /// </param>
        public void FillComboWithLanguages(ComboBox comboBox)
        {
            if (!this._googleTranslate)
            {
                if (comboBox == this.comboBoxTo)
                {
                    foreach (ComboBoxItem item in this.comboBoxFrom.Items)
                    {
                        this.comboBoxTo.Items.Add(new ComboBoxItem(item.Text, item.Value));
                    }

                    return;
                }

                // MicrosoftTranslationService.SoapService client = MsTranslationServiceClient;

                // string[] locales = client.GetLanguagesForTranslate(BingApiId);
                string[] locales = GetMsLocales();

                // string[] names = client.GetLanguageNames(BingApiId, "en", locales);
                string[] names = GetMsNames();

                for (int i = 0; i < locales.Length; i++)
                {
                    if (names.Length > i && locales.Length > i)
                    {
                        comboBox.Items.Add(new ComboBoxItem(names[i], locales[i]));
                    }
                }

                return;
            }

            this.FillComboWithGoogleLanguages(comboBox);
        }

        /// <summary>
        /// The get ms locales.
        /// </summary>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] GetMsLocales()
        {
            return new[] { "ar", "bg", "zh-CHS", "zh-CHT", "cs", "da", "nl", "en", "et", "fi", "fr", "de", "el", "ht", "he", "hu", "id", "it", "ja", "ko", "lv", "lt", "no", "pl", "pt", "ro", "ru", "sk", "sl", "es", "sv", "th", "tr", "uk", "vi" };
        }

        /// <summary>
        /// The get ms names.
        /// </summary>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private static string[] GetMsNames()
        {
            return new[] { "Arabic", "Bulgarian", "Chinese Simplified", "Chinese Traditional", "Czech", "Danish", "Dutch", "English", "Estonian", "Finnish", "French", "German", "Greek", "Haitian Creole", "Hebrew", "Hungarian", "Indonesian", "Italian", "Japanese", "Korean", "Latvian", "Lithuanian", "Norwegian", "Polish", "Portuguese", "Romanian", "Russian", "Slovak", "Slovenian", "Spanish", "Swedish", "Thai", "Turkish", "Ukrainian", "Vietnamese" };
        }

        /// <summary>
        /// The fill combo with google languages.
        /// </summary>
        /// <param name="comboBox">
        /// The combo box.
        /// </param>
        public void FillComboWithGoogleLanguages(ComboBox comboBox)
        {
            comboBox.Items.Add(new ComboBoxItem("AFRIKAANS", "af"));
            comboBox.Items.Add(new ComboBoxItem("ALBANIAN", "sq"));

            // comboBox.Items.Add(new ComboBoxItem("AMHARIC" , "am"));
            comboBox.Items.Add(new ComboBoxItem("ARABIC", "ar"));
            comboBox.Items.Add(new ComboBoxItem("ARMENIAN", "hy"));
            comboBox.Items.Add(new ComboBoxItem("AZERBAIJANI", "az"));
            comboBox.Items.Add(new ComboBoxItem("BASQUE", "eu"));
            comboBox.Items.Add(new ComboBoxItem("BELARUSIAN", "be"));
            comboBox.Items.Add(new ComboBoxItem("BENGALI", "bn"));

            // comboBox.Items.Add(new ComboBoxItem("BIHARI" , "bh"));
            comboBox.Items.Add(new ComboBoxItem("BOSNIAN", "bs"));
            comboBox.Items.Add(new ComboBoxItem("BULGARIAN", "bg"));

            // comboBox.Items.Add(new ComboBoxItem("BURMESE" , "my"));
            comboBox.Items.Add(new ComboBoxItem("CATALAN", "ca"));
            comboBox.Items.Add(new ComboBoxItem("CEBUANO", "ceb"));

            // comboBox.Items.Add(new ComboBoxItem("CHEROKEE" , "chr"));
            comboBox.Items.Add(new ComboBoxItem("CHINESE", "zh"));
            comboBox.Items.Add(new ComboBoxItem("CHINESE_SIMPLIFIED", "zh-CN"));
            comboBox.Items.Add(new ComboBoxItem("CHINESE_TRADITIONAL", "zh-TW"));
            comboBox.Items.Add(new ComboBoxItem("CROATIAN", "hr"));
            comboBox.Items.Add(new ComboBoxItem("CZECH", "cs"));
            comboBox.Items.Add(new ComboBoxItem("DANISH", "da"));

            // comboBox.Items.Add(new ComboBoxItem("DHIVEHI" , "dv"));
            comboBox.Items.Add(new ComboBoxItem("DUTCH", "nl"));
            comboBox.Items.Add(new ComboBoxItem("ENGLISH", "en"));
            comboBox.Items.Add(new ComboBoxItem("ESPERANTO", "eo"));
            comboBox.Items.Add(new ComboBoxItem("ESTONIAN", "et"));
            comboBox.Items.Add(new ComboBoxItem("FILIPINO", "tl"));
            comboBox.Items.Add(new ComboBoxItem("FINNISH", "fi"));
            comboBox.Items.Add(new ComboBoxItem("FRENCH", "fr"));
            comboBox.Items.Add(new ComboBoxItem("GALICIAN", "gl"));
            comboBox.Items.Add(new ComboBoxItem("GEORGIAN", "ka"));
            comboBox.Items.Add(new ComboBoxItem("GERMAN", "de"));
            comboBox.Items.Add(new ComboBoxItem("GREEK", "el"));

            // comboBox.Items.Add(new ComboBoxItem("GUARANI" , "gn"));
            comboBox.Items.Add(new ComboBoxItem("GUJARATI", "gu"));
            comboBox.Items.Add(new ComboBoxItem("HAITIAN CREOLE", "ht"));
            comboBox.Items.Add(new ComboBoxItem("HAUSA", "ha"));
            comboBox.Items.Add(new ComboBoxItem("HEBREW", "iw"));
            comboBox.Items.Add(new ComboBoxItem("HINDI", "hi"));
            comboBox.Items.Add(new ComboBoxItem("HMOUNG", "hmn"));
            comboBox.Items.Add(new ComboBoxItem("HUNGARIAN", "hu"));
            comboBox.Items.Add(new ComboBoxItem("ICELANDIC", "is"));
            comboBox.Items.Add(new ComboBoxItem("IGBO", "ig"));
            comboBox.Items.Add(new ComboBoxItem("INDONESIAN", "id"));
            comboBox.Items.Add(new ComboBoxItem("IRISH", "ga"));

            // comboBox.Items.Add(new ComboBoxItem("INUKTITUT" , "iu"));
            comboBox.Items.Add(new ComboBoxItem("ITALIAN", "it"));
            comboBox.Items.Add(new ComboBoxItem("JAPANESE", "ja"));
            comboBox.Items.Add(new ComboBoxItem("JAVANESE", "jw"));
            comboBox.Items.Add(new ComboBoxItem("KANNADA", "kn"));

            // comboBox.Items.Add(new ComboBoxItem("KAZAKH" , "kk"));
            comboBox.Items.Add(new ComboBoxItem("KHMER", "km"));
            comboBox.Items.Add(new ComboBoxItem("KOREAN", "ko"));

            // comboBox.Items.Add(new ComboBoxItem("KURDISH", "ku"));
            // comboBox.Items.Add(new ComboBoxItem("KYRGYZ", "ky"));
            comboBox.Items.Add(new ComboBoxItem("LAO", "lo"));
            comboBox.Items.Add(new ComboBoxItem("LATIN", "la"));
            comboBox.Items.Add(new ComboBoxItem("LATVIAN", "lv"));
            comboBox.Items.Add(new ComboBoxItem("LITHUANIAN", "lt"));
            comboBox.Items.Add(new ComboBoxItem("MACEDONIAN", "mk"));
            comboBox.Items.Add(new ComboBoxItem("MALAY", "ms"));
            comboBox.Items.Add(new ComboBoxItem("MALAYALAM", "ml"));
            comboBox.Items.Add(new ComboBoxItem("MALTESE", "mt"));
            comboBox.Items.Add(new ComboBoxItem("MAORI", "mi"));
            comboBox.Items.Add(new ComboBoxItem("MARATHI", "mr"));
            comboBox.Items.Add(new ComboBoxItem("MONGOLIAN", "mn"));
            comboBox.Items.Add(new ComboBoxItem("NEPALI", "ne"));
            comboBox.Items.Add(new ComboBoxItem("NORWEGIAN", "no"));

            // comboBox.Items.Add(new ComboBoxItem("ORIYA" , "or"));
            // comboBox.Items.Add(new ComboBoxItem("PASHTO" , "ps"));
            comboBox.Items.Add(new ComboBoxItem("PERSIAN", "fa"));
            comboBox.Items.Add(new ComboBoxItem("POLISH", "pl"));
            comboBox.Items.Add(new ComboBoxItem("PORTUGUESE", "pt"));
            comboBox.Items.Add(new ComboBoxItem("PUNJABI", "pa"));
            comboBox.Items.Add(new ComboBoxItem("ROMANIAN", "ro"));

            if (comboBox == this.comboBoxTo && !this._googleApiNotWorking)
            {
                comboBox.Items.Add(new ComboBoxItem("ROMANJI", "romanji"));
            }

            comboBox.Items.Add(new ComboBoxItem("RUSSIAN", "ru"));

            // comboBox.Items.Add(new ComboBoxItem("SANSKRIT" , "sa"));
            comboBox.Items.Add(new ComboBoxItem("SERBIAN", "sr"));

            // comboBox.Items.Add(new ComboBoxItem("SINDHI" , "sd"));
            comboBox.Items.Add(new ComboBoxItem("SESOTHO", "st"));
            comboBox.Items.Add(new ComboBoxItem("SINHALA", "si"));
            comboBox.Items.Add(new ComboBoxItem("SLOVAK", "sk"));
            comboBox.Items.Add(new ComboBoxItem("SLOVENIAN", "sl"));
            comboBox.Items.Add(new ComboBoxItem("SOMALI", "so"));
            comboBox.Items.Add(new ComboBoxItem("SPANISH", "es"));
            comboBox.Items.Add(new ComboBoxItem("SWAHILI", "sw"));
            comboBox.Items.Add(new ComboBoxItem("SWEDISH", "sv"));

            // comboBox.Items.Add(new ComboBoxItem("TAJIK" , "tg"));
            comboBox.Items.Add(new ComboBoxItem("TAMIL", "ta"));

            // comboBox.Items.Add(new ComboBoxItem("TAGALOG" , "tl"));
            comboBox.Items.Add(new ComboBoxItem("TELUGU", "te"));
            comboBox.Items.Add(new ComboBoxItem("THAI", "th"));

            // comboBox.Items.Add(new ComboBoxItem("TIBETAN" , "bo"));
            comboBox.Items.Add(new ComboBoxItem("TURKISH", "tr"));
            comboBox.Items.Add(new ComboBoxItem("UKRAINIAN", "uk"));
            comboBox.Items.Add(new ComboBoxItem("URDU", "ur"));
            comboBox.Items.Add(new ComboBoxItem("UZBEK", "uz"));

            // comboBox.Items.Add(new ComboBoxItem("UIGHUR" , "ug"));
            comboBox.Items.Add(new ComboBoxItem("VIETNAMESE", "vi"));
            comboBox.Items.Add(new ComboBoxItem("WELSH", "cy"));
            comboBox.Items.Add(new ComboBoxItem("YIDDISH", "yi"));
            comboBox.Items.Add(new ComboBoxItem("YORUBA", "yo"));
            comboBox.Items.Add(new ComboBoxItem("ZULU", "zu"));
        }

        /// <summary>
        /// The link label 1 link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LinkLabel1LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this._googleTranslate)
            {
                System.Diagnostics.Process.Start("https://www.google.com/translate");
            }
            else
            {
                System.Diagnostics.Process.Start("http://www.microsofttranslator.com/");
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
            if (this.subtitleListViewTo.Items.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// The pre translate.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string PreTranslate(string s)
        {
            if ((this.comboBoxFrom.SelectedItem as ComboBoxItem).Value == "en")
            {
                s = Regex.Replace(s, @"\bI'm ", "I am ");
                s = Regex.Replace(s, @"\bI've ", "I have ");
                s = Regex.Replace(s, @"\bI'll ", "I will ");
                s = Regex.Replace(s, @"\bI'd ", "I would "); // had or would???
                s = Regex.Replace(s, @"\b(I|i)t's ", "$1t is ");
                s = Regex.Replace(s, @"\b(Y|y)ou're ", "$1ou are ");
                s = Regex.Replace(s, @"\b(Y|y)ou've ", "$1ou have ");
                s = Regex.Replace(s, @"\b(Y|y)ou'll ", "$1ou will ");
                s = Regex.Replace(s, @"\b(Y|y)ou'd ", "$1ou would "); // had or would???
                s = Regex.Replace(s, @"\b(H|h)e's ", "$1e is ");
                s = Regex.Replace(s, @"\b(S|s)he's ", "$1he is ");
                s = Regex.Replace(s, @"\b(W|w)e're ", "$1e are ");
                s = Regex.Replace(s, @"\bwon't ", "will not ");
                s = Regex.Replace(s, @"\b(W|w)e're ", "$1e are ");
                s = Regex.Replace(s, @"\bwon't ", "will not ");
                s = Regex.Replace(s, @"\b(T|t)hey're ", "$1hey are ");
                s = Regex.Replace(s, @"\b(W|w)ho's ", "$1ho is ");
                s = Regex.Replace(s, @"\b(T|t)hat's ", "$1hat is ");
                s = Regex.Replace(s, @"\b(W|w)hat's ", "$1hat is ");
                s = Regex.Replace(s, @"\b(W|w)here's ", "$1here is ");
                s = Regex.Replace(s, @"\b(W|w)ho's ", "$1ho is ");
                s = Regex.Replace(s, @"\B'(C|c)ause ", "$1ecause "); // \b (word boundry) does not workig with '
            }

            return s;
        }

        /// <summary>
        /// The post translate.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string PostTranslate(string s)
        {
            if ((this.comboBoxTo.SelectedItem as ComboBoxItem).Value == "da")
            {
                s = s.Replace("Jeg ved.", "Jeg ved det.");
                s = s.Replace(", jeg ved.", ", jeg ved det.");

                s = s.Replace("Jeg er ked af.", "Jeg er ked af det.");
                s = s.Replace(", jeg er ked af.", ", jeg er ked af det.");

                s = s.Replace("Come on.", "Kom nu.");
                s = s.Replace(", come on.", ", kom nu.");
                s = s.Replace("Come on,", "Kom nu,");

                s = s.Replace("Hey ", "Hej ");
                s = s.Replace("Hey,", "Hej,");

                s = s.Replace(" gonna ", " ville ");
                s = s.Replace("Gonna ", "Vil ");

                s = s.Replace("Ked af.", "Undskyld.");
            }

            return s;
        }

        /// <summary>
        /// The form google translate_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormGoogleTranslate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && this.labelPleaseWait.Visible == false)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.Escape && this.labelPleaseWait.Visible)
            {
                this._breakTranslation = true;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#translation");
            }
            else if (e.Control && e.Shift && e.Alt && e.KeyCode == Keys.L)
            {
                Cursor.Current = Cursors.WaitCursor;
                Configuration.Settings.Language.Save(Path.Combine(Configuration.BaseDirectory, "LanguageMaster.xml"));
                TranslateViaGoogle((this.comboBoxFrom.SelectedItem as ComboBoxItem).Value + "|" + (this.comboBoxTo.SelectedItem as ComboBoxItem).Value);
                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// The translate via google.
        /// </summary>
        /// <param name="languagePair">
        /// The language pair.
        /// </param>
        public static void TranslateViaGoogle(string languagePair)
        {
            var doc = new XmlDocument();
            doc.Load(Configuration.BaseDirectory + "Language.xml");
            if (doc.DocumentElement != null)
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    TranslateNode(node, languagePair);
                }
            }

            doc.Save(Configuration.BaseDirectory + "Language.xml");
        }

        /// <summary>
        /// The translate node.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="languagePair">
        /// The language pair.
        /// </param>
        private static void TranslateNode(XmlNode node, string languagePair)
        {
            if (node.ChildNodes.Count == 0)
            {
                string oldText = node.InnerText;
                string newText = TranslateTextViaApi(node.InnerText, languagePair);
                if (!string.IsNullOrEmpty(oldText) && !string.IsNullOrEmpty(newText))
                {
                    if (oldText.Contains("{0:"))
                    {
                        newText = oldText;
                    }
                    else
                    {
                        if (!oldText.Contains(" / "))
                        {
                            newText = newText.Replace(" / ", "/");
                        }

                        if (!oldText.Contains(" ..."))
                        {
                            newText = newText.Replace(" ...", "...");
                        }

                        if (!oldText.Contains("& "))
                        {
                            newText = newText.Replace("& ", "&");
                        }

                        if (!oldText.Contains("# "))
                        {
                            newText = newText.Replace("# ", "#");
                        }

                        if (!oldText.Contains("@ "))
                        {
                            newText = newText.Replace("@ ", "@");
                        }

                        if (oldText.Contains("{0}"))
                        {
                            newText = newText.Replace("(0)", "{0}");
                            newText = newText.Replace("(1)", "{1}");
                            newText = newText.Replace("(2)", "{2}");
                            newText = newText.Replace("(3)", "{3}");
                            newText = newText.Replace("(4)", "{4}");
                            newText = newText.Replace("(5)", "{5}");
                            newText = newText.Replace("(6)", "{6}");
                            newText = newText.Replace("(7)", "{7}");
                        }
                    }
                }

                node.InnerText = newText;
            }
            else
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    TranslateNode(childNode, languagePair);
                }
            }
        }

        /// <summary>
        /// The google translate_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GoogleTranslate_Resize(object sender, EventArgs e)
        {
            int width = (this.Width / 2) - (this.subtitleListViewFrom.Left * 3) + 19;
            this.subtitleListViewFrom.Width = width;
            this.subtitleListViewTo.Width = width;

            int height = this.Height - (this.subtitleListViewFrom.Top + this.buttonTranslate.Height + 60);
            this.subtitleListViewFrom.Height = height;
            this.subtitleListViewTo.Height = height;

            this.comboBoxFrom.Left = this.subtitleListViewFrom.Left + (this.subtitleListViewFrom.Width - this.comboBoxFrom.Width);
            this.labelFrom.Left = this.comboBoxFrom.Left - 5 - this.labelFrom.Width;

            this.subtitleListViewTo.Left = width + (this.subtitleListViewFrom.Left * 2);
            this.labelTo.Left = this.subtitleListViewTo.Left;
            this.comboBoxTo.Left = this.labelTo.Left + this.labelTo.Width + 5;
            this.buttonTranslate.Left = this.comboBoxTo.Left + this.comboBoxTo.Width + 9;
            this.labelPleaseWait.Left = this.buttonTranslate.Left + this.buttonTranslate.Width + 9;
            this.progressBar1.Left = this.labelPleaseWait.Left;
            this.progressBar1.Width = this.subtitleListViewTo.Width - (this.progressBar1.Left - this.subtitleListViewTo.Left);
        }

        /// <summary>
        /// The do microsoft translate.
        /// </summary>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        public void DoMicrosoftTranslate(string from, string to)
        {
            SoapService client = this.MsTranslationServiceClient;

            this._breakTranslation = false;
            this.buttonTranslate.Text = Configuration.Settings.Language.General.Cancel;
            const int textMaxSize = 10000;
            Cursor.Current = Cursors.WaitCursor;
            this.progressBar1.Maximum = this._subtitle.Paragraphs.Count;
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = true;
            this.labelPleaseWait.Visible = true;
            int start = 0;
            bool overQuota = false;
            try
            {
                var sb = new StringBuilder();
                int index = 0;
                foreach (Paragraph p in this._subtitle.Paragraphs)
                {
                    string text = string.Format("{1}{0}|", p.Text, SplitterString);
                    if (!overQuota)
                    {
                        if (Utilities.UrlEncode(sb + text).Length >= textMaxSize)
                        {
                            try
                            {
                                this.FillTranslatedText(client.Translate(Configuration.Settings.Tools.MicrosoftBingApiId, sb.ToString().Replace(Environment.NewLine, "<br />"), from, to, "text/plain", "general"), start, index - 1);
                            }
                            catch (SoapHeaderException exception)
                            {
                                MessageBox.Show("Sorry, Microsoft is closing their free api: " + exception.Message);
                                overQuota = true;
                            }

                            sb = new StringBuilder();
                            this.progressBar1.Refresh();
                            Application.DoEvents();
                            start = index;
                        }

                        sb.Append(text);
                    }

                    index++;
                    this.progressBar1.Value = index;
                    if (this._breakTranslation)
                    {
                        break;
                    }
                }

                if (sb.Length > 0 && !overQuota)
                {
                    try
                    {
                        this.FillTranslatedText(client.Translate(Configuration.Settings.Tools.MicrosoftBingApiId, sb.ToString().Replace(Environment.NewLine, "<br />"), from, to, "text/plain", "general"), start, index - 1);
                    }
                    catch (SoapHeaderException exception)
                    {
                        MessageBox.Show("Sorry, Microsoft is closing their free api: " + exception.Message);
                        overQuota = true;
                    }
                }
            }
            finally
            {
                this.labelPleaseWait.Visible = false;
                this.progressBar1.Visible = false;
                Cursor.Current = Cursors.Default;
                this.buttonTranslate.Text = Configuration.Settings.Language.GoogleTranslate.Translate;
                this.buttonTranslate.Enabled = true;
            }
        }

        /// <summary>
        /// The subtitle list view from_ double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListViewFrom_DoubleClick(object sender, EventArgs e)
        {
            if (this.subtitleListViewFrom.SelectedItems.Count > 0)
            {
                int index = this.subtitleListViewFrom.SelectedItems[0].Index;
                if (index < this.subtitleListViewTo.Items.Count)
                {
                    this.subtitleListViewTo.SelectIndexAndEnsureVisible(index);
                }
            }
        }

        /// <summary>
        /// The subtitle list view to_ double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void subtitleListViewTo_DoubleClick(object sender, EventArgs e)
        {
            if (this.subtitleListViewTo.SelectedItems.Count > 0)
            {
                int index = this.subtitleListViewTo.SelectedItems[0].Index;
                if (index < this.subtitleListViewFrom.Items.Count)
                {
                    this.subtitleListViewFrom.SelectIndexAndEnsureVisible(index);
                }
            }
        }

        /// <summary>
        /// The formatting type.
        /// </summary>
        private enum FormattingType
        {
            /// <summary>
            /// The none.
            /// </summary>
            None, 

            /// <summary>
            /// The italic.
            /// </summary>
            Italic, 

            /// <summary>
            /// The italic two lines.
            /// </summary>
            ItalicTwoLines
        }

        /// <summary>
        /// The combo box item.
        /// </summary>
        public class ComboBoxItem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ComboBoxItem"/> class.
            /// </summary>
            /// <param name="text">
            /// The text.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            public ComboBoxItem(string text, string value)
            {
                if (text.Length > 1)
                {
                    text = char.ToUpper(text[0]) + text.Substring(1).ToLower();
                }

                this.Text = text;

                this.Value = value;
            }

            /// <summary>
            /// Gets or sets the text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Text;
            }
        }
    }
}