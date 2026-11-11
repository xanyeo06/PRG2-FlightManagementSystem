using PRG2Assignment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http.Headers;

Terminal terminal = new Terminal("Terminal 5");
Dictionary<string, Airline> Airlines = terminal.Airlines;
Dictionary<string, BoardingGate> BoardingGates = terminal.BoardingGates;
Dictionary<string, Flight> Flights = terminal.Flights;
Dictionary<string, string> flightSpecialRequestCodes = new Dictionary<string, string>();

int airlineCount = 0;
int boardingGateCount = 0;
int flightCount = 0;

//NEW LOADING OF AIRLINES.CSV
Console.WriteLine($"Loading Airlines...");
using (StreamReader sr = new StreamReader("airlines.csv", false))
{
    string? s;
    s = sr.ReadLine(); //skips header
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        Airline a = new Airline(data[0], data[1]); //new airline object with [airlinename, airlinecode]
        if (terminal.AddAirline(a)) //if added successfully, increment airlineCount
        {
            airlineCount++;
        }
        else
        {
            Console.WriteLine($"Failed to add airline with code {data[0]}."); //display error message if failed to add (csv file wrong format etc.)
        }
    }
}
Console.WriteLine($"{airlineCount} Airlines Loaded!"); //display total number of airlines loaded

//NEW LOADING OF BOARDINGGATES.CSV
Console.WriteLine("Loading Boarding Gates...");
using (StreamReader sr = new StreamReader("boardinggates.csv", false))
{
    string? s;
    s = sr.ReadLine(); // skips header
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        //Convert.ToBoolean : converts string representation to boolean
        bool ddjbstatus = Convert.ToBoolean(data[1]);
        bool cfftstatus = Convert.ToBoolean(data[2]);
        bool lwttstatus = Convert.ToBoolean(data[3]);
        BoardingGate b = new BoardingGate(data[0], cfftstatus, ddjbstatus, lwttstatus, null); //create a new boardinggate with the parameters
        if (terminal.AddBoardingGate(b))
        {
            boardingGateCount++;
        }
        else
        {
            Console.WriteLine($"Failed to add boarding gate with name {data[0]}.");
        }
    }
}
Console.WriteLine($"{boardingGateCount} Boarding Gates Loaded!");

// Loading flights.csv
Console.WriteLine("Loading Flights...");
using (StreamReader sr = new StreamReader("flights.csv", false))
{
    string? s;
    s = sr.ReadLine(); //skips the header
    while ((s = sr.ReadLine()) != null)
    {
        string[] data = s.Split(',');
        DateTime expectedtime = Convert.ToDateTime(data[3]);
        Flight? f = null; // Use nullable type
        string flightSpecialRequestCode = data[4];
        double requestFee = 0;

        if (string.IsNullOrEmpty(flightSpecialRequestCode))
        {
            flightSpecialRequestCode = "N.A"; //if special request code in csv empty, set default value to N.A
        }

        //Create flights based on the SRC
        if (flightSpecialRequestCode == "DDJB")
        {
            f = new DDJBFlight(data[0], data[1], data[2], expectedtime, "Scheduled", requestFee);
        }
        else if (flightSpecialRequestCode == "CFFT")
        {
            f = new CFFTFlight(data[0], data[1], data[2], expectedtime, "Scheduled", requestFee);
        }
        else if (flightSpecialRequestCode == "LWTT")
        {
            f = new LWTTFlight(data[0], data[1], data[2], expectedtime, "Scheduled", requestFee);
        }
        else if (flightSpecialRequestCode == "N.A")
        {
            f = new NORMFlight(data[0], data[1], data[2], expectedtime, "Scheduled");
        }

        if (f != null) //if flight is not empty
        {
            terminal.Flights.Add(data[0], f); //add flight to terminal, with the flight no. as the key
            flightSpecialRequestCodes[data[0]] = flightSpecialRequestCode; // Store the special request code
            flightCount++;

            // Add the flight to the respective airline
            string airlineCode = data[0].Split(' ')[0]; //extract airline code from flight number
            if (terminal.Airlines.ContainsKey(airlineCode)) //check if airline exists
            {
                terminal.Airlines[airlineCode].AddFlight(f);
            }
            else
            {
                Console.WriteLine($"Error: Airline with code {airlineCode} not found for flight {data[0]}.");
            }
        }
        else
        {
            Console.WriteLine($"Error: Flight object for {data[0]} could not be created.");
        }
    }
}
Console.WriteLine($"{flightCount} Flights Loaded!");

