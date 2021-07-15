using System.Collections.Generic;
using System.Linq;
using Evolution.Interpreters;

namespace Evolution
{
    public class DnaInterpreter
    {
        public const int PredefinedCommandsCount = 21;
        public const int DnaLength = 64;
        public const int TotalCommands = PredefinedCommandsCount + DnaLength;

        private readonly IPoint[] _field;
        private readonly int _width;
        private readonly int _height;
        private static readonly Wall NullWall = new Wall();

        public static int[] DefaultDna = { 5, 1, 2, 5, 1, 2, 1 };

        public DnaInterpreter(IPoint[] field, int width, int height)
        {
            _field = field;
            _width = width;
            _height = height;
        }
        public Movement GetMovement(Creature creature)
        {
            ProcessingCreature processingCreature = new ProcessingCreature(creature, TotalCommands);

            while (processingCreature.NotProcessed())
            {
                if (processingCreature.Command < 2)
                {
                    var processor = new MoveProcessor();
                    return processor.Process(processingCreature.Command, processingCreature).Movement;
                }

                if (processingCreature.Command < 5)
                {
                    // rotation {2,3,4}
                    //  2 |   |  4    -1    1 |   | 3    x2    2 |   | 6      direction+
                    // ------------   =>   -----------   =>   -----------         =>         new direction
                    //    | 3 |               | 2 |              | 4 |
                    var rotation = processingCreature.Command - 1;
                    rotation = rotation * 2;
                    processingCreature.Direction = (processingCreature.Direction + rotation) % 8;
                    processingCreature.ProcessingIndex++;
                }
                else if (processingCreature.Command < 13)
                {
                    // direction check {5-12} (example :current direction=2, arrow -> "0")
                    //  12 | 11 | 10          7 | 6 | 5                1 | 0 | 7
                    // --------------   -5   -----------    rotate    -----------         /  currentIndex + 1, canMove
                    //  5  | <- | 9     =>    0 | < | 4       =>       2 | < | 6    =>   {
                    // --------------        -----------              -----------         \  currentIndex + 2, !canMove
                    //  6  | 7  | 8           1 | 2 | 3                3 | 4 | 5                 
                    var tempDirection = processingCreature.Command - 5;
                    tempDirection = (tempDirection + processingCreature.Direction) % 8;
                    var target = GetAt(creature.X, creature.Y, tempDirection);
                    processingCreature.ProcessingIndex++;
                    if (!(target == null || target is Food))
                    {
                        processingCreature.ProcessingIndex++;
                    }
                }
                else if (processingCreature.Command < 21)
                {
                    // direction check {13-20} (example :current direction=2, arrow -> "0")
                    //  20 | 19 | 18          7 | 6 | 5                1 | 0 | 7
                    // --------------   -13  -----------    rotate    -----------         /  currentIndex + 1, canMove
                    //  13 | <- | 17    =>    0 | < | 4       =>       2 | < | 6    =>   {
                    // --------------        -----------              -----------         \  currentIndex + 2, !canMove
                    //  14 | 15 | 16          1 | 2 | 3                3 | 4 | 5                 
                    var tempDirection = processingCreature.Command - 13;
                    tempDirection = (tempDirection + processingCreature.Direction) % 8;
                    var target = GetAt(creature.X, creature.Y, tempDirection);
                    processingCreature.ProcessingIndex++;
                    if (!(target is Food))
                    {
                        processingCreature.ProcessingIndex++;
                    }
                }
                else
                {
                    processingCreature.ProcessingIndex = processingCreature.Command - PredefinedCommandsCount;
                }

                if (processingCreature.ProcessingIndex >= DnaLength)
                {
                    processingCreature.ProcessingIndex = 0;
                }
            }
            // Infinitive loop
            return NullMovement(processingCreature.Direction);
        }

        private Movement NullMovement(int direction)
        {
            return new Movement(0, 0, direction);
        }
        private Movement DirectMovement(int direction)
        {
            switch (direction)
            {
                case 0: return new Movement(0, -1, 0);
                case 2: return new Movement(-1, 0, 2);
                case 4: return new Movement(0, 1, 4);
                case 6: return new Movement(1, 0, 6);
            }
            return new Movement(0, 0, 0);
        }

        public IPoint GetAt(int x, int y, int tempDirection)
        {
            int[] up = { 1, 0, 7 };
            int[] down = { 3, 4, 5 };
            int[] left = { 1, 2, 3 };
            int[] right = { 5, 6, 7 };

            if (up.Contains(tempDirection))
            {
                y--;
            }

            if (down.Contains(tempDirection))
            {
                y++;
            }

            if (left.Contains(tempDirection))
            {
                x--;
            }

            if (right.Contains(tempDirection))
            {
                x++;
            }

            if (x < 0 || y < 0 || x >= _width || y >= _height)
            {
                return NullWall;
            }

            return _field[y * _width + x];
        }
    }
}
