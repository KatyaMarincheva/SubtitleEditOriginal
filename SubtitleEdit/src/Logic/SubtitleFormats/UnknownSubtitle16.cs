namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    public class UnknownSubtitle16 : SubtitleFormat
    {
        public override string Extension
        {
            get
            {
                return ".cip";
            }
        }

        public override string Name
        {
            get
            {
                return "Unknown 16";
            }
        }

        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > this._errorCount;
        }

        public override string ToText(Subtitle subtitle, string title)
        {
            UnknownSubtitle52 u52 = new UnknownSubtitle52();
            using (RichTextBox rtBox = new RichTextBox { Text = u52.ToText(subtitle, title) })
            {
                return rtBox.Rtf;
            }
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;

            if (lines.Count == 0 || !lines[0].TrimStart().StartsWith("{\\rtf1"))
            {
                return;
            }

            // load as text via RichTextBox
            StringBuilder text = new StringBuilder();
            foreach (string s in lines)
            {
                text.AppendLine(s);
            }

            using (RichTextBox rtBox = new RichTextBox())
            {
                rtBox.Rtf = text.ToString();
                List<string> lines2 = new List<string>();
                foreach (string line in rtBox.Lines)
                {
                    lines2.Add(line);
                }

                UnknownSubtitle52 u52 = new UnknownSubtitle52();
                u52.LoadSubtitle(subtitle, lines2, fileName);
                this._errorCount = u52.ErrorCount;
            }
        }
    }
}