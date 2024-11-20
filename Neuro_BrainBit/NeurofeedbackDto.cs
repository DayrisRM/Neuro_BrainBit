using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neuro_BrainBit
{
    public class NeurofeedbackDto
    {        
        public DateTime Date { get; set; }
        public double Beta { get; set; }
        public double Theta { get; set; }
        public double Ratio { get; set; }

        public NeurofeedbackDto(double beta, double theta, double ratio) 
        { 
            Date = DateTime.Now;
            Beta = beta;
            Theta = theta;
            Ratio = ratio;
        }

    }
}
