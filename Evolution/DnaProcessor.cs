using Evolution.Interpreters;
using Evolution.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public static class DnaProcessor
    {
        public const int GenerationSetCount = 10;

        public static HashSet<Creature> GetCreatures(Generation generation, Random random)
        {
            var processors = generation.Processors.Select(p => Activator.CreateInstance(Type.GetType(p)) as IProcessor).ToList();

            var creatures = new HashSet<Creature>();
            foreach (CreatureRecord creatureRecord in generation.Records)
            {
                creatures.Add(CreateCreature(creatureRecord, 8, processors, random));
                creatures.Add(CreateCreature(creatureRecord, 4, processors, random));
                creatures.Add(CreateCreature(creatureRecord, 4, processors, random));
                for (int i = 0; i < GenerationSetCount - 3; i++)
                {
                    creatures.Add(CreateCreature(creatureRecord, 0, processors, random));
                }
            }

            return creatures;
        }

        private static Creature CreateCreature(CreatureRecord creatureRecord, int mutations, List<IProcessor> processors,
            Random random)
        {
            var dna = creatureRecord.Dna.Take(RedirectProcessor.DnaLength).ToList();
            while (dna.Count < RedirectProcessor.DnaLength)
            {
                dna.Add(0);
            }
            for (int i = 0; i < mutations; i++)
            {
                var index = random.Next(RedirectProcessor.DnaLength);
                dna[index] = random.Next(DnaInterpreter.TotalCommands);
            }

            return new Creature
            {
                Parent = creatureRecord.Id,
                Dna = DnaInterpreter.Decode(processors, dna)
            };
        }
    }
}
