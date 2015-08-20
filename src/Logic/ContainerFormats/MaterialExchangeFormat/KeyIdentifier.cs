// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyIdentifier.cs" company="">
//   
// </copyright>
// <summary>
//   The key identifier.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.MaterialExchangeFormat
{
    /// <summary>
    /// The key identifier.
    /// </summary>
    public enum KeyIdentifier
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown, 

        /// <summary>
        /// The partition pack.
        /// </summary>
        PartitionPack, 

        /// <summary>
        /// The preface.
        /// </summary>
        Preface, 

        /// <summary>
        /// The essence element.
        /// </summary>
        EssenceElement, 

        /// <summary>
        /// The operational pattern.
        /// </summary>
        OperationalPattern, 

        /// <summary>
        /// The partition metadata.
        /// </summary>
        PartitionMetadata, 

        /// <summary>
        /// The structural metadata.
        /// </summary>
        StructuralMetadata, 

        /// <summary>
        /// The data definition video.
        /// </summary>
        DataDefinitionVideo, 

        /// <summary>
        /// The data definition audio.
        /// </summary>
        DataDefinitionAudio, 

        /// <summary>
        /// The primer.
        /// </summary>
        Primer
    }
}