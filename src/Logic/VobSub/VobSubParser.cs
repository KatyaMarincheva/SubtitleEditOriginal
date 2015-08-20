// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubParser.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// The vob sub parser.
    /// </summary>
    public class VobSubParser
    {
        /// <summary>
        /// The packetized elementary stream maximum length.
        /// </summary>
        private const int PacketizedElementaryStreamMaximumLength = 2028;

        /// <summary>
        /// The idx languages.
        /// </summary>
        public List<string> IdxLanguages = new List<string>();

        /// <summary>
        /// The idx palette.
        /// </summary>
        public List<Color> IdxPalette = new List<Color>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubParser"/> class.
        /// </summary>
        /// <param name="isPal">
        /// The is pal.
        /// </param>
        public VobSubParser(bool isPal)
        {
            this.IsPal = isPal;
            this.VobSubPacks = new List<VobSubPack>();
        }

        /// <summary>
        /// Gets a value indicating whether is pal.
        /// </summary>
        public bool IsPal { get; private set; }

        /// <summary>
        /// Gets the vob sub packs.
        /// </summary>
        public List<VobSubPack> VobSubPacks { get; private set; }

        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void Open(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.Open(fs);
            }
        }

        /// <summary>
        /// Can be used with e.g. MemoryStream or FileStream
        /// </summary>
        /// <param name="ms">
        /// </param>
        public void Open(Stream ms)
        {
            this.VobSubPacks = new List<VobSubPack>();
            ms.Position = 0;
            var buffer = new byte[0x800]; // 2048
            long position = 0;
            while (position < ms.Length)
            {
                ms.Seek(position, SeekOrigin.Begin);
                ms.Read(buffer, 0, 0x0800);
                if (IsSubtitlePack(buffer))
                {
                    this.VobSubPacks.Add(new VobSubPack(buffer, null));
                }

                position += 0x800;
            }
        }

        /// <summary>
        /// The open sub idx.
        /// </summary>
        /// <param name="vobSubFileName">
        /// The vob sub file name.
        /// </param>
        /// <param name="idxFileName">
        /// The idx file name.
        /// </param>
        internal void OpenSubIdx(string vobSubFileName, string idxFileName)
        {
            this.VobSubPacks = new List<VobSubPack>();

            if (!string.IsNullOrEmpty(idxFileName) && File.Exists(idxFileName))
            {
                var idx = new Idx(idxFileName);
                this.IdxPalette = idx.Palette;
                this.IdxLanguages = idx.Languages;
                if (idx.IdxParagraphs.Count > 0)
                {
                    var buffer = new byte[0x800]; // 2048
                    using (var fs = new FileStream(vobSubFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        foreach (var p in idx.IdxParagraphs)
                        {
                            if (p.FilePosition + 100 < fs.Length)
                            {
                                long position = p.FilePosition;
                                fs.Seek(position, SeekOrigin.Begin);
                                fs.Read(buffer, 0, 0x0800);
                                if (IsSubtitlePack(buffer) || IsPrivateStream1(buffer, 0))
                                {
                                    var vsp = new VobSubPack(buffer, p);
                                    this.VobSubPacks.Add(vsp);

                                    if (IsPrivateStream1(buffer, 0))
                                    {
                                        position += vsp.PacketizedElementaryStream.Length + 6;
                                    }
                                    else
                                    {
                                        position += 0x800;
                                    }

                                    int currentSubPictureStreamId = vsp.PacketizedElementaryStream.SubPictureStreamId.Value;
                                    while (vsp.PacketizedElementaryStream != null && vsp.PacketizedElementaryStream.SubPictureStreamId.HasValue && (vsp.PacketizedElementaryStream.Length == PacketizedElementaryStreamMaximumLength || currentSubPictureStreamId != vsp.PacketizedElementaryStream.SubPictureStreamId.Value) && position < fs.Length)
                                    {
                                        fs.Seek(position, SeekOrigin.Begin);
                                        fs.Read(buffer, 0, 0x800);
                                        vsp = new VobSubPack(buffer, p); // idx position?

                                        if (vsp.PacketizedElementaryStream != null && vsp.PacketizedElementaryStream.SubPictureStreamId.HasValue && currentSubPictureStreamId == vsp.PacketizedElementaryStream.SubPictureStreamId.Value)
                                        {
                                            this.VobSubPacks.Add(vsp);

                                            if (IsPrivateStream1(buffer, 0))
                                            {
                                                position += vsp.PacketizedElementaryStream.Length + 6;
                                            }
                                            else
                                            {
                                                position += 0x800;
                                            }
                                        }
                                        else
                                        {
                                            position += 0x800;
                                            fs.Seek(position, SeekOrigin.Begin);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return;
                }
            }

            // No valid idx file found - just open like vob file
            this.Open(vobSubFileName);
        }

        /// <summary>
        /// Demultiplex multiplexed packs together each streamId at a time + removing bad packs + fixing displaytimes
        /// </summary>
        /// <returns>List of complete packs each with a complete sub image</returns>
        public List<VobSubMergedPack> MergeVobSubPacks()
        {
            var list = new List<VobSubMergedPack>();
            var pts = new TimeSpan();
            var ms = new MemoryStream();
            int streamId = 0;

            float ticksPerMillisecond = 90.000F;
            if (!this.IsPal)
            {
                ticksPerMillisecond = 90.090F; // TODO: What should this be for NTSC?
            }

            // get unique streamIds
            var uniqueStreamIds = new List<int>();
            foreach (var p in this.VobSubPacks)
            {
                if (p.PacketizedElementaryStream != null && p.PacketizedElementaryStream.SubPictureStreamId.HasValue && !uniqueStreamIds.Contains(p.PacketizedElementaryStream.SubPictureStreamId.Value))
                {
                    uniqueStreamIds.Add(p.PacketizedElementaryStream.SubPictureStreamId.Value);
                }
            }

            IdxParagraph lastIdxParagraph = null;
            foreach (int uniqueStreamId in uniqueStreamIds)
            {
                // packets must be merged in streamId order (so they don't get mixed)
                foreach (var p in this.VobSubPacks)
                {
                    if (p.PacketizedElementaryStream != null && p.PacketizedElementaryStream.SubPictureStreamId.HasValue && p.PacketizedElementaryStream.SubPictureStreamId.Value == uniqueStreamId)
                    {
                        if (p.PacketizedElementaryStream.PresentationTimestampDecodeTimestampFlags > 0)
                        {
                            if (lastIdxParagraph == null || p.IdxLine.FilePosition != lastIdxParagraph.FilePosition)
                            {
                                if (ms.Length > 0)
                                {
                                    list.Add(new VobSubMergedPack(ms.ToArray(), pts, streamId, lastIdxParagraph));
                                }

                                ms.Close();
                                ms = new MemoryStream();
                                pts = TimeSpan.FromMilliseconds(Convert.ToDouble(p.PacketizedElementaryStream.PresentationTimestamp / ticksPerMillisecond)); // 90000F * 1000)); (PAL)
                                streamId = p.PacketizedElementaryStream.SubPictureStreamId.Value;
                            }
                        }

                        lastIdxParagraph = p.IdxLine;
                        p.PacketizedElementaryStream.WriteToStream(ms);
                    }
                }

                if (ms.Length > 0)
                {
                    list.Add(new VobSubMergedPack(ms.ToArray(), pts, streamId, lastIdxParagraph));
                    ms.Close();
                    ms = new MemoryStream();
                }
            }

            ms.Close();

            // Remove any bad packs
            for (int i = list.Count - 1; i >= 0; i--)
            {
                VobSubMergedPack pack = list[i];
                if (pack.SubPicture == null || pack.SubPicture.ImageDisplayArea.Width <= 3 || pack.SubPicture.ImageDisplayArea.Height <= 2)
                {
                    list.RemoveAt(i);
                }
                else if (pack.EndTime.TotalSeconds - pack.StartTime.TotalSeconds < 0.1 && pack.SubPicture.ImageDisplayArea.Width <= 10 && pack.SubPicture.ImageDisplayArea.Height <= 10)
                {
                    list.RemoveAt(i);
                }
            }

            // Fix subs with no duration (completely normal) or negative duration or duration > 10 seconds
            for (int i = 0; i < list.Count; i++)
            {
                VobSubMergedPack pack = list[i];
                if (pack.SubPicture.Delay.TotalMilliseconds > 0)
                {
                    pack.EndTime = pack.StartTime.Add(pack.SubPicture.Delay);
                }

                if (pack.EndTime < pack.StartTime || pack.EndTime.TotalSeconds - pack.StartTime.TotalSeconds > 10.0)
                {
                    if (i + 1 < list.Count)
                    {
                        pack.EndTime = TimeSpan.FromMilliseconds(list[i].StartTime.TotalMilliseconds - 100);
                    }
                    else
                    {
                        pack.EndTime = TimeSpan.FromMilliseconds(pack.StartTime.TotalMilliseconds + 3000);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// The is mpeg 2 pack header.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsMpeg2PackHeader(byte[] buffer)
        {
            return buffer.Length >= 3 && buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 1 && buffer[3] == 0xba; // 0xba == 186 - MPEG-2 Pack Header
        }

        /// <summary>
        /// The is private stream 1.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsPrivateStream1(byte[] buffer, int index)
        {
            return buffer.Length >= index + 3 && buffer[index + 0] == 0 && buffer[index + 1] == 0 && buffer[index + 2] == 1 && buffer[index + 3] == 0xbd; // 0xbd == 189 - MPEG-2 Private stream 1 (non MPEG audio, subpictures)
        }

        /// <summary>
        /// The is private stream 2.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsPrivateStream2(byte[] buffer, int index)
        {
            return buffer.Length >= index + 3 && buffer[index + 0] == 0 && buffer[index + 1] == 0 && buffer[index + 2] == 1 && buffer[index + 3] == 0xbf; // 0xbf == 191 - MPEG-2 Private stream 2
        }

        /// <summary>
        /// The is subtitle pack.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool IsSubtitlePack(byte[] buffer)
        {
            if (IsMpeg2PackHeader(buffer) && IsPrivateStream1(buffer, Mpeg2Header.Length))
            {
                int pesHeaderDataLength = buffer[Mpeg2Header.Length + 8];
                int streamId = buffer[Mpeg2Header.Length + 8 + 1 + pesHeaderDataLength];
                if (streamId >= 0x20 && streamId <= 0x3f)
                {
                    // Subtitle IDs allowed (or x3f to x40?)
                    return true;
                }
            }

            return false;
        }
    }
}