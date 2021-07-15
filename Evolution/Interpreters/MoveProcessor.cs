using Evolution.Model;

namespace Evolution.Interpreters
{
    public class MoveProcessor : IProcessor
    {
        public int Length => 2;

        public Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature)
        {
            Command command = new Command
            {
                Movement = NullMovement(processingCreature.Direction)
            };

            if (normalizedCommandNumber == 1)
            {
                command.Movement = DirectMovement(processingCreature.Direction);
            }

            return command;
        }

        public static Movement NullMovement(int direction)
        {
            return new Movement(0, 0, direction);
        }

        private static Movement DirectMovement(int direction)
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
    }
}
