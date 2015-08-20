// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SplitLongLinesHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The split long lines helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Forms
{
    using System;
    using System.Collections.Generic;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The split long lines helper.
    /// </summary>
    public static class SplitLongLinesHelper
    {
        /// <summary>
        /// The qualifies for split.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="singleLineMaxCharacters">
        /// The single line max characters.
        /// </param>
        /// <param name="totalLineMaxCharacters">
        /// The total line max characters.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool QualifiesForSplit(string text, int singleLineMaxCharacters, int totalLineMaxCharacters)
        {
            string s = HtmlUtil.RemoveHtmlTags(text.Trim(), true);
            if (s.Length > totalLineMaxCharacters)
            {
                return true;
            }

            string[] arr = s.SplitToLines();
            foreach (string line in arr)
            {
                if (line.Length > singleLineMaxCharacters)
                {
                    return true;
                }
            }

            string tempText = Utilities.UnbreakLine(text);
            if (Utilities.CountTagInText(tempText, '-') == 2 && (text.StartsWith('-') || text.StartsWith("<i>-")))
            {
                int idx = tempText.IndexOfAny(new[] { ". -", "! -", "? -" }, StringComparison.Ordinal);
                if (idx > 1)
                {
                    idx++;
                    string dialogText = tempText.Remove(idx, 1).Insert(idx, Environment.NewLine);
                    foreach (string line in dialogText.SplitToLines())
                    {
                        if (line.Length > singleLineMaxCharacters)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The split long lines in subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="totalLineMaxCharacters">
        /// The total line max characters.
        /// </param>
        /// <param name="singleLineMaxCharacters">
        /// The single line max characters.
        /// </param>
        /// <returns>
        /// The <see cref="Subtitle"/>.
        /// </returns>
        public static Subtitle SplitLongLinesInSubtitle(Subtitle subtitle, int totalLineMaxCharacters, int singleLineMaxCharacters)
        {
            List<int> splittedIndexes = new List<int>();
            List<int> autoBreakedIndexes = new List<int>();
            Subtitle splittedSubtitle = new Subtitle();
            string language = Utilities.AutoDetectGoogleLanguage(subtitle);
            for (int i = 0; i < subtitle.Paragraphs.Count; i++)
            {
                bool added = false;
                Paragraph p = subtitle.GetParagraphOrDefault(i);
                if (p != null && p.Text != null)
                {
                    if (QualifiesForSplit(p.Text, singleLineMaxCharacters, totalLineMaxCharacters))
                    {
                        string text = Utilities.AutoBreakLine(p.Text, language);
                        if (!QualifiesForSplit(text, singleLineMaxCharacters, totalLineMaxCharacters))
                        {
                            Paragraph newParagraph = new Paragraph(p) { Text = text };
                            autoBreakedIndexes.Add(splittedSubtitle.Paragraphs.Count);
                            splittedSubtitle.Paragraphs.Add(newParagraph);
                            added = true;
                        }
                        else
                        {
                            if (text.Contains(Environment.NewLine))
                            {
                                string[] arr = text.SplitToLines();
                                if (arr.Length == 2)
                                {
                                    int minMsBtwnLnBy2 = Configuration.Settings.General.MinimumMillisecondsBetweenLines / 2;
                                    int spacing1 = minMsBtwnLnBy2;
                                    int spacing2 = minMsBtwnLnBy2;
                                    if (Configuration.Settings.General.MinimumMillisecondsBetweenLines % 2 == 1)
                                    {
                                        spacing2++;
                                    }

                                    double duration = p.Duration.TotalMilliseconds / 2.0;
                                    Paragraph newParagraph1 = new Paragraph(p);
                                    Paragraph newParagraph2 = new Paragraph(p);
                                    newParagraph1.Text = Utilities.AutoBreakLine(arr[0], language);
                                    newParagraph1.EndTime.TotalMilliseconds = p.StartTime.TotalMilliseconds + duration - spacing1;
                                    newParagraph2.Text = Utilities.AutoBreakLine(arr[1], language);
                                    newParagraph2.StartTime.TotalMilliseconds = newParagraph1.EndTime.TotalMilliseconds + spacing2;

                                    splittedIndexes.Add(splittedSubtitle.Paragraphs.Count);
                                    splittedIndexes.Add(splittedSubtitle.Paragraphs.Count + 1);

                                    string p1 = HtmlUtil.RemoveHtmlTags(newParagraph1.Text);
                                    int len = p1.Length - 1;
                                    if (p1.Length > 0 && (p1[len] == '.' || p1[len] == '!' || p1[len] == '?' || p1[len] == ':' || p1[len] == ')' || p1[len] == ']' || p1[len] == '♪'))
                                    {
                                        if (newParagraph1.Text.StartsWith('-') && newParagraph2.Text.StartsWith('-'))
                                        {
                                            newParagraph1.Text = newParagraph1.Text.Remove(0, 1).Trim();
                                            newParagraph2.Text = newParagraph2.Text.Remove(0, 1).Trim();
                                        }
                                        else if (newParagraph1.Text.StartsWith("<i>-", StringComparison.Ordinal) && newParagraph2.Text.StartsWith('-'))
                                        {
                                            newParagraph1.Text = newParagraph1.Text.Remove(3, 1).Trim();
                                            if (newParagraph1.Text.StartsWith("<i> ", StringComparison.Ordinal))
                                            {
                                                newParagraph1.Text = newParagraph1.Text.Remove(3, 1).Trim();
                                            }

                                            newParagraph2.Text = newParagraph2.Text.Remove(0, 1).Trim();
                                        }
                                    }
                                    else
                                    {
                                        if (newParagraph1.Text.EndsWith("</i>", StringComparison.Ordinal))
                                        {
                                            const string post = "</i>";
                                            newParagraph1.Text = newParagraph1.Text.Remove(newParagraph1.Text.Length - post.Length);
                                        }

                                        // newParagraph1.Text += comboBoxLineContinuationEnd.Text.TrimEnd() + post;
                                        if (newParagraph2.Text.StartsWith("<i>", StringComparison.Ordinal))
                                        {
                                            const string pre = "<i>";
                                            newParagraph2.Text = newParagraph2.Text.Remove(0, pre.Length);
                                        }

                                        // newParagraph2.Text = pre + comboBoxLineContinuationBegin.Text + newParagraph2.Text;
                                    }

                                    int indexOfItalicOpen1 = newParagraph1.Text.IndexOf("<i>", StringComparison.Ordinal);
                                    if (indexOfItalicOpen1 >= 0 && indexOfItalicOpen1 < 10 && newParagraph1.Text.IndexOf("</i>", StringComparison.Ordinal) < 0 && newParagraph2.Text.Contains("</i>") && newParagraph2.Text.IndexOf("<i>", StringComparison.Ordinal) < 0)
                                    {
                                        newParagraph1.Text += "</i>";
                                        newParagraph2.Text = "<i>" + newParagraph2.Text;
                                    }

                                    splittedSubtitle.Paragraphs.Add(newParagraph1);
                                    splittedSubtitle.Paragraphs.Add(newParagraph2);
                                    added = true;
                                }
                            }
                        }
                    }
                }

                if (!added)
                {
                    splittedSubtitle.Paragraphs.Add(new Paragraph(p));
                }
            }

            splittedSubtitle.Renumber();
            return splittedSubtitle;
        }
    }
}