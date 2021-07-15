﻿using System.Collections.Generic;

namespace Evolution
{
    public interface IPoint
    {

    }

    public class Creature : IPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int FoodValue { get; set; }
        public int[] Dna { get; set; }
    }

    public class Food : IPoint
    {
        public int Value { get; set; } = 10;
    }

    public class Wall : IPoint
    {

    }
}