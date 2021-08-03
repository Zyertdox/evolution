using Evolution.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public class FieldProcessor
    {
        private readonly Random _random;
        
        public const int GenerationSetCount = 10;

        private readonly FieldWrapper _field;
        private readonly HashSet<Creature> _creatures;
        private int _foodBuffer;
        private readonly DnaInterpreter _creatureProcessor;
        private readonly StorageController _storageController;

        public short[] Field
        {
            get
            {
                short[] result = new short[_field.Length];
                for (int i = 0; i < _field.Length; i++)
                {
                    if (_field[i] is Creature)
                    {
                        result[i] = 2;
                    }

                    if (_field[i] is Food)
                    {
                        result[i] = 1;
                    }
                }

                return result;
            }
        }

        public FieldProcessor(StorageController storageController)
        {
            _storageController = storageController;
            Generation generation = _storageController.LoadLatest();

            _random = new Random(generation.RandomSeed);

            _creatures = new HashSet<Creature>(_field.Field.Points.Where(p => p is Creature).Cast<Creature>().ToList());
            Field field = StorageControllerExtension.FromStorageData(generation);
            _field = new FieldWrapper(field);

            FillFood();
            _creatureProcessor = new DnaInterpreter(_field);
        }

        public bool Step()
        {
            foreach (Creature creature in _creatures)
            {
                creature.FoodValue--;
                _foodBuffer++;

                Movement movement = _creatureProcessor.GetMovement(creature);

                int nextX = creature.X + movement.MoveX;
                int nextY = creature.Y + movement.MoveY;
                if (nextX < 0 || nextX >= _field.Width || nextY < 0 || nextY >= _field.Height)
                {
                    nextX = creature.X;
                    nextY = creature.Y;
                }

                if (_field[nextX, nextY] is Food food)
                {
                    creature.FoodValue += food.Value;
                    _field[nextX, nextY] = null;
                }

                _field[creature.X, creature.Y] = null;
                if (creature.FoodValue > 0)
                {
                    if (_field[nextX, nextY] == null)
                    {
                        creature.X = nextX;
                        creature.Y = nextY;
                        creature.Rotation = movement.Direction;
                    }
                    _field[creature.X, creature.Y] = creature;
                }
                else
                {
                    _foodBuffer += Creature.CreatureSkinFoodValue;
                }
            }

            if (_creatures.Count(c => c.FoodValue > 0) > GenerationSetCount)
            {
                _creatures.RemoveWhere(c => c.FoodValue <= 0);
                FillFood();
                return true;
            }

            while (_creatures.Count > GenerationSetCount)
            {
                _creatures.Remove(_creatures.First(c => c.FoodValue <= 0));
            }
            _storageController.Save(_field.Field);

            return false;
        }

        private void FillFood()
        {
            while (_foodBuffer > 0)
            {
                int x = _random.Next(_field.Width);
                int y = _random.Next(_field.Height);
                if (_field[x, y] == null)
                {
                    Food food = new Food();
                    _field[x, y] = food;
                    _foodBuffer -= food.Value;
                }
            }
        }

        public static Field InitDefaultField()
        {
            Field data = new Field(300, 300, Food.Default);
            data.Points[0] = Creature.Default;
            return data;
        }
    }
}
