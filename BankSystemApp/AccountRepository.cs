
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BankSystemApp
{
    public class AccountRepository
    {
        private readonly string _connectionString;
        private readonly TransactionRepository _transactionRepository;

        public AccountRepository()
        {
            _connectionString = DatabaseHelper.ConnectionString;
            _transactionRepository = new TransactionRepository();
        }

        public AccountRepository(string connectionString)
        {
            _connectionString = connectionString;
            _transactionRepository = new TransactionRepository();
        }

        public bool CreateAccount(Account account)
        {
            string query = @"INSERT INTO Accounts (CustomerID, AccountType, Balance, IsActive) 
                            VALUES (@CustomerID, @AccountType, @Balance, @IsActive);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", account.CustomerID);
                    cmd.Parameters.AddWithValue("@AccountType", account.AccountType);
                    cmd.Parameters.AddWithValue("@Balance", account.Balance);
                    cmd.Parameters.AddWithValue("@IsActive", account.IsActive);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public Account GetAccountById(int accountId)
        {
            string query = "SELECT AccountID, CustomerID, AccountType, Balance, IsActive, CreatedAt FROM Accounts WHERE AccountID = @AccountID";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Account
                            {
                                AccountID = Convert.ToInt32(reader["AccountID"]),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                AccountType = reader["AccountType"].ToString(),
                                Balance = Convert.ToDecimal(reader["Balance"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<Account> GetAccountsByCustomerId(int customerId)
        {
            List<Account> accounts = new List<Account>();
            string query = "SELECT AccountID, CustomerID, AccountType, Balance, IsActive, CreatedAt FROM Accounts WHERE CustomerID = @CustomerID";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account
                            {
                                AccountID = Convert.ToInt32(reader["AccountID"]),
                                CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                AccountType = reader["AccountType"].ToString(),
                                Balance = Convert.ToDecimal(reader["Balance"]),
                                IsActive = Convert.ToBoolean(reader["IsActive"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                            });
                        }
                    }
                }
            }
            return accounts;
        }

        public bool Deposit(int accountId, decimal amount, string details = "Direct Cash Deposit")
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.");

            string query = "UPDATE Accounts SET Balance = Balance + @Amount WHERE AccountID = @AccountID AND IsActive = 1";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        _transactionRepository.LogTransaction(accountId, "Deposit", amount, details);
                        return true;
                    }
                    return false;
                }
            }
        }

        public bool Withdraw(int accountId, decimal amount, string details = "Direct Cash Withdrawal")
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.");

            Account account = GetAccountById(accountId);
            if (account == null || account.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance or account not active.");
            }

            string query = "UPDATE Accounts SET Balance = Balance - @Amount WHERE AccountID = @AccountID AND IsActive = 1";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        _transactionRepository.LogTransaction(accountId, "Withdrawal", amount, details);
                        return true;
                    }
                    return false;
                }
            }
        }

        /// <summary>
        /// Deactivates an account (Soft Delete) after verifying that the balance is 0.
        /// </summary>
        public bool DeactivateAccount(int accountId)
        {
            Account account = GetAccountById(accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Account does not exist.");
            }

            if (!account.IsActive)
            {
                throw new InvalidOperationException("Account is already deactivated.");
            }

            if (account.Balance > 0)
            {
                throw new InvalidOperationException($"Cannot close account #{accountId}. Remaining balance is {account.Balance:N2} JOD. Please withdraw or transfer the funds first.");
            }

            string query = "UPDATE Accounts SET IsActive = 0 WHERE AccountID = @AccountID";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Fetches system-wide statistical reporting for the main dashboard.
        /// </summary>
        public void DisplayBankDashboard()
        {
            string query = @"
                SELECT 
                    (SELECT COUNT(*) FROM Customers) AS TotalCustomers,
                    (SELECT COUNT(*) FROM Accounts WHERE IsActive = 1) AS ActiveAccounts,
                    (SELECT COUNT(*) FROM Accounts WHERE IsActive = 0) AS ClosedAccounts,
                    (SELECT ISNULL(SUM(Balance), 0) FROM Accounts WHERE IsActive = 1) AS TotalLiquidity,
                    (SELECT ISNULL(MAX(Balance), 0) FROM Accounts WHERE IsActive = 1) AS HighestBalance;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int totalCustomers = Convert.ToInt32(reader["TotalCustomers"]);
                            int activeAccounts = Convert.ToInt32(reader["ActiveAccounts"]);
                            int closedAccounts = Convert.ToInt32(reader["ClosedAccounts"]);
                            decimal totalLiquidity = Convert.ToDecimal(reader["TotalLiquidity"]);
                            decimal highestBalance = Convert.ToDecimal(reader["HighestBalance"]);

                            Console.Clear();
                            Console.WriteLine("=============================================");
                            Console.WriteLine("       BANK ENTERPRISE - DASHBOARD           ");
                            Console.WriteLine("=============================================");
                            Console.WriteLine($" Total Customers Registered : {totalCustomers}");
                            Console.WriteLine($" Total Active Accounts     : {activeAccounts}");
                            Console.WriteLine($" Total Deactivated Accounts: {closedAccounts}");
                            Console.WriteLine("---------------------------------------------");
                            Console.WriteLine($" Total Bank Liquidity      : {totalLiquidity:C2}");
                            Console.WriteLine($" Highest Account Balance   : {highestBalance:C2}");
                            Console.WriteLine("=============================================");
                        }
                    }
                }
            }
        }
    }
}