using System;
using System.Collections.Generic;
using System.Text;


    namespace BankingSystem.Domain.Entities
    {
        public class Transaction
        {
            public int Id { get; set; }
            public int AccountId { get; set; }
            public string TransactionType { get; set; } = string.Empty; // Deposit, Withdraw, Transfer
            public decimal Amount { get; set; }
            public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
            public string Details { get; set; } = string.Empty;

            // Navigation Property
            public Account Account { get; set; } = null!;
        }
    }