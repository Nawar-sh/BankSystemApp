using System;
using System.Collections.Generic;
using System.Text;

namespace BankingSystem.Application.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
    }

    public class TransactionRequestDto
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}