using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    interface IRepository<T> where T : class, IEntity, new()
    {
        IQueryable<T> Items { get; }
        T Get(int id);
        T Add(T item);
        void Delete(int id);
        void Update(T item);
    }
}
