namespace Nikse.SubtitleEdit.Logic.ContainerFormats.Ebml
{
    internal class Element
    {
        private readonly long _dataPosition;

        private readonly long _dataSize;

        private readonly ElementId _id;

        public Element(ElementId id, long dataPosition, long dataSize)
        {
            this._id = id;
            this._dataPosition = dataPosition;
            this._dataSize = dataSize;
        }

        public ElementId Id
        {
            get
            {
                return this._id;
            }
        }

        public long DataPosition
        {
            get
            {
                return this._dataPosition;
            }
        }

        public long DataSize
        {
            get
            {
                return this._dataSize;
            }
        }

        public long EndPosition
        {
            get
            {
                return this._dataPosition + this._dataSize;
            }
        }

        public override string ToString()
        {
            return string.Format(@"{0} ({1})", this._id, this._dataSize);
        }
    }
}