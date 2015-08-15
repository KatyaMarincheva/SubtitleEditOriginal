namespace Nikse.SubtitleEdit.Logic.Forms
{
    using System;
    using System.Collections.Generic;

    public class RemoveTextForHISettings
    {
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

        public bool OnlyIfInSeparateLine { get; set; }

        public bool RemoveIfAllUppercase { get; set; }

        public bool RemoveTextBeforeColon { get; set; }

        public bool RemoveTextBeforeColonOnlyUppercase { get; set; }

        public bool ColonSeparateLine { get; set; }

        public bool RemoveWhereContains { get; set; }

        public List<string> RemoveIfTextContains { get; set; }

        public bool RemoveTextBetweenCustomTags { get; set; }

        public bool RemoveInterjections { get; set; }

        public bool RemoveTextBetweenSquares { get; set; }

        public bool RemoveTextBetweenBrackets { get; set; }

        public bool RemoveTextBetweenQuestionMarks { get; set; }

        public bool RemoveTextBetweenParentheses { get; set; }

        public string CustomStart { get; set; }

        public string CustomEnd { get; set; }
    }
}