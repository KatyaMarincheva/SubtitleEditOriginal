// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mp4Parser.cs" company="">
//   
// </copyright>
// <summary>
//   http://wiki.multimedia.cx/index.php?title=QuickTime_container
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;

    using Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes;

    /// <summary>
    ///     http://wiki.multimedia.cx/index.php?title=QuickTime_container
    /// </summary>
    public class MP4Parser : Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MP4Parser"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public MP4Parser(string fileName)
        {
            this.FileName = fileName;
            using (FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                this.ParseMp4(fs);
                fs.Close();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MP4Parser"/> class.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        public MP4Parser(FileStream fs)
        {
            this.FileName = null;
            this.ParseMp4(fs);
        }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the moov.
        /// </summary>
        public Moov Moov { get; private set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                if (this.Moov != null && this.Moov.Mvhd != null && this.Moov.Mvhd.TimeScale > 0)
                {
                    return TimeSpan.FromSeconds((double)this.Moov.Mvhd.Duration / this.Moov.Mvhd.TimeScale);
                }

                return new TimeSpan();
            }
        }

        /// <summary>
        /// Gets the creation date.
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                if (this.Moov != null && this.Moov.Mvhd != null && this.Moov.Mvhd.TimeScale > 0)
                {
                    return new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc).Add(TimeSpan.FromSeconds(this.Moov.Mvhd.CreationTime));
                }

                return DateTime.Now;
            }
        }

        /// <summary>
        ///     Resolution of first video track. If not present returns 0.0
        /// </summary>
        public Point VideoResolution
        {
            get
            {
                if (this.Moov != null && this.Moov.Tracks != null)
                {
                    foreach (Trak trak in this.Moov.Tracks)
                    {
                        if (trak != null && trak.Mdia != null && trak.Tkhd != null)
                        {
                            if (trak.Mdia.IsVideo)
                            {
                                return new Point((int)trak.Tkhd.Width, (int)trak.Tkhd.Height);
                            }
                        }
                    }
                }

                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Gets the frame rate.
        /// </summary>
        internal double FrameRate
        {
            get
            {
                // Formula: moov.mdia.stbl.stsz.samplecount / (moov.trak.tkhd.duration / moov.mvhd.timescale) - http://www.w3.org/2008/WebVideo/Annotations/drafts/ontology10/CR/test.php?table=containerMPEG4
                if (this.Moov != null && this.Moov.Mvhd != null && this.Moov.Mvhd.TimeScale > 0)
                {
                    List<Trak> videoTracks = this.GetVideoTracks();
                    if (videoTracks.Count > 0 && videoTracks[0].Tkhd != null && videoTracks[0].Mdia != null && videoTracks[0].Mdia.Minf != null && videoTracks[0].Mdia.Minf.Stbl != null)
                    {
                        double duration = videoTracks[0].Tkhd.Duration;
                        double sampleCount = videoTracks[0].Mdia.Minf.Stbl.StszSampleCount;
                        return sampleCount / (duration / this.Moov.Mvhd.TimeScale);
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// The get subtitle tracks.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Trak> GetSubtitleTracks()
        {
            List<Trak> list = new List<Trak>();
            if (this.Moov != null && this.Moov.Tracks != null)
            {
                foreach (Trak trak in this.Moov.Tracks)
                {
                    if (trak.Mdia != null && (trak.Mdia.IsTextSubtitle || trak.Mdia.IsVobSubSubtitle || trak.Mdia.IsClosedCaption) && trak.Mdia.Minf != null && trak.Mdia.Minf.Stbl != null)
                    {
                        list.Add(trak);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// The get audio tracks.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Trak> GetAudioTracks()
        {
            List<Trak> list = new List<Trak>();
            if (this.Moov != null && this.Moov.Tracks != null)
            {
                foreach (Trak trak in this.Moov.Tracks)
                {
                    if (trak.Mdia != null && trak.Mdia.IsAudio)
                    {
                        list.Add(trak);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// The get video tracks.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Trak> GetVideoTracks()
        {
            List<Trak> list = new List<Trak>();
            if (this.Moov != null && this.Moov.Tracks != null)
            {
                foreach (Trak trak in this.Moov.Tracks)
                {
                    if (trak.Mdia != null && trak.Mdia.IsVideo)
                    {
                        list.Add(trak);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// The parse mp 4.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        private void ParseMp4(FileStream fs)
        {
            int count = 0;
            this.Position = 0;
            fs.Seek(0, SeekOrigin.Begin);
            bool moreBytes = true;
            while (moreBytes)
            {
                moreBytes = this.InitializeSizeAndName(fs);
                if (this.Size < 8)
                {
                    return;
                }

                if (this.Name == "moov" && this.Moov == null)
                {
                    this.Moov = new Moov(fs, this.Position); // only scan first "moov" element
                }

                count++;
                if (count > 100)
                {
                    break;
                }

                if (this.Position > (ulong)fs.Length)
                {
                    break;
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }

            fs.Close();
        }
    }
}