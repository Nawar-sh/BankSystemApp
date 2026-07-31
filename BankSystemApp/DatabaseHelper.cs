using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace BankSystemApp
{
    public static class DatabaseHelper
    {
        // اسم السيرفر الدقيق المأخوذ من شاشتك: (localdb)\MSSQLLocalDB
        // تعديل اسم المتغير ليصبح ConnectionString (حرف C كبير) مع خاصية Get
        public static string ConnectionString { get; } =
            @"Server=(localdb)\MSSQLLocalDB;Database=BankSystemDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}