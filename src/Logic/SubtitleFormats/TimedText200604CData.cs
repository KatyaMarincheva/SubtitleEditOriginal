// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedText200604CData.cs" company="">
//   
// </copyright>
// <summary>
//   The timed text 200604 c data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    /// <summary>
    /// The timed text 200604 c data.
    /// </summary>
    public class TimedText200604CData : TimedText200604
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimedText200604CData"/> class.
        /// </summary>
        public TimedText200604CData()
        {
            this.UseCDataForParagraphText = true;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Timed Text draft 2006-04 CDATA";
            }
        }
    }
}