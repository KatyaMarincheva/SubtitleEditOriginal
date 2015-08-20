// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element.cs" company="">
//   
// </copyright>
// <summary>
//   The element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Ebml
{
    /// <summary>
    /// The element.
    /// </summary>
    internal class Element
    {
        /// <summary>
        /// The _data position.
        /// </summary>
        private readonly long _dataPosition;

        /// <summary>
        /// The _data size.
        /// </summary>
        private readonly long _dataSize;

        /// <summary>
        /// The _id.
        /// </summary>
        private readonly ElementId _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Element"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="dataPosition">
        /// The data position.
        /// </param>
        /// <param name="dataSize">
        /// The data size.
        /// </param>
        public Element(ElementId id, long dataPosition, long dataSize)
        {
            this._id = id;
            this._dataPosition = dataPosition;
            this._dataSize = dataSize;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public ElementId Id
        {
            get
            {
                return this._id;
            }
        }

        /// <summary>
        /// Gets the data position.
        /// </summary>
        public long DataPosition
        {
            get
            {
                return this._dataPosition;
            }
        }

        /// <summary>
        /// Gets the data size.
        /// </summary>
        public long DataSize
        {
            get
            {
                return this._dataSize;
            }
        }

        /// <summary>
        /// Gets the end position.
        /// </summary>
        public long EndPosition
        {
            get
            {
                return this._dataPosition + this._dataSize;
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(@"{0} ({1})", this._id, this._dataSize);
        }
    }
}