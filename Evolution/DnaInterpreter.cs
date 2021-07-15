using Evolution.Interpreters;
using Evolution.Model;

namespace Evolution
{
    public class DnaInterpreter
    {
        public const int PredefinedCommandsCount = 21;
        public const int TotalCommands = PredefinedCommandsCount + RedirectProcessor.DnaLength;

        private readonly Field _field;

        public static int[] DefaultDna = { 5, 1, 2, 5, 1, 2, 1 };

        public DnaInterpreter(Field field)
        {
            _field = field;
        }

        public Movement GetMovement(Creature creature)
        {
            ProcessingCreature processingCreature = new ProcessingCreature(creature, TotalCommands, _field);

            while (processingCreature.NotProcessed())
            {
                if (processingCreature.Command < 2)
                {
                    MoveProcessor processor = new MoveProcessor();
                    return processor.Process(processingCreature.Command, processingCreature).Movement;
                }

                if (processingCreature.Command < 5)
                {
                    int normalized = processingCreature.Command - 2;
                    RotationProcessor processor = new RotationProcessor();
                    processor.Process(normalized, processingCreature);
                }
                else if (processingCreature.Command < 13)
                {
                    int normalzed = processingCreature.Command - 5;
                    FocusProcessor focusProcessor = new FocusProcessor();
                    focusProcessor.Process(normalzed, processingCreature);        // processingCreature.ProcessingIndex - +1
                    IdentifyProcessor processor = new IdentifyProcessor();
                    processor.Process(0, processingCreature); // processingCreature.ProcessingIndex - +1-2
                    processor.Process(1, processingCreature); // processingCreature.ProcessingIndex - +2-1
                    processingCreature.ProcessingIndex -= 3;
                }
                else if (processingCreature.Command < 21)
                {
                    int normalzed = processingCreature.Command - 13;
                    FocusProcessor focusProcessor = new FocusProcessor();
                    focusProcessor.Process(normalzed, processingCreature);        // processingCreature.ProcessingIndex - +1
                    IdentifyProcessor processor = new IdentifyProcessor();
                    processor.Process(1, processingCreature); // processingCreature.ProcessingIndex - +1-2
                    processingCreature.ProcessingIndex--;
                }
                else
                {
                    RedirectProcessor processor = new RedirectProcessor();
                    Command command = processor.Process(processingCreature.Command - PredefinedCommandsCount, processingCreature);
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
