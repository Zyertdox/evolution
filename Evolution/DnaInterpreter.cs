using Evolution.Interpreters;
using Evolution.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public class DnaInterpreter
    {
        public const int PredefinedCommandsCount = 21;
        public const int TotalCommands = PredefinedCommandsCount + RedirectProcessor.DnaLength;

        private readonly Field _field;
        public static List<Tuple<int, IProcessor>> Processors = new List<Tuple<int, IProcessor>>();

        public static int[] DefaultDna = { 5, 15, 23, 16, 23, 1, 2, 5, 15, 30, 16, 30, 1, 2, 1 };

        public DnaInterpreter(Field field)
        {
            _field = field;
        }

        static DnaInterpreter()
        {
            Register(new IProcessor[]
            {
                new MoveProcessor(),
                new RotationProcessor(),
                new FocusProcessor(),
                new IdentifyProcessor(),
                new RedirectProcessor()
            });
        }

        private static void Register(IEnumerable<IProcessor> processors)
        {
            int index = 0;

            foreach (IProcessor processor in processors)
            {
                Processors.Add(new Tuple<int, IProcessor>(index, processor));
                index += processor.Length;
            }
        }

        public Movement GetMovement(Creature creature)
        {
            ProcessingCreature processingCreature = new ProcessingCreature(creature, TotalCommands, _field);

            while (processingCreature.NotProcessed())
            {
                int commandIndex = processingCreature.Command;
                foreach ((int level, IProcessor processor) in Processors)
                {
                    if (commandIndex >= level && commandIndex < level + processor.Length)
                    {
                        int normalized = commandIndex - level;
                        Command command = processor.Process(normalized, processingCreature);
                        if (command != null)
                        {
                            return command.Movement;
                        }
                        break;
                    }
                }
            }
            // Infinitive loop
            return MoveProcessor.NullMovement(processingCreature.Direction);
        }

        public static ExtendedCommand[] Decode(IList<IProcessor> processors, IList<int> dna)
        {
            ExtendedCommand[] commands = new ExtendedCommand[dna.Count];
            for (int i = 0; i < dna.Count; i++)
            {
                var value = dna[i];
                int index = 0;
                while (true)
                {
                    if (index >= processors.Count)
                    {
                        index = 0;
                        value = 0;
                        break;
                    }
                    if (value < processors[index].Length)
                    {
                        break;
                    }

                    value -= processors[index].Length;
                    index++;
                }
                commands[i] = new ExtendedCommand
                {
                    LocalCommand = value,
                    Processor = processors[index]
                };
            }
            return commands;
        }

        public static int[] Encode(List<Tuple<int, IProcessor>> processors, ExtendedCommand[] commands)
        {
            var procLevels = processors.ToDictionary(p => p.Item2.GetType().FullName, p => p.Item1);
            int[] dna = new int[commands.Length];
            for(int i = 0; i < commands.Length; i++)
            {
                var command = commands[i];
                var level = procLevels[command.Processor.GetType().FullName];
                dna[i] = level + command.LocalCommand;
            }
            return dna;
        }
    }
}
