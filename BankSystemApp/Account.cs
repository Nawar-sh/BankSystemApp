using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystemApp
{
    public class Account
    {
        public int AccountID { get; private set; }
        public int CustomerID { get; private set; }
        public string AccountType { get; set; } // Checking or Savings

        private decimal _balance;
        public decimal Balance
        {
            get => _balance;
            private set
            {
                if (value < 0)
                    throw new InvalidOperationException("الرصيد لا يمكن أن يكون بالسالب!");
                _balance = value;
            }
        }

        public bool IsActive { get; set; }

        public Account(int accountId, int customerId, string accountType, decimal initialBalance)
        {
            AccountID = accountId;
            CustomerID = customerId;
            AccountType = accountType;
            Balance = initialBalance;
            IsActive = true;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("مبلغ الإيداع يجب أن يكون أكبر من zero!");

            _balance += amount;
            Console.WriteLine($"تم إيداع {amount} JOD بنجاح. الرصيد الحالي: {_balance} JOD");
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("مبلغ السحب يجب أن يكون أكبر من zero!");

            if (amount > _balance)
            {
                Console.WriteLine("عذراً، الرصيد غير كافٍ إجراء هذه العملية!");
                return false;
            }

            _balance -= amount;
            Console.WriteLine($"تم سحب {amount} JOD بنجاح. الرصيد المتبقي: {_balance} JOD");
            return true;
        }
    }
}
