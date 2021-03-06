﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sami.cs" company="">
//   
// </copyright>
// <summary>
//   The sami.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The sami.
    /// </summary>
    public class Sami : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".smi";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SAMI";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is time based.
        /// </summary>
        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has style support.
        /// </summary>
        public override bool HasStyleSupport
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The get styles from header.
        /// </summary>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<string> GetStylesFromHeader(string header)
        {
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(header) && header.StartsWith("<style", StringComparison.OrdinalIgnoreCase))
            {
                foreach (string line in header.SplitToLines())
                {
                    string s = line.Trim();
                    if (s.StartsWith('.') && s.IndexOf(' ') > 2)
                    {
                        string name = s.Substring(1, s.IndexOf(' ') - 1);
                        list.Add(name);
                    }
                }
            }
            else
            {
                list.Add("ENUSCC");
            }

            return list;
        }

        /// <summary>
        /// The is mine.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsMine(List<string> lines, string fileName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string l in lines)
            {
                sb.AppendLine(l);
            }

            if (sb.ToString().Contains("</SYNC>"))
            {
                return false;
            }

            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > this._errorCount;
        }

        /// <summary>
        /// The to text.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToText(Subtitle subtitle, string title)
        {
            string language = Utilities.AutoDetectLanguageName("en_US", subtitle);
            CultureInfo ci = CultureInfo.GetCultureInfo(language.Replace("_", "-"));
            string languageTag = string.Format("{0}CC", language.Replace("_", string.Empty).ToUpper());
            string languageName = ci.EnglishName;
            if (ci.Parent != null)
            {
                languageName = ci.Parent.EnglishName;
            }

            string languageStyle = string.Format(".{0} [ name: {1}; lang: {2} ; SAMIType: CC ; ]", languageTag, languageName, language.Replace("_", "-"));
            languageStyle = languageStyle.Replace("[", "{").Replace("]", "}");

            string header = @"<SAMI>
                                <HEAD>
                                <TITLE>_TITLE_</TITLE>
                                <SAMIParam>
                                  Metrics {time:ms;}
                                  Spec {MSFT:1.0;}
                                </SAMIParam>
                                <STYLE TYPE=""text/css"">
                                <!--
                                  P { font-family: Arial; font-weight: normal; color: white; background-color: black; text-align: center; }
                                  _LANGUAGE-STYLE_
                                -->
                                </STYLE>
                                </HEAD>
                                <BODY>
                                <-- Open play menu, choose Captions and Subtiles, On if available -->
                                <-- Open tools menu, Security, Show local captions when present -->";

            bool useExtra = false;
            if (!string.IsNullOrEmpty(subtitle.Header) && subtitle.Header.StartsWith("<style", StringComparison.OrdinalIgnoreCase))
            {
                useExtra = true;
                header = @"<SAMI>
                            <HEAD>
                            <TITLE>_TITLE_</TITLE>
                            <SAMIParam>
                              Metrics {time:ms;}
                              Spec {MSFT:1.0;}
                            </SAMIParam>
                            " + subtitle.Header.Trim() + @"
                            </HEAD>
                            <BODY>
                            <-- Open play menu, choose Captions and Subtiles, On if available -->
                            <-- Open tools menu, Security, Show local captions when present -->";
            }

            // Example text (start numbers are milliseconds)
            // <SYNC Start=65264><P>Let's go!
            // <SYNC Start=66697><P><BR>
            string paragraphWriteFormat = @"<SYNC Start={0}><P Class={3}>{2}" + Environment.NewLine + @"<SYNC Start={1}><P Class={3}>&nbsp;";
            string paragraphWriteFormatOpen = @"<SYNC Start={0}><P Class={2}>{1}";
            if (this.Name == new SamiModern().Name)
            {
                paragraphWriteFormat = "<SYNC Start=\"{0}\"><P Class=\"{3}\">{2}</P></SYNC>" + Environment.NewLine + "<SYNC Start=\"{1}\"><P Class=\"{3}\">&nbsp;</P></SYNC>";
                paragraphWriteFormatOpen = "<SYNC Start=\"{0}\"><P Class=\"{2}\">{1}</P></SYNC>";
            }
            else if (this.Name == new SamiYouTube().Name)
            {
                paragraphWriteFormat = "<SYNC Start=\"{0}\"><P Class=\"{3}\">{2}</P></SYNC>" + Environment.NewLine + "<SYNC Start=\"{1}\"><P Class=\"{3}\"></P></SYNC>";
                paragraphWriteFormatOpen = "<SYNC Start=\"{0}\"><P Class=\"{2}\">{1}</P></SYNC>";
            }

            int count = 1;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header.Replace("_TITLE_", title).Replace("_LANGUAGE-STYLE_", languageStyle));
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                Paragraph next = subtitle.GetParagraphOrDefault(count);
                string text = p.Text;

                if (text.Contains('<') && text.Contains('>'))
                {
                    StringBuilder total = new StringBuilder();
                    StringBuilder partial = new StringBuilder();
                    bool tagOn = false;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text.Substring(i).StartsWith("<font") || text.Substring(i).StartsWith("<div") || text.Substring(i).StartsWith("<i") || text.Substring(i).StartsWith("<b") || text.Substring(i).StartsWith("<s") || text.Substring(i).StartsWith("</"))
                        {
                            total.Append(HtmlUtil.EncodeText(partial.ToString()));
                            partial = new StringBuilder();
                            tagOn = true;
                            total.Append('<');
                        }
                        else if (text.Substring(i).StartsWith('>') && tagOn)
                        {
                            tagOn = false;
                            total.Append('>');
                        }
                        else if (!tagOn)
                        {
                            partial.Append(text[i]);
                        }
                        else
                        {
                            total.Append(text[i]);
                        }
                    }

                    total.Append(HtmlUtil.EncodeText(partial.ToString()));
                    text = total.ToString();
                }
                else
                {
                    text = HtmlUtil.EncodeText(text);
                }

                if (this.Name == new SamiModern().Name)
                {
                    text = text.Replace(Environment.NewLine, "<br />");
                }
                else
                {
                    text = text.Replace(Environment.NewLine, "<br>");
                }

                string currentClass = languageTag;
                if (useExtra && !string.IsNullOrEmpty(p.Extra))
                {
                    currentClass = p.Extra;
                }

                if (next != null && Math.Abs(next.StartTime.TotalMilliseconds - p.EndTime.TotalMilliseconds) < 1)
                {
                    sb.AppendLine(string.Format(paragraphWriteFormatOpen, p.StartTime.TotalMilliseconds, text, currentClass));
                }
                else
                {
                    sb.AppendLine(string.Format(paragraphWriteFormat, p.StartTime.TotalMilliseconds, p.EndTime.TotalMilliseconds, text, currentClass));
                }

                count++;
            }

            sb.AppendLine("</BODY>");
            sb.AppendLine("</SAMI>");
            return sb.ToString().Trim();
        }

        /// <summary>
        /// The load subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;
            StringBuilder sb = new StringBuilder();
            foreach (string l in lines)
            {
                sb.AppendLine(l.Replace("<SYNC Start= \"", "<SYNC Start=\"").Replace("<SYNC Start = \"", "<SYNC Start=\"").Replace("<SYNC Start =\"", "<SYNC Start=\"").Replace("<SYNC  Start=\"", "<SYNC Start=\""));
            }

            string allInput = sb.ToString();
            string allInputLower = allInput.ToLower();
            if (!allInputLower.Contains("<sync "))
            {
                return;
            }

            int styleStart = allInputLower.IndexOf("<style", StringComparison.Ordinal);
            if (styleStart > 0)
            {
                int styleEnd = allInputLower.IndexOf("</style>", StringComparison.Ordinal);
                if (styleEnd > 0)
                {
                    subtitle.Header = allInput.Substring(styleStart, styleEnd - styleStart + 8);
                }
            }

            const string syncTag = "<sync start=";
            const string syncTagEnc = "<sync encrypted=\"true\" start=";
            int syncStartPos = allInputLower.IndexOf(syncTag, StringComparison.Ordinal);
            int index = syncStartPos + syncTag.Length;

            int syncStartPosEnc = allInputLower.IndexOf(syncTagEnc, StringComparison.Ordinal);
            if ((syncStartPosEnc >= 0 && syncStartPosEnc < syncStartPos) || syncStartPos == -1)
            {
                syncStartPos = syncStartPosEnc;
                index = syncStartPosEnc + syncTagEnc.Length;
            }

            Paragraph p = new Paragraph();
            while (syncStartPos >= 0)
            {
                string millisecAsString = string.Empty;
                while (index < allInput.Length && @"""'0123456789".Contains(allInput[index]))
                {
                    if (allInput[index] != '"' && allInput[index] != '\'')
                    {
                        millisecAsString += allInput[index];
                    }

                    index++;
                }

                while (index < allInput.Length && allInput[index] != '>')
                {
                    index++;
                }

                if (index < allInput.Length && allInput[index] == '>')
                {
                    index++;
                }

                int syncEndPos = allInputLower.IndexOf(syncTag, index, StringComparison.Ordinal);
                int syncEndPosEnc = allInputLower.IndexOf(syncTagEnc, index, StringComparison.Ordinal);
                if ((syncStartPosEnc >= 0 && syncStartPosEnc < syncStartPos) || syncEndPos == -1)
                {
                    syncEndPos = syncEndPosEnc;
                }

                string text;
                if (syncEndPos >= 0)
                {
                    text = allInput.Substring(index, syncEndPos - index);
                }
                else
                {
                    text = allInput.Substring(index);
                }

                string textToLower = text.ToLower();
                if (textToLower.Contains(" class="))
                {
                    StringBuilder className = new StringBuilder();
                    int startClass = textToLower.IndexOf(" class=", StringComparison.Ordinal);
                    int indexClass = startClass + 7;
                    while (indexClass < textToLower.Length && (Utilities.LowercaseLettersWithNumbers + @"'""").Contains(textToLower[indexClass]))
                    {
                        className.Append(text[indexClass]);
                        indexClass++;
                    }

                    p.Extra = className.ToString().Trim(' ', '\'', '"');
                }

                if (text.Contains("ID=\"Source\"") || text.Contains("ID=Source"))
                {
                    int sourceIndex = text.IndexOf("ID=\"Source\"", StringComparison.Ordinal);
                    if (sourceIndex < 0)
                    {
                        sourceIndex = text.IndexOf("ID=Source", StringComparison.Ordinal);
                    }

                    int st = sourceIndex - 1;
                    while (st > 0 && text.Substring(st, 2).ToUpper() != "<P")
                    {
                        st--;
                    }

                    if (st > 0)
                    {
                        text = text.Substring(0, st) + text.Substring(sourceIndex);
                    }

                    int et = st;
                    while (et < text.Length - 5 && text.Substring(et, 3).ToUpper() != "<P>" && text.Substring(et, 4).ToUpper() != "</P>")
                    {
                        et++;
                    }

                    text = text.Substring(0, st) + text.Substring(et);
                }

                text = text.Replace(Environment.NewLine, " ");
                text = text.Replace("  ", " ");

                text = text.TrimEnd();
                text = Regex.Replace(text, @"<br {0,2}/?>", Environment.NewLine, RegexOptions.IgnoreCase);

                while (text.Contains("  "))
                {
                    text = text.Replace("  ", " ");
                }

                text = text.Replace("</BODY>", string.Empty).Replace("</SAMI>", string.Empty).TrimEnd();

                int endSyncPos = text.ToUpper().IndexOf("</SYNC>", StringComparison.Ordinal);
                if (text.IndexOf('>') > 0 && (text.IndexOf('>') < endSyncPos || endSyncPos == -1))
                {
                    text = text.Remove(0, text.IndexOf('>') + 1);
                }

                text = text.TrimEnd();

                if (text.EndsWith("</sync>", StringComparison.OrdinalIgnoreCase))
                {
                    text = text.Substring(0, text.Length - 7).TrimEnd();
                }

                if (text.EndsWith("</p>", StringComparison.Ordinal) || text.EndsWith("</P>", StringComparison.Ordinal))
                {
                    text = text.Substring(0, text.Length - 4).TrimEnd();
                }

                text = text.Replace("&nbsp;", " ").Replace("&NBSP;", " ");

                if (text.Contains("<font color=") && !text.Contains("</font>"))
                {
                    text += "</font>";
                }

                if (text.StartsWith("<FONT COLOR=") && !text.Contains("</font>") && !text.Contains("</FONT>"))
                {
                    text += "</FONT>";
                }

                if (text.Contains('<') && text.Contains('>'))
                {
                    StringBuilder total = new StringBuilder();
                    StringBuilder partial = new StringBuilder();
                    bool tagOn = false;
                    for (int i = 0; i < text.Length && i < 999; i++)
                    {
                        string tmp = text.Substring(i);
                        if (tmp.StartsWith('<') && (tmp.StartsWith("<font", StringComparison.Ordinal) || tmp.StartsWith("<div", StringComparison.Ordinal) || tmp.StartsWith("<i", StringComparison.Ordinal) || tmp.StartsWith("<b", StringComparison.Ordinal) || tmp.StartsWith("<s", StringComparison.Ordinal) || tmp.StartsWith("</", StringComparison.Ordinal)))
                        {
                            total.Append(WebUtility.HtmlDecode(partial.ToString()));
                            partial = new StringBuilder();
                            tagOn = true;
                            total.Append('<');
                        }
                        else if (text.Substring(i).StartsWith('>') && tagOn)
                        {
                            tagOn = false;
                            total.Append('>');
                        }
                        else if (!tagOn)
                        {
                            partial.Append(text[i]);
                        }
                        else
                        {
                            total.Append(text[i]);
                        }
                    }

                    total.Append(WebUtility.HtmlDecode(partial.ToString()));
                    text = total.ToString();
                }
                else
                {
                    text = WebUtility.HtmlDecode(text);
                }

                string cleanText = text;
                while (cleanText.Contains("  "))
                {
                    cleanText = cleanText.Replace("  ", " ");
                }

                while (cleanText.Contains(Environment.NewLine + " "))
                {
                    cleanText = cleanText.Replace(Environment.NewLine + " ", Environment.NewLine);
                }

                while (cleanText.Contains(" " + Environment.NewLine))
                {
                    cleanText = cleanText.Replace(" " + Environment.NewLine, Environment.NewLine);
                }

                cleanText = cleanText.Trim();

                if (!string.IsNullOrEmpty(p.Text) && !string.IsNullOrEmpty(millisecAsString))
                {
                    p.EndTime = new TimeCode(long.Parse(millisecAsString));
                    subtitle.Paragraphs.Add(p);
                    p = new Paragraph();
                }

                p.Text = cleanText;
                long l;
                if (long.TryParse(millisecAsString, out l))
                {
                    p.StartTime = new TimeCode(l);
                }

                if (syncEndPos <= 0)
                {
                    syncStartPos = -1;
                }
                else
                {
                    syncStartPos = allInputLower.IndexOf(syncTag, syncEndPos, StringComparison.Ordinal);
                    index = syncStartPos + syncTag.Length;

                    syncStartPosEnc = allInputLower.IndexOf(syncTagEnc, syncEndPos, StringComparison.Ordinal);
                    if ((syncStartPosEnc >= 0 && syncStartPosEnc < syncStartPos) || syncStartPos == -1)
                    {
                        syncStartPos = syncStartPosEnc;
                        index = syncStartPosEnc + syncTagEnc.Length;
                    }
                }
            }

            if (!string.IsNullOrEmpty(p.Text) && !subtitle.Paragraphs.Contains(p))
            {
                p.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + Utilities.GetOptimalDisplayMilliseconds(p.Text);
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();

            if (subtitle.Paragraphs.Count > 0 && (subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].Text.ToUpper().Trim() == "</BODY>" || subtitle.Paragraphs[subtitle.Paragraphs.Count - 1].Text.ToUpper().Trim() == "<BODY>"))
            {
                subtitle.Paragraphs.RemoveAt(subtitle.Paragraphs.Count - 1);
            }

            foreach (Paragraph p2 in subtitle.Paragraphs)
            {
                p2.Text = p2.Text.Replace(Convert.ToChar(160), ' '); // non-breaking space to normal space
            }
        }
    }
}