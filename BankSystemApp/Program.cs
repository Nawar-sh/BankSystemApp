
using System;
using System.Collections.Generic;

namespace BankSystemApp
{
    internal class Program
    {
        private static CustomerRepository _customerRepo = new CustomerRepository();
        private static AccountRepository _accountRepo = new AccountRepository();
        private static TransactionRepository _transactionRepo = new TransactionRepository();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.WriteLine("=============================================");
                Console.WriteLine("     BANK ENTERPRISE SYSTEM - MAIN MENU      ");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. Add New Customer");
                Console.WriteLine("2. View All Customers");
                Console.WriteLine("3. Search Customer (by National ID)");
                Console.WriteLine("4. Create New Account");
                Console.WriteLine("5. Deposit Money");
                Console.WriteLine("6. Withdraw Money");
                Console.WriteLine("7. Transfer Money");
                Console.WriteLine("8. View Account Transactions");
                Console.WriteLine("9. Export Account Statement (.txt)");
                Console.WriteLine("10. Close / Deactivate Account");
                Console.WriteLine("11. Delete Customer");
                Console.WriteLine("12. View Bank System Dashboard (Reports)");
                Console.WriteLine("13. Exit Application");
                Console.WriteLine("=============================================");
                Console.Write("Select an option (1-13): ");

