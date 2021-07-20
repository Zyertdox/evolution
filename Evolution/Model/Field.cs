namespace Evolution.Model
{
    public class Field
    {
        private readonly FieldData _fieldData;
        public int Width => _fieldData.Width;
        public int Height => _fieldData.Height;
        public int Length => Width * Height;

        public IPoint this[int x, int y]
        {
            get
            {
                if (x >= Width || x < 0 || y < 0 || y >= Height)
                {
                    return Wall.Default;
                }
                return _fieldData.Points[x + y * Width];
            }
            set => _fieldData.Points[x + y * Width] = value;
        }

        public IPoint this[int index]
        {
            get
            {
                if (index < 0 || index >= Width * Height)
                {
                    return null;
                }
                return _fieldData.Points[index];
            }
        }

        public Field(int width, int height)
        {
            _fieldData = new FieldData(width, height, null);
        }
    }
}
