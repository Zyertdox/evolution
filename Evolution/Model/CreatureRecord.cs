using System;
using System.Collections.Generic;

namespace Evolution.Model
{
    public class CreatureRecord
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public IList<int> Dna { get; set; }
    }
}