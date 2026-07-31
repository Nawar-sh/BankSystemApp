using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystemApp
{
    public class Transaction : IPrintable
    {
        public int TransactionID { get; private set; }
        public int AccountID { get; private set; }
        public string TransactionType { get; private set; } // Deposit, Withdrawal, Transfer
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public string Details { get; private set; }

        public Transaction(int transactionId, int accountId, string type, decimal amount, string details)
        {
            TransactionID = transactionId;
            AccountID = accountId;
            TransactionType = type;
            Amount = amount;
            Details = details;
            Date = DateTime.Now;
        }

        public void PrintDetails()
        {
            Console.WriteLine($"======== إيصال حركة مالية ========");
            Console.WriteLine($"رقم الحركة: {TransactionID} | رقم الحساب: {AccountID}");
            Console.WriteLine($"نوع الحركة: {TransactionType} | المبلغ: {Amount} JOD");
            Console.WriteLine($"التاريخ: {Date}");
            Console.WriteLine($"التفاصيل: {Details}");
            Console.WriteLine($"==================================");
        }

        public string GetSummary()
        {
            return $"Tx #{TransactionID} | {TransactionType} | {Amount} JOD | {Date.ToShortTimeString()}";
        }
    }
}
