namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Matroska
{
    using System;
    using System.Text;

    internal class MatroskaSubtitle
    {
        public MatroskaSubtitle(byte[] data, long start, long duration)
        {
            this.Data = data;
            this.Start = start;
            this.Duration = duration;
        }

        public MatroskaSubtitle(byte[] data, long start)
            : this(data, start, 0)
        {
        }

        public byte[] Data { get; set; }

        public long Start { get; set; }

        public long Duration { get; set; }

        public long End
        {
            get
            {
                return this.Start + this.Duration;
            }
        }

        public string Text
        {
            get
            {
                if (this.Data != null)
                {
                    return Encoding.UTF8.GetString(this.Data).Replace("\\N", Environment.NewLine);
                }

                return string.Empty;
            }
        }
    }
}