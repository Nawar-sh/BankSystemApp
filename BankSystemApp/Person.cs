using System;

namespace BankSystemApp
{
    public abstract class Person
    {
        public int Id { get; protected set; }

        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Full name cannot be empty.");

                _fullName = value;
            }
        }

        public string PhoneNumber { get; set; } = string.Empty;

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !value.Contains("@"))
                    throw new ArgumentException("Invalid email format!");

                _email = value;
            }
        }

        public DateTime CreatedAt { get; private set; }

        protected Person(int id, string fullName, string phoneNumber, string email)
        {
            Id = id;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Email = email;
            CreatedAt = DateTime.Now;
        }

        // Methods to be overridden in derived classes (Customer / Employee)
        public abstract void DisplayRolePermissions();

        public virtual void PrintDetails()
        {
            Console.WriteLine($"ID: {Id} | Name: {FullName} | Email: {Email} | Phone: {PhoneNumber}");
        }
    }
}