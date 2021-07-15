using Evolution.Interpreters;
using Evolution.Model;

namespace Evolution
{
    public class DnaInterpreter
    {
        public const int PredefinedCommandsCount = 21;
        public const int TotalCommands = PredefinedCommandsCount + RedirectProcessor.DnaLength;

        private readonly Field _field;

        public static int[] DefaultDna = { 5, 15, 23, 16, 23, 1, 2, 5, 15, 30, 16, 30, 1, 2, 1 };

        public DnaInterpreter(Field field)
        {
            _field = field;
        }

        public Movement GetMovement(Creature creature)
        {
            ProcessingCreature processingCreature = new ProcessingCreature(creature, TotalCommands, _field);

            while (processingCreature.NotProcessed())
            {
                if (processingCreature.ProcessingIndex >= RedirectProcessor.DnaLength)
                {
                    return MoveProcessor.NullMovement(processingCreature.Direction);
                }
                if (processingCreature.Command < 2)
                {
                    int normalized = processingCreature.Command;
                    MoveProcessor processor = new MoveProcessor();
                    return processor.Process(normalized, processingCreature).Movement;
                }
                else if (processingCreature.Command < 5)
                {
                    int normalized = processingCreature.Command - 2;
                    RotationProcessor processor = new RotationProcessor();
                    processor.Process(normalized, processingCreature);
                }
                else if (processingCreature.Command < 13)
                {
                    int normalized = processingCreature.Command - 5;
                    IProcessor processor = new FocusProcessor();
                    processor.Process(normalized, processingCreature);
                }
                else if (processingCreature.Command < 17)
                {
                    int normalized = processingCreature.Command - 13;
                    IProcessor processor = new IdentifyProcessor();
                    processor.Process(normalized, processingCreature);
                }
                else if (processingCreature.Command < 81)
                {
                    int normalized = processingCreature.Command - 17;
                    IProcessor processor = new RedirectProcessor();
                    Command command = processor.Process(normalized, processingCreature);
                    if (command != null)
                    {
                        return command.Movement;
                    }
                }
            }
            // Infinitive loop
            return MoveProcessor.NullMovement(processingCreature.Direction);
        }
    }
}
