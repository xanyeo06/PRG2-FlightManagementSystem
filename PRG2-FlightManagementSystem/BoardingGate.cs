using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2Assignment
{
    internal class BoardingGate
    {
        public string GateName { get; set; }
        public bool SupportsCFFT { get; set; }
        public bool SupportsDDJB { get; set; }
        public bool SupportsLWTT { get; set; }
        public Flight Flight { get; set; }

        public BoardingGate(string gn, bool supportscfft, bool supportsddjb, bool supportslwtt, Flight f)
        {
            GateName = gn;
            SupportsCFFT = supportscfft;
            SupportsDDJB = supportsddjb;
            SupportsLWTT = supportslwtt;
            Flight = f;
        }

        public double CalculateFees()
        {
            return 0;
        }


        public override string ToString()
        {
            return GateName + SupportsCFFT + SupportsDDJB + SupportsLWTT + Flight;
        }
    }
}
