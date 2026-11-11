using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2Assignment
{
    internal class NORMFlight : Flight
    {
        public NORMFlight(string fno, string o, string dest, DateTime expecttime, string stat) : base(fno, o, dest, expecttime, stat)
        {

        }

        public double CalculateFees()
        {
            double fees = 0;
            //Arriving flight fee
            if (Destination == "Singapore (SIN)")
            {
                fees += 500;
            }

            //Departure flight fee + Boarding gate base fee
            if (Origin == "Singapore (SIN)")
            {
                fees += 800; //departure
                fees += 300; //boarding gate
            }

            return fees;
        }

        public override string ToString()
        {
            return $"{FlightNo}, {Origin}, {Destination}, {ExpectedTime}, {Status}";
        }
    }
}
