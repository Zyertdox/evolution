using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public class FieldProcessor
    {
        private readonly int _width;
        private readonly int _height;
        private readonly Random _random;

        private readonly IPoint[] _points;
        private readonly HashSet<Creature> _creatures;
        private int _foodBuffer;
        private readonly CreatureProcessor _creatureProcessor;
        private readonly DnaProcessor _dnaProcessor;

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

        public FieldProcessor(int width, int height, DnaProcessor dnaProcessor)
        {
            _width = width;
            _height = height;
            _dnaProcessor = dnaProcessor;
            _creatures = dnaProcessor.GetCreatures();
            _random = dnaProcessor.Random;
            _points = new IPoint[width * height];

            _foodBuffer = 10000;

            foreach (var creature in _creatures)
            {
                int x;
                int y;
                do
                {
                    x = _random.Next(width);
                    y = _random.Next(height);
                } while (_points[y * width + x] != null);

                creature.X = x;
                creature.Y = y;
                creature.Rotation = _random.Next(4);
                creature.FoodValue = 20;
                _points[y * width + x] = creature;
                _foodBuffer -= creature.FoodValue;
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

            if (_creatures.Count(c => c.FoodValue > 0) > DnaProcessor.GenerationSetCount)
            {
                _creatures.RemoveWhere(c => c.FoodValue <= 0);
                FillFood();
                return true;
            }

            while (_creatures.Count > DnaProcessor.GenerationSetCount)
            {
                _creatures.Remove(_creatures.First(c => c.FoodValue <= 0));
            }
            _dnaProcessor.SaveCreatures(_creatures);
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
