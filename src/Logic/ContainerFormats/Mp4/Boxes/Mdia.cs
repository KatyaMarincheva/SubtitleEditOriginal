// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Mdia.cs" company="">
//   
// </copyright>
// <summary>
//   The mdia.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    /// <summary>
    /// The mdia.
    /// </summary>
    public class Mdia : Box
    {
        /// <summary>
        /// The handler name.
        /// </summary>
        public readonly string HandlerName = string.Empty;

        /// <summary>
        /// The handler type.
        /// </summary>
        public readonly string HandlerType;

        /// <summary>
        /// The mdhd.
        /// </summary>
        public Mdhd Mdhd;

        /// <summary>
        /// The minf.
        /// </summary>
        public Minf Minf;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mdia"/> class.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <param name="maximumLength">
        /// The maximum length.
        /// </param>
        public Mdia(FileStream fs, ulong maximumLength)
        {
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "minf" && this.IsTextSubtitle || this.IsVobSubSubtitle || this.IsClosedCaption || this.IsVideo)
                {
                    uint timeScale = 90000;
                    if (this.Mdhd != null)
                    {
                        timeScale = this.Mdhd.TimeScale;
                    }

                    this.Minf = new Minf(fs, this.Position, timeScale, this.HandlerType, this);
                }
                else if (this.Name == "hdlr")
                {
                    this.Buffer = new byte[this.Size - 4];
                    fs.Read(this.Buffer, 0, this.Buffer.Length);
                    this.HandlerType = this.GetString(8, 4);
                    if (this.Size > 25)
                    {
                        this.HandlerName = this.GetString(24, this.Buffer.Length - (24 + 5)); // TODO: How to find this?
                    }
                }
                else if (this.Name == "mdhd")
                {
                    this.Mdhd = new Mdhd(fs, this.Size);
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is text subtitle.
        /// </summary>
        public bool IsTextSubtitle
        {
            get
            {
                return this.HandlerType == "sbtl" || this.HandlerType == "text";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is vob sub subtitle.
        /// </summary>
        public bool IsVobSubSubtitle
        {
            get
            {
                return this.HandlerType == "subp";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is closed caption.
        /// </summary>
        public bool IsClosedCaption
        {
            get
            {
                return this.HandlerType == "clcp";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is video.
        /// </summary>
        public bool IsVideo
        {
            get
            {
                return this.HandlerType == "vide";
            }
        }

        /// <summary>
        /// Gets a value indicating whether is audio.
        /// </summary>
        public bool IsAudio
        {
            get
            {
                return this.HandlerType == "soun";
            }
        }
    }
}