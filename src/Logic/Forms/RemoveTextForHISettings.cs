// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveTextForHISettings.cs" company="">
//   
// </copyright>
// <summary>
//   The remove text for hi settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.Forms
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The remove text for hi settings.
    /// </summary>
    public class RemoveTextForHISettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveTextForHISettings"/> class.
        /// </summary>
        public RemoveTextForHISettings()
        {
            this.OnlyIfInSeparateLine = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenOnlySeperateLines;
            this.RemoveIfAllUppercase = Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfAllUppercase;
            this.RemoveTextBeforeColon = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColon;
            this.RemoveTextBeforeColonOnlyUppercase = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyIfUppercase;
            this.ColonSeparateLine = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBeforeColonOnlyOnSeparateLine;
            this.RemoveWhereContains = Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContains;
            this.RemoveIfTextContains = new List<string>();
            foreach (string item in Configuration.Settings.RemoveTextForHearingImpaired.RemoveIfContainsText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                this.RemoveIfTextContains.Add(item.Trim());
            }

            this.RemoveTextBetweenCustomTags = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustom;
            this.RemoveInterjections = Configuration.Settings.RemoveTextForHearingImpaired.RemoveInterjections;
            this.RemoveTextBetweenSquares = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenBrackets;
            this.RemoveTextBetweenBrackets = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCurlyBrackets;
            this.RemoveTextBetweenQuestionMarks = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenQuestionMarks;
            this.RemoveTextBetweenParentheses = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenParentheses;
            this.CustomStart = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomBefore;
            this.CustomEnd = Configuration.Settings.RemoveTextForHearingImpaired.RemoveTextBetweenCustomAfter;
        }

        /// <summary>
        /// Gets or sets a value indicating whether only if in separate line.
        /// </summary>
        public bool OnlyIfInSeparateLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove if all uppercase.
        /// </summary>
        public bool RemoveIfAllUppercase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text before colon.
        /// </summary>
        public bool RemoveTextBeforeColon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text before colon only uppercase.
        /// </summary>
        public bool RemoveTextBeforeColonOnlyUppercase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether colon separate line.
        /// </summary>
        public bool ColonSeparateLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove where contains.
        /// </summary>
        public bool RemoveWhereContains { get; set; }

        /// <summary>
        /// Gets or sets the remove if text contains.
        /// </summary>
        public List<string> RemoveIfTextContains { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between custom tags.
        /// </summary>
        public bool RemoveTextBetweenCustomTags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove interjections.
        /// </summary>
        public bool RemoveInterjections { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between squares.
        /// </summary>
        public bool RemoveTextBetweenSquares { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between brackets.
        /// </summary>
        public bool RemoveTextBetweenBrackets { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between question marks.
        /// </summary>
        public bool RemoveTextBetweenQuestionMarks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether remove text between parentheses.
        /// </summary>
        public bool RemoveTextBetweenParentheses { get; set; }

        /// <summary>
        /// Gets or sets the custom start.
        /// </summary>
        public string CustomStart { get; set; }

        /// <summary>
        /// Gets or sets the custom end.
        /// </summary>
        public string CustomEnd { get; set; }
    }
}