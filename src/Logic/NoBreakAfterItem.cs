// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NoBreakAfterItem.cs" company="">
//   
// </copyright>
// <summary>
//   The no break after item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The no break after item.
    /// </summary>
    public class NoBreakAfterItem : IComparable
    {
        /// <summary>
        /// The regex.
        /// </summary>
        public readonly Regex Regex;

        /// <summary>
        /// The text.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoBreakAfterItem"/> class.
        /// </summary>
        /// <param name="regex">
        /// The regex.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        public NoBreakAfterItem(Regex regex, string text)
        {
            this.Regex = regex;
            this.Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoBreakAfterItem"/> class.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        public NoBreakAfterItem(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// The compare to.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }

            var o = obj as NoBreakAfterItem;
            if (o == null)
            {
                return -1;
            }

            if (o.Text == null && this.Text == null)
            {
                return 0;
            }

            if (o.Text == null)
            {
                return -1;
            }

            return string.Compare(this.Text, o.Text, StringComparison.Ordinal);
        }

        /// <summary>
        /// The is match.
        /// </summary>
        /// <param name="line">
        /// The line.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsMatch(string line)
        {
            if (this.Regex != null)
            {
                return this.Regex.IsMatch(line);
            }

            if (!string.IsNullOrEmpty(this.Text) && line.EndsWith(this.Text, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

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