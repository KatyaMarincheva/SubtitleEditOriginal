using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nikse.SubtitleEdit.Logic;
using System.Web;

namespace Nikse.SubtitleEdit.Core
{
    using System.Net;

    /// <summary>
    /// HTML specific string manipulations.
    /// </summary>
    internal static class HtmlUtil
    {
        public const string TagItalic = "i";
        public const string TagBold = "b";
        public const string TagUnderline = "u";
        public const string TagParagraph = "p";
        public const string TagFont = "font";
        public const string TagCyrillicI = "\u0456"; // Cyrillic Small Letter Byelorussian-Ukrainian i (http://graphemica.com/%D1%96)

        private static readonly Regex TagOpenRegex = new Regex(@"<\s*(?:/\s*)?(\w+)[^>]*>", RegexOptions.Compiled);

        /// <summary>
        /// Remove all of the specified opening and closing tags from the source HTML string.
        /// </summary>
        /// <param name="source">The source string to search for specified HTML tags.</param>
        /// <param name="tags">The HTML tags to remove.</param>
        /// <returns>A new string without the specified opening and closing tags.</returns>
        public static string RemoveOpenCloseTags(string source, params string[] tags)
        {
            // This pattern matches these tag formats:
            // <tag*>
            // < tag*>
            // </tag*>
            // < /tag*>
            // </ tag*>
            // < / tag*>
            return TagOpenRegex.Replace(
                source,
                m => tags.Contains(m.Groups[1].Value, StringComparer.OrdinalIgnoreCase) ? string.Empty : m.Value);
        }

        /// <summary>
        /// Converts a string to an HTML-encoded string using named character references.
        /// </summary>
        /// <param name="source">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string EncodeText(string source)
        {
            if (source == null)
                return string.Empty;

            string[] inputChars = source.ToCharArray().Select(ch => ch.ToString()).ToArray();

            var encoded = new StringBuilder(source.Length);

            for (int i = 0; i < source.Length; i++)
            {
                int aciiCode = source[i];

                if ((aciiCode >= 128 && aciiCode <= 160) || aciiCode > 255)
                {
                    encoded.Append("&#" + aciiCode + ";");
                }
                else
                {
                    encoded.Append(WebUtility.HtmlEncode(inputChars[i]));
                }
            }

            return encoded.ToString();
        }

        public static string RemoveHtmlTags(string s, bool alsoSsaTags = false)
        {
            if (s == null || s.Length < 3)
                return s;

            if (alsoSsaTags)
                s = Utilities.RemoveSsaTags(s);

            if (s.IndexOf('<') < 0)
                return s;

            if (s.IndexOf("< ", StringComparison.Ordinal) >= 0)
                s = FixInvalidItalicTags(s);

            return RemoveOpenCloseTags(s, TagItalic, TagBold, TagUnderline, TagParagraph, TagFont, TagCyrillicI);
        }

        public static bool IsUrl(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 6 || !text.Contains('.') || text.Contains(' '))
                return false;

            var allLower = text.ToLower();
            if (allLower.StartsWith("http://", StringComparison.Ordinal) || allLower.StartsWith("https://", StringComparison.Ordinal) ||
                allLower.StartsWith("www.", StringComparison.Ordinal) || allLower.EndsWith(".org", StringComparison.Ordinal) ||
                allLower.EndsWith(".com", StringComparison.Ordinal) || allLower.EndsWith(".net", StringComparison.Ordinal))
                return true;

            if (allLower.Contains(".org/") || allLower.Contains(".com/") || allLower.Contains(".net/"))
                return true;

            return false;
        }

        public static bool StartsWithUrl(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var arr = text.Trim().TrimEnd('.').TrimEnd().Split();
            if (arr.Length == 0)
                return false;

            return IsUrl(arr[0]);
        }

