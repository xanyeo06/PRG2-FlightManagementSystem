using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2Assignment
{
    public class Flight: IComparable<Flight>
    {
        public string FlightNo { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }

        public Flight(string fno, string o, string dest, DateTime expecttime, string stat) 
        {
            FlightNo = fno;
            Origin = o;
            Destination = dest;
            ExpectedTime = expecttime;
            Status = stat;
        }

        public double CalculateFees()
        {
            return 0;
        }

        public override string ToString()
        {
            return FlightNo + Origin + Destination + ExpectedTime + Status;
        }
        public int CompareTo(Flight other)
        {
            return this.ExpectedTime.CompareTo(other.ExpectedTime);
        }
    }
}
