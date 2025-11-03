using System;
using System.Collections.Generic;


public class UserManager
{
    public enum UserRole { Customer, Admin, BankOwner }
    public enum UserStatus { Active, Locked }

    // List of all the users
    private List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();

    // runs the program, we can move this later just did it for testing
    public void Run()
    {
        bool running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("1. Register user");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Show all the users");
            Console.WriteLine("4. Exit");
            Console.Write("Choice: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    LoginUser();
                    break;
                case "3":
                    ShowAllUsers();
                    break;
                case "4":
                    running = false;
                    Console.WriteLine("Bye!");
                    break;
                default:
                    Console.WriteLine("Invalid answer!");
                    break;
            }

            if (running)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }

    // -------------------------------------------------------
    // Register new user. Just move this to customer or make a new one
    // -------------------------------------------------------
    private void RegisterUser()
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

        Console.Write("Choose role (Customer/Admin/BankOwner or C/A/B): ");
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
            case "bankowner":
            case "b":
                role = UserRole.BankOwner;
                break;
            default:
                Console.WriteLine("Invalid role. choose C, A or B.");
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

        users.Add(newUser);
        Console.WriteLine($" User '{username}' Created as {role}.");
    }

    // -------------------------------------------------------
    // Login
    // -------------------------------------------------------
    private void LoginUser()
    {
        Console.Clear();
        Console.WriteLine("Login");

        Console.Write("Username: ");
        string username = Console.ReadLine();

        var user = FindUser(username);

        if (user == null)
        {
            Console.WriteLine("Username does not exist.");
            return;
        }

        if ((UserStatus)user["Status"] == UserStatus.Locked)
        {
            Console.WriteLine("Account locked due to to many faild attempts.");
            return;
        }

        //Runs login as long as the pssword is wrong
        while (true)
        {
            Console.Write("Password: ");
            string password = Console.ReadLine();

            if ((string)user["Password"] == password)
            {
                Console.WriteLine($"Login success {user["Username"]} ({user["Role"]}).");
                user["FailedAttempts"] = 0; // sets FailedAttempts back to 0 
                return;
            }
            else
            {
                int attempts = (int)user["FailedAttempts"] + 1;
                user["FailedAttempts"] = attempts;

                Console.WriteLine($"Wrong password ({attempts}/3).");

                if (attempts >= 3)
                {
                    user["Status"] = UserStatus.Locked;
                    Console.WriteLine("Account is locked!");
                    return;
                }
            }
        }
    }

    // -------------------------------------------------------
    // Shows all the users in the list
    // -------------------------------------------------------
    private void ShowAllUsers()
    {
        Console.Clear();
        Console.WriteLine("Registerd users:");

        if (users.Count == 0)
        {
            Console.WriteLine("(No registerd users yet.)");
            return;
        }

        foreach (var user in users)
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
        foreach (var user in users)
        {
            if (string.Equals((string)user["Username"], username, StringComparison.OrdinalIgnoreCase))
                return user;
        }
        return null;
    }
}

