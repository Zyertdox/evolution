using Evolution.Interpreters;
using Evolution.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public class DnaProcessor
    {
        public const int GenerationSetCount = 10;

        public Random Random;
        public DnaProcessor()
        {
            Random = new Random();
        }

        public HashSet<Creature> GetCreatures(Generation generation)
        {
            var processors = generation.Processors.Select(p => Activator.CreateInstance(Type.GetType(p)) as IProcessor).ToList();

            Random = new Random(generation.RandomSeed);
            var creatures = new HashSet<Creature>();
            foreach (CreatureRecord creatureRecord in generation.Records)
            {
                creatures.Add(CreateCreature(creatureRecord, 8, processors));
                creatures.Add(CreateCreature(creatureRecord, 4, processors));
                creatures.Add(CreateCreature(creatureRecord, 4, processors));
                for (int i = 0; i < GenerationSetCount - 3; i++)
                {
                    creatures.Add(CreateCreature(creatureRecord, 0, processors));
                }
            }

            return creatures;
        }

        private Creature CreateCreature(CreatureRecord creatureRecord, int mutations, List<IProcessor> processors)
        {
            var dna = creatureRecord.Dna.Take(RedirectProcessor.DnaLength).ToList();
            while (dna.Count < RedirectProcessor.DnaLength)
            {
                dna.Add(0);
            }
            for (int i = 0; i < mutations; i++)
            {
                var index = Random.Next(RedirectProcessor.DnaLength);
                dna[index] = Random.Next(DnaInterpreter.TotalCommands);
            }

            return new Creature
            {
                Parent = creatureRecord.Id,
                Dna = DnaInterpreter.Decode(processors, dna)
            };
        }
    }
}
