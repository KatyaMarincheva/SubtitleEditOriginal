// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatroskaFile.cs" company="">
//   
// </copyright>
// <summary>
//   The matroska file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Nikse.SubtitleEdit.Core;
    using Nikse.SubtitleEdit.Logic.ContainerFormats.Ebml;

    /// <summary>
    /// The matroska file.
    /// </summary>
    internal class MatroskaFile : IDisposable
    {
        /// <summary>
        /// The load matroska callback.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="total">
        /// The total.
        /// </param>
        public delegate void LoadMatroskaCallback(long position, long total);

        /// <summary>
        /// The _path.
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The _segment element.
        /// </summary>
        private readonly Element _segmentElement;

        /// <summary>
        /// The _stream.
        /// </summary>
        private readonly FileStream _stream;

        /// <summary>
        /// The _subtitle rip.
        /// </summary>
        private readonly List<MatroskaSubtitle> _subtitleRip = new List<MatroskaSubtitle>();

        /// <summary>
        /// The _valid.
        /// </summary>
        private readonly bool _valid;

        /// <summary>
        /// The _duration.
        /// </summary>
        private double _duration;

        /// <summary>
        /// The _frame rate.
        /// </summary>
        private double _frameRate;

        /// <summary>
        /// The _pixel height.
        /// </summary>
        private int _pixelHeight;

        /// <summary>
        /// The _pixel width.
        /// </summary>
        private int _pixelWidth;

        /// <summary>
        /// The _subtitle rip track number.
        /// </summary>
        private int _subtitleRipTrackNumber;

        /// <summary>
        /// The _timecode scale.
        /// </summary>
        private long _timecodeScale = 1000000;

        /// <summary>
        /// The _tracks.
        /// </summary>
        private List<MatroskaTrackInfo> _tracks;

        /// <summary>
        /// The _video codec id.
        /// </summary>
        private string _videoCodecId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatroskaFile"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public MatroskaFile(string path)
        {
            this._path = path;
            this._stream = new FastFileStream(path);

            // read header
            Element headerElement = this.ReadElement();
            if (headerElement != null && headerElement.Id == ElementId.Ebml)
            {
                // read segment
                this._stream.Seek(headerElement.DataSize, SeekOrigin.Current);
                this._segmentElement = this.ReadElement();
                if (this._segmentElement != null && this._segmentElement.Id == ElementId.Segment)
                {
                    this._valid = true; // matroska file must start with ebml header and segment
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this._valid;
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path
        {
            get
            {
                return this._path;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this._stream != null)
            {
                this._stream.Dispose();
            }
        }

        /// <summary>
        /// The get tracks.
        /// </summary>
        /// <param name="subtitleOnly">
        /// The subtitle only.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<MatroskaTrackInfo> GetTracks(bool subtitleOnly = false)
        {
            this.ReadSegmentInfoAndTracks();

            if (this._tracks == null)
            {
                return new List<MatroskaTrackInfo>();
            }

            return subtitleOnly ? this._tracks.Where(t => t.IsSubtitle).ToList() : this._tracks;
        }

        /// <summary>
        /// Get first time of track
        /// </summary>
        /// <param name="trackNumber">
        /// Track number
        /// </param>
        /// <returns>
        /// Start time in milliseconds
        /// </returns>
        public long GetTrackStartTime(int trackNumber)
        {
            // go to segment
            this._stream.Seek(this._segmentElement.DataPosition, SeekOrigin.Begin);

            Element element;
            while (this._stream.Position < this._stream.Length && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.Info:
                        this.ReadInfoElement(element);
                        break;
                    case ElementId.Tracks:
                        this.ReadTracksElement(element);
                        break;
                    case ElementId.Cluster:
                        return this.FindTrackStartInCluster(element, trackNumber);
                }

                this._stream.Seek(element.EndPosition, SeekOrigin.Begin);
            }

            return 0;
        }

        /// <summary>
        /// The get info.
        /// </summary>
        /// <param name="frameRate">
        /// The frame Rate.
        /// </param>
        /// <param name="pixelWidth">
        /// The pixel Width.
        /// </param>
        /// <param name="pixelHeight">
        /// The pixel Height.
        /// </param>
        /// <param name="duration">
        /// Duration of the segment in milliseconds.
        /// </param>
        /// <param name="videoCodec">
        /// The video Codec.
        /// </param>
        public void GetInfo(out double frameRate, out int pixelWidth, out int pixelHeight, out double duration, out string videoCodec)
        {
            this.ReadSegmentInfoAndTracks();

            pixelWidth = this._pixelWidth;
            pixelHeight = this._pixelHeight;
            frameRate = this._frameRate;
            duration = this._duration;
            videoCodec = this._videoCodecId;
        }

        /// <summary>
        /// The get subtitle.
        /// </summary>
        /// <param name="trackNumber">
        /// The track number.
        /// </param>
        /// <param name="progressCallback">
        /// The progress callback.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<MatroskaSubtitle> GetSubtitle(int trackNumber, LoadMatroskaCallback progressCallback)
        {
            this._subtitleRipTrackNumber = trackNumber;
            this.ReadSegmentCluster(progressCallback);
            return this._subtitleRip;
        }

        /// <summary>
        /// The find track start in cluster.
        /// </summary>
        /// <param name="cluster">
        /// The cluster.
        /// </param>
        /// <param name="targetTrackNumber">
        /// The target track number.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long FindTrackStartInCluster(Element cluster, int targetTrackNumber)
        {
            long clusterTimeCode = 0L;
            long trackStartTime = -1L;
            bool done = false;

            Element element;
            while (this._stream.Position < cluster.EndPosition && (element = this.ReadElement()) != null && !done)
            {
                switch (element.Id)
                {
                    case ElementId.None:
                        done = true;
                        break;
                    case ElementId.Timecode:

                        // Absolute timestamp of the cluster (based on TimecodeScale)
                        clusterTimeCode = (long)this.ReadUInt((int)element.DataSize);
                        break;
                    case ElementId.BlockGroup:
                        this.ReadBlockGroupElement(element, clusterTimeCode);
                        break;
                    case ElementId.SimpleBlock:
                        int trackNumber = (int)this.ReadVariableLengthUInt();
                        if (trackNumber == targetTrackNumber)
                        {
                            // Timecode (relative to Cluster timecode, signed int16)
                            trackStartTime = this.ReadInt16();
                            done = true;
                        }

                        break;
                }

                this._stream.Seek(element.EndPosition, SeekOrigin.Begin);
            }

            return (clusterTimeCode + trackStartTime) * this._timecodeScale / 1000000;
        }

        /// <summary>
        /// The read video element.
        /// </summary>
        /// <param name="videoElement">
        /// The video element.
        /// </param>
        private void ReadVideoElement(Element videoElement)
        {
            Element element;
            while (this._stream.Position < videoElement.EndPosition && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.PixelWidth:
                        this._pixelWidth = (int)this.ReadUInt((int)element.DataSize);
                        break;
                    case ElementId.PixelHeight:
                        this._pixelHeight = (int)this.ReadUInt((int)element.DataSize);
                        break;
                    default:
                        this._stream.Seek(element.DataSize, SeekOrigin.Current);
                        break;
                }
            }
        }

        /// <summary>
        /// The read track entry element.
        /// </summary>
        /// <param name="trackEntryElement">
        /// The track entry element.
        /// </param>
        private void ReadTrackEntryElement(Element trackEntryElement)
        {
            long defaultDuration = 0;
            bool isVideo = false;
            bool isAudio = false;
            bool isSubtitle = false;
            int trackNumber = 0;
            string name = string.Empty;
            string language = string.Empty;
            string codecId = string.Empty;
            string codecPrivate = string.Empty;

            // var biCompression = string.Empty;
            int contentCompressionAlgorithm = -1;
            int contentEncodingType = -1;

            Element element;
            while (this._stream.Position < trackEntryElement.EndPosition && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.DefaultDuration:
                        defaultDuration = (int)this.ReadUInt((int)element.DataSize);
                        break;
                    case ElementId.Video:
                        this.ReadVideoElement(element);
                        isVideo = true;
                        break;
                    case ElementId.Audio:
                        isAudio = true;
                        break;
                    case ElementId.TrackNumber:
                        trackNumber = (int)this.ReadUInt((int)element.DataSize);
                        break;
                    case ElementId.Name:
                        name = this.ReadString((int)element.DataSize, Encoding.UTF8);
                        break;
                    case ElementId.Language:
                        language = this.ReadString((int)element.DataSize, Encoding.ASCII);
                        break;
                    case ElementId.CodecId:
                        codecId = this.ReadString((int)element.DataSize, Encoding.ASCII);
                        break;
                    case ElementId.TrackType:
                        switch (this._stream.ReadByte())
                        {
                            case 1:
                                isVideo = true;
                                break;
                            case 2:
                                isAudio = true;
                                break;
                            case 17:
                                isSubtitle = true;
                                break;
                        }

                        break;
                    case ElementId.CodecPrivate:
                        codecPrivate = this.ReadString((int)element.DataSize, Encoding.UTF8);

                        // if (codecPrivate.Length > 20)
                        // biCompression = codecPrivate.Substring(16, 4);
                        break;
                    case ElementId.ContentEncodings:
                        contentCompressionAlgorithm = 0; // default value
                        contentEncodingType = 0; // default value

                        Element contentEncodingElement = this.ReadElement();
                        if (contentEncodingElement != null && contentEncodingElement.Id == ElementId.ContentEncoding)
                        {
                            this.ReadContentEncodingElement(element, ref contentCompressionAlgorithm, ref contentEncodingType);
                        }

                        break;
                }

                this._stream.Seek(element.EndPosition, SeekOrigin.Begin);
            }

            this._tracks.Add(new MatroskaTrackInfo { TrackNumber = trackNumber, IsVideo = isVideo, IsAudio = isAudio, IsSubtitle = isSubtitle, Language = language, CodecId = codecId, CodecPrivate = codecPrivate, Name = name, ContentEncodingType = contentEncodingType, ContentCompressionAlgorithm = contentCompressionAlgorithm });

            if (isVideo)
            {
                if (defaultDuration > 0)
                {
                    this._frameRate = 1.0 / (defaultDuration / 1000000000.0);
                }

                this._videoCodecId = codecId;
            }
        }

        /// <summary>
        /// The read content encoding element.
        /// </summary>
        /// <param name="contentEncodingElement">
        /// The content encoding element.
        /// </param>
        /// <param name="contentCompressionAlgorithm">
        /// The content compression algorithm.
        /// </param>
        /// <param name="contentEncodingType">
        /// The content encoding type.
        /// </param>
        private void ReadContentEncodingElement(Element contentEncodingElement, ref int contentCompressionAlgorithm, ref int contentEncodingType)
        {
            Element element;
            while (this._stream.Position < contentEncodingElement.EndPosition && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.ContentEncodingOrder:
                        ulong contentEncodingOrder = this.ReadUInt((int)element.DataSize);
                        Debug.WriteLine("ContentEncodingOrder: " + contentEncodingOrder);
                        break;
                    case ElementId.ContentEncodingScope:
                        ulong contentEncodingScope = this.ReadUInt((int)element.DataSize);
                        Debug.WriteLine("ContentEncodingScope: " + contentEncodingScope);
                        break;
                    case ElementId.ContentEncodingType:
                        contentEncodingType = (int)this.ReadUInt((int)element.DataSize);
                        break;
                    case ElementId.ContentCompression:
                        Element compElement;
                        while (this._stream.Position < element.EndPosition && (compElement = this.ReadElement()) != null)
                        {
                            switch (compElement.Id)
                            {
                                case ElementId.ContentCompAlgo:
                                    contentCompressionAlgorithm = (int)this.ReadUInt((int)compElement.DataSize);
                                    break;
                                case ElementId.ContentCompSettings:
                                    ulong contentCompSettings = this.ReadUInt((int)compElement.DataSize);
                                    Debug.WriteLine("ContentCompSettings: " + contentCompSettings);
                                    break;
                                default:
                                    this._stream.Seek(element.DataSize, SeekOrigin.Current);
                                    break;
                            }
                        }

                        break;
                    default:
                        this._stream.Seek(element.DataSize, SeekOrigin.Current);
                        break;
                }
            }
        }

        /// <summary>
        /// The read info element.
        /// </summary>
        /// <param name="infoElement">
        /// The info element.
        /// </param>
        private void ReadInfoElement(Element infoElement)
        {
            Element element;
            while (this._stream.Position < infoElement.EndPosition && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.TimecodeScale:

                        // Timestamp scale in nanoseconds (1.000.000 means all timestamps in the segment are expressed in milliseconds)
                        this._timecodeScale = (int)this.ReadUInt((int)element.DataSize);
                        break;
                    case ElementId.Duration:

                        // Duration of the segment (based on TimecodeScale)
                        this._duration = element.DataSize == 4 ? this.ReadFloat32() : this.ReadFloat64();
                        this._duration /= this._timecodeScale * 1000000.0;
                        break;
                    default:
                        this._stream.Seek(element.DataSize, SeekOrigin.Current);
                        break;
                }
            }
        }

        /// <summary>
        /// The read tracks element.
        /// </summary>
        /// <param name="tracksElement">
        /// The tracks element.
        /// </param>
        private void ReadTracksElement(Element tracksElement)
        {
            this._tracks = new List<MatroskaTrackInfo>();

            Element element;
            while (this._stream.Position < tracksElement.EndPosition && (element = this.ReadElement()) != null)
            {
                if (element.Id == ElementId.TrackEntry)
                {
                    this.ReadTrackEntryElement(element);
                }
                else
                {
                    this._stream.Seek(element.DataSize, SeekOrigin.Current);
                }
            }
        }

        /// <summary>
        /// The read cluster.
        /// </summary>
        /// <param name="clusterElement">
        /// The cluster element.
        /// </param>
        private void ReadCluster(Element clusterElement)
        {
            long clusterTimeCode = 0;

            Element element;
            while (this._stream.Position < clusterElement.EndPosition && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.Timecode:
                        clusterTimeCode = (long)this.ReadUInt((int)element.DataSize);
                        break;
                    case ElementId.BlockGroup:
                        this.ReadBlockGroupElement(element, clusterTimeCode);
                        break;
                    case ElementId.SimpleBlock:
                        MatroskaSubtitle subtitle = this.ReadSubtitleBlock(element, clusterTimeCode);
                        if (subtitle != null)
                        {
                            this._subtitleRip.Add(subtitle);
                        }

                        break;
                    default:
                        this._stream.Seek(element.DataSize, SeekOrigin.Current);
                        break;
                }
            }
        }

        /// <summary>
        /// The read block group element.
        /// </summary>
        /// <param name="clusterElement">
        /// The cluster element.
        /// </param>
        /// <param name="clusterTimeCode">
        /// The cluster time code.
        /// </param>
        private void ReadBlockGroupElement(Element clusterElement, long clusterTimeCode)
        {
            MatroskaSubtitle subtitle = null;

            Element element;
            while (this._stream.Position < clusterElement.EndPosition && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.Block:
                        subtitle = this.ReadSubtitleBlock(element, clusterTimeCode);
                        if (subtitle == null)
                        {
                            return;
                        }

                        this._subtitleRip.Add(subtitle);
                        break;
                    case ElementId.BlockDuration:
                        long duration = (long)this.ReadUInt((int)element.DataSize);
                        if (subtitle != null)
                        {
                            subtitle.Duration = duration;
                        }

                        break;
                    default:
                        this._stream.Seek(element.DataSize, SeekOrigin.Current);
                        break;
                }
            }
        }

        /// <summary>
        /// The read subtitle block.
        /// </summary>
        /// <param name="blockElement">
        /// The block element.
        /// </param>
        /// <param name="clusterTimeCode">
        /// The cluster time code.
        /// </param>
        /// <returns>
        /// The <see cref="MatroskaSubtitle"/>.
        /// </returns>
        private MatroskaSubtitle ReadSubtitleBlock(Element blockElement, long clusterTimeCode)
        {
            int trackNumber = (int)this.ReadVariableLengthUInt();
            if (trackNumber != this._subtitleRipTrackNumber)
            {
                this._stream.Seek(blockElement.EndPosition, SeekOrigin.Begin);
                return null;
            }

            short timeCode = this.ReadInt16();

            // lacing
            byte flags = (byte)this._stream.ReadByte();
            int frames;
            switch (flags & 6)
            {
                case 0: // 00000000 = No lacing
                    Debug.Print("No lacing");
                    break;
                case 2: // 00000010 = Xiph lacing
                    frames = this._stream.ReadByte() + 1;
                    Debug.Print("Xiph lacing ({0} frames)", frames);
                    break;
                case 4: // 00000100 = Fixed-size lacing
                    frames = this._stream.ReadByte() + 1;
                    for (int i = 0; i < frames; i++)
                    {
                        this._stream.ReadByte(); // frames
                    }

                    Debug.Print("Fixed-size lacing ({0} frames)", frames);
                    break;
                case 6: // 00000110 = EMBL lacing
                    frames = this._stream.ReadByte() + 1;
                    Debug.Print("EBML lacing ({0} frames)", frames);
                    break;
            }

            // save subtitle data
            int dataLength = (int)(blockElement.EndPosition - this._stream.Position);
            byte[] data = new byte[dataLength];
            this._stream.Read(data, 0, dataLength);

            return new MatroskaSubtitle(data, clusterTimeCode + timeCode);
        }

        /// <summary>
        /// The read segment info and tracks.
        /// </summary>
        private void ReadSegmentInfoAndTracks()
        {
            // go to segment
            this._stream.Seek(this._segmentElement.DataPosition, SeekOrigin.Begin);

            Element element;
            while (this._stream.Position < this._segmentElement.EndPosition && (element = this.ReadElement()) != null)
            {
                switch (element.Id)
                {
                    case ElementId.Info:
                        this.ReadInfoElement(element);
                        break;
                    case ElementId.Tracks:
                        this.ReadTracksElement(element);
                        return;
                    default:
                        this._stream.Seek(element.DataSize, SeekOrigin.Current);
                        break;
                }
            }
        }

        /// <summary>
        /// The read segment cluster.
        /// </summary>
        /// <param name="progressCallback">
        /// The progress callback.
        /// </param>
        private void ReadSegmentCluster(LoadMatroskaCallback progressCallback)
        {
            // go to segment
            this._stream.Seek(this._segmentElement.DataPosition, SeekOrigin.Begin);

            Element element;
            while (this._stream.Position < this._segmentElement.EndPosition && (element = this.ReadElement()) != null)
            {
                if (element.Id == ElementId.Cluster)
                {
                    this.ReadCluster(element);
                }
                else
                {
                    this._stream.Seek(element.DataSize, SeekOrigin.Current);
                }

                if (progressCallback != null)
                {
                    progressCallback.Invoke(element.EndPosition, this._stream.Length);
                }
            }
        }

        /// <summary>
        /// The read element.
        /// </summary>
        /// <returns>
        /// The <see cref="Element"/>.
        /// </returns>
        private Element ReadElement()
        {
            ElementId id = (ElementId)this.ReadVariableLengthUInt(false);
            if (id <= ElementId.None)
            {
                return null;
            }

            long size = (long)this.ReadVariableLengthUInt();
            return new Element(id, this._stream.Position, size);
        }

        /// <summary>
        /// The read variable length u int.
        /// </summary>
        /// <param name="unsetFirstBit">
        /// The unset first bit.
        /// </param>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        private ulong ReadVariableLengthUInt(bool unsetFirstBit = true)
        {
            // Begin loop with byte set to newly read byte
            int first = this._stream.ReadByte();
            int length = 0;

            // Begin by counting the bits unset before the highest set bit
            int mask = 0x80;
            for (int i = 0; i < 8; i++)
            {
                // Start at left, shift to right
                if ((first & mask) == mask)
                {
                    length = i + 1;
                    break;
                }

                mask >>= 1;
            }

            if (length == 0)
            {
                return 0;
            }

            // Read remaining big endian bytes and convert to 64-bit unsigned integer.
            ulong result = (ulong)(unsetFirstBit ? first & (0xFF >> length) : first);
            result <<= --length * 8;
            for (int i = 1; i <= length; i++)
            {
                result |= (ulong)this._stream.ReadByte() << (length - i) * 8;
            }

            return result;
        }

        /// <summary>
        /// Reads a fixed length unsigned integer from the current stream and advances the current
        ///     position of the stream by the integer length in bytes.
        /// </summary>
        /// <param name="length">
        /// The length in bytes of the integer.
        /// </param>
        /// <returns>
        /// A 64-bit unsigned integer.
        /// </returns>
        private ulong ReadUInt(int length)
        {
            byte[] data = new byte[length];
            this._stream.Read(data, 0, length);

            // Convert the big endian byte array to a 64-bit unsigned integer.
            ulong result = 0UL;
            int shift = 0;
            for (int i = length - 1; i >= 0; i--)
            {
                result |= (ulong)data[i] << shift;
                shift += 8;
            }

            return result;
        }

        /// <summary>
        ///     Reads a 2-byte signed integer from the current stream and advances the current position
        ///     of the stream by two bytes.
        /// </summary>
        /// <returns>A 2-byte signed integer read from the current stream.</returns>
        private short ReadInt16()
        {
            byte[] data = new byte[2];
            this._stream.Read(data, 0, 2);
            return (short)(data[0] << 8 | data[1]);
        }

        /// <summary>
        ///     Reads a 4-byte floating point value from the current stream and advances the current
        ///     position of the stream by four bytes.
        /// </summary>
        /// <returns>A 4-byte floating point value read from the current stream.</returns>
        private unsafe float ReadFloat32()
        {
            byte[] data = new byte[4];
            this._stream.Read(data, 0, 4);

            int result = data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3];
            return *(float*)&result;
        }

        /// <summary>
        ///     Reads a 8-byte floating point value from the current stream and advances the current
        ///     position of the stream by eight bytes.
        /// </summary>
        /// <returns>A 8-byte floating point value read from the current stream.</returns>
        private unsafe double ReadFloat64()
        {
            byte[] data = new byte[8];
            this._stream.Read(data, 0, 8);

            int lo = data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3];
            int hi = data[4] << 24 | data[5] << 16 | data[6] << 8 | data[7];
            long result = (uint)hi | (long)lo << 32;
            return *(double*)&result;
        }

        /// <summary>
        /// Reads a fixed length string from the current stream using the specified encoding.
        /// </summary>
        /// <param name="length">
        /// The length in bytes of the string.
        /// </param>
        /// <param name="encoding">
        /// The encoding of the string.
        /// </param>
        /// <returns>
        /// The string being read.
        /// </returns>
        private string ReadString(int length, Encoding encoding)
        {
            byte[] buffer = new byte[length];
            this._stream.Read(buffer, 0, length);
            return encoding.GetString(buffer);
        }
    }
}