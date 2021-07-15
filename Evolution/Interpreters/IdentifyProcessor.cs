using Evolution.Model;

namespace Evolution.Interpreters
{
    public class IdentifyProcessor : IProcessor
    {
        public Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature)
        {
            processingCreature.ProcessingIndex++;
            bool isCorrectTarget =
                normalizedCommandNumber == 0 && processingCreature.Target == null ||
                normalizedCommandNumber == 1 && processingCreature.Target is Food ||
                normalizedCommandNumber == 2 && processingCreature.Target is Wall ||
                normalizedCommandNumber == 3 && processingCreature.Target is Creature;
            if (!isCorrectTarget)
            {
                processingCreature.ProcessingIndex++;
            }

            return null;
        }
    }
}
