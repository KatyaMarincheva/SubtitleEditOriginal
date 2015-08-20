// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AviRiffData.cs">
//   
// </copyright>
// <summary>
//   The avimainheader.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.ContainerFormats
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The avimainheader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AVIMAINHEADER
    { // 'avih'

        /// <summary>
        /// The dw micro sec per frame.
        /// </summary>
        public int dwMicroSecPerFrame;

        /// <summary>
        /// The dw max bytes per sec.
        /// </summary>
        public int dwMaxBytesPerSec;

        /// <summary>
        /// The dw padding granularity.
        /// </summary>
        public int dwPaddingGranularity;

        /// <summary>
        /// The dw flags.
        /// </summary>
        public int dwFlags;

        /// <summary>
        /// The dw total frames.
        /// </summary>
        public int dwTotalFrames;

        /// <summary>
        /// The dw initial frames.
        /// </summary>
        public int dwInitialFrames;

        /// <summary>
        /// The dw streams.
        /// </summary>
        public int dwStreams;

        /// <summary>
        /// The dw suggested buffer size.
        /// </summary>
        public int dwSuggestedBufferSize;

        /// <summary>
        /// The dw width.
        /// </summary>
        public int dwWidth;

        /// <summary>
        /// The dw height.
        /// </summary>
        public int dwHeight;

        /// <summary>
        /// The dw reserved 0.
        /// </summary>
        public int dwReserved0;

        /// <summary>
        /// The dw reserved 1.
        /// </summary>
        public int dwReserved1;

        /// <summary>
        /// The dw reserved 2.
        /// </summary>
        public int dwReserved2;

        /// <summary>
        /// The dw reserved 3.
        /// </summary>
        public int dwReserved3;
    }

    /// <summary>
    /// The aviextheader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AVIEXTHEADER
    { // 'dmlh'

        /// <summary>
        /// The dw grand frames.
        /// </summary>
        public int dwGrandFrames; // total number of frames in the file

        /// <summary>
        /// The dw future.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 244)]
        public int[] dwFuture; // to be defined later
    }

    /// <summary>
    /// The rect.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RECT
    {
        /// <summary>
        /// The left.
        /// </summary>
        public short left;

        /// <summary>
        /// The top.
        /// </summary>
        public short top;

        /// <summary>
        /// The right.
        /// </summary>
        public short right;

        /// <summary>
        /// The bottom.
        /// </summary>
        public short bottom;
    }

    /// <summary>
    /// The avistreamheader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AVISTREAMHEADER
    { // 'strh'

        /// <summary>
        /// The fcc type.
        /// </summary>
        public int fccType; // stream type codes

        /// <summary>
        /// The fcc handler.
        /// </summary>
        public int fccHandler;

        /// <summary>
        /// The dw flags.
        /// </summary>
        public int dwFlags;

        /// <summary>
        /// The w priority.
        /// </summary>
        public short wPriority;

        /// <summary>
        /// The w language.
        /// </summary>
        public short wLanguage;

        /// <summary>
        /// The dw initial frames.
        /// </summary>
        public int dwInitialFrames;

        /// <summary>
        /// The dw scale.
        /// </summary>
        public int dwScale;

        /// <summary>
        /// The dw rate.
        /// </summary>
        public int dwRate; // dwRate/dwScale is stream tick rate in ticks/s

        /// <summary>
        /// The dw start.
        /// </summary>
        public int dwStart;

        /// <summary>
        /// The dw length.
        /// </summary>
        public int dwLength;

        /// <summary>
        /// The dw suggested buffer size.
        /// </summary>
        public int dwSuggestedBufferSize;

        /// <summary>
        /// The dw quality.
        /// </summary>
        public int dwQuality;

        /// <summary>
        /// The dw sample size.
        /// </summary>
        public int dwSampleSize;

        /// <summary>
        /// The rc frame.
        /// </summary>
        public RECT rcFrame;
    }

    /// <summary>
    /// The avioldindexentry.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AVIOLDINDEXENTRY
    {
        /// <summary>
        /// The dw chunk id.
        /// </summary>
        public int dwChunkId;

        /// <summary>
        /// The dw flags.
        /// </summary>
        public int dwFlags;

        /// <summary>
        /// The dw offset.
        /// </summary>
        public int dwOffset; // offset of riff chunk header for the data

        /// <summary>
        /// The dw size.
        /// </summary>
        public int dwSize; // size of the data (excluding riff header size)
    }

    /// <summary>
    /// The timecode.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TIMECODE
    {
        /// <summary>
        /// The w frame rate.
        /// </summary>
        public short wFrameRate;

        /// <summary>
        /// The w frame fract.
        /// </summary>
        public short wFrameFract;

        /// <summary>
        /// The c frames.
        /// </summary>
        public int cFrames;
    }

    /// <summary>
    /// The timecodedata.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TIMECODEDATA
    {
        /// <summary>
        /// The time.
        /// </summary>
        private readonly TIMECODE time;

        /// <summary>
        /// The dw smpt eflags.
        /// </summary>
        public int dwSMPTEflags;

        /// <summary>
        /// The dw user.
        /// </summary>
        public int dwUser;
    }

    /// <summary>
    /// The waveformatex.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct WAVEFORMATEX
    {
        /// <summary>
        /// The w format tag.
        /// </summary>
        public short wFormatTag;

        /// <summary>
        /// The n channels.
        /// </summary>
        public short nChannels;

        /// <summary>
        /// The n samples per sec.
        /// </summary>
        public int nSamplesPerSec;

        /// <summary>
        /// The n avg bytes per sec.
        /// </summary>
        public int nAvgBytesPerSec;

        /// <summary>
        /// The n block align.
        /// </summary>
        public short nBlockAlign;

        /// <summary>
        /// The w bits per sample.
        /// </summary>
        public short wBitsPerSample;

        /// <summary>
        /// The cb size.
        /// </summary>
        public short cbSize;
    }

    /// <summary>
    /// The avi riff data.
    /// </summary>
    internal static class AviRiffData
    {
        #region AVI constants

        // AVIMAINHEADER flags
        /// <summary>
        /// The avi f_ hasindex.
        /// </summary>
        public const int AVIF_HASINDEX = 0x00000010; // Index at end of file?

        /// <summary>
        /// The avi f_ mustuseindex.
        /// </summary>
        public const int AVIF_MUSTUSEINDEX = 0x00000020;

        /// <summary>
        /// The avi f_ isinterleaved.
        /// </summary>
        public const int AVIF_ISINTERLEAVED = 0x00000100;

        /// <summary>
        /// The avi f_ trustcktype.
        /// </summary>
        public const int AVIF_TRUSTCKTYPE = 0x00000800; // Use CKType to find key frames

        /// <summary>
        /// The avi f_ wascapturefile.
        /// </summary>
        public const int AVIF_WASCAPTUREFILE = 0x00010000;

        /// <summary>
        /// The avi f_ copyrighted.
        /// </summary>
        public const int AVIF_COPYRIGHTED = 0x00020000;

        // AVISTREAMINFO flags
        /// <summary>
        /// The avis f_ disabled.
        /// </summary>
        public const int AVISF_DISABLED = 0x00000001;

        /// <summary>
        /// The avis f_ vide o_ palchanges.
        /// </summary>
        public const int AVISF_VIDEO_PALCHANGES = 0x00010000;

        // AVIOLDINDEXENTRY flags
        /// <summary>
        /// The avii f_ list.
        /// </summary>
        public const int AVIIF_LIST = 0x00000001;

        /// <summary>
        /// The avii f_ keyframe.
        /// </summary>
        public const int AVIIF_KEYFRAME = 0x00000010;

        /// <summary>
        /// The avii f_ n o_ time.
        /// </summary>
        public const int AVIIF_NO_TIME = 0x00000100;

        /// <summary>
        /// The avii f_ compressor.
        /// </summary>
        public const int AVIIF_COMPRESSOR = 0x0FFF0000; // unused?

        // TIMECODEDATA flags
        /// <summary>
        /// The timecod e_ smpt e_ binar y_ group.
        /// </summary>
        public const int TIMECODE_SMPTE_BINARY_GROUP = 0x07;

        /// <summary>
        /// The timecod e_ smpt e_ colo r_ frame.
        /// </summary>
        public const int TIMECODE_SMPTE_COLOR_FRAME = 0x08;

        // AVI stream FourCC codes
        /// <summary>
        /// The streamtype video.
        /// </summary>
        public static readonly int streamtypeVIDEO = RiffParser.ToFourCC("vids");

        /// <summary>
        /// The streamtype audio.
        /// </summary>
        public static readonly int streamtypeAUDIO = RiffParser.ToFourCC("auds");

        // public static readonly int streamtypeMIDI = RiffParser.ToFourCC("mids");
        // public static readonly int streamtypeTEXT = RiffParser.ToFourCC("txts");

        // AVI section FourCC codes
        /// <summary>
        /// The ckid avi header list.
        /// </summary>
        public static readonly int ckidAVIHeaderList = RiffParser.ToFourCC("hdrl");

        /// <summary>
        /// The ckid main avi header.
        /// </summary>
        public static readonly int ckidMainAVIHeader = RiffParser.ToFourCC("avih");

        // public static readonly int ckidODML = RiffParser.ToFourCC("odml");
        // public static readonly int ckidAVIExtHeader = RiffParser.ToFourCC("dmlh");
        /// <summary>
        /// The ckid avi stream list.
        /// </summary>
        public static readonly int ckidAVIStreamList = RiffParser.ToFourCC("strl");

        /// <summary>
        /// The ckid avi stream header.
        /// </summary>
        public static readonly int ckidAVIStreamHeader = RiffParser.ToFourCC("strh");

        // public static readonly int ckidStreamFormat = RiffParser.ToFourCC("strf");
        // public static readonly int ckidAVIOldIndex = RiffParser.ToFourCC("idx1");
        /// <summary>
        /// The ckid info list.
        /// </summary>
        public static readonly int ckidINFOList = RiffParser.ToFourCC("INFO");

        /// <summary>
        /// The ckid aviisft.
        /// </summary>
        public static readonly int ckidAVIISFT = RiffParser.ToFourCC("ISFT");

        /// <summary>
        /// The ckid m p 3.
        /// </summary>
        public const int ckidMP3 = 0x0055;

        /// <summary>
        /// The ckid wave fmt.
        /// </summary>
        public static readonly int ckidWaveFMT = RiffParser.ToFourCC("fmt ");

        #endregion AVI constants
    }
}