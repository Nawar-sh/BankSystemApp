using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace BankingSystem.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string NationalID { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // علاقة: العميل يملك عدة حسابات
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
