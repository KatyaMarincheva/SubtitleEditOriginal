// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellCheck.cs" company="">
//   
// </copyright>
// <summary>
//   The spell check.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic;
    using Nikse.SubtitleEdit.Logic.Dictionaries;
    using Nikse.SubtitleEdit.Logic.Enums;
    using Nikse.SubtitleEdit.Logic.SpellCheck;

    /// <summary>
    /// The spell check.
    /// </summary>
    public sealed partial class SpellCheck : Form
    {
        /// <summary>
        /// The _action.
        /// </summary>
        private SpellCheckAction _action = SpellCheckAction.Skip;

        /// <summary>
        /// The _change all dictionary.
        /// </summary>
        private Dictionary<string, string> _changeAllDictionary = new Dictionary<string, string>();

        /// <summary>
        /// The _current action.
        /// </summary>
        private string _currentAction;

        /// <summary>
        /// The _current dictionary.
        /// </summary>
        private string _currentDictionary;

        /// <summary>
        /// The _current index.
        /// </summary>
        private int _currentIndex;

        /// <summary>
        /// The _current paragraph.
        /// </summary>
        private Paragraph _currentParagraph;

        /// <summary>
        /// The _current spell check word.
        /// </summary>
        private SpellCheckWord _currentSpellCheckWord;

        /// <summary>
        /// The _current word.
        /// </summary>
        private string _currentWord;

        /// <summary>
        /// The _first change.
        /// </summary>
        private bool _firstChange = true;

        /// <summary>
        /// The _hunspell.
        /// </summary>
        private Hunspell _hunspell;

        /// <summary>
        /// The _language name.
        /// </summary>
        private string _languageName;

        /// <summary>
        /// The _main window.
        /// </summary>
        private Main _mainWindow;

        /// <summary>
        /// The _names etc list.
        /// </summary>
        private HashSet<string> _namesEtcList = new HashSet<string>();

        /// <summary>
        /// The _names etc list uppercase.
        /// </summary>
        private List<string> _namesEtcListUppercase = new List<string>();

        /// <summary>
        /// The _names etc list with apostrophe.
        /// </summary>
        private List<string> _namesEtcListWithApostrophe = new List<string>();

        /// <summary>
        /// The _names etc multi word list.
        /// </summary>
        private HashSet<string> _namesEtcMultiWordList = new HashSet<string>();

        /// <summary>
        /// The _names list.
        /// </summary>
        private NamesList _namesList;

        /// <summary>
        /// The _no of added words.
        /// </summary>
        private int _noOfAddedWords;

        /// <summary>
        /// The _no of changed words.
        /// </summary>
        private int _noOfChangedWords;

        /// <summary>
        /// The _no of correct words.
        /// </summary>
        private int _noOfCorrectWords;

        /// <summary>
        /// The _no of names etc.
        /// </summary>
        private int _noOfNamesEtc;

        /// <summary>
        /// The _no of skipped words.
        /// </summary>
        private int _noOfSkippedWords;

        /// <summary>
        /// The _original word.
        /// </summary>
        private string _originalWord;

        /// <summary>
        /// The _postfix.
        /// </summary>
        private string _postfix = string.Empty;

        /// <summary>
        /// The _prefix.
        /// </summary>
        private string _prefix = string.Empty;

        /// <summary>
        /// The _skip all list.
        /// </summary>
        private List<string> _skipAllList = new List<string>();

        /// <summary>
        /// The _subtitle.
        /// </summary>
        private Subtitle _subtitle;

        /// <summary>
        /// The _suggestions.
        /// </summary>
        private List<string> _suggestions;

        /// <summary>
        /// The _undo list.
        /// </summary>
        private List<UndoObject> _undoList = new List<UndoObject>();

        /// <summary>
        /// The _user phrase list.
        /// </summary>
        private List<string> _userPhraseList = new List<string>();

        /// <summary>
        /// The _user word list.
        /// </summary>
        private List<string> _userWordList = new List<string>();

        /// <summary>
        /// The _words.
        /// </summary>
        private List<SpellCheckWord> _words;

        /// <summary>
        /// The _words index.
        /// </summary>
        private int _wordsIndex;

        /// <summary>
        /// The _words with dashes or periods.
        /// </summary>
        private List<string> _wordsWithDashesOrPeriods = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellCheck"/> class.
        /// </summary>
        public SpellCheck()
        {
            this.InitializeComponent();
            this.labelActionInfo.Text = string.Empty;
            this.Text = Configuration.Settings.Language.SpellCheck.Title;
            this.labelFullText.Text = Configuration.Settings.Language.SpellCheck.FullText;
            this.labelLanguage.Text = Configuration.Settings.Language.SpellCheck.Language;
            this.groupBoxWordNotFound.Text = Configuration.Settings.Language.SpellCheck.WordNotFound;
            this.buttonAddToDictionary.Text = Configuration.Settings.Language.SpellCheck.AddToUserDictionary;
            this.buttonChange.Text = Configuration.Settings.Language.SpellCheck.Change;
            this.buttonChangeAll.Text = Configuration.Settings.Language.SpellCheck.ChangeAll;
            this.buttonSkipAll.Text = Configuration.Settings.Language.SpellCheck.SkipAll;
            this.buttonSkipOnce.Text = Configuration.Settings.Language.SpellCheck.SkipOnce;
            this.buttonUseSuggestion.Text = Configuration.Settings.Language.SpellCheck.Use;
            this.buttonUseSuggestionAlways.Text = Configuration.Settings.Language.SpellCheck.UseAlways;
            this.buttonAbort.Text = Configuration.Settings.Language.SpellCheck.Abort;
            this.buttonEditWholeText.Text = Configuration.Settings.Language.SpellCheck.EditWholeText;
            this.checkBoxAutoChangeNames.Text = Configuration.Settings.Language.SpellCheck.AutoFixNames;
            this.checkBoxAutoChangeNames.Checked = Configuration.Settings.Tools.SpellCheckAutoChangeNames;
            this.groupBoxEditWholeText.Text = Configuration.Settings.Language.SpellCheck.EditWholeText;
            this.buttonChangeWholeText.Text = Configuration.Settings.Language.SpellCheck.Change;
            this.buttonSkipText.Text = Configuration.Settings.Language.SpellCheck.SkipOnce;
            this.groupBoxSuggestions.Text = Configuration.Settings.Language.SpellCheck.Suggestions;
            this.buttonAddToNames.Text = Configuration.Settings.Language.SpellCheck.AddToNamesAndIgnoreList;
            this.buttonGoogleIt.Text = Configuration.Settings.Language.Main.VideoControls.GoogleIt;
            Utilities.FixLargeFonts(this, this.buttonAbort);
        }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        public SpellCheckAction Action
        {
            get
            {
                return this._action;
            }

            set
            {
                this._action = value;
            }
        }

        /// <summary>
        /// Gets or sets the change word.
        /// </summary>
        public string ChangeWord
        {
            get
            {
                return this.textBoxWord.Text;
            }

            set
            {
                this.textBoxWord.Text = value;
            }
        }

        /// <summary>
        /// Gets the change whole text.
        /// </summary>
        public string ChangeWholeText
        {
            get
            {
                return this.textBoxWholeText.Text;
            }
        }

        /// <summary>
        /// Gets a value indicating whether auto fix names.
        /// </summary>
        public bool AutoFixNames
        {
            get
            {
                return this.checkBoxAutoChangeNames.Checked;
            }
        }

        /// <summary>
        /// Gets the current line index.
        /// </summary>
        public int CurrentLineIndex
        {
            get
            {
                return this._currentIndex;
            }
        }

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
        /// The initialize.
        /// </summary>
        /// <param name="languageName">
        /// The language name.
        /// </param>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="suggestions">
        /// The suggestions.
        /// </param>
        /// <param name="paragraph">
        /// The paragraph.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        public void Initialize(string languageName, SpellCheckWord word, List<string> suggestions, string paragraph, string progress)
        {
            this._originalWord = word.Text;
            this._suggestions = suggestions;
            this.groupBoxWordNotFound.Visible = true;
            this.groupBoxEditWholeText.Visible = false;
            this.buttonEditWholeText.Text = Configuration.Settings.Language.SpellCheck.EditWholeText;
            this.Text = Configuration.Settings.Language.SpellCheck.Title + " [" + languageName + "] - " + progress;
            this.textBoxWord.Text = word.Text;
            this.textBoxWholeText.Text = paragraph;
            this.listBoxSuggestions.Items.Clear();
            foreach (string suggestion in suggestions)
            {
                this.listBoxSuggestions.Items.Add(suggestion);
            }

            if (this.listBoxSuggestions.Items.Count > 0)
            {
                this.listBoxSuggestions.SelectedIndex = 0;
            }

            this.richTextBoxParagraph.Text = paragraph;

            this.FillSpellCheckDictionaries(languageName);
            this.ShowActiveWordWithColor(word);
            this._action = SpellCheckAction.Skip;
            this.DialogResult = DialogResult.None;
        }

        /// <summary>
        /// The fill spell check dictionaries.
        /// </summary>
        /// <param name="languageName">
        /// The language name.
        /// </param>
        private void FillSpellCheckDictionaries(string languageName)
        {
            this.comboBoxDictionaries.SelectedIndexChanged -= this.ComboBoxDictionariesSelectedIndexChanged;
            this.comboBoxDictionaries.Items.Clear();
            foreach (string name in Utilities.GetDictionaryLanguages())
            {
                this.comboBoxDictionaries.Items.Add(name);
                if (name.Contains("[" + languageName + "]"))
                {
                    this.comboBoxDictionaries.SelectedIndex = this.comboBoxDictionaries.Items.Count - 1;
                }
            }

            this.comboBoxDictionaries.SelectedIndexChanged += this.ComboBoxDictionariesSelectedIndexChanged;
        }

        /// <summary>
        /// The show active word with color.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        private void ShowActiveWordWithColor(SpellCheckWord word)
        {
            this.richTextBoxParagraph.SelectAll();
            this.richTextBoxParagraph.SelectionColor = Color.Black;
            this.richTextBoxParagraph.SelectionLength = 0;

            for (int i = 0; i < 10; i++)
            {
                int idx = word.Index - i;
                if (idx >= 0 && idx < this.richTextBoxParagraph.Text.Length && this.richTextBoxParagraph.Text.Substring(idx).StartsWith(word.Text))
                {
                    this.richTextBoxParagraph.SelectionStart = idx;
                    this.richTextBoxParagraph.SelectionLength = word.Text.Length;
                    this.richTextBoxParagraph.SelectionColor = Color.Red;
                    break;
                }

                idx = word.Index + i;
                if (idx >= 0 && idx < this.richTextBoxParagraph.Text.Length && this.richTextBoxParagraph.Text.Substring(idx).StartsWith(word.Text))
                {
                    this.richTextBoxParagraph.SelectionStart = idx;
                    this.richTextBoxParagraph.SelectionLength = word.Text.Length;
                    this.richTextBoxParagraph.SelectionColor = Color.Red;
                    break;
                }
            }
        }

        /// <summary>
        /// The form spell check_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FormSpellCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this._action = SpellCheckAction.Abort;
                this.DialogResult = DialogResult.Cancel;
            }
            else if (e.KeyCode == Keys.F1)
            {
                Utilities.ShowHelp("#spellcheck");
                e.SuppressKeyPress = true;
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.G)
            {
                e.SuppressKeyPress = true;
                System.Diagnostics.Process.Start("https://www.google.com/search?q=" + Utilities.UrlEncode(this.textBoxWord.Text));
            }
            else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
            {
                if (this.buttonUndo.Visible)
                {
                    this.buttonUndo_Click(null, null);
                    e.SuppressKeyPress = true;
                }
            }
        }

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
            this.ShowEndStatusMessage(Configuration.Settings.Language.SpellCheck.SpellCheckAborted);
            this.DialogResult = DialogResult.Abort;
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
            this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.Change, this._currentWord + " > " + this.textBoxWord.Text), SpellCheckAction.Change);
            this.DoAction(SpellCheckAction.Change);
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
                this.textBoxWord.Text = this.listBoxSuggestions.SelectedItem.ToString();
                this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.Change, this._currentWord + " > " + this.textBoxWord.Text), SpellCheckAction.Change);
                this.DoAction(SpellCheckAction.Change);
            }
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
            this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.SkipAll, this.textBoxWord.Text), SpellCheckAction.SkipAll);
            this.DoAction(SpellCheckAction.SkipAll);
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
            this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.SkipOnce, this.textBoxWord.Text), SpellCheckAction.Skip);
            this.DoAction(SpellCheckAction.Skip);
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
            this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.AddToUserDictionary, this.textBoxWord.Text), SpellCheckAction.AddToDictionary);
            this.DoAction(SpellCheckAction.AddToDictionary);
        }

        /// <summary>
        /// The combo box dictionaries selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ComboBoxDictionariesSelectedIndexChanged(object sender, EventArgs e)
        {
            Configuration.Settings.General.SpellCheckLanguage = this.LanguageString;
            Configuration.Settings.Save();
            this._languageName = this.LanguageString;
            string dictionary = Utilities.DictionaryFolder + this._languageName;
            this.LoadDictionaries(Utilities.DictionaryFolder, dictionary);
            this._wordsIndex--;
            this.PrepareNextWord();
        }

        /// <summary>
        /// The load hunspell.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        private void LoadHunspell(string dictionary)
        {
            this._currentDictionary = dictionary;
            if (this._hunspell != null)
            {
                this._hunspell.Dispose();
            }

            this._hunspell = Hunspell.GetHunspell(dictionary);
        }

        /// <summary>
        /// The do spell.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool DoSpell(string word)
        {
            return this._hunspell.Spell(word);
        }

        /// <summary>
        /// The do suggest.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<string> DoSuggest(string word)
        {
            var parameter = new SuggestionParameter(word, this._hunspell);
            var suggestThread = new System.Threading.Thread(DoWork);
            suggestThread.Start(parameter);
            suggestThread.Join(3000); // wait max 3 seconds
            suggestThread.Abort();
            if (!parameter.Success)
            {
                this.LoadHunspell(this._currentDictionary);
            }

            return parameter.Suggestions;
        }

        /// <summary>
        /// The do work.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public static void DoWork(object data)
        {
            var parameter = (SuggestionParameter)data;
            parameter.Suggestions = parameter.Hunspell.Suggest(parameter.InputWord);
            parameter.Success = true;
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
            this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.ChangeAll, this._currentWord + " > " + this.textBoxWord.Text), SpellCheckAction.ChangeAll);
            this.DoAction(SpellCheckAction.ChangeAll);
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
                this.textBoxWord.Text = this.listBoxSuggestions.SelectedItem.ToString();
                this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.ChangeAll, this._currentWord + " > " + this.textBoxWord.Text), SpellCheckAction.ChangeAll);
                this.DoAction(SpellCheckAction.ChangeAll);
            }
        }

        /// <summary>
        /// The spell check_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SpellCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration.Settings.Tools.SpellCheckAutoChangeNames = this.checkBoxAutoChangeNames.Checked;
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.DialogResult = DialogResult.Abort;
            }
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
            this.PushUndo(string.Format("{0}: {1}", Configuration.Settings.Language.SpellCheck.AddToNamesAndIgnoreList, this.textBoxWord.Text), SpellCheckAction.AddToNamesEtc);
            this.DoAction(SpellCheckAction.AddToNamesEtc);
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
            if (this.groupBoxWordNotFound.Visible)
            {
                this.groupBoxWordNotFound.Visible = false;
                this.groupBoxEditWholeText.Visible = true;
                this.buttonEditWholeText.Text = Configuration.Settings.Language.SpellCheck.EditWordOnly;
                this.textBoxWholeText.Focus();
            }
            else
            {
                this.groupBoxWordNotFound.Visible = true;
                this.groupBoxEditWholeText.Visible = false;
                this.buttonEditWholeText.Text = Configuration.Settings.Language.SpellCheck.EditWholeText;
                this.textBoxWord.Focus();
            }
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
            this.PushUndo(string.Format("{0}", Configuration.Settings.Language.SpellCheck.SkipOnce), SpellCheckAction.Skip);
            this.DoAction(SpellCheckAction.Skip);
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
            this.PushUndo(string.Format("{0}", Configuration.Settings.Language.SpellCheck.EditWholeText), SpellCheckAction.ChangeWholeText);
            this.DoAction(SpellCheckAction.ChangeWholeText);
        }

        /// <summary>
        /// The context menu strip 1 opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ContextMenuStrip1Opening(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.richTextBoxParagraph.SelectedText))
            {
                string word = this.richTextBoxParagraph.SelectedText.Trim();
                this.addXToNamesnoiseListToolStripMenuItem.Text = string.Format(Configuration.Settings.Language.SpellCheck.AddXToNamesEtc, word);
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// The add x to namesnoise list tool strip menu item click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AddXToNamesnoiseListToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.richTextBoxParagraph.SelectedText))
            {
                this.ChangeWord = this.richTextBoxParagraph.SelectedText.Trim();
                this.DoAction(SpellCheckAction.AddToNamesEtc);
            }
        }

        /// <summary>
        /// The check box auto change names checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxAutoChangeNamesCheckedChanged(object sender, EventArgs e)
        {
            if (this.textBoxWord.Text.Length < 2)
            {
                return;
            }

            string s = char.ToUpper(this.textBoxWord.Text[0]) + this.textBoxWord.Text.Substring(1);
            if (this.checkBoxAutoChangeNames.Checked && this._suggestions != null && this._suggestions.Contains(s))
            {
                this.ChangeWord = s;
                this.DoAction(SpellCheckAction.ChangeAll);
            }
        }

        /// <summary>
        /// The list box suggestions mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListBoxSuggestionsMouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ButtonUseSuggestionAlwaysClick(null, null);
        }

        /// <summary>
        /// The do action.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        public void DoAction(SpellCheckAction action)
        {
            switch (action)
            {
                case SpellCheckAction.Change:
                    this._noOfChangedWords++;
                    this._mainWindow.CorrectWord(this._prefix + this.ChangeWord + this._postfix, this._currentParagraph, this._prefix + this._currentWord + this._postfix, ref this._firstChange);
                    break;
                case SpellCheckAction.ChangeAll:
                    this._noOfChangedWords++;
                    if (!this._changeAllDictionary.ContainsKey(this._currentWord))
                    {
                        this._changeAllDictionary.Add(this._currentWord, this.ChangeWord);
                    }

                    this._mainWindow.CorrectWord(this._prefix + this.ChangeWord + this._postfix, this._currentParagraph, this._prefix + this._currentWord + this._postfix, ref this._firstChange);
                    break;
                case SpellCheckAction.Skip:
                    this._noOfSkippedWords++;
                    break;
                case SpellCheckAction.SkipAll:
                    this._noOfSkippedWords++;
                    this._skipAllList.Add(this.ChangeWord.ToUpper());
                    if (this.ChangeWord.EndsWith('\'') || this.ChangeWord.StartsWith('\''))
                    {
                        this._skipAllList.Add(this.ChangeWord.ToUpper().Trim('\''));
                    }

                    break;
                case SpellCheckAction.AddToDictionary:
                    if (this._userWordList.IndexOf(this.ChangeWord) < 0)
                    {
                        this._noOfAddedWords++;
                        string s = this.ChangeWord.Trim().ToLower();
                        if (s.Contains(' '))
                        {
                            this._userPhraseList.Add(s);
                        }
                        else
                        {
                            this._userWordList.Add(s);
                        }

                        Utilities.AddToUserDictionary(s, this._languageName);
                    }

                    break;
                case SpellCheckAction.AddToNamesEtc:
                    if (this.ChangeWord.Length > 1 && !this._namesEtcList.Contains(this.ChangeWord))
                    {
                        this._namesEtcList.Add(this.ChangeWord);
                        this._namesEtcListUppercase.Add(this.ChangeWord.ToUpper());
                        if (this._languageName.StartsWith("en_") && !this.ChangeWord.EndsWith('s'))
                        {
                            this._namesEtcList.Add(this.ChangeWord + "s");
                            this._namesEtcListUppercase.Add(this.ChangeWord.ToUpper() + "S");
                        }

                        if (!this.ChangeWord.EndsWith('s'))
                        {
                            this._namesEtcListWithApostrophe.Add(this.ChangeWord + "'s");
                            this._namesEtcListUppercase.Add(this.ChangeWord.ToUpper() + "'S");
                        }

                        if (!this.ChangeWord.EndsWith('\''))
                        {
                            this._namesEtcListWithApostrophe.Add(this.ChangeWord + "'");
                        }

                        var namesList = new NamesList(Configuration.DictionariesFolder, this._languageName, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
                        namesList.Add(this.ChangeWord);
                    }

                    break;
                case SpellCheckAction.ChangeWholeText:
                    this._mainWindow.ShowStatus(string.Format(Configuration.Settings.Language.Main.SpellCheckChangedXToY, this._currentParagraph.Text.Replace(Environment.NewLine, " "), this.ChangeWholeText.Replace(Environment.NewLine, " ")));
                    this._currentParagraph.Text = this.ChangeWholeText;
                    this._mainWindow.ChangeWholeTextMainPart(ref this._noOfChangedWords, ref this._firstChange, this._currentIndex, this._currentParagraph);

                    break;
            }

            this.labelActionInfo.Text = string.Empty;
            this.PrepareNextWord();
            this.CheckActions();
        }

        /// <summary>
        /// The check actions.
        /// </summary>
        private void CheckActions()
        {
            if (string.IsNullOrEmpty(this._currentAction))
            {
                return;
            }

            if (this._currentAction == Configuration.Settings.Language.SpellCheck.Change)
            {
                this.ShowActionInfo(this._currentAction, this._currentWord + " > " + this.textBoxWord.Text);
            }
            else if (this._currentAction == Configuration.Settings.Language.SpellCheck.ChangeAll)
            {
                this.ShowActionInfo(this._currentAction, this._currentWord + " > " + this.textBoxWord.Text);
            }
            else
            {
                this.ShowActionInfo(this._currentAction, this.textBoxWord.Text);
            }
        }

        /// <summary>
        /// The is word in user phrases.
        /// </summary>
        /// <param name="userPhraseList">
        /// The user phrase list.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="words">
        /// The words.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsWordInUserPhrases(List<string> userPhraseList, int index, List<SpellCheckWord> words)
        {
            string current = words[index].Text;
            string prev = "-";
            if (index > 0)
            {
                prev = words[index - 1].Text;
            }

            string next = "-";
            if (index < words.Count - 1)
            {
                next = words[index + 1].Text;
            }

            foreach (string userPhrase in userPhraseList)
            {
                if (userPhrase == current + " " + next)
                {
                    return true;
                }

                if (userPhrase == prev + " " + current)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The prepare next word.
        /// </summary>
        private void PrepareNextWord()
        {
            while (true)
            {
                if (this._wordsIndex + 1 < this._words.Count)
                {
                    this._wordsIndex++;
                    this._currentWord = this._words[this._wordsIndex].Text;
                    this._currentSpellCheckWord = this._words[this._wordsIndex];
                }
                else
                {
                    if (this._currentIndex + 1 < this._subtitle.Paragraphs.Count)
                    {
                        this._currentIndex++;
                        this._currentParagraph = this._subtitle.Paragraphs[this._currentIndex];
                        this.SetWords(this._currentParagraph.Text);
                        this._wordsIndex = 0;
                        if (this._words.Count == 0)
                        {
                            this._currentWord = string.Empty;
                        }
                        else
                        {
                            this._currentWord = this._words[this._wordsIndex].Text;
                            this._currentSpellCheckWord = this._words[this._wordsIndex];
                        }
                    }
                    else
                    {
                        this.ShowEndStatusMessage(Configuration.Settings.Language.SpellCheck.SpellCheckCompleted);
                        this.DialogResult = DialogResult.OK;
                        return;
                    }
                }

                int minLength = 2;
                if (Configuration.Settings.Tools.SpellCheckOneLetterWords)
                {
                    minLength = 1;
                }

                if (this._currentWord.Trim().Length >= minLength && !this._currentWord.Contains(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '%', '&', '@', '$', '*', '=', '£', '#', '_', '½', '^' }))
                {
                    this._prefix = string.Empty;
                    this._postfix = string.Empty;
                    if (this._currentWord.Length > 1)
                    {
                        if (this._currentWord.StartsWith('\''))
                        {
                            this._prefix = "'";
                            this._currentWord = this._currentWord.Substring(1);
                        }

                        if (this._currentWord.StartsWith('`'))
                        {
                            this._prefix = "`";
                            this._currentWord = this._currentWord.Substring(1);
                        }
                    }

                    if (this._namesEtcList.Contains(this._currentWord) || (this._currentWord.StartsWith('\'') || this._currentWord.EndsWith('\'')) && this._namesEtcList.Contains(this._currentWord.Trim('\'')))
                    {
                        this._noOfNamesEtc++;
                    }
                    else if (this._skipAllList.Contains(this._currentWord.ToUpper()) || (this._currentWord.StartsWith('\'') || this._currentWord.EndsWith('\'')) && this._skipAllList.Contains(this._currentWord.Trim('\'').ToUpper()))
                    {
                        this._noOfSkippedWords++;
                    }
                    else if (this._userWordList.Contains(this._currentWord.ToLower()) || (this._currentWord.StartsWith('\'') || this._currentWord.EndsWith('\'')) && this._userWordList.Contains(this._currentWord.Trim('\'').ToLower()))
                    {
                        this._noOfCorrectWords++;
                    }
                    else if (this._changeAllDictionary.ContainsKey(this._currentWord))
                    {
                        this._noOfChangedWords++;
                        this._mainWindow.CorrectWord(this._changeAllDictionary[this._currentWord], this._currentParagraph, this._currentWord, ref this._firstChange);
                    }
                    else if (this._changeAllDictionary.ContainsKey(this._currentWord.Trim('\'')))
                    {
                        this._noOfChangedWords++;
                        this._mainWindow.CorrectWord(this._changeAllDictionary[this._currentWord], this._currentParagraph, this._currentWord.Trim('\''), ref this._firstChange);
                    }
                    else if (this._namesEtcListUppercase.Contains(this._currentWord) || this._namesEtcListWithApostrophe.Contains(this._currentWord) || this._namesList.IsInNamesEtcMultiWordList(this._currentParagraph.Text, this._currentWord))
                    {
                        // TODO: Verify this!
                        this._noOfNamesEtc++;
                    }
                    else if (IsWordInUserPhrases(this._userPhraseList, this._wordsIndex, this._words))
                    {
                        this._noOfCorrectWords++;
                    }
                    else
                    {
                        bool correct;

                        if (this._prefix == "'" && this._currentWord.Length >= 1 && (this.DoSpell(this._prefix + this._currentWord) || this._userWordList.Contains(this._prefix + this._currentWord)))
                        {
                            correct = true;
                        }
                        else if (this._currentWord.Length > 1)
                        {
                            correct = this.DoSpell(this._currentWord);
                            if (!correct && "`'".Contains(this._currentWord[this._currentWord.Length - 1]))
                            {
                                correct = this.DoSpell(this._currentWord.TrimEnd('\'').TrimEnd('`'));
                            }

                            if (!correct && this._currentWord.EndsWith("'s", StringComparison.Ordinal) && this._currentWord.Length > 4)
                            {
                                correct = this.DoSpell(this._currentWord.TrimEnd('s').TrimEnd('\''));
                            }

                            if (!correct && this._currentWord.EndsWith('\'') && this.DoSpell(this._currentWord.TrimEnd('\'')))
                            {
                                this._currentWord = this._currentWord.TrimEnd('\'');
                                correct = true;
                            }
                        }
                        else
                        {
                            correct = false;
                            if (this._currentWord == "'")
                            {
                                correct = true;
                            }
                            else if (this._languageName.StartsWith("en_", StringComparison.Ordinal) && (this._currentWord.Equals("a", StringComparison.OrdinalIgnoreCase) || this._currentWord == "I"))
                            {
                                correct = true;
                            }
                            else if (this._languageName.StartsWith("da_", StringComparison.Ordinal) && this._currentWord.Equals("i", StringComparison.OrdinalIgnoreCase))
                            {
                                correct = true;
                            }
                        }

                        if (!correct && Configuration.Settings.Tools.SpellCheckEnglishAllowInQuoteAsIng && this._languageName.StartsWith("en_", StringComparison.Ordinal) && this._currentWord.EndsWith("in'", StringComparison.OrdinalIgnoreCase))
                        {
                            correct = this.DoSpell(this._currentWord.TrimEnd('\'') + "g");
                        }

                        if (correct)
                        {
                            this._noOfCorrectWords++;
                        }
                        else
                        {
                            this._mainWindow.FocusParagraph(this._currentIndex);

                            List<string> suggestions = new List<string>();

                            if ((this._currentWord == "Lt's" || this._currentWord == "Lt'S") && this._languageName.StartsWith("en_"))
                            {
                                suggestions.Add("It's");
                            }
                            else
                            {
                                if (this._currentWord.ToUpper() != "LT'S" && this._currentWord.ToUpper() != "SOX'S" && !this._currentWord.ToUpper().StartsWith("HTTP", StringComparison.Ordinal))
                                {
                                    // TODO: Get fixed nhunspell
                                    suggestions = this.DoSuggest(this._currentWord); // TODO: 0.9.6 fails on "Lt'S"
                                }

                                if (this._languageName.StartsWith("fr_", StringComparison.Ordinal) && (this._currentWord.StartsWith("I'", StringComparison.Ordinal) || this._currentWord.StartsWith("I’", StringComparison.Ordinal)))
                                {
                                    if (this._currentWord.Length > 3 && Utilities.LowercaseLetters.Contains(this._currentWord[2]) && this._currentSpellCheckWord.Index > 3)
                                    {
                                        string ending = this._currentParagraph.Text.Substring(0, this._currentSpellCheckWord.Index - 1).Trim();
                                        if (ending.Length > 1 && !".!?".Contains(ending[ending.Length - 1]))
                                        {
                                            for (int i = 0; i < suggestions.Count; i++)
                                            {
                                                if (suggestions[i].StartsWith("L'") || suggestions[i].StartsWith("L’"))
                                                {
                                                    suggestions[i] = @"l" + suggestions[i].Substring(1);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            suggestions.Remove(this._currentWord);
                            if (this._currentWord.Length == 1)
                            {
                                if ((this._currentWord == "L") && this._languageName.StartsWith("en_", StringComparison.Ordinal))
                                {
                                    suggestions.Remove("I");
                                    suggestions.Insert(0, "I");
                                }
                            }

                            if (this.AutoFixNames && this._currentWord.Length > 1 && suggestions.Contains(char.ToUpper(this._currentWord[0]) + this._currentWord.Substring(1)))
                            {
                                this.ChangeWord = char.ToUpper(this._currentWord[0]) + this._currentWord.Substring(1);
                                this.DoAction(SpellCheckAction.ChangeAll);
                                return;
                            }

                            if (this.AutoFixNames && this._currentWord.Length > 3 && suggestions.Contains(this._currentWord.ToUpper()))
                            { // does not work well with two letter words like "da" and "de" which get auto-corrected to "DA" and "DE"
                                this.ChangeWord = this._currentWord.ToUpper();
                                this.DoAction(SpellCheckAction.ChangeAll);
                                return;
                            }

                            if (this.AutoFixNames && this._currentWord.Length > 1 && this._namesEtcList.Contains(char.ToUpper(this._currentWord[0]) + this._currentWord.Substring(1)))
                            {
                                this.ChangeWord = char.ToUpper(this._currentWord[0]) + this._currentWord.Substring(1);
                                this.DoAction(SpellCheckAction.ChangeAll);
                                return;
                            }

                            if (this._prefix != null && this._prefix == "''" && this._currentWord.EndsWith("''"))
                            {
                                this._prefix = string.Empty;
                                this._currentSpellCheckWord.Index += 2;
                                this._currentWord = this._currentWord.Trim('\'');
                            }

                            if (this._prefix != null && this._prefix == "'" && this._currentWord.EndsWith('\''))
                            {
                                this._prefix = string.Empty;
                                this._currentSpellCheckWord.Index++;
                                this._currentWord = this._currentWord.Trim('\'');
                            }

                            if (this._postfix != null && this._postfix == "'")
                            {
                                this._currentSpellCheckWord.Text = this._currentWord + this._postfix;
                                this.Initialize(this._languageName, this._currentSpellCheckWord, suggestions, this._currentParagraph.Text, string.Format(Configuration.Settings.Language.Main.LineXOfY, this._currentIndex + 1, this._subtitle.Paragraphs.Count));
                            }
                            else
                            {
                                this._currentSpellCheckWord.Text = this._currentWord;
                                this.Initialize(this._languageName, this._currentSpellCheckWord, suggestions, this._currentParagraph.Text, string.Format(Configuration.Settings.Language.Main.LineXOfY, this._currentIndex + 1, this._subtitle.Paragraphs.Count));
                            }

                            if (!this.Visible)
                            {
                                this.ShowDialog(this._mainWindow);
                            }

                            return; // wait for user input
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The split.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private static List<SpellCheckWord> Split(string s)
        {
            const string splitChars = " -.,?!:;\"“”()[]{}|<>/+\r\n¿¡…—–♪♫„“";
            var list = new List<SpellCheckWord>();
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (splitChars.Contains(s[i]))
                {
                    if (sb.Length > 0)
                    {
                        list.Add(new SpellCheckWord { Text = sb.ToString(), Index = i - sb.Length });
                    }

                    sb = new StringBuilder();
                }
                else
                {
                    sb.Append(s[i]);
                }
            }

            if (sb.Length > 0)
            {
                list.Add(new SpellCheckWord { Text = sb.ToString(), Index = s.Length - 1 - sb.Length });
            }

            return list;
        }

        /// <summary>
        /// The set words.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string SetWords(string s)
        {
            s = ReplaceHtmlTagsWithBlanks(s);
            s = this.ReplaceKnownWordsOrNamesWithBlanks(s);
            this._words = Split(s);
            return s;
        }

        /// <summary>
        /// The replace known words or names with blanks.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ReplaceKnownWordsOrNamesWithBlanks(string s)
        {
            List<string> replaceIds = new List<string>();
            List<string> replaceNames = new List<string>();
            this.GetTextWithoutUserWordsAndNames(replaceIds, replaceNames, s);
            foreach (string name in replaceNames)
            {
                int start = s.IndexOf(name, StringComparison.Ordinal);
                while (start >= 0)
                {
                    bool startOk = start == 0 || " -.,?!:;\"“”()[]{}|<>/+\r\n¿¡…—–♪♫„“".Contains(s[start - 1]);
                    if (startOk)
                    {
                        int end = start + name.Length;
                        bool endOk = end >= s.Length || " -.,?!:;\"“”()[]{}|<>/+\r\n¿¡…—–♪♫„“".Contains(s[end]);
                        if (endOk)
                        {
                            s = s.Remove(start, name.Length).Insert(start, string.Empty.PadLeft(name.Length));
                        }
                    }

                    if (start + 1 < s.Length)
                    {
                        start = s.IndexOf(name, start + 1, StringComparison.Ordinal);
                    }
                    else
                    {
                        start = -1;
                    }
                }
            }

            return s;
        }

        /// <summary>
        /// The replace html tags with blanks.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ReplaceHtmlTagsWithBlanks(string s)
        {
            int start = s.IndexOf('<');
            while (start >= 0)
            {
                int end = s.IndexOf('>', start + 1);
                if (end < start)
                {
                    break;
                }

                int l = end - start + 1;
                s = s.Remove(start, l).Insert(start, string.Empty.PadLeft(l));
                end++;
                if (end >= s.Length)
                {
                    break;
                }

                start = s.IndexOf('<', end);
            }

            return s;
        }

        /// <summary>
        /// Removes words with dash'es that are correct, so spell check can ignore the combination (do not split correct words with dash'es)
        /// </summary>
        /// <param name="replaceIds">
        /// The replace Ids.
        /// </param>
        /// <param name="replaceNames">
        /// The replace Names.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetTextWithoutUserWordsAndNames(List<string> replaceIds, List<string> replaceNames, string text)
        {
            string[] wordsWithDash = text.Split(new[] { ' ', '.', ',', '?', '!', ':', ';', '"', '“', '”', '(', ')', '[', ']', '{', '}', '|', '<', '>', '/', '+', '\r', '\n', '¿', '¡', '…', '—', '–', '♪', '♫', '„', '“' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string w in wordsWithDash)
            {
                if (w.Contains('-') && this.DoSpell(w) && !this._wordsWithDashesOrPeriods.Contains(w))
                {
                    this._wordsWithDashesOrPeriods.Add(w);
                }
            }

            if (text.Contains(new[] { '.', '-' }))
            {
                int i = 0;
                foreach (string wordWithDashesOrPeriods in this._wordsWithDashesOrPeriods)
                {
                    bool found = true;
                    int startSearchIndex = 0;
                    while (found)
                    {
                        int indexStart = text.IndexOf(wordWithDashesOrPeriods, startSearchIndex, StringComparison.Ordinal);

                        if (indexStart >= 0)
                        {
                            int endIndexPlus = indexStart + wordWithDashesOrPeriods.Length;
                            bool startOk = indexStart == 0 || (@" (['""" + Environment.NewLine).Contains(text[indexStart - 1]);
                            bool endOk = endIndexPlus == text.Length;
                            if (!endOk && endIndexPlus < text.Length && @",!?:;. ])<'""".Contains(text[endIndexPlus]))
                            {
                                endOk = true;
                            }

                            if (startOk && endOk)
                            {
                                i++;
                                string id = string.Format("_@{0}_", i);
                                replaceIds.Add(id);
                                replaceNames.Add(wordWithDashesOrPeriods);
                                text = text.Remove(indexStart, wordWithDashesOrPeriods.Length).Insert(indexStart, id);
                            }
                            else
                            {
                                startSearchIndex = indexStart + 1;
                            }
                        }
                        else
                        {
                            found = false;
                        }
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// The show end status message.
        /// </summary>
        /// <param name="completedMessage">
        /// The completed message.
        /// </param>
        private void ShowEndStatusMessage(string completedMessage)
        {
            LanguageStructure.Main mainLanguage = Configuration.Settings.Language.Main;
            if (this._noOfChangedWords > 0 || this._noOfAddedWords > 0 || this._noOfSkippedWords > 0 || completedMessage == Configuration.Settings.Language.SpellCheck.SpellCheckCompleted)
            {
                this.Hide();
                if (Configuration.Settings.Tools.SpellCheckShowCompletedMessage)
                {
                    var form = new DialogDoNotShowAgain(this._mainWindow.Title + " - " + mainLanguage.SpellCheck, completedMessage + Environment.NewLine + Environment.NewLine + string.Format(mainLanguage.NumberOfCorrectedWords, this._noOfChangedWords) + Environment.NewLine + string.Format(mainLanguage.NumberOfSkippedWords, this._noOfSkippedWords) + Environment.NewLine + string.Format(mainLanguage.NumberOfCorrectWords, this._noOfCorrectWords) + Environment.NewLine + string.Format(mainLanguage.NumberOfWordsAddedToDictionary, this._noOfAddedWords) + Environment.NewLine + string.Format(mainLanguage.NumberOfNameHits, this._noOfNamesEtc));
                    form.ShowDialog(this._mainWindow);
                    Configuration.Settings.Tools.SpellCheckShowCompletedMessage = !form.DoNoDisplayAgain;
                    form.Dispose();
                }
                else
                {
                    if (this._noOfChangedWords > 0)
                    {
                        this._mainWindow.ShowStatus(completedMessage + "  " + string.Format(mainLanguage.NumberOfCorrectedWords, this._noOfChangedWords));
                    }
                    else
                    {
                        this._mainWindow.ShowStatus(completedMessage);
                    }
                }
            }
        }

        /// <summary>
        /// The continue spell check.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public void ContinueSpellCheck(Subtitle subtitle)
        {
            this._subtitle = subtitle;

            this.buttonUndo.Visible = false;
            this._undoList = new List<UndoObject>();

            if (this._currentIndex >= subtitle.Paragraphs.Count)
            {
                this._currentIndex = 0;
            }

            this._currentParagraph = this._subtitle.GetParagraphOrDefault(this._currentIndex);
            if (this._currentParagraph == null)
            {
                return;
            }

            this.SetWords(this._currentParagraph.Text);
            this._wordsIndex = -1;

            this.PrepareNextWord();
        }

        /// <summary>
        /// The do spell check.
        /// </summary>
        /// <param name="autoDetect">
        /// The auto detect.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="dictionaryFolder">
        /// The dictionary folder.
        /// </param>
        /// <param name="mainWindow">
        /// The main window.
        /// </param>
        /// <param name="startLine">
        /// The start line.
        /// </param>
        public void DoSpellCheck(bool autoDetect, Subtitle subtitle, string dictionaryFolder, Main mainWindow, int startLine)
        {
            this._subtitle = subtitle;
            LanguageStructure.Main mainLanguage = Configuration.Settings.Language.Main;
            this._mainWindow = mainWindow;

            this._namesEtcListUppercase = new List<string>();
            this._namesEtcListWithApostrophe = new List<string>();

            this._skipAllList = new List<string>();

            this._noOfSkippedWords = 0;
            this._noOfChangedWords = 0;
            this._noOfCorrectWords = 0;
            this._noOfNamesEtc = 0;
            this._noOfAddedWords = 0;
            this._firstChange = true;

            if (!string.IsNullOrEmpty(Configuration.Settings.General.SpellCheckLanguage) && File.Exists(Path.Combine(dictionaryFolder, Configuration.Settings.General.SpellCheckLanguage + ".dic")))
            {
                this._languageName = Configuration.Settings.General.SpellCheckLanguage;
            }
            else
            {
                string name = Utilities.GetDictionaryLanguages()[0];
                int start = name.LastIndexOf('[');
                int end = name.LastIndexOf(']');
                if (start > 0 && end > start)
                {
                    start++;
                    name = name.Substring(start, end - start);
                    this._languageName = name;
                }
                else
                {
                    MessageBox.Show(string.Format(mainLanguage.InvalidLanguageNameX, name));
                    return;
                }
            }

            if (autoDetect || string.IsNullOrEmpty(this._languageName))
            {
                this._languageName = Utilities.AutoDetectLanguageName(this._languageName, subtitle);
            }

            string dictionary = Utilities.DictionaryFolder + this._languageName;

            this.LoadDictionaries(dictionaryFolder, dictionary);

            this._currentIndex = 0;
            if (startLine >= 0 && startLine < this._subtitle.Paragraphs.Count)
            {
                this._currentIndex = startLine;
            }

            this._currentParagraph = this._subtitle.Paragraphs[this._currentIndex];
            this.SetWords(this._currentParagraph.Text);
            this._wordsIndex = -1;

            this.PrepareNextWord();
        }

        /// <summary>
        /// The load dictionaries.
        /// </summary>
        /// <param name="dictionaryFolder">
        /// The dictionary folder.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        private void LoadDictionaries(string dictionaryFolder, string dictionary)
        {
            this._changeAllDictionary = new Dictionary<string, string>();
            this._skipAllList = new List<string>();
            this._namesList = new NamesList(Configuration.DictionariesFolder, this._languageName, Configuration.Settings.WordLists.UseOnlineNamesEtc, Configuration.Settings.WordLists.NamesEtcUrl);
            this._namesEtcList = this._namesList.GetNames();
            this._namesEtcMultiWordList = this._namesList.GetMultiNames();

            foreach (string namesItem in this._namesEtcList)
            {
                this._namesEtcListUppercase.Add(namesItem.ToUpper());
            }

            if (this._languageName.StartsWith("en_", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string namesItem in this._namesEtcList)
                {
                    if (!namesItem.EndsWith('s'))
                    {
                        this._namesEtcListWithApostrophe.Add(namesItem + "'s");
                        this._namesEtcListWithApostrophe.Add(namesItem + "’s");
                    }
                    else if (!namesItem.EndsWith('\''))
                    {
                        this._namesEtcListWithApostrophe.Add(namesItem + "'");
                    }
                }
            }

            this._userWordList = new List<string>();
            this._userPhraseList = new List<string>();
            if (File.Exists(dictionaryFolder + this._languageName + "_user.xml"))
            {
                var userWordDictionary = new XmlDocument();
                userWordDictionary.Load(dictionaryFolder + this._languageName + "_user.xml");
                foreach (XmlNode node in userWordDictionary.DocumentElement.SelectNodes("word"))
                {
                    string word = node.InnerText.Trim().ToLower();
                    if (word.Contains(' '))
                    {
                        this._userPhraseList.Add(word);
                    }
                    else
                    {
                        this._userWordList.Add(word);
                    }
                }
            }

            // Add names/userdic with "." or " " or "-"
            this._wordsWithDashesOrPeriods = new List<string>();
            this._wordsWithDashesOrPeriods.AddRange(this._namesEtcMultiWordList);
            foreach (string name in this._namesEtcList)
            {
                if (name.Contains(new[] { '.', '-' }))
                {
                    this._wordsWithDashesOrPeriods.Add(name);
                }
            }

            foreach (string word in this._userWordList)
            {
                if (word.Contains(new[] { '.', '-' }))
                {
                    this._wordsWithDashesOrPeriods.Add(word);
                }
            }

            this._wordsWithDashesOrPeriods.AddRange(this._userPhraseList);

            this._changeAllDictionary = new Dictionary<string, string>();
            this.LoadHunspell(dictionary);
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
        /// The button add to dictionary_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddToDictionary_MouseEnter(object sender, EventArgs e)
        {
            this.ShowActionInfo(Configuration.Settings.Language.SpellCheck.AddToUserDictionary, this.textBoxWord.Text);
        }

        /// <summary>
        /// The show action info.
        /// </summary>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private void ShowActionInfo(string label, string text)
        {
            this.labelActionInfo.Text = string.Format("{0}: {1}", label, text.Trim());
            this._currentAction = label;
        }

        /// <summary>
        /// The button add to dictionary_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddToDictionary_MouseLeave(object sender, EventArgs e)
        {
            this.labelActionInfo.Text = string.Empty;
            this._currentAction = null;
        }

        /// <summary>
        /// The button add to names_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddToNames_MouseEnter(object sender, EventArgs e)
        {
            this.ShowActionInfo(Configuration.Settings.Language.SpellCheck.AddToNamesAndIgnoreList, this.textBoxWord.Text);
        }

        /// <summary>
        /// The button add to names_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonAddToNames_MouseLeave(object sender, EventArgs e)
        {
            this.labelActionInfo.Text = string.Empty;
            this._currentAction = null;
        }

        /// <summary>
        /// The button skip once_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSkipOnce_MouseEnter(object sender, EventArgs e)
        {
            this.ShowActionInfo(Configuration.Settings.Language.SpellCheck.SkipOnce, this.textBoxWord.Text);
        }

        /// <summary>
        /// The button skip once_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSkipOnce_MouseLeave(object sender, EventArgs e)
        {
            this.labelActionInfo.Text = string.Empty;
            this._currentAction = null;
        }

        /// <summary>
        /// The button skip all_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSkipAll_MouseEnter(object sender, EventArgs e)
        {
            this.ShowActionInfo(Configuration.Settings.Language.SpellCheck.SkipAll, this.textBoxWord.Text);
        }

        /// <summary>
        /// The button skip all_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSkipAll_MouseLeave(object sender, EventArgs e)
        {
            this.labelActionInfo.Text = string.Empty;
            this._currentAction = null;
        }

        /// <summary>
        /// The button change_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonChange_MouseEnter(object sender, EventArgs e)
        {
            this.ShowActionInfo(Configuration.Settings.Language.SpellCheck.Change, this._currentWord + " > " + this.textBoxWord.Text);
        }

        /// <summary>
        /// The button change_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonChange_MouseLeave(object sender, EventArgs e)
        {
            this.labelActionInfo.Text = string.Empty;
            this._currentAction = null;
        }

        /// <summary>
        /// The button change all_ mouse enter.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonChangeAll_MouseEnter(object sender, EventArgs e)
        {
            this.ShowActionInfo(Configuration.Settings.Language.SpellCheck.ChangeAll, this._currentWord + " > " + this.textBoxWord.Text);
        }

        /// <summary>
        /// The button change all_ mouse leave.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonChangeAll_MouseLeave(object sender, EventArgs e)
        {
            this.labelActionInfo.Text = string.Empty;
            this._currentAction = null;
        }

        /// <summary>
        /// The button spell check download_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonSpellCheckDownload_Click(object sender, EventArgs e)
        {
            using (var gd = new GetDictionaries())
            {
                gd.ShowDialog(this);
            }

            this.FillSpellCheckDictionaries(Utilities.AutoDetectLanguageName(null, this._subtitle));
            if (this.comboBoxDictionaries.Items.Count > 0 && this.comboBoxDictionaries.SelectedIndex == -1)
            {
                this.comboBoxDictionaries.SelectedIndex = 0;
            }

            this.ComboBoxDictionariesSelectedIndexChanged(null, null);
        }

        /// <summary>
        /// The push undo.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        private void PushUndo(string text, SpellCheckAction action)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (action == SpellCheckAction.ChangeAll && this._changeAllDictionary.ContainsKey(this._currentWord))
            {
                return;
            }

            string format = Configuration.Settings.Language.SpellCheck.UndoX;
            if (string.IsNullOrEmpty(format))
            {
                format = "Undo: {0}";
            }

            string undoText = string.Format(format, text);

            this._undoList.Add(new UndoObject { CurrentIndex = this._currentIndex, UndoText = undoText, UndoWord = this.textBoxWord.Text.Trim(), Action = action, CurrentWord = this._currentWord, Subtitle = new Subtitle(this._subtitle), NoOfSkippedWords = this._noOfSkippedWords, NoOfChangedWords = this._noOfChangedWords, NoOfCorrectWords = this._noOfCorrectWords, NoOfNamesEtc = this._noOfNamesEtc, NoOfAddedWords = this._noOfAddedWords, });
            this.buttonUndo.Text = undoText;
            this.buttonUndo.Visible = true;
        }

        /// <summary>
        /// The button undo_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void buttonUndo_Click(object sender, EventArgs e)
        {
            if (this._undoList.Count > 0)
            {
                var undo = this._undoList[this._undoList.Count - 1];
                this._currentIndex = undo.CurrentIndex - 1;
                this._wordsIndex = int.MaxValue - 1;
                this._noOfSkippedWords = undo.NoOfSkippedWords;
                this._noOfChangedWords = undo.NoOfChangedWords;
                this._noOfCorrectWords = undo.NoOfCorrectWords;
                this._noOfNamesEtc = undo.NoOfNamesEtc;
                this._noOfAddedWords = undo.NoOfAddedWords;

                switch (undo.Action)
                {
                    case SpellCheckAction.Change:
                        this._subtitle = this._mainWindow.UndoFromSpellCheck(undo.Subtitle);
                        break;
                    case SpellCheckAction.ChangeAll:
                        this._subtitle = this._mainWindow.UndoFromSpellCheck(undo.Subtitle);
                        this._changeAllDictionary.Remove(undo.CurrentWord);
                        break;
                    case SpellCheckAction.Skip:
                        break;
                    case SpellCheckAction.SkipAll:
                        this._skipAllList.Remove(undo.UndoWord.ToUpper());
                        if (undo.UndoWord.EndsWith('\'') || undo.UndoWord.StartsWith('\''))
                        {
                            this._skipAllList.Remove(undo.UndoWord.ToUpper().Trim('\''));
                        }

                        break;
                    case SpellCheckAction.AddToDictionary:
                        this._userWordList.Remove(undo.UndoWord);
                        this._userPhraseList.Remove(undo.UndoWord);
                        Utilities.RemoveFromUserDictionary(undo.UndoWord, this._languageName);
                        break;
                    case SpellCheckAction.AddToNamesEtc:
                        if (undo.UndoWord.Length > 1 && this._namesEtcList.Contains(undo.UndoWord))
                        {
                            this._namesEtcList.Remove(undo.UndoWord);
                            this._namesEtcListUppercase.Remove(undo.UndoWord.ToUpper());
                            if (this._languageName.StartsWith("en_", StringComparison.Ordinal) && !undo.UndoWord.EndsWith('s'))
                            {
                                this._namesEtcList.Remove(undo.UndoWord + "s");
                                this._namesEtcListUppercase.Remove(undo.UndoWord.ToUpper() + "S");
                            }

                            if (!undo.UndoWord.EndsWith('s'))
                            {
                                this._namesEtcListWithApostrophe.Remove(undo.UndoWord + "'s");
                                this._namesEtcListUppercase.Remove(undo.UndoWord.ToUpper() + "'S");
                            }

                            if (!undo.UndoWord.EndsWith('\''))
                            {
                                this._namesEtcListWithApostrophe.Remove(undo.UndoWord + "'");
                            }

                            this._namesList.Remove(undo.UndoWord);
                        }

                        break;
                    case SpellCheckAction.ChangeWholeText:
                        this._subtitle = this._mainWindow.UndoFromSpellCheck(undo.Subtitle);
                        break;
                }

                this._undoList.RemoveAt(this._undoList.Count - 1);
                if (this._undoList.Count > 0)
                {
                    this.buttonUndo.Text = this._undoList[this._undoList.Count - 1].UndoText;
                }
                else
                {
                    this.buttonUndo.Visible = false;
                }
            }

            this.PrepareNextWord();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                if (this._hunspell != null)
                {
                    this._hunspell.Dispose();
                    this._hunspell = null;
                }

                this.components.Dispose();
            }

            base.Dispose(disposing);
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
            string text = this.textBoxWord.Text.Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                System.Diagnostics.Process.Start("https://www.google.com/search?q=" + Utilities.UrlEncode(text));
            }
        }

        /// <summary>
        /// The undo object.
        /// </summary>
        private class UndoObject
        {
            /// <summary>
            /// Gets or sets the current index.
            /// </summary>
            public int CurrentIndex { get; set; }

            /// <summary>
            /// Gets or sets the undo text.
            /// </summary>
            public string UndoText { get; set; }

            /// <summary>
            /// Gets or sets the undo word.
            /// </summary>
            public string UndoWord { get; set; }

            /// <summary>
            /// Gets or sets the current word.
            /// </summary>
            public string CurrentWord { get; set; }

            /// <summary>
            /// Gets or sets the action.
            /// </summary>
            public SpellCheckAction Action { get; set; }

            /// <summary>
            /// Gets or sets the subtitle.
            /// </summary>
            public Subtitle Subtitle { get; set; }

            /// <summary>
            /// Gets or sets the no of skipped words.
            /// </summary>
            public int NoOfSkippedWords { get; set; }

            /// <summary>
            /// Gets or sets the no of changed words.
            /// </summary>
            public int NoOfChangedWords { get; set; }

            /// <summary>
            /// Gets or sets the no of correct words.
            /// </summary>
            public int NoOfCorrectWords { get; set; }

            /// <summary>
            /// Gets or sets the no of names etc.
            /// </summary>
            public int NoOfNamesEtc { get; set; }

            /// <summary>
            /// Gets or sets the no of added words.
            /// </summary>
            public int NoOfAddedWords { get; set; }
        }

        /// <summary>
        /// The suggestion parameter.
        /// </summary>
        public class SuggestionParameter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SuggestionParameter"/> class.
            /// </summary>
            /// <param name="word">
            /// The word.
            /// </param>
            /// <param name="hunspell">
            /// The hunspell.
            /// </param>
            public SuggestionParameter(string word, Hunspell hunspell)
            {
                this.InputWord = word;
                this.Suggestions = new List<string>();
                this.Hunspell = hunspell;
                this.Success = false;
            }

            /// <summary>
            /// Gets or sets the input word.
            /// </summary>
            public string InputWord { get; set; }

            /// <summary>
            /// Gets or sets the suggestions.
            /// </summary>
            public List<string> Suggestions { get; set; }

            /// <summary>
            /// Gets or sets the hunspell.
            /// </summary>
            public Hunspell Hunspell { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether success.
            /// </summary>
            public bool Success { get; set; }
        }
    }
}