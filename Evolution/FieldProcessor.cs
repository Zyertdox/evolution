using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public class FieldProcessor
    {
        public const int GenerationResetCount = 10;

        private readonly int _width;
        private readonly int _height;
        private readonly Random _random;

        private readonly IPoint[] _points;
        private readonly HashSet<Creature> _creatures;
        private int _foodBuffer;
        private readonly CreatureProcessor _creatureProcessor;
        private readonly StorageController _storageController;

        public short[] Field
        {
            get
            {
                var result = new short[_points.Length];
                for (var i = 0; i < _points.Length; i++)
                {
                    if (_points[i] is Creature)
                    {
                        result[i] = 2;
                    }

                    if (_points[i] is Food)
                    {
                        result[i] = 1;
                    }
                }

                return result;
            }
        }

        public FieldProcessor(int width, int height, StorageController storageController)
        {
            _width = width;
            _height = height;
            _storageController = storageController;
            var generation = storageController.LoadLatest();
            _random = new Random(generation.RandomSeed);
            _points = new IPoint[width * height];
            _creatures = new HashSet<Creature>();

            _foodBuffer = 4500;

            foreach (var record in generation.Records)
            {
                for (var i = 0; i < 10; i++)
                {
                    int x;
                    int y;
                    do
                    {
                        x = _random.Next(width);
                        y = _random.Next(height);
                    } while (_points[y * width + x] != null);

                    var point = new Creature
                    {
                        X = x,
                        Y = y,
                        FoodValue = 20,
                        Dna = record.Dna,
                        Parent = record.Id
                    };
                    _points[y * width + x] = point;
                    _creatures.Add(point);
                    _foodBuffer -= point.FoodValue;
                }
            }

            FillFood();
            _creatureProcessor = new CreatureProcessor(_points, width, height);
        }

        public bool Step()
        {
            foreach (var creature in _creatures)
            {
                creature.FoodValue--;
                _foodBuffer++;

                var movement = _creatureProcessor.GetMovement(creature);

                var nextX = creature.X + movement.MoveX;
                var nextY = creature.Y + movement.MoveY;
                if (nextX < 0 || nextX >= _width || nextY < 0 || nextY >= _height)
                {
                    nextX = creature.X;
                    nextY = creature.Y;
                }

                if (_points[nextY * _width + nextX] is Food food)
                {
                    creature.FoodValue += food.Value;
                    _points[nextY * _width + nextX] = null;
                }

                _points[creature.Y * _width + creature.X] = null;
                if (creature.FoodValue > 0)
                {
                    if (_points[nextY * _width + nextX] == null)
                    {
                        creature.X = nextX;
                        creature.Y = nextY;
                    }
                    _points[creature.Y * _width + creature.X] = creature;
                }
            }

            if (_creatures.Count(c => c.FoodValue > 0) > GenerationResetCount)
            {
                _creatures.RemoveWhere(c => c.FoodValue <= 0);
                FillFood();
                return true;
            }

            while (_creatures.Count > GenerationResetCount)
            {
                _creatures.Remove(_creatures.First(c => c.FoodValue <= 0));
            }

            var creatures = _creatures.Select(c => new CreatureRecord
            {
                Id = Guid.NewGuid(),
                ParentId = c.Parent,
                Dna = c.Dna
            }).ToArray();
            _storageController.Save(creatures);
            return false;
        }

        private void FillFood()
        {
            while (_foodBuffer > 0)
            {
                var x = _random.Next(_width);
                var y = _random.Next(_height);
                if (_points[y * _width + x] == null)
                {
                    var food = new Food();
                    _points[y * _width + x] = food;
                    _foodBuffer -= food.Value;
                }
            }
        }
    }
}
