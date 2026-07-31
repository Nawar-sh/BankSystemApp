using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystemApp
{
    public enum UserRole
    {
        Admin,
        Teller,
        Manager
    }

    public class Employee : Person
    {
        private string _username = string.Empty;
        private string _passwordHash = string.Empty;

        public string Username
        {
            get => _username;
            private set => _username = value;
        }

        public UserRole Role { get; set; }
        public bool IsActive { get; private set; }

        public Employee(int id, string fullName, string username, string passwordHash, UserRole role, string phone, string email)
            : base(id, fullName, phone, email)
        {
            _username = username;
            _passwordHash = passwordHash;
            Role = role;
            IsActive = true;
        }

        public bool ValidatePassword(string inputPassword)
        {
            return _passwordHash == inputPassword;
        }

        public override void DisplayRolePermissions()
        {
            Console.WriteLine($"[صلاحيات الموظف {FullName} - دور: {Role}]:");
            switch (Role)
            {
                case UserRole.Admin:
                    Console.WriteLine("- إدارة الموظفين، فتح وإغلاق الحسابات، والتحكم بالصلاحيات والـ Logs.");
                    break;
                case UserRole.Teller:
                    Console.WriteLine("- تنفيذ عمليات السحب والإيداع المباشر للعملاء.");
                    break;
                case UserRole.Manager:
                    Console.WriteLine("- الموافقة على التحويلات المالية الكبيرة وعرض التقارير.");
                    break;
            }
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"=== ملف الموظف ===");
            Console.WriteLine(GetSummary());
            Console.WriteLine($"اسم المستخدم: {Username}");
            Console.WriteLine($"الدور: {Role}");
            Console.WriteLine($"الحالة: {(IsActive ? "نشط" : "معطل")}");
        }
    }
}
