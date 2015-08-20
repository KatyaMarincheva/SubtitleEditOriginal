// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbuPesDataField.cs" company="">
//   
// </copyright>
// <summary>
//   The ebu pes data field text.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    /// <summary>
    /// The ebu pes data field text.
    /// </summary>
    public class EbuPesDataFieldText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EbuPesDataFieldText"/> class.
        /// </summary>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public EbuPesDataFieldText(byte[] buffer, int index)
        {
            this.FieldParity = (buffer[index] & Helper.B00100000) > 0;
            this.LineOffset = buffer[index] & Helper.B00011111;
            this.FramingCode = buffer[index + 1];
            this.MagazineAndPacketAddress = Helper.GetEndianWord(buffer, index + 2);
        }

        /// <summary>
        /// Gets or sets a value indicating whether field parity.
        /// </summary>
        public bool FieldParity { get; set; }

        /// <summary>
        /// Gets or sets the line offset.
        /// </summary>
        public int LineOffset { get; set; }

        /// <summary>
        /// Gets or sets the framing code.
        /// </summary>
        public int FramingCode { get; set; }

        /// <summary>
        /// Gets or sets the magazine and packet address.
        /// </summary>
        public int MagazineAndPacketAddress { get; set; }

        /// <summary>
        /// Gets or sets the data block.
        /// </summary>
        public byte[] DataBlock { get; set; }
    }
}