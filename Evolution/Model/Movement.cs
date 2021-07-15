namespace Evolution.Model
{
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