// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AvidStl.cs" company="">
//   
// </copyright>
// <summary>
//   The avid stl.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Nikse.SubtitleEdit.Core;

    /// <summary>
    /// The avid stl.
    /// </summary>
    public class AvidStl : SubtitleFormat
    {
        /// <summary>
        /// The name of format.
        /// </summary>
        public const string NameOfFormat = "Avid STL";

        /// <summary>
        /// The text length.
        /// </summary>
        private const int TextLength = 112;

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".stl";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return NameOfFormat;
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
        /// The write subtitle block.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="number">
        /// The number.
        /// </param>
        public static void WriteSubtitleBlock(FileStream fs, Paragraph p, int number)
        {
            fs.WriteByte(0);
            fs.WriteByte((byte)(number % 256)); // number - low byte
            fs.WriteByte((byte)(number / 256)); // number - high byte
            fs.WriteByte(0xff);
            fs.WriteByte(0);
            WriteTimeCode(fs, p.StartTime);
            WriteTimeCode(fs, p.EndTime);
            fs.WriteByte(1);
            fs.WriteByte(2);
            fs.WriteByte(0);
            byte[] buffer = Encoding.GetEncoding(1252).GetBytes(p.Text.Replace(Environment.NewLine, "Š"));
            if (buffer.Length <= 128)
            {
                fs.Write(buffer, 0, buffer.Length);
                for (int i = buffer.Length; i < TextLength; i++)
                {
                    fs.WriteByte(0x8f);
                }
            }
            else
            {
                for (int i = 0; i < TextLength; i++)
                {
                    fs.WriteByte(buffer[i]);
                }
            }
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        public static void Save(string fileName, Subtitle subtitle)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = { 0x38, 0x35, 0x30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x30, 0x30, 0x30, 0x39 };
                fs.Write(buffer, 0, buffer.Length);
                for (int i = 0; i < 0xde; i++)
                {
                    fs.WriteByte(0);
                }

                string numberOfLines = subtitle.Paragraphs.Count.ToString("D5");

                buffer = Encoding.ASCII.GetBytes(numberOfLines + numberOfLines + "001");
                fs.Write(buffer, 0, buffer.Length);
                for (int i = 0; i < 0x15; i++)
                {
                    fs.WriteByte(0);
                }

                buffer = Encoding.ASCII.GetBytes("11");
                fs.Write(buffer, 0, buffer.Length);
                while (fs.Length < 1024)
                {
                    fs.WriteByte(0);
                }

                int subtitleNumber = 0;
                foreach (Paragraph p in subtitle.Paragraphs)
                {
                    WriteSubtitleBlock(fs, p, subtitleNumber);
                    subtitleNumber++;
                }
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
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                try
                {
                    FileInfo fi = new FileInfo(fileName);
                    if (fi.Length > 1150 && fi.Length < 1024000)
                    {
                        // not too small or too big
                        byte[] buffer = FileUtil.ReadAllBytesShared(fileName);
                        if (buffer[0] == 0x38 && buffer[1] == 0x35 && buffer[2] == 0x30 && buffer[1024] == 0 && buffer[1025] == 0 && buffer[1026] == 0 && buffer[1027] == 0xff)
                        {
                            return true;
                        }

                        if (fileName.EndsWith(".stl", StringComparison.OrdinalIgnoreCase) && buffer.Length > 1283 && buffer[1024] == 0 && buffer[1025] == 1 && buffer[1026] == 0 && buffer[1027] == 0xff && buffer[1152] == 0 && buffer[1153] == 2 && buffer[1154] == 0 && buffer[1155] == 0xff && buffer[1280] == 0 && buffer[1281] == 3 && buffer[1282] == 0 && buffer[1283] == 0xff)
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
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
            return "Not supported!";
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
            subtitle.Paragraphs.Clear();
            subtitle.Header = null;
            byte[] buffer = FileUtil.ReadAllBytesShared(fileName);

            int index = 1024;
            while (index <= buffer.Length - 128)
            {
                Paragraph p = ReadSubtitleBlock(buffer, index);
                subtitle.Paragraphs.Add(p);
                index += 128;
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The read subtitle block.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Paragraph"/>.
        /// </returns>
        private static Paragraph ReadSubtitleBlock(byte[] buffer, int index)
        {
            index += 5;
            Paragraph p = new Paragraph();
            p.StartTime = ReadTimeCode(buffer, ref index);
            p.EndTime = ReadTimeCode(buffer, ref index);
            index += 3;
            for (int i = index; i < index + TextLength; i++)
            {
                if (buffer[i] == 0x8f || buffer[i] == 0)
                {
                    buffer[i] = 32;
                }
                else if (buffer[i] == 0x8a)
                {
                    buffer[i] = 0xa;
                }
            }

            p.Text = Encoding.GetEncoding(1252).GetString(buffer, index, TextLength).Trim();
            p.Text = p.Text.Replace("\n", Environment.NewLine);
            return p;
        }

        /// <summary>
        /// The read time code.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="TimeCode"/>.
        /// </returns>
        private static TimeCode ReadTimeCode(byte[] buffer, ref int index)
        {
            int hours = buffer[index];
            int minutes = buffer[index + 1];
            int seconds = buffer[index + 2];
            int milliseconds = FramesToMillisecondsMax999(buffer[index + 3]);
            index += 4;
            return new TimeCode(hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        /// The write time code.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <param name="tc">
        /// The tc.
        /// </param>
        private static void WriteTimeCode(FileStream fs, TimeCode tc)
        {
            fs.WriteByte((byte)tc.Hours);
            fs.WriteByte((byte)tc.Minutes);
            fs.WriteByte((byte)tc.Seconds);
            fs.WriteByte((byte)MillisecondsToFramesMaxFrameRate(tc.Milliseconds));
        }
    }
}