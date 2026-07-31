using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystemApp
{
    public class Account
    {
        // Properties with public getters and setters
        public int AccountID { get; set; }
        public int CustomerID { get; set; }
        public string AccountType { get; set; } = "Savings";
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 1. Parameterless Constructor (ضروري جداً عشان تزبط الأسطر اللي بالصورة)
        public Account()
        {
        }

        // 2. Constructor لإنشاء حساب جديد
        public Account(int customerID, string accountType, decimal initialBalance = 0.00m)
        {
            if (accountType != "Savings" && accountType != "Checking")
            {
                throw new ArgumentException("Account type must be either 'Savings' or 'Checking'.");
            }

            if (initialBalance < 0)
            {
                throw new ArgumentException("Initial balance cannot be negative.");
            }

            CustomerID = customerID;
            AccountType = accountType;
            Balance = initialBalance;
            IsActive = true;
            CreatedAt = DateTime.Now;
        }
    }
}

