// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimelineMvt.cs" company="">
//   
// </copyright>
// <summary>
//   Timeline - THE MOVIE TITRE EDITOR - http://www.pld.ttu.ee/~priidu/timeline/ by priidu@pld.ttu.ee
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    ///     Timeline - THE MOVIE TITRE EDITOR - http://www.pld.ttu.ee/~priidu/timeline/ by priidu@pld.ttu.ee
    /// </summary>
    public class TimeLineMvt : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".mvt";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Timeline mvt";
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
            return string.Empty;
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
            byte[] bytes = File.ReadAllBytes(fileName);
            if (bytes.Length < 100 || bytes[0] != 0x54 || bytes[1] != 0x50 || bytes[2] != 0x46)
            {
                return;
            }

            // title
            int index = 9;
            while (index < bytes.Length && bytes[index] != 0x6)
            {
                index++;
            }

            if (index + 1 >= bytes.Length)
            {
                return;
            }

            string title = Encoding.UTF8.GetString(bytes, 9, index - 9);
            Console.WriteLine(title);

            // language1
            index += 2;
            int start = index;
            while (index < bytes.Length && bytes[index] != 0x6)
            {
                index++;
            }

            if (index + 1 >= bytes.Length)
            {
                return;
            }

            string language1 = Encoding.UTF8.GetString(bytes, start, index - start);
            Console.WriteLine(language1);

            // language2
            index += 2;
            start = index;
            while (index < bytes.Length && bytes[index] != 0x6)
            {
                index++;
            }

            if (index + 1 >= bytes.Length)
            {
                return;
            }

            string language2 = Encoding.UTF8.GetString(bytes, start, index - start);
            Console.WriteLine(language2);

            Encoding encoding1 = this.GetEncodingFromLanguage(language1);
            Encoding encoding2 = this.GetEncodingFromLanguage(language2);

            this._errorCount = 0;
            while (index < bytes.Length - 20)
            {
                if (bytes[index] == 5 && bytes[index + 1] == 0 && bytes[index + 2] == 0)
                {
                    // find subtitle
                    // time codes
                    int timeCodeIndexStart = index + 4;
                    int timeCodeIndexEnd = index + 15;
                    index += 22;
                    while (index < bytes.Length && bytes[index] != 0x6)
                    {
                        index++;
                    }

                    index += 2;
                    while (index < bytes.Length && bytes[index] == 0x6)
                    {
                        index += 2;
                    }

                    if (index < bytes.Length - 3)
                    {
                        // first line
                        start = index;
                        while (index < bytes.Length && bytes[index] != 0x6)
                        {
                            index++;
                        }

                        if (index < bytes.Length - 3)
                        {
                            string text1 = encoding1.GetString(bytes, start, index - start);
                            index += 2;

                            // second line
                            start = index;
                            while (index < bytes.Length && bytes[index] != 0x5)
                            {
                                index++;
                            }

                            if (index + 1 < bytes.Length)
                            {
                                string text2 = encoding2.GetString(bytes, start, index - start);
                                Paragraph p = new Paragraph { Text = text1 + Environment.NewLine + text2, StartTime = { TotalMilliseconds = this.GetTimeCode(bytes, timeCodeIndexStart) }, EndTime = { TotalMilliseconds = this.GetTimeCode(bytes, timeCodeIndexEnd) } };
                                subtitle.Paragraphs.Add(p);
                                index--;
                            }
                        }
                    }
                }

                index++;
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The get time code.
        /// </summary>
        /// <param name="bytes">
        /// The bytes.
        /// </param>
        /// <param name="timeCodeIndex">
        /// The time code index.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private double GetTimeCode(byte[] bytes, int timeCodeIndex)
        {
            // TODO: figure out how to get time code from these 7 bytes!
            if (bytes == null || bytes.Length < timeCodeIndex + 8)
            {
                return 0;
            }

            Console.WriteLine(bytes[timeCodeIndex + 0].ToString("X2") + " " + bytes[timeCodeIndex + 1].ToString("X2") + " " + bytes[timeCodeIndex + 2].ToString("X2") + " " + bytes[timeCodeIndex + 3].ToString("X2") + " " + bytes[timeCodeIndex + 4].ToString("X2") + " " + bytes[timeCodeIndex + 5].ToString("X2") + " " + bytes[timeCodeIndex + 6].ToString("X2"));
            return ((bytes[timeCodeIndex + 5] << 24) + (bytes[timeCodeIndex + 4] << 16) + (bytes[timeCodeIndex + 3] << 8) + bytes[timeCodeIndex + 2]) / 1800.0;
        }

        /// <summary>
        /// The get encoding from language.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <returns>
        /// The <see cref="Encoding"/>.
        /// </returns>
        private Encoding GetEncodingFromLanguage(string language)
        {
            if (language == "Russian")
            {
                return Encoding.GetEncoding(1251);
            }

            if (language == "Estonian" || language == "Latvian" || language == "Lithuanian")
            {
                return Encoding.GetEncoding(1257);
            }

            return Encoding.GetEncoding(1252);
        }
    }
}