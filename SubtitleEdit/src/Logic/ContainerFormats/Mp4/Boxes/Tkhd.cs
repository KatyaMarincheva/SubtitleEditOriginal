namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;

    public class Tkhd : Box
    {
        public readonly ulong Duration;

        public readonly uint Height;

        public readonly uint TrackId;

        public readonly uint Width;

        public Tkhd(FileStream fs)
        {
            this.Buffer = new byte[84];
            int bytesRead = fs.Read(this.Buffer, 0, this.Buffer.Length);
            if (bytesRead < this.Buffer.Length)
            {
                return;
            }

            int version = this.Buffer[0];
            int addToIndex64Bit = 0;
            if (version == 1)
            {
                addToIndex64Bit = 8;
            }

            this.TrackId = this.GetUInt(12 + addToIndex64Bit);
            if (version == 1)
            {
                this.Duration = this.GetUInt64(20 + addToIndex64Bit);
                addToIndex64Bit += 4;
            }
            else
            {
                this.Duration = this.GetUInt(20 + addToIndex64Bit);
            }

            this.Width = (uint)this.GetWord(76 + addToIndex64Bit); // skip decimals
            this.Height = (uint)this.GetWord(80 + addToIndex64Bit); // skip decimals

            // System.Windows.Forms.MessageBox.Show("Width: " + GetWord(76 + addToIndex64Bit).ToString() + "." + GetWord(78 + addToIndex64Bit).ToString());
            // System.Windows.Forms.MessageBox.Show("Height: " + GetWord(80 + addToIndex64Bit).ToString() + "." + GetWord(82 + addToIndex64Bit).ToString());
        }
    }
}