Console.WriteLine(""); //separation line

// Main menu
MainMenu();
void MainMenu()
{
    while (true)
    {
        try
        {
            Console.WriteLine(" ");
            Console.WriteLine("==============================================");
            Console.WriteLine("Welcome to Changi Airport Terminal 5");
            Console.WriteLine("==============================================");
            Console.WriteLine("1. List All Flights");
            Console.WriteLine("2. List Boarding Gates");
            Console.WriteLine("3. Assign a Boarding Gate to a Flight");
            Console.WriteLine("4. Create Flight");
            Console.WriteLine("5. Display Airline Flights");
            Console.WriteLine("6. Modify Flight Details");
            Console.WriteLine("7. Display Flight Schedule");
            Console.WriteLine("8. Display Total Fees for an Airline [Additional Feature b]");
            Console.WriteLine("9. Process all unassigned flights to boarding gates in bulk [Additional feature a] ");
            Console.WriteLine("0. Exit");
            Console.WriteLine("");
            Console.Write("Please select your option:");

            string option = Console.ReadLine();

            if (option == "1")
            {
                ListAllFlights();
            }

            else if (option == "2")
            {
                ListBoardingGates();
            }

            else if (option == "3")
            {
                AssignBoardingGateToFlight();
            }

            else if (option == "4")
            {
                CreateFlight();
            }

            else if (option == "5")
            {
                DisplayAirlineFlights(out Airline selectedAirline);
            }

            else if (option == "6")
            {
                ModifyFlightDetails();
            }

            else if (option == "7")

            {
                DisplayFlightSchedule();
            }

            else if (option == "8")
            {
                DisplayTotalFeePerAirlineForTheDay();
            }

            else if (option == "9")
            {
                ProcessUnassignedFlightsInBulk();
            }

            else if (option == "0")
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }

           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine("Invalid option. Please try again.");
        }
    }
}
void ListAllFlights()
{
    // Displaying basic info
    Console.WriteLine(" ");
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Flights for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("Flight Number   Airline Name         Origin               Destination          Expected Departure/Arrival Time");

    foreach (var flight in terminal.Flights.Values)
    {
        Airline airline = terminal.GetAirlineFromFlight(flight); //get airline from flight
        string airlineName = airline != null ? airline.Name : "Unknown";

        Console.WriteLine($"{flight.FlightNo,-15} {airlineName,-20} {flight.Origin,-20} {flight.Destination,-20} {flight.ExpectedTime:dd/MM/yyyy hh:mm tt}");
    }
    Console.WriteLine(" "); //separation line
}

void ListBoardingGates()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("Gate Name       DDJB                   CFFT                   LWTT");

    foreach (var gate in terminal.BoardingGates.Values)
    {
        Console.WriteLine($"{gate.GateName,-15} {gate.SupportsDDJB,-22} {gate.SupportsCFFT,-22} {gate.SupportsLWTT,-22}");
    }
    Console.WriteLine(" "); //separation line
}

