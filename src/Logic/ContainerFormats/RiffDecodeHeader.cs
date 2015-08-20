// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="RiffDecodeHeader.cs">
//   
// </copyright>
// <summary>
//   The riff decode header.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace Nikse.SubtitleEdit.Logic.ContainerFormats
{
    using System.Text;

    /// <summary>
    /// The riff decode header.
    /// </summary>
    public class RiffDecodeHeader
    {
        #region Default element processing

        /// <summary>
        /// Default list element handler - skip the entire list
        /// </summary>
        /// <param name="rp">
        /// </param>
        /// <param name="FourCC">
        /// </param>
        /// <param name="length">
        /// </param>
        private void ProcessList(RiffParser rp, int FourCC, int length)
        {
            rp.SkipData(length);
        }

        #endregion Default element processing

        #region private members

        /// <summary>
        /// The m_frame rate.
        /// </summary>
        private double m_frameRate;

        /// <summary>
        /// The m_max bit rate.
        /// </summary>
        private int m_maxBitRate;

        /// <summary>
        /// The m_num streams.
        /// </summary>
        private int m_numStreams;

        /// <summary>
        /// The m_vid data rate.
        /// </summary>
        private double m_vidDataRate;

        /// <summary>
        /// The m_aud data rate.
        /// </summary>
        private double m_audDataRate;

        /// <summary>
        /// The m_aud handler.
        /// </summary>
        private string m_audHandler;

        /// <summary>
        /// The m_num channels.
        /// </summary>
        private int m_numChannels;

        /// <summary>
        /// The m_samples per sec.
        /// </summary>
        private int m_samplesPerSec;

        /// <summary>
        /// The m_bits per sec.
        /// </summary>
        private int m_bitsPerSec;

        /// <summary>
        /// The m_bits per sample.
        /// </summary>
        private int m_bitsPerSample;

        #endregion private members

        #region public members

        /// <summary>
        ///     Access the internal parser object
        /// </summary>
        public RiffParser Parser { get; private set; }

        /// <summary>
        /// Gets the frame rate.
        /// </summary>
        public double FrameRate
        {
            get
            {
                double rate = 0.0;
                if (this.m_frameRate > 0.0)
                {
                    rate = 1000000.0 / this.m_frameRate;
                }

                return rate;
            }
        }

        /// <summary>
        /// Gets the max bit rate.
        /// </summary>
        public string MaxBitRate
        {
            get
            {
                return string.Format("{0:N} Kb/Sec", this.m_maxBitRate / 128);
            }
        }

        /// <summary>
        /// Gets the total frames.
        /// </summary>
        public int TotalFrames { get; private set; }

        /// <summary>
        /// Gets the total milliseconds.
        /// </summary>
        public double TotalMilliseconds
        {
            get
            {
                double totalTime = 0.0;
                if (this.m_frameRate > 0.0)
                {
                    totalTime = this.TotalFrames * this.m_frameRate / TimeCode.BaseUnit;
                }

                return totalTime;
            }
        }

        /// <summary>
        /// Gets the num streams.
        /// </summary>
        public string NumStreams
        {
            get
            {
                return string.Format("Streams in file: {0:G}", this.m_numStreams);
            }
        }

        /// <summary>
        /// Gets the frame size.
        /// </summary>
        public string FrameSize
        {
            get
            {
                return string.Format("{0:G} x {1:G} pixels per frame", this.Width, this.Height);
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the video data rate.
        /// </summary>
        public string VideoDataRate
        {
            get
            {
                return string.Format("Video rate {0:N2} frames/Sec", this.m_vidDataRate);
            }
        }

        /// <summary>
        /// Gets the audio data rate.
        /// </summary>
        public string AudioDataRate
        {
            get
            {
                return string.Format("Audio rate {0:N2} Kb/Sec", this.m_audDataRate / TimeCode.BaseUnit);
            }
        }

        /// <summary>
        /// Gets the video handler.
        /// </summary>
        public string VideoHandler { get; private set; }

        /// <summary>
        /// Gets the audio handler.
        /// </summary>
        public string AudioHandler
        {
            get
            {
                return string.Format("Audio handler 4CC code: {0}", this.m_audHandler);
            }
        }

        /// <summary>
        /// Gets the isft.
        /// </summary>
        public string ISFT { get; private set; }

        /// <summary>
        /// Gets the num channels.
        /// </summary>
        public string NumChannels
        {
            get
            {
                return string.Format("Audio channels: {0}", this.m_numChannels);
            }
        }

        /// <summary>
        /// Gets the samples per sec.
        /// </summary>
        public string SamplesPerSec
        {
            get
            {
                return string.Format("Audio rate: {0:N0} Samples/Sec", this.m_samplesPerSec);
            }
        }

        /// <summary>
        /// Gets the bits per sec.
        /// </summary>
        public string BitsPerSec
        {
            get
            {
                return string.Format("Audio rate: {0:N0} Bytes/Sec", this.m_bitsPerSec);
            }
        }

        /// <summary>
        /// Gets the bits per sample.
        /// </summary>
        public string BitsPerSample
        {
            get
            {
                return string.Format("Audio data: {0:N0} bits/Sample", this.m_bitsPerSample);
            }
        }

        #endregion public members

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffDecodeHeader"/> class.
        /// </summary>
        /// <param name="rp">
        /// The rp.
        /// </param>
        public RiffDecodeHeader(RiffParser rp)
        {
            this.Parser = rp;
        }

        /// <summary>
        /// The clear.
        /// </summary>
        private void Clear()
        {
            this.m_frameRate = 0;
            this.Height = 0;
            this.m_maxBitRate = 0;
            this.m_numStreams = 0;
            this.TotalFrames = 0;
            this.Width = 0;

            this.ISFT = string.Empty;

            this.m_vidDataRate = 0;
            this.m_audDataRate = 0;
            this.VideoHandler = string.Empty;
            this.m_audHandler = string.Empty;

            this.m_numChannels = 0;
            this.m_samplesPerSec = 0;
            this.m_bitsPerSample = 0;
            this.m_bitsPerSec = 0;
        }

        #endregion Constructor

        #region Decode AVI

        /// <summary>
        /// Handle chunk elements found in the AVI file. Ignores unknown chunks and
        /// </summary>
        /// <param name="rp">
        /// </param>
        /// <param name="FourCC">
        /// </param>
        /// <param name="unpaddedLength">
        /// </param>
        /// <param name="paddedLength">
        /// </param>
        private void ProcessAVIChunk(RiffParser rp, int FourCC, int unpaddedLength, int paddedLength)
        {
            if (AviRiffData.ckidMainAVIHeader == FourCC)
            {
                // Main AVI header
                this.DecodeAVIHeader(rp, paddedLength);
            }
            else if (AviRiffData.ckidAVIStreamHeader == FourCC)
            {
                // Stream header
                this.DecodeAVIStream(rp, paddedLength);
            }
            else if (AviRiffData.ckidAVIISFT == FourCC)
            {
                byte[] ba = new byte[paddedLength];
                rp.ReadData(ba, 0, paddedLength);
                StringBuilder sb = new StringBuilder(unpaddedLength);
                for (int i = 0; i < unpaddedLength; ++i)
                {
                    if (0 != ba[i])
                    {
                        sb.Append((char)ba[i]);
                    }
                }

                this.ISFT = sb.ToString();
            }
            else
            {
                // Unknon chunk - skip
                rp.SkipData(paddedLength);
            }
        }

        /// <summary>
        /// Handle List elements found in the AVI file. Ignores unknown lists and recursively looks
        ///     at the content of known lists.
        /// </summary>
        /// <param name="rp">
        /// </param>
        /// <param name="FourCC">
        /// </param>
        /// <param name="length">
        /// </param>
        private void ProcessAVIList(RiffParser rp, int FourCC, int length)
        {
            RiffParser.ProcessChunkElement pac = this.ProcessAVIChunk;
            RiffParser.ProcessListElement pal = this.ProcessAVIList;

            // Is this the header?
            if ((AviRiffData.ckidAVIHeaderList == FourCC) || (AviRiffData.ckidAVIStreamList == FourCC) || (AviRiffData.ckidINFOList == FourCC))
            {
                while (length > 0)
                {
                    if (false == rp.ReadElement(ref length, pac, pal))
                    {
                        break;
                    }
                }
            }
            else
            {
                // Unknown lists - ignore
                rp.SkipData(length);
            }
        }

        /// <summary>
        /// The process main avi.
        /// </summary>
        public void ProcessMainAVI()
        {
            this.Clear();
            int length = this.Parser.DataSize;

            RiffParser.ProcessChunkElement pdc = this.ProcessAVIChunk;
            RiffParser.ProcessListElement pal = this.ProcessAVIList;

            while (length > 0)
            {
                if (false == this.Parser.ReadElement(ref length, pdc, pal))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// The decode avi header.
        /// </summary>
        /// <param name="rp">
        /// The rp.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <exception cref="RiffParserException">
        /// </exception>
        private unsafe void DecodeAVIHeader(RiffParser rp, int length)
        {
            // if (length < sizeof(AVIMAINHEADER))
            // {
            // throw new RiffParserException(String.Format("Header size mismatch. Needed {0} but only have {1}",
            // sizeof(AVIMAINHEADER), length));
            // }
            byte[] ba = new byte[length];

            if (rp.ReadData(ba, 0, length) != length)
            {
                throw new RiffParserException("Problem reading AVI header.");
            }

            fixed (byte* bp = &ba[0])
            {
                AVIMAINHEADER* avi = (AVIMAINHEADER*)bp;
                this.m_frameRate = avi->dwMicroSecPerFrame;
                this.Height = avi->dwHeight;
                this.m_maxBitRate = avi->dwMaxBytesPerSec;
                this.m_numStreams = avi->dwStreams;
                this.TotalFrames = avi->dwTotalFrames;
                this.Width = avi->dwWidth;
            }
        }

        /// <summary>
        /// The decode avi stream.
        /// </summary>
        /// <param name="rp">
        /// The rp.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <exception cref="RiffParserException">
        /// </exception>
        private unsafe void DecodeAVIStream(RiffParser rp, int length)
        {
            byte[] ba = new byte[length];

            if (rp.ReadData(ba, 0, length) != length)
            {
                throw new RiffParserException("Problem reading AVI header.");
            }

            fixed (byte* bp = &ba[0])
            {
                AVISTREAMHEADER* avi = (AVISTREAMHEADER*)bp;

                if (AviRiffData.streamtypeVIDEO == avi->fccType)
                {
                    this.VideoHandler = RiffParser.FromFourCC(avi->fccHandler);
                    if (avi->dwScale > 0)
                    {
                        this.m_vidDataRate = avi->dwRate / (double)avi->dwScale;
                    }
                    else
                    {
                        this.m_vidDataRate = 0.0;
                    }
                }
                else if (AviRiffData.streamtypeAUDIO == avi->fccType)
                {
                    if (AviRiffData.ckidMP3 == avi->fccHandler)
                    {
                        this.m_audHandler = "MP3";
                    }
                    else
                    {
                        this.m_audHandler = RiffParser.FromFourCC(avi->fccHandler);
                    }

                    if (avi->dwScale > 0)
                    {
                        this.m_audDataRate = 8.0 * avi->dwRate / avi->dwScale;
                        if (avi->dwSampleSize > 0)
                        {
                            this.m_audDataRate /= avi->dwSampleSize;
                        }
                    }
                    else
                    {
                        this.m_audDataRate = 0.0;
                    }
                }
            }
        }

        #endregion Decode AVI

        #region WAVE processing

        /// <summary>
        /// The process wave chunk.
        /// </summary>
        /// <param name="rp">
        /// The rp.
        /// </param>
        /// <param name="FourCC">
        /// The four cc.
        /// </param>
        /// <param name="unpaddedLength">
        /// The unpadded length.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        private void ProcessWaveChunk(RiffParser rp, int FourCC, int unpaddedLength, int length)
        {
            // Is this a 'fmt' chunk?
            if (AviRiffData.ckidWaveFMT == FourCC)
            {
                this.DecodeWave(rp, length);
            }
            else
            {
                rp.SkipData(length);
            }
        }

        /// <summary>
        /// The decode wave.
        /// </summary>
        /// <param name="rp">
        /// The rp.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        private unsafe void DecodeWave(RiffParser rp, int length)
        {
            byte[] ba = new byte[length];
            rp.ReadData(ba, 0, length);

            fixed (byte* bp = &ba[0])
            {
                WAVEFORMATEX* wave = (WAVEFORMATEX*)bp;
                this.m_numChannels = wave->nChannels;
                this.m_bitsPerSec = wave->nAvgBytesPerSec;
                this.m_bitsPerSample = wave->wBitsPerSample;
                this.m_samplesPerSec = wave->nSamplesPerSec;
            }
        }

        /// <summary>
        /// The process main wave.
        /// </summary>
        public void ProcessMainWAVE()
        {
            this.Clear();
            int length = this.Parser.DataSize;

            RiffParser.ProcessChunkElement pdc = this.ProcessWaveChunk;
            RiffParser.ProcessListElement pal = this.ProcessList;

            while (length > 0)
            {
                if (false == this.Parser.ReadElement(ref length, pdc, pal))
                {
                    break;
                }
            }
        }

        #endregion WAVE processing
    }
}