using System;
using System.Collections.Generic;
using System.Text;

using BankingSystem.Application.Interfaces;
using BankingSystem.Domain.Entities;
using BankingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Account>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task CreateAccountAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public async Task<bool> DepositAsync(int accountId, decimal amount, string details)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null || !account.IsActive) return false;

            account.Balance += amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionType = "Deposit",
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Details = details
            };

            await _context.Transactions.AddAsync(transaction);
            return await SaveChangesAsync();
        }

        public async Task<bool> WithdrawAsync(int accountId, decimal amount, string details)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null || !account.IsActive || account.Balance < amount) return false;

            account.Balance -= amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionType = "Withdrawal",
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Details = details
            };

            await _context.Transactions.AddAsync(transaction);
            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}