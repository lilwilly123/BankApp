using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankApp
{
    public class Admin
    {
        public string AdminID { get; set; } // Unique identifier for the admin
        public string Name { get; set; } // Admin's full name

        public enum UserRole { Customer, Admin, SystemOwner }
        public enum UserStatus { Active, Locked }

        // List of all the users
        public static List<Dictionary<string, object>> UsersList = new List<Dictionary<string, object>>();

        // -------------------------------------------------------
        // Register new user. Just move this to customer or make a new one
        // -------------------------------------------------------
        public void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("Register a new user");

            Console.Write("Input username: ");
            string username = Console.ReadLine();

            //Looks for the user in the list
            if (FindUser(username) != null)
            {
                Console.WriteLine("Username already exist.");
                return;
            }

            Console.Write("Input password: ");
            string password = Console.ReadLine();

            Console.Write("Choose role (Customer/Admin or C/A): ");
            string input = Console.ReadLine().Trim().ToLower();

            //Assigns the user a role
            UserRole role;
            switch (input)
            {
                case "customer":
                case "c":
                    role = UserRole.Customer;
                    break;
                case "admin":
                case "a":
                    role = UserRole.Admin;
                    break;
                default:
                    Console.WriteLine("Invalid role. choose C or A.");
                    return;
            }

            //If the role is Customer, create a real Customer object
            object userObject = null;

            if (role == UserRole.Customer)
            {
                userObject = new Customer(); //Customer will hold accounts, loans, etc.
            }

            var newUser = new Dictionary<string, object>()
            {
                {"Username", username},
                {"Password", password},
                {"Role", role},
                {"Status", UserStatus.Active},
                {"FailedAttempts", 0},
                {"UserObject", userObject} //Store the object (Customer or null)
            };

            UsersList.Add(newUser);
            Console.WriteLine($"User '{username}' Created as {role}.");
        }


        // -------------------------------------------------------
        // Login
        // -------------------------------------------------------
        public Dictionary<string, object> LoginUser()
        {
            Console.Clear();
            Console.WriteLine("Login");

            Console.Write("Username: ");
            string username = Console.ReadLine();

            var user = FindUser(username);

            if (user == null)
            {
                Console.WriteLine("Username does not exist.");
                return null;
            }

            if ((UserStatus)user["Status"] == UserStatus.Locked)
            {
                Console.WriteLine("Account locked due to too many failed attempts.");
                return null;
            }

            while (true)
            {
                Console.Write("Password: ");
                string password = Console.ReadLine();

                if ((string)user["Password"] == password)
                {
                    Console.WriteLine($"Login successful! Welcome {user["Username"]} ({user["Role"]}).");
                    user["FailedAttempts"] = 0;
                    return user; // Return the logged-in user
                }
                else
                {
                    int attempts = (int)user["FailedAttempts"] + 1;
                    user["FailedAttempts"] = attempts;
                    Console.WriteLine($"Wrong password ({attempts}/3).");

                    if (attempts >= 3)
                    {
                        user["Status"] = UserStatus.Locked;
                        Console.WriteLine("Account is now locked!");
                        return null;
                    }
                }
            }
        }
        // -------------------------------------------------------
        // Shows all the users in the list
        // -------------------------------------------------------
        public void ShowAllUsers()
        {
            Console.Clear();
            Console.WriteLine("Registerd users:");

            if (UsersList.Count == 0)
            {
                Console.WriteLine("(No registerd users yet.)");
                return;
            }

            foreach (var user in UsersList)
            {
                string username = (string)user["Username"];
                string role = user["Role"].ToString();
                string status = user["Status"].ToString();
                Console.WriteLine($" - {username} ({role}) [{status}]");
            }
        }

        // -------------------------------------------------------
        // Finds the user in the list
        // -------------------------------------------------------
        private Dictionary<string, object> FindUser(string username)
        {
            foreach (var user in UsersList)
            {
                if (string.Equals((string)user["Username"], username, StringComparison.OrdinalIgnoreCase))
                    return user;
            }
            return null;
        }
        // -------------------------------------------------------
        // Change Currency Rates
        // -------------------------------------------------------
        public void ChangeCurrencyRates() // Update exchange rates
        {
            Console.WriteLine("Change currency rate");

            foreach (var rate in CurrencyRate.rates)
            {
                Console.WriteLine($"{rate.Key.From} -> {rate.Key.To}: {rate.Value}");
            }

            Console.WriteLine("Please select currency rate (eg. USD/SEK/EUR), to exchange from: ");
            string fromCurrency = Console.ReadLine();

            Console.WriteLine("Please select currency rate (eg. USD/SEK/EUR), to exchange to: ");
            string toCurrency = Console.ReadLine();

            Console.WriteLine("Please write in decimal form, the new rate: ");
            decimal newRate = decimal.Parse(Console.ReadLine());

            // Convert input strings to CurrencyType enums — required since rates use enums, not strings
            var from = Enum.Parse<CurrencyType>(fromCurrency, true);
            var to = Enum.Parse<CurrencyType>(toCurrency, true);

            // Create a tuple key (From, To) to check if the rate exists or to add a new one
            var ratePair = (from, to);

            if (CurrencyRate.rates.ContainsKey(ratePair))
            {
                CurrencyRate.rates[ratePair] = newRate;
                Console.WriteLine("Rate updated successfully.");
            }
            else
            {
                Console.WriteLine("Rate pair not found, do you want to add it? (y/n): ");
                string answer = Console.ReadLine();
                if (answer == "y")
                {
                    CurrencyRate.rates.Add(ratePair, newRate);
                    Console.WriteLine("Rate pair added along with a exchange rate.");
                }
                else
                {
                    Console.WriteLine("Rate pair not added. Exiting.");
                    return;
                }
            }

            foreach (var rate in CurrencyRate.rates)
            {
                Console.WriteLine($"{rate.Key.From} -> {rate.Key.To}: {rate.Value}");
            }

        }
        internal static List<Customer> GetAllCustomers()
        {
            return UsersList
                .Where(u => u["Role"].ToString() == "Customer")
                .Select(u => (Customer)u["UserObject"])
                .ToList();
        }

    }
}
