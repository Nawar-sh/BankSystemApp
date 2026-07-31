using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystemApp
{
    public class Customer : Person
    {
        private string _nationalID = string.Empty;

        public string NationalID
        {
            get => _nationalID;
            private set
            {
                if (value.Length != 10)
                    throw new ArgumentException("الرقم الوطني يجب أن يتكون من 10 أرقام!");
                _nationalID = value;
            }
        }

        public Customer(int id, string fullName, string nationalID, string phoneNumber, string email)
            : base(id, fullName, phoneNumber, email)
        {
            NationalID = nationalID;
        }

        public override void DisplayRolePermissions()
        {
            Console.WriteLine($"[صلاحيات العميل {FullName}]: عرض الحسابات، طلب تحويل، واستخراج كشف حساب.");
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"=== ملف العميل ===");
            Console.WriteLine(GetSummary());
            Console.WriteLine($"الرقم الوطني: {NationalID}");
            Console.WriteLine($"رقم الهاتف: {PhoneNumber}");
            Console.WriteLine($"تاريخ الانضمام: {CreatedAt.ToShortDateString()}");
        }
    }
}