using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2Assignment
{
    internal class Terminal
    {
        public string TerminalName { get; set; }
        public Dictionary<string, Airline> Airlines { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }
        public Dictionary<string, BoardingGate> BoardingGates { get; set; }
        public Dictionary<string, double> GateFees { get; set; }

        public Terminal(string tn)
        {
            TerminalName = tn;
            Airlines = new Dictionary<string, Airline>();
            Flights = new Dictionary<string, Flight>();
            BoardingGates = new Dictionary<string, BoardingGate>();
            GateFees = new Dictionary<string, double>();
        }

        public bool AddAirline(Airline al)
        {
            if (Airlines.ContainsKey(al.Code))
            {
                return false;
            }
            Airlines.Add(al.Code, al);
            return true;

        }

        public bool AddBoardingGate(BoardingGate bG)
        {
            if (BoardingGates.ContainsKey(bG.GateName))
            {
                return false;
            }
            BoardingGates.Add(bG.GateName, bG);
            return true;
        }

        public Airline GetAirlineFromFlight(Flight flight)
        {
            foreach (var airl in Airlines.Values)
            {
                if (airl.Flights.ContainsKey(flight.FlightNo))
                {
                    return airl;
                }

            }
            return null;
        }

        public void PrintAirlineFees(Dictionary<string, string> flightSpecialRequestCodes)
        {
            foreach (var airl in Airlines.Values)
            {
                double fees = Convert.ToDouble(airl.CalculateFees(flightSpecialRequestCodes));
                Console.WriteLine($"Airline: {airl.Name}, Fees: {fees}");
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
