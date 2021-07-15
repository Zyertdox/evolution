using Evolution.Model;

namespace Evolution.Interpreters
{
    public interface IProcessor
    {
        int Length { get; }
        Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature);
    }
}