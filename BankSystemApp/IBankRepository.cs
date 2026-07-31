using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystemApp
{
    public interface IBankRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        bool Add(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
