using System;

namespace Evolution.Interpreters
{
    internal class RotationProcessor : IProcessor
    {
        public Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature)
        {
            // rotation {0,1,2}
            //  0 |   |  2    -1    1 |   | 3    x2    2 |   | 6      direction+
            // ------------   =>   -----------   =>   -----------         =>         new direction
            //    | 1 |               | 2 |              | 4 |

            var rotation = (normalizedCommandNumber + 1) * 2;
            processingCreature.Direction = (processingCreature.Direction + rotation) % 8;
            processingCreature.ProcessingIndex++;
            return new Command();
        }
    }
}
