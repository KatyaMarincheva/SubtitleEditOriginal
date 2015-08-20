// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TmpegEncText.cs" company="">
//   
// </copyright>
// <summary>
//   The tmpeg enc text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// The tmpeg enc text.
    /// </summary>
    public class TmpegEncText : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".subtitle";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Tmpeg Encoder Text";
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
            sb.AppendLine(@"[LayoutData]
'Picture bottom layout',4,Tahoma,0.069,17588159451135,0,0,0,0,1,2,0,1,0.00345,0
'Picture top layout',4,Tahoma,0.1,17588159451135,0,0,0,0,1,0,0,1,0.005,0
'Picture left layout',4,Tahoma,0.1,17588159451135,0,0,0,0,0,1,1,1,0.005,0
'Picture right layout',4,Tahoma,0.1,17588159451135,0,0,0,0,2,1,1,1,0.005,0

[LayoutDataEx]
1,0
1,0
1,0
1,1

[ItemData]").Replace("'", "\"");
            int i = 0;
            foreach (Paragraph p in subtitle.Paragraphs)
            {
                i++;
                sb.AppendLine(string.Format("{0},1,\"{1}\",\"{2}\",0,\"{3}\"", i, p.StartTime, p.EndTime, p.Text.Replace(Environment.NewLine, "\\n").Replace("\"", string.Empty)));
            }

            return sb.ToString().Trim() + Environment.NewLine;
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
            // 1,1,"00:01:57,269","00:01:59,169",0,"These hills here are full of Apaches."
            StringBuilder temp = new StringBuilder();
            foreach (string l in lines)
            {
                temp.Append(l);
            }

            string all = temp.ToString();
            if (!all.Contains("[ItemData]"))
            {
                return;
            }

            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                string[] arr = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length >= 8 && Utilities.IsInteger(arr[0]) && Utilities.IsInteger(arr[1]))
                {
                    Paragraph p = new Paragraph();
                    try
                    {
                        p.StartTime = GetTimeCode(arr[2] + "," + arr[3]);
                        p.EndTime = GetTimeCode(arr[4] + "," + arr[5]);
                        p.Text = line.Trim().TrimEnd('"');
                        p.Text = p.Text.Substring(p.Text.LastIndexOf('"')).TrimStart('"');
                        p.Text = p.Text.Replace("\\n", Environment.NewLine);
                        subtitle.Paragraphs.Add(p);
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                        this._errorCount++;
                    }
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode GetTimeCode(string code)
        {
            code = code.Trim().Trim('"');
            string[] arr = code.Split(new[] { ':', '.', ',' }, StringSplitOptions.RemoveEmptyEntries);
            int h = int.Parse(arr[0]);
            int m = int.Parse(arr[1]);
            int s = int.Parse(arr[2]);
            int ms = int.Parse(arr[3]);
            return new TimeCode(h, m, s, ms);
        }
    }
}