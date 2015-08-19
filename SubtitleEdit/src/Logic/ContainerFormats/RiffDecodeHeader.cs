// (c) Giora Tamir (giora@gtamir.com), 2005

namespace Nikse.SubtitleEdit.Logic.ContainerFormats
{
    using System.Text;

    public class RiffDecodeHeader
    {
        #region Default element processing

        /// <summary>
        ///     Default list element handler - skip the entire list
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="FourCC"></param>
        /// <param name="length"></param>
        private void ProcessList(RiffParser rp, int FourCC, int length)
        {
            rp.SkipData(length);
        }

        #endregion Default element processing

        #region private members

        private double m_frameRate;

        private int m_maxBitRate;

        private int m_numStreams;

        private double m_vidDataRate;

        private double m_audDataRate;

        private string m_audHandler;

        private int m_numChannels;

        private int m_samplesPerSec;

        private int m_bitsPerSec;

        private int m_bitsPerSample;

        #endregion private members

        #region public members

        /// <summary>
        ///     Access the internal parser object
        /// </summary>
        public RiffParser Parser { get; private set; }

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

        public string MaxBitRate
        {
            get
            {
                return string.Format("{0:N} Kb/Sec", this.m_maxBitRate / 128);
            }
        }

        public int TotalFrames { get; private set; }

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

        public string NumStreams
        {
            get
            {
                return string.Format("Streams in file: {0:G}", this.m_numStreams);
            }
        }

        public string FrameSize
        {
            get
            {
                return string.Format("{0:G} x {1:G} pixels per frame", this.Width, this.Height);
            }
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public string VideoDataRate
        {
            get
            {
                return string.Format("Video rate {0:N2} frames/Sec", this.m_vidDataRate);
            }
        }

        public string AudioDataRate
        {
            get
            {
                return string.Format("Audio rate {0:N2} Kb/Sec", this.m_audDataRate / TimeCode.BaseUnit);
            }
        }

        public string VideoHandler { get; private set; }

        public string AudioHandler
        {
            get
            {
                return string.Format("Audio handler 4CC code: {0}", this.m_audHandler);
            }
        }

        public string ISFT { get; private set; }

        public string NumChannels
        {
            get
            {
                return string.Format("Audio channels: {0}", this.m_numChannels);
            }
        }

        public string SamplesPerSec
        {
            get
            {
                return string.Format("Audio rate: {0:N0} Samples/Sec", this.m_samplesPerSec);
            }
        }

        public string BitsPerSec
        {
            get
            {
                return string.Format("Audio rate: {0:N0} Bytes/Sec", this.m_bitsPerSec);
            }
        }

        public string BitsPerSample
        {
            get
            {
                return string.Format("Audio data: {0:N0} bits/Sample", this.m_bitsPerSample);
            }
        }

        #endregion public members

        #region Constructor

        public RiffDecodeHeader(RiffParser rp)
        {
            this.Parser = rp;
        }

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
        ///     Handle chunk elements found in the AVI file. Ignores unknown chunks and
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="FourCC"></param>
        /// <param name="unpaddedLength"></param>
        /// <param name="paddedLength"></param>
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
        ///     Handle List elements found in the AVI file. Ignores unknown lists and recursively looks
        ///     at the content of known lists.
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="FourCC"></param>
        /// <param name="length"></param>
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