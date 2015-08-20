// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VobSubPack.cs" company="">
//   
// </copyright>
// <summary>
//   The vob sub pack.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.VobSub
{
    /// <summary>
    /// The vob sub pack.
    /// </summary>
    public class VobSubPack
    {
        /// <summary>
        /// The _buffer.
        /// </summary>
        private readonly byte[] _buffer;

        /// <summary>
        /// The mpeg 2 header.
        /// </summary>
        public Mpeg2Header Mpeg2Header;

        /// <summary>
        /// The packetized elementary stream.
        /// </summary>
        public PacketizedElementaryStream PacketizedElementaryStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="VobSubPack"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="idxLine">
        /// The idx line.
        /// </param>
        public VobSubPack(byte[] buffer, IdxParagraph idxLine)
        {
            this._buffer = buffer;
            this.IdxLine = idxLine;

            if (VobSubParser.IsMpeg2PackHeader(buffer))
            {
                this.Mpeg2Header = new Mpeg2Header(buffer);
                this.PacketizedElementaryStream = new PacketizedElementaryStream(buffer, Mpeg2Header.Length);
            }
            else if (VobSubParser.IsPrivateStream1(buffer, 0))
            {
                this.PacketizedElementaryStream = new PacketizedElementaryStream(buffer, 0);
            }
        }

        /// <summary>
        /// Gets the idx line.
        /// </summary>
        public IdxParagraph IdxLine { get; private set; }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return this._buffer;
            }
        }
    }
}