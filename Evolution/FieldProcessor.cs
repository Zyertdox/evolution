﻿using System;
using System.Collections.Generic;
using System.Linq;
using Evolution.Model;

namespace Evolution
{
    public class FieldProcessor
    {
        private readonly Random _random;

        private readonly FieldWrapper _field;
        private readonly HashSet<Creature> _creatures;
        private int _foodBuffer;
        private readonly DnaInterpreter _creatureProcessor;
        private readonly StorageController _storageController;

        public short[] Field
        {
            get
            {
                var result = new short[_field.Length];
                for (var i = 0; i < _field.Length; i++)
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

        public FieldProcessor(int width, int height, StorageController storageController)
        {
            _storageController = storageController;
            var generation = _storageController.LoadLatest();

            _random = new Random(generation.RandomSeed);
            
            _creatures = DnaProcessor.GetCreatures(generation, _random);
            _field = new FieldWrapper(width, height);

            _foodBuffer = 10000;

            foreach (var creature in _creatures)
            {
                int x;
                int y;
                do
                {
                    x = _random.Next(width);
                    y = _random.Next(height);
                } while (_field[x, y] != null);

                creature.X = x;
                creature.Y = y;
                creature.Rotation = _random.Next(4) * 2;
                creature.FoodValue = 20;
                _field[x, y] = creature;
                _foodBuffer -= creature.FoodValue + Creature.CreatureSkinFoodValue;
            }

            FillFood();
            _creatureProcessor = new DnaInterpreter(_field);
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
            
            
            var creatureRecords = _creatures.Select(c => new CreatureRecord
            {
                Id = Guid.NewGuid(),
                ParentId = c.Parent,
                Dna = DnaInterpreter.Encode(DnaInterpreter.Processors, c.Dna)
            }).ToList();
            _storageController.Save(creatureRecords, _field.Field);
            
            return false;
        }

        private void FillFood()
        {
            while (_foodBuffer > 0)
            {
                var x = _random.Next(_field.Width);
                var y = _random.Next(_field.Height);
                if (_field[x, y] == null)
                {
                    var food = new Food();
                    _field[x, y] = food;
                    _foodBuffer -= food.Value;
                }
            }
        }

        public static Field InitDefaultField()
        {
            var data = new Field(300, 300, Food.Default);
            data.Points[0] = Creature.Default;
            return data;
        }
    }
}
