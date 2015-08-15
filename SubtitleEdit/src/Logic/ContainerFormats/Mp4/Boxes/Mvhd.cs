namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    public class Mvhd : Box
    {
        public readonly uint CreationTime;

        public readonly uint Duration;

        public readonly uint ModificationTime;

        public readonly uint TimeScale;

        public Mvhd(FileStream fs)
        {
            this.Buffer = new byte[20];
            int bytesRead = fs.Read(this.Buffer, 0, this.Buffer.Length);
            if (bytesRead < this.Buffer.Length)
            {
                return;
            }

            this.CreationTime = this.GetUInt(4);
            this.ModificationTime = this.GetUInt(8);
            this.TimeScale = this.GetUInt(12);
            this.Duration = this.GetUInt(16);
        }
    }
}