// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="TextST.cs">
//   
// </copyright>
// <summary>
//   The text st.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.SubtitleFormats
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic.BluRaySup;
    using Nikse.SubtitleEdit.Logic.TransportStream;

    /// <summary>
    /// The text st.
    /// </summary>
    public class TextST : SubtitleFormat
    {
        /// <summary>
        /// The text subtitle stream pid.
        /// </summary>
        private const int TextSubtitleStreamPid = 0x1800;

        /// <summary>
        /// The segment type dialog style.
        /// </summary>
        private const byte SegmentTypeDialogStyle = 0x81;

        /// <summary>
        /// The segment type dialog presentation.
        /// </summary>
        private const byte SegmentTypeDialogPresentation = 0x82;

        /// <summary>
        /// The presentation segments.
        /// </summary>
        public List<DialogPresentationSegment> PresentationSegments;

        /// <summary>
        /// The style segment.
        /// </summary>
        public DialogStyleSegment StyleSegment;

        /// <summary>
        /// Gets the extension.
        /// </summary>
        public override string Extension
        {
            get
            {
                return ".m2ts";
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Blu-ray TextST";
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
            if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".m2ts", StringComparison.OrdinalIgnoreCase) || !FileUtil.IsM2TransportStream(fileName))
            {
                return false;
            }

            var subtitle = new Subtitle();
            this.LoadSubtitle(subtitle, lines, fileName);
            return subtitle.Paragraphs.Count > 0;
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
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.LoadSubtitle(subtitle, fs);
            }
        }

        /// <summary>
        /// The load subtitle.
        /// </summary>
        /// <param name="subtitle">
        /// The subtitle.
        /// </param>
        /// <param name="ms">
        /// The ms.
        /// </param>
        public void LoadSubtitle(Subtitle subtitle, Stream ms)
        {
            // TODO: Parse PES
            var subtitlePackets = new List<Packet>();
            const int packetLength = 188;
            bool isM2TransportStream = this.DetectFormat(ms);
            var packetBuffer = new byte[packetLength];
            var m2TsTimeCodeBuffer = new byte[4];
            long position = 0;
            ms.Position = 0;

            // check for Topfield .rec file
            ms.Seek(position, SeekOrigin.Begin);
            ms.Read(m2TsTimeCodeBuffer, 0, 3);
            if (m2TsTimeCodeBuffer[0] == 0x54 && m2TsTimeCodeBuffer[1] == 0x46 && m2TsTimeCodeBuffer[2] == 0x72)
            {
                position = 3760;
            }

            long transportStreamLength = ms.Length;
            while (position < transportStreamLength)
            {
                ms.Seek(position, SeekOrigin.Begin);
                if (isM2TransportStream)
                {
                    ms.Read(m2TsTimeCodeBuffer, 0, m2TsTimeCodeBuffer.Length);
                    var tc = (m2TsTimeCodeBuffer[0] << 24) + (m2TsTimeCodeBuffer[1] << 16) + (m2TsTimeCodeBuffer[2] << 8) + (m2TsTimeCodeBuffer[3] & Helper.B00111111);

                    // should m2ts time code be used in any way?
                    var msecs = (ulong)Math.Round(tc / 27.0); // 27 or 90?
                    TimeCode tc2 = new TimeCode(msecs);
                    System.Diagnostics.Debug.WriteLine(tc2);
                    position += m2TsTimeCodeBuffer.Length;
                }

                ms.Read(packetBuffer, 0, packetLength);
                byte syncByte = packetBuffer[0];
                if (syncByte == Packet.SynchronizationByte)
                {
                    var packet = new Packet(packetBuffer);
                    if (packet.PacketId == TextSubtitleStreamPid)
                    {
                        subtitlePackets.Add(packet);
                    }

                    position += packetLength;
                }
                else
                {
                    position++;
                }
            }

            // TODO: merge ts packets
            this.PresentationSegments = new List<DialogPresentationSegment>();
            foreach (var item in subtitlePackets)
            {
                if (item.Payload != null && item.Payload.Length > 10 && VobSub.VobSubParser.IsPrivateStream2(item.Payload, 0))
                {
                    if (item.Payload[6] == SegmentTypeDialogPresentation)
                    {
                        var dps = new DialogPresentationSegment(item.Payload);
                        this.PresentationSegments.Add(dps);
                        subtitle.Paragraphs.Add(new Paragraph(dps.PlainText.Trim(), dps.StartPtsMilliseconds, dps.EndPtsMilliseconds));
                    }
                    else if (item.Payload[6] == SegmentTypeDialogStyle)
                    {
                        this.StyleSegment = new DialogStyleSegment(item.Payload);
                    }
                }
            }

            subtitle.Renumber();
        }

        /// <summary>
        /// The detect format.
        /// </summary>
        /// <param name="ms">
        /// The ms.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool DetectFormat(Stream ms)
        {
            if (ms.Length > 192 + 192 + 5)
            {
                ms.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[192 + 192 + 5];
                ms.Read(buffer, 0, buffer.Length);
                if (buffer[0] == Packet.SynchronizationByte && buffer[188] == Packet.SynchronizationByte)
                {
                    return false;
                }

                if (buffer[4] == Packet.SynchronizationByte && buffer[192 + 4] == Packet.SynchronizationByte && buffer[192 + 192 + 4] == Packet.SynchronizationByte)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The palette.
        /// </summary>
        public class Palette
        {
            /// <summary>
            /// Gets or sets the palette entry id.
            /// </summary>
            public int PaletteEntryId { get; set; }

            /// <summary>
            /// Gets or sets the y.
            /// </summary>
            public int Y { get; set; }

            /// <summary>
            /// Gets or sets the cr.
            /// </summary>
            public int Cr { get; set; }

            /// <summary>
            /// Gets or sets the cb.
            /// </summary>
            public int Cb { get; set; }

            /// <summary>
            /// Gets or sets the t.
            /// </summary>
            public int T { get; set; }

            /// <summary>
            /// Gets the color.
            /// </summary>
            public Color Color
            {
                get
                {
                    var arr = BluRaySupPalette.YCbCr2Rgb(this.Y, this.Cb, this.Cr, false);
                    return Color.FromArgb(this.T, arr[0], arr[1], arr[2]);
                }
            }
        }

        /// <summary>
        /// The region style.
        /// </summary>
        public class RegionStyle
        {
            /// <summary>
            /// Gets or sets the region style id.
            /// </summary>
            public int RegionStyleId { get; set; }

            /// <summary>
            /// Gets or sets the region horizontal position.
            /// </summary>
            public int RegionHorizontalPosition { get; set; }

            /// <summary>
            /// Gets or sets the region vertical position.
            /// </summary>
            public int RegionVerticalPosition { get; set; }

            /// <summary>
            /// Gets or sets the region width.
            /// </summary>
            public int RegionWidth { get; set; }

            /// <summary>
            /// Gets or sets the region height.
            /// </summary>
            public int RegionHeight { get; set; }

            /// <summary>
            /// Gets or sets the region bg palette entry id ref.
            /// </summary>
            public int RegionBgPaletteEntryIdRef { get; set; }

            /// <summary>
            /// Gets or sets the text box horizontal position.
            /// </summary>
            public int TextBoxHorizontalPosition { get; set; }

            /// <summary>
            /// Gets or sets the text box vertical position.
            /// </summary>
            public int TextBoxVerticalPosition { get; set; }

            /// <summary>
            /// Gets or sets the text box width.
            /// </summary>
            public int TextBoxWidth { get; set; }

            /// <summary>
            /// Gets or sets the text box height.
            /// </summary>
            public int TextBoxHeight { get; set; }

            /// <summary>
            /// Gets or sets the text flow.
            /// </summary>
            public int TextFlow { get; set; }

            /// <summary>
            /// Gets or sets the text horizontal alignment.
            /// </summary>
            public int TextHorizontalAlignment { get; set; }

            /// <summary>
            /// Gets or sets the text vertical alignment.
            /// </summary>
            public int TextVerticalAlignment { get; set; }

            /// <summary>
            /// Gets or sets the line space.
            /// </summary>
            public int LineSpace { get; set; }

            /// <summary>
            /// Gets or sets the font id ref.
            /// </summary>
            public int FontIdRef { get; set; }

            /// <summary>
            /// Gets or sets the font style.
            /// </summary>
            public int FontStyle { get; set; }

            /// <summary>
            /// Gets or sets the font size.
            /// </summary>
            public int FontSize { get; set; }

            /// <summary>
            /// Gets or sets the font palette entry id ref.
            /// </summary>
            public int FontPaletteEntryIdRef { get; set; }

            /// <summary>
            /// Gets or sets the font outline palette entry id ref.
            /// </summary>
            public int FontOutlinePaletteEntryIdRef { get; set; }

            /// <summary>
            /// Gets or sets the font outline thickness.
            /// </summary>
            public int FontOutlineThickness { get; set; }
        }

        /// <summary>
        /// The user style.
        /// </summary>
        public class UserStyle
        {
            /// <summary>
            /// Gets or sets the user style id.
            /// </summary>
            public int UserStyleId { get; set; }

            /// <summary>
            /// Gets or sets the region horizontal position direction.
            /// </summary>
            public int RegionHorizontalPositionDirection { get; set; }

            /// <summary>
            /// Gets or sets the region horizontal position delta.
            /// </summary>
            public int RegionHorizontalPositionDelta { get; set; }

            /// <summary>
            /// Gets or sets the region vertical position direction.
            /// </summary>
            public int RegionVerticalPositionDirection { get; set; }

            /// <summary>
            /// Gets or sets the region vertical position delta.
            /// </summary>
            public int RegionVerticalPositionDelta { get; set; }

            /// <summary>
            /// Gets or sets the font size inc dec.
            /// </summary>
            public int FontSizeIncDec { get; set; }

            /// <summary>
            /// Gets or sets the font size delta.
            /// </summary>
            public int FontSizeDelta { get; set; }

            /// <summary>
            /// Gets or sets the text box horizontal position direction.
            /// </summary>
            public int TextBoxHorizontalPositionDirection { get; set; }

            /// <summary>
            /// Gets or sets the text box horizontal position delta.
            /// </summary>
            public int TextBoxHorizontalPositionDelta { get; set; }

            /// <summary>
            /// Gets or sets the text box vertical position direction.
            /// </summary>
            public int TextBoxVerticalPositionDirection { get; set; }

            /// <summary>
            /// Gets or sets the text box vertical position delta.
            /// </summary>
            public int TextBoxVerticalPositionDelta { get; set; }

            /// <summary>
            /// Gets or sets the text box width inc dec.
            /// </summary>
            public int TextBoxWidthIncDec { get; set; }

            /// <summary>
            /// Gets or sets the text box width delta.
            /// </summary>
            public int TextBoxWidthDelta { get; set; }

            /// <summary>
            /// Gets or sets the text box height inc dec.
            /// </summary>
            public int TextBoxHeightIncDec { get; set; }

            /// <summary>
            /// Gets or sets the text box height delta.
            /// </summary>
            public int TextBoxHeightDelta { get; set; }

            /// <summary>
            /// Gets or sets the line space inc dec.
            /// </summary>
            public int LineSpaceIncDec { get; set; }

            /// <summary>
            /// Gets or sets the line space delta.
            /// </summary>
            public int LineSpaceDelta { get; set; }
        }

        /// <summary>
        /// The region.
        /// </summary>
        public class Region
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Region"/> class.
            /// </summary>
            public Region()
            {
                this.UserStyles = new List<UserStyle>();
            }

            /// <summary>
            /// Gets or sets the region style.
            /// </summary>
            public RegionStyle RegionStyle { get; set; }

            /// <summary>
            /// Gets or sets the user styles.
            /// </summary>
            public List<UserStyle> UserStyles { get; set; }
        }

        /// <summary>
        /// The dialog style segment.
        /// </summary>
        public class DialogStyleSegment
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DialogStyleSegment"/> class.
            /// </summary>
            /// <param name="buffer">
            /// The buffer.
            /// </param>
            public DialogStyleSegment(byte[] buffer)
            {
                this.PlayerStyleFlag = (buffer[9] & Helper.B10000000) > 0;
                this.NumberOfRegionStyles = buffer[11];
                this.NumberOfUserStyles = buffer[12];

                int idx = 13;
                this.Regions = new List<Region>(this.NumberOfRegionStyles);
                for (int i = 0; i < this.NumberOfRegionStyles; i++)
                {
                    var region = new Region();
                    var rs = new RegionStyle { RegionStyleId = buffer[idx], RegionHorizontalPosition = (buffer[idx + 1] << 8) + buffer[idx + 2], RegionVerticalPosition = (buffer[idx + 3] << 8) + buffer[idx + 4], RegionWidth = (buffer[idx + 5] << 8) + buffer[idx + 6], RegionHeight = (buffer[idx + 7] << 8) + buffer[idx + 8], RegionBgPaletteEntryIdRef = buffer[idx + 9], TextBoxHorizontalPosition = (buffer[idx + 11] << 8) + buffer[idx + 12], TextBoxVerticalPosition = (buffer[idx + 13] << 8) + buffer[idx + 14], TextBoxWidth = (buffer[idx + 15] << 8) + buffer[idx + 16], TextBoxHeight = (buffer[idx + 17] << 8) + buffer[idx + 18], TextFlow = buffer[idx + 19], TextHorizontalAlignment = buffer[idx + 20], TextVerticalAlignment = buffer[idx + 21], LineSpace = buffer[idx + 22], FontIdRef = buffer[idx + 23], FontStyle = buffer[idx + 24], FontSize = buffer[idx + 25], FontPaletteEntryIdRef = buffer[idx + 26], FontOutlinePaletteEntryIdRef = buffer[idx + 27], FontOutlineThickness = buffer[idx + 28] };
                    region.RegionStyle = rs;
                    idx += 29;

                    for (int j = 0; j < this.NumberOfUserStyles; j++)
                    {
                        var us = new UserStyle { UserStyleId = buffer[idx], RegionHorizontalPositionDirection = buffer[idx + 1] >> 7, RegionHorizontalPositionDelta = ((buffer[idx + 1] & Helper.B01111111) << 8) + buffer[idx + 2], RegionVerticalPositionDirection = buffer[idx + 3] >> 7, RegionVerticalPositionDelta = ((buffer[idx + 3] & Helper.B01111111) << 8) + buffer[idx + 4], FontSizeIncDec = buffer[idx + 5] >> 7, FontSizeDelta = buffer[idx + 5] & Helper.B01111111, TextBoxHorizontalPositionDirection = buffer[idx + 6] >> 7, TextBoxHorizontalPositionDelta = ((buffer[idx + 6] & Helper.B01111111) << 8) + buffer[idx + 7], TextBoxVerticalPositionDirection = buffer[idx + 8] >> 7, TextBoxVerticalPositionDelta = ((buffer[idx + 8] & Helper.B01111111) << 8) + buffer[idx + 9], TextBoxWidthIncDec = buffer[idx + 10] >> 7, TextBoxWidthDelta = ((buffer[idx + 10] & Helper.B01111111) << 8) + buffer[idx + 11], TextBoxHeightIncDec = buffer[idx + 12] >> 7, TextBoxHeightDelta = ((buffer[idx + 12] & Helper.B01111111) << 8) + buffer[idx + 13], LineSpaceIncDec = buffer[idx + 14] >> 7, LineSpaceDelta = buffer[idx + 14] & Helper.B01111111 };
                        region.UserStyles.Add(us);
                        idx += 15;
                    }

                    this.Regions.Add(region);
                }

                int numberOfPalettees = ((buffer[idx] << 8) + buffer[idx + 1]) / 5;
                this.Palettes = new List<Palette>(numberOfPalettees);
                idx += 2;
                for (int i = 0; i < numberOfPalettees; i++)
                {
                    var palette = new Palette { PaletteEntryId = buffer[idx], Y = buffer[idx + 1], Cr = buffer[idx + 2], Cb = buffer[idx + 3], T = buffer[idx + 4] };
                    this.Palettes.Add(palette);
                    idx += 5;
                }

                this.NumberOfDialogPresentationSegments = (buffer[idx] << 8) + buffer[idx + 1];
            }

            /// <summary>
            /// Gets or sets a value indicating whether player style flag.
            /// </summary>
            public bool PlayerStyleFlag { get; set; }

            /// <summary>
            /// Gets or sets the number of region styles.
            /// </summary>
            public int NumberOfRegionStyles { get; set; }

            /// <summary>
            /// Gets or sets the number of user styles.
            /// </summary>
            public int NumberOfUserStyles { get; set; }

            /// <summary>
            /// Gets or sets the regions.
            /// </summary>
            public List<Region> Regions { get; set; }

            /// <summary>
            /// Gets or sets the palettes.
            /// </summary>
            public List<Palette> Palettes { get; set; }

            /// <summary>
            /// Gets or sets the number of dialog presentation segments.
            /// </summary>
            public int NumberOfDialogPresentationSegments { get; set; }
        }

        /// <summary>
        /// The subtitle region.
        /// </summary>
        public class SubtitleRegion
        {
            /// <summary>
            /// Gets or sets a value indicating whether continuous presentation.
            /// </summary>
            public bool ContinuousPresentation { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether forced.
            /// </summary>
            public bool Forced { get; set; }

            /// <summary>
            /// Gets or sets the region style id.
            /// </summary>
            public int RegionStyleId { get; set; }

            /// <summary>
            /// Gets or sets the texts.
            /// </summary>
            public List<string> Texts { get; set; }
        }

        /// <summary>
        /// The dialog presentation segment.
        /// </summary>
        public class DialogPresentationSegment
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DialogPresentationSegment"/> class.
            /// </summary>
            /// <param name="buffer">
            /// The buffer.
            /// </param>
            public DialogPresentationSegment(byte[] buffer)
            {
                this.StartPts = buffer[13];
                this.StartPts += (ulong)buffer[12] << 8;
                this.StartPts += (ulong)buffer[11] << 16;
                this.StartPts += (ulong)buffer[10] << 24;
                this.StartPts += (ulong)(buffer[9] & Helper.B00000001) << 32;

                this.EndPts = buffer[18];
                this.EndPts += (ulong)buffer[17] << 8;
                this.EndPts += (ulong)buffer[16] << 16;
                this.EndPts += (ulong)buffer[15] << 24;
                this.EndPts += (ulong)(buffer[14] & Helper.B00000001) << 32;

                this.PaletteUpdate = (buffer[19] & Helper.B10000000) > 0;
                int idx = 20;
                if (this.PaletteUpdate)
                {
                    this.NumberOfPaletteEntries = buffer[21] + (buffer[20] << 8);
                    idx += this.NumberOfPaletteEntries * 5;
                }

                int numberOfRegions = buffer[idx];
                idx++;
                this.Regions = new List<SubtitleRegion>(numberOfRegions);
                for (int i = 0; i < numberOfRegions; i++)
                {
                    var region = new SubtitleRegion { ContinuousPresentation = (buffer[idx] & Helper.B10000000) > 0, Forced = (buffer[idx] & Helper.B01000000) > 0 };
                    idx++;
                    region.RegionStyleId = buffer[idx];
                    idx++;
                    int regionSubtitleLength = buffer[idx + 1] + (buffer[idx] << 8);
                    idx += 2;
                    int processedLength = 0;
                    region.Texts = new List<string>();
                    string endStyle = string.Empty;
                    while (processedLength < regionSubtitleLength)
                    {
                        byte escapeCode = buffer[idx];
                        idx++;
                        byte dataType = buffer[idx];
                        idx++;
                        byte dataLength = buffer[idx];
                        idx++;
                        processedLength += 3;
                        if (dataType == 0x01)
                        {
                            // Text
                            string text = Encoding.UTF8.GetString(buffer, idx, dataLength);
                            region.Texts.Add(text);
                        }
                        else if (dataType == 0x02)
                        {
                            // Change a font set
                            System.Diagnostics.Debug.WriteLine("font set");
                        }
                        else if (dataType == 0x03)
                        {
                            // Change a font style
                            System.Diagnostics.Debug.WriteLine("font style");
                            var fontStyle = buffer[idx];
                            switch (fontStyle)
                            {
                                case 1:
                                    region.Texts.Add("<b>");
                                    endStyle = "</b>";
                                    break;
                                case 2:
                                    region.Texts.Add("<i>");
                                    endStyle = "</i>";
                                    break;
                                case 3:
                                    region.Texts.Add("<b><i>");
                                    endStyle = "</i></b>";
                                    break;
                                case 5:
                                    region.Texts.Add("<b>");
                                    endStyle = "</b>";
                                    break;
                                case 6:
                                    region.Texts.Add("<i>");
                                    endStyle = "</i>";
                                    break;
                                case 7:
                                    region.Texts.Add("<b><i>");
                                    endStyle = "</i></b>";
                                    break;
                            }
                        }
                        else if (dataType == 0x04)
                        {
                            // Change a font size
                            System.Diagnostics.Debug.WriteLine("font size");
                        }
                        else if (dataType == 0x05)
                        {
                            // Change a font color
                            System.Diagnostics.Debug.WriteLine("font color");
                        }
                        else if (dataType == 0x0A)
                        {
                            // Line break
                            region.Texts.Add(Environment.NewLine);
                        }
                        else if (dataType == 0x0B)
                        {
                            // End of inline style
                            System.Diagnostics.Debug.WriteLine("End inline style");
                            if (!string.IsNullOrEmpty(endStyle))
                            {
                                region.Texts.Add(endStyle);
                                endStyle = string.Empty;
                            }
                        }

                        processedLength += dataLength;
                        idx += dataLength;
                    }

                    if (!string.IsNullOrEmpty(endStyle))
                    {
                        region.Texts.Add(endStyle);
                    }

                    this.Regions.Add(region);
                }
            }

            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// Gets or sets the start pts.
            /// </summary>
            public ulong StartPts { get; set; }

            /// <summary>
            /// Gets or sets the end pts.
            /// </summary>
            public ulong EndPts { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether palette update.
            /// </summary>
            public bool PaletteUpdate { get; set; }

            /// <summary>
            /// Gets or sets the number of palette entries.
            /// </summary>
            public int NumberOfPaletteEntries { get; set; }

            /// <summary>
            /// Gets or sets the regions.
            /// </summary>
            public List<SubtitleRegion> Regions { get; set; }

            /// <summary>
            /// Gets the plain text.
            /// </summary>
            public string PlainText
            {
                get
                {
                    var sb = new StringBuilder();
                    foreach (var region in this.Regions)
                    {
                        foreach (string text in region.Texts)
                        {
                            sb.Append(text);
                        }
                    }

                    return sb.ToString();
                }
            }

            /// <summary>
            /// Gets the start pts milliseconds.
            /// </summary>
            public ulong StartPtsMilliseconds
            {
                get
                {
                    return (ulong)Math.Round(this.StartPts / 90.0);
                }
            }

            /// <summary>
            /// Gets the end pts milliseconds.
            /// </summary>
            public ulong EndPtsMilliseconds
            {
                get
                {
                    return (ulong)Math.Round(this.EndPts / 90.0);
                }
            }
        }
    }
}