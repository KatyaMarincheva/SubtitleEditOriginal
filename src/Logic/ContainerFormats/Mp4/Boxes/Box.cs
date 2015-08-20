// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Box.cs" company="">
//   
// </copyright>
// <summary>
//   The box.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Mp4.Boxes
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// The box.
    /// </summary>
    public class Box
    {
        /// <summary>
        /// The buffer.
        /// </summary>
        internal byte[] Buffer;

        /// <summary>
        /// The name.
        /// </summary>
        internal string Name;

        /// <summary>
        /// The position.
        /// </summary>
        internal ulong Position;

        /// <summary>
        /// The size.
        /// </summary>
        internal ulong Size;

        /// <summary>
        /// The get u int.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public static uint GetUInt(byte[] buffer, int index)
        {
            return (uint)((buffer[index] << 24) + (buffer[index + 1] << 16) + (buffer[index + 2] << 8) + buffer[index + 3]);
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetString(byte[] buffer, int index, int count)
        {
            if (count <= 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(buffer, index, count);
        }

        /// <summary>
        /// The get u int 64.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public static ulong GetUInt64(byte[] buffer, int index)
        {
            return (ulong)buffer[index] << 56 | (ulong)buffer[index + 1] << 48 | (ulong)buffer[index + 2] << 40 | (ulong)buffer[index + 3] << 32 | (ulong)buffer[index + 4] << 24 | (ulong)buffer[index + 5] << 16 | (ulong)buffer[index + 6] << 8 | buffer[index + 7];
        }

        /// <summary>
        /// The get word.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetWord(byte[] buffer, int index)
        {
            return (buffer[index] << 8) + buffer[index + 1];
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetString(int index, int count)
        {
            return Encoding.UTF8.GetString(this.Buffer, index, count);
        }

        /// <summary>
        /// The get word.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetWord(int index)
        {
            return (this.Buffer[index] << 8) + this.Buffer[index + 1];
        }

        /// <summary>
        /// The get u int.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        public uint GetUInt(int index)
        {
            return (uint)((this.Buffer[index] << 24) + (this.Buffer[index + 1] << 16) + (this.Buffer[index + 2] << 8) + this.Buffer[index + 3]);
        }

        /// <summary>
        /// The get u int 64.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public ulong GetUInt64(int index)
        {
            return (ulong)this.Buffer[index] << 56 | (ulong)this.Buffer[index + 1] << 48 | (ulong)this.Buffer[index + 2] << 40 | (ulong)this.Buffer[index + 3] << 32 | (ulong)this.Buffer[index + 4] << 24 | (ulong)this.Buffer[index + 5] << 16 | (ulong)this.Buffer[index + 6] << 8 | this.Buffer[index + 7];
        }

        /// <summary>
        /// The initialize size and name.
        /// </summary>
        /// <param name="fs">
        /// The fs.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool InitializeSizeAndName(FileStream fs)
        {
            this.Buffer = new byte[8];
            int bytesRead = fs.Read(this.Buffer, 0, this.Buffer.Length);
            if (bytesRead < this.Buffer.Length)
            {
                return false;
            }

            this.Size = this.GetUInt(0);
            this.Name = this.GetString(4, 4);

            if (this.Size == 0)
            {
                this.Size = (ulong)(fs.Length - fs.Position);
            }

            if (this.Size == 1)
            {
                bytesRead = fs.Read(this.Buffer, 0, this.Buffer.Length);
                if (bytesRead < this.Buffer.Length)
                {
                    return false;
                }

                this.Size = this.GetUInt64(0) - 8;
            }

            this.Position = ((ulong)fs.Position) + this.Size - 8;
            return true;
        }
    }
}