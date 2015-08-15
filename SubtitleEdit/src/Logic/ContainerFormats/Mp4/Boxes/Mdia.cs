namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    public class Mdia : Box
    {
        public readonly string HandlerName = string.Empty;

        public readonly string HandlerType;

        public Mdhd Mdhd;

        public Minf Minf;

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

        public bool IsTextSubtitle
        {
            get
            {
                return this.HandlerType == "sbtl" || this.HandlerType == "text";
            }
        }

        public bool IsVobSubSubtitle
        {
            get
            {
                return this.HandlerType == "subp";
            }
        }

        public bool IsClosedCaption
        {
            get
            {
                return this.HandlerType == "clcp";
            }
        }

        public bool IsVideo
        {
            get
            {
                return this.HandlerType == "vide";
            }
        }

        public bool IsAudio
        {
            get
            {
                return this.HandlerType == "soun";
            }
        }
    }
}