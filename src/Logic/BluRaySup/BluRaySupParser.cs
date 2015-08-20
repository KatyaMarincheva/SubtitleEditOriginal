// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="BluRaySupParser.cs">
//   
// </copyright>
// <summary>
//   The blu ray sup parser.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.BluRaySup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The blu ray sup parser.
    /// </summary>
    public static class BluRaySupParser
    {
        /// <summary>
        ///     PGS composition state
        /// </summary>
        public enum CompositionState
        {
            /// <summary>
            ///     Normal: doesn't have to be complete
            /// </summary>
            Normal, 

            /// <summary>
            ///     Acquisition point
            /// </summary>
            AcquPoint, 

            /// <summary>
            ///     Epoch start - clears the screen
            /// </summary>
            EpochStart, 

            /// <summary>
            ///     Epoch continue
            /// </summary>
            EpochContinue, 

            /// <summary>
            ///     Unknown value
            /// </summary>
            Invalid
        }

        /// <summary>
        /// The header size.
        /// </summary>
        private const int HeaderSize = 13;

        /// <summary>
        /// Parses a Blu-ray sup file
        /// </summary>
        /// <param name="fileName">
        /// BluRay sup file name
        /// </param>
        /// <param name="log">
        /// Parsing info is logged here
        /// </param>
        /// <returns>
        /// List of BluRaySupPictures
        /// </returns>
        public static List<PcsData> ParseBluRaySup(string fileName, StringBuilder log)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return ParseBluRaySup(fs, log, false);
            }
        }

        /// <summary>
        /// The parse blu ray sup.
        /// </summary>
        /// <param name="ms">
        /// The ms.
        /// </param>
        /// <param name="log">
        /// The log.
        /// </param>
        /// <param name="fromMatroskaFile">
        /// The from matroska file.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<PcsData> ParseBluRaySup(Stream ms, StringBuilder log, bool fromMatroskaFile)
        {
            long position = ms.Position;
            int segmentCount = 0;
            Dictionary<int, List<PaletteInfo>> palettes = new Dictionary<int, List<PaletteInfo>>();
            bool forceFirstOds = true;
            Dictionary<int, List<OdsData>> bitmapObjects = new Dictionary<int, List<OdsData>>();
            PcsData latestPcs = null;
            int latestCompNum = -1;
            List<PcsData> pcsList = new List<PcsData>();
            byte[] headerBuffer;
            if (fromMatroskaFile)
            {
                headerBuffer = new byte[3];
            }
            else
            {
                headerBuffer = new byte[HeaderSize];
            }

            while (position < ms.Length)
            {
                ms.Seek(position, SeekOrigin.Begin);

                // Read segment header
                ms.Read(headerBuffer, 0, headerBuffer.Length);
                SupSegment segment;
                if (fromMatroskaFile)
                {
                    segment = ParseSegmentHeaderFromMatroska(headerBuffer);
                }
                else
                {
                    segment = ParseSegmentHeader(headerBuffer, log);
                }

                position += headerBuffer.Length;

                // Read segment data
                byte[] buffer = new byte[segment.Size];
                ms.Read(buffer, 0, buffer.Length);
                log.Append(segmentCount + ": ");

                switch (segment.Type)
                {
                    case 0x14: // Palette
                        if (latestPcs != null)
                        {
                            log.AppendLine(string.Format("0x14 - Palette - PDS offset={0} size={1}", position, segment.Size));
                            PdsData pds = ParsePds(buffer, segment);
                            log.AppendLine(pds.Message);
                            if (pds.PaletteInfo != null)
                            {
                                if (!palettes.ContainsKey(pds.PaletteId))
                                {
                                    palettes[pds.PaletteId] = new List<PaletteInfo>();
                                }
                                else
                                {
                                    if (latestPcs.PaletteUpdate)
                                    {
                                        palettes[pds.PaletteId].RemoveAt(palettes[pds.PaletteId].Count - 1);
                                    }
                                    else
                                    {
                                        log.AppendLine("Extra Palette");
                                    }
                                }

                                palettes[pds.PaletteId].Add(pds.PaletteInfo);
                            }
                        }

                        break;

                    case 0x15: // Image bitmap data
                        if (latestPcs != null)
                        {
                            log.AppendLine(string.Format("0x15 - Bitmap data - ODS offset={0} size={1}", position, segment.Size));
                            OdsData ods = ParseOds(buffer, segment, forceFirstOds);
                            log.AppendLine(ods.Message);
                            if (!latestPcs.PaletteUpdate)
                            {
                                List<OdsData> odsList;
                                if (ods.IsFirst)
                                {
                                    odsList = new List<OdsData>();
                                    odsList.Add(ods);
                                    bitmapObjects[ods.ObjectId] = odsList;
                                }
                                else
                                {
                                    if (bitmapObjects.TryGetValue(ods.ObjectId, out odsList))
                                    {
                                        odsList.Add(ods);
                                    }
                                    else
                                    {
                                        log.AppendLine(string.Format("INVALID ObjectId {0} in ODS, offset={1}", ods.ObjectId, position));
                                    }
                                }
                            }
                            else
                            {
                                log.AppendLine(string.Format("Bitmap Data Ignore due to PaletteUpdate offset={0}", position));
                            }

                            forceFirstOds = false;
                        }

                        break;

                    case 0x16: // Picture time codes
                        if (latestPcs != null)
                        {
                            if (CompletePcs(latestPcs, bitmapObjects, palettes))
                            {
                                pcsList.Add(latestPcs);
                            }

                            latestPcs = null;
                        }

                        log.AppendLine(string.Format("0x16 - Picture codes, offset={0} size={1}", position, segment.Size));
                        forceFirstOds = true;
                        PcsData nextPcs = ParsePicture(buffer, segment);
                        log.AppendLine(nextPcs.Message);
                        latestPcs = nextPcs;
                        latestCompNum = nextPcs.CompNum;
                        if (latestPcs.CompositionState == CompositionState.EpochStart)
                        {
                            bitmapObjects.Clear();
                            palettes.Clear();
                        }

                        break;

                    case 0x17: // Window display
                        if (latestPcs != null)
                        {
                            log.AppendLine(string.Format("0x17 - Window display offset={0} size={1}", position, segment.Size));
                            int windowCount = buffer[0];
                            int offset = 0;
                            for (int nextWindow = 0; nextWindow < windowCount; nextWindow++)
                            {
                                int windowId = buffer[1 + offset];
                                int x = BigEndianInt16(buffer, 2 + offset);
                                int y = BigEndianInt16(buffer, 4 + offset);
                                int width = BigEndianInt16(buffer, 6 + offset);
                                int height = BigEndianInt16(buffer, 8 + offset);
                                log.AppendLine(string.Format("WinId: {4}, X: {0}, Y: {1}, Width: {2}, Height: {3}", x, y, width, height, windowId));
                                offset += 9;
                            }
                        }

                        break;

                    case 0x80:
                        forceFirstOds = true;
                        log.AppendLine(string.Format("0x80 - END offset={0} size={1}", position, segment.Size));
                        if (latestPcs != null)
                        {
                            if (CompletePcs(latestPcs, bitmapObjects, palettes))
                            {
                                pcsList.Add(latestPcs);
                            }

                            latestPcs = null;
                        }

                        break;

                    default:
                        log.AppendLine(string.Format("0x?? - END offset={0} UNKOWN SEGMENT TYPE={1}", position, segment.Type));
                        break;
                }

                position += segment.Size;
                segmentCount++;
            }

            if (latestPcs != null)
            {
                if (CompletePcs(latestPcs, bitmapObjects, palettes))
                {
                    pcsList.Add(latestPcs);
                }

                latestPcs = null;
            }

            for (int pcsIndex = 1; pcsIndex < pcsList.Count; pcsIndex++)
            {
                pcsList[pcsIndex - 1].EndTime = pcsList[pcsIndex].StartTime;
            }

            pcsList.RemoveAll(pcs => pcs.PcsObjects.Count == 0);

            foreach (PcsData pcs in pcsList)
            {
                foreach (List<OdsData> odsList in pcs.BitmapObjects)
                {
                    if (odsList.Count > 1)
                    {
                        int bufSize = 0;
                        foreach (OdsData ods in odsList)
                        {
                            bufSize += ods.Fragment.ImagePacketSize;
                        }

                        byte[] buf = new byte[bufSize];
                        int offset = 0;
                        foreach (OdsData ods in odsList)
                        {
                            Buffer.BlockCopy(ods.Fragment.ImageBuffer, 0, buf, offset, ods.Fragment.ImagePacketSize);
                            offset += ods.Fragment.ImagePacketSize;
                        }

                        odsList[0].Fragment.ImageBuffer = buf;
                        odsList[0].Fragment.ImagePacketSize = bufSize;
                        while (odsList.Count > 1)
                        {
                            odsList.RemoveAt(1);
                        }
                    }
                }
            }

            for (int pcsIndex = pcsList.Count - 1; pcsIndex > 0; pcsIndex--)
            {
                PcsData cur = pcsList[pcsIndex];
                PcsData prev = pcsList[pcsIndex - 1];
                if (Math.Abs(prev.EndTime - cur.StartTime) < 10 && prev.Size.Width == cur.Size.Width && prev.Size.Height == cur.Size.Height)
                {
                    if (cur.BitmapObjects.Count > 0 && cur.BitmapObjects[0].Count > 0 && prev.BitmapObjects.Count > 0 && prev.BitmapObjects[0].Count > 0 && ByteArraysEqual(cur.BitmapObjects[0][0].Fragment.ImageBuffer, prev.BitmapObjects[0][0].Fragment.ImageBuffer))
                    {
                        prev.EndTime = cur.EndTime;
                        pcsList.RemoveAt(pcsIndex);
                    }
                }
            }

            return pcsList;
        }

        /// <summary>
        /// The parse segment header.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="log">
        /// The log.
        /// </param>
        /// <returns>
        /// The <see cref="SupSegment"/>.
        /// </returns>
        private static SupSegment ParseSegmentHeader(byte[] buffer, StringBuilder log)
        {
            SupSegment segment = new SupSegment();
            if (buffer[0] == 0x50 && buffer[1] == 0x47)
            {
                // 80 + 71 - P G
                segment.PtsTimestamp = BigEndianInt32(buffer, 2); // read PTS
                segment.DtsTimestamp = BigEndianInt32(buffer, 6); // read PTS
                segment.Type = buffer[10];
                segment.Size = BigEndianInt16(buffer, 11);
            }
            else
            {
                log.AppendLine("Unable to read segment - PG missing!");
            }

            return segment;
        }

        /// <summary>
        /// The parse segment header from matroska.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="SupSegment"/>.
        /// </returns>
        private static SupSegment ParseSegmentHeaderFromMatroska(byte[] buffer)
        {
            SupSegment segment = new SupSegment();
            segment.Type = buffer[0];
            segment.Size = BigEndianInt16(buffer, 1);
            return segment;
        }

        /// <summary>
        /// Parse an PCS packet which contains width/height info
        /// </summary>
        /// <param name="buffer">
        /// Raw data buffer, starting right after segment
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// The <see cref="PcsObject"/>.
        /// </returns>
        private static PcsObject ParsePcs(byte[] buffer, int offset)
        {
            PcsObject pcs = new PcsObject();

            // composition_object:
            pcs.ObjectId = BigEndianInt16(buffer, 11 + offset); // 16bit object_id_ref
            pcs.WindowId = buffer[13 + offset];

            // skipped:  8bit  window_id_ref
            // object_cropped_flag: 0x80, forced_on_flag = 0x040, 6bit reserved
            int forcedCropped = buffer[14 + offset];
            pcs.IsForced = (forcedCropped & 0x40) == 0x40;
            pcs.Origin = new Point(BigEndianInt16(buffer, 15 + offset), BigEndianInt16(buffer, 17 + offset));
            return pcs;
        }

        /// <summary>
        /// The parse picture.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="segment">
        /// The segment.
        /// </param>
        /// <returns>
        /// The <see cref="PcsData"/>.
        /// </returns>
        private static PcsData ParsePicture(byte[] buffer, SupSegment segment)
        {
            StringBuilder sb = new StringBuilder();
            PcsData pcs = new PcsData();
            pcs.Size = new Size(BigEndianInt16(buffer, 0), BigEndianInt16(buffer, 2));
            pcs.FramesPerSecondType = buffer[4]; // hi nibble: frame_rate, lo nibble: reserved
            pcs.CompNum = BigEndianInt16(buffer, 5);
            pcs.CompositionState = GetCompositionState(buffer[7]);
            pcs.StartTime = segment.PtsTimestamp;

            // 8bit  palette_update_flag (0x80), 7bit reserved
            pcs.PaletteUpdate = buffer[8] == 0x80;
            pcs.PaletteId = buffer[9]; // 8bit  palette_id_ref
            int compositionObjectCount = buffer[10]; // 8bit  number_of_composition_objects (0..2)

            sb.AppendFormat("CompNum: {0}, Pts: {1}, State: {2}, PalUpdate: {3}, PalId {4}", pcs.CompNum, ToolBox.PtsToTimeString(pcs.StartTime), pcs.CompositionState, pcs.PaletteUpdate, pcs.PaletteId);

            if (pcs.CompositionState == CompositionState.Invalid)
            {
                sb.Append("Illegal composition state Invalid");
            }
            else
            {
                int offset = 0;
                pcs.PcsObjects = new List<PcsObject>();
                for (int compObjIndex = 0; compObjIndex < compositionObjectCount; compObjIndex++)
                {
                    PcsObject pcsObj = ParsePcs(buffer, offset);
                    pcs.PcsObjects.Add(pcsObj);
                    sb.AppendLine();
                    sb.AppendFormat("ObjId: {0}, WinId: {1}, Forced: {2}, X: {3}, Y: {4}", pcsObj.ObjectId, pcsObj.WindowId, pcsObj.IsForced, pcsObj.Origin.X, pcsObj.Origin.Y);
                    offset += 8;
                }
            }

            pcs.Message = sb.ToString();
            return pcs;
        }

        /// <summary>
        /// The complete pcs.
        /// </summary>
        /// <param name="pcs">
        /// The pcs.
        /// </param>
        /// <param name="bitmapObjects">
        /// The bitmap objects.
        /// </param>
        /// <param name="palettes">
        /// The palettes.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool CompletePcs(PcsData pcs, Dictionary<int, List<OdsData>> bitmapObjects, Dictionary<int, List<PaletteInfo>> palettes)
        {
            if (pcs == null || pcs.PcsObjects == null || palettes == null)
            {
                return false;
            }

            if (pcs.PcsObjects.Count == 0)
            {
                return true;
            }

            if (!palettes.ContainsKey(pcs.PaletteId))
            {
                return false;
            }

            pcs.PaletteInfos = new List<PaletteInfo>(palettes[pcs.PaletteId]);
            pcs.BitmapObjects = new List<List<OdsData>>();
            for (int index = 0; index < pcs.PcsObjects.Count; index++)
            {
                int objId = pcs.PcsObjects[index].ObjectId;
                if (!bitmapObjects.ContainsKey(objId))
                {
                    return false;
                }

                pcs.BitmapObjects.Add(bitmapObjects[objId]);
            }

            return true;
        }

        /// <summary>
        /// parse an PDS packet which contain palette info
        /// </summary>
        /// <param name="buffer">
        /// Buffer of raw byte data, starting right after segment
        /// </param>
        /// <param name="segment">
        /// object containing info about the current segment
        /// </param>
        /// <returns>
        /// number of valid palette entries (-1 for fault)
        /// </returns>
        private static PdsData ParsePds(byte[] buffer, SupSegment segment)
        {
            int paletteId = buffer[0]; // 8bit palette ID (0..7)

            // 8bit palette version number (incremented for each palette change)
            int paletteUpdate = buffer[1];

            PaletteInfo p = new PaletteInfo();
            p.PaletteSize = (segment.Size - 2) / 5;

            if (p.PaletteSize <= 0)
            {
                return new PdsData { Message = "Empty palette" };
            }

            p.PaletteBuffer = new byte[p.PaletteSize * 5];
            Buffer.BlockCopy(buffer, 2, p.PaletteBuffer, 0, p.PaletteSize * 5); // save palette buffer in palette object

            return new PdsData { Message = "PalId: " + paletteId + ", update: " + paletteUpdate + ", " + p.PaletteSize + " entries", PaletteId = paletteId, PaletteVersion = paletteUpdate, PaletteInfo = p };
        }

        /// <summary>
        /// parse an ODS packet which contain the image data
        /// </summary>
        /// <param name="buffer">
        /// raw byte date, starting right after segment
        /// </param>
        /// <param name="segment">
        /// object containing info about the current segment
        /// </param>
        /// <param name="forceFirst">
        /// The force First.
        /// </param>
        /// <returns>
        /// true if this is a valid new object (neither invalid nor a fragment)
        /// </returns>
        private static OdsData ParseOds(byte[] buffer, SupSegment segment, bool forceFirst)
        {
            int objId = BigEndianInt16(buffer, 0); // 16bit object_id
            int objVer = buffer[2]; // 16bit object_id nikse - index 2 or 1???
            int objSeq = buffer[3]; // 8bit  first_in_sequence (0x80),

            // last_in_sequence (0x40), 6bits reserved
            bool first = ((objSeq & 0x80) == 0x80) || forceFirst;
            bool last = (objSeq & 0x40) == 0x40;

            ImageObjectFragment info = new ImageObjectFragment();
            if (first)
            {
                int width = BigEndianInt16(buffer, 7); // object_width
                int height = BigEndianInt16(buffer, 9); // object_height

                info.ImagePacketSize = segment.Size - 11; // Image packet size (image bytes)
                info.ImageBuffer = new byte[info.ImagePacketSize];
                Buffer.BlockCopy(buffer, 11, info.ImageBuffer, 0, info.ImagePacketSize);

                return new OdsData { IsFirst = true, Size = new Size(width, height), ObjectId = objId, ObjectVersion = objVer, Fragment = info, Message = "ObjId: " + objId + ", ver: " + objVer + ", seq: first" + (last ? "/" : string.Empty) + (last ? string.Empty + "last" : string.Empty) + ", width: " + width + ", height: " + height };
            }

            info.ImagePacketSize = segment.Size - 4;
            info.ImageBuffer = new byte[info.ImagePacketSize];
            Buffer.BlockCopy(buffer, 4, info.ImageBuffer, 0, info.ImagePacketSize);

            return new OdsData { IsFirst = false, ObjectId = objId, ObjectVersion = objVer, Fragment = info, Message = "Continued ObjId: " + objId + ", ver: " + objVer + ", seq: " + (last ? string.Empty + "last" : string.Empty) };
        }

        /// <summary>
        /// The byte arrays equal.
        /// </summary>
        /// <param name="b1">
        /// The b 1.
        /// </param>
        /// <param name="b2">
        /// The b 2.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2)
            {
                return true;
            }

            if (b1 == null || b2 == null)
            {
                return false;
            }

            if (b1.Length != b2.Length)
            {
                return false;
            }

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The get composition state.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="CompositionState"/>.
        /// </returns>
        private static CompositionState GetCompositionState(byte type)
        {
            switch (type)
            {
                case 0x00:
                    return CompositionState.Normal;
                case 0x40:
                    return CompositionState.AcquPoint;
                case 0x80:
                    return CompositionState.EpochStart;
                case 0xC0:
                    return CompositionState.EpochContinue;
                default:
                    return CompositionState.Invalid;
            }
        }

        /// <summary>
        /// The big endian int 16.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int BigEndianInt16(byte[] buffer, int index)
        {
            if (buffer.Length < 2)
            {
                return 0;
            }

            return buffer[index + 1] + (buffer[index + 0] << 8);
        }

        /// <summary>
        /// The big endian int 32.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        private static uint BigEndianInt32(byte[] buffer, int index)
        {
            if (buffer.Length < 4)
            {
                return 0;
            }

            return (uint)(buffer[index + 3] + (buffer[index + 2] << 8) + (buffer[index + 1] << 0x10) + (buffer[index + 0] << 0x18));
        }

        /// <summary>
        /// The sup segment.
        /// </summary>
        private class SupSegment
        {
            /// <summary>
            ///     segment DTS time stamp
            /// </summary>
            public long DtsTimestamp;

            /// <summary>
            ///     segment PTS time stamp
            /// </summary>
            public long PtsTimestamp;

            /// <summary>
            ///     segment size in bytes
            /// </summary>
            public int Size;

            /// <summary>
            ///     Type of segment
            /// </summary>
            public int Type;
        }

        /// <summary>
        /// The pcs object.
        /// </summary>
        public class PcsObject
        {
            /// <summary>
            /// The is forced.
            /// </summary>
            public bool IsForced;

            /// <summary>
            /// The object id.
            /// </summary>
            public int ObjectId;

            /// <summary>
            /// The origin.
            /// </summary>
            public Point Origin;

            /// <summary>
            /// The window id.
            /// </summary>
            public int WindowId;
        }

        /// <summary>
        /// The sup decoder.
        /// </summary>
        public static class SupDecoder
        {
            /// <summary>
            /// The alpha crop.
            /// </summary>
            private const int AlphaCrop = 14;

            /// <summary>
            /// The decode palette.
            /// </summary>
            /// <param name="paletteInfos">
            /// The palette infos.
            /// </param>
            /// <returns>
            /// The <see cref="BluRaySupPalette"/>.
            /// </returns>
            public static BluRaySupPalette DecodePalette(IList<PaletteInfo> paletteInfos)
            {
                BluRaySupPalette palette = new BluRaySupPalette(256);

                // by definition, index 0xff is always completely transparent
                // also all entries must be fully transparent after initialization
                bool fadeOut = false;
                for (int j = 0; j < paletteInfos.Count; j++)
                {
                    PaletteInfo p = paletteInfos[j];
                    int index = 0;

                    for (int i = 0; i < p.PaletteSize; i++)
                    {
                        // each palette entry consists of 5 bytes
                        int palIndex = p.PaletteBuffer[index];
                        int y = p.PaletteBuffer[++index];
                        int cr = p.PaletteBuffer[++index];
                        int cb = p.PaletteBuffer[++index];
                        int alpha = p.PaletteBuffer[++index];

                        int alphaOld = palette.GetAlpha(palIndex);

                        // avoid fading out
                        if (alpha >= alphaOld)
                        {
                            if (alpha < AlphaCrop)
                            { // to not mess with scaling algorithms, make transparent color black
                                y = 16;
                                cr = 128;
                                cb = 128;
                            }

                            palette.SetAlpha(palIndex, alpha);
                        }
                        else
                        {
                            fadeOut = true;
                        }

                        palette.SetYCbCr(palIndex, y, cb, cr);
                        index++;
                    }
                }

                if (fadeOut)
                {
                    Debug.Print("fade out detected -> patched palette\n");
                }

                return palette;
            }

            /// <summary>
            /// Decode caption from the input stream
            /// </summary>
            /// <param name="pcs">
            /// The pcs.
            /// </param>
            /// <param name="data">
            /// The data.
            /// </param>
            /// <param name="palettes">
            /// The palettes.
            /// </param>
            /// <returns>
            /// bitmap of the decoded caption
            /// </returns>
            public static Bitmap DecodeImage(PcsObject pcs, IList<OdsData> data, List<PaletteInfo> palettes)
            {
                long ticks = DateTime.Now.Ticks;

                if (pcs == null || data == null || data.Count == 0)
                {
                    return new Bitmap(1, 1);
                }

                int w = data[0].Size.Width;
                int h = data[0].Size.Height;

                FastBitmap bm = new FastBitmap(new Bitmap(w, h));
                bm.LockImage();
                BluRaySupPalette pal = DecodePalette(palettes);

                int index = 0;
                int ofs = 0;
                int xpos = 0;

                byte[] buf = data[0].Fragment.ImageBuffer;
                index = 0;
                do
                {
                    int b = buf[index++] & 0xff;
                    if (b == 0 && index < buf.Length)
                    {
                        b = buf[index++] & 0xff;
                        if (b == 0)
                        {
                            // next line
                            ofs = (ofs / w) * w;
                            if (xpos < w)
                            {
                                ofs += w;
                            }

                            xpos = 0;
                        }
                        else
                        {
                            int size;
                            if ((b & 0xC0) == 0x40)
                            {
                                if (index < buf.Length)
                                {
                                    // 00 4x xx -> xxx zeroes
                                    size = ((b - 0x40) << 8) + (buf[index++] & 0xff);
                                    Color c = Color.FromArgb(pal.GetArgb(0));
                                    for (int i = 0; i < size; i++)
                                    {
                                        PutPixel(bm, ofs++, c);
                                    }

                                    xpos += size;
                                }
                            }
                            else if ((b & 0xC0) == 0x80)
                            {
                                if (index < buf.Length)
                                {
                                    // 00 8x yy -> x times value y
                                    size = b - 0x80;
                                    b = buf[index++] & 0xff;
                                    Color c = Color.FromArgb(pal.GetArgb(b));
                                    for (int i = 0; i < size; i++)
                                    {
                                        PutPixel(bm, ofs++, c);
                                    }

                                    xpos += size;
                                }
                            }
                            else if ((b & 0xC0) != 0)
                            {
                                if (index < buf.Length)
                                {
                                    // 00 cx yy zz -> xyy times value z
                                    size = ((b - 0xC0) << 8) + (buf[index++] & 0xff);
                                    b = buf[index++] & 0xff;
                                    Color c = Color.FromArgb(pal.GetArgb(b));
                                    for (int i = 0; i < size; i++)
                                    {
                                        PutPixel(bm, ofs++, c);
                                    }

                                    xpos += size;
                                }
                            }
                            else
                            {
                                // 00 xx -> xx times 0
                                Color c = Color.FromArgb(pal.GetArgb(0));
                                for (int i = 0; i < b; i++)
                                {
                                    PutPixel(bm, ofs++, c);
                                }

                                xpos += b;
                            }
                        }
                    }
                    else
                    {
                        PutPixel(bm, ofs++, b, pal);
                        xpos++;
                    }
                }
                while (index < buf.Length);

                bm.UnlockImage();
                return bm.GetBitmap();
            }

            /// <summary>
            /// The put pixel.
            /// </summary>
            /// <param name="bmp">
            /// The bmp.
            /// </param>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <param name="color">
            /// The color.
            /// </param>
            /// <param name="palette">
            /// The palette.
            /// </param>
            private static void PutPixel(FastBitmap bmp, int index, int color, BluRaySupPalette palette)
            {
                int x = index % bmp.Width;
                int y = index / bmp.Width;
                if (x < bmp.Width && y < bmp.Height)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(palette.GetArgb(color)));
                }
            }

            /// <summary>
            /// The put pixel.
            /// </summary>
            /// <param name="bmp">
            /// The bmp.
            /// </param>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <param name="color">
            /// The color.
            /// </param>
            private static void PutPixel(FastBitmap bmp, int index, Color color)
            {
                if (color.A > 0)
                {
                    int x = index % bmp.Width;
                    int y = index / bmp.Width;
                    if (x < bmp.Width && y < bmp.Height)
                    {
                        bmp.SetPixel(x, y, color);
                    }
                }
            }
        }

        /// <summary>
        /// The pcs data.
        /// </summary>
        public class PcsData
        {
            /// <summary>
            /// The bitmap objects.
            /// </summary>
            public List<List<OdsData>> BitmapObjects;

            /// <summary>
            /// The comp num.
            /// </summary>
            public int CompNum;

            /// <summary>
            /// The composition state.
            /// </summary>
            public CompositionState CompositionState;

            /// <summary>
            /// The end time.
            /// </summary>
            public long EndTime; // end Pts

            /// <summary>
            /// The frames per second type.
            /// </summary>
            public int FramesPerSecondType;

            /// <summary>
            /// The message.
            /// </summary>
            public string Message;

            /// <summary>
            /// The palette id.
            /// </summary>
            public int PaletteId;

            /// <summary>
            /// The palette infos.
            /// </summary>
            public List<PaletteInfo> PaletteInfos;

            /// <summary>
            /// The palette update.
            /// </summary>
            public bool PaletteUpdate;

            /// <summary>
            /// The pcs objects.
            /// </summary>
            public List<PcsObject> PcsObjects;

            /// <summary>
            /// The size.
            /// </summary>
            public Size Size;

            /// <summary>
            /// The start time.
            /// </summary>
            public long StartTime; // Pts

            /// <summary>
            ///     if true, contains forced entry
            /// </summary>
            public bool IsForced
            {
                get
                {
                    foreach (PcsObject obj in this.PcsObjects)
                    {
                        if (obj.IsForced)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            /// <summary>
            /// The get bitmap.
            /// </summary>
            /// <returns>
            /// The <see cref="Bitmap"/>.
            /// </returns>
            public Bitmap GetBitmap()
            {
                if (this.PcsObjects.Count == 1)
                {
                    return SupDecoder.DecodeImage(this.PcsObjects[0], this.BitmapObjects[0], this.PaletteInfos);
                }

                Rectangle r = Rectangle.Empty;
                for (int ioIndex = 0; ioIndex < this.PcsObjects.Count; ioIndex++)
                {
                    Rectangle ioRect = new Rectangle(this.PcsObjects[ioIndex].Origin, this.BitmapObjects[ioIndex][0].Size);
                    if (r.IsEmpty)
                    {
                        r = ioRect;
                    }
                    else
                    {
                        r = Rectangle.Union(r, ioRect);
                    }
                }

                Bitmap mergedBmp = new Bitmap(r.Width, r.Height, PixelFormat.Format32bppArgb);
                for (int ioIndex = 0; ioIndex < this.PcsObjects.Count; ioIndex++)
                {
                    Point offset = this.PcsObjects[ioIndex].Origin - new Size(r.Location);
                    using (Bitmap singleBmp = SupDecoder.DecodeImage(this.PcsObjects[ioIndex], this.BitmapObjects[ioIndex], this.PaletteInfos))
                    using (Graphics gSideBySide = Graphics.FromImage(mergedBmp))
                    {
                        gSideBySide.DrawImage(singleBmp, offset.X, offset.Y);
                    }
                }

                return mergedBmp;
            }
        }

        /// <summary>
        /// The pds data.
        /// </summary>
        public class PdsData
        {
            /// <summary>
            /// The message.
            /// </summary>
            public string Message;

            /// <summary>
            /// The palette id.
            /// </summary>
            public int PaletteId;

            /// <summary>
            /// The palette info.
            /// </summary>
            public PaletteInfo PaletteInfo;

            /// <summary>
            /// The palette version.
            /// </summary>
            public int PaletteVersion;
        }

        /// <summary>
        /// The ods data.
        /// </summary>
        public class OdsData
        {
            /// <summary>
            /// The fragment.
            /// </summary>
            public ImageObjectFragment Fragment;

            /// <summary>
            /// The is first.
            /// </summary>
            public bool IsFirst;

            /// <summary>
            /// The message.
            /// </summary>
            public string Message;

            /// <summary>
            /// The object id.
            /// </summary>
            public int ObjectId;

            /// <summary>
            /// The object version.
            /// </summary>
            public int ObjectVersion;

            /// <summary>
            /// The size.
            /// </summary>
            public Size Size;
        }
    }
}