// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransportStreamParser.cs" company="">
//   
// </copyright>
// <summary>
//   MPEG transport stream parser
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Nikse.SubtitleEdit.Logic.BluRaySup;
    using Nikse.SubtitleEdit.Logic.VobSub;

    /// <summary>
    /// MPEG transport stream parser
    /// </summary>
    public class TransportStreamParser
    {
        /// <summary>
        /// The load transport stream callback.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="total">
        /// The total.
        /// </param>
        public delegate void LoadTransportStreamCallback(long position, long total);

        /// <summary>
        /// Gets the number of null packets.
        /// </summary>
        public int NumberOfNullPackets { get; private set; }

        /// <summary>
        /// Gets the total number of packets.
        /// </summary>
        public long TotalNumberOfPackets { get; private set; }

        /// <summary>
        /// Gets the total number of private stream 1.
        /// </summary>
        public long TotalNumberOfPrivateStream1 { get; private set; }

        /// <summary>
        /// Gets the total number of private stream 1 continuation 0.
        /// </summary>
        public long TotalNumberOfPrivateStream1Continuation0 { get; private set; }

        /// <summary>
        /// Gets the subtitle packet ids.
        /// </summary>
        public List<int> SubtitlePacketIds { get; private set; }

        /// <summary>
        /// Gets the subtitle packets.
        /// </summary>
        public List<Packet> SubtitlePackets { get; private set; }

        // public List<Packet> ProgramAssociationTables { get; private set; }
        /// <summary>
        /// Gets or sets the subtitles lookup.
        /// </summary>
        private Dictionary<int, List<DvbSubPes>> SubtitlesLookup { get; set; }

        /// <summary>
        /// Gets or sets the dvb subtitles lookup.
        /// </summary>
        private Dictionary<int, List<TransportStreamSubtitle>> DvbSubtitlesLookup { get; set; }

        /// <summary>
        /// Gets a value indicating whether is m 2 transport stream.
        /// </summary>
        public bool IsM2TransportStream { get; private set; }

        /// <summary>
        /// Gets the first video pts.
        /// </summary>
        public ulong FirstVideoPts { get; private set; }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void Parse(string fileName, LoadTransportStreamCallback callback)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.Parse(fs, callback);
            }
        }

        /// <summary>
        /// Can be used with e.g. MemoryStream or FileStream
        /// </summary>
        /// <param name="ms">
        /// Input stream
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void Parse(Stream ms, LoadTransportStreamCallback callback)
        {
            bool firstVideoPtsFound = false;
            this.IsM2TransportStream = false;
            this.NumberOfNullPackets = 0;
            this.TotalNumberOfPackets = 0;
            this.TotalNumberOfPrivateStream1 = 0;
            this.TotalNumberOfPrivateStream1Continuation0 = 0;
            this.SubtitlePacketIds = new List<int>();
            this.SubtitlePackets = new List<Packet>();

            // ProgramAssociationTables = new List<Packet>();
            ms.Position = 0;
            const int packetLength = 188;
            this.DetectFormat(ms);
            var packetBuffer = new byte[packetLength];
            var m2TsTimeCodeBuffer = new byte[4];
            long position = 0;

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

                if (this.IsM2TransportStream)
                {
                    ms.Read(m2TsTimeCodeBuffer, 0, m2TsTimeCodeBuffer.Length);

                    // var tc = (m2tsTimeCodeBuffer[0]<< 24) | (m2tsTimeCodeBuffer[1] << 16) | (m2tsTimeCodeBuffer[2] << 8) | (m2tsTimeCodeBuffer[3]);
                    position += m2TsTimeCodeBuffer.Length;
                }

                ms.Read(packetBuffer, 0, packetLength);
                byte syncByte = packetBuffer[0];
                if (syncByte == Packet.SynchronizationByte)
                {
                    var packet = new Packet(packetBuffer);

                    if (packet.IsNullPacket)
                    {
                        this.NumberOfNullPackets++;
                    }
                    else if (!firstVideoPtsFound && packet.IsVideoStream)
                    {
                        if (packet.Payload != null && packet.Payload.Length > 10)
                        {
                            int presentationTimestampDecodeTimestampFlags = packet.Payload[7] >> 6;
                            if (presentationTimestampDecodeTimestampFlags == Helper.B00000010 || presentationTimestampDecodeTimestampFlags == Helper.B00000011)
                            {
                                this.FirstVideoPts = (ulong)packet.Payload[9 + 4] >> 1;
                                this.FirstVideoPts += (ulong)packet.Payload[9 + 3] << 7;
                                this.FirstVideoPts += (ulong)(packet.Payload[9 + 2] & Helper.B11111110) << 14;
                                this.FirstVideoPts += (ulong)packet.Payload[9 + 1] << 22;
                                this.FirstVideoPts += (ulong)(packet.Payload[9 + 0] & Helper.B00001110) << 29;
                                firstVideoPtsFound = true;
                            }
                        }
                    }
                    else if (packet.IsProgramAssociationTable)
                    {
                        // var sb = new StringBuilder();
                        // sb.AppendLine("PacketNo: " + TotalNumberOfPackets + 1);
                        // sb.AppendLine("PacketId: " + packet.PacketId);
                        // sb.AppendLine();
                        // sb.AppendLine("TransportErrorIndicator: " + packet.TransportErrorIndicator);
                        // sb.AppendLine("PayloadUnitStartIndicator: " + packet.PayloadUnitStartIndicator);
                        // sb.AppendLine("TransportPriority: " + packet.TransportPriority);
                        // sb.AppendLine("ScramblingControl: " + packet.ScramblingControl);
                        // sb.AppendLine("AdaptationFieldExist: " + packet.AdaptationFieldControl);
                        // sb.AppendLine("ContinuityCounter: " + packet.ContinuityCounter);
                        // sb.AppendLine();
                        // if (packet.AdaptationField != null)
                        // {
                        // sb.AppendLine("AdaptationFieldLength: " + packet.AdaptationField.Length);
                        // sb.AppendLine("DiscontinuityIndicator: " + packet.AdaptationField.DiscontinuityIndicator);
                        // sb.AppendLine("RandomAccessIndicator: " + packet.AdaptationField.RandomAccessIndicator);
                        // sb.AppendLine("ElementaryStreamPriorityIndicator: " + packet.AdaptationField.ElementaryStreamPriorityIndicator);
                        // sb.AppendLine("PcrFlag: " + packet.AdaptationField.PcrFlag);
                        // sb.AppendLine("OpcrFlag: " + packet.AdaptationField.OpcrFlag);
                        // sb.AppendLine("SplicingPointFlag: " + packet.AdaptationField.SplicingPointFlag);
                        // sb.AppendLine("TransportPrivateDataFlag: " + packet.AdaptationField.TransportPrivateDataFlag);
                        // sb.AppendLine("AdaptationFieldExtensionFlag: " + packet.AdaptationField.AdaptationFieldExtensionFlag);
                        // sb.AppendLine();
                        // }
                        // sb.AppendLine("TableId: " + packet.ProgramAssociationTable.TableId);
                        // sb.AppendLine("SectionLength: " + packet.ProgramAssociationTable.SectionLength);
                        // sb.AppendLine("TransportStreamId: " + packet.ProgramAssociationTable.TransportStreamId);
                        // sb.AppendLine("VersionNumber: " + packet.ProgramAssociationTable.VersionNumber);
                        // sb.AppendLine("CurrentNextIndicator: " + packet.ProgramAssociationTable.CurrentNextIndicator);
                        // sb.AppendLine("SectionNumber: " + packet.ProgramAssociationTable.SectionNumber);
                        // sb.AppendLine("LastSectionNumber: " + packet.ProgramAssociationTable.LastSectionNumber);
                        // ProgramAssociationTables.Add(packet);
                    }
                    else if (packet.IsPrivateStream1 || this.SubtitlePacketIds.Contains(packet.PacketId))
                    {
                        this.TotalNumberOfPrivateStream1++;

                        this.SubtitlePackets.Add(packet);

                        if (!this.SubtitlePacketIds.Contains(packet.PacketId))
                        {
                            this.SubtitlePacketIds.Add(packet.PacketId);
                        }

                        if (packet.ContinuityCounter == 0)
                        {
                            this.TotalNumberOfPrivateStream1Continuation0++;

                            // int pesExtensionlength = 0;
                            // if (12 + packet.AdaptionFieldLength < packetBuffer.Length)
                            // pesExtensionlength = 0xFF & packetBuffer[12 + packet.AdaptionFieldLength];
                            // int pesOffset = 13 + packet.AdaptionFieldLength + pesExtensionlength;
                            // bool isTeletext = (pesExtensionlength == 0x24 && (0xFF & packetBuffer[pesOffset]) >> 4 == 1);

                            //// workaround uk freesat teletext
                            // if (!isTeletext)
                            // isTeletext = (pesExtensionlength == 0x24 && (0xFF & packetBuffer[pesOffset]) == 0x99);

                            // if (!isTeletext)
                            // {

                            // }
                        }

                        if (callback != null)
                        {
                            callback.Invoke(ms.Position, transportStreamLength);
                        }
                    }

                    this.TotalNumberOfPackets++;
                    position += packetLength;
                }
                else
                {
                    position++;
                }
            }

            if (this.IsM2TransportStream)
            {
                this.DvbSubtitlesLookup = new Dictionary<int, List<TransportStreamSubtitle>>();
                this.SubtitlesLookup = new Dictionary<int, List<DvbSubPes>>();
                foreach (int pid in this.SubtitlePacketIds)
                {
                    var bdMs = new MemoryStream();
                    var list = MakeSubtitlePesPackets(pid, this.SubtitlePackets);
                    var startMsList = new List<ulong>();
                    var endMsList = new List<ulong>();
                    foreach (var item in list)
                    {
                        item.WriteToStream(bdMs);
                        if (item.DataIdentifier == 0x16)
                        {
                            if (startMsList.Count <= endMsList.Count)
                            {
                                startMsList.Add(item.PresentationTimestampToMilliseconds());
                            }
                            else
                            {
                                endMsList.Add(item.PresentationTimestampToMilliseconds());
                            }
                        }

                        // else if (item.DataBuffer[0] == 0x80)
                        // { // TODO: Load bd sub after 0x80, so we can be sure to get correct time code???
                        // endMsList.Add(item.PresentationTimestampToMilliseconds() / 90);
                        // }
                    }

                    this.SubtitlesLookup.Add(pid, list);

                    bdMs.Position = 0;
                    var sb = new StringBuilder();
                    var bdList = BluRaySupParser.ParseBluRaySup(bdMs, sb, true);
                    if (bdList.Count > 0)
                    {
                        var subList = new List<TransportStreamSubtitle>();
                        for (int k = 0; k < bdList.Count; k++)
                        {
                            var bdSup = bdList[k];
                            ulong startMs = 0;
                            if (k < startMsList.Count)
                            {
                                startMs = startMsList[k];
                            }

                            ulong endMs = 0;
                            if (k < endMsList.Count)
                            {
                                endMs = endMsList[k];
                            }

                            subList.Add(new TransportStreamSubtitle(bdSup, startMs, endMs, (ulong)((this.FirstVideoPts + 45) / 90.0)));
                        }

                        this.DvbSubtitlesLookup.Add(pid, subList);
                    }
                }

                this.SubtitlePacketIds.Clear();
                foreach (int key in this.DvbSubtitlesLookup.Keys)
                {
                    this.SubtitlePacketIds.Add(key);
                }

                this.SubtitlePacketIds.Sort();
                return;
            }

            // check for SubPictureStreamId = 32
            this.SubtitlesLookup = new Dictionary<int, List<DvbSubPes>>();
            foreach (int pid in this.SubtitlePacketIds)
            {
                var list = MakeSubtitlePesPackets(pid, this.SubtitlePackets);
                bool hasImageSubtitles = false;
                foreach (var item in list)
                {
                    if (item.IsDvbSubpicture)
                    {
                        hasImageSubtitles = true;
                        break;
                    }
                }

                if (hasImageSubtitles)
                {
                    this.SubtitlesLookup.Add(pid, list);
                }
            }

            this.SubtitlePacketIds.Clear();
            foreach (int key in this.SubtitlesLookup.Keys)
            {
                this.SubtitlePacketIds.Add(key);
            }

            this.SubtitlePacketIds.Sort();

            // Merge packets and set start/end time
            this.DvbSubtitlesLookup = new Dictionary<int, List<TransportStreamSubtitle>>();
            var firstVideoMs = (ulong)((this.FirstVideoPts + 45) / 90.0);
            foreach (int pid in this.SubtitlePacketIds)
            {
                var subtitles = new List<TransportStreamSubtitle>();
                var list = this.GetSubtitlePesPackets(pid);
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var pes = list[i];
                        pes.ParseSegments();
                        if (pes.ObjectDataList.Count > 0)
                        {
                            var sub = new TransportStreamSubtitle();
                            sub.StartMilliseconds = pes.PresentationTimestampToMilliseconds();
                            sub.Pes = pes;
                            if (i + 1 < list.Count && list[i + 1].PresentationTimestampToMilliseconds() > 25)
                            {
                                sub.EndMilliseconds = list[i + 1].PresentationTimestampToMilliseconds() - 25;
                            }

                            if (sub.EndMilliseconds < sub.StartMilliseconds)
                            {
                                sub.EndMilliseconds = sub.StartMilliseconds + 3500;
                            }

                            subtitles.Add(sub);
                            if (sub.StartMilliseconds < firstVideoMs)
                            {
                                firstVideoMs = sub.StartMilliseconds;
                            }
                        }
                    }
                }

                foreach (var s in subtitles)
                {
                    s.OffsetMilliseconds = firstVideoMs;
                }

                this.DvbSubtitlesLookup.Add(pid, subtitles);
            }

            this.SubtitlePacketIds.Clear();
            foreach (int key in this.DvbSubtitlesLookup.Keys)
            {
                if (this.DvbSubtitlesLookup[key].Count > 0)
                {
                    this.SubtitlePacketIds.Add(key);
                }
            }

            this.SubtitlePacketIds.Sort();
        }

        /// <summary>
        /// The get dvb subtitles.
        /// </summary>
        /// <param name="packetId">
        /// The packet id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<TransportStreamSubtitle> GetDvbSubtitles(int packetId)
        {
            if (this.DvbSubtitlesLookup.ContainsKey(packetId))
            {
                return this.DvbSubtitlesLookup[packetId];
            }

            return null;
        }

        /// <summary>
        /// The make subtitle pes packets.
        /// </summary>
        /// <param name="packetId">
        /// The packet id.
        /// </param>
        /// <param name="subtitlePackets">
        /// The subtitle packets.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<DvbSubPes> MakeSubtitlePesPackets(int packetId, List<Packet> subtitlePackets)
        {
            var list = new List<DvbSubPes>();
            int last = -1;
            var packetList = new List<Packet>();
            foreach (Packet packet in subtitlePackets)
            {
                if (packet.PacketId == packetId)
                {
                    if (packet.PayloadUnitStartIndicator)
                    {
                        if (packetList.Count > 0)
                        {
                            AddPesPacket(list, packetList);
                        }

                        packetList = new List<Packet>();
                    }

                    if (packet.Payload != null && last != packet.ContinuityCounter)
                    {
                        packetList.Add(packet);
                    }

                    last = packet.ContinuityCounter;
                }
            }

            if (packetList.Count > 0)
            {
                AddPesPacket(list, packetList);
            }

            return list;
        }

        /// <summary>
        /// The get subtitle pes packets.
        /// </summary>
        /// <param name="packetId">
        /// The packet id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<DvbSubPes> GetSubtitlePesPackets(int packetId)
        {
            if (this.SubtitlesLookup.ContainsKey(packetId))
            {
                return this.SubtitlesLookup[packetId];
            }

            return null;
        }

        /// <summary>
        /// The add pes packet.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="packetList">
        /// The packet list.
        /// </param>
        private static void AddPesPacket(List<DvbSubPes> list, List<Packet> packetList)
        {
            int bufferSize = 0;
            foreach (Packet p in packetList)
            {
                bufferSize += p.Payload.Length;
            }

            var pesData = new byte[bufferSize];
            int pesIndex = 0;
            foreach (Packet p in packetList)
            {
                Buffer.BlockCopy(p.Payload, 0, pesData, pesIndex, p.Payload.Length);
                pesIndex += p.Payload.Length;
            }

            DvbSubPes pes;
            if (VobSubParser.IsMpeg2PackHeader(pesData))
            {
                pes = new DvbSubPes(pesData, Mpeg2Header.Length);
            }
            else if (VobSubParser.IsPrivateStream1(pesData, 0))
            {
                pes = new DvbSubPes(pesData, 0);
            }
            else
            {
                pes = new DvbSubPes(pesData, 0);
            }

            list.Add(pes);
        }

        /// <summary>
        /// The detect format.
        /// </summary>
        /// <param name="ms">
        /// The ms.
        /// </param>
        internal void DetectFormat(Stream ms)
        {
            if (ms.Length > 192 + 192 + 5)
            {
                ms.Seek(0, SeekOrigin.Begin);
                var buffer = new byte[192 + 192 + 5];
                ms.Read(buffer, 0, buffer.Length);
                if (buffer[0] == Packet.SynchronizationByte && buffer[188] == Packet.SynchronizationByte)
                {
                    return;
                }

                if (buffer[4] == Packet.SynchronizationByte && buffer[192 + 4] == Packet.SynchronizationByte && buffer[192 + 192 + 4] == Packet.SynchronizationByte)
                {
                    this.IsM2TransportStream = true;
                }
            }
        }

        /// <summary>
        /// The is dvb sup.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDvbSup(string fileName)
        {
            try
            {
                byte[] pesData = File.ReadAllBytes(fileName);
                if (pesData[0] != 0x20 || pesData[1] != 0 || pesData[2] != 0x0F)
                {
                    return false;
                }

                var pes = new DvbSubPes(0, pesData);
                return pes.SubtitleSegments.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The get dvb sup.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<TransportStreamSubtitle> GetDvbSup(string fileName)
        {
            byte[] pesData = File.ReadAllBytes(fileName);
            var list = new List<DvbSubPes>();
            int index = 0;
            while (index < pesData.Length - 10)
            {
                var pes = new DvbSubPes(index, pesData);
                index = pes.Length + 1;
                list.Add(pes);
            }

            var subtitles = new List<TransportStreamSubtitle>();
            int seconds = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var pes = list[i];
                pes.ParseSegments();
                if (pes.ObjectDataList.Count > 0)
                {
                    var sub = new TransportStreamSubtitle();
                    sub.StartMilliseconds = (ulong)seconds * 1000UL;
                    seconds += pes.PageCompositions[0].PageTimeOut;
                    if (pes.PageCompositions.Count > 0)
                    {
                        sub.EndMilliseconds = sub.StartMilliseconds + (ulong)pes.PageCompositions[0].PageTimeOut * 1000UL;
                    }
                    else
                    {
                        sub.EndMilliseconds = sub.StartMilliseconds + 2500;
                    }

                    sub.Pes = pes;
                    subtitles.Add(sub);
                }

                if (pes.PageCompositions.Count > 0)
                {
                    seconds += pes.PageCompositions[0].PageTimeOut;
                }
            }

            return subtitles;
        }
    }
}