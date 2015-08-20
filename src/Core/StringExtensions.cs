// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The string extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Core
{
    using System;
    using System.Text;

    /// <summary>
    /// The string extensions.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// The line starts with html tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threeLengthTag">
        /// The three length tag.
        /// </param>
        /// <param name="includeFont">
        /// The include font.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool LineStartsWithHtmlTag(this string text, bool threeLengthTag, bool includeFont = false)
        {
            if (text == null || (!threeLengthTag && !includeFont))
            {
                return false;
            }

            return StartsWithHtmlTag(text, threeLengthTag, includeFont);
        }

        /// <summary>
        /// The line ends with html tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threeLengthTag">
        /// The three length tag.
        /// </param>
        /// <param name="includeFont">
        /// The include font.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool LineEndsWithHtmlTag(this string text, bool threeLengthTag, bool includeFont = false)
        {
            if (text == null || text.Length < 6)
            {
                return false;
            }

            return EndsWithHtmlTag(text, threeLengthTag, includeFont);
        }

        /// <summary>
        /// The line break starts with html tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threeLengthTag">
        /// The three length tag.
        /// </param>
        /// <param name="includeFont">
        /// The include font.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool LineBreakStartsWithHtmlTag(this string text, bool threeLengthTag, bool includeFont = false)
        {
            if (text == null || (!threeLengthTag && !includeFont))
            {
                return false;
            }

            var newLineIdx = text.IndexOf(Environment.NewLine, StringComparison.Ordinal);
            if (newLineIdx < 0 || text.Length < newLineIdx + 5)
            {
                return false;
            }

            text = text.Substring(newLineIdx + 2);
            return StartsWithHtmlTag(text, threeLengthTag, includeFont);
        }

        /// <summary>
        /// The starts with html tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threeLengthTag">
        /// The three length tag.
        /// </param>
        /// <param name="includeFont">
        /// The include font.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool StartsWithHtmlTag(string text, bool threeLengthTag, bool includeFont)
        {
            if (threeLengthTag && text.Length > 3 && text[0] == '<' && text[2] == '>' && (text[1] == 'i' || text[1] == 'I' || text[1] == 'u' || text[1] == 'U' || text[1] == 'b' || text[1] == 'B'))
            {
                return true;
            }

            if (includeFont && text.Length > 5 && text.StartsWith("<font", StringComparison.OrdinalIgnoreCase))
            {
                return text.IndexOf('>', 5) >= 5; // <font> or <font color="#000000">
            }

            return false;
        }

        /// <summary>
        /// The ends with html tag.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threeLengthTag">
        /// The three length tag.
        /// </param>
        /// <param name="includeFont">
        /// The include font.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool EndsWithHtmlTag(string text, bool threeLengthTag, bool includeFont)
        {
            var len = text.Length;
            if (text[len - 1] != '>')
            {
                return false;
            }

            // </font> </i>
            if (threeLengthTag && len > 3 && text[len - 4] == '<' && text[len - 3] == '/')
            {
                return true;
            }

            if (includeFont && len > 8 && text[len - 7] == '<' && text[len - 6] == '/')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The starts with.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool StartsWith(this string s, char c)
        {
            return s.Length > 0 && s[0] == c;
        }

        /// <summary>
        /// The starts with.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool StartsWith(this StringBuilder sb, char c)
        {
            return sb.Length > 0 && sb[0] == c;
        }

        /// <summary>
        /// The ends with.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool EndsWith(this string s, char c)
        {
            return s.Length > 0 && s[s.Length - 1] == c;
        }

        /// <summary>
        /// The ends with.
        /// </summary>
        /// <param name="sb">
        /// The sb.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool EndsWith(this StringBuilder sb, char c)
        {
            return sb.Length > 0 && sb[sb.Length - 1] == c;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Contains(this string source, char value)
        {
            return source.IndexOf(value) >= 0;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Contains(this string source, char[] value)
        {
            return source.IndexOfAny(value) >= 0;
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="comparisonType">
        /// The comparison type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        /// <summary>
        /// The split to lines.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public static string[] SplitToLines(this string source)
        {
            return source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        }

        // http://www.codeproject.com/Articles/43726/Optimizing-string-operations-in-C
        /// <summary>
        /// The fast index of.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="pattern">
        /// The pattern.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public static int FastIndexOf(this string source, string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException();
            }

            if (pattern.Length == 0)
            {
                return 0;
            }

            if (pattern.Length == 1)
            {
                return source.IndexOf(pattern[0]);
            }

            int limit = source.Length - pattern.Length + 1;
            if (limit < 1)
            {
                return -1;
            }

            // Store the first 2 characters of "pattern"
            char c0 = pattern[0];
            char c1 = pattern[1];

            // Find the first occurrence of the first character
            int first = source.IndexOf(c0, 0, limit);
            while (first != -1)
            {
                // Check if the following character is the same like
                // the 2nd character of "pattern"
                if (source[first + 1] != c1)
                {
                    first = source.IndexOf(c0, ++first, limit - first);
                    continue;
                }

                // Check the rest of "pattern" (starting with the 3rd character)
                bool found = true;
                for (var j = 2; j < pattern.Length; j++)
                {
                    if (source[first + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                }

                // If the whole word was found, return its index, otherwise try again
                if (found)
                {
                    return first;
                }

                first = source.IndexOf(c0, ++first, limit - first);
            }

            return -1;
        }

        /// <summary>
        /// The index of any.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="words">
        /// The words.
        /// </param>
        /// <param name="comparionType">
        /// The comparion type.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int IndexOfAny(this string s, string[] words, StringComparison comparionType)
        {
            if (words == null || string.IsNullOrEmpty(s))
            {
                return -1;
            }

            for (int i = 0; i < words.Length; i++)
            {
                var idx = s.IndexOf(words[i], comparionType);
                if (idx >= 0)
                {
                    return idx;
                }
            }

            return -1;
        }

        /// <summary>
        /// The fix extra spaces.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FixExtraSpaces(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            while (s.Contains("  "))
            {
                s = s.Replace("  ", " ");
            }

            s = s.Replace(" " + Environment.NewLine, Environment.NewLine);
            return s.Replace(Environment.NewLine + " ", Environment.NewLine);
        }

        /// <summary>
        /// The contains letter.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ContainsLetter(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return false;
            }

            foreach (var c in s)
            {
                if (char.IsLetter(c))
                {
                    return true;
                }
            }

            return false;
        }
    }
}