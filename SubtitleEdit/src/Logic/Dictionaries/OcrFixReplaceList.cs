namespace Nikse.SubtitleEdit.Logic.Dictionaries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using System.Xml;

    using Nikse.SubtitleEdit.Core;

    public class OcrFixReplaceList
    {
        private static readonly Regex RegExQuestion = new Regex(@"\S\?[A-ZÆØÅÄÖÉÈÀÙÂÊÎÔÛËÏa-zæøåäöéèàùâêîôûëï]", RegexOptions.Compiled);

        private static readonly Regex RegExIandZero = new Regex(@"[a-zæøåöääöéèàùâêîôûëï][I1]", RegexOptions.Compiled);

        private static readonly Regex RegExTime1 = new Regex(@"[a-zæøåöääöéèàùâêîôûëï]0", RegexOptions.Compiled);

        private static readonly Regex RegExTime2 = new Regex(@"0[a-zæøåöääöéèàùâêîôûëï]", RegexOptions.Compiled);

        private static readonly Regex HexNumber = new Regex(@"^#?[\dABDEFabcdef]+$", RegexOptions.Compiled);

        private static readonly Regex StartEndEndsWithNumber = new Regex(@"^\d+.+\d$", RegexOptions.Compiled);

        private readonly Dictionary<string, string> _beginLineReplaceList;

        private readonly Dictionary<string, string> _endLineReplaceList;

        private readonly Dictionary<string, string> _partialLineAlwaysReplaceList;

        private readonly Dictionary<string, string> _partialWordReplaceList;

        private readonly Dictionary<string, string> _partialWordReplaceListAlways;

        private readonly Dictionary<string, string> _regExList;

        private readonly string _replaceListXmlFileName;

        private readonly Dictionary<string, string> _wholeLineReplaceList;

        public Dictionary<string, string> PartialLineWordBoundaryReplaceList;

        public Dictionary<string, string> WordReplaceList;

        public OcrFixReplaceList(string replaceListXmlFileName)
        {
            this._replaceListXmlFileName = replaceListXmlFileName;
            this.WordReplaceList = new Dictionary<string, string>();
            this.PartialLineWordBoundaryReplaceList = new Dictionary<string, string>();
            this._partialLineAlwaysReplaceList = new Dictionary<string, string>();
            this._beginLineReplaceList = new Dictionary<string, string>();
            this._endLineReplaceList = new Dictionary<string, string>();
            this._wholeLineReplaceList = new Dictionary<string, string>();
            this._partialWordReplaceListAlways = new Dictionary<string, string>();
            this._partialWordReplaceList = new Dictionary<string, string>();
            this._regExList = new Dictionary<string, string>();

            XmlDocument doc = this.LoadXmlReplaceListDocument();
            XmlDocument userDoc = this.LoadXmlReplaceListUserDocument();

            this.WordReplaceList = LoadReplaceList(doc, "WholeWords");
            this._partialWordReplaceListAlways = LoadReplaceList(doc, "PartialWordsAlways");
            this._partialWordReplaceList = LoadReplaceList(doc, "PartialWords");
            this.PartialLineWordBoundaryReplaceList = LoadReplaceList(doc, "PartialLines");
            this._partialLineAlwaysReplaceList = LoadReplaceList(doc, "PartialAlwaysLines");
            this._beginLineReplaceList = LoadReplaceList(doc, "BeginLines");
            this._endLineReplaceList = LoadReplaceList(doc, "EndLines");
            this._wholeLineReplaceList = LoadReplaceList(doc, "WholeLines");
            this._regExList = LoadRegExList(doc, "RegularExpressions");

            foreach (KeyValuePair<string, string> kp in LoadReplaceList(userDoc, "RemovedWholeWords"))
            {
                if (this.WordReplaceList.ContainsKey(kp.Key))
                {
                    this.WordReplaceList.Remove(kp.Key);
                }
            }

            foreach (KeyValuePair<string, string> kp in LoadReplaceList(userDoc, "WholeWords"))
            {
                if (!this.WordReplaceList.ContainsKey(kp.Key))
                {
                    this.WordReplaceList.Add(kp.Key, kp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kp in LoadReplaceList(userDoc, "RemovedPartialLines"))
            {
                if (this.PartialLineWordBoundaryReplaceList.ContainsKey(kp.Key))
                {
                    this.PartialLineWordBoundaryReplaceList.Remove(kp.Key);
                }
            }

            foreach (KeyValuePair<string, string> kp in LoadReplaceList(userDoc, "PartialLines"))
            {
                if (!this.PartialLineWordBoundaryReplaceList.ContainsKey(kp.Key))
                {
                    this.PartialLineWordBoundaryReplaceList.Add(kp.Key, kp.Value);
                }
            }
        }

        private string ReplaceListXmlFileNameUser
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(this._replaceListXmlFileName), Path.GetFileNameWithoutExtension(this._replaceListXmlFileName) + "_User" + Path.GetExtension(this._replaceListXmlFileName));
            }
        }

        public static OcrFixReplaceList FromLanguageId(string languageId)
        {
            return new OcrFixReplaceList(Configuration.DictionariesFolder + languageId + "_OCRFixReplaceList.xml");
        }

        public static string FixLowerCaseLInsideUpperCaseWord(string word)
        {
            if (word.Length > 3 && word.Replace("l", string.Empty).ToUpper() == word.Replace("l", string.Empty))
            {
                if (!word.Contains('<') && !word.Contains('>') && !word.Contains('\''))
                {
                    word = word.Replace('l', 'I');
                }
            }

            return word;
        }

        public static string FixIor1InsideLowerCaseWord(string word)
        {
            if (StartEndEndsWithNumber.IsMatch(word))
            {
                return word;
            }

            if (word.Contains('2') || word.Contains('3') || word.Contains('4') || word.Contains('5') || word.Contains('6') || word.Contains('7') || word.Contains('8') || word.Contains('9'))
            {
                return word;
            }

            if (HexNumber.IsMatch(word))
            {
                return word;
            }

            if (word.LastIndexOf('I') > 0 || word.LastIndexOf('1') > 0)
            {
                Match match = RegExIandZero.Match(word);
                while (match.Success)
                {
                    if (word[match.Index + 1] == 'I' || word[match.Index + 1] == '1')
                    {
                        bool doFix = word[match.Index + 1] != 'I' && match.Index >= 1 && word.Substring(match.Index - 1).StartsWith("Mc");
                        if (word[match.Index + 1] == 'I' && match.Index >= 2 && word.Substring(match.Index - 2).StartsWith("Mac"))
                        {
                            doFix = false;
                        }

                        if (doFix)
                        {
                            string oldText = word;
                            word = word.Substring(0, match.Index + 1) + "l";
                            if (match.Index + 2 < oldText.Length)
                            {
                                word += oldText.Substring(match.Index + 2);
                            }
                        }
                    }

                    match = RegExIandZero.Match(word, match.Index + 1);
                }
            }

            return word;
        }

        public static string Fix0InsideLowerCaseWord(string word)
        {
            if (StartEndEndsWithNumber.IsMatch(word))
            {
                return word;
            }

            if (word.Contains('1') || word.Contains('2') || word.Contains('3') || word.Contains('4') || word.Contains('5') || word.Contains('6') || word.Contains('7') || word.Contains('8') || word.Contains('9') || word.EndsWith("a.m", StringComparison.Ordinal) || word.EndsWith("p.m", StringComparison.Ordinal) || word.EndsWith("am", StringComparison.Ordinal) || word.EndsWith("pm", StringComparison.Ordinal))
            {
                return word;
            }

            if (HexNumber.IsMatch(word))
            {
                return word;
            }

            if (word.LastIndexOf('0') > 0)
            {
                Match match = RegExTime1.Match(word);
                while (match.Success)
                {
                    if (word[match.Index + 1] == '0')
                    {
                        string oldText = word;
                        word = word.Substring(0, match.Index + 1) + "o";
                        if (match.Index + 2 < oldText.Length)
                        {
                            word += oldText.Substring(match.Index + 2);
                        }
                    }

                    match = RegExTime1.Match(word);
                }

                match = RegExTime2.Match(word);
                while (match.Success)
                {
                    if (word[match.Index] == '0')
                    {
                        if (match.Index == 0 || !@"123456789".Contains(word[match.Index - 1]))
                        {
                            string oldText = word;
                            word = word.Substring(0, match.Index) + "o";
                            if (match.Index + 1 < oldText.Length)
                            {
                                word += oldText.Substring(match.Index + 1);
                            }
                        }
                    }

                    match = RegExTime2.Match(word, match.Index + 1);
                }
            }

            return word;
        }

        public static string ReplaceWord(string text, string word, string newWord)
        {
            StringBuilder sb = new StringBuilder();
            if (word != null && text != null && text.Contains(word))
            {
                const string startChars = @" ¡¿<>-""”“()[]'‘`´¶♪¿¡.…—!?,:;/";
                int appendFrom = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text.Substring(i).StartsWith(word) && i >= appendFrom)
                    {
                        bool startOk = i == 0;
                        if (!startOk)
                        {
                            startOk = (startChars + Environment.NewLine).Contains(text[i - 1]);
                        }

                        if (!startOk && word.StartsWith(' '))
                        {
                            startOk = true;
                        }

                        if (startOk)
                        {
                            bool endOk = i + word.Length == text.Length;
                            if (!endOk)
                            {
                                endOk = (startChars + Environment.NewLine).Contains(text[i + word.Length]);
                            }

                            if (!endOk)
                            {
                                endOk = newWord.EndsWith(' ');
                            }

                            if (endOk)
                            {
                                sb.Append(newWord);
                                appendFrom = i + word.Length;
                            }
                        }
                    }

                    if (i >= appendFrom)
                    {
                        sb.Append(text[i]);
                    }
                }
            }

            return sb.ToString();
        }

        public string FixOcrErrorViaLineReplaceList(string input)
        {
            // Whole fromLine
            foreach (string from in this._wholeLineReplaceList.Keys)
            {
                if (input == from)
                {
                    return this._wholeLineReplaceList[from];
                }
            }

            string newText = input;
            string pre = string.Empty;
            if (newText.StartsWith("<i>", StringComparison.Ordinal))
            {
                pre += "<i>";
                newText = newText.Remove(0, 3);
            }

            while (newText.Length > 1 && @" -""['¶(".Contains(newText[0]))
            {
                pre += newText[0];
                newText = newText.Substring(1);
            }

            if (newText.StartsWith("<i>", StringComparison.Ordinal))
            {
                pre += "<i>";
                newText = newText.Remove(0, 3);
            }

            // begin fromLine
            string[] lines = newText.SplitToLines();
            StringBuilder sb = new StringBuilder();
            foreach (string l in lines)
            {
                string s = l;
                foreach (string from in this._beginLineReplaceList.Keys)
                {
                    if (s.Contains(from))
                    {
                        if (s.StartsWith(from))
                        {
                            s = s.Remove(0, from.Length).Insert(0, this._beginLineReplaceList[from]);
                        }

                        if (s.Contains(". " + from))
                        {
                            s = s.Replace(". " + from, ". " + this._beginLineReplaceList[from]);
                        }

                        if (s.Contains("! " + from))
                        {
                            s = s.Replace("! " + from, "! " + this._beginLineReplaceList[from]);
                        }

                        if (s.Contains("? " + from))
                        {
                            s = s.Replace("? " + from, "? " + this._beginLineReplaceList[from]);
                        }

                        if (s.Contains("." + Environment.NewLine + from))
                        {
                            s = s.Replace(". " + Environment.NewLine + from, ". " + Environment.NewLine + this._beginLineReplaceList[from]);
                        }

                        if (s.Contains("! " + Environment.NewLine + from))
                        {
                            s = s.Replace("! " + Environment.NewLine + from, "! " + Environment.NewLine + this._beginLineReplaceList[from]);
                        }

                        if (s.Contains("? " + Environment.NewLine + from))
                        {
                            s = s.Replace("? " + Environment.NewLine + from, "? " + Environment.NewLine + this._beginLineReplaceList[from]);
                        }

                        if (s.StartsWith('"') && !from.StartsWith('"') && s.StartsWith("\"" + from))
                        {
                            s = s.Replace("\"" + from, "\"" + this._beginLineReplaceList[from]);
                        }
                    }
                }

                sb.AppendLine(s);
            }

            newText = sb.ToString().TrimEnd('\r').TrimEnd('\n').TrimEnd('\r').TrimEnd('\n');
            newText = pre + newText;

            string post = string.Empty;
            if (newText.EndsWith("</i>", StringComparison.Ordinal))
            {
                newText = newText.Remove(newText.Length - 4, 4);
                post = "</i>";
            }

            foreach (string from in this._endLineReplaceList.Keys)
            {
                if (newText.EndsWith(from, StringComparison.Ordinal))
                {
                    int position = newText.Length - from.Length;
                    newText = newText.Remove(position).Insert(position, this._endLineReplaceList[from]);
                }
            }

            newText += post;

            foreach (string from in this.PartialLineWordBoundaryReplaceList.Keys)
            {
                if (newText.FastIndexOf(from) >= 0)
                {
                    newText = ReplaceWord(newText, from, this.PartialLineWordBoundaryReplaceList[from]);
                }
            }

            foreach (string from in this._partialLineAlwaysReplaceList.Keys)
            {
                if (newText.FastIndexOf(from) >= 0)
                {
                    newText = newText.Replace(from, this._partialLineAlwaysReplaceList[from]);
                }
            }

            foreach (string findWhat in this._regExList.Keys)
            {
                newText = Regex.Replace(newText, findWhat, this._regExList[findWhat], RegexOptions.Multiline);
            }

            return newText;
        }

        public IEnumerable<string> CreateGuessesFromLetters(string word)
        {
            List<string> list = new List<string>();
            foreach (string letter in this._partialWordReplaceList.Keys)
            {
                string s = word;
                int i = 0;
                while (s.Contains(letter) && i < 10)
                {
                    int index = s.FastIndexOf(letter);
                    s = AddToGuessList(list, s, index, letter, this._partialWordReplaceList[letter]);
                    AddToGuessList(list, word, index, letter, this._partialWordReplaceList[letter]);
                    i++;
                }

                s = word;
                i = 0;
                while (s.Contains(letter) && i < 10)
                {
                    int index = s.LastIndexOf(letter, StringComparison.Ordinal);
                    s = AddToGuessList(list, s, index, letter, this._partialWordReplaceList[letter]);
                    AddToGuessList(list, word, index, letter, this._partialWordReplaceList[letter]);
                    i++;
                }
            }

            return list;
        }

        public string FixCommonWordErrors(string word)
        {
            if (Configuration.Settings.Tools.OcrFixUseHardcodedRules)
            {
                word = word.Replace("ﬁ", "fi");
                word = word.Replace("ν", "v"); // NOTE: first 'v' is a special unicode character!!!!

                if (word.Contains('’'))
                {
                    word = word.Replace('’', '\'');
                }

                if (word.Contains('`'))
                {
                    word = word.Replace('`', '\'');
                }

                if (word.Contains('‘'))
                {
                    word = word.Replace('‘', '\'');
                }

                if (word.Contains('—'))
                {
                    word = word.Replace('—', '-');
                }

                while (word.Contains("--"))
                {
                    word = word.Replace("--", "-");
                }

                if (word.Contains('|'))
                {
                    word = word.Replace('|', 'l');
                }

                if (word.Contains("vx/"))
                {
                    word = word.Replace("vx/", "w");
                }

                if (word.Contains('¤'))
                {
                    if (Regex.IsMatch(word, "[A-ZÆØÅÄÖÉÈÀÙÂÊÎÔÛËÏa-zæøåäöéèàùâêîôûëï]¤"))
                    {
                        word = word.Replace('¤', 'o');
                    }
                }
            }

            // always replace list
            foreach (string letter in this._partialWordReplaceListAlways.Keys)
            {
                word = word.Replace(letter, this._partialWordReplaceListAlways[letter]);
            }

            string pre = string.Empty;
            string post = string.Empty;

            if (word.StartsWith("<i>", StringComparison.Ordinal))
            {
                pre += "<i>";
                word = word.Remove(0, 3);
            }

            while (word.Length > 2 && word.StartsWith(Environment.NewLine, StringComparison.Ordinal))
            {
                pre += Environment.NewLine;
                word = word.Substring(2);
            }

            while (word.Length > 1 && word[0] == '-')
            {
                pre += "-";
                word = word.Substring(1);
            }

            while (word.Length > 1 && word[0] == '.')
            {
                pre += ".";
                word = word.Substring(1);
            }

            while (word.Length > 1 && word[0] == '"')
            {
                pre += "\"";
                word = word.Substring(1);
            }

            if (word.Length > 1 && word[0] == '(')
            {
                pre += "(";
                word = word.Substring(1);
            }

            if (word.StartsWith("<i>", StringComparison.Ordinal))
            {
                pre += "<i>";
                word = word.Remove(0, 3);
            }

            while (word.Length > 2 && word.EndsWith(Environment.NewLine))
            {
                post += Environment.NewLine;
                word = word.Substring(0, word.Length - 2);
            }

            while (word.Length > 1 && word.EndsWith('"'))
            {
                post = post + "\"";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.Length > 1 && word.EndsWith('.'))
            {
                post = post + ".";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith(',') && word.Length > 1)
            {
                post = post + ",";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith('?') && word.Length > 1)
            {
                post = post + "?";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith('!') && word.Length > 1)
            {
                post = post + "!";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith(')') && word.Length > 1)
            {
                post = post + ")";
                word = word.Substring(0, word.Length - 1);
            }

            if (word.EndsWith("</i>", StringComparison.Ordinal))
            {
                post = post + "</i>";
                word = word.Remove(word.Length - 4, 4);
            }

            string preWordPost = pre + word + post;
            if (word.Length == 0)
            {
                return preWordPost;
            }

            if (word.Contains('?'))
            {
                Match match = RegExQuestion.Match(word);
                if (match.Success)
                {
                    word = word.Insert(match.Index + 2, " ");
                }
            }

            foreach (string from in this.WordReplaceList.Keys)
            {
                if (word.Length == from.Length)
                {
                    if (word == from)
                    {
                        return pre + this.WordReplaceList[from] + post;
                    }
                }
                else if (word.Length + post.Length == from.Length)
                {
                    if (string.CompareOrdinal(word + post, from) == 0)
                    {
                        return pre + this.WordReplaceList[from];
                    }
                }

                if (pre.Length + word.Length + post.Length == from.Length && string.CompareOrdinal(preWordPost, from) == 0)
                {
                    return this.WordReplaceList[from];
                }
            }

            if (Configuration.Settings.Tools.OcrFixUseHardcodedRules)
            {
                // uppercase I or 1 inside lowercase fromWord (will be replaced by lowercase L)
                word = FixIor1InsideLowerCaseWord(word);

                // uppercase 0 inside lowercase fromWord (will be replaced by lowercase L)
                word = Fix0InsideLowerCaseWord(word);

                // uppercase I or 1 inside lowercase fromWord (will be replaced by lowercase L)
                word = FixIor1InsideLowerCaseWord(word);

                word = FixLowerCaseLInsideUpperCaseWord(word); // eg. SCARLETTl => SCARLETTI
            }

            // Retry fromWord replace list
            foreach (string from in this.WordReplaceList.Keys)
            {
                if (word.Length == from.Length)
                {
                    if (string.CompareOrdinal(word, from) == 0)
                    {
                        return pre + this.WordReplaceList[from] + post;
                    }
                }
                else if (word.Length + post.Length == from.Length)
                {
                    if (string.CompareOrdinal(word + post, from) == 0)
                    {
                        return pre + this.WordReplaceList[from];
                    }
                }

                if (pre.Length + word.Length + post.Length == from.Length && string.CompareOrdinal(preWordPost, from) == 0)
                {
                    return this.WordReplaceList[from];
                }
            }

            return preWordPost;
        }

        public string FixCommonWordErrorsQuick(string word)
        {
            // always replace list
            foreach (string letter in this._partialWordReplaceListAlways.Keys)
            {
                word = word.Replace(letter, this._partialWordReplaceListAlways[letter]);
            }

            string pre = string.Empty;
            string post = string.Empty;

            if (word.StartsWith("<i>", StringComparison.Ordinal))
            {
                pre += "<i>";
                word = word.Remove(0, 3);
            }

            while (word.StartsWith(Environment.NewLine) && word.Length > 2)
            {
                pre += Environment.NewLine;
                word = word.Substring(2);
            }

            while (word.Length > 1 && word[0] == '-')
            {
                pre += "-";
                word = word.Substring(1);
            }

            while (word.Length > 1 && word[0] == '.')
            {
                pre += ".";
                word = word.Substring(1);
            }

            while (word.Length > 1 && word[0] == '"')
            {
                pre += "\"";
                word = word.Substring(1);
            }

            if (word.Length > 1 && word[0] == '(')
            {
                pre += "(";
                word = word.Substring(1);
            }

            if (word.StartsWith("<i>", StringComparison.Ordinal))
            {
                pre += "<i>";
                word = word.Remove(0, 3);
            }

            while (word.EndsWith(Environment.NewLine) && word.Length > 2)
            {
                post += Environment.NewLine;
                word = word.Substring(0, word.Length - 2);
            }

            while (word.EndsWith('"') && word.Length > 1)
            {
                post = post + "\"";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith('.') && word.Length > 1)
            {
                post = post + ".";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith(',') && word.Length > 1)
            {
                post = post + ",";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith('?') && word.Length > 1)
            {
                post = post + "?";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith('!') && word.Length > 1)
            {
                post = post + "!";
                word = word.Substring(0, word.Length - 1);
            }

            while (word.EndsWith(')') && word.Length > 1)
            {
                post = post + ")";
                word = word.Substring(0, word.Length - 1);
            }

            if (word.EndsWith("</i>", StringComparison.Ordinal))
            {
                post = post + "</i>";
                word = word.Remove(word.Length - 4, 4);
            }

            string preWordPost = pre + word + post;
            if (word.Length == 0)
            {
                return preWordPost;
            }

            foreach (string from in this.WordReplaceList.Keys)
            {
                if (word.Length == from.Length)
                {
                    if (string.CompareOrdinal(word, from) == 0)
                    {
                        return pre + this.WordReplaceList[from] + post;
                    }
                }
                else if (word.Length + post.Length == from.Length)
                {
                    if (string.CompareOrdinal(word + post, from) == 0)
                    {
                        return pre + this.WordReplaceList[from];
                    }
                }

                if (pre.Length + word.Length + post.Length == from.Length && string.CompareOrdinal(preWordPost, from) == 0)
                {
                    return this.WordReplaceList[from];
                }
            }

            return preWordPost;
        }

        public bool RemoveWordOrPartial(string word)
        {
            if (word.Contains(' '))
            {
                if (this.DeletePartialLineFromWordList(word))
                {
                    if (this.PartialLineWordBoundaryReplaceList.ContainsKey(word))
                    {
                        this.PartialLineWordBoundaryReplaceList.Remove(word);
                    }

                    return true;
                }

                return false;
            }

            if (this.DeleteWordFromWordList(word))
            {
                if (this.WordReplaceList.ContainsKey(word))
                {
                    this.WordReplaceList.Remove(word);
                }

                return true;
            }

            return false;
        }

        public bool AddWordOrPartial(string fromWord, string toWord)
        {
            if (fromWord.Contains(' '))
            {
                if (this.SavePartialLineToWordList(fromWord, toWord))
                {
                    if (!this.PartialLineWordBoundaryReplaceList.ContainsKey(fromWord))
                    {
                        this.PartialLineWordBoundaryReplaceList.Add(fromWord, toWord);
                    }

                    return true;
                }

                return false;
            }

            if (this.SaveWordToWordList(fromWord, toWord))
            {
                if (!this.WordReplaceList.ContainsKey(fromWord))
                {
                    this.WordReplaceList.Add(fromWord, toWord);
                }

                return true;
            }

            return false;
        }

        public void AddToWholeLineList(string fromLine, string toLine)
        {
            try
            {
                XmlDocument userDocument = this.LoadXmlReplaceListUserDocument();
                if (!this._wholeLineReplaceList.ContainsKey(fromLine))
                {
                    this._wholeLineReplaceList.Add(fromLine, toLine);
                }

                XmlNode wholeWordsNode = userDocument.DocumentElement.SelectSingleNode("WholeLines");
                if (wholeWordsNode != null)
                {
                    XmlNode newNode = userDocument.CreateNode(XmlNodeType.Element, "Line", null);
                    XmlAttribute aFrom = userDocument.CreateAttribute("from");
                    XmlAttribute aTo = userDocument.CreateAttribute("to");
                    aTo.InnerText = toLine;
                    aFrom.InnerText = fromLine;
                    newNode.Attributes.Append(aFrom);
                    newNode.Attributes.Append(aTo);
                    wholeWordsNode.AppendChild(newNode);
                    userDocument.Save(this._replaceListXmlFileName);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception + Environment.NewLine + exception.StackTrace);
            }
        }

        private static Dictionary<string, string> LoadReplaceList(XmlDocument doc, string name)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            if (doc.DocumentElement != null)
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode(name);
                if (node != null)
                {
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        if (item.Attributes != null && item.Attributes["to"] != null && item.Attributes["from"] != null)
                        {
                            string to = item.Attributes["to"].InnerText;
                            string from = item.Attributes["from"].InnerText;
                            if (!list.ContainsKey(from))
                            {
                                list.Add(from, to);
                            }
                        }
                    }
                }
            }

            return list;
        }

        private static Dictionary<string, string> LoadRegExList(XmlDocument doc, string name)
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            if (doc.DocumentElement != null)
            {
                XmlNode node = doc.DocumentElement.SelectSingleNode(name);
                if (node != null)
                {
                    foreach (XmlNode item in node.ChildNodes)
                    {
                        if (item.Attributes != null && item.Attributes["replaceWith"] != null && item.Attributes["find"] != null)
                        {
                            string to = item.Attributes["replaceWith"].InnerText;
                            string from = item.Attributes["find"].InnerText;
                            if (!list.ContainsKey(from))
                            {
                                list.Add(from, to);
                            }
                        }
                    }
                }
            }

            return list;
        }

        private static string AddToGuessList(List<string> list, string word, int index, string letter, string replaceLetters)
        {
            if (string.IsNullOrEmpty(word) || index < 0 || index + letter.Length - 1 >= word.Length)
            {
                return word;
            }

            string s = word.Remove(index, letter.Length);
            if (index >= s.Length)
            {
                s += replaceLetters;
            }
            else
            {
                s = s.Insert(index, replaceLetters);
            }

            if (!list.Contains(s))
            {
                list.Add(s);
            }

            return s;
        }

        private bool DeleteWordFromWordList(string fromWord)
        {
            const string replaceListName = "WholeWords";

            XmlDocument doc = this.LoadXmlReplaceListDocument();
            Dictionary<string, string> list = LoadReplaceList(doc, replaceListName);

            XmlDocument userDoc = this.LoadXmlReplaceListUserDocument();
            Dictionary<string, string> userList = LoadReplaceList(userDoc, replaceListName);

            return this.DeleteFromList(fromWord, userDoc, replaceListName, "Word", list, userList);
        }

        private bool DeletePartialLineFromWordList(string fromWord)
        {
            const string replaceListName = "PartialLines";

            XmlDocument doc = this.LoadXmlReplaceListDocument();
            Dictionary<string, string> list = LoadReplaceList(doc, replaceListName);

            XmlDocument userDoc = this.LoadXmlReplaceListUserDocument();
            Dictionary<string, string> userList = LoadReplaceList(userDoc, replaceListName);

            return this.DeleteFromList(fromWord, userDoc, replaceListName, "LinePart", list, userList);
        }

        private bool DeleteFromList(string word, XmlDocument userDoc, string replaceListName, string elementName, Dictionary<string, string> dictionary, Dictionary<string, string> userDictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (userDictionary == null)
            {
                throw new ArgumentNullException("userDictionary");
            }

            bool removed = false;
            if (userDictionary.ContainsKey(word))
            {
                userDictionary.Remove(word);
                XmlNode wholeWordsNode = userDoc.DocumentElement.SelectSingleNode(replaceListName);
                if (wholeWordsNode != null)
                {
                    wholeWordsNode.RemoveAll();
                    foreach (KeyValuePair<string, string> kvp in userDictionary)
                    {
                        XmlNode newNode = userDoc.CreateNode(XmlNodeType.Element, elementName, null);
                        XmlAttribute aFrom = userDoc.CreateAttribute("from");
                        XmlAttribute aTo = userDoc.CreateAttribute("to");
                        aFrom.InnerText = kvp.Key;
                        aTo.InnerText = kvp.Value;
                        newNode.Attributes.Append(aTo);
                        newNode.Attributes.Append(aFrom);
                        wholeWordsNode.AppendChild(newNode);
                    }

                    userDoc.Save(this.ReplaceListXmlFileNameUser);
                    removed = true;
                }
            }

            if (dictionary.ContainsKey(word))
            {
                XmlNode wholeWordsNode = userDoc.DocumentElement.SelectSingleNode("Removed" + replaceListName);
                if (wholeWordsNode != null)
                {
                    XmlNode newNode = userDoc.CreateNode(XmlNodeType.Element, elementName, null);
                    XmlAttribute aFrom = userDoc.CreateAttribute("from");
                    XmlAttribute aTo = userDoc.CreateAttribute("to");
                    aFrom.InnerText = word;
                    aTo.InnerText = string.Empty;
                    newNode.Attributes.Append(aTo);
                    newNode.Attributes.Append(aFrom);
                    wholeWordsNode.AppendChild(newNode);
                    userDoc.Save(this.ReplaceListXmlFileNameUser);
                    removed = true;
                }
            }

            return removed;
        }

        private XmlDocument LoadXmlReplaceListDocument()
        {
            const string xmlText = "<ReplaceList><WholeWords/><PartialLines/><BeginLines/><EndLines/><WholeLines/></ReplaceList>";
            XmlDocument doc = new XmlDocument();
            if (File.Exists(this._replaceListXmlFileName))
            {
                try
                {
                    doc.Load(this._replaceListXmlFileName);
                }
                catch
                {
                    doc.LoadXml(xmlText);
                }
            }
            else
            {
                doc.LoadXml(xmlText);
            }

            return doc;
        }

        private XmlDocument LoadXmlReplaceListUserDocument()
        {
            const string xmlText = "<ReplaceList><WholeWords/><PartialLines/><BeginLines/><EndLines/><WholeLines/><RemovedWholeWords/><RemovedPartialLines/><RemovedBeginLines/><RemovedEndLines/><RemovedWholeLines/></ReplaceList>";
            XmlDocument doc = new XmlDocument();
            if (File.Exists(this.ReplaceListXmlFileNameUser))
            {
                try
                {
                    doc.Load(this.ReplaceListXmlFileNameUser);
                }
                catch
                {
                    doc.LoadXml(xmlText);
                }
            }
            else
            {
                doc.LoadXml(xmlText);
            }

            return doc;
        }

        private bool SaveWordToWordList(string fromWord, string toWord)
        {
            const string replaceListName = "WholeWords";

            XmlDocument doc = this.LoadXmlReplaceListDocument();
            Dictionary<string, string> list = LoadReplaceList(doc, replaceListName);

            XmlDocument userDoc = this.LoadXmlReplaceListUserDocument();
            Dictionary<string, string> userList = LoadReplaceList(userDoc, replaceListName);

            return this.SaveToList(fromWord, toWord, userDoc, replaceListName, "Word", list, userList);
        }

        private bool SavePartialLineToWordList(string fromWord, string toWord)
        {
            const string replaceListName = "PartialLines";

            XmlDocument doc = this.LoadXmlReplaceListDocument();
            Dictionary<string, string> list = LoadReplaceList(doc, replaceListName);

            XmlDocument userDoc = this.LoadXmlReplaceListUserDocument();
            Dictionary<string, string> userList = LoadReplaceList(userDoc, replaceListName);

            return this.SaveToList(fromWord, toWord, userDoc, replaceListName, "LinePart", list, userList);
        }

        private bool SaveToList(string fromWord, string toWord, XmlDocument userDoc, string replaceListName, string elementName, Dictionary<string, string> dictionary, Dictionary<string, string> userDictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (userDictionary == null)
            {
                throw new ArgumentNullException("userDictionary");
            }

            if (userDictionary.ContainsKey(fromWord))
            {
                return false;
            }

            userDictionary.Add(fromWord, toWord);
            XmlNode wholeWordsNode = userDoc.DocumentElement.SelectSingleNode(replaceListName);
            if (wholeWordsNode != null)
            {
                XmlNode newNode = userDoc.CreateNode(XmlNodeType.Element, elementName, null);
                XmlAttribute aFrom = userDoc.CreateAttribute("from");
                XmlAttribute aTo = userDoc.CreateAttribute("to");
                aTo.InnerText = toWord;
                aFrom.InnerText = fromWord;
                newNode.Attributes.Append(aFrom);
                newNode.Attributes.Append(aTo);
                wholeWordsNode.AppendChild(newNode);
                userDoc.Save(this.ReplaceListXmlFileNameUser);
            }

            return true;
        }
    }
}