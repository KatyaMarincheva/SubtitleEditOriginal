namespace Nikse.SubtitleEdit.Logic.Ocr
{
    using System.Collections.Generic;

    public class OcrAlphabet
    {
        public OcrAlphabet()
        {
            this.OcrCharacters = new List<OcrCharacter>();
        }

        public List<OcrCharacter> OcrCharacters { get; private set; }

        public int CalculateMaximumSize()
        {
            int max = 0;
            foreach (OcrCharacter c in this.OcrCharacters)
            {
                foreach (OcrImage img in c.OcrImages)
                {
                    int size = img.Bmp.Width * img.Bmp.Height;
                    if (size > max)
                    {
                        max = size;
                    }
                }
            }

            return max;
        }

        public OcrCharacter GetOcrCharacter(string text, bool addIfNotExists)
        {
            foreach (OcrCharacter ocrCharacter in this.OcrCharacters)
            {
                if (ocrCharacter.Text == text)
                {
                    return ocrCharacter;
                }
            }

            if (addIfNotExists)
            {
                OcrCharacter ch = new OcrCharacter(text);
                this.OcrCharacters.Add(ch);
                return ch;
            }

            return null;
        }
    }
}