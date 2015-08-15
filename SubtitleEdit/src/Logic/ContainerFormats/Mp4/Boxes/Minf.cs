namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    public class Minf : Box
    {
        public Stbl Stbl;

        public Minf(FileStream fs, ulong maximumLength, uint timeScale, string handlerType, Mdia mdia)
        {
            this.Position = (ulong)fs.Position;
            while (fs.Position < (long)maximumLength)
            {
                if (!this.InitializeSizeAndName(fs))
                {
                    return;
                }

                if (this.Name == "stbl")
                {
                    this.Stbl = new Stbl(fs, this.Position, timeScale, handlerType, mdia);
                }

                fs.Seek((long)this.Position, SeekOrigin.Begin);
            }
        }
    }
}