﻿namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Forms;

    /// <summary>
    ///     EBU Subtitling data exchange format
    /// </summary>
    public class Ebu : SubtitleFormat
    {
        public EbuGeneralSubtitleInformation Header;

        public List<int> JustificationCodes = new List<int>();

        public List<int> VerticalPositions = new List<int>();

        private static readonly Regex RegExpr = new Regex(@"^[a-f0-9]{6}$", RegexOptions.Compiled);

        public override string Extension
        {
            get
            {
                return ".stl";
            }
        }

        public override string Name
        {
            get
            {
                return "EBU STL";
            }
        }

        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

        public override bool IsTextBased
        {
            get
            {
                return false;
            }
        }

        public static void Save(string fileName, Subtitle subtitle)
        {
            Save(fileName, subtitle, false);
        }

        public static void Save(string fileName, Subtitle subtitle, bool batchMode)
        {
            EbuGeneralSubtitleInformation header = new EbuGeneralSubtitleInformation();
            using (EbuSaveOptions saveOptions = new EbuSaveOptions())
            {
                if (subtitle.Header != null && subtitle.Header.Length > 1024 && (subtitle.Header.Contains("STL24") || subtitle.Header.Contains("STL25") || subtitle.Header.Contains("STL29") || subtitle.Header.Contains("STL30")))
                {
                    header = ReadHeader(Encoding.UTF8.GetBytes(subtitle.Header));
                    saveOptions.Initialize(header, 0, null, subtitle);
                }
                else
                {
                    saveOptions.Initialize(header, 0, fileName, subtitle);
                }

                if (!batchMode && saveOptions.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    header.TotalNumberOfSubtitles = subtitle.Paragraphs.Count.ToString("D5"); // seems to be 1 higher than actual number of subtitles
                    header.TotalNumberOfTextAndTimingInformationBlocks = header.TotalNumberOfSubtitles;

                    string today = string.Format("{0:yyMMdd}", DateTime.Now);
                    if (today.Length == 6)
                    {
                        header.CreationDate = today;
                        header.RevisionDate = today;
                    }

                    Paragraph firstParagraph = subtitle.GetParagraphOrDefault(0);
                    if (firstParagraph != null)
                    {
                        TimeCode tc = firstParagraph.StartTime;
                        string firstTimeCode = string.Format("{0:00}{1:00}{2:00}{3:00}", tc.Hours, tc.Minutes, tc.Seconds, EbuTextTimingInformation.GetFrameFromMilliseconds(tc.Milliseconds, header.FrameRate));
                        if (firstTimeCode.Length == 8)
                        {
                            header.TimeCodeFirstInCue = firstTimeCode;
                        }
                    }

                    byte[] buffer = Encoding.Default.GetBytes(header.ToString());
                    fs.Write(buffer, 0, buffer.Length);

                    int subtitleNumber = 0;
                    foreach (Paragraph p in subtitle.Paragraphs)
                    {
                        EbuTextTimingInformation tti = new EbuTextTimingInformation();

                        int rows;
                        if (!int.TryParse(header.MaximumNumberOfDisplayableRows, out rows))
                        {
                            rows = 23;
                        }

                        if (header.DisplayStandardCode == "1" || header.DisplayStandardCode == "2")
                        {
                            // teletext
                            rows = 23;
                        }
                        else if (header.DisplayStandardCode == "0" && header.MaximumNumberOfDisplayableRows == "02")
                        {
                            // open subtitling
                            rows = 15;
                        }

                        if (p.Text.StartsWith("{\\an7}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an8}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an9}", StringComparison.Ordinal))
                        {
                            tti.VerticalPosition = 1; // top (vertical)
                        }
                        else if (p.Text.StartsWith("{\\an4}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an5}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an6}", StringComparison.Ordinal))
                        {
                            tti.VerticalPosition = (byte)(rows / 2); // middle (vertical)
                        }
                        else
                        {
                            int startRow = (rows - 1) - Utilities.CountTagInText(p.Text, Environment.NewLine) * 2;
                            if (startRow < 0)
                            {
                                startRow = 0;
                            }

                            tti.VerticalPosition = (byte)startRow; // bottom (vertical)
                        }

                        tti.JustificationCode = saveOptions.JustificationCode;
                        if (p.Text.StartsWith("{\\an1}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an4}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an7}", StringComparison.Ordinal))
                        {
                            tti.JustificationCode = 1; // 01h=left-justified text
                        }
                        else if (p.Text.StartsWith("{\\an3}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an6}", StringComparison.Ordinal) || p.Text.StartsWith("{\\an9}", StringComparison.Ordinal))
                        {
                            tti.JustificationCode = 3; // 03h=right-justified
                        }
                        else
                        {
                            // If it's not left- or right-justified, it's centred.
                            tti.JustificationCode = 2; // 02h=centred text
                        }

                        tti.SubtitleNumber = (ushort)subtitleNumber;
                        tti.TextField = p.Text;
                        int startTag = tti.TextField.IndexOf('}');
                        if (tti.TextField.StartsWith("{\\", StringComparison.Ordinal) && startTag > 0 && startTag < 10)
                        {
                            tti.TextField = tti.TextField.Remove(0, startTag + 1);
                        }

                        tti.TimeCodeInHours = p.StartTime.Hours;
                        tti.TimeCodeInMinutes = p.StartTime.Minutes;
                        tti.TimeCodeInSeconds = p.StartTime.Seconds;
                        tti.TimeCodeInMilliseconds = p.StartTime.Milliseconds;
                        tti.TimeCodeOutHours = p.EndTime.Hours;
                        tti.TimeCodeOutMinutes = p.EndTime.Minutes;
                        tti.TimeCodeOutSeconds = p.EndTime.Seconds;
                        tti.TimeCodeOutMilliseconds = p.EndTime.Milliseconds;
                        buffer = tti.GetBytes(header);
                        fs.Write(buffer, 0, buffer.Length);
                        subtitleNumber++;
                    }
                }
            }
        }

        public static EbuGeneralSubtitleInformation ReadHeader(byte[] buffer)
        {
            EbuGeneralSubtitleInformation header = new EbuGeneralSubtitleInformation();
            header.CodePageNumber = Encoding.Default.GetString(buffer, 0, 3);
            header.DiskFormatCode = Encoding.Default.GetString(buffer, 3, 8);
            header.DisplayStandardCode = Encoding.Default.GetString(buffer, 11, 1);
            header.CharacterCodeTableNumber = Encoding.Default.GetString(buffer, 12, 2);
            header.LanguageCode = Encoding.Default.GetString(buffer, 14, 2);
            header.OriginalProgrammeTitle = Encoding.Default.GetString(buffer, 16, 32);
            header.OriginalEpisodeTitle = Encoding.Default.GetString(buffer, 48, 32);
            header.TranslatedProgrammeTitle = Encoding.Default.GetString(buffer, 80, 32);
            header.TranslatedEpisodeTitle = Encoding.Default.GetString(buffer, 112, 32);
            header.TranslatorsName = Encoding.Default.GetString(buffer, 144, 32);
            header.TranslatorsContactDetails = Encoding.Default.GetString(buffer, 176, 32);
            header.SubtitleListReferenceCode = Encoding.Default.GetString(buffer, 208, 16);
            header.CreationDate = Encoding.Default.GetString(buffer, 224, 6);
            header.RevisionDate = Encoding.Default.GetString(buffer, 230, 6);
            header.RevisionNumber = Encoding.Default.GetString(buffer, 236, 2);
            header.TotalNumberOfTextAndTimingInformationBlocks = Encoding.Default.GetString(buffer, 238, 5);
            header.TotalNumberOfSubtitles = Encoding.Default.GetString(buffer, 243, 5);
            header.TotalNumberOfSubtitleGroups = Encoding.Default.GetString(buffer, 248, 3);
            header.MaximumNumberOfDisplayableCharactersInAnyTextRow = Encoding.Default.GetString(buffer, 251, 2);
            header.MaximumNumberOfDisplayableRows = Encoding.Default.GetString(buffer, 253, 2);
            header.TimeCodeStatus = Encoding.Default.GetString(buffer, 255, 1);
            header.TimeCodeStartOfProgramme = Encoding.Default.GetString(buffer, 256, 8);
            header.CountryOfOrigin = Encoding.Default.GetString(buffer, 274, 3);
            header.SpareBytes = Encoding.Default.GetString(buffer, 373, 75);
            header.UserDefinedArea = Encoding.Default.GetString(buffer, 448, 576);

            return header;
        }

        public override bool IsMine(List<string> lines, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                FileInfo fi = new FileInfo(fileName);
                if (fi.Length >= 1024 + 128 && fi.Length < 1024000)
                {
                    // not too small or too big
                    try
                    {
                        byte[] buffer = FileUtil.ReadAllBytesShared(fileName);
                        EbuGeneralSubtitleInformation header = ReadHeader(buffer);
                        if (header.DiskFormatCode.StartsWith("STL23", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL24", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL25", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL29", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL30", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL35", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL48", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL50", StringComparison.Ordinal) || header.DiskFormatCode.StartsWith("STL60", StringComparison.Ordinal))
                        {
                            return Utilities.IsInteger(header.CodePageNumber) || fileName.EndsWith("stl", StringComparison.OrdinalIgnoreCase);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public override string ToText(Subtitle subtitle, string title)
        {
            return "Not supported!";
        }

        public override void LoadSubtitle(Subtitle subtitle, List<string> lines, string fileName)
        {
            subtitle.Paragraphs.Clear();
            subtitle.Header = null;
            byte[] buffer = FileUtil.ReadAllBytesShared(fileName);
            EbuGeneralSubtitleInformation header = ReadHeader(buffer);
            subtitle.Header = Encoding.UTF8.GetString(buffer);
            Paragraph last = null;
            byte lastExtensionBlockNumber = 0xff;
            this.JustificationCodes = new List<int>();
            this.VerticalPositions = new List<int>();
            foreach (EbuTextTimingInformation tti in this.ReadTTI(buffer, header))
            {
                if (tti.ExtensionBlockNumber != 0xfe)
                {
                    // FEh : Reserved for User Data
                    Paragraph p = new Paragraph();
                    p.Text = tti.TextField;
                    p.StartTime = new TimeCode(tti.TimeCodeInHours, tti.TimeCodeInMinutes, tti.TimeCodeInSeconds, tti.TimeCodeInMilliseconds);
                    p.EndTime = new TimeCode(tti.TimeCodeOutHours, tti.TimeCodeOutMinutes, tti.TimeCodeOutSeconds, tti.TimeCodeOutMilliseconds);

                    if (lastExtensionBlockNumber != 0xff && last != null)
                    {
                        last.Text += p.Text; // merge text
                    }
                    else
                    {
                        subtitle.Paragraphs.Add(p);
                        last = p;
                    }

                    lastExtensionBlockNumber = tti.ExtensionBlockNumber;
                }
            }

            subtitle.Renumber();
            this.Header = header;
        }

        /// <summary>
        ///     Get text with regard code page from header
        /// </summary>
        /// <param name="skipNext">Skip next character</param>
        /// <param name="header">EBU header</param>
        /// <param name="buffer">data buffer</param>
        /// <param name="index">index to current byte in buffer</param>
        /// <returns>Character at index</returns>
        private static string GetCharacter(out bool skipNext, EbuGeneralSubtitleInformation header, byte[] buffer, int index)
        {
            skipNext = false;
            if (header.CharacterCodeTableNumber == "00")
            {
                // note that 0xC1—0xCF combines characters - http://en.wikipedia.org/wiki/ISO/IEC_6937
                Encoding encoding = Encoding.GetEncoding(20269);
                string next = encoding.GetString(buffer, index + 1, 1);
                switch (buffer[index])
                {
                    case 0xc1: // Grave
                        skipNext = @"AEIOUaeiou".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "À";
                            case "E":
                                return "È";
                            case "I":
                                return "Ì";
                            case "O":
                                return "Ò";
                            case "U":
                                return "Ù";
                            case "a":
                                return "à";
                            case "e":
                                return "è";
                            case "i":
                                return "ì";
                            case "o":
                                return "ò";
                            case "u":
                                return "ù";
                        }

                        return string.Empty;
                    case 0xc2: // Acute
                        skipNext = @"ACEILNORSUYZacegilnorsuyz".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Á";
                            case "C":
                                return "Ć";
                            case "E":
                                return "É";
                            case "I":
                                return "Í";
                            case "L":
                                return "Ĺ";
                            case "N":
                                return "Ń";
                            case "O":
                                return "Ó";
                            case "R":
                                return "Ŕ";
                            case "S":
                                return "Ś";
                            case "U":
                                return "Ú";
                            case "Y":
                                return "Ý";
                            case "Z":
                                return "Ź";
                            case "a":
                                return "á";
                            case "c":
                                return "ć";
                            case "e":
                                return "é";
                            case "g":
                                return "ģ";
                            case "i":
                                return "í";
                            case "l":
                                return "ĺ";
                            case "n":
                                return "ń";
                            case "o":
                                return "ó";
                            case "r":
                                return "ŕ";
                            case "s":
                                return "ś";
                            case "u":
                                return "ú";
                            case "y":
                                return "ý";
                            case "z":
                                return "ź";
                        }

                        return string.Empty;
                    case 0xc3: // Circumflex
                        skipNext = @"ACEGHIJOSUWYaceghijosuwy".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Â";
                            case "C":
                                return "Ĉ";
                            case "E":
                                return "Ê";
                            case "G":
                                return "Ĝ";
                            case "H":
                                return "Ĥ";
                            case "I":
                                return "Î";
                            case "J":
                                return "Ĵ";
                            case "O":
                                return "Ô";
                            case "S":
                                return "Ŝ";
                            case "U":
                                return "Û";
                            case "W":
                                return "Ŵ";
                            case "Y":
                                return "Ŷ";
                            case "a":
                                return "â";
                            case "c":
                                return "ĉ";
                            case "e":
                                return "ê";
                            case "g":
                                return "ĝ";
                            case "h":
                                return "ĥ";
                            case "i":
                                return "î";
                            case "j":
                                return "ĵ";
                            case "o":
                                return "ô";
                            case "s":
                                return "ŝ";
                            case "u":
                                return "û";
                            case "w":
                                return "ŵ";
                            case "y":
                                return "ŷ";
                        }

                        return string.Empty;
                    case 0xc4: // Tilde
                        skipNext = @"AINOUainou".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Ã";
                            case "I":
                                return "Ĩ";
                            case "N":
                                return "Ñ";
                            case "O":
                                return "Õ";
                            case "U":
                                return "Ũ";
                            case "a":
                                return "ã";
                            case "i":
                                return "ĩ";
                            case "n":
                                return "ñ";
                            case "o":
                                return "õ";
                            case "u":
                                return "ũ";
                        }

                        return string.Empty;
                    case 0xc5: // Macron
                        skipNext = @"AEIOUaeiou".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Ā";
                            case "E":
                                return "Ē";
                            case "I":
                                return "Ī";
                            case "O":
                                return "Ō";
                            case "U":
                                return "Ū";
                            case "a":
                                return "ā";
                            case "e":
                                return "ē";
                            case "i":
                                return "ī";
                            case "o":
                                return "ō";
                            case "u":
                                return "ū";
                        }

                        return string.Empty;
                    case 0xc6: // Breve
                        skipNext = @"AGUagu".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Ă";
                            case "G":
                                return "Ğ";
                            case "U":
                                return "Ŭ";
                            case "a":
                                return "ă";
                            case "g":
                                return "ğ";
                            case "u":
                                return "ŭ";
                        }

                        return string.Empty;
                    case 0xc7: // Dot
                        skipNext = @"CEGIZcegiz".Contains(next);
                        switch (next)
                        {
                            case "C":
                                return "Ċ";
                            case "E":
                                return "Ė";
                            case "G":
                                return "Ġ";
                            case "I":
                                return "İ";
                            case "Z":
                                return "Ż";
                            case "c":
                                return "ċ";
                            case "e":
                                return "ė";
                            case "g":
                                return "ġ";
                            case "i":
                                return "ı";
                            case "z":
                                return "ż";
                        }

                        return string.Empty;
                    case 0xc8: // Umlaut or diæresis
                        skipNext = @"AEIOUYaeiouy".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Ä";
                            case "E":
                                return "Ë";
                            case "I":
                                return "Ï";
                            case "O":
                                return "Ö";
                            case "U":
                                return "Ü";
                            case "Y":
                                return "Ÿ";
                            case "a":
                                return "ä";
                            case "e":
                                return "ë";
                            case "i":
                                return "ï";
                            case "o":
                                return "ö";
                            case "u":
                                return "ü";
                            case "y":
                                return "ÿ";
                        }

                        return string.Empty;
                    case 0xca: // Ring
                        skipNext = @"AUau".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Å";
                            case "U":
                                return "Ů";
                            case "a":
                                return "å";
                            case "u":
                                return "ů";
                        }

                        return string.Empty;
                    case 0xcb: // Cedilla
                        skipNext = @"CGKLNRSTcklnrst".Contains(next);
                        switch (next)
                        {
                            case "C":
                                return "Ç";
                            case "G":
                                return "Ģ";
                            case "K":
                                return "Ķ";
                            case "L":
                                return "Ļ";
                            case "N":
                                return "Ņ";
                            case "R":
                                return "Ŗ";
                            case "S":
                                return "Ş";
                            case "T":
                                return "Ţ";
                            case "c":
                                return "ç";
                            case "k":
                                return "ķ";
                            case "l":
                                return "ļ";
                            case "n":
                                return "ņ";
                            case "r":
                                return "ŗ";
                            case "s":
                                return "ş";
                            case "t":
                                return "ţ";
                        }

                        return string.Empty;
                    case 0xcd: // DoubleAcute
                        skipNext = @"OUou".Contains(next);
                        switch (next)
                        {
                            case "O":
                                return "Ő";
                            case "U":
                                return "Ű";
                            case "o":
                                return "ő";
                            case "u":
                                return "ű";
                        }

                        return string.Empty;
                    case 0xce: // Ogonek
                        skipNext = @"AEIUaeiu".Contains(next);
                        switch (next)
                        {
                            case "A":
                                return "Ą";
                            case "E":
                                return "Ę";
                            case "I":
                                return "Į";
                            case "U":
                                return "Ų";
                            case "a":
                                return "ą";
                            case "e":
                                return "ę";
                            case "i":
                                return "į";
                            case "u":
                                return "ų";
                        }

                        return string.Empty;
                    case 0xcf: // Caron
                        skipNext = @"CDELNRSTZcdelnrstz".Contains(next);
                        switch (next)
                        {
                            case "C":
                                return "Č";
                            case "D":
                                return "Ď";
                            case "E":
                                return "Ě";
                            case "L":
                                return "Ľ";
                            case "N":
                                return "Ň";
                            case "R":
                                return "Ř";
                            case "S":
                                return "Š";
                            case "T":
                                return "Ť";
                            case "Z":
                                return "Ž";
                            case "c":
                                return "č";
                            case "d":
                                return "ď";
                            case "e":
                                return "ě";
                            case "l":
                                return "ľ";
                            case "n":
                                return "ň";
                            case "r":
                                return "ř";
                            case "s":
                                return "š";
                            case "t":
                                return "ť";
                            case "z":
                                return "ž";
                        }

                        return string.Empty;
                    default:
                        return encoding.GetString(buffer, index, 1);
                }
            }

            if (header.CharacterCodeTableNumber == "01")
            {
                // Latin/Cyrillic alphabet - from ISO 8859/5-1988
                Encoding encoding = Encoding.GetEncoding("ISO-8859-5");
                return encoding.GetString(buffer, index, 1);
            }

            if (header.CharacterCodeTableNumber == "02")
            {
                // Latin/Arabic alphabet - from ISO 8859/6-1987
                Encoding encoding = Encoding.GetEncoding("ISO-8859-6");
                return encoding.GetString(buffer, index, 1);
            }

            if (header.CharacterCodeTableNumber == "03")
            {
                // Latin/Greek alphabet - from ISO 8859/7-1987
                Encoding encoding = Encoding.GetEncoding("ISO-8859-7"); // or ISO-8859-1 ?
                return encoding.GetString(buffer, index, 1);
            }

            if (header.CharacterCodeTableNumber == "04")
            {
                // Latin/Hebrew alphabet - from ISO 8859/8-1988
                Encoding encoding = Encoding.GetEncoding("ISO-8859-8");
                return encoding.GetString(buffer, index, 1);
            }

            return string.Empty;
        }

        private IEnumerable<EbuTextTimingInformation> ReadTTI(byte[] buffer, EbuGeneralSubtitleInformation header)
        {
            const int startOfTTI = 1024;
            const int ttiSize = 128;
            const byte textFieldCRLF = 0x8A;
            const byte textFieldTerminator = 0x8F;
            const byte italicsOn = 0x80;
            const byte italicsOff = 0x81;
            const byte underlineOn = 0x82;
            const byte underlineOff = 0x83;

            List<EbuTextTimingInformation> list = new List<EbuTextTimingInformation>();
            int index = startOfTTI;
            while (index + ttiSize <= buffer.Length)
            {
                EbuTextTimingInformation tti = new EbuTextTimingInformation();

                tti.SubtitleGroupNumber = buffer[index];
                tti.SubtitleNumber = (ushort)(buffer[index + 2] * 256 + buffer[index + 1]);
                tti.ExtensionBlockNumber = buffer[index + 3];
                tti.CumulativeStatus = buffer[index + 4];

                tti.TimeCodeInHours = buffer[index + 5 + 0];
                tti.TimeCodeInMinutes = buffer[index + 5 + 1];
                tti.TimeCodeInSeconds = buffer[index + 5 + 2];
                tti.TimeCodeInMilliseconds = (int)(TimeCode.BaseUnit / (header.FrameRate / buffer[index + 5 + 3]));

                tti.TimeCodeOutHours = buffer[index + 9 + 0];
                tti.TimeCodeOutMinutes = buffer[index + 9 + 1];
                tti.TimeCodeOutSeconds = buffer[index + 9 + 2];
                tti.TimeCodeOutMilliseconds = (int)(1000 / (header.FrameRate / buffer[index + 9 + 3]));

                tti.VerticalPosition = buffer[index + 13];
                this.VerticalPositions.Add(tti.VerticalPosition);
                tti.JustificationCode = buffer[index + 14];
                this.JustificationCodes.Add(tti.JustificationCode);
                tti.CommentFlag = buffer[index + 15];

                // build text
                bool skipNext = false;
                StringBuilder sb = new StringBuilder();
                string endTags = string.Empty;
                string color = string.Empty;
                string lastColor = string.Empty;
                for (int i = 0; i < 112; i++)
                {
                    // skip fist byte (seems to be always 0xd/32/space - thx Iban)
                    byte b = buffer[index + 16 + i];
                    if (b <= 0xf && (i == 0 || i == 2 || i == 3))
                    {
                        // not used, 0=0xd, 2=0xb, 3=0xb
                    }
                    else if (skipNext)
                    {
                        skipNext = false;
                    }
                    else
                    {
                        if (b <= 0x17)
                        {
                            switch (b)
                            {
                                case 0x00:
                                case 0x10:
                                    color = "Black";
                                    break;
                                case 0x01:
                                case 0x11:
                                    color = "Red";
                                    break;
                                case 0x02:
                                case 0x12:
                                    color = "Green";
                                    break;
                                case 0x03:
                                case 0x13:
                                    color = "Yellow";
                                    break;
                                case 0x04:
                                case 0x14:
                                    color = "Blue";
                                    break;
                                case 0x05:
                                case 0x15:
                                    color = "Magenta";
                                    break;
                                case 0x06:
                                case 0x16:
                                    color = "Cyan";
                                    break;
                                case 0x07:
                                case 0x17:
                                    color = "White";
                                    break;
                            }
                        }

                        if (b == textFieldCRLF)
                        {
                            sb.AppendLine();
                        }
                        else if (b == italicsOn)
                        {
                            sb.Append("<i>");
                        }
                        else if (b == italicsOff)
                        {
                            sb.Append("</i>");
                        }
                        else if (b == underlineOn)
                        {
                            sb.Append("<u>");
                        }
                        else if (b == underlineOff)
                        {
                            sb.Append("</u>");
                        }

                        // else if (b == 0xD0) // em-dash
                        // sb.Append('–');
                        else if (b == textFieldTerminator)
                        {
                            break;
                        }
                        else if ((b >= 0x20 && b <= 0x7F) || b >= 0xA1)
                        {
                            string ch = GetCharacter(out skipNext, header, buffer, index + 16 + i);
                            if (ch != " ")
                            {
                                if (color != lastColor && color.Length > 0)
                                {
                                    endTags = "</font>";
                                    if (lastColor.Length > 0)
                                    {
                                        sb.Append("</font>");
                                    }

                                    sb.Append("<font color=\"" + color + "\">");
                                }

                                lastColor = color;
                            }

                            sb.Append(ch);
                        }
                    }
                }

                tti.TextField = sb.ToString().Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine).TrimEnd() + endTags;

                int rows;
                if (!int.TryParse(header.MaximumNumberOfDisplayableRows, out rows))
                {
                    rows = 23;
                }

                if (tti.VerticalPosition < 3)
                {
                    if (tti.JustificationCode == 1)
                    {
                        // left
                        tti.TextField = "{\\an7}" + tti.TextField;
                    }
                    else if (tti.JustificationCode == 3)
                    {
                        // right
                        tti.TextField = "{\\an9}" + tti.TextField;
                    }
                    else
                    {
                        tti.TextField = "{\\an8}" + tti.TextField;
                    }
                }
                else if (tti.VerticalPosition <= rows / 2 + 1)
                {
                    if (tti.JustificationCode == 1)
                    {
                        // left
                        tti.TextField = "{\\an4}" + tti.TextField;
                    }
                    else if (tti.JustificationCode == 3)
                    {
                        // right
                        tti.TextField = "{\\an6}" + tti.TextField;
                    }
                    else
                    {
                        tti.TextField = "{\\an5}" + tti.TextField;
                    }
                }
                else
                {
                    if (tti.JustificationCode == 1)
                    {
                        // left
                        tti.TextField = "{\\an1}" + tti.TextField;
                    }
                    else if (tti.JustificationCode == 3)
                    {
                        // right
                        tti.TextField = "{\\an3}" + tti.TextField;
                    }
                }

                index += ttiSize;
                list.Add(tti);
            }

            return list;
        }

        /// <summary>
        ///     GSI block (1024 bytes)
        /// </summary>
        public class EbuGeneralSubtitleInformation
        {
            public EbuGeneralSubtitleInformation()
            {
                this.CodePageNumber = "437";
                this.DiskFormatCode = "STL25.01";
                this.DisplayStandardCode = "0";
                this.CharacterCodeTableNumber = "00";
                this.LanguageCode = "0A";
                this.OriginalProgrammeTitle = "No Title                        ";
                this.OriginalEpisodeTitle = "                                ";
                this.TranslatedProgrammeTitle = string.Empty.PadLeft(32, ' ');
                this.TranslatedEpisodeTitle = string.Empty.PadLeft(32, ' ');
                this.TranslatorsName = string.Empty.PadLeft(32, ' ');
                this.TranslatorsContactDetails = string.Empty.PadLeft(32, ' ');
                this.SubtitleListReferenceCode = "0               ";
                this.CreationDate = "101021";
                this.RevisionDate = "101021";
                this.RevisionNumber = "01";
                this.TotalNumberOfTextAndTimingInformationBlocks = "00725";
                this.TotalNumberOfSubtitles = "00725";
                this.TotalNumberOfSubtitleGroups = "001";
                this.MaximumNumberOfDisplayableCharactersInAnyTextRow = "40";
                this.MaximumNumberOfDisplayableRows = "23";
                this.TimeCodeStatus = "1";
                this.TimeCodeStartOfProgramme = "00000000";
                this.TimeCodeFirstInCue = "00000001";
                this.TotalNumberOfDisks = "1";
                this.DiskSequenceNumber = "1";
                this.CountryOfOrigin = "USA";
                this.Publisher = string.Empty.PadLeft(32, ' ');
                this.EditorsName = string.Empty.PadLeft(32, ' ');
                this.EditorsContactDetails = string.Empty.PadLeft(32, ' ');
                this.SpareBytes = string.Empty.PadLeft(75, ' ');
                this.UserDefinedArea = string.Empty.PadLeft(576, ' ');
            }

            public string CodePageNumber { get; set; } // 0..2

            public string DiskFormatCode { get; set; } // 3..10

            public string DisplayStandardCode { get; set; } // 11

            public string CharacterCodeTableNumber { get; set; } // 12..13

            public string LanguageCode { get; set; } // 14..15

            public string OriginalProgrammeTitle { get; set; } // 16..47

            public string OriginalEpisodeTitle { get; set; }

            public string TranslatedProgrammeTitle { get; set; }

            public string TranslatedEpisodeTitle { get; set; }

            public string TranslatorsName { get; set; }

            public string TranslatorsContactDetails { get; set; }

            public string SubtitleListReferenceCode { get; set; }

            public string CreationDate { get; set; }

            public string RevisionDate { get; set; }

            public string RevisionNumber { get; set; }

            public string TotalNumberOfTextAndTimingInformationBlocks { get; set; }

            public string TotalNumberOfSubtitles { get; set; }

            public string TotalNumberOfSubtitleGroups { get; set; }

            public string MaximumNumberOfDisplayableCharactersInAnyTextRow { get; set; }

            public string MaximumNumberOfDisplayableRows { get; set; }

            public string TimeCodeStatus { get; set; }

            public string TimeCodeStartOfProgramme { get; set; }

            public string TimeCodeFirstInCue { get; set; }

            public string TotalNumberOfDisks { get; set; }

            public string DiskSequenceNumber { get; set; }

            public string CountryOfOrigin { get; set; }

            public string Publisher { get; set; }

            public string EditorsName { get; set; }

            public string EditorsContactDetails { get; set; }

            public string SpareBytes { get; set; }

            public string UserDefinedArea { get; set; }

            public double FrameRate
            {
                get
                {
                    if (this.DiskFormatCode.StartsWith("STL23", StringComparison.Ordinal))
                    {
                        return 23.0;
                    }

                    if (this.DiskFormatCode.StartsWith("STL24", StringComparison.Ordinal))
                    {
                        return 24.0;
                    }

                    if (this.DiskFormatCode.StartsWith("STL25", StringComparison.Ordinal))
                    {
                        return 25.0;
                    }

                    if (this.DiskFormatCode.StartsWith("STL29", StringComparison.Ordinal))
                    {
                        return 29.0;
                    }

                    if (this.DiskFormatCode.StartsWith("STL35", StringComparison.Ordinal))
                    {
                        return 35.0;
                    }

                    if (this.DiskFormatCode.StartsWith("STL48", StringComparison.Ordinal))
                    {
                        return 48.0;
                    }

                    if (this.DiskFormatCode.StartsWith("STL50", StringComparison.Ordinal))
                    {
                        return 50.0;
                    }

                    if (this.DiskFormatCode.StartsWith("STL60", StringComparison.Ordinal))
                    {
                        return 60.0;
                    }

                    return 30.0; // should be DiskFormatcode STL30.01
                }
            }

            public override string ToString()
            {
                string result = this.CodePageNumber + this.DiskFormatCode + this.DisplayStandardCode + this.CharacterCodeTableNumber + this.LanguageCode + this.OriginalProgrammeTitle + this.OriginalEpisodeTitle + this.TranslatedProgrammeTitle + this.TranslatedEpisodeTitle + this.TranslatorsName + this.TranslatorsContactDetails + this.SubtitleListReferenceCode + this.CreationDate + this.RevisionDate + this.RevisionNumber + this.TotalNumberOfTextAndTimingInformationBlocks + this.TotalNumberOfSubtitles + this.TotalNumberOfSubtitleGroups + this.MaximumNumberOfDisplayableCharactersInAnyTextRow + this.MaximumNumberOfDisplayableRows + this.TimeCodeStatus + this.TimeCodeStartOfProgramme + this.TimeCodeFirstInCue + this.TotalNumberOfDisks + this.DiskSequenceNumber + this.CountryOfOrigin + this.Publisher + this.EditorsName + this.EditorsContactDetails + this.SpareBytes + this.UserDefinedArea;
                if (result.Length == 1024)
                {
                    return result;
                }

                return "Length must be 1024 but is " + result.Length;
            }
        }

        /// <summary>
        ///     TTI block 128 bytes
        /// </summary>
        private class EbuTextTimingInformation
        {
            public EbuTextTimingInformation()
            {
                this.SubtitleGroupNumber = 0;
                this.ExtensionBlockNumber = 255;
                this.CumulativeStatus = 0;
                this.VerticalPosition = 0x16;
                this.JustificationCode = 2;
                this.CommentFlag = 0;
            }

            public byte SubtitleGroupNumber { get; set; }

            public ushort SubtitleNumber { get; set; }

            public byte ExtensionBlockNumber { get; set; }

            public byte CumulativeStatus { get; set; }

            public int TimeCodeInHours { get; set; }

            public int TimeCodeInMinutes { get; set; }

            public int TimeCodeInSeconds { get; set; }

            public int TimeCodeInMilliseconds { get; set; }

            public int TimeCodeOutHours { get; set; }

            public int TimeCodeOutMinutes { get; set; }

            public int TimeCodeOutSeconds { get; set; }

            public int TimeCodeOutMilliseconds { get; set; }

            public byte VerticalPosition { get; set; }

            public byte JustificationCode { get; set; }

            public byte CommentFlag { get; set; }

            public string TextField { get; set; }

            public static byte GetFrameFromMilliseconds(int milliseconds, double frameRate)
            {
                int frame = (int)(milliseconds / (TimeCode.BaseUnit / frameRate));
                return (byte)frame;
            }

            public byte[] GetBytes(EbuGeneralSubtitleInformation header)
            {
                byte[] buffer = new byte[128]; // Text and Timing Information (TTI) block consists of 128 bytes

                buffer[0] = this.SubtitleGroupNumber;
                byte[] temp = BitConverter.GetBytes(this.SubtitleNumber);
                buffer[1] = temp[0];
                buffer[2] = temp[1];
                buffer[3] = this.ExtensionBlockNumber;
                buffer[4] = this.CumulativeStatus;

                buffer[5] = (byte)this.TimeCodeInHours;
                buffer[6] = (byte)this.TimeCodeInMinutes;
                buffer[7] = (byte)this.TimeCodeInSeconds;
                buffer[8] = GetFrameFromMilliseconds(this.TimeCodeInMilliseconds, header.FrameRate);

                buffer[9] = (byte)this.TimeCodeOutHours;
                buffer[10] = (byte)this.TimeCodeOutMinutes;
                buffer[11] = (byte)this.TimeCodeOutSeconds;
                buffer[12] = GetFrameFromMilliseconds(this.TimeCodeOutMilliseconds, header.FrameRate);

                buffer[13] = this.VerticalPosition;
                buffer[14] = this.JustificationCode;
                buffer[15] = this.CommentFlag;

                Encoding encoding = Encoding.Default;
                if (header.CharacterCodeTableNumber == "00")
                {
                    encoding = Encoding.GetEncoding(20269);

                    // 0xC1—0xCF combines characters - http://en.wikipedia.org/wiki/ISO/IEC_6937
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc1 }), "ÀÈÌÒÙàèìòù", "AEIOUaeiou");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc2 }), "ÁĆÉÍĹŃÓŔŚÚÝŹáćéģíĺńóŕśúýź", "ACEILNORSUYZacegilnorsuyz");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc3 }), "ÂĈÊĜĤÎĴÔŜÛŴŶâĉêĝĥîĵôŝûŵŷ", "ACEGHIJOSUWYaceghijosuwy");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc4 }), "ÃĨÑÕŨãĩñõũ", "AINOUainou");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc5 }), "ĀĒĪŌŪāēīōū", "AEIOUaeiou");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc6 }), "ĂĞŬăğŭ", "AGUagu");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc7 }), "ĊĖĠİŻċėġıż", "CEGIZcegiz");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xc8 }), "ÄËÏÖÜŸäëïöüÿ", "AEIOUYaeiouy");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xca }), "ÅŮåů", "AUau");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xcb }), "ÇĢĶĻŅŖŞŢçķļņŗşţ", "CGKLNRSTcklnrst");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xcd }), "ŐŰőű", "OUou");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xce }), "ĄĘĮŲąęįų", "AEIUaeiu");
                    this.TextField = ReplaceSpecialCharactersWithTwoByteEncoding(this.TextField, encoding.GetString(new byte[] { 0xcf }), "ČĎĚĽŇŘŠŤŽčďěľňřšťž", "CDELNRSTZcdelnrstz");
                }
                else if (header.CharacterCodeTableNumber == "01")
                {
                    // Latin/Cyrillic alphabet - from ISO 8859/5-1988
                    encoding = Encoding.GetEncoding("ISO-8859-5");
                }
                else if (header.CharacterCodeTableNumber == "02")
                {
                    // Latin/Arabic alphabet - from ISO 8859/6-1987
                    encoding = Encoding.GetEncoding("ISO-8859-6");
                }
                else if (header.CharacterCodeTableNumber == "03")
                {
                    // Latin/Greek alphabet - from ISO 8859/7-1987
                    encoding = Encoding.GetEncoding("ISO-8859-7"); // or ISO-8859-1 ?
                }
                else if (header.CharacterCodeTableNumber == "04")
                {
                    // Latin/Hebrew alphabet - from ISO 8859/8-1988
                    encoding = Encoding.GetEncoding("ISO-8859-8");
                }

                // italic/underline
                string italicsOn = encoding.GetString(new byte[] { 0x80 });
                string italicsOff = encoding.GetString(new byte[] { 0x81 });
                string underlineOn = encoding.GetString(new byte[] { 0x82 });
                string underlineOff = encoding.GetString(new byte[] { 0x83 });
                if (Utilities.CountTagInText(this.TextField, "<i>") == 1 && this.TextField.StartsWith("<i>") && this.TextField.EndsWith("</i>"))
                {
                    // italic on all lines
                    this.TextField = this.TextField.Replace(Environment.NewLine, Environment.NewLine + "<i>");
                }

                this.TextField = this.TextField.Replace("<i>", italicsOn);
                this.TextField = this.TextField.Replace("<I>", italicsOn);
                this.TextField = this.TextField.Replace("</i>", italicsOff);
                this.TextField = this.TextField.Replace("</I>", italicsOff);
                this.TextField = this.TextField.Replace("<u>", underlineOn);
                this.TextField = this.TextField.Replace("<U>", underlineOn);
                this.TextField = this.TextField.Replace("</u>", underlineOff);
                this.TextField = this.TextField.Replace("</U>", underlineOff);

                // em-dash (–) tags
                // TextField = TextField.Replace("–", "Ð");

                // font tags
                if (header.DisplayStandardCode == "0")
                {
                    // Open subtitling
                    this.TextField = HtmlUtil.RemoveHtmlTags(this.TextField, true);
                }
                else
                {
                    // teletext
                    string[] lines = this.TextField.SplitToLines();
                    StringBuilder sb = new StringBuilder();
                    string veryFirstColor = null;
                    foreach (string line in lines)
                    {
                        string firstColor = null;
                        string s = line;
                        int start = s.IndexOf("<font ", StringComparison.Ordinal);
                        if (start >= 0)
                        {
                            int end = s.IndexOf('>', start);
                            if (end > 0)
                            {
                                string f = s.Substring(start, end - start);
                                if (f.Contains(" color="))
                                {
                                    int colorStart = f.IndexOf(" color=", StringComparison.Ordinal);
                                    if (s.IndexOf('"', colorStart + " color=".Length + 1) > 0)
                                    {
                                        int colorEnd = f.IndexOf('"', colorStart + " color=".Length + 1);
                                        if (colorStart > 1)
                                        {
                                            string color = f.Substring(colorStart + 7, colorEnd - (colorStart + 7));
                                            color = color.Trim('\'');
                                            color = color.Trim('\"');
                                            color = color.Trim('#');

                                            s = s.Remove(start, end - start + 1);
                                            if (veryFirstColor == null)
                                            {
                                                veryFirstColor = GetNearestEbuColorCode(color, encoding);
                                            }

                                            firstColor = GetNearestEbuColorCode(color, encoding);
                                        }
                                    }
                                }
                            }
                        }

                        // byte colorByte = 0x07; // white
                        byte colorByte = 255;
                        if (!string.IsNullOrEmpty(veryFirstColor))
                        {
                            colorByte = encoding.GetBytes(veryFirstColor)[0];
                        }

                        if (!string.IsNullOrEmpty(firstColor))
                        {
                            colorByte = encoding.GetBytes(firstColor)[0];
                        }

                        string prefix = encoding.GetString(new byte[] { 0xd, colorByte, 0xb, 0xb });

                        if (colorByte != 255)
                        {
                            sb.Append(prefix);
                        }

                        sb.AppendLine(s);
                    }

                    this.TextField = HtmlUtil.RemoveHtmlTags(sb.ToString()).TrimEnd();
                }

                // newline
                string newline = encoding.GetString(new byte[] { 0x0a, 0x0a, 0x8a, 0x8a });

                if (header.DisplayStandardCode == "0")
                {
                    newline = encoding.GetString(new byte[] { 0x8A }); // 8Ah=CR/LF
                }

                // newline = encoding.GetString(new byte[] { 0x85, 0x8A, 0x0D, 0x84, 0x80 }); //85h=boxing off, 8Ah=CR/LF, 84h=boxing on, 80h, Italics on
                this.TextField = this.TextField.Replace(Environment.NewLine, newline);

                string endOfLine = encoding.GetString(new byte[] { 0x0a, 0x0a, 0x8a });

                if (header.DisplayStandardCode == "0")
                {
                    endOfLine = string.Empty;
                }

                this.TextField += endOfLine;

                // save em-dash indexes (–)
                List<int> indexOfEmdash = new List<int>();
                for (int j = 0; j < this.TextField.Length; j++)
                {
                    if (this.TextField[j] == '–')
                    {
                        indexOfEmdash.Add(j);
                    }
                }

                // convert text to bytes
                byte[] bytes = encoding.GetBytes(this.TextField);

                // restore em-dashes (–)
                foreach (int index in indexOfEmdash)
                {
                    bytes[index] = 0xd0;
                }

                for (int i = 0; i < 112; i++)
                {
                    if (i < bytes.Length)
                    {
                        buffer[16 + i] = bytes[i];
                    }

                    // else if (i == bytes.Length)
                    // buffer[16 + i] = 0x8f;
                    else
                    {
                        buffer[16 + i] = 0x8f;
                    }
                }

                return buffer;
            }

            private static string GetNearestEbuColorCode(string color, Encoding encoding)
            {
                color = color.ToLower();
                if (color == "black" || color == "000000")
                {
                    return encoding.GetString(new byte[] { 0x00 }); // black
                }

                if (color == "red" || color == "ff0000")
                {
                    return encoding.GetString(new byte[] { 0x01 }); // red
                }

                if (color == "green" || color == "00ff00")
                {
                    return encoding.GetString(new byte[] { 0x02 }); // green
                }

                if (color == "yellow" || color == "ffff00")
                {
                    return encoding.GetString(new byte[] { 0x03 }); // yellow
                }

                if (color == "blue" || color == "0000ff")
                {
                    return encoding.GetString(new byte[] { 0x04 }); // blue
                }

                if (color == "magenta" || color == "ff00ff")
                {
                    return encoding.GetString(new byte[] { 0x05 }); // magenta
                }

                if (color == "cyan" || color == "00ffff")
                {
                    return encoding.GetString(new byte[] { 0x06 }); // cyan
                }

                if (color == "white" || color == "ffffff")
                {
                    return encoding.GetString(new byte[] { 0x07 }); // white
                }

                if (color.Length == 6)
                {
                    if (RegExpr.IsMatch(color))
                    {
                        const int maxDiff = 130;
                        int r = int.Parse(color.Substring(0, 2), NumberStyles.HexNumber);
                        int g = int.Parse(color.Substring(2, 2), NumberStyles.HexNumber);
                        int b = int.Parse(color.Substring(4, 2), NumberStyles.HexNumber);
                        if (r < maxDiff && g < maxDiff && b < maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x00 }); // black
                        }

                        if (r > 255 - maxDiff && g < maxDiff && b < maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x01 }); // red
                        }

                        if (r < maxDiff && g > 255 - maxDiff && b < maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x02 }); // green
                        }

                        if (r > 255 - maxDiff && g > 255 - maxDiff && b < maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x03 }); // yellow
                        }

                        if (r < maxDiff && g < maxDiff && b > 255 - maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x04 }); // blue
                        }

                        if (r > 255 - maxDiff && g < maxDiff && b > 255 - maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x05 }); // magenta
                        }

                        if (r < maxDiff && g > 255 - maxDiff && b > 255 - maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x06 }); // cyan
                        }

                        if (r > 255 - maxDiff && g > 255 - maxDiff && b > 255 - maxDiff)
                        {
                            return encoding.GetString(new byte[] { 0x07 }); // white
                        }
                    }
                }

                return string.Empty;
            }

            private static string ReplaceSpecialCharactersWithTwoByteEncoding(string text, string specialCharacter, string originalCharacters, string newCharacters)
            {
                if (originalCharacters.Length != newCharacters.Length)
                {
                    throw new ArgumentException("originalCharacters and newCharacters must have equal length");
                }

                for (int i = 0; i < newCharacters.Length; i++)
                {
                    text = text.Replace(originalCharacters[i].ToString(), specialCharacter + newCharacters[i]);
                }

                return text;
            }
        }
    }
}