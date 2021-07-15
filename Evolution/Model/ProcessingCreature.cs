using System.Collections.Generic;

namespace Evolution.Model
{
    public class ProcessingCreature
    {
        public IList<int> Dna { get; set; }
        public HashSet<long> Processed { get; } = new HashSet<long>();
        public int X { get; set; }
        public int Y { get; set; }
        public int ProcessingIndex { get; set; } = 0;
        public int Direction { get; set; }

        public int Command => Dna[ProcessingIndex];

        private readonly int _totalCommands;

        public ProcessingCreature(Creature creature, int totalCommands)
        {
            Dna = creature.Dna;
            X = creature.X;
            Y = creature.Y;
            Direction = creature.Rotation;
            _totalCommands = totalCommands;
        }

        public bool NotProcessed()
        {
            return Processed.Add(ProcessingIndex + Direction * _totalCommands);
        }
    }
}
