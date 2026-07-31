using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BankSystemApp;

namespace BankSystemApp
{
    public class CustomerRepository
    {
        // 1. دالة إضافة عميل جديد لقاعدة البيانات
        public bool AddCustomer(Customer customer)
        {
            string query = @"INSERT INTO Customers (FullName, NationalID, PhoneNumber, Email) 
                            VALUES (@FullName, @NationalID, @PhoneNumber, @Email)";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                    cmd.Parameters.AddWithValue("@NationalID", customer.NationalID);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", customer.Email ?? (object)DBNull.Value);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // 2. دالة قراءة جميع العملاء من قاعدة البيانات
        public List<Customer> GetAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            string query = "SELECT CustomerID, FullName, NationalID, PhoneNumber, Email FROM Customers";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Customer customer = new Customer(
                                id: Convert.ToInt32(reader["CustomerID"]),
                                fullName: reader["FullName"].ToString(),
                                nationalID: reader["NationalID"].ToString(),
                                phoneNumber: reader["PhoneNumber"] != DBNull.Value ? reader["PhoneNumber"].ToString() : "",
                                email: reader["Email"] != DBNull.Value ? reader["Email"].ToString() : ""
                            );

                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }
    }
}