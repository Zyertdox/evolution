using System;
using System.Collections.Generic;
using System.Linq;
using Evolution.Model;

namespace Evolution
{
    public static class StorageControllerExtension
    {
        public static Generation ToStorageData(FieldData fieldData)
        {
            var walls = new List<int>();
            var food = new List<int>();
            var creatures = new List<CreatureItem>();

            for (int i = 0; i < fieldData.Points.Count; i++)
            {
                if (fieldData.Points[i] == null)
                {
                    continue;
                }

                if (fieldData.Points[i] is Wall)
                {
                    walls.Add(i);
                }

                if (fieldData.Points[i] is Food)
                {
                    food.Add(i);
                }

                if (fieldData.Points[i] is Creature creature)
                {
                    creatures.Add(new CreatureItem
                    {
                        I = Guid.NewGuid(),
                        P = creature.Parent,
                        R = creature.Rotation,
                        X = i,
                        V = creature.FoodValue,
                        D = DnaInterpreter.Encode(DnaInterpreter.Processors, creature.Dna)
                    });
                }
            }

            return new Generation
            {
                Walls = walls.Any() ? walls : null,
                Creatures = creatures,
                Food = food.Any() ? food : null,
                Height = fieldData.Height,
                Width = fieldData.Width
            };
        }

        public static FieldData FromStorageData(Generation generation)
        {
            FieldData data = new FieldData(generation.Width, generation.Height, null);
            foreach (var i in generation.Walls)
            {
                data.Points[i] = Wall.Default;
            }

            foreach (var i in generation.Food)
            {
                data.Points[i] = new Food();
            }

            foreach (var creature in generation.Creatures)
            {
                data.Points[creature.X] = new Creature()
                {
                    Parent = creature.P,
                    FoodValue = creature.V,
                    X = creature.X % generation.Width,
                    Y = creature.X / generation.Width,
                    Rotation = creature.R,
                    Dna = DnaInterpreter.Decode(DnaInterpreter.Processors.Select(p => p.Item2).ToList(), creature.D)
                };
            }

            return data;
        }
    }
}