using System;
using System.Collections.Generic;

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

        public FieldProcessor(int width, int height, int seed)
        {
            _width = width;
            _height = height;
            _random = new Random(seed);
            _points = new IPoint[width * height];
            _creatures = new HashSet<Creature>();

            for (var i = 0; i < 100; i++)
            {
                var x = _random.Next(width);
                var y = _random.Next(height);
                if (_points[y * width + x] == null)
                {
                    var point = new Creature
                    {
                        X = x,
                        Y = y,
                        FoodValue = 20
                    };
                    _points[y * width + x] = point;
                    _creatures.Add(point);
                }
                else
                {
                    i--;
                }
            }

            _foodBuffer = 2500;
            FillFood();
        }

        public void Step()
        {
            foreach (var creature in _creatures)
            {
                creature.FoodValue--;
                _foodBuffer++;

                var nextX = creature.X - 1;
                var nextY = creature.Y;
                if (nextX < 0)
                {
                    nextX = creature.X;
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

            _creatures.RemoveWhere(c => c.FoodValue <= 0);
            FillFood();
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