void AssignBoardingGateToFlight()
{
    while (true)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("Assign a Boarding Gate to a Flight");
        Console.WriteLine("=============================================");
        Console.Write("Enter Flight Number: ");
        string flightNo = Console.ReadLine().ToUpper();

        if (!Flights.ContainsKey(flightNo)) //if flight number not found
        {
            Console.WriteLine("Flight not found. Please try again.");
            continue;
        }

        string gateName;
        while (true)
        {
            Console.Write("Enter Boarding Gate Name: ");
            gateName = Console.ReadLine().ToUpper();

            if (!BoardingGates.ContainsKey(gateName))
            {
                Console.WriteLine("The Boarding Gate does not exist. Please try again.");
            }
            else if (BoardingGates[gateName].Flight != null)
            {
                Console.WriteLine("The Boarding Gate is already assigned to another flight. Please try again.");
            }
            else
            {
                // Remove the flight from any previously assigned gate
                foreach (var currentGate in BoardingGates.Values)
                {
                    if (currentGate.Flight == Flights[flightNo])
                    {
                        currentGate.Flight = null;
                        break;
                    }
                }

                BoardingGates[gateName].Flight = Flights[flightNo];
                Flights[flightNo].Status = "Scheduled";
                break;
            }
        }

        // Retrieving details
        Flight flight = Flights[flightNo];
        BoardingGate gate = BoardingGates[gateName];
        string specialRequestCode = flightSpecialRequestCodes[flightNo];

        // Displaying details
        Console.WriteLine(" "); // separation line
        Console.WriteLine($"Flight Number: {flight.FlightNo}");
        Console.WriteLine($"Origin: {flight.Origin}");
        Console.WriteLine($"Destination: {flight.Destination}");
        Console.WriteLine($"Expected Time: {flight.ExpectedTime}");
        Console.WriteLine($"Special Request Code: {specialRequestCode}");
        Console.WriteLine($"Boarding Gate Name: {gateName}");
        Console.WriteLine($"Supports DDJB: {gate.SupportsDDJB}");
        Console.WriteLine($"Supports CFFT: {gate.SupportsCFFT}");
        Console.WriteLine($"Supports LWTT: {gate.SupportsLWTT}");

        // Prompt user if want to update status
        while (true)
        {
            Console.Write("Would you like to update the status of the flight? (Y/N): ");
            string updateStatus = Console.ReadLine().ToUpper();

            if (updateStatus == "Y")
            {
                while (true)
                {
                    Console.WriteLine("1. Delayed");
                    Console.WriteLine("2. Boarding");
                    Console.WriteLine("3. On Time");
                    Console.Write("Please select the new status of the flight: ");
                    string statusOption = Console.ReadLine();

                    if (statusOption == "1")
                    {
                        flight.Status = "Delayed";
                        break;
                    }
                    else if (statusOption == "2")
                    {
                        flight.Status = "Boarding";
                        break;
                    }
                    else if (statusOption == "3")
                    {
                        flight.Status = "On Time";
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid option. Please try again.");
                        continue;
                    }
                }
                break;
            }
            else if (updateStatus == "N")
            {
                // Do nothing, just continue
                flight.Status = "On Time";
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Please try again.");
            }
        }

        Console.WriteLine($"Flight {flight.FlightNo} has been assigned to Boarding Gate {gate.GateName}!");
        break;
    }
}

