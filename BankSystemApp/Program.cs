using System;
using System.Collections.Generic;
using BankSystemApp;

namespace BankSystemApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("    Bank Enterprise System - OOP Test Run    ");
            Console.WriteLine("=============================================\n");

            try
            {
                // 1. Create Customer
                Customer customer1 = new Customer(
                    id: 101,
                    fullName: "Ahmad Ali",
                    nationalID: "1234567890",
                    phoneNumber: "0791234567",
                    email: "ahmad@example.com"
                );

                customer1.PrintDetails();
                customer1.DisplayRolePermissions();
                Console.WriteLine();

                // 2. Create Bank Account
                Account acc1 = new Account(
                    accountId: 5001,
                    customerId: customer1.Id,
                    accountType: "Checking",
                    initialBalance: 250.00m
                );

                acc1.Deposit(100.00m);
                acc1.Withdraw(50.00m);
                Console.WriteLine();

                // 3. Create Employee
                Employee emp1 = new Employee(
                    id: 1,
                    fullName: "Nawar Admin",
                    username: "admin_nawar",
                    passwordHash: "securePass123",
                    role: UserRole.Admin,
                    phone: "0788888888",
                    email: "admin@bank.com"
                );

                emp1.PrintDetails();
                emp1.DisplayRolePermissions();
                Console.WriteLine();

                // 4. Test Polymorphism using IPrintable
                List<IPrintable> printables = new List<IPrintable>
                {
                    customer1,
                    emp1,
                    new Transaction(7001, acc1.AccountID, "Deposit", 100.00m, "ATM Cash Deposit")
                };

                Console.WriteLine("\n--- Polymorphism Output ---");
                foreach (var item in printables)
                {
                    item.PrintDetails();
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}