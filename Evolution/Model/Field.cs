using System.Collections.Generic;

namespace Evolution.Model
{
    public class Field
    {
        public IList<IPoint> Points { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Field(): this(0, 0, null)
        {
            
        }

        public Field(int width, int height, IPoint defaultValue)
        {
            Width = width;
            Height = height;
            Points = new IPoint[width * height];
            if (defaultValue != null)
            {
                for (int i = 0; i < Points.Count; i++)
                {
                    Points[i] = defaultValue;
                }
            }
        }
    }
}