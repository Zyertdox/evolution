using System.Collections.Generic;

namespace Evolution.Model
{
    public class Generation
    {
        public int RandomSeed { get; set; }
        public IList<CreatureRecord> Records { get; set; }
        public IList<string> Processors { get; set; }
        
        public int Width { get; set; }
        public int Height { get; set; }
        public IList<Point> Points { get; set; }
    }
}