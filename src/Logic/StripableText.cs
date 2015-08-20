// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StripableText.cs" company="">
//   
// </copyright>
// <summary>
//   The stripable text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The stripable text.
    /// </summary>
    public class StripableText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StripableText"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public StripableText(string text)
            : this(text, " >-\"”“['‘`´¶(♪¿¡.…—", " -\"”“]'`´¶)♪.!?:…—")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StripableText"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="stripStartCharacters">
        /// The strip start characters.
        /// </param>
        /// <param name="stripEndCharacters">
        /// The strip end characters.
        /// </param>
        public StripableText(string text, string stripStartCharacters, string stripEndCharacters)
        {
            this.OriginalText = text;

            this.Pre = string.Empty;
            if (text.Length > 0 && ("<{" + stripStartCharacters).Contains(text[0]))
            {
                int beginLength;
                do
                {
                    beginLength = text.Length;

                    while (text.Length > 0 && stripStartCharacters.Contains(text[0]))
                    {
                        this.Pre += text[0];
                        text = text.Remove(0, 1);
                    }

                    // ASS/SSA codes like {\an9}
                    int endIndex = text.IndexOf('}');
                    if (endIndex > 0 && text.StartsWith("{\\", StringComparison.Ordinal))
                    {
                        int nextStartIndex = text.IndexOf('{', 2);
                        if (nextStartIndex == -1 || nextStartIndex > endIndex)
                        {
                            endIndex++;
                            this.Pre += text.Substring(0, endIndex);
                            text = text.Remove(0, endIndex);
                        }
                    }

                    // tags like <i> or <font face="Segoe Print" color="#ff0000">
                    endIndex = text.IndexOf('>');
                    if (text.StartsWith('<') && endIndex >= 2)
                    {
                        endIndex++;
                        this.Pre += text.Substring(0, endIndex);
                        text = text.Remove(0, endIndex);
                    }
                }
                while (text.Length < beginLength);
            }

            this.Post = string.Empty;
            if (text.Length > 0 && (">" + stripEndCharacters).Contains(text[text.Length - 1]))
            {
                int beginLength;
                do
                {
                    beginLength = text.Length;

                    while (text.Length > 0 && stripEndCharacters.Contains(text[text.Length - 1]))
                    {
                        this.Post = text[text.Length - 1] + this.Post;
                        text = text.Substring(0, text.Length - 1);
                    }

                    if (text.EndsWith('>'))
                    {
                        // tags </i> </b> </u>
                        if (text.EndsWith("</i>", StringComparison.OrdinalIgnoreCase) || text.EndsWith("</b>", StringComparison.OrdinalIgnoreCase) || text.EndsWith("</u>", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Post = text.Substring(text.Length - 4) + this.Post;
                            text = text.Substring(0, text.Length - 4);
                        }

                        // tag </font>
                        if (text.EndsWith("</font>", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Post = text.Substring(text.Length - 7) + this.Post;
                            text = text.Substring(0, text.Length - 7);
                        }
                    }
                }
                while (text.Length < beginLength);
            }

            this.StrippedText = text;
        }

        /// <summary>
        /// Gets or sets the pre.
        /// </summary>
        public string Pre { get; set; }

        /// <summary>
        /// Gets or sets the post.
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// Gets or sets the stripped text.
        /// </summary>
        public string StrippedText { get; set; }

        /// <summary>
        /// Gets the original text.
        /// </summary>
        public string OriginalText { get; private set; }

        /// <summary>
        /// Gets the merged string.
        /// </summary>
        public string MergedString
        {
            get
            {
                return this.Pre + this.StrippedText + this.Post;
            }
        }

        /// <summary>
        /// The get and insert next id.
        /// </summary>
        /// <param name="replaceIds">
        /// The replace ids.
        /// </param>
        /// <param name="replaceNames">
        /// The replace names.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetAndInsertNextId(List<string> replaceIds, List<string> replaceNames, string name)
        {
            int i = 0;
            string id = string.Format("_@{0}_", i);
            while (replaceIds.Contains(id))
            {
                i++;
                id = string.Format("_@{0}_", i);
            }

            replaceIds.Add(id);
            replaceNames.Add(name);
            return id;
        }

        /// <summary>
        /// The replace names 1 remove.
        /// </summary>
        /// <param name="namesEtc">
        /// The names etc.
        /// </param>
        /// <param name="replaceIds">
        /// The replace ids.
        /// </param>
        /// <param name="replaceNames">
        /// The replace names.
        /// </param>
        /// <param name="originalNames">
        /// The original names.
        /// </param>
        private void ReplaceNames1Remove(List<string> namesEtc, List<string> replaceIds, List<string> replaceNames, List<string> originalNames)
        {
            if (this.Post.StartsWith('.'))
            {
                this.StrippedText += ".";
                this.Post = this.Post.Remove(0, 1);
            }

            string lower = this.StrippedText.ToLower();

            foreach (string name in namesEtc)
            {
                int start = lower.IndexOf(name, StringComparison.OrdinalIgnoreCase);
                while (start >= 0 && start < lower.Length)
                {
                    bool startOk = (start == 0) || (lower[start - 1] == ' ') || (lower[start - 1] == '-') || (lower[start - 1] == '"') || (lower[start - 1] == '\'') || (lower[start - 1] == '>') || Environment.NewLine.EndsWith(lower[start - 1]);

                    if (startOk && string.CompareOrdinal(name, "Don") == 0 && lower.Substring(start).StartsWith("don't"))
                    {
                        startOk = false;
                    }

                    if (startOk)
                    {
                        int end = start + name.Length;
                        bool endOk = end <= lower.Length;
                        if (endOk)
                        {
                            endOk = end == lower.Length || (@" ,.!?:;')- <""" + Environment.NewLine).Contains(lower[end]);
                        }

                        if (endOk && this.StrippedText.Length >= start + name.Length)
                        {
                            string originalName = this.StrippedText.Substring(start, name.Length);
                            originalNames.Add(originalName);
                            this.StrippedText = this.StrippedText.Remove(start, name.Length);
                            this.StrippedText = this.StrippedText.Insert(start, GetAndInsertNextId(replaceIds, replaceNames, name));
                            lower = this.StrippedText.ToLower();
                        }
                    }

                    if (start + 3 > lower.Length)
                    {
                        start = lower.Length + 1;
                    }
                    else
                    {
                        start = lower.IndexOf(name.ToLower(), start + 3, StringComparison.Ordinal);
                    }
                }
            }

            if (this.StrippedText.EndsWith('.'))
            {
                this.Post = "." + this.Post;
                this.StrippedText = this.StrippedText.TrimEnd('.');
            }
        }

        /// <summary>
        /// The replace names 2 fix.
        /// </summary>
        /// <param name="replaceIds">
        /// The replace ids.
        /// </param>
        /// <param name="replaceNames">
        /// The replace names.
        /// </param>
        private void ReplaceNames2Fix(List<string> replaceIds, List<string> replaceNames)
        {
            for (int i = 0; i < replaceIds.Count; i++)
            {
                this.StrippedText = this.StrippedText.Replace(replaceIds[i], replaceNames[i]);
            }
        }

        /// <summary>
        /// The fix casing.
        /// </summary>
        /// <param name="namesEtc">
        /// The names etc.
        /// </param>
        /// <param name="changeNameCases">
        /// The change name cases.
        /// </param>
        /// <param name="makeUppercaseAfterBreak">
        /// The make uppercase after break.
        /// </param>
        /// <param name="checkLastLine">
        /// The check last line.
        /// </param>
        /// <param name="lastLine">
        /// The last line.
        /// </param>
        public void FixCasing(List<string> namesEtc, bool changeNameCases, bool makeUppercaseAfterBreak, bool checkLastLine, string lastLine)
        {
            var replaceIds = new List<string>();
            var replaceNames = new List<string>();
            var originalNames = new List<string>();
            this.ReplaceNames1Remove(namesEtc, replaceIds, replaceNames, originalNames);

            if (checkLastLine)
            {
                string s = HtmlUtil.RemoveHtmlTags(lastLine).TrimEnd().TrimEnd('\"').TrimEnd();

                bool startWithUppercase = string.IsNullOrEmpty(s) || s.EndsWith('.') || s.EndsWith('!') || s.EndsWith('?') || s.EndsWith(". ♪", StringComparison.Ordinal) || s.EndsWith("! ♪", StringComparison.Ordinal) || s.EndsWith("? ♪", StringComparison.Ordinal) || s.EndsWith(']') || s.EndsWith(')') || s.EndsWith(':');

                // start with uppercase after music symbol - but only if next line does not start with music symbol
                if (!startWithUppercase && (s.EndsWith('♪') || s.EndsWith('♫')))
                {
                    if (!this.Pre.Contains(new[] { '♪', '♫' }))
                    {
                        startWithUppercase = true;
                    }
                }

                if (startWithUppercase && this.StrippedText.Length > 0 && !this.Pre.Contains("..."))
                {
                    this.StrippedText = char.ToUpper(this.StrippedText[0]) + this.StrippedText.Substring(1);
                }
            }

            if (makeUppercaseAfterBreak && this.StrippedText.Contains(new[] { '.', '!', '?', ':', ';', ')', ']', '}', '(', '[', '{' }))
            {
                const string breakAfterChars = @".!?:;)]}([{";

                var sb = new StringBuilder();
                bool lastWasBreak = false;
                for (int i = 0; i < this.StrippedText.Length; i++)
                {
                    var s = this.StrippedText[i];
                    if (lastWasBreak)
                    {
                        if (("\"`´'()<>!?.- " + Environment.NewLine).Contains(s))
                        {
                            sb.Append(s);
                        }
                        else if ((sb.EndsWith('<') || sb.ToString().EndsWith("</", StringComparison.Ordinal)) && i + 1 < this.StrippedText.Length && this.StrippedText[i + 1] == '>')
                        { // tags
                            sb.Append(s);
                        }
                        else if (sb.EndsWith('<') && s == '/' && i + 2 < this.StrippedText.Length && this.StrippedText[i + 2] == '>')
                        { // tags
                            sb.Append(s);
                        }
                        else if (sb.ToString().EndsWith("... ", StringComparison.Ordinal))
                        {
                            sb.Append(s);
                            lastWasBreak = false;
                        }
                        else
                        {
                            if (breakAfterChars.Contains(s))
                            {
                                sb.Append(s);
                            }
                            else
                            {
                                lastWasBreak = false;
                                sb.Append(char.ToUpper(s));
                            }
                        }
                    }
                    else
                    {
                        sb.Append(s);
                        if (breakAfterChars.Contains(s))
                        {
                            var idx = sb.ToString().IndexOf('[');
                            if (s == ']' && idx > 1)
                            { // I [Motor roaring] love you!
                                string temp = sb.ToString(0, idx - 1).Trim();
                                if (temp.Length > 0 && !Utilities.LowercaseLetters.Contains(temp[temp.Length - 1]))
                                {
                                    lastWasBreak = true;
                                }
                            }
                            else
                            {
                                lastWasBreak = true;
                            }
                        }
                    }
                }

                this.StrippedText = sb.ToString();
            }

            if (changeNameCases)
            {
                this.ReplaceNames2Fix(replaceIds, replaceNames);
            }
            else
            {
                this.ReplaceNames2Fix(replaceIds, originalNames);
            }
        }
    }
}