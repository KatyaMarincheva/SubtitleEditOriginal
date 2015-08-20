// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSubtitle23.cs" company="">
//   
// </copyright>
// <summary>
//   The unknown subtitle 23.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// The unknown subtitle 23.
    /// </summary>
    public class UnknownSubtitle23 : SubtitleFormat
    {
        // 1:  01:00:19.04  01:00:21.05
        /// <summary>
        /// The regex time code 1.
        /// </summary>
        private static readonly Regex RegexTimeCode1 = new Regex(@"^\s*\d+:\s+\d\d:\d\d:\d\d.\d\d\s+\d\d:\d\d:\d\d.\d\d\s*$", RegexOptions.Compiled);

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".rtf";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown 23";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is time based.
        /// </summary>
        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The is mine.
        /// </summary>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsMine(List<string> lines, string fileName)
        {
            Subtitle subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > this._errorCount;
        }

        /// <summary>
        /// The to text.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToText(Subtitle subtitle, string title)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(title);
            sb.AppendLine(@"1ab

23/03/2012
03/05/2012
**:**:**.**
01:00:00.00
**:**:**.**
**:**:**.**
01:01:01.12
01:02:30.00
01:02:54.01
**:**:**.**
**:**:**.**
01:19:33.08
");

            int count = 1;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                sb.AppendLine(string.Format("{0}:  {1}  {2}\r\n{3}", count.ToString(CultureInfo.InvariantCulture).PadLeft(9, ' '), MakeTimeCode(p.StartTime), MakeTimeCode(p.EndTime), p.Text));
                count++;
            }

            using (RichTextBox rtBox = new RichTextBox { Text = sb.ToString().Trim() })
            {
                return rtBox.Rtf;
            }
        }

        /// <summary>
        /// The load subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="lines">
        /// The lines.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
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

            string text = string.Empty;
            RichTextBox rtBox = new RichTextBox();
            try
            {
                rtBox.Rtf = rtf;
                text = rtBox.Text.Replace("\r\n", "\n");
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                return;
            }
            finally
            {
                rtBox.Dispose();
            }

            lines = new List<string>();
            foreach (string line in text.Split('\n'))
            {
                lines.Add(line);
            }

            this._errorCount = 0;
            Paragraph p = null;
            sb = new StringBuilder();
            foreach (string line in lines)
            {
                string s = line.TrimEnd();
                if (RegexTimeCode1.IsMatch(s))
                {
                    try
                    {
                        if (p != null)
                        {
                            p.Text = sb.ToString().Trim();
                            subtitle.Paragraphs.Add(p);
                        }

                        sb = new StringBuilder();
                        string[] arr = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length == 3)
                        {
                            p = new Paragraph(DecodeTimeCode(arr[1]), DecodeTimeCode(arr[2]), string.Empty);
                        }
                    }
                    catch
                    {
                        this._errorCount++;
                        p = null;
                    }
                }
                else if (p != null && s.Length > 0)
                {
                    sb.AppendLine(s.Trim());
                }
                else if (!string.IsNullOrWhiteSpace(s))
                {
                    this._errorCount++;
                }
            }

            if (p != null)
            {
                p.Text = sb.ToString().Trim();
                subtitle.Paragraphs.Add(p);
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The make time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MakeTimeCode(TimeCode timeCode)
        {
            return timeCode.ToHHMMSSPeriodFF();
        }

        /// <summary>
        /// The decode time code.
        /// </summary>
        /// <param name="timeCode">
        /// The time code.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode DecodeTimeCode(string timeCode)
        {
            string[] arr = timeCode.Split(new[] { ':', ';', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            return new TimeCode(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]), FramesToMillisecondsMax999(int.Parse(arr[3])));
        }
    }
}