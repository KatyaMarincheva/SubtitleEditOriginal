// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramAssociationTable.cs" company="">
//   
// </copyright>
// <summary>
//   The program association table.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    using System.Collections.Generic;

    /// <summary>
    /// The program association table.
    /// </summary>
    public class ProgramAssociationTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramAssociationTable"/> class.
        /// </summary>
        /// <param name="packetBuffer">
        /// The packet buffer.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        public ProgramAssociationTable(byte[] packetBuffer, int index)
        {
            this.TableId = packetBuffer[index];
            this.SectionLength = (packetBuffer[index + 1] & Helper.B00000011) * 256 + packetBuffer[index + 2];
            this.TransportStreamId = packetBuffer[index + 3] * 256 + packetBuffer[index + 4];
            this.VersionNumber = (packetBuffer[index + 5] & Helper.B00111110) >> 1;
            this.CurrentNextIndicator = packetBuffer[index + 5] & 1;
            this.SectionNumber = packetBuffer[index + 6];
            this.LastSectionNumber = packetBuffer[index + 7];

            this.ProgramNumbers = new List<int>();
            this.ProgramIds = new List<int>();
            index = index + 8;
            for (int i = 0; i < (this.SectionLength - 5) / 8; i++)
            {
                if (index + 3 < packetBuffer.Length)
                {
                    int programNumber = packetBuffer[index] * 256 + packetBuffer[index + 1];
                    int programId = (packetBuffer[index + 2] & Helper.B00011111) * 256 + packetBuffer[index + 3];
                    this.ProgramNumbers.Add(programNumber);
                    this.ProgramIds.Add(programId);
                    index += 8;
                }
            }
        }

        /// <summary>
        /// Gets or sets the table id.
        /// </summary>
        public int TableId { get; set; }

        /// <summary>
        /// Gets or sets the section length.
        /// </summary>
        public int SectionLength { get; set; }

        /// <summary>
        /// Gets or sets the transport stream id.
        /// </summary>
        public int TransportStreamId { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the current next indicator.
        /// </summary>
        public int CurrentNextIndicator { get; set; }

        /// <summary>
        /// Gets or sets the section number.
        /// </summary>
        public int SectionNumber { get; set; }

        /// <summary>
        /// Gets or sets the last section number.
        /// </summary>
        public int LastSectionNumber { get; set; }

        /// <summary>
        /// Gets or sets the program numbers.
        /// </summary>
        public List<int> ProgramNumbers { get; set; }

        /// <summary>
        /// Gets or sets the program ids.
        /// </summary>
        public List<int> ProgramIds { get; set; }
    }
}