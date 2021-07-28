namespace Evolution.Model
{
    public class FieldWrapper
    {
        public FieldData FieldData { get; set; }
        public int Width => FieldData.Width;
        public int Height => FieldData.Height;
        public int Length => Width * Height;

        public IPoint this[int x, int y]
        {
            get
            {
                if (x >= Width || x < 0 || y < 0 || y >= Height)
                {
                    return Wall.Default;
                }
                return FieldData.Points[x + y * Width];
            }
            set => FieldData.Points[x + y * Width] = value;
        }

        public IPoint this[int index]
        {
            get
            {
                if (index < 0 || index >= Width * Height)
                {
                    return null;
                }
                return FieldData.Points[index];
            }
        }

        public FieldWrapper(int width, int height)
        {
            FieldData = new FieldData(width, height, null);
        }
    }
}
