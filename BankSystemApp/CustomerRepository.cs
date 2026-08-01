using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BankSystemApp
{
    public class CustomerRepository
    {
        /// <summary>
        /// Inserts a new customer record into the database
        /// </summary>
        public bool AddCustomer(Customer customer)
        {
            string query = @"INSERT INTO Customers (FullName, NationalID, PhoneNumber, Email)
                            VALUES (@FullName, @NationalID, @PhoneNumber, @Email);";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", customer.FullName);
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

                            // Standardized Constructor Order: (id, fullName, phoneNumber, email, nationalID)
                            Customer customer = new Customer(id, fullName, phone, email, nationalID);
                            customers.Add(customer);
                        }
                    }
                }
            }
            return customers;
        }
    }
}