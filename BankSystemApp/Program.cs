using System;
using System.Collections.Generic;
using BankSystem.DAL;
using BankSystem;

namespace BankSystemApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=============================================");
            Console.WriteLine("  Bank Enterprise System - Database DAL Test ");
            Console.WriteLine("=============================================\n");

            try
            {
                // ==========================================
                // 1. اختبار الـ CustomerRepository
                // ==========================================
                CustomerRepository customerRepo = new CustomerRepository();

                Console.WriteLine("--- Testing Insert Customer to Database ---");

                // توليد رقم عشوائي مكون من 10 أرقام لتجاوز الـ Validation
                Random random = new Random();
                string nationalId10Digits = random.Next(100000000, 999999999).ToString() + random.Next(0, 9).ToString();
                string timeStamp = DateTime.Now.ToString("HHmmss");

                Customer newCustomer = new Customer(
                    id: 0,
                    fullName: "Ahmad Ali",
                    nationalID: nationalId10Digits,
                    phoneNumber: "0791234567",
                    email: $"ahmad_{timeStamp}@gmail.com"
                );

                bool isAdded = customerRepo.AddCustomer(newCustomer);

                if (isAdded)
                {
                    Console.WriteLine("[SUCCESS] New customer added successfully to SQL Server!");
                }
                else
                {
                    Console.WriteLine("[FAILED] Failed to insert customer.");
                }

                Console.WriteLine();

                // قراءة كل العملاء من الـ Database
                Console.WriteLine("--- Fetching All Customers from Database ---");
                List<Customer> customerList = customerRepo.GetAllCustomers();

                foreach (var customer in customerList)
                {
                    customer.PrintDetails();
                    Console.WriteLine("---------------------------------------------");
                }

                // ==========================================
                // 2. اختبار الـ AccountRepository (الجديد)
                // ==========================================
                Console.WriteLine("\n=============================================");
                Console.WriteLine("  Testing Account Operations (DAL) ");
                Console.WriteLine("=============================================\n");

                // استخدام الـ Connection String المركزي من DatabaseHelper بدلاً من كتابته يدوياً
                AccountRepository accountRepo = new AccountRepository(DatabaseHelper.ConnectionString);

                // استعلام عن الحساب رقم 1001 (الموجود في سكريبت الـ SQL)
                Console.WriteLine("--- Fetching Account 1001 ---");
                Account acc = accountRepo.GetAccountById(1001);

                if (acc != null)
                {
                    Console.WriteLine($"[FOUND] AccountID: {acc.AccountID} | CustomerID: {acc.CustomerID} | Type: {acc.AccountType} | Current Balance: {acc.Balance} JOD");

                    // تجربة إيداع 100 دينار
                    Console.WriteLine("\n--- Testing Deposit (Adding 100 JOD) ---");
                    if (accountRepo.Deposit(1001, 100.00m))
                    {
                        acc = accountRepo.GetAccountById(1001);
                        Console.WriteLine($"[SUCCESS] Deposit complete! New Balance: {acc.Balance} JOD");
                    }

                    // تجربة سحب 50 دينار
                    Console.WriteLine("\n--- Testing Withdrawal (Withdrawing 50 JOD) ---");
                    if (accountRepo.Withdraw(1001, 50.00m))
                    {
                        acc = accountRepo.GetAccountById(1001);
                        Console.WriteLine($"[SUCCESS] Withdrawal complete! New Balance: {acc.Balance} JOD");
                    }
                }
                else
                {
                    Console.WriteLine("[INFO] Account 1001 not found. Make sure you inserted data in SQL Server.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Connection or Query failed: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}