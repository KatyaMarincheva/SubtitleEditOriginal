namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    public class Trak : Box
    {
        public Mdia Mdia;

        public Tkhd Tkhd;

        public Trak(FileStream fs, ulong maximumLength)
        {
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "mdia")
                {
                    this.Mdia = new Mdia(fs, this.Position);
                }
                else if (this.Name == "tkhd")
                {
                    this.Tkhd = new Tkhd(fs);
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }
    }
}