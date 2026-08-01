using System;

namespace BankSystemApp
{
    public class Customer : Person
    {
        private string _nationalID = string.Empty;

        public string NationalID
        {
            get => _nationalID;
            private set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length != 10)
                    throw new ArgumentException("National ID must be exactly 10 digits!");

                _nationalID = value;
            }
        }

        // Standardized Constructor Order: (id, fullName, phoneNumber, email, nationalID)
        public Customer(int id, string fullName, string phoneNumber, string email, string nationalID)
            : base(id, fullName, phoneNumber, email)
        {
            NationalID = nationalID;
        }

        public override void DisplayRolePermissions()
        {
            Console.WriteLine($"[Customer Permissions for {FullName}]: View Accounts, Request Transfers, and Export Bank Statements.");
        }

        public override void PrintDetails()
        {
            Console.WriteLine("=== Customer Profile ===");
            Console.WriteLine($"ID: {Id} | Name: {FullName} | Email: {Email}");
            Console.WriteLine($"National ID: {NationalID}");
            Console.WriteLine($"Phone: {PhoneNumber}");
            Console.WriteLine($"Created At: {CreatedAt.ToShortDateString()}");
        }
    }
}