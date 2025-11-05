using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp
{
    internal class Ui : Admin
    {
       

        // runs the program, we can move this later just did it for testing
        public void Run()
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("1. Register user");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterUser();
                        break;
                    case "2":
                        var user = LoginUser();
                        if (user != null)
                            HandleLoggedInUser(user);
                        break;
                    case "3":
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

        // ----------------------------------------------------------
        // Handels logged in users
        // ----------------------------------------------------------
        static void HandleLoggedInUser(Dictionary<string, object> user)
        {
            string role = user["Role"].ToString();

            switch (role)
            {
                case "Customer":
                    ShowCustomerMenu(user);
                    break;
                case "Admin":
                    ShowAdminMenu(user);
                    break;
                case "SystemOwner":
                    ShowSystemOwnerMenu(user);
                    break;
                default:
                    Console.WriteLine("Unknown role!");
                    break;
            }
        }

        // ----------------------------------------------------------
        // Menu Customer
        // ----------------------------------------------------------
        static void ShowCustomerMenu(Dictionary<string, object> user)
        {
            Customer cust = new Customer();
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"Welcome {user["Username"]} (Customer)");
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Deposit Funds");
                Console.WriteLine("3. Withdraw Funds");
                Console.WriteLine("4. List Accounts");
                Console.WriteLine("5.Create Loan");
                Console.WriteLine("6. Logout");
                Console.Write("Choose: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        //Own accounts
                        cust.CreateAccount();
                        break;
                    case "2":
                        Console.Write("Account number: ");
                        string accNum = Console.ReadLine();
                        Console.Write("Amount: ");
                        decimal dep = Convert.ToDecimal(Console.ReadLine());
                        cust.DepositFunds(accNum, dep);
                        break;
                    case "3":
                        Console.Write("Account number: ");
                        string accNum2 = Console.ReadLine();
                        Console.Write("Amount: ");
                        decimal wd = Convert.ToDecimal(Console.ReadLine());
                        cust.WithdrawFunds(accNum2, wd);
                        break;
                    case "4":
                        cust.ListAccounts();
                        break;
                    case "5":
                        Console.Write("Loan Id: ");
                        string loanId = Console.ReadLine();
                        Console.Write("Principal amount: ");
                        decimal principalAmount = Convert.ToDecimal(Console.ReadLine());
                        Console.Write("Interest rate(Perscent): ");
                        float interestRate = float.Parse(Console.ReadLine());

                        DateTime StartDate = DateTime.Now;

                        Console.Write("Due date(yyyy-mm-dd) ");
                        DateTime DueDate = DateTime.Parse(Console.ReadLine());

                        Loan newloan = new Loan(loanId, user["Username"].ToString(), principalAmount, interestRate, StartDate, DueDate);
                        Console.WriteLine($"Loan created! Outstanding amount: {newloan.OutstandingAmount}");
                        break;
                    case "6":
                        loggedIn = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (loggedIn)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        // ----------------------------------------------------------
        // Menu Admin
        // ----------------------------------------------------------
        static void ShowAdminMenu(Dictionary<string, object> user)
        {
            Admin admin = new Admin();
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"Welcome {user["Username"]} (Admin)");
                Console.WriteLine("1. Register New User");
                Console.WriteLine("2. Show All Users");
                Console.WriteLine("3. Change Currency Rates");
                Console.WriteLine("4. Logout");
                Console.Write("Choose: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        admin.RegisterUser();
                        break;
                    case "2":
                        admin.ShowAllUsers();
                        break;
                    case "3":
                        admin.ChangeCurrencyRates();
                        break;
                    case "4":
                        loggedIn = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (loggedIn)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        // ----------------------------------------------------------
        // Menu SystemOwner
        // ----------------------------------------------------------
        static void ShowSystemOwnerMenu(Dictionary<string, object> user)
        {
            SystemOwner systemowner = new SystemOwner();
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"Welcome {user["Username"]} (System Owner)");
                Console.WriteLine("1. Set Max Loan Limit");
                Console.WriteLine("2. Restrict Failed Logins");
                Console.WriteLine("3. Logout");
                Console.Write("Choose: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        systemowner.SetLoanPolicy(5);
                        break;
                    case "2":
                        
                        break;
                    case "3":
                        loggedIn = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }

                if (loggedIn)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }
    }
}
