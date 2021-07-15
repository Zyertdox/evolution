namespace Evolution.Interpreters
{
    public interface IProcessor
    {
        Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature);
    }
}