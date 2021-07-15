using Evolution.Model;
using System;

namespace Evolution.Interpreters
{
    public class FocusProcessor : IProcessor
    {
        public Command Process(int normalizedCommandNumber, ProcessingCreature processingCreature)
        {
            int direction = normalizedCommandNumber;
            direction = (direction + processingCreature.Direction) % 8;
            processingCreature.Target = GetAt(direction, processingCreature);
            processingCreature.ProcessingIndex++;
            return null;
        }

        private IPoint GetAt(int direction, ProcessingCreature processingCreature)
        {
            int x = processingCreature.X;
            int y = processingCreature.Y;
            int[] up = { 1, 0, 7 };
            int[] down = { 3, 4, 5 };
            int[] left = { 1, 2, 3 };
            int[] right = { 5, 6, 7 };

            if (Array.IndexOf(up, direction) >= 0)
            {
                y--;
            }

            if (Array.IndexOf(down, direction) >= 0)
            {
                y++;
            }

            if (Array.IndexOf(left, direction) >= 0)
            {
                x--;
            }

            if (Array.IndexOf(right, direction) >= 0)
            {
                x++;
            }

            return processingCreature.Field[x, y];
        }
    }
}
