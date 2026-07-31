using System;
using System.Collections.Generic;
using BankSystemApp;

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
                CustomerRepository customerRepo = new CustomerRepository();

                Console.WriteLine("--- Testing Insert Customer to Database ---");

                // توليد رقم عشوائي مكون من 10 أرقام بالضبط لتجاوز الـ Validation
                Random random = new Random();
                string nationalId10Digits = random.Next(100000000, 999999999).ToString() + random.Next(0, 9).ToString();
                string timeStamp = DateTime.Now.ToString("HHmmss");

                Customer newCustomer = new Customer(
                    id: 0,
                    fullName: "Ahmad Ali",
                    nationalID: nationalId10Digits, // 10 أرقام تماماً
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