using System;
using System.Collections.Generic;

namespace Evolution
{
    public interface IPoint
    {

    }

    public class Creature : IPoint
    {
        public const int CreatureSkinFoodValue = 20;

        public int X { get; set; }
        public int Y { get; set; }
        public int FoodValue { get; set; }
        public IList<int> Dna { get; set; }
        public Guid Parent { get; set; }
        
        // Rotations
        //   | 0 |
        // 1 |   | 3
        //   | 2 |
        public int Rotation { get; set; }
    }

    public class Food : IPoint
    {
        public int Value { get; set; } = 7;
    }

    public class Wall : IPoint
    {

    }
}