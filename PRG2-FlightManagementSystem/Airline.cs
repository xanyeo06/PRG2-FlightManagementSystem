using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG2Assignment
{
    internal class Airline
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }

        public Airline(string n, string c)
        {
            Name = n;
            Code = c;
            Flights = new Dictionary<string, Flight>();
        }

        public bool AddFlight(Flight flight)
        {
            if (Flights.ContainsKey(flight.FlightNo))
            {
                return false;
            }
            Flights.Add(flight.FlightNo, flight);
            return true;
        }

        public bool RemoveFlight(Flight flight)
        {
            if (Flights.ContainsKey(flight.FlightNo))
            {
                Flights.Remove(flight.FlightNo);
                return true;
            }
            return false;
        }

        public (double subtotal, double discount) CalculateFees(Dictionary<string, string> flightSpecialRequestCodes)
        {
            double subtotal = 0;
            double discount = 0;
            int flightCount = 0;
            int earlyLateFlightCount = 0;
            int customOriginCount = 0;
            int noSpecialRequestCodeCount = 0;

            foreach (var flight in Flights.Values)
            {
                double flightFee = 0;

                // Base fee for arrival or departure
                if (flight.Destination == "Singapore (SIN)")
                {
                    flightFee = 500; // Fee for arriving flight
                }
                else if (flight.Origin == "Singapore (SIN)")
                {
                    flightFee = 800; // Fee for departing flight

                    // Boarding gate base fee
                    flightFee += 300;
                }

                // Additional fee for special request codes
                string specialRequestCode = "N.A";
                foreach (var kvp in flightSpecialRequestCodes)
                {
                    if (kvp.Key == flight.FlightNo)
                    {
                        specialRequestCode = kvp.Value;
                        break;
                    }
                }

                if (specialRequestCode == "DDJB")
                {
                    flightFee += 300;
                }
                else if (specialRequestCode == "CFFT")
                {
                    flightFee += 150;
                }
                else if (specialRequestCode == "LWTT")
                {
                    flightFee += 500;
                }
                else
                {
                    noSpecialRequestCodeCount++;
                }

                subtotal += flightFee;
                flightCount++;

                if (flight.ExpectedTime.TimeOfDay <= new TimeSpan(11, 0, 0) || flight.ExpectedTime.TimeOfDay >= new TimeSpan(21, 0, 0))
                {
                    earlyLateFlightCount++;
                }

                if (flight.Origin == "Dubai (DXB)" || flight.Origin == "Bangkok (BKK)" || flight.Origin == "Tokyo (NRT)") 
                {
                    customOriginCount++;
                }
            }

            // For more than 5 flights arriving/departing, airlines receive an additional discount of 3% off the Total Bill //TESTED. WORKING
            if (flightCount > 5)
            {
                discount += subtotal * 0.03;
            }

            // For flights arriving/departing before 11am or after 9pm, airlines receive a discount of $110 //TESTED. WORKING
            discount += earlyLateFlightCount * 110;

            // For airlines with the Origin of Dubai (DXB), Bangkok (BKK) or Tokyo (NRT), airlines receive a discount of $25 //TESTED. WORKING
            discount += customOriginCount * 25;

            // For not indicating any Special Request Codes, airlines receive a discount of $50 //TESTED.WORKING
            discount += noSpecialRequestCodeCount * 50;

            // For every 3 flights arriving/departing, airlines will receive a discount of $350 //TESTED.WORKING
            discount += (flightCount / 3) * 350;

            return (subtotal, discount);
        }

        public override string ToString()
        {
            return Name + Code;
        }
    }
}
