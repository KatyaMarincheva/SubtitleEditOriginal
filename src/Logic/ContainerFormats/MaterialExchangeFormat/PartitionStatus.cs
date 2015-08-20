// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartitionStatus.cs" company="">
//   
// </copyright>
// <summary>
//   The partition status.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.MaterialExchangeFormat
{
    /// <summary>
    /// The partition status.
    /// </summary>
    public enum PartitionStatus
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown = 0, 

        /// <summary>
        /// The open and incomplete.
        /// </summary>
        OpenAndIncomplete = 1, 

        /// <summary>
        /// The closed and incomplete.
        /// </summary>
        ClosedAndIncomplete = 2, 

        /// <summary>
        /// The open and complete.
        /// </summary>
        OpenAndComplete = 3, 

        /// <summary>
        /// The closed and complete.
        /// </summary>
        ClosedAndComplete = 4
    }
}