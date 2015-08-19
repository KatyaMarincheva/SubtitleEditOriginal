namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Text.RegularExpressions;

    public class ScenaristClosedCaptionsDropFrame : ScenaristClosedCaptions
    {
        // 00:01:00:29   9420 9420 94ae 94ae 94d0 94d0 4920 f761 7320 ...    semi colon (instead of colon) before frame number is used to indicate drop frame
        private const string _timeCodeRegEx = @"^\d+:\d\d:\d\d[;,]\d\d\t";

        private static readonly Regex _regex = new Regex(_timeCodeRegEx, RegexOptions.Compiled);

        public ScenaristClosedCaptionsDropFrame()
        {
            this.DropFrame = true;
        }

        protected override Regex RegexTimeCodes
        {
            get
            {
                return _regex;
            }
        }

        public override string Name
        {
            get
            {
                return "Scenarist Closed Captions Drop Frame";
            }
        }
    }
}