        internal static string FixUpperTags(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var tags = new string[] { "<I>", "<U>", "<B>", "<FONT", "</I>", "</U>", "</B>", "</FONT>" };
            var idx = text.IndexOfAny(tags, StringComparison.Ordinal);
            while (idx >= 0)
            {
                var endIdx = text.IndexOf('>', idx + 2);
                if (endIdx < idx)
                    break;
                var tag = text.Substring(idx, endIdx - idx).ToLowerInvariant();
                text = text.Remove(idx, endIdx - idx).Insert(idx, tag);
                idx = text.IndexOfAny(tags, StringComparison.Ordinal);
            }
            return text;
        }

        public static string FixInvalidItalicTags(string text)
        {
            const string beginTag = "<i>";
            const string endTag = "</i>";

            text = text.Replace("< i >", beginTag);
            text = text.Replace("< i>", beginTag);
            text = text.Replace("<i >", beginTag);
            text = text.Replace("< I>", beginTag);
            text = text.Replace("<I >", beginTag);

            text = text.Replace("< / i >", endTag);
            text = text.Replace("< /i>", endTag);
            text = text.Replace("</ i>", endTag);
            text = text.Replace("< /i>", endTag);
            text = text.Replace("< /i >", endTag);
            text = text.Replace("</i >", endTag);
            text = text.Replace("</ i >", endTag);
            text = text.Replace("< / i>", endTag);
            text = text.Replace("< /I>", endTag);
            text = text.Replace("</ I>", endTag);
            text = text.Replace("< /I>", endTag);
            text = text.Replace("< / I >", endTag);

            text = text.Replace("</i> <i>", "_@_");
            text = text.Replace(" _@_", "_@_");
            text = text.Replace(" _@_ ", "_@_");
            text = text.Replace("_@_", " ");

            if (text.Contains(beginTag))
                text = text.Replace("<i/>", endTag);
            else
                text = text.Replace("<i/>", string.Empty);

            text = text.Replace(beginTag + beginTag, beginTag);
            text = text.Replace(endTag + endTag, endTag);

            int italicBeginTagCount = Utilities.CountTagInText(text, beginTag);
            int italicEndTagCount = Utilities.CountTagInText(text, endTag);
            int noOfLines = Utilities.GetNumberOfLines(text);
            if (italicBeginTagCount + italicEndTagCount > 0)
            {
                if (italicBeginTagCount == 1 && italicEndTagCount == 1 && text.IndexOf(beginTag, StringComparison.Ordinal) > text.IndexOf(endTag, StringComparison.Ordinal))
                {
                    text = text.Replace(beginTag, "___________@");
                    text = text.Replace(endTag, beginTag);
                    text = text.Replace("___________@", endTag);
                }

                if (italicBeginTagCount == 2 && italicEndTagCount == 0)
                {
                    int firstIndex = text.IndexOf(beginTag, StringComparison.Ordinal);
                    int lastIndex = text.LastIndexOf(beginTag, StringComparison.Ordinal);
                    int lastIndexWithNewLine = text.LastIndexOf(Environment.NewLine + beginTag, StringComparison.Ordinal) + Environment.NewLine.Length;
                    if (noOfLines == 2 && lastIndex == lastIndexWithNewLine && firstIndex < 2)
                        text = text.Replace(Environment.NewLine, "</i>" + Environment.NewLine) + "</i>";
                    else if (text.Length > lastIndex + endTag.Length)
                        text = text.Substring(0, lastIndex) + endTag + text.Substring(lastIndex - 1 + endTag.Length);
                    else
                        text = text.Substring(0, lastIndex) + endTag;
                }

                if (italicBeginTagCount == 1 && italicEndTagCount == 2)
                {
                    int firstIndex = text.IndexOf(endTag, StringComparison.Ordinal);
                    if (text.StartsWith("</i>-<i>-", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (text.StartsWith("</i>- <i>-", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (text.StartsWith("</i>- <i> -", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (text.StartsWith("</i>-<i> -", StringComparison.Ordinal))
                        text = text.Remove(0, 5);
                    else if (firstIndex == 0)
                        text = text.Remove(0, 4);
                    else
                        text = text.Substring(0, firstIndex) + text.Substring(firstIndex + endTag.Length);
                }

                if (italicBeginTagCount == 2 && italicEndTagCount == 1)
                {
                    var lines = text.SplitToLines();
                    if (lines.Length == 2 && lines[0].StartsWith("<i>", StringComparison.Ordinal) && lines[0].EndsWith("</i>", StringComparison.Ordinal) &&
                        lines[1].StartsWith("<i>", StringComparison.Ordinal))
                    {
                        text = text.TrimEnd() + "</i>";
                    }
                    else
                    {
                        int lastIndex = text.LastIndexOf(beginTag, StringComparison.Ordinal);
                        if (text.Length > lastIndex + endTag.Length)
                            text = text.Substring(0, lastIndex) + text.Substring(lastIndex - 1 + endTag.Length);
                        else
                            text = text.Substring(0, lastIndex - 1) + endTag;
                    }
                    if (text.StartsWith("<i>", StringComparison.Ordinal) && text.EndsWith("</i>", StringComparison.Ordinal) && text.Contains("</i>" + Environment.NewLine + "<i>"))
                    {
                        text = text.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine);
                    }
                }

                if (italicBeginTagCount == 1 && italicEndTagCount == 0)
                {
                    int lastIndexWithNewLine = text.LastIndexOf(Environment.NewLine + beginTag, StringComparison.Ordinal) + Environment.NewLine.Length;
                    int lastIndex = text.LastIndexOf(beginTag, StringComparison.Ordinal);

                    if (text.StartsWith(beginTag, StringComparison.Ordinal))
                        text += endTag;
                    else if (noOfLines == 2 && lastIndex == lastIndexWithNewLine)
                        text += endTag;
                    else
                        text = text.Replace(beginTag, string.Empty);
                }

                if (italicBeginTagCount == 0 && italicEndTagCount == 1)
                {
                    var cleanText = HtmlUtil.RemoveOpenCloseTags(text, HtmlUtil.TagItalic, HtmlUtil.TagBold, HtmlUtil.TagUnderline, HtmlUtil.TagCyrillicI);
                    bool isFixed = false;

                    // Foo.</i>
                    if (text.EndsWith(endTag, StringComparison.Ordinal) && !cleanText.StartsWith('-') && !cleanText.Contains(Environment.NewLine + "-"))
                    {
                        text = beginTag + text;
                        isFixed = true;
                    }

                    // - Foo</i> | - Foo.
                    // - Bar.    | - Foo.</i>
                    if (!isFixed && Utilities.GetNumberOfLines(cleanText) == 2)
                    {
                        int newLineIndex = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                        if (newLineIndex > 0)
                        {
                            var firstLine = text.Substring(0, newLineIndex).Trim();
                            var secondLine = text.Substring(newLineIndex + 2).Trim();
                            if (firstLine.EndsWith(endTag, StringComparison.Ordinal))
                            {
                                firstLine = beginTag + firstLine;
                                isFixed = true;
                            }
                            if (secondLine.EndsWith(endTag, StringComparison.Ordinal))
                            {
                                secondLine = beginTag + secondLine;
                                isFixed = true;
                            }
                            text = firstLine + Environment.NewLine + secondLine;
                        }
                    }
                    if (!isFixed)
                        text = text.Replace(endTag, string.Empty);
                }

                // - foo.</i>
                // - bar.</i>
                if (italicBeginTagCount == 0 && italicEndTagCount == 2 && text.Contains(endTag + Environment.NewLine, StringComparison.Ordinal) && text.EndsWith(endTag, StringComparison.Ordinal))
                {
                    text = text.Replace(endTag, string.Empty);
                    text = beginTag + text + endTag;
                }

                if (italicBeginTagCount == 0 && italicEndTagCount == 2 && text.StartsWith("</i>", StringComparison.Ordinal) && text.EndsWith("</i>", StringComparison.Ordinal))
                {
                    int firstIndex = text.IndexOf(endTag, StringComparison.Ordinal);
                    text = text.Remove(firstIndex, endTag.Length).Insert(firstIndex, "<i>");
                }

                // <i>Foo</i>
                // <i>Bar</i>
                if (italicBeginTagCount == 2 && italicEndTagCount == 2 && Utilities.GetNumberOfLines(text) == 2)
                {
                    int index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    if (index > 0 && text.Length > index + (beginTag.Length + endTag.Length))
                    {
                        var firstLine = text.Substring(0, index).Trim();
                        var secondLine = text.Substring(index + 2).Trim();

                        if (firstLine.Length > 10 && firstLine.StartsWith("- <i>", StringComparison.Ordinal) && firstLine.EndsWith(endTag, StringComparison.Ordinal))
                        {
                            text = "<i>- " + firstLine.Remove(0, 5) + Environment.NewLine + secondLine;
                            text = text.Replace("<i>-  ", "<i>- ");
                            index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                            firstLine = text.Substring(0, index).Trim();
                            secondLine = text.Substring(index + 2).Trim();
                        }
                        if (secondLine.Length > 10 && secondLine.StartsWith("- <i>", StringComparison.Ordinal) && secondLine.EndsWith(endTag, StringComparison.Ordinal))
                        {
                            text = firstLine + Environment.NewLine + "<i>- " + secondLine.Remove(0, 5);
                            text = text.Replace("<i>-  ", "<i>- ");
                            index = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                            firstLine = text.Substring(0, index).Trim();
                            secondLine = text.Substring(index + 2).Trim();
                        }

                        if (Utilities.StartsAndEndsWithTag(firstLine, beginTag, endTag) && Utilities.StartsAndEndsWithTag(secondLine, beginTag, endTag))
                        {
                            text = text.Replace(beginTag, String.Empty).Replace(endTag, String.Empty).Trim();
                            text = beginTag + text + endTag;
                        }
                    }

                    //FALCONE:<i> I didn't think</i><br /><i>it was going to be you,</i>
                    var colIdx = text.IndexOf(':');
                    if (colIdx > -1 && Utilities.CountTagInText(text, "<i>") + Utilities.CountTagInText(text, "</i>") == 4 && text.Length > colIdx + 1 && !char.IsDigit(text[colIdx + 1]))
                    {
                        var firstLine = text.Substring(0, index);
                        var secondLine = text.Substring(index).TrimStart();

                        var secIdxCol = secondLine.IndexOf(':');
                        if (secIdxCol < 0 || !Utilities.IsBetweenNumbers(secondLine, secIdxCol))
                        {
                            var idx = firstLine.IndexOf(':');
                            if (idx > 1)
                            {
                                var pre = text.Substring(0, idx + 1).TrimStart();
                                text = text.Remove(0, idx + 1);
                                text = FixInvalidItalicTags(text).Trim();
                                if (text.StartsWith("<i> ", StringComparison.OrdinalIgnoreCase))
                                    text = Utilities.RemoveSpaceBeforeAfterTag(text, "<i>");
                                text = pre + " " + text;
                            }
                        }
                    }
                }

                //<i>- You think they're they gone?<i>
                //<i>- That can't be.</i>
                if ((italicBeginTagCount == 3 && italicEndTagCount == 1) && Utilities.GetNumberOfLines(text) == 2)
                {
                    var newLineIdx = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
                    var firstLine = text.Substring(0, newLineIdx).Trim();
                    var secondLine = text.Substring(newLineIdx).Trim();

                    if ((Utilities.StartsAndEndsWithTag(firstLine, beginTag, beginTag) && Utilities.StartsAndEndsWithTag(secondLine, beginTag, endTag)) ||
                        (Utilities.StartsAndEndsWithTag(secondLine, beginTag, beginTag) && Utilities.StartsAndEndsWithTag(firstLine, beginTag, endTag)))
                    {
                        text = text.Replace("<i>", string.Empty);
                        text = text.Replace("</i>", string.Empty);
                        text = text.Replace("  ", " ").Trim();
                        text = "<i>" + text + "</i>";
                    }
                }
                text = text.Replace("<i></i>", string.Empty);
                text = text.Replace("<i> </i>", string.Empty);
                text = text.Replace("<i>  </i>", string.Empty);
            }
            return text;
        }

    }
}