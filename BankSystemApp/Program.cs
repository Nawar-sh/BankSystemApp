using BankSystem.DAL;
using System;
using System.Collections.Generic;

namespace BankSystemApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Set console output encoding to UTF-8
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=============================================");
            Console.WriteLine("   Bank Enterprise System - Full DAL Test    ");
            Console.WriteLine("=============================================\n");

            // Initialize Repositories
            CustomerRepository customerRepo = new CustomerRepository();
            AccountRepository accountRepo = new AccountRepository();
            TransactionRepository transactionRepo = new TransactionRepository();

            // -------------------------------------------------------------
            // 1. Testing Customer Operations
            // -------------------------------------------------------------
            Console.WriteLine("--- Testing Insert Customer to Database ---");

            // Generating unique Email and NationalID for testing
            string uniqueEmail = $"ahmad_{DateTime.Now:HHmmss}@gmail.com";
            string randomNationalID = new Random().Next(100000000, 999999999).ToString() + "0";

            // Order: (id, fullName, phoneNumber, email, nationalID)
            Customer newCustomer = new Customer(0, "Ahmad Ali", "0791234567", uniqueEmail, randomNationalID);

            if (customerRepo.AddCustomer(newCustomer))
            {
                Console.WriteLine("[SUCCESS] New customer added successfully to SQL Server!\n");
            }

            Console.WriteLine("--- Fetching All Customers from Database ---");
            List<Customer> customers = customerRepo.GetAllCustomers();
            foreach (var customer in customers)
            {
                Console.WriteLine("=== Customer Profile ===");
                Console.WriteLine($"Name: {customer.FullName} | Email: {customer.Email}");
                Console.WriteLine($"National ID: {customer.NationalID}");
                Console.WriteLine($"Phone: {customer.PhoneNumber}");
                Console.WriteLine($"Created At: {customer.CreatedAt:d}");
                Console.WriteLine("---------------------------------------------");
            }
            Console.WriteLine();

            // -------------------------------------------------------------
            // 2. Testing Account Operations
            // -------------------------------------------------------------
            Console.WriteLine("=============================================");
            Console.WriteLine("  Testing Account Operations (Deposit & Withdraw)");
            Console.WriteLine("=============================================\n");

            int testAccountId = 1001;
            Console.WriteLine($"--- Fetching Account #{testAccountId} ---");
            Account acc = accountRepo.GetAccountById(testAccountId);

            if (acc != null)
            {
                Console.WriteLine($"[FOUND] AccountID: {acc.AccountID} | CustomerID: {acc.CustomerID} | Type: {acc.AccountType} | Current Balance: {acc.Balance:N2} JOD\n");

                // Testing Deposit (Passing English details explicitly)
                decimal depositAmount = 100.00m;
                Console.WriteLine($"--- Testing Deposit (+{depositAmount:N2} JOD) ---");
                if (accountRepo.Deposit(testAccountId, depositAmount, "Direct Cash Deposit"))
                {
                    acc = accountRepo.GetAccountById(testAccountId);
                    Console.WriteLine($"[SUCCESS] Deposit complete! New Balance: {acc.Balance:N2} JOD\n");
                }

                // Testing Withdrawal (Passing English details explicitly)
                decimal withdrawAmount = 50.00m;
                Console.WriteLine($"--- Testing Withdrawal (-{withdrawAmount:N2} JOD) ---");
                if (accountRepo.Withdraw(testAccountId, withdrawAmount, "Direct Cash Withdrawal"))
                {
                    acc = accountRepo.GetAccountById(testAccountId);
                    Console.WriteLine($"[SUCCESS] Withdrawal complete! New Balance: {acc.Balance:N2} JOD\n");
                }
            }

            // -------------------------------------------------------------
            // 3. Testing Fund Transfer & Transaction Audit Log
            // -------------------------------------------------------------
            Console.WriteLine("=============================================");
            Console.WriteLine("  Testing Fund Transfer & Transaction Audit ");
            Console.WriteLine("=============================================\n");

            int sourceAccId = 1001;
            int destAccId = 1002;
            decimal transferAmount = 25.00m;

            Console.WriteLine($"--- Transferring {transferAmount:N2} JOD from Acc #{sourceAccId} to Acc #{destAccId} ---");
            if (transactionRepo.Transfer(sourceAccId, destAccId, transferAmount))
            {
                Console.WriteLine("[SUCCESS] Fund transfer completed atomically via SqlTransaction!\n");
            }

            // Fetching Audit Logs
            Console.WriteLine($"--- Transaction History Audit Log for Account #{sourceAccId} ---");
            List<Transaction> transactions = transactionRepo.GetTransactionsByAccountId(sourceAccId);

            foreach (var tx in transactions)
            {
                Console.WriteLine("======== TRANSACTION RECEIPT ========");
                Console.WriteLine($"Transaction ID: {tx.TransactionID} | Account ID: {tx.AccountID}");
                Console.WriteLine($"Type: {tx.TransactionType} | Amount: {tx.Amount:N2} JOD");
                Console.WriteLine($"Details: {tx.Details}");
                Console.WriteLine("=====================================\n");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}