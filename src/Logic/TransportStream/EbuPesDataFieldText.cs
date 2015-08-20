// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EbuPesDataFieldText.cs" company="">
//   
// </copyright>
// <summary>
//   The ebu pes data field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.TransportStream
{
    /// <summary>
    /// The ebu pes data field.
    /// </summary>
    public class EbuPesDataField
    {
        /// <summary>
        /// The field text.
        /// </summary>
        public EbuPesDataFieldText FieldText;

        /// <summary>
        /// Gets or sets the data unit id.
        /// </summary>
        public int DataUnitId { get; set; }

        /// <summary>
        /// Gets or sets the data unit length.
        /// </summary>
        public int DataUnitLength { get; set; }

        /// <summary>
        /// Gets or sets the data field.
        /// </summary>
        public byte[] DataField { get; set; }
    }
}