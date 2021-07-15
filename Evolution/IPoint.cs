using Evolution.Interpreters;
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
        public DnaNode[] Dna { get; set; }
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
        public static Wall Default = new Wall();
    }

    public class DnaNode
    {
        public static DnaNode Create<T>(int localCommand) where T: IProcessor, new()
        {
            return new DnaNode
            {
                LocalCommand = localCommand,
                Processor = new T()
            };
        }
        public int LocalCommand { get; set; }
        public IProcessor Processor { get; set; }
    }
}