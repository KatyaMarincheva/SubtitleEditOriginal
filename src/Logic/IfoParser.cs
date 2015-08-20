// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IfoParser.cs" company="">
//   
// </copyright>
// <summary>
//   The ifo parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The ifo parser.
    /// </summary>
    public class IfoParser : IDisposable
    {
        /// <summary>
        /// The array of language code.
        /// </summary>
        public static List<string> ArrayOfLanguageCode = new List<string> { "  ", "aa", "ab", "af", "am", "ar", "as", "ay", "az", "ba", "be", "bg", "bh", "bi", "bn", "bo", "br", "ca", "co", "cs", "cy", "da", "de", "dz", "el", "en", "eo", "es", "et", "eu", "fa", "fi", "fj", "fo", "fr", "fy", "ga", "gd", "gl", "gn", "gu", "ha", "he", "hi", "hr", "hu", "hy", "ia", "id", "ie", "ik", "in", "is", "it", "iu", "iw", "ja", "ji", "jw", "ka", "kk", "kl", "km", "kn", "ko", "ks", "ku", "ky", "la", "ln", "lo", "lt", "lv", "mg", "mi", "mk", "ml", "mn", "mo", "mr", "ms", "mt", "my", "na", "ne", "nl", "no", "oc", "om", "or", "pa", "pl", "ps", "pt", "qu", "rm", "rn", "ro", "ru", "rw", "sa", "sd", "sg", "sh", "si", "sk", "sl", "sm", "sn", "so", "sq", "sr", "ss", "st", "su", "sv", "sw", "ta", "te", "tg", "th", "ti", "tk", "tl", "tn", "to", "tr", "ts", "tt", "tw", "ug", "uk", "ur", "uz", "vi", "vo", "wo", "xh", "yi", "yo", "za", "zh", "zu", string.Empty };

        /// <summary>
        /// The array of language.
        /// </summary>
        public static List<string> ArrayOfLanguage = new List<string> { "Not Specified", "Afar", "Abkhazian", "Afrikaans", "Amharic", "Arabic", "Assamese", "Aymara", "Azerbaijani", "Bashkir", "Byelorussian", "Bulgarian", "Bihari", "Bislama", "Bengali; Bangla", "Tibetan", "Breton", "Catalan", "Corsican", "Czech(Ceske)", "Welsh", "Dansk", "Deutsch", "Bhutani", "Greek", "English", "Esperanto", "Espanol", "Estonian", "Basque", "Persian", "Suomi", "Fiji", "Faroese", "Français", "Frisian", "Irish", "Scots Gaelic", "Galician", "Guarani", "Gujarati", "Hausa", "Hebrew", "Hindi", "Hrvatski", "Magyar", "Armenian", "Interlingua", "Indonesian", "Interlingue", "Inupiak", "Indonesian", "Islenska", "Italiano", "Inuktitut", "Hebrew", "Japanese", "Yiddish", "Javanese", "Georgian", "Kazakh", "Greenlandic", "Cambodian", "Kannada", "Korean", "Kashmiri", "Kurdish", "Kirghiz", "Latin", "Lingala", "Laothian", "Lithuanian", "Latvian, Lettish", "Malagasy", "Maori", "Macedonian", "Malayalam", "Mongolian", "Moldavian", "Marathi", "Malay", "Maltese", "Burmese", "Nauru", "Nepali", "Nederlands", "Norsk", "Occitan", "(Afan) Oromo", "Oriya", "Punjabi", "Polish", "Pashto, Pushto", "Portugues", "Quechua", "Rhaeto-Romance", "Kirundi", "Romanian", "Russian", "Kinyarwanda", "Sanskrit", "Sindhi", "Sangho", "Serbo-Croatian", "Sinhalese", "Slovak", "Slovenian", "Samoan", "Shona", "Somali", "Albanian", "Serbian", "Siswati", "Sesotho", "Sundanese", "Svenska", "Swahili", "Tamil", "Telugu", "Tajik", "Thai", "Tigrinya", "Turkmen", "Tagalog", "Setswana", "Tonga", "Turkish", "Tsonga", "Tatar", "Twi", "Uighur", "Ukrainian", "Urdu", "Uzbek", "Vietnamese", "Volapuk", "Wolof", "Xhosa", "Yiddish", "Yoruba", "Zhuang", "Chinese", "Zulu", "???" };

        /// <summary>
        /// The _array of aspect.
        /// </summary>
        private readonly List<string> _arrayOfAspect = new List<string> { "4:3", "...", "...", "16:9" };

        /// <summary>
        /// The _array of audio extension.
        /// </summary>
        private readonly List<string> _arrayOfAudioExtension = new List<string> { "unspecified", "normal", "for visually impaired", "director's comments", "alternate director's comments" };

        /// <summary>
        /// The _array of audio mode.
        /// </summary>
        private readonly List<string> _arrayOfAudioMode = new List<string> { "AC3", "...", "MPEG1", "MPEG2", "LPCM", "...", "DTS" };

        /// <summary>
        /// The _array of coding mode.
        /// </summary>
        private readonly List<string> _arrayOfCodingMode = new List<string> { "MPEG1", "MPEG2" };

        /// <summary>
        /// The _array of ntsc resolution.
        /// </summary>
        private readonly List<string> _arrayOfNtscResolution = new List<string> { "720x480", "704x480", "352x480", "352x240" };

        /// <summary>
        /// The _array of pal resolution.
        /// </summary>
        private readonly List<string> _arrayOfPalResolution = new List<string> { "720x576", "704x576", "352x576", "352x288" };

        /// <summary>
        /// The _array of standard.
        /// </summary>
        private readonly List<string> _arrayOfStandard = new List<string> { "NTSC", "PAL", "...", "..." };

        /// <summary>
        /// The _vts pgci.
        /// </summary>
        private readonly VtsPgci _vtsPgci = new VtsPgci();

        /// <summary>
        /// The _vts vobs.
        /// </summary>
        private readonly VtsVobs _vtsVobs = new VtsVobs();

        /// <summary>
        /// The _fs.
        /// </summary>
        private FileStream _fs;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfoParser"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public IfoParser(string fileName)
        {
            try
            {
                this._fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                var buffer = new byte[12];
                this._fs.Position = 0;
                this._fs.Read(buffer, 0, 12);
                string id = Encoding.UTF8.GetString(buffer);
                if (id != "DVDVIDEO-VTS")
                {
                    this.ErrorMessage = string.Format(Configuration.Settings.Language.DvdSubRip.WrongIfoType, id, Environment.NewLine, fileName);
                    return;
                }

                this.ParseVtsVobs();
                this.ParseVtsPgci();
                this._fs.Close();
            }
            catch (Exception exception)
            {
                this.ErrorMessage = exception.Message + Environment.NewLine + exception.StackTrace;
            }
        }

        /// <summary>
        /// Gets the video title set program chain table.
        /// </summary>
        public VtsPgci VideoTitleSetProgramChainTable
        {
            get
            {
                return this._vtsPgci;
            }
        }

        /// <summary>
        /// Gets the video title set vobs.
        /// </summary>
        public VtsVobs VideoTitleSetVobs
        {
            get
            {
                return this._vtsVobs;
            }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The parse vts vobs.
        /// </summary>
        private void ParseVtsVobs()
        {
            var buffer = new byte[16];

            // retrieve video info
            this._fs.Position = 0x200;
            var data = IntToBin(this.GetEndian(2), 16);
            this._vtsVobs.VideoStream.CodingMode = this._arrayOfCodingMode[BinToInt(MidStr(data, 0, 2))];
            this._vtsVobs.VideoStream.Standard = this._arrayOfStandard[BinToInt(MidStr(data, 2, 2))];
            this._vtsVobs.VideoStream.Aspect = this._arrayOfAspect[BinToInt(MidStr(data, 4, 2))];
            if (this._vtsVobs.VideoStream.Standard == "PAL")
            {
                this._vtsVobs.VideoStream.Resolution = this._arrayOfPalResolution[BinToInt(MidStr(data, 13, 2))];
            }
            else if (this._vtsVobs.VideoStream.Standard == "NTSC")
            {
                this._vtsVobs.VideoStream.Resolution = this._arrayOfNtscResolution[BinToInt(MidStr(data, 13, 2))];
            }

            // retrieve audio info
            this._fs.Position = 0x202; // useless but here for readability
            this._vtsVobs.NumberOfAudioStreams = this.GetEndian(2);
            for (int i = 0; i < this._vtsVobs.NumberOfAudioStreams; i++)
            {
                var audioStream = new AudioStream();
                data = IntToBin(this.GetEndian(2), 16);
                audioStream.LanguageTypeSpecified = Convert.ToInt32(MidStr(data, 4, 2));
                audioStream.CodingMode = this._arrayOfAudioMode[BinToInt(MidStr(data, 0, 3))];
                audioStream.Channels = BinToInt(MidStr(data, 13, 3)) + 1;
                this._fs.Read(buffer, 0, 2);
                audioStream.LanguageCode = new string(new[] { Convert.ToChar(buffer[0]), Convert.ToChar(buffer[1]) });
                if (ArrayOfLanguageCode.Contains(audioStream.LanguageCode))
                {
                    audioStream.Language = ArrayOfLanguage[ArrayOfLanguageCode.IndexOf(audioStream.LanguageCode)];
                }

                this._fs.Seek(1, SeekOrigin.Current);
                audioStream.Extension = this._arrayOfAudioExtension[this._fs.ReadByte()];
                this._fs.Seek(2, SeekOrigin.Current);
                this._vtsVobs.AudioStreams.Add(audioStream);
            }

            // retrieve subs info (only name)
            this._fs.Position = 0x254;
            this._vtsVobs.NumberOfSubtitles = this.GetEndian(2);
            this._fs.Position += 2;
            for (int i = 0; i < this._vtsVobs.NumberOfSubtitles; i++)
            {
                this._fs.Read(buffer, 0, 2);
                var languageTwoLetter = new string(new[] { Convert.ToChar(buffer[0]), Convert.ToChar(buffer[1]) });
                this._vtsVobs.Subtitles.Add(InterpretLanguageCode(languageTwoLetter));
                this._fs.Read(buffer, 0, 2); // reserved for language code extension + code extension

                // switch (buffer[0])      // 4, 8, 10-12 unused
                // {
                // // http://dvd.sourceforge.net/dvdinfo/sprm.html
                // case 1: subtitleFormat = "(caption/normal size char)"; break; //0 = unspecified caption
                // case 2: subtitleFormat = "(caption/large size char)"; break;
                // case 3: subtitleFormat = "(caption for children)"; break;
                // case 5: subtitleFormat = "(closed caption/normal size char)"; break;
                // case 6: subtitleFormat = "(closed caption/large size char)"; break;
                // case 7: subtitleFormat = "(closed caption for children)"; break;
                // case 9: subtitleFormat = "(forced caption)"; break;
                // case 13: subtitleFormat = "(director comments/normal size char)"; break;
                // case 14: subtitleFormat = "(director comments/large size char)"; break;
                // case 15: subtitleFormat = "(director comments for children)"; break;
                // }
                this._fs.Position += 2;
            }
        }

        /// <summary>
        /// The bin to int.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int BinToInt(string p)
        {
            return Convert.ToInt32(p, 2);
        }

        /// <summary>
        /// The mid str.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MidStr(string data, int start, int count)
        {
            return data.Substring(start, count);
        }

        /// <summary>
        /// The int to bin.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="digits">
        /// The digits.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string IntToBin(int value, int digits)
        {
            string result = Convert.ToString(value, 2);
            while (result.Length < digits)
            {
                result = "0" + result;
            }

            return result;
        }

        /// <summary>
        /// The get endian.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetEndian(int count)
        {
            int result = 0;
            for (int i = count; i > 0; i--)
            {
                int b = this._fs.ReadByte();
                result = (result << 8) + b;
            }

            return result;
        }

        /// <summary>
        /// The interpret language code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string InterpretLanguageCode(string code)
        {
            int i = 0;
            while (ArrayOfLanguageCode[i] != code && i < 143)
            {
                i++;
            }

            return ArrayOfLanguage[i];
        }

        /// <summary>
        /// The parse vts pgci.
        /// </summary>
        private void ParseVtsPgci()
        {
            const int sectorSize = 2048;

            this._fs.Position = 0xCC; // Get VTS_PGCI adress
            int tableStart = sectorSize * this.GetEndian(4);

            this._fs.Position = tableStart;
            this._vtsPgci.NumberOfProgramChains = this.GetEndian(2);
            this._vtsPgci.ProgramChains = new List<ProgramChain>();

            for (int i = 0; i < this._vtsPgci.NumberOfProgramChains; i++)
            {
                // Parse PGC Header
                var programChain = new ProgramChain();
                this._fs.Position = tableStart + 4 + 8 * (i + 1); // Get PGC adress
                int programChainAdress = this.GetEndian(4);
                this._fs.Position = tableStart + programChainAdress + 2; // Move to PGC
                programChain.NumberOfPgc = this._fs.ReadByte();
                programChain.NumberOfCells = this._fs.ReadByte();
                programChain.PlaybackTime = InterpretTime(this.GetEndian(4));
                this._fs.Seek(4, SeekOrigin.Current);

                // check if audio streams are available for this PGC
                this._fs.Position = tableStart + programChainAdress + 0xC;
                for (int j = 0; j < this._vtsVobs.NumberOfAudioStreams; j++)
                {
                    string temp = IntToBin(this._fs.ReadByte(), 8);
                    programChain.AudioStreamsAvailable.Add(temp[0]);
                    this._fs.Seek(1, SeekOrigin.Current);
                }

                // check if subtitles are available for this PGC
                this._fs.Position = tableStart + programChainAdress + 0x1C;
                for (int j = 0; j < this._vtsVobs.NumberOfSubtitles; j++)
                {
                    // read and save full subpicture stream info inside program chain
                    var subtitle = new byte[4];
                    this._fs.Read(subtitle, 0, 4);
                    programChain.SubtitlesAvailable.Add(subtitle);
                }

                this.CalculateSubtitleTypes(programChain);

                // Parse Color LookUp Table (CLUT) - offset 00A4, 16*4 (0, Y, Cr, Cb)
                this._fs.Position = tableStart + programChainAdress + 0xA4;
                for (int colorNumber = 0; colorNumber < 16; colorNumber++)
                {
                    var colors = new byte[4];
                    this._fs.Read(colors, 0, 4);
                    int y = colors[1] - 16;
                    int cr = colors[2] - 128;
                    int cb = colors[3] - 128;
                    int r = (int)Math.Min(Math.Max(Math.Round(1.1644F * y + 1.596F * cr), 0), 255);
                    int g = (int)Math.Min(Math.Max(Math.Round(1.1644F * y - 0.813F * cr - 0.391F * cb), 0), 255);
                    int b = (int)Math.Min(Math.Max(Math.Round(1.1644F * y + 2.018F * cb), 0), 255);

                    programChain.ColorLookupTable.Add(Color.FromArgb(r, g, b));
                }

                // Parse Program Map
                this._fs.Position = tableStart + programChainAdress + 0xE6;
                this._fs.Position = tableStart + programChainAdress + this.GetEndian(2);
                for (int j = 0; j < programChain.NumberOfPgc; j++)
                {
                    programChain.PgcEntryCells.Add((byte)this._fs.ReadByte());
                }

                // Cell Playback Info Table to retrieve duration
                this._fs.Position = tableStart + programChainAdress + 0xE8;
                this._fs.Position = tableStart + programChainAdress + this.GetEndian(2);
                var timeArray = new List<int>();
                for (int k = 0; k < programChain.NumberOfPgc; k++)
                {
                    int time = 0;
                    int max;
                    if (k == programChain.NumberOfPgc - 1)
                    {
                        max = programChain.NumberOfCells;
                    }
                    else
                    {
                        max = programChain.PgcEntryCells[k + 1] - 1;
                    }

                    for (int j = programChain.PgcEntryCells[k]; j <= max; j++)
                    {
                        this._fs.Seek(4, SeekOrigin.Current);
                        time += TimeToMs(this.GetEndian(4));
                        this._fs.Seek(16, SeekOrigin.Current);
                    }

                    programChain.PgcPlaybackTimes.Add(MsToTime(time));
                    timeArray.Add(time);

                    // convert to start time
                    time = 0;
                    for (int l = 1; l <= k; l++)
                    {
                        time += timeArray[l - 1];
                    }

                    if (k == 0)
                    {
                        programChain.PgcStartTimes.Add(MsToTime(0));
                    }

                    if (k > 0)
                    {
                        programChain.PgcStartTimes.Add(MsToTime(time));
                    }
                }

                this._vtsPgci.ProgramChains.Add(programChain);
            }
        }

        /// <summary>
        /// The calculate subtitle types.
        /// </summary>
        /// <param name="programChain">
        /// The program chain.
        /// </param>
        private void CalculateSubtitleTypes(ProgramChain programChain)
        {
            // Additional Code to analyse stream bytes
            if (this._vtsVobs.NumberOfSubtitles > 0)
            {
                // load the 'last' subpicture stream info,
                // because if we have more than one subtitle stream,
                // all subtitle positions > 0
                // lastSubtitle[0] is related to 4:3
                // lastSubtitle[1] is related to Wide
                // lastSubtitle[2] is related to letterboxed
                // lastSubtitle[3] is related to pan&scan
                byte[] lastSubtitle = programChain.SubtitlesAvailable[programChain.SubtitlesAvailable.Count - 1];

                int countSubs = 0;

                // set defaults for all possible subpicture types and positions
                programChain.Has43Subs = false;
                programChain.HasWideSubs = false;
                programChain.HasLetterSubs = false;
                programChain.HasPanSubs = false;
                programChain.HasNoSpecificSubs = true;

                int pos43Subs = -1;
                int posWideSubs = -1;
                int posLetterSubs = -1;
                int posPanSubs = -1;

                // parse different subtitle bytes
                if (lastSubtitle[0] > 0x80)
                {
                    programChain.Has43Subs = true;
                    countSubs++; // 4:3
                }

                if (lastSubtitle[1] > 0)
                {
                    programChain.HasWideSubs = true;
                    countSubs++; // wide
                }

                if (lastSubtitle[2] > 0)
                {
                    programChain.HasLetterSubs = true;
                    countSubs++; // letterboxed
                }

                if (lastSubtitle[3] > 0)
                {
                    programChain.HasPanSubs = true;
                    countSubs++; // pan&scan
                }

                if (countSubs == 0)
                {
                    // may be, only a 4:3 stream exists
                    // -> lastSubtitle[0] = 0x80
                }
                else
                {
                    if (this._vtsVobs.NumberOfSubtitles == 1)
                    {
                        // only 1 stream exists, may be letterboxed
                        // if so we cound't find wide id, because lastSubtitle[1] = 0 !!
                        // corresponding wide stream byte is 0 => wide id = 0x20
                        // letterboxed = 0x21
                        if (programChain.HasLetterSubs && !programChain.HasWideSubs)
                        {
                            // repair it
                            programChain.HasWideSubs = true;
                        }
                    }

                    programChain.HasNoSpecificSubs = false;
                }

                // subpucture streams start with 0x20
                int subStream = 0x20;

                // Now we know all about available subpicture streams, including position type
                // And we can create whole complete definitions for all avalable streams
                foreach (byte[] subtitle in programChain.SubtitlesAvailable)
                {
                    if (programChain.HasNoSpecificSubs)
                    {
                        // only one unspezified subpicture stream exists
                        this._vtsVobs.SubtitleIDs.Add(string.Format("0x{0:x2}", subStream++));
                        this._vtsVobs.SubtitleTypes.Add("unspecific");
                    }
                    else
                    {
                        // read stream position for evey subtitle type from subtitle byte
                        if (programChain.Has43Subs)
                        {
                            pos43Subs = subtitle[0] - 0x80;
                        }

                        if (programChain.HasWideSubs)
                        {
                            posWideSubs = subtitle[1];
                        }

                        if (programChain.HasLetterSubs)
                        {
                            posLetterSubs = subtitle[2];
                        }

                        if (programChain.HasPanSubs)
                        {
                            posPanSubs = subtitle[3];
                        }

                        // Now we can create subpicture id's and types for every stream
                        // All used subpicture id's and types will beappended to string, separated by colon
                        // So it's possible to split it later
                        string sub = string.Empty;
                        string subType = string.Empty;
                        if (programChain.Has43Subs)
                        {
                            sub = string.Format("0x{0:x2}", subStream + pos43Subs);
                            subType = "4:3";
                        }

                        if (programChain.HasWideSubs)
                        {
                            if (sub.Length > 0)
                            {
                                sub += ", ";
                                subType += ", ";
                            }

                            sub += string.Format("0x{0:x2}", subStream + posWideSubs);
                            subType += "wide";
                        }

                        if (programChain.HasLetterSubs)
                        {
                            if (sub.Length > 0)
                            {
                                sub += ", ";
                                subType += ", ";
                            }

                            sub += string.Format("0x{0:x2}", subStream + posLetterSubs);
                            subType += "letterboxed";
                        }

                        if (programChain.HasPanSubs)
                        {
                            if (sub.Length > 0)
                            {
                                sub += ", ";
                                subType += ", ";
                            }

                            sub += string.Format("0x{0:x2}", subStream + posPanSubs);
                            subType += "pan&scan";
                        }

                        this._vtsVobs.SubtitleIDs.Add(sub);
                        this._vtsVobs.SubtitleTypes.Add(subType);
                    }
                }
            }
        }

        /// <summary>
        /// The time to ms.
        /// </summary>
        /// <param name="time">
        /// The time.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int TimeToMs(int time)
        {
            double fps;

            var temp = IntToBin(time, 32);
            var result = StrToInt(IntToHex(BinToInt(MidStr(temp, 0, 8)), 1)) * 3600000;
            result = result + StrToInt(IntToHex(BinToInt(MidStr(temp, 8, 8)), 2)) * 60000;
            result = result + StrToInt(IntToHex(BinToInt(MidStr(temp, 16, 8)), 2)) * 1000;
            if (temp.Substring(24, 2) == "11")
            {
                fps = 30;
            }
            else
            {
                fps = 25;
            }

            result += (int)Math.Round((TimeCode.BaseUnit / fps) * StrToFloat(IntToHex(BinToInt(MidStr(temp, 26, 6)), 3)));
            return result;
        }

        /// <summary>
        /// The str to float.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double StrToFloat(string p)
        {
            return Convert.ToDouble(p, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The str to int.
        /// </summary>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int StrToInt(string p)
        {
            return int.Parse(p);
        }

        /// <summary>
        /// The int to hex.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="digits">
        /// The digits.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string IntToHex(int value, int digits)
        {
            string hex = value.ToString("X");

            return hex.PadLeft(digits, '0');
        }

        /// <summary>
        /// The ms to time.
        /// </summary>
        /// <param name="milliseconds">
        /// The milliseconds.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string MsToTime(double milliseconds)
        {
            var ts = TimeSpan.FromMilliseconds(milliseconds);
            string s = string.Format("{0:#0}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            return s;
        }

        /// <summary>
        /// The interpret time.
        /// </summary>
        /// <param name="timeNumber">
        /// The time number.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string InterpretTime(int timeNumber)
        {
            string timeBytes = IntToBin(timeNumber, 32);
            int h = StrToInt(IntToHex(BinToInt(timeBytes.Substring(0, 8)), 1));
            int m = StrToInt(IntToHex(BinToInt(timeBytes.Substring(8, 8)), 2));
            int s = StrToInt(IntToHex(BinToInt(timeBytes.Substring(16, 8)), 2));
            int fps = 25;
            if (timeBytes.Substring(24, 2) == "11")
            {
                fps = 30;
            }

            int milliseconds = (int)Math.Round((TimeCode.BaseUnit / fps) * StrToFloat(IntToHex(BinToInt(timeBytes.Substring(26, 6)), 3)));
            var ts = new TimeSpan(0, h, m, s, milliseconds);
            return MsToTime(ts.TotalMilliseconds);
        }

        /// <summary>
        /// The release managed resources.
        /// </summary>
        private void ReleaseManagedResources()
        {
            if (this._fs != null)
            {
                this._fs.Dispose();
                this._fs = null;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ReleaseManagedResources();
            }
        }

        /// <summary>
        /// The audio stream.
        /// </summary>
        public struct AudioStream
        {
            /// <summary>
            /// The channels.
            /// </summary>
            public int Channels;

            /// <summary>
            /// The coding mode.
            /// </summary>
            public string CodingMode;

            /// <summary>
            /// The extension.
            /// </summary>
            public string Extension;

            /// <summary>
            /// The language.
            /// </summary>
            public string Language;

            /// <summary>
            /// The language code.
            /// </summary>
            public string LanguageCode;

            /// <summary>
            /// The language type specified.
            /// </summary>
            public int LanguageTypeSpecified;
        };

        /// <summary>
        /// The video stream.
        /// </summary>
        public struct VideoStream
        {
            /// <summary>
            /// The aspect.
            /// </summary>
            public string Aspect;

            /// <summary>
            /// The coding mode.
            /// </summary>
            public string CodingMode;

            /// <summary>
            /// The resolution.
            /// </summary>
            public string Resolution;

            /// <summary>
            /// The standard.
            /// </summary>
            public string Standard;
        }

        /// <summary>
        /// The vts vobs.
        /// </summary>
        public class VtsVobs
        {
            /// <summary>
            /// The audio streams.
            /// </summary>
            public List<AudioStream> AudioStreams;

            /// <summary>
            /// The number of audio streams.
            /// </summary>
            public int NumberOfAudioStreams;

            /// <summary>
            /// The number of subtitles.
            /// </summary>
            public int NumberOfSubtitles;

            /// <summary>
            /// The subtitle i ds.
            /// </summary>
            public List<string> SubtitleIDs;

            /// <summary>
            /// The subtitles.
            /// </summary>
            public List<string> Subtitles;

            /// <summary>
            /// The subtitle types.
            /// </summary>
            public List<string> SubtitleTypes;

            /// <summary>
            /// The video stream.
            /// </summary>
            public VideoStream VideoStream;

            /// <summary>
            /// Initializes a new instance of the <see cref="VtsVobs"/> class.
            /// </summary>
            public VtsVobs()
            {
                this.VideoStream = new VideoStream();
                this.AudioStreams = new List<AudioStream>();
                this.Subtitles = new List<string>();
                this.SubtitleIDs = new List<string>();
                this.SubtitleTypes = new List<string>();
            }

            /// <summary>
            /// The get all languages.
            /// </summary>
            /// <returns>
            /// The <see cref="List"/>.
            /// </returns>
            public List<string> GetAllLanguages()
            {
                var list = new List<string>();
                for (int i = 0; i < this.Subtitles.Count; i++)
                {
                    if (i < this.SubtitleIDs.Count && i < this.SubtitleTypes.Count)
                    {
                        var ids = this.SubtitleIDs[i].Split(',');
                        var types = this.SubtitleTypes[i].Split(',');
                        if (ids.Length == 2 && ids[0].Trim() == ids[1].Trim() || ids.Length == 3 && ids[0].Trim() == ids[1].Trim() && ids[1].Trim() == ids[2].Trim())
                        {
                            list.Add(this.Subtitles[i] + " (" + ids[0].Trim() + ")");
                        }
                        else
                        {
                            if (ids.Length >= 1 && types.Length >= 1)
                            {
                                list.Add(this.Subtitles[i] + ", " + types[0].Trim() + " (" + ids[0].Trim() + ")");
                            }

                            if (ids.Length >= 2 && types.Length >= 2)
                            {
                                list.Add(this.Subtitles[i] + ", " + types[1].Trim() + " (" + ids[1].Trim() + ")");
                            }

                            if (ids.Length >= 3 && types.Length >= 3)
                            {
                                list.Add(this.Subtitles[i] + ", " + types[2].Trim() + " (" + ids[2].Trim() + ")");
                            }

                            if (ids.Length >= 4 && types.Length >= 4)
                            {
                                list.Add(this.Subtitles[i] + ", " + types[3].Trim() + " (" + ids[3].Trim() + ")");
                            }
                        }
                    }
                }

                return list;
            }
        };

        /// <summary>
        /// The program chain.
        /// </summary>
        public class ProgramChain
        {
            /// <summary>
            /// The audio streams available.
            /// </summary>
            public List<char> AudioStreamsAvailable;

            /// <summary>
            /// The color lookup table.
            /// </summary>
            public List<Color> ColorLookupTable;

            /// <summary>
            /// The number of cells.
            /// </summary>
            public int NumberOfCells;

            /// <summary>
            /// The number of pgc.
            /// </summary>
            public int NumberOfPgc;

            /// <summary>
            /// The pgc entry cells.
            /// </summary>
            public List<byte> PgcEntryCells;

            /// <summary>
            /// The pgc playback times.
            /// </summary>
            public List<string> PgcPlaybackTimes;

            /// <summary>
            /// The pgc start times.
            /// </summary>
            public List<string> PgcStartTimes;

            /// <summary>
            /// The playback time.
            /// </summary>
            public string PlaybackTime;

            /// <summary>
            /// The subtitles available.
            /// </summary>
            public List<byte[]> SubtitlesAvailable;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProgramChain"/> class.
            /// </summary>
            public ProgramChain()
            {
                this.PgcEntryCells = new List<byte>();
                this.PgcPlaybackTimes = new List<string>();
                this.PgcStartTimes = new List<string>();
                this.AudioStreamsAvailable = new List<char>();
                this.SubtitlesAvailable = new List<byte[]>();
                this.ColorLookupTable = new List<Color>();
            }

            /// <summary>
            /// Gets or sets a value indicating whether has 43 subs.
            /// </summary>
            public bool Has43Subs { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has wide subs.
            /// </summary>
            public bool HasWideSubs { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has letter subs.
            /// </summary>
            public bool HasLetterSubs { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has pan subs.
            /// </summary>
            public bool HasPanSubs { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has no specific subs.
            /// </summary>
            public bool HasNoSpecificSubs { get; set; }
        };

        /// <summary>
        /// The vts pgci.
        /// </summary>
        public class VtsPgci
        {
            /// <summary>
            /// The number of program chains.
            /// </summary>
            public int NumberOfProgramChains;

            /// <summary>
            /// The program chains.
            /// </summary>
            public List<ProgramChain> ProgramChains;

            /// <summary>
            /// Initializes a new instance of the <see cref="VtsPgci"/> class.
            /// </summary>
            public VtsPgci()
            {
                this.ProgramChains = new List<ProgramChain>();
            }
        };
    }
}