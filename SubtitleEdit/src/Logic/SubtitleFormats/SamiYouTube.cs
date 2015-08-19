namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Collections.Generic;

    public class SamiYouTube : Sami
    {
        public override string Name
        {
            get
            {
                return "SAMI YouTube";
            }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > this._errorCount;
        }
    }
}