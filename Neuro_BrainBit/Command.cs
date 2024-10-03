using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuro_BrainBit
{
    public class Command
    {
        public string Usage { get; }
        public string Description { get; }       

        public Command(string usage, string description)
        {
            Usage = usage;
            Description = description;           
        }
    }
}
