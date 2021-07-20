using System;

namespace Evolution.Model
{
    public class Point
    {
        public PointType T { get; set; }
        
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? V { get; set; }
        public int[] D { get; set; }
        public Guid? P { get; set; }
        public int? R { get; set; }
    }

    public enum PointType
    {
        Wall,
        Food,
        Creature
    }
}