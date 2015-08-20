// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindReplaceDialogHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The find replace dialog helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Logic.Enums;

    /// <summary>
    /// The find replace dialog helper.
    /// </summary>
    public class FindReplaceDialogHelper
    {
        /// <summary>
        /// The _find text.
        /// </summary>
        private readonly string _findText = string.Empty;

        /// <summary>
        /// The _reg ex.
        /// </summary>
        private readonly Regex _regEx;

        /// <summary>
        /// The _replace text.
        /// </summary>
        private readonly string _replaceText = string.Empty;

        /// <summary>
        /// The _find text lenght.
        /// </summary>
        private int _findTextLenght;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindReplaceDialogHelper"/> class.
        /// </summary>
        /// <param name="findType">
        /// The find type.
        /// </param>
        /// <param name="findText">
        /// The find text.
        /// </param>
        /// <param name="regEx">
        /// The reg ex.
        /// </param>
        /// <param name="replaceText">
        /// The replace text.
        /// </param>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="top">
        /// The top.
        /// </param>
        /// <param name="startLineIndex">
        /// The start line index.
        /// </param>
        public FindReplaceDialogHelper(FindType findType, string findText, Regex regEx, string replaceText, int left, int top, int startLineIndex)
        {
            this.FindType = findType;
            this._findText = findText;
            this._replaceText = replaceText;
            this._regEx = regEx;
            this._findTextLenght = findText.Length;
            this.WindowPositionLeft = left;
            this.WindowPositionTop = top;
            this.StartLineIndex = startLineIndex;
        }

        /// <summary>
        /// Gets or sets a value indicating whether success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the find type.
        /// </summary>
        public FindType FindType { get; set; }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the selected position.
        /// </summary>
        public int SelectedPosition { get; set; }

        /// <summary>
        /// Gets or sets the window position left.
        /// </summary>
        public int WindowPositionLeft { get; set; }

        /// <summary>
        /// Gets or sets the window position top.
        /// </summary>
        public int WindowPositionTop { get; set; }

        /// <summary>
        /// Gets or sets the start line index.
        /// </summary>
        public int StartLineIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether match in original.
        /// </summary>
        public bool MatchInOriginal { get; set; }

        /// <summary>
        /// Gets the find text length.
        /// </summary>
        public int FindTextLength
        {
            get
            {
                return this._findTextLenght;
            }
        }

        /// <summary>
        /// Gets the find text.
        /// </summary>
        public string FindText
        {
            get
            {
                return this._findText;
            }
        }

        /// <summary>
        /// Gets the replace text.
        /// </summary>
        public string ReplaceText
        {
            get
            {
                return this._replaceText;
            }
        }

        /// <summary>
        /// The find.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="originalSubtitle">
        /// The original subtitle.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Find(Subtitle subtitle, Subtitle originalSubtitle, int startIndex)
        {
            return this.FindNext(subtitle, originalSubtitle, startIndex, 0, Configuration.Settings.General.AllowEditOfOriginalSubtitle);
        }

        /// <summary>
        /// The find.
        /// </summary>
        /// <param name="textBox">
        /// The text box.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Find(TextBox textBox, int startIndex)
        {
            return this.FindNext(textBox, startIndex);
        }

        /// <summary>
        /// The find position in text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int FindPositionInText(string text, int startIndex)
        {
            if (startIndex >= text.Length && !(this.FindType == FindType.RegEx && startIndex == 0))
            {
                return -1;
            }

            switch (this.FindType)
            {
                case FindType.Normal:
                    return text.IndexOf(this._findText, startIndex, System.StringComparison.OrdinalIgnoreCase);
                case FindType.CaseSensitive:
                    return text.IndexOf(this._findText, startIndex, System.StringComparison.Ordinal);
                case FindType.RegEx:
                    {
                        Match match = this._regEx.Match(text, startIndex);
                        if (match.Success)
                        {
                            string groupName = Utilities.GetRegExGroup(this._findText);
                            if (groupName != null && match.Groups[groupName] != null && match.Groups[groupName].Success)
                            {
                                this._findTextLenght = match.Groups[groupName].Length;
                                return match.Groups[groupName].Index;
                            }

                            this._findTextLenght = match.Length;
                            return match.Index;
                        }

                        return -1;
                    }
            }

            return -1;
        }

        /// <summary>
        /// The find next.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="originalSubtitle">
        /// The original subtitle.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="allowEditOfOriginalSubtitle">
        /// The allow edit of original subtitle.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool FindNext(Subtitle subtitle, Subtitle originalSubtitle, int startIndex, int position, bool allowEditOfOriginalSubtitle)
        {
            this.Success = false;
            int index = 0;
            if (position < 0)
            {
                position = 0;
            }

            foreach (Paragraph p in subtitle.Paragraphs)
            {
                if (index >= startIndex)
                {
                    int pos = 0;
                    if (!this.MatchInOriginal)
                    {
                        pos = this.FindPositionInText(p.Text, position);
                        if (pos >= 0)
                        {
                            this.MatchInOriginal = false;
                            this.SelectedIndex = index;
                            this.SelectedPosition = pos;
                            this.Success = true;
                            return true;
                        }

                        position = 0;
                    }

                    this.MatchInOriginal = false;

                    if (originalSubtitle != null && allowEditOfOriginalSubtitle)
                    {
                        Paragraph o = Utilities.GetOriginalParagraph(index, p, originalSubtitle.Paragraphs);
                        if (o != null)
                        {
                            pos = this.FindPositionInText(o.Text, position);
                            if (pos >= 0)
                            {
                                this.MatchInOriginal = true;
                                this.SelectedIndex = index;
                                this.SelectedPosition = pos;
                                this.Success = true;
                                return true;
                            }
                        }
                    }
                }

                index++;
            }

            return false;
        }

        /// <summary>
        /// The get reg ex context menu.
        /// </summary>
        /// <param name="textBox">
        /// The text box.
        /// </param>
        /// <returns>
        /// The <see cref="ContextMenu"/>.
        /// </returns>
        public static ContextMenu GetRegExContextMenu(TextBox textBox)
        {
            var cm = new ContextMenu();
            var l = Configuration.Settings.Language.RegularExpressionContextMenu;
            cm.MenuItems.Add(l.WordBoundary, delegate { textBox.SelectedText = "\\b"; });
            cm.MenuItems.Add(l.NonWordBoundary, delegate { textBox.SelectedText = "\\B"; });
            cm.MenuItems.Add(l.NewLine, delegate { textBox.SelectedText = "\\r\\n"; });
            cm.MenuItems.Add(l.AnyDigit, delegate { textBox.SelectedText = "\\d"; });
            cm.MenuItems.Add(l.NonDigit, delegate { textBox.SelectedText = "\\D"; });
            cm.MenuItems.Add(l.AnyCharacter, delegate { textBox.SelectedText = "."; });
            cm.MenuItems.Add(l.AnyWhitespace, delegate { textBox.SelectedText = "\\s"; });
            cm.MenuItems.Add(l.NonSpaceCharacter, delegate { textBox.SelectedText = "\\S"; });
            cm.MenuItems.Add(l.ZeroOrMore, delegate { textBox.SelectedText = "*"; });
            cm.MenuItems.Add(l.OneOrMore, delegate { textBox.SelectedText = "+"; });
            cm.MenuItems.Add(l.InCharacterGroup, delegate { textBox.SelectedText = "[test]"; });
            cm.MenuItems.Add(l.NotInCharacterGroup, delegate { textBox.SelectedText = "[^test]"; });
            return cm;
        }

        /// <summary>
        /// The get reg ex context menu.
        /// </summary>
        /// <param name="comboBox">
        /// The combo box.
        /// </param>
        /// <returns>
        /// The <see cref="ContextMenu"/>.
        /// </returns>
        public static ContextMenu GetRegExContextMenu(ComboBox comboBox)
        {
            var cm = new ContextMenu();
            var l = Configuration.Settings.Language.RegularExpressionContextMenu;
            cm.MenuItems.Add(l.WordBoundary, delegate { comboBox.SelectedText = "\\b"; });
            cm.MenuItems.Add(l.NonWordBoundary, delegate { comboBox.SelectedText = "\\B"; });
            cm.MenuItems.Add(l.NewLine, delegate { comboBox.SelectedText = "\\r\\n"; });
            cm.MenuItems.Add(l.AnyDigit, delegate { comboBox.SelectedText = "\\d"; });
            cm.MenuItems.Add(l.NonDigit, delegate { comboBox.SelectedText = "\\D"; });
            cm.MenuItems.Add(l.AnyCharacter, delegate { comboBox.SelectedText = "."; });
            cm.MenuItems.Add(l.AnyWhitespace, delegate { comboBox.SelectedText = "\\s"; });
            cm.MenuItems.Add(l.NonSpaceCharacter, delegate { comboBox.SelectedText = "\\S"; });
            cm.MenuItems.Add(l.ZeroOrMore, delegate { comboBox.SelectedText = "*"; });
            cm.MenuItems.Add(l.OneOrMore, delegate { comboBox.SelectedText = "+"; });
            cm.MenuItems.Add(l.InCharacterGroup, delegate { comboBox.SelectedText = "[test]"; });
            cm.MenuItems.Add(l.NotInCharacterGroup, delegate { comboBox.SelectedText = "[^test]"; });
            return cm;
        }

        /// <summary>
        /// The get replace text context menu.
        /// </summary>
        /// <param name="textBox">
        /// The text box.
        /// </param>
        /// <returns>
        /// The <see cref="ContextMenu"/>.
        /// </returns>
        public static ContextMenu GetReplaceTextContextMenu(TextBox textBox)
        {
            var cm = new ContextMenu();
            cm.MenuItems.Add(Configuration.Settings.Language.RegularExpressionContextMenu.NewLineShort, delegate { textBox.SelectedText = "\\n"; });
            return cm;
        }

        /// <summary>
        /// The find next.
        /// </summary>
        /// <param name="textBox">
        /// The text box.
        /// </param>
        /// <param name="startIndex">
        /// The start index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool FindNext(TextBox textBox, int startIndex)
        {
            this.Success = false;
            startIndex++;
            if (startIndex < textBox.Text.Length)
            {
                if (this.FindType == FindType.RegEx)
                {
                    Match match = this._regEx.Match(textBox.Text, startIndex);
                    if (match.Success)
                    {
                        string groupName = Utilities.GetRegExGroup(this._findText);
                        if (groupName != null && match.Groups[groupName] != null && match.Groups[groupName].Success)
                        {
                            this._findTextLenght = match.Groups[groupName].Length;
                            this.SelectedIndex = match.Groups[groupName].Index;
                        }
                        else
                        {
                            this._findTextLenght = match.Length;
                            this.SelectedIndex = match.Index;
                        }

                        this.Success = true;
                    }

                    return match.Success;
                }

                string searchText = textBox.Text.Substring(startIndex);
                int pos = this.FindPositionInText(searchText, 0);
                if (pos >= 0)
                {
                    this.SelectedIndex = pos + startIndex;
                    return true;
                }
            }

            return false;
        }
    }
}