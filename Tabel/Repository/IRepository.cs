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
        T Add(T item, bool Autosave = false);
        void Delete(int id, bool Autosave = false);
        void Delete(T item, bool Autosave = false);
        void Update(T item, bool Autosave = false);


        bool Save();

        //BaseModel CreateContext();

        //BaseModel GetDB();
    }
}
