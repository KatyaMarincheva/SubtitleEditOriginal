﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Ayato.cs" company="">
//   
// </copyright>
// <summary>
//   The ayato.
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
    /// The ayato.
    /// </summary>
    public class Ayato : SubtitleFormat
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return "aya";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Ayato";
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
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                FileInfo fi = new FileInfo(fileName);
                if (fi.Length >= 3000 && fi.Length < 1024000)
                {
                    // not too small or too big
                    if (!fileName.EndsWith(".aya", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }

                    Subtitle sub = new Subtitle();
                    this.LoadSubtitle(sub, lines, fileName);
                    return sub.Paragraphs.Count > 0;
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override string ToText(Subtitle subtitle, string title)
        {
            throw new NotImplementedException();
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
            const int startPosition = 0xa99;
            const int textPosition = 72;

            this._errorCount = 0;
            subtitle.Paragraphs.Clear();
            subtitle.Header = null;
            byte[] buffer = FileUtil.ReadAllBytesShared(fileName);
            int index = startPosition;
            if (buffer[index] != 1)
            {
                return;
            }

            while (index + textPosition < buffer.Length)
            {
                int textLength = buffer[index + 16];
                if (textLength > 0 && index + textPosition + textLength < buffer.Length)
                {
                    string text = GetText(index + textPosition, textLength, buffer);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        int startFrames = GetFrames(index + 4, buffer);
                        int endFrames = GetFrames(index + 8, buffer);
                        subtitle.Paragraphs.Add(new Paragraph(text, FramesToMilliseconds(startFrames), FramesToMilliseconds(endFrames)));
                    }
                }

                index += textPosition + textLength;
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The get text.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetText(int index, int length, byte[] buffer)
        {
            if (length < 1)
            {
                return string.Empty;
            }

            int offset = 0;
            if (buffer[index] == 7)
            {
                offset = 1;
            }
            else if (buffer[index + 1] == 7)
            {
                offset = 2;
            }
            else if (buffer[index + 2] == 7)
            {
                offset = 3;
            }

            if (length - offset < 1)
            {
                return string.Empty;
            }

            const string newline1 = ""; // unicode chars
            const string newline2 = ""; // unicode char
            string s = Encoding.UTF8.GetString(buffer, index + offset, length - offset);
            s = s.Replace(newline1, Environment.NewLine);
            s = s.Replace(newline2, Environment.NewLine);
            return s;
        }

        /// <summary>
        /// The get frames.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int GetFrames(int index, byte[] buffer)
        {
            return (buffer[index + 2] << 16) + (buffer[index + 1] << 8) + buffer[index];
        }
    }
}