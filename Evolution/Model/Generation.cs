using System;
using System.Collections.Generic;

namespace Evolution.Model
{
    public class Generation
    {
        public int RandomSeed { get; set; }
        public IList<CreatureRecord> Records { get; set; }
        public IList<string> Processors { get; set; }
        
        public int Width { get; set; }
        public int Height { get; set; }
        
        public IList<int> Walls { get; set; }
        public IList<int> Food { get; set; }
        public IList<CreatureItem> Creatures { get; set; }
        
    }

    public class CreatureItem
    {
        public Guid I { get; set; }
        public int X { get; set; }
        public int V { get; set; }
        public int[] D { get; set; }
        public Guid P { get; set; }
        public int R { get; set; }
    }
}