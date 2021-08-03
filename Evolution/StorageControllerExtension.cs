using Evolution.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public static class StorageControllerExtension
    {
        public static Generation ToStorageData(Field field)
        {
            List<int> walls = new List<int>();
            List<int> food = new List<int>();
            List<CreatureItem> creatures = new List<CreatureItem>();

            for (int i = 0; i < field.Points.Count; i++)
            {
                if (field.Points[i] == null)
                {
                    continue;
                }

                if (field.Points[i] is Wall)
                {
                    walls.Add(i);
                }

                if (field.Points[i] is Food)
                {
                    food.Add(i);
                }

                if (field.Points[i] is Creature creature)
                {
                    creatures.Add(new CreatureItem
                    {
                        I = Guid.NewGuid(),
                        P = creature.Parent,
                        R = creature.Rotation,
                        X = i,
                        V = creature.FoodValue,
                        D = DnaInterpreter.Encode(creature.Dna)
                    });
                }
            }

            return new Generation
            {
                Walls = walls.Any() ? walls : null,
                Creatures = creatures,
                Food = food.Any() ? food : null,
                Height = field.Height,
                Width = field.Width
            };
        }

        public static Field FromStorageData(Generation generation)
        {
            Field data = new Field(generation.Width, generation.Height, null);
            foreach (int i in generation.Walls)
            {
                data.Points[i] = Wall.Default;
            }

            foreach (int i in generation.Food)
            {
                data.Points[i] = new Food();
            }

            foreach (CreatureItem creature in generation.Creatures)
            {
                data.Points[creature.X] = new Creature()
                {
                    Parent = creature.P,
                    FoodValue = creature.V,
                    X = creature.X % generation.Width,
                    Y = creature.X / generation.Width,
                    Rotation = creature.R,
                    Dna = DnaInterpreter.Decode(ProcessorFactory.GetProcessors(generation.Processors), creature.D)
                };
            }

            return data;
        }
    }
}