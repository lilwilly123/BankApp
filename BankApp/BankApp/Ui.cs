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
            new SystemOwner().ProcessPendingTransactions(GetAllCustomers());

            Customer cust = (Customer)user["UserObject"];
            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"Welcome {user["Username"]} (Customer)");
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Deposit Funds");
                Console.WriteLine("3. Withdraw Funds");
                Console.WriteLine("4. List Accounts");
                Console.WriteLine("5. Show transactions");
                Console.WriteLine("6. Create Loan");
                Console.WriteLine("7. Transfer");
                Console.WriteLine("8. Logout");
                Console.Write("Choose: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        //Own accounts
                        cust.CreateAccount();
                        break;
                    case "2":
                        cust.DepositFunds();
                        break;
                    case "3":
                        cust.WithdrawFunds();
                        break;
                    case "4":
                        Console.Clear();
                        cust.ListAccounts();
                        break;
                    case "5":
                        Console.Clear();
                        cust.TransactionHistory();
                        break;
                    case "6":
                        Console.Write("Loan Id: ");
                        string loanId = Console.ReadLine();
                        Console.Write("Principal amount: ");
                        decimal principalAmount = Convert.ToDecimal(Console.ReadLine());
                        Console.Write("Interest rate(Perscent): ");
                        float interestRate = float.Parse(Console.ReadLine());

                        DateTime StartDate = DateTime.Now;

                        Console.Write("Due date(yyyy-mm-dd) ");
                        DateTime DueDate = DateTime.Parse(Console.ReadLine());

                        Loan newloan = new Loan(principalAmount, interestRate, StartDate, DueDate);
                        Console.WriteLine($"Loan created! Outstanding amount: {newloan.OutstandingAmount}");
                        break;
                    //-----------------
                    //Transfer balance
                    //-----------------
                    case "7":
                        Console.WriteLine("Transfer Options:");
                        Console.WriteLine("1. Transfer between your own accounts");
                        Console.WriteLine("2. Transfer to another customer");
                        Console.Write("Choose: ");

                        string transferChoice = Console.ReadLine();

                        switch (transferChoice)
                        {
                            //---------------------
                            //Transfer own accounts
                            //---------------------
                            case "1":
                                cust.TransferBetweenOwnAccounts();
                                break;
                            //-----------------
                            //Transfer to user
                            //-----------------
                            case "2":
                                Console.Write("Target customer's username: ");
                                string targetUsername = Console.ReadLine();

                                var targetUser = Admin.UsersList.Find(u =>
                                    (string)u["Username"] == targetUsername);

                                if (targetUser == null || targetUser["Role"].ToString() != "Customer")
                                {
                                    Console.WriteLine("Customer not found.");
                                    break;
                                }

                                Customer targetCustomer = (Customer)targetUser["UserObject"];
                                cust.TransferToOtherCustomer(targetCustomer);  
                                break;

                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                        break;
                    case "8":
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
            SystemOwner owner = new SystemOwner();

            bool loggedIn = true;

            while (loggedIn)
            {
                Console.Clear();
                Console.WriteLine($"Welcome {user["Username"]} (Admin)");
                Console.WriteLine("1. Register New User");
                Console.WriteLine("2. Show All Users");
                Console.WriteLine("3. Change Currency Rates");
                Console.WriteLine("4. Process Pending Transactions");
                Console.WriteLine("5. Pending Trasaction Log");
                Console.WriteLine("6. Logout");
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
                        //Run pending transactions immediately
                        Console.WriteLine("\nProcessing pending transactions...");
                        owner.ProcessPendingTransactions(Admin.GetAllCustomers());
                        Console.WriteLine("All eligible transactions processed!");
                        break;
                    case "5":
                        Console.WriteLine("=== Pending Transactions ===");
                        if (SystemOwner.PendingTransactions.Count == 0)
                            Console.WriteLine("(none)");
                        else
                            foreach (var t in SystemOwner.PendingTransactions) t.PrintTransaction();
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
        // Menu SystemOwner
        // ----------------------------------------------------------
    }
}