void CreateFlight()
{
    while (true)
    {
        string flightNo;
        while (true)
        {
            Console.Write("Enter Flight Number: ");
            flightNo = Console.ReadLine().ToUpper();

            // Check if flight number is empty
            if (string.IsNullOrWhiteSpace(flightNo))
            {
                Console.WriteLine("Flight Number cannot be empty. Please try again.");
                continue;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(flightNo, @"^[A-Z]{2}\s?\d{3}$")) //Regex: Ensures flight number is in the format of 2 letters followed by 3 digits
            {
                Console.WriteLine("Invalid Flight Number format. Please try again.");
                continue;
            }

            // Check if flight already exists
            if (terminal.Flights.ContainsKey(flightNo))
            {
                Console.WriteLine("Flight Number already exists. Please try again.");
                continue;
            }

            break;
        }

        string origin;
        while (true)
        {
            Console.Write("Enter Origin: ");
            origin = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(origin))
            {
                Console.WriteLine("Origin cannot be empty. Please try again.");
                continue;
            }

            break;
        }

        string destination;
        while (true)
        {
            Console.Write("Enter Destination: ");
            destination = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(destination))
            {
                Console.WriteLine("Destination cannot be empty. Please try again.");
                continue;
            }

            break;
        }

        DateTime expectedTime;
        while (true)
        {
            Console.Write("Enter Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
            string expectedTimeStr = Console.ReadLine();

            if (DateTime.TryParseExact(expectedTimeStr, "dd/MM/yyyy HH:mm", //Parse: Converts the string representation of a date and time to its DateTime equivalent
                System.Globalization.CultureInfo.InvariantCulture, //CultureInfo: Ensures date format is interpreted correctly
                System.Globalization.DateTimeStyles.None, out expectedTime)) //DateTimeStyles: Specifies the style for a date and time value that is passed to the TryParseExact methods.
            {
                break; // Exits the loop if parsing is successful
            }

            Console.WriteLine("Invalid date/time format. Please use dd/mm/yyyy hh:mm");
        }

        string specialRequestCode;
        while (true)
        {
            Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
            specialRequestCode = Console.ReadLine().Trim().ToUpper(); // Makes input uppercase and trim spaces

            if (specialRequestCode != "CFFT" && specialRequestCode != "DDJB" &&
                specialRequestCode != "LWTT" && specialRequestCode != "NONE")
            {
                Console.WriteLine("Invalid Special Request Code. Please try again.");
                continue; // Retry for invalid input
            }

            break; // Valid input, exit loop
        }

        // Create flight object based on special request code
        Flight newFlight;
        if (specialRequestCode == "DDJB")
        {
            newFlight = new DDJBFlight(flightNo, origin, destination, expectedTime, "Scheduled", 0);
        }
        else if (specialRequestCode == "CFFT")
        {
            newFlight = new CFFTFlight(flightNo, origin, destination, expectedTime, "Scheduled", 0);
        }
        else if (specialRequestCode == "LWTT")
        {
            newFlight = new LWTTFlight(flightNo, origin, destination, expectedTime, "Scheduled", 0);
        }
        else
        {
            newFlight = new NORMFlight(flightNo, origin, destination, expectedTime, "Scheduled");
        }

        // Add flight to dict
        terminal.Flights.Add(flightNo, newFlight);
        Console.WriteLine($"Flight {flightNo} has been added!");

        // Extract airline code from flight number
        string airlineCode = flightNo.Split(' ')[0];

        // Check if airline exists
        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            Airline airline = terminal.Airlines[airlineCode];
            if (airline.AddFlight(newFlight))
            {
                // Store special request code
                flightSpecialRequestCodes[flightNo] = specialRequestCode == "" ? "N.A" : specialRequestCode;

                // Update flights.csv file
                using (StreamWriter writer = File.AppendText("flights.csv"))
                {
                    writer.WriteLine($"{flightNo},{origin},{destination},{expectedTime:dd/MM/yyyy HH:mm},{specialRequestCode}");
                }
            }
            else
            {
                Console.WriteLine("Failed to add flight to the airline.");
            }
        }
        else
        {
            Console.WriteLine($"Error: Airline with code {airlineCode} not found.");
        }

        Console.Write("Would you like to add another flight? (Y/N): ");
        string addAnotherFlight = Console.ReadLine().ToUpper();
        if (addAnotherFlight != "Y")
        {
            break;
        }
    }
}

void DisplayAirlineFlights(out Airline selectedAirline) //after the method is called, selectedAirline will contain the airline selected by user, which is used in subsequent code (ModifyFlightDetails)
{
    selectedAirline = null;
    while (true) // loop until a valid airline is selected by user
    {
        try
        {
            //headers
            Console.WriteLine("==============================================");
            Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
            Console.WriteLine("==============================================");
            Console.WriteLine($"{"Airline Code",-13}{"Airline Name",-19}");


            // Listing all the airlines available
            foreach (var airline in terminal.Airlines.Values)
            {
                Console.WriteLine($"{airline.Code,-13}{airline.Name,-19}");
            }
            Console.WriteLine(" "); // separation line

            // Prompting user for airline code
            Console.Write("Enter the 2-Letter Airline Code (e.g. SQ or MH, etc.): ");
            string airlineCode = Console.ReadLine().ToUpper();

            // Validate the input
            if (string.IsNullOrEmpty(airlineCode) || airlineCode.Length != 2)
            {
                Console.WriteLine("Invalid Airline Code. Please enter a valid 2-letter code.");
                continue;
            }

            // Retrieve the selected airline
            foreach (var airline in terminal.Airlines.Values)
            {
                if (airline.Code == airlineCode)
                {
                    selectedAirline = airline;
                    break;
                }
            }

            // Check if the airline code is valid
            if (selectedAirline == null)
            {
                Console.WriteLine("Airline Code not found. Please try again.");
                continue;
            }

            // Display flights for the selected airline
            Console.WriteLine("=============================================");
            Console.WriteLine($"List of Flights for: {selectedAirline.Name}");
            Console.WriteLine("=============================================");
            Console.WriteLine($"{"Flight Number",-15}{"Airline Name",-25}{"Origin",-19}{"Destination",-20}{"Expected Departure/Arrival Time",-40}");

            foreach (var flight in terminal.Flights.Values)
            {
                if (flight.FlightNo.StartsWith(airlineCode))
                {
                    Console.WriteLine($"{flight.FlightNo,-15}{selectedAirline.Name,-25}{flight.Origin,-19}{flight.Destination,-20}{flight.ExpectedTime,-40}");
                }
            }
            Console.WriteLine(" "); // separation line
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

Flight GetSelectedFlight()
{
    while (true)
    {
        Console.Write("Choose an existing Flight to modify or delete: ");
        string flightNumber = Console.ReadLine().ToUpper();

        // Retrieve the Flight
        foreach (var flight in terminal.Flights.Values) //Iterates through all flights in terminal
        {
            if (flight.FlightNo == flightNumber) //Check if the flight number matches user input
            {
                return flight; //if yes, return the matching flight object
            }
        }

        Console.WriteLine("Invalid flight number. Please try again."); 
    }
}


void ModifyFlightDetails()
{
    while (true)
    {
        try
        {
            DisplayAirlineFlights(out Airline selectedAirline); //Display airlines and get the selectedAirline that the user inputs 

            Flight selectedFlight = GetSelectedFlight(); //get selectedFlight

            if (selectedFlight != null) //if valid flight is selected
            {
                while (true) //loop until user enters 1 (modify) or 2 (delete)
                {
                    Console.WriteLine("1. Modify Flight");
                    Console.WriteLine("2. Delete Flight");
                    Console.Write("Choose an option:");

                    string modifyorDelete = Console.ReadLine(); 

                    if (modifyorDelete == "1") //if user choose to Modify Flight
                    {
                        Console.WriteLine(" "); // separation line
                        Console.WriteLine("1. Modify Basic Information");
                        Console.WriteLine("2. Modify Status");
                        Console.WriteLine("3. Modify Special Request Code");
                        Console.WriteLine("4. Modify Boarding Gate");
                        Console.Write("Choose an option: ");
                        string opt = Console.ReadLine();

                        if (opt == "1")
                        {
                            while (true)
                            {
                                // New origin
                                Console.Write("Enter new Origin: ");
                                string newOrigin = Console.ReadLine();

                                if (string.IsNullOrEmpty(newOrigin)) //if user input above is null / empty
                                {
                                    Console.WriteLine("Invalid origin. Please try again.");
                                    continue;
                                }
                                selectedFlight.Origin = newOrigin;

                                // New destination
                                Console.Write("Enter new Destination: ");
                                string newDestination = Console.ReadLine();
                                if (string.IsNullOrEmpty(newDestination))
                                {
                                    Console.WriteLine("Invalid destination. Please try again");
                                    continue;
                                }
                                selectedFlight.Destination = newDestination;

                                // New arrival / departure time
                                Console.Write("Enter new Expected Departure / Arrival Time (dd/mm/yyyy hh:mm): ");
                                string newExpectedTimeInput = Console.ReadLine();

                                // if (newExpectedTime < DateTime.Now) -> error msg
                                try
                                {
                                    DateTime newexpectedTime = Convert.ToDateTime(newExpectedTimeInput); //convert string to DateTime object
                                    selectedFlight.ExpectedTime = newexpectedTime;
                                    Console.WriteLine("Flight updated!");
                                    break;
                                }

                                catch (FormatException)
                                {
                                    Console.WriteLine("Invalid Date/Time format. Please try again!");
                                    continue;
                                }
                            }
                        }
                        else if (opt == "2")
                        {
                            while (true)
                            {
                                Console.WriteLine("1. Delayed");
                                Console.WriteLine("2. Boarding");
                                Console.WriteLine("3. On Time");
                                Console.Write("Please select the new status of the flight (1/2/3): ");
                                string statusOption = Console.ReadLine();

                                if (statusOption == "1")
                                {
                                    selectedFlight.Status = "Delayed";
                                    break;
                                }
                                else if (statusOption == "2")
                                {
                                    selectedFlight.Status = "Boarding";
                                    break;
                                }
                                else if (statusOption == "3")
                                {
                                    selectedFlight.Status = "On Time";
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid option. Please try again.");
                                }
                            }

                            Console.WriteLine("Status updated!");
                        }
                        else if (opt == "3")
                        {
                            while (true)
                            {
                                Console.Write("Enter new Special Request Code (CFFT/LWTT/DDJB): ");
                                string newSpecialRequestCode = Console.ReadLine().ToUpper();
                                if (string.IsNullOrEmpty(newSpecialRequestCode) || !flightSpecialRequestCodes.ContainsValue(newSpecialRequestCode))
                                    //if user input is not CFFT/LWTTT/DDJB or is empty, print out error, else change the status
                                {
                                    Console.WriteLine("Invalid Special Request Code. Please try again.");
                                    continue;
                                }
                                //flightSpecialRequestCodes is a dictionary that maps flight no (key) to special request code (value). Using the flight no. to access the s.r.c and updating it
                                flightSpecialRequestCodes[selectedFlight.FlightNo] = newSpecialRequestCode;
                                Console.WriteLine("Special Request Code updated!");
                                break;
                            }
                        }
                        else if (opt == "4")
                        {
                            while (true)
                            {
                                Console.Write("Enter new Boarding Gate: ");
                                string newGate = Console.ReadLine().ToUpper();
                                if (string.IsNullOrEmpty(newGate) || !terminal.BoardingGates.ContainsKey(newGate))
                                    //if user input is not any of the gate in the dict BoardingGates, or empty user input
                                {
                                    Console.WriteLine("Invalid Boarding Gate. Please try again.");
                                    continue;
                                }

                                //Check if the gate the user input is already assigned to another flight
                                if (terminal.BoardingGates[newGate].Flight != null)
                                {
                                    Console.WriteLine("The boarding gate is already assigned to another flight!");
                                    continue;
                                }

                                // Remove flight from the current gate
                                foreach (var gate in terminal.BoardingGates.Values)
                                {
                                    if (gate.Flight == selectedFlight)
                                    {
                                        gate.Flight = null;
                                        break;
                                    }
                                }

                                // Assign the flight to the new gate
                                terminal.BoardingGates[newGate].Flight = selectedFlight;
                                Console.WriteLine("Boarding Gate updated!");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid option. Please try again.");
                            continue;
                        }

                        // Get boarding gate
                        //Iterates through all the boarding gates to find the gate that is assigned to selectedFlight. If found, assignedGate variable is updated with the name, else remains Unassigned
                        string assignedGate = "Unassigned";
                        foreach (var gate in terminal.BoardingGates.Values)
                        {
                            if (gate.Flight == selectedFlight)
                            {
                                assignedGate = gate.GateName;
                            }
                        }

                        // Display updated flight details
                        Console.WriteLine("=============================================");
                        Console.WriteLine("Updated Flight Details");
                        Console.WriteLine("=============================================");
                        Console.WriteLine($"Flight Number: {selectedFlight.FlightNo}");
                        Console.WriteLine($"Airline Name: {selectedAirline.Name}");
                        Console.WriteLine($"Origin: {selectedFlight.Origin}");
                        Console.WriteLine($"Destination: {selectedFlight.Destination}");
                        Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}");
                        Console.WriteLine($"Status: {selectedFlight.Status}");
                        Console.WriteLine($"Special Request Code: {flightSpecialRequestCodes[selectedFlight.FlightNo]}");
                        Console.WriteLine($"Boarding Gate: {assignedGate}");
                        Console.WriteLine(" "); // separation line
                        return; // Return to main menu
                    }
                    else if (modifyorDelete == "2")
                    {
                        while (true)
                        {
                            Console.Write($"Confirm deletion of flight {selectedFlight.FlightNo} (Y to confirm, N to reject): ");
                            string delOption = Console.ReadLine().ToUpper();

                            if (delOption == "Y")
                            {

                                //Remove the flight from boarding gate
                                foreach (var gate in terminal.BoardingGates.Values)
                                {
                                    if (gate.Flight == selectedFlight)
                                    {
                                        gate.Flight = null;
                                        break;
                                    }
                                }

                                // Remove the flight from the airline
                                Airline airline = terminal.GetAirlineFromFlight(selectedFlight);
                                if (airline != null && airline.RemoveFlight(selectedFlight))
                                {
                                    // Remove the flight from the terminal
                                    if (terminal.Flights.Remove(selectedFlight.FlightNo))
                                    {
                                        Console.WriteLine($"Flight {selectedFlight.FlightNo} has been deleted.");
                                        return; // Return to main menu
                                    }
                                }

                                else //in case of any errors
                                {
                                    Console.WriteLine($"Failed to remove flight {selectedFlight.FlightNo} from the airline.");
                                }

                                

                                
                            }
                            else if (delOption == "N")
                            {
                                Console.WriteLine("Deletion cancelled.");
                                return; // Return to main menu
                            }
                            else //if delOption not equals Y or N
                            {
                                Console.WriteLine("Invalid option. Please try again.");
                            }
                        }
                    }
                    else //if modifyorDelete not equals 1 or 2
                    {
                        Console.WriteLine("Invalid option. Please try again.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid flight number. Please try again.");
                continue;
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

void DisplayFlightSchedule()
{
    try
    {
        //header
        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine($"{"Flight Number",-20}{"Airline Name",-20}{"Origin",-20}{"Destination",-25}{"Expected Departure / Arrival Time",-32}{"Status",-15}{"Boarding Gate",-15}");

        //IComparable
        var sortedFlights = terminal.Flights.Values.OrderBy(f => f);

        foreach (var flight in sortedFlights)
        {
            Airline airline = terminal.GetAirlineFromFlight(flight); //Get airline from flight
            string airlineName = airline.Name; //Get airline name or "Unknown" if null (in case of error)

            // Get boarding gate
            string assignedGate = "Unassigned"; //Default value for unassigned gate
            foreach (var gate in terminal.BoardingGates.Values) //loops through every boardinggate number
            {
                if (gate.Flight == flight) //if the gate is assigned to the current flight
                {
                    assignedGate = gate.GateName; //update assignedGate with the gate name that is in the current foreach loop iteration
                    break; //exit loop once the gate is found
                }
            }

            // Check for empty status and replace with "N.A."
            string status = string.IsNullOrEmpty(flight.Status) ? "N.A." : flight.Status; //if flight.status is null/empty, status = N.A., else if true, status is flight.status
           

            Console.WriteLine($"{flight.FlightNo,-20}{airlineName,-20}{flight.Origin,-20}{flight.Destination,-25}{flight.ExpectedTime,-32}{status,-15}{assignedGate,-15}");
        }
        Console.WriteLine(" "); // separation line
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}

void DisplayTotalFeePerAirlineForTheDay()
{
    // Check if all flights have been assigned boarding gates
    bool allFlightsAssigned = true;
    foreach (var flight in terminal.Flights.Values)
    {
        bool assigned = false;
        foreach (var gate in terminal.BoardingGates.Values)
        {
            if (gate.Flight == flight)
            {
                assigned = true;
                break;
            }
        }
        if (!assigned)
        {
            allFlightsAssigned = false;
            Console.WriteLine($"Flight {flight.FlightNo} has not been assigned a boarding gate.");
        }
    }

    if (!allFlightsAssigned)
    {
        Console.WriteLine("Please ensure that all unassigned flights have their boarding gates assigned before running this feature again.");
        return;
    }

    Console.WriteLine("All flights have been assigned boarding gates.");

    // Variables to store the overall totals
    double overallSubtotal = 0;
    double overallDiscount = 0;
    double overallFinalFees = 0;

    // Calculate and display the total fee per airline
    Console.WriteLine("=============================================");
    Console.WriteLine("Total Fee Per Airline for the Day");
    Console.WriteLine("=============================================");
    foreach (var airline in terminal.Airlines.Values)
    {
        var (subtotal, discount) = airline.CalculateFees(flightSpecialRequestCodes);
        double finalFees = subtotal - discount;

        // Update the overall totals
        overallSubtotal += subtotal;
        overallDiscount += discount;
        overallFinalFees += finalFees;

        // Display the breakdown for each airline
        Console.WriteLine($"Airline: {airline.Name}");
        Console.WriteLine($"Subtotal: {subtotal:C}");
        Console.WriteLine($"Discount: {discount:C}");
        Console.WriteLine($"Total Fee: {finalFees:C}");
        Console.WriteLine("---------------------------------------------");
    }

    // Calculate the percentage of the subtotal discounts over the final total of fees
    double discountPercentage = (overallDiscount / overallFinalFees) * 100;

    // Display the overall totals
    Console.WriteLine("=============================================");
    Console.WriteLine("Overall Totals for All Airlines");
    Console.WriteLine("=============================================");
    Console.WriteLine($"Overall Subtotal Charges: {overallSubtotal:C}");
    Console.WriteLine($"Overall Discount Deducted: {overallDiscount:C}");
    Console.WriteLine($"Overall Final Fees Terminal 5 collects: {overallFinalFees:C}");
    Console.WriteLine($"Discount Percentage (discounts over final total fees): {discountPercentage:F2}%");
    Console.WriteLine("=============================================");
    Console.WriteLine(" "); // separation line
}

void ProcessUnassignedFlightsInBulk()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("Processing Unassigned Flights in Bulk");
    Console.WriteLine("=============================================");

    // Create a queue for unassigned flights
    Queue<Flight> unassignedFlights = new Queue<Flight>();
    int totalUnassignedFlights = 0;
    int totalUnassignedGates = 0;
    int totalProcessed = 0;
    int totalPreviouslyAssigned = 0;

    // Check for unassigned flights and add them to queue
    foreach (var flight in terminal.Flights.Values)
    {
        bool isAssigned = false;
        foreach (var gate in terminal.BoardingGates.Values)
        {
            if (gate.Flight == flight) //Checks all gates to see if the flight is assigned to any of them
            {
                isAssigned = true;
                totalPreviouslyAssigned++; //adds to the totalPreviouslyAssigned count
                break;
            }
        }

        if (!isAssigned)
        {
            unassignedFlights.Enqueue(flight); //if flight is not assigned to any gate, add to the queue, incease totalUnassignedFlights count
            totalUnassignedFlights++;
        }
    }

    // Count unassigned gates
    foreach (var gate in terminal.BoardingGates.Values) //checks all gates to see if they are unassigned
    {
        if (gate.Flight == null) //if gate is unassigned, increase totalUnassignedGates count
        {
            totalUnassignedGates++;
        }
    }

    Console.WriteLine($"Total Unassigned Flights: {totalUnassignedFlights}");
    Console.WriteLine($"Total Available Gates: {totalUnassignedGates}");
    Console.WriteLine("---------------------------------------------");

    // Process each flight in the queue
    while (unassignedFlights.Count > 0)   //while there are still flights in the queue
    {
        Flight currentFlight = unassignedFlights.Dequeue(); //remove the flight from the queue
        string specialRequestCode = flightSpecialRequestCodes.ContainsKey(currentFlight.FlightNo) //if the flight has a special request code, store it in specialRequestCode, else store N.A.
            ? flightSpecialRequestCodes[currentFlight.FlightNo]
            : "N.A";

        // Find appropriate gate based on special request code
        BoardingGate assignedGate = null; 
        foreach (var gate in terminal.BoardingGates.Values)
        {
            if (gate.Flight != null) continue;

            bool isCompatible = specialRequestCode switch  //checks if the gate is compatible with the special request code
            {
                "DDJB" => gate.SupportsDDJB,
                "CFFT" => gate.SupportsCFFT,
                "LWTT" => gate.SupportsLWTT,
                _ => true // For flights with no special requests
            };

            if (isCompatible)
            {
                assignedGate = gate;
                break;
            }
        }

        // Assign gate if found
        if (assignedGate != null)
        {
            assignedGate.Flight = currentFlight;
            totalProcessed++;

            // Display flight details
            Airline airline = terminal.GetAirlineFromFlight(currentFlight); 
            string airlineName = airline != null ? airline.Name : "Unknown"; //if airline is not null, store the airline name in airlineName, else store Unknown

            Console.WriteLine($"Flight Details:");
            Console.WriteLine($"Flight Number: {currentFlight.FlightNo}");
            Console.WriteLine($"Airline Name: {airlineName}");
            Console.WriteLine($"Origin: {currentFlight.Origin}");
            Console.WriteLine($"Destination: {currentFlight.Destination}");
            Console.WriteLine($"Expected Time: {currentFlight.ExpectedTime}");
            Console.WriteLine($"Special Request Code: {specialRequestCode}");
            Console.WriteLine($"Assigned Gate: {assignedGate.GateName}");
            Console.WriteLine("---------------------------------------------");
        }
        else
        {
            Console.WriteLine($"No compatible gate available for flight {currentFlight.FlightNo}");
        }
    }

    // Calculate and display statistics
    double processedPercentage = (totalProcessed / (double)(totalProcessed + totalPreviouslyAssigned)) * 100;

    Console.WriteLine("\nProcessing Summary:");
    Console.WriteLine($"Total Flights Processed and Assigned: {totalProcessed}");
    Console.WriteLine($"Previously Assigned Flights: {totalPreviouslyAssigned}");
    Console.WriteLine($"Percentage of Flights Processed Automatically: {processedPercentage:F2}%");
    Console.WriteLine("======================================================");
}