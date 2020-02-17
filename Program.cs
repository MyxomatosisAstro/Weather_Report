using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;


// Namespace

namespace Weather_Report
{

    // Main class 
    public class Program
    {
        // Entry Point Method
        public static void Main(string[] args)
        {
            // START HERE //

            GetAppInfo(); // Run GetAppInf function to get info

            GreetUser(); // Run GreetUser function to initialize program

            // Available weather report requests
            int[] availableRequests = { 1, 2, 3 };

            // Init request
            int request = 0;

            // Ask user what question they would like answered 
            Console.WriteLine("I have three different reports from the SMHI weather network available for you. What would you like to know? " +
                                "Please choose a number between 1-3, where 1 is the avarage temperature in Sweden for the last hour,  " +
                                "2 is the total amount of rainfall in Lund for the last months " +
                                "and 3 is the temperature for each of the institute’s weatherstations.");

            while (availableRequests.Contains(request) == false)
            {
                // Get user input
                string input = Console.ReadLine();

                // Make sure it's a number
                if (!int.TryParse(input, out request))
                {
                    // Print error message
                    PrintColorMessage(ConsoleColor.Red, "Please use an actual number");

                    // Keep going 
                    continue;
                }

                // Cast to int and put in request
                request = Int32.Parse(input);

                // Match request to available reports
                if (availableRequests.Contains(request) == false)
                {
                    // Print error message
                    PrintColorMessage(ConsoleColor.Red, "No report available. Please choose between reports 1, 2 and 3");
                }

                // Logic for presenting report 1, the avarage temp in Sweden for the last hour
                if (request == 1)
                {
                    double averageTemp = AvarageTempFinder();
                    Console.WriteLine("The average temperature in Sweden for the last hour is " + averageTemp + "°C");
                }
                
                // Logic for presenting report 2, the total rainfall in Lund for the last months
                if (request == 2)
                {
                    double totalPrecipitation = TotalPrecipitation();
                    Console.WriteLine("The total precipitation in Lund over the last four months is" + totalPrecipitation + " mm");
                }

                // Logic for presenting report 3, the temperature for each of the institute’s weatherstations
                if (request == 3)
                {
                    CurrentTemperature();
                }

                // Ask if user wants another report
                Console.WriteLine("Do you need another report? [Y or N]");

                string answer = Console.ReadLine().ToUpper();

                if (answer == "Y")
                {
                    request = 0;
                    Console.WriteLine("Enter a new request");
                    continue;
                }
                else if (answer == "N")
                {
                    return;
                }
                else
                {
                    return;
                }
            }
        }

        // Logic for finding the avarage temp in Sweden the last hour
        private static double AvarageTempFinder()
        {
            // The string here is our endpoint
            var data = ApiService.GetDataAsync("version/latest/parameter/1/station-set/all/period/latest-hour/data.json");

            // Converts our Json string to an object
            Stations stations = JsonConvert.DeserializeObject<Stations>(data.Result);

            List<double> temperatures = new List<double>();

            // Formatting rules for the data, allowing negative values
            var fmt = new NumberFormatInfo();
            fmt.NegativeSign = "-";

            // Iterates through the different stations and adds the temperatures to the "temperatures" list
            foreach (var station in stations.station)
            {
                if (station.value != null)
                {
                    var temperatureStr = station.value.FirstOrDefault().value;
                    
                    var temperature = double.Parse(temperatureStr, fmt);

                    temperatures.Add(temperature);
                }
            }
            // Calculates and returns the avarage temperature

            var averageTemperature = temperatures.Sum() / temperatures.Count;

            var roundedTemperature = Math.Round(averageTemperature, 1);

            return roundedTemperature;
        }

        // Logic for finding the total precipitation over the last months in Lund
        private static double TotalPrecipitation()
        {
            // The string here is our endpoint
            var data = ApiService.GetDataAsync("version/latest/parameter/23/station/53430/period/latest-months/data.json");

            // Converts our Json string to an object
            TempValue precipitation = JsonConvert.DeserializeObject<TempValue>(data.Result);

            double totalPrecipitation = 0;

            foreach (var value in precipitation.value)
            {
                var precipitationStr = precipitation.value;
                var precipitationParsed = double.Parse(precipitationStr);
                totalPrecipitation += precipitationParsed;
            }

            return totalPrecipitation;
        }

        // Logic for finding the most recent temperature measurements for each station
        public static async Task CurrentTemperature()
        {
            // Fetches the data for all stations
            var data = ApiService.GetDataAsync("version/latest/parameter/1.json");

            // Converts our Json string to an object
            Stations stations = JsonConvert.DeserializeObject<Stations>(data.Result);
            
            // Uses a cancellation token to allow cancellation of task
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var keyBoardTask = Task.Run(() =>
                {
                    Console.WriteLine("Press any key to cancel");
                    Console.ReadKey();

                    cancellationTokenSource.Cancel();
                });

                try
                {
                    var getTempsTask = GetTemp(stations, cancellationTokenSource.Token);
                }

                catch (TaskCanceledException)
                {
                    Console.WriteLine("Task was cancelled");
                }

                await keyBoardTask;
            }
        }
        // Returns current temperature for each station if available and unless cancelled by token
        private static Task GetTemp(Stations stations, CancellationToken cancellationToken)
        {
            Task task = null;

            task = Task.Run(() =>
           {
               foreach (var item in stations.station)
               {
                   if (cancellationToken.IsCancellationRequested)
                   {
                       return;
                   }

                   string result = "";

                   var stationsStr = ApiService.GetDataAsync("version/latest/parameter/1/station/" + item.key + ".json");
                   
                   StationCurrentTemp station = JsonConvert.DeserializeObject<StationCurrentTemp>(stationsStr.Result);

                   Period latest = station.period.ToList().Find(x => x.key == "latest-hour");

                   if (latest != null)
                   {
                       var tempStr = ApiService.GetDataAsync("version/latest/parameter/1/station/" + item.key + "/period/latest-hour/data.json");
                       StationTemp temperature = JsonConvert.DeserializeObject<StationTemp>(tempStr.Result);

                       if (temperature.value != null)
                       {
                           result = temperature.station.name + " " + temperature.value[0].value + "°C";
                           Console.WriteLine(result);
                       }

                       else
                       {
                           Console.WriteLine(temperature.station.name + " does not have current temperature readings");
                       }
                   }
                   Thread.Sleep(100);

               }
           });
            return task;
        }


        // Generic app info
        static void GetAppInfo()
        {
            // Set app vars
            string appName = "Weather Report";
            string appVersion = "1.0.0";
            string appAuthor = "Elias Wahlstedt";

            // Change text color
            Console.ForegroundColor = ConsoleColor.Green;

            // Write out app info
            Console.WriteLine("{0}: Version {1} by {2}", appName, appVersion, appAuthor);

            // Reset text color
            Console.ResetColor();
        }

        // Method for greeting the User
        static void GreetUser()
        {
            // Ask users name
            Console.WriteLine("What is your name?");

            // Get user input
            string inputName = Console.ReadLine();

            Console.WriteLine("Hello {0}, welcome to my simple Weather Report program. ", inputName);
        }

        // Generic color-coded response message (for invalid-inputs etc.)
        static void PrintColorMessage(ConsoleColor color, string message)
        {
            // Change text color
            Console.ForegroundColor = color;

            // Tell user its not a number
            Console.WriteLine(message);

            // Reset text color
            Console.ResetColor();
        }
    }
}
