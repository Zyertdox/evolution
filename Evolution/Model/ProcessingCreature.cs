using System.Collections.Generic;

namespace Evolution.Model
{
    public class ProcessingCreature
    {
        public ExtendedCommand[] DnaDecoded { get; private set; }
        public HashSet<long> Processed { get; } = new HashSet<long>();
        public int X { get; set; }
        public int Y { get; set; }
        public int ProcessingIndex { get; set; } = 0;
        public int Direction { get; set; }
        public Field Field { get; }
        public IPoint Target { get; set; }

        private readonly int _totalCommands;

        public ProcessingCreature(Creature creature, int totalCommands, Field field)
        {
            DnaDecoded = creature.DnaDecoded;
            X = creature.X;
            Y = creature.Y;
            Direction = creature.Rotation;
            _totalCommands = totalCommands;

            Field = field;
        }

        public bool NotProcessed()
        {
            return Processed.Add(ProcessingIndex + Direction * _totalCommands);
        }
    }
}
