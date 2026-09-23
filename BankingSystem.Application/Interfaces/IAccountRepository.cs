using System;
using System.Collections.Generic;
using System.Text;

using BankingSystem.Domain.Entities;

namespace BankingSystem.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int id);
        Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId);
        Task CreateAccountAsync(Account account);
        Task<bool> DepositAsync(int accountId, decimal amount, string details);
        Task<bool> WithdrawAsync(int accountId, decimal amount, string details);
        Task<bool> SaveChangesAsync();
    }
}