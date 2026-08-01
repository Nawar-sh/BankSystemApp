using System;

namespace BankSystemApp
{
    public class Employee : Person
    {
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }

        public Employee(int id, string fullName, string phoneNumber, string email, string position, decimal salary)
            : base(id, fullName, phoneNumber, email)
        {
            Position = position;
            Salary = salary;
        }

        public override void DisplayRolePermissions()
        {
            Console.WriteLine($"[Employee Permissions for {FullName}]: Manage Accounts, Process Transactions, and Access Reports.");
        }

        public override void PrintDetails()
        {
            Console.WriteLine("=== Employee Profile ===");
            Console.WriteLine($"ID: {Id} | Name: {FullName} | Position: {Position}");
            Console.WriteLine($"Email: {Email} | Phone: {PhoneNumber}");
            Console.WriteLine($"Salary: {Salary:N2} JOD");
            Console.WriteLine($"Created At: {CreatedAt.ToShortDateString()}");
        }

        // Helper method to display employee details summary
        public string GetEmployeeDetails()
        {
            return $"Employee #{Id}: {FullName} - Position: {Position}";
        }
    }
}