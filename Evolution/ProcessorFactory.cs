using System;
using System.Collections.Generic;
using System.Linq;
using Evolution.Interpreters;

namespace Evolution
{
    public static class ProcessorFactory
    {
        private static readonly Dictionary<string, IProcessor> BaseProcessors;

        public static readonly Dictionary<IProcessor, int> IndexedProcessors = new();
        
        static ProcessorFactory()
        {
            var type = typeof(IProcessor);

            BaseProcessors = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.IsClass && type.IsAssignableFrom(p))
                .OrderBy(s => s.FullName)
                .Select(Activator.CreateInstance)
                .Cast<IProcessor>()
                .ToDictionary(p => p.GetType().FullName, p => p);

            int index = 0;
            foreach (var processor in BaseProcessors)
            {
                IndexedProcessors.Add(processor.Value, index);
                index += processor.Value.Length;
            }
        }

        public static List<(int StartIndex, IProcessor Processor)> GetProcessors(IList<string> processorNames)
        {
            var sortedProcessors = new List<(int StartIndex, IProcessor Processor)>();

            int index = 0;

            foreach (var processorName in processorNames)
            {
                sortedProcessors.Add((index, BaseProcessors[processorName]));
                index += BaseProcessors[processorName].Length;
            }

            return sortedProcessors;
        }
    }
}
