// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScenaristClosedCaptionsDropFrame.cs" company="">
//   
// </copyright>
// <summary>
//   The scenarist closed captions drop frame.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// The scenarist closed captions drop frame.
    /// </summary>
    public class ScenaristClosedCaptionsDropFrame : ScenaristClosedCaptions
    {
        // 00:01:00:29   9420 9420 94ae 94ae 94d0 94d0 4920 f761 7320 ...    semi colon (instead of colon) before frame number is used to indicate drop frame
        /// <summary>
        /// The _time code reg ex.
        /// </summary>
        private const string _timeCodeRegEx = @"^\d+:\d\d:\d\d[;,]\d\d\t";

        /// <summary>
        /// The _regex.
        /// </summary>
        private static readonly Regex _regex = new Regex(_timeCodeRegEx, RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenaristClosedCaptionsDropFrame"/> class.
        /// </summary>
        public ScenaristClosedCaptionsDropFrame()
        {
            this.DropFrame = true;
        }

        /// <summary>
        /// Gets the regex time codes.
        /// </summary>
        protected override Regex RegexTimeCodes
        {
            get
            {
                return _regex;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Scenarist Closed Captions Drop Frame";
            }
        }
    }
}