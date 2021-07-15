using System.Collections.Generic;
using System.Linq;

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
            var passed = new HashSet<int>();
            var currentIndex = 0;
            var direction = creature.Rotation;
            while (!passed.Contains(currentIndex + direction * TotalCommands))
            {
                passed.Add(currentIndex + direction * TotalCommands);
                if (creature.Dna[currentIndex] == 0)
                {
                    return NullMovement(direction);
                }
                if (creature.Dna[currentIndex] == 1)
                {
                    return DirectMovement(direction);
                }

                if (creature.Dna[currentIndex] < 5)
                {
                    // rotation {2,3,4}
                    //  2 |   |  4    -1    1 |   | 3    x2    2 |   | 6      direction+
                    // ------------   =>   -----------   =>   -----------         =>         new direction
                    //    | 3 |               | 2 |              | 4 |
                    var rotation = creature.Dna[currentIndex] - 1;
                    rotation = rotation * 2;
                    direction = (direction + rotation) % 8;
                    currentIndex++;
                }
                else if (creature.Dna[currentIndex] < 13)
                {
                    // direction check {5-12} (example :current direction=2, arrow -> "0")
                    //  12 | 11 | 10          7 | 6 | 5                1 | 0 | 7
                    // --------------   -5   -----------    rotate    -----------         /  currentIndex + 1, canMove
                    //  5  | <- | 9     =>    0 | < | 4       =>       2 | < | 6    =>   {
                    // --------------        -----------              -----------         \  currentIndex + 2, !canMove
                    //  6  | 7  | 8           1 | 2 | 3                3 | 4 | 5                 
                    var tempDirection = creature.Dna[currentIndex] - 5;
                    tempDirection = (tempDirection + direction) % 8;
                    var target = GetAt(creature.X, creature.Y, tempDirection);
                    currentIndex++;
                    if (!(target == null || target is Food))
                    {
                        currentIndex++;
                    }
                }
                else if (creature.Dna[currentIndex] < 21)
                {
                    // direction check {13-20} (example :current direction=2, arrow -> "0")
                    //  20 | 19 | 18          7 | 6 | 5                1 | 0 | 7
                    // --------------   -13  -----------    rotate    -----------         /  currentIndex + 1, canMove
                    //  13 | <- | 17    =>    0 | < | 4       =>       2 | < | 6    =>   {
                    // --------------        -----------              -----------         \  currentIndex + 2, !canMove
                    //  14 | 15 | 16          1 | 2 | 3                3 | 4 | 5                 
                    var tempDirection = creature.Dna[currentIndex] - 13;
                    tempDirection = (tempDirection + direction) % 8;
                    var target = GetAt(creature.X, creature.Y, tempDirection);
                    currentIndex++;
                    if (!(target is Food))
                    {
                        currentIndex++;
                    }
                }
                else
                {
                    currentIndex = creature.Dna[currentIndex] - PredefinedCommandsCount;
                }

                if (currentIndex >= DnaLength)
                {
                    currentIndex = 0;
                }
            }
            // Infinitive loop
            return NullMovement(direction);
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

    public class Movement
    {
        public int MoveX { get; }
        public int MoveY { get; }
        public int Direction { get; }

        public Movement(int moveX, int moveY, int direction)
        {
            MoveX = moveX;
            MoveY = moveY;
            Direction = direction;
        }
    }
}
