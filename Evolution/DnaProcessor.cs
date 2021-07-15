using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Evolution
{
    public class DnaProcessor
    {
        public const int GenerationSetCount = 10;

        public Random Random;

        private readonly StorageController _storageController;
        public DnaProcessor(StorageController storageController)
        {
            _storageController = storageController;
            Random = new Random();
        }

        public HashSet<Creature> GetCreatures()
        {
            var baseCreatures = _storageController.LoadLatest();
            Random = new Random(baseCreatures.RandomSeed);
            var creatures = new HashSet<Creature>();
            foreach (CreatureRecord creatureRecord in baseCreatures.Records)
            {
                creatures.Add(CreateCreature(creatureRecord, 8));
                creatures.Add(CreateCreature(creatureRecord, 4));
                creatures.Add(CreateCreature(creatureRecord, 4));
                for (int i = 0; i < GenerationSetCount-3; i++)
                {
                    creatures.Add(CreateCreature(creatureRecord, 0));
                }
            }

            return creatures;
        }

        public void SaveCreatures(IEnumerable<Creature> creatures)
        {
            var creatureRecords = creatures.Select(c => new CreatureRecord
            {
                Id = Guid.NewGuid(),
                ParentId = c.Parent,
                Dna = c.Dna
            }).ToList();
            _storageController.Save(creatureRecords);
        }

        private Creature CreateCreature(CreatureRecord creatureRecord, int mutations)
        {
            var dna = creatureRecord.Dna.Take(DnaInterpreter.DnaLength).ToList();
            while (dna.Count < DnaInterpreter.DnaLength)
            {
                dna.Add(0);
            }
            for (int i = 0; i < mutations; i++)
            {
                var index = Random.Next(DnaInterpreter.DnaLength);
                dna[index] = Random.Next(DnaInterpreter.TotalCommands);
            }
            return new Creature
            {
                Parent = creatureRecord.Id,
                Dna = dna
            };
        }
    }
}
