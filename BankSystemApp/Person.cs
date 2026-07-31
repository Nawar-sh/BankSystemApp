using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystemApp
{
    public abstract class Person : IPrintable
    {
        private string _fullName = string.Empty;
        private string _email = string.Empty;

        public int Id { get; protected set; }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("الاسم لا يمكن أن يكون فارغاً!");
                _fullName = value;
            }
        }

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email
        {
            get => _email;
            set
            {
                if (!value.Contains("@"))
                    throw new ArgumentException("صيغة البريد الإلكتروني غير صحيحة!");
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

        // Abstract Methods: تتغير طريقة تطبيقها حسب نوع المستخدِم (Polymorphism)
        public abstract void DisplayRolePermissions();
        public abstract void PrintDetails();

        public virtual string GetSummary()
        {
            return $"ID: {Id} | Name: {FullName} | Email: {Email}";
        }
    }
}