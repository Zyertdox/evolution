using Evolution.Model;

namespace Evolution.Interpreters
{
    public class RedirectProcessor : IProcessor
    {
        public const int DnaLength = 64;
        public int Length => DnaLength;

        public Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature)
        {
            processingCreature.ProcessingIndex = normalizedCommandNumber;
            return null;
        }
    }
}
