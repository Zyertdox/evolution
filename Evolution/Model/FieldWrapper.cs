namespace Evolution.Model
{
    public class FieldWrapper
    {
        public Field Field { get; set; }
        public int Width => Field.Width;
        public int Height => Field.Height;
        public int Length => Width * Height;

        public IPoint this[int x, int y]
        {
            get
            {
                if (x >= Width || x < 0 || y < 0 || y >= Height)
                {
                    return Wall.Default;
                }
                return Field.Points[x + y * Width];
            }
            set => Field.Points[x + y * Width] = value;
        }

        public IPoint this[int index]
        {
            get
            {
                if (index < 0 || index >= Width * Height)
                {
                    return null;
                }
                return Field.Points[index];
            }
        }

        public FieldWrapper(Field field)
        {
            Field = field;
        }
    }
}
