using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BankSystemApp
{
    public class TransactionRepository
    {
        /// <summary>
        /// Logs a single financial transaction (Deposit or Withdrawal)
        /// </summary>
        public bool LogTransaction(int accountId, string type, decimal amount, string details)
        {
            string query = @"INSERT INTO Transactions (AccountID, TransactionType, Amount, TransactionDate, Details)
                            VALUES (@AccountID, @TransactionType, @Amount, @TransactionDate, @Details);";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    cmd.Parameters.AddWithValue("@TransactionType", type);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Details", (object)details ?? DBNull.Value);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Retrieves all transactions for a specific account
        /// </summary>
        public List<Transaction> GetTransactionsByAccountId(int accountId)
        {
            List<Transaction> transactions = new List<Transaction>();
            string query = @"SELECT TransactionID, AccountID, TransactionType, Amount, Details, TransactionDate 
                            FROM Transactions 
                            WHERE AccountID = @AccountID 
                            ORDER BY TransactionDate DESC;";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AccountID", accountId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int txId = Convert.ToInt32(reader["TransactionID"]);
                            int accId = Convert.ToInt32(reader["AccountID"]);
                            string type = reader["TransactionType"].ToString();
                            decimal amount = Convert.ToDecimal(reader["Amount"]);
                            string details = reader["Details"] != DBNull.Value ? reader["Details"].ToString() : "";

                            Transaction tx = new Transaction(txId, accId, type, amount, details);
                            transactions.Add(tx);
                        }
                    }
                }
            }
            return transactions;
        }

        /// <summary>
        /// Executes a safe fund transfer between two accounts using SqlTransaction
        /// </summary>
        public bool Transfer(int sourceAccountId, int destinationAccountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.");

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    // 1. Deduct from source account
                    string deductQuery = @"UPDATE Accounts 
                                          SET Balance = Balance - @Amount 
                                          WHERE AccountID = @SourceAccountId AND Balance >= @Amount;";

                    using (SqlCommand cmdDeduct = new SqlCommand(deductQuery, conn, transaction))
                    {
                        cmdDeduct.Parameters.AddWithValue("@Amount", amount);
                        cmdDeduct.Parameters.AddWithValue("@SourceAccountId", sourceAccountId);

                        if (cmdDeduct.ExecuteNonQuery() == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    // 2. Add to destination account
                    string addQuery = @"UPDATE Accounts 
                                       SET Balance = Balance + @Amount 
                                       WHERE AccountID = @DestinationAccountId;";

                    using (SqlCommand cmdAdd = new SqlCommand(addQuery, conn, transaction))
                    {
                        cmdAdd.Parameters.AddWithValue("@Amount", amount);
                        cmdAdd.Parameters.AddWithValue("@DestinationAccountId", destinationAccountId);

                        if (cmdAdd.ExecuteNonQuery() == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    // 3. Log Transfer for source
                    string logSourceQuery = @"INSERT INTO Transactions (AccountID, TransactionType, Amount, TransactionDate, Details)
                                             VALUES (@SourceAccountID, 'Transfer', @Amount, @TransactionDate, @Details);";

                    using (SqlCommand cmdLogSource = new SqlCommand(logSourceQuery, conn, transaction))
                    {
                        cmdLogSource.Parameters.AddWithValue("@SourceAccountID", sourceAccountId);
                        cmdLogSource.Parameters.AddWithValue("@Amount", amount);
                        cmdLogSource.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                        cmdLogSource.Parameters.AddWithValue("@Details", $"Transfer Out to Account #{destinationAccountId}");
                        cmdLogSource.ExecuteNonQuery();
                    }

                    // 4. Log Transfer for destination
                    string logDestQuery = @"INSERT INTO Transactions (AccountID, TransactionType, Amount, TransactionDate, Details)
                                           VALUES (@DestinationAccountID, 'Transfer', @Amount, @TransactionDate, @Details);";

                    using (SqlCommand cmdLogDest = new SqlCommand(logDestQuery, conn, transaction))
                    {
                        cmdLogDest.Parameters.AddWithValue("@DestinationAccountID", destinationAccountId);
                        cmdLogDest.Parameters.AddWithValue("@Amount", amount);
                        cmdLogDest.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
                        cmdLogDest.Parameters.AddWithValue("@Details", $"Transfer In from Account #{sourceAccountId}");
                        cmdLogDest.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}