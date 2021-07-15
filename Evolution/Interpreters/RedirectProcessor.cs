using Evolution.Model;

namespace Evolution.Interpreters
{
    public class RedirectProcessor : IProcessor
    {
        public const int DnaLength = 64;
        public Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature)
        {
            if (normalizedCommandNumber < DnaLength)
            {
                processingCreature.ProcessingIndex = normalizedCommandNumber;
                return null;
            }

            return new Command
            {
                Movement = MoveProcessor.NullMovement(processingCreature.Direction)
            };
        }
    }
}
