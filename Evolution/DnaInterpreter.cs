using Evolution.Interpreters;
using Evolution.Model;
using System.Collections.Generic;
using System.Linq;

namespace Evolution
{
    public class DnaInterpreter
    {
        private static int TotalCommands => ProcessorFactory.IndexedProcessors.Keys.Sum(p => p.Length) + RedirectProcessor.DnaLength;

        private readonly FieldWrapper _field;

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

        public static DnaNode[] Decode(List<(int StartIndex, IProcessor Processor)> processors, IList<int> dna)
        {
            DnaNode[] commands = new DnaNode[dna.Count];
            for (int i = 0; i < dna.Count; i++)
            {
                var processorGroup =
                    processors.First(p => p.StartIndex <= dna[i] && p.StartIndex + p.Processor.Length > dna[i]);

                var local = dna[i] - processorGroup.StartIndex;
                
                commands[i] = new DnaNode
                {
                    LocalCommand = local,
                    Processor = processorGroup.Processor
                };
            }
            return commands;
        }

        public static int[] Encode(DnaNode[] commands)
        {
            int[] dna = new int[commands.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                DnaNode command = commands[i];
                int level = ProcessorFactory.IndexedProcessors[command.Processor];
                dna[i] = level + command.LocalCommand;
            }
            return dna;
        }
    }
}
