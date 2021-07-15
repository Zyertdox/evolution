using System.Collections.Generic;

namespace Evolution.Model
{
    public class Generation
    {
        public int RandomSeed { get; set; }
        public IList<CreatureRecord> Records { get; set; }
        public IList<string> Processors { get; set; }
    }
}