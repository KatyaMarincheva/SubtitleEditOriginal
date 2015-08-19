namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System.Collections.Generic;

    public class OcrCharacter
    {
        public OcrCharacter(string text)
        {
            this.Text = text;
            this.OcrImages = new List<OcrImage>();
        }

        public string Text { get; private set; }

        public List<OcrImage> OcrImages { get; set; }
    }
}