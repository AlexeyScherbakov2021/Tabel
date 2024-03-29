﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryMSSQL<T> : IRepository<T> where T : class, IEntity, new()
    {
        //SqlConnection conn;
        protected readonly BaseModel db;
        protected readonly DbSet<T> _Set;
        public virtual IQueryable<T> Items => _Set;


        public RepositoryMSSQL()
        {
            db = new BaseModel();
            _Set = db.Set<T>();

//            ConnectionStringSettings settings;

//#if !DEBUG
//            settings = ConfigurationManager.ConnectionStrings["BaseModel"];
//            conn = new SqlConnection(settings.ConnectionString);

//#else
//                settings = ConfigurationManager.ConnectionStrings["BaseModel"];
//                SecureString theSecureString = new NetworkCredential("", "ctcnhjt,s").SecurePassword;
//                theSecureString.MakeReadOnly();
//                SqlCredential credential = new SqlCredential("fpLoginName", theSecureString);
//                conn = new SqlConnection(settings.ConnectionString, credential);

//#endif

        }

        public T Add(T item, bool Autosave = false)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            db.Entry(item).State = EntityState.Added;
            if(Autosave)
                db.SaveChanges();
            return item;
        }

        public void Delete(int id, bool Autosave = false)
        {
            if (id < 1)
                return;

            var item = _Set.Local.FirstOrDefault(i => i.id == id) ?? new T { id = id };
            Delete(item, Autosave);

            //db.Entry(item).State = EntityState.Deleted;
            //if (Autosave)
            //    db.SaveChanges();
        }

        public void Delete(T item, bool Autosave = false)
        {
            if (item is null || item.id <= 0)
                return;

            db.Entry(item).State = EntityState.Deleted;
            if (Autosave)
                db.SaveChanges();
        }


        public T Get(int id)
        {
            return Items.SingleOrDefault(it => it.id == id);
        }


        public void Update(T item, bool Autosave = false)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));
            db.Entry(item).State = EntityState.Modified;
            if (Autosave)
                db.SaveChanges();

        }

        public void Save()
        {
            db.SaveChanges();
        }

    }
}
