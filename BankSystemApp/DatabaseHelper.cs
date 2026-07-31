using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Data.SqlClient;

namespace BankSystemApp
{
    public static class DatabaseHelper
    {
        // اسم السيرفر الدقيق المأخوذ من شاشتك: (localdb)\MSSQLLocalDB
        private static readonly string connectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=BankSystemDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}