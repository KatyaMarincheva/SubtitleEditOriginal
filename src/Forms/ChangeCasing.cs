// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeCasing.cs" company="">
//   
// </copyright>
// <summary>
//   The change casing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Dictionaries;

    /// <summary>
    /// The change casing.
    /// </summary>
    public sealed partial class ChangeCasing : PositionAndSizeForm
    {
        /// <summary>
        /// The _no of lines changed.
        /// </summary>
        private int _noOfLinesChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeCasing"/> class.
        /// </summary>
        public ChangeCasing()
        {
            this.InitializeComponent();

            LanguageStructure.ChangeCasing language = Configuration.Settings.Language.ChangeCasing;
            this.Text = language.Title;
            this.groupBoxChangeCasing.Text = language.ChangeCasingTo;
            this.radioButtonNormal.Text = language.NormalCasing;
            this.checkBoxFixNames.Text = language.FixNamesCasing;
            this.radioButtonFixOnlyNames.Text = language.FixOnlyNamesCasing;
            this.checkBoxOnlyAllUpper.Text = language.OnlyChangeAllUppercaseLines;
            this.radioButtonUppercase.Text = language.AllUppercase;
            this.radioButtonLowercase.Text = language.AllLowercase;
            this.buttonOK.Text = Configuration.Settings.Language.General.Ok;
            this.buttonCancel.Text = Configuration.Settings.Language.General.Cancel;
            this.FixLargeFonts();

            if (Configuration.Settings.Tools.ChangeCasingChoice == "NamesOnly")
            {
                this.radioButtonFixOnlyNames.Checked = true;
            }
            else if (Configuration.Settings.Tools.ChangeCasingChoice == "Uppercase")
            {
                this.radioButtonUppercase.Checked = true;
            }
            else if (Configuration.Settings.Tools.ChangeCasingChoice == "Lowercase")
            {
                this.radioButtonLowercase.Checked = true;
            }
        }

        /// <summary>
        /// Gets the lines changed.
        /// </summary>
        public int LinesChanged
        {
            get
            {
                return this._noOfLinesChanged;
            }
        }

        /// <summary>
        /// Gets a value indicating whether change names too.
        /// </summary>
        public bool ChangeNamesToo
        {
            get
            {
                return this.radioButtonFixOnlyNames.Checked || (this.radioButtonNormal.Checked && this.checkBoxFixNames.Checked);
            }
        }

        /// <summary>
        /// The fix large fonts.
        /// </summary>
        private void FixLargeFonts()
        {
            if (this.radioButtonNormal.Left + this.radioButtonNormal.Width + 40 > this.Width)
            {
                this.Width = this.radioButtonNormal.Left + this.radioButtonNormal.Width + 40;
            }

            Utilities.FixLargeFonts(this, this.buttonOK);
        }

        /// <summary>
        /// The fix casing.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        internal void FixCasing(Subtitle subtitle, string language)
        {
            var namesList = new NamesList(Configuration.DictionariesFolder, language, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
            var namesEtc = namesList.GetAllNames();

            // Longer names must be first
            namesEtc.Sort((s1, s2) => s2.Length.CompareTo(s1.Length));

            string lastLine = string.Empty;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                p.Text = this.FixCasing(p.Text, lastLine, namesEtc);

                // fix casing of English alone i to I
                if (this.radioButtonNormal.Checked && language.StartsWith("en", StringComparison.Ordinal))
                {
                    p.Text = FixEnglishAloneILowerToUpper(p.Text);
                }

                lastLine = p.Text;
            }
        }

        /// <summary>
        /// The fix english alone i lower to upper.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FixEnglishAloneILowerToUpper(string text)
        {
            const string pre = " >¡¿♪♫([";
            const string post = " <!?.:;,♪♫)]";
            for (var indexOfI = text.IndexOf('i'); indexOfI >= 0; indexOfI = text.IndexOf('i', indexOfI + 1))
            {
                if (indexOfI == 0 || pre.Contains(text[indexOfI - 1]))
                {
                    if (indexOfI + 1 == text.Length || post.Contains(text[indexOfI + 1]))
                    {
                        text = text.Remove(indexOfI, 1).Insert(indexOfI, "I");
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// The fix casing.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="lastLine">
        /// The last line.
        /// </param>
        /// <param name="namesEtc">
        /// The names etc.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string FixCasing(string text, string lastLine, List<string> namesEtc)
        {
            string original = text;
            if (this.radioButtonNormal.Checked)
            {
                if (this.checkBoxOnlyAllUpper.Checked && text != text.ToUpper())
                {
                    return text;
                }

                if (text.Length > 1)
                {
                    // first all to lower
                    text = text.ToLower().Trim();
                    text = text.FixExtraSpaces();
                    var st = new StripableText(text);
                    st.FixCasing(namesEtc, false, true, true, lastLine); // fix all casing but names (that's a seperate option)
                    text = st.MergedString;
                }
            }
            else if (this.radioButtonUppercase.Checked)
            {
                var st = new StripableText(text);
                text = st.Pre + st.StrippedText.ToUpper() + st.Post;
                text = HtmlUtil.FixUpperTags(text); // tags inside text
            }
            else if (this.radioButtonLowercase.Checked)
            {
                text = text.ToLower();
            }

            if (original != text)
            {
                this._noOfLinesChanged++;
            }

            return text;
        }

        /// <summary>
        /// The form change casing_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormChangeCasing_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
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
            if (this.radioButtonNormal.Checked)
            {
                Configuration.Settings.Tools.ChangeCasingChoice = "Normal";
            }
            else if (this.radioButtonFixOnlyNames.Checked)
            {
                Configuration.Settings.Tools.ChangeCasingChoice = "NamesOnly";
            }
            else if (this.radioButtonUppercase.Checked)
            {
                Configuration.Settings.Tools.ChangeCasingChoice = "Uppercase";
            }
            else if (this.radioButtonLowercase.Checked)
            {
                Configuration.Settings.Tools.ChangeCasingChoice = "Lowercase";
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// The radio button_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            var isNormalCasing = sender == this.radioButtonNormal;
            this.checkBoxFixNames.Enabled = isNormalCasing;
            this.checkBoxOnlyAllUpper.Enabled = isNormalCasing;
        }
    }
}