using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BankSystemApp
{
    public class CustomerRepository
    {
        /// <summary>
        /// Inserts a new customer record into the database with UTF-16 Unicode support
        /// </summary>
        public bool AddCustomer(Customer customer)
        {
            string query = @"INSERT INTO Customers (FullName, NationalID, PhoneNumber, Email)
                            VALUES (@FullName, @NationalID, @PhoneNumber, @Email);";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // صراحة تحديد NVarChar لدعم النصوص العربية والدولية دون تشويه
                    cmd.Parameters.Add("@FullName", System.Data.SqlDbType.NVarChar).Value = (object)customer.FullName ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@NationalID", customer.NationalID);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        /// <summary>
        /// Retrieves all customer records from the database
        /// </summary>
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            string query = @"SELECT CustomerID, FullName, NationalID, PhoneNumber, Email, CreatedAt FROM Customers;";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["CustomerID"]);
                            string fullName = reader["FullName"].ToString();
                            string nationalID = reader["NationalID"].ToString();
                            string phone = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "";
                            string email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";

                            Customer customer = new Customer(id, fullName, phone, email, nationalID);
                            customers.Add(customer);
                        }
                    }
                }
            }
            return customers;
        }

        /// <summary>
        /// Searches for a customer using their National ID
        /// </summary>
        public Customer GetCustomerByNationalId(string nationalId)
        {
            string query = @"SELECT CustomerID, FullName, NationalID, PhoneNumber, Email, CreatedAt 
                            FROM Customers WHERE NationalID = @NationalID;";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NationalID", nationalId);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["CustomerID"]);
                            string fullName = reader["FullName"].ToString();
                            string nationalID = reader["NationalID"].ToString();
                            string phone = reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "";
                            string email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";

                            return new Customer(id, fullName, phone, email, nationalID);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes a customer if they have no active accounts
        /// </summary>
        public bool DeleteCustomer(int customerId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // 1. Check if the customer has active accounts
                string checkQuery = "SELECT COUNT(*) FROM Accounts WHERE CustomerID = @CustomerID AND IsActive = 1;";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@CustomerID", customerId);
                    int activeAccounts = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (activeAccounts > 0)
                    {
                        throw new Exception("Cannot delete customer! They still have active bank account(s). Close or deactivate their accounts first.");
                    }
                }

                // 2. Delete the customer
                string deleteQuery = "DELETE FROM Customers WHERE CustomerID = @CustomerID;";
                using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@CustomerID", customerId);
                    int rowsAffected = deleteCmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        /// <summary>
        /// Returns total count of registered customers for dashboard reporting
        /// </summary>
        public int GetTotalCustomersCount()
        {
            string query = "SELECT COUNT(*) FROM Customers;";
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}