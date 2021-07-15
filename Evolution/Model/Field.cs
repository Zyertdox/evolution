using System.Collections.Generic;

namespace Evolution.Model
{
    public class Field
    {
        public int Width { get; }
        public int Height { get; }
        public int Length => Width * Height;

        private readonly IList<IPoint> _points;

        public IPoint this[int x, int y]
        {
            get
            {
                if (x >= Width || x < 0 || y < 0 || y >= Height)
                {
                    return null;
                }
                return _points[x + y * Width];
            }
            set => _points[x + y * Width] = value;
        }

        public IPoint this[int index]
        {
            get
            {
                if (index < 0 || index >= Width * Height)
                {
                    return null;
                }
                return _points[index];
            }
        }

        public Field(int width, int height) : this(width, height, new IPoint[width * height])
        {
        }

        public Field(int width, int height, IList<IPoint> field)
        {
            Width = width;
            Height = height;
            _points = field;
        }
    }
}
