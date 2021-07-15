using System.Collections.Generic;

namespace Evolution
{
    public class DnaInterpreter
    {
        public const int PredefinedCommandsCount = 25;
        public const int DnaLength = 64;

        private readonly IPoint[] _field;
        private readonly int _width;
        private readonly int _height;

        private static readonly Wall NullWall = new Wall();
        private static readonly Movement NullMovement = new Movement(0, 0);

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
            while (!passed.Contains(currentIndex))
            {
                passed.Add(currentIndex);
                switch (creature.Dna[currentIndex])
                {
                    #region Movement (0-8)
                    // Movements
                    //  4 | 3 | 2
                    // -----------
                    //  5 | 0 | 1
                    // -----------
                    //  6 | 7 | 8 
                    case 0: return NullMovement;
                    case 1: return new Movement(1, 0);
                    case 2: return new Movement(1, 1);
                    case 3: return new Movement(0, 1);
                    case 4: return new Movement(-1, 1);
                    case 5: return new Movement(-1, 0);
                    case 6: return new Movement(-1, -1);
                    case 7: return new Movement(0, -1);
                    case 8: return new Movement(1, -1);
                    #endregion Movement
                    #region Can Move (9-16)
                    // Can move?
                    //  12 | 11 | 10
                    // --------------
                    //  13 |    |  9 
                    // --------------
                    //  14 | 15 | 16 
                    case 9:
                        currentIndex++;
                        if (!CanMove(creature.X + 1, creature.Y))
                        {
                            currentIndex++;
                        }
                        break;
                    case 10:
                        currentIndex++;
                        if (!CanMove(creature.X + 1, creature.Y + 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 11:
                        currentIndex++;
                        if (!CanMove(creature.X, creature.Y + 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 12:
                        currentIndex++;
                        if (!CanMove(creature.X - 1, creature.Y + 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 13:
                        currentIndex++;
                        if (!CanMove(creature.X - 1, creature.Y))
                        {
                            currentIndex++;
                        }
                        break;
                    case 14:
                        currentIndex++;
                        if (!CanMove(creature.X - 1, creature.Y - 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 15:
                        currentIndex++;
                        if (!CanMove(creature.X, creature.Y - 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 16:
                        currentIndex++;
                        if (!CanMove(creature.X + 1, creature.Y - 1))
                        {
                            currentIndex++;
                        }
                        break;
                    #endregion Can Move
                    #region Can Eat (17-24)
                    // Can move?
                    //  20 | 19 | 18
                    // --------------
                    //  21 |    | 17 
                    // --------------
                    //  22 | 23 | 24 
                    case 17:
                        currentIndex++;
                        if (!CanEat(creature.X + 1, creature.Y))
                        {
                            currentIndex++;
                        }
                        break;
                    case 18:
                        currentIndex++;
                        if (!CanEat(creature.X + 1, creature.Y + 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 19:
                        currentIndex++;
                        if (!CanEat(creature.X, creature.Y + 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 20:
                        currentIndex++;
                        if (!CanEat(creature.X - 1, creature.Y + 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 21:
                        currentIndex++;
                        if (!CanEat(creature.X - 1, creature.Y))
                        {
                            currentIndex++;
                        }
                        break;
                    case 22:
                        currentIndex++;
                        if (!CanEat(creature.X - 1, creature.Y - 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 23:
                        currentIndex++;
                        if (!CanEat(creature.X, creature.Y - 1))
                        {
                            currentIndex++;
                        }
                        break;
                    case 24:
                        currentIndex++;
                        if (!CanEat(creature.X + 1, creature.Y - 1))
                        {
                            currentIndex++;
                        }
                        break;
                        #endregion Can Eat
                    default:
                        currentIndex = creature.Dna[currentIndex] - PredefinedCommandsCount;
                        break;
                }

                if (currentIndex >= DnaLength)
                {
                    currentIndex = 0;
                }
            }
            // Infinitive loop
            return NullMovement;
        }

        public bool CanEat(int x, int y)
        {
            var target = GetAt(x, y);
            return !(target is Food);
        }
        public bool CanMove(int x, int y)
        {
            var target = GetAt(x, y);
            return !(target is Wall || target is Creature);
        }
        public IPoint GetAt(int x, int y)
        {
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

        public Movement(int moveX, int moveY)
        {
            MoveX = moveX;
            MoveY = moveY;
        }
    }
}
