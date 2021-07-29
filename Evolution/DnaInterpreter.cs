using Evolution.Interpreters;
using Evolution.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public class DnaInterpreter
    {
        public static int PredefinedCommandsCount => Processors.Sum(p => p.Item2.Length);
        public static int TotalCommands => PredefinedCommandsCount + RedirectProcessor.DnaLength;

        private readonly FieldWrapper _field;
        public static List<Tuple<int, IProcessor>> Processors = new List<Tuple<int, IProcessor>>();

        public static DnaNode[] DefaultDnaDecrypted = {
            DnaNode.Create<FocusProcessor>(0),
            DnaNode.Create<IdentifyProcessor>(2),
            DnaNode.Create<RedirectProcessor>(6),
            DnaNode.Create<IdentifyProcessor>(3),
            DnaNode.Create<RedirectProcessor>(6),
            DnaNode.Create<MoveProcessor>(1),
            DnaNode.Create<RotationProcessor>(0),
            DnaNode.Create<FocusProcessor>(0),
            DnaNode.Create<IdentifyProcessor>(2),
            DnaNode.Create<RedirectProcessor>(13),
            DnaNode.Create<IdentifyProcessor>(3),
            DnaNode.Create<RedirectProcessor>(13),
            DnaNode.Create<MoveProcessor>(1),
            DnaNode.Create<RotationProcessor>(0),
            DnaNode.Create<MoveProcessor>(1)
        };

        public DnaInterpreter(FieldWrapper field)
        {
            _field = field;
        }

        static DnaInterpreter()
        {
            Register(new MoveProcessor(),
                new RotationProcessor(),
                new FocusProcessor(),
                new IdentifyProcessor(),
                new RedirectProcessor());
        }

        private static void Register(params IProcessor[] processors)
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
                DnaNode dnaNode = processingCreature.ProcessingIndex >= processingCreature.Dna.Length
                    ? DnaNode.Create<MoveProcessor>(0)
                    : processingCreature.Dna[processingCreature.ProcessingIndex];
                Command command = dnaNode.Processor.Process(dnaNode.LocalCommand, processingCreature);
                if (command != null)
                {
                    return command.Movement;
                }
            }
            // Infinitive loop
            return MoveProcessor.NullMovement(processingCreature.Direction);
        }

        public static DnaNode[] Decode(IList<IProcessor> processors, IList<int> dna)
        {
            DnaNode[] commands = new DnaNode[dna.Count];
            for (int i = 0; i < dna.Count; i++)
            {
                int value = dna[i];
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
                commands[i] = new DnaNode
                {
                    LocalCommand = value,
                    Processor = processors[index]
                };
            }
            return commands;
        }

        public static int[] Encode(List<Tuple<int, IProcessor>> processors, DnaNode[] commands)
        {
            Dictionary<string, int> procLevels = processors.ToDictionary(p => p.Item2.GetType().FullName, p => p.Item1);
            int[] dna = new int[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                DnaNode command = commands[i];
                int level = procLevels[command.Processor.GetType().FullName];
                dna[i] = level + command.LocalCommand;
            }
            return dna;
        }
    }
}
