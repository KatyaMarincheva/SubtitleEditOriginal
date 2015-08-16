namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Windows.Forms;

    public class F4Rtf : F4Text
    {
        public override string Extension
        {
            get
            {
                return ".rtf";
            }
        }

        public override string Name
        {
            get
            {
                return "F4 Rich Text Format";
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
            if (fileName != null && !fileName.EndsWith(this.Extension, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > this._errorCount;
        }

        public override string ToText(Subtitle subtitle, string title)
        {
            RichTextBox rtBox = new RichTextBox();
            rtBox.Text = ToF4Text(subtitle);
            string rtf = rtBox.Rtf;
            rtBox.Dispose();
            return rtf;
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            this._errorCount = 0;
            StringBuilder sb = new StringBuilder();
            foreach (string line in lines)
            {
                sb.AppendLine(line);
            }

            string rtf = sb.ToString().Trim();
            if (!rtf.StartsWith("{\\rtf"))
            {
                return;
            }

            RichTextBox rtBox = new RichTextBox();
            try
            {
                rtBox.Rtf = rtf;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return;
            }

            string text = rtBox.Text;
            rtBox.Dispose();
            this.LoadF4TextSubtitle(subtitle, text);
        }
    }
}