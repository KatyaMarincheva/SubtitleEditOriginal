namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.Collections.Generic;
    using System.IO;

    public class Moov : Box
    {
        public Mvhd Mvhd;

        public List<Trak> Tracks;

        public Moov(FileStream fs, ulong maximumLength)
        {
            this.Tracks = new List<Trak>();
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "trak")
                {
                    this.Tracks.Add(new Trak(fs, this.Position));
                }
                else if (this.Name == "mvhd")
                {
                    this.Mvhd = new Mvhd(fs);
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }
    }
}