                string choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        AddNewCustomerUI();
                        break;
                    case "2":
                        ViewAllCustomersUI();
                        break;
                    case "3":
                        SearchCustomerUI();
                        break;
                    case "4":
                        CreateAccountUI();
                        break;
                    case "5":
                        DepositUI();
                        break;
                    case "6":
                        WithdrawUI();
                        break;
                    case "7":
                        TransferUI();
                        break;
                    case "8":
                        ViewTransactionsUI();
                        break;
                    case "9":
                        ExportStatementUI();
                        break;
                    case "10":
                        DeactivateAccountUI();
                        break;
                    case "11":
                        DeleteCustomerUI();
                        break;
                    case "12":
                        DisplayBankDashboardUI();
                        break;
                    case "13":
                        running = false;
                        Console.WriteLine("Exiting application... Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option! Press Any Key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // 1. Add New Customer UI
        static void AddNewCustomerUI()
        {
            Console.WriteLine("--- Add New Customer ---");
            Console.Write("Enter Full Name: ");
            string name = Console.ReadLine()?.Trim();

            Console.Write("Enter National ID (10 digits): ");
            string nationalId = Console.ReadLine()?.Trim();

            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine()?.Trim();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine()?.Trim();

            try
            {
                Customer customer = new Customer(0, name, phone, email, nationalId);

                if (_customerRepo.AddCustomer(customer))
                {
                    Console.WriteLine("\n[SUCCESS] Customer added successfully to the database!");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Failed to add customer.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n[VALIDATION ERROR] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 2. View All Customers UI
        static void ViewAllCustomersUI()
        {
            Console.WriteLine("--- All Registered Customers ---");
            try
            {
                List<Customer> customers = _customerRepo.GetAllCustomers();

                if (customers == null || customers.Count == 0)
                {
                    Console.WriteLine("No customers found in the database.");
                }
                else
                {
                    foreach (var c in customers)
                    {
                        Console.WriteLine($"ID: {c.Id} | Name: {c.FullName} | NationalID: {c.NationalID} | Phone: {c.PhoneNumber} | Email: {c.Email}");
                        Console.WriteLine("--------------------------------------------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Could not fetch customers: {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 3. Search Customer UI
        static void SearchCustomerUI()
        {
            Console.WriteLine("--- Search Customer by National ID ---");
            Console.Write("Enter National ID: ");
            string nationalId = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(nationalId))
            {
                Console.WriteLine("\n[INVALID INPUT] National ID cannot be empty.");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                Customer customer = _customerRepo.GetCustomerByNationalId(nationalId);

                if (customer == null)
                {
                    Console.WriteLine("\n[NOT FOUND] No customer found with this National ID.");
                }
                else
                {
                    Console.WriteLine("\n================ CUSTOMER DETAILS ================");
                    Console.WriteLine($"Customer ID : {customer.Id}");
                    Console.WriteLine($"Full Name   : {customer.FullName}");
                    Console.WriteLine($"National ID : {customer.NationalID}");
                    Console.WriteLine($"Phone       : {customer.PhoneNumber}");
                    Console.WriteLine($"Email       : {customer.Email}");
                    Console.WriteLine("------------------------------------------------");

                    List<Account> accounts = _accountRepo.GetAccountsByCustomerId(customer.Id);
                    Console.WriteLine($"Linked Bank Accounts ({accounts?.Count ?? 0}):");

                    if (accounts != null && accounts.Count > 0)
                    {
                        foreach (var acc in accounts)
                        {
                            string status = acc.IsActive ? "ACTIVE" : "INACTIVE";
                            Console.WriteLine($"  -> Account #{acc.AccountID} [{acc.AccountType}] | Balance: {acc.Balance:N2} JOD | Status: {status}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  -> No linked bank accounts found.");
                    }
                    Console.WriteLine("================================================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 4. Create Account UI
        static void CreateAccountUI()
        {
            Console.WriteLine("--- Create New Account ---");
            Console.Write("Enter Customer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId) || customerId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid positive Customer ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Account Type (Checking/Savings): ");
            string type = Console.ReadLine()?.Trim();

            Console.Write("Enter Initial Deposit Balance: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal balance) || balance < 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid positive initial deposit amount!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                Account account = new Account
                {
                    CustomerID = customerId,
                    AccountType = type,
                    Balance = balance,
                    IsActive = true
                };

                if (_accountRepo.CreateAccount(account))
                {
                    Console.WriteLine("\n[SUCCESS] Account created successfully!");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Could not create account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 5. Deposit UI
        static void DepositUI()
        {
            Console.WriteLine("--- Deposit Money ---");
            Console.Write("Enter Account ID: ");
            if (!int.TryParse(Console.ReadLine(), out int accId) || accId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid Account ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Deposit Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Deposit amount must be greater than zero!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                if (_accountRepo.Deposit(accId, amount, "Manual Cash Deposit via Console"))
                {
                    Console.WriteLine("\n[SUCCESS] Deposit processed successfully!");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Deposit failed. Check if the Account ID is correct and active.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 6. Withdraw UI
        static void WithdrawUI()
        {
            Console.WriteLine("--- Withdraw Money ---");
            Console.Write("Enter Account ID: ");
            if (!int.TryParse(Console.ReadLine(), out int accId) || accId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid Account ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Withdrawal Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Withdrawal amount must be greater than zero!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                if (_accountRepo.Withdraw(accId, amount, "Manual Cash Withdrawal via Console"))
                {
                    Console.WriteLine("\n[SUCCESS] Withdrawal processed successfully!");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Withdrawal failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 7. Transfer UI
        static void TransferUI()
        {
            Console.WriteLine("--- Transfer Money Between Accounts ---");
            Console.Write("Enter Source Account ID (From): ");
            if (!int.TryParse(Console.ReadLine(), out int srcId) || srcId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Invalid Source Account ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Destination Account ID (To): ");
            if (!int.TryParse(Console.ReadLine(), out int destId) || destId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Invalid Destination Account ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            if (srcId == destId)
            {
                Console.WriteLine("\n[INVALID OPERATION] Source and Destination accounts cannot be the same!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Transfer Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Transfer amount must be greater than zero!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                if (_transactionRepo.Transfer(srcId, destId, amount))
                {
                    Console.WriteLine("\n[SUCCESS] Transfer completed successfully!");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Transfer failed. Please check account balances and statuses.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 8. View Transactions UI
        static void ViewTransactionsUI()
        {
            Console.WriteLine("--- View Account Transactions Audit Log ---");
            Console.Write("Enter Account ID: ");
            if (!int.TryParse(Console.ReadLine(), out int accId) || accId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid Account ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                List<Transaction> list = _transactionRepo.GetTransactionsByAccountId(accId);

                if (list == null || list.Count == 0)
                {
                    Console.WriteLine("\nNo transactions found for this account.");
                }
                else
                {
                    Console.WriteLine("\n================ TRANSACTION HISTORY ================");
                    foreach (var tx in list)
                    {
                        Console.WriteLine($"Tx ID: {tx.TransactionID} | Type: {tx.TransactionType} | Amount: {tx.Amount:N2} JOD | Details: {tx.Details}");
                    }
                    Console.WriteLine("====================================================");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 9. Export Statement UI
        static void ExportStatementUI()
        {
            Console.WriteLine("--- Export Account Statement (.txt) ---");
            Console.Write("Enter Account ID: ");
            if (!int.TryParse(Console.ReadLine(), out int accId) || accId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid Account ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                List<Transaction> list = _transactionRepo.GetTransactionsByAccountId(accId);

                if (list == null || list.Count == 0)
                {
                    Console.WriteLine("\nNo transactions found to export for this account.");
                }
                else
                {
                    string filePath = _transactionRepo.ExportAccountStatement(accId, list);
                    Console.WriteLine($"\n[SUCCESS] Account statement exported successfully!");
                    Console.WriteLine($"File saved as: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Failed to export statement: {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 10. Deactivate Account UI
        static void DeactivateAccountUI()
        {
            Console.WriteLine("--- Close / Deactivate Account ---");
            Console.Write("Enter Account ID to deactivate: ");
            if (!int.TryParse(Console.ReadLine(), out int accId) || accId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid Account ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                if (_accountRepo.DeactivateAccount(accId))
                {
                    Console.WriteLine($"\n[SUCCESS] Account #{accId} has been successfully deactivated.");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Could not deactivate account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 11. Delete Customer UI
        static void DeleteCustomerUI()
        {
            Console.WriteLine("--- Delete Customer ---");
            Console.Write("Enter Customer ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId) || customerId <= 0)
            {
                Console.WriteLine("\n[INVALID INPUT] Please enter a valid Customer ID!");
                Console.WriteLine("\nPress Any Key to return to menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                if (_customerRepo.DeleteCustomer(customerId))
                {
                    Console.WriteLine($"\n[SUCCESS] Customer #{customerId} deleted successfully.");
                }
                else
                {
                    Console.WriteLine("\n[FAILED] Could not delete customer.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }

        // 12. Display Bank Dashboard UI
        static void DisplayBankDashboardUI()
        {
            try
            {
                _accountRepo.DisplayBankDashboard();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] Could not generate dashboard reports: {ex.Message}");
            }

            Console.WriteLine("\nPress Any Key to return to menu...");
            Console.ReadKey();
        }
    }
}