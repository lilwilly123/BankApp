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
                    Console.WriteLine("Invalid role. choose C, A or S.");
                    return;
            }

            var newUser = new Dictionary<string, object>()
        {
            {"Username", username},
            {"Password", password},
            {"Role", role},
            {"Status", UserStatus.Active},
            {"FailedAttempts", 0}
        };

            UsersList.Add(newUser);
            Console.WriteLine($" User '{username}' Created as {role}.");
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

        public void ChangeCurrencyRates() // Update exchange rates
        {

        }

    }
}
