﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Models;

namespace Tabel.Repository
{
    public class RepositoryMSSQL<T> : IRepository<T> where T : class, IEntity, new()
    {
        //SqlConnection conn;
        //protected BaseModel db => App.db;
        protected BaseModel db;// = new BaseModel();
        protected readonly DbSet<T> _Set;
        public virtual IQueryable<T> Items => _Set;


        public RepositoryMSSQL(/*BaseModel Db = null*/)
        {
            App.Log.WriteLineLog("RepositoryMSSQL");

            db = /*Db ??*/ BaseModel.CreateDB();

            App.Log.WriteLineLog($"db = {db}");

            //db = new BaseModel();
            _Set = db.Set<T>();

            //App.Log.WriteLineLog($"_Set = {_Set}");

            //db.Configuration.LazyLoadingEnabled = true;
            //db.Configuration.ProxyCreationEnabled = true;
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

        //        private BaseModel CreateDB()
        //        {
        //            string ConnectString;

        //#if DEBUG
        //            ConnectString = ConfigurationManager.ConnectionStrings["BaseModel"].ConnectionString;
        //            ConnectString += ";user id=fpLoginName;password=ctcnhjt,s";
        //            //ConnectString = ConfigurationManager.ConnectionStrings["ModelLocal"].ConnectionString;
        //#else
        //            ConnectString = ConfigurationManager.ConnectionStrings["BaseModel"].ConnectionString;
        //            ConnectString += ";user id=fpLoginName;password=ctcnhjt,s";
        //#endif
        //            return new BaseModel(ConnectString);

        //        }



        public T Add(T item, bool Autosave = false)
        {
            //return db.Set<T>().Add(item);

            if (item is null) throw new ArgumentNullException(nameof(item));
            //db.Entry(item).State = EntityState.Added;
            _Set.Add(item);
            if (Autosave)
                //db.SaveChanges();
                Save();
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
               Save();
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
               Save();

        }

        public void Save()
        {

            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка записи в базу", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Remove(T item)
        {
            db.Set<T>().Remove(item);
            Save();
        }

        public BaseModel GetDB() => db;



    }
}
