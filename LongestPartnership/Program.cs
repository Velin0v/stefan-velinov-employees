using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using LongestPartnership.Models;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace LongestPartnership
{
    public class Program
    {
        private static readonly string defaultFileLocation = "..\\..\\Sources\\TextFiles\\employees.txt";
        
        //TO DO: fix the bussiness logic
        [STAThread]
        static void Main(string[] args)
        {
            var dataLines = ReadTexFile(null);

            if (dataLines != null && dataLines.Count > 0)
            {
                var employees = MapEmployeesData(dataLines);

                GetTopTeam(employees);
                var topPairs = GetTopTeam(employees);
                PrintReport(topPairs);

                Console.WriteLine("Do you want load another file? Y/N");
                var userInput = Console.ReadLine();
                var openNewFile = HandleUserInput(userInput);
            }
        }

        public static List<string> ReadTexFile(string fileLocation)
        {
            var location = !String.IsNullOrWhiteSpace(fileLocation) ? fileLocation : defaultFileLocation;
            var lines = new List<string>();

            //TO DO: add try catch statement
            using (StreamReader sr = File.OpenText(location))
            {
                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    lines.Add(s);
                }

                sr.Dispose();
            }

            return lines;
        }

        public static List<Employee> MapEmployeesData(List<string> dataLines)
        {
            var employees = new List<Employee>();
            
            foreach (var line in dataLines)
            {
                string[] data = Regex.Split(line, @", ");
                DateTime fromDate;
                DateTime toDate;
                
                //TO DO: add tryParse for ints
                if (DateTime.TryParse(data[2], out fromDate))
                {
                }

                if (DateTime.TryParse(data[3], out toDate))
                {
                }

                else
                {
                    toDate = DateTime.Today;
                }

                employees.Add(new Employee()
                {
                    EmployeeId = int.Parse(data[0]),
                    ProjectId = int.Parse(data[1]),
                    FromDate = fromDate,
                    ToDate = toDate
                });

            }

            return employees;
        }

        public static bool HandleUserInput(string input)
        {
            var openNewFile = false;

            switch (input.ToLower())
            {
                case "y":
                    GetReport();
                    break;
                case "n":
                    Environment.Exit(0);
                    break;
                default :
                    Console.WriteLine("Do you want load another file? Y/N");
                    var userInput = Console.ReadLine();
                    HandleUserInput(input);
                    break;
            }

            return openNewFile;
        }
        
        public static string GetFileLocation()
        {
            string fileName;
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            fileName = fd.FileName;

            return fileName;
        }

        public static Dictionary<int, List<Employee>> GetTopTeam(List<Employee> employees)
        {
            var employeesInProject = employees.GroupBy(x => x.ProjectId).ToList();
            var topPairs = new Dictionary<int, List<Employee>>();

            foreach (var project in employeesInProject)
            {
                var orderedEployees = project.OrderByDescending(e => ((e.ToDate - e.FromDate).Value.TotalDays)).Take(2).ToList();
                
                if (orderedEployees.Count == 2)
                {
                    if (!topPairs.ContainsKey(project.Select(x => x.ProjectId).FirstOrDefault()))
                    {
                        topPairs.Add(project.Select(x => x.ProjectId).FirstOrDefault(), orderedEployees);
                    }
                }
            }
            
            return topPairs;
        }

        public static void GetReport()
        {
            var newLocation = GetFileLocation();
            var dataLines = ReadTexFile(newLocation);

            var employees = MapEmployeesData(dataLines);
            var topPairs = GetTopTeam(employees);
            PrintReport(topPairs);

            Console.WriteLine("Do you want load another file? Y / N");
            var userInput = Console.ReadLine();

            HandleUserInput(userInput);
        }

        public static void PrintReport(Dictionary<int, List<Employee>> topPairs)
        {
            StringBuilder sb = new StringBuilder("Report:");

            foreach (var project in topPairs)
            {
                sb.AppendLine();
                sb.AppendFormat("Project: {0}; ", project.Key);

                foreach (var val in project.Value)
                {
                    var i = 1;
                    sb.AppendFormat("EmployeeId {0}: {1}; ",i , val.EmployeeId);
                    i++;
                }
            }

            Console.WriteLine(sb);
        }
    }
}
