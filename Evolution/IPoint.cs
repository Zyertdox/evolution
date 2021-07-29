using Evolution.Interpreters;
using System;

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

        public static Creature Default = new Creature
        {
            X = 0,
            Y = 0,
            FoodValue = 7,
            Dna = DnaInterpreter.DefaultDnaDecrypted,
            Parent = new Guid(),
            Rotation = 0
        };
    }

    public class Food : IPoint
    {
        public static Food Default = new Food();
        public int Value => 7;
    }

    public class Wall : IPoint
    {
        public static Wall Default = new Wall();
    }

    public class DnaNode
    {
        public static DnaNode Create<T>(int localCommand) where T : IProcessor, new()
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