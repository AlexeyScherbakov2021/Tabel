using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryMSSQL : IRepository
    {
        //SqlConnection conn;
        private readonly BaseModel ctx;

        public RepositoryMSSQL()
        {
            ctx = new BaseModel();

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

        public int AddOtdel(Otdel newOtdel, Otdel parent = null)
        {
            throw new NotImplementedException();
        }

        public int AddUser(User newUser)
        {
            throw new NotImplementedException();
        }

        public int DeleteOtdel(int idOtdel)
        {
            throw new NotImplementedException();
        }

        public int DeleteUser(int idUser)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Otdel> GetOtdels()
        {
            return ctx.otdels.ToArray();
        }

        public IEnumerable<User> GetUsers()
        {
            return ctx.users.ToArray();
            
        }

        public int UpdateOtdel(Otdel editOtdel)
        {
            throw new NotImplementedException();
        }
    }
}
