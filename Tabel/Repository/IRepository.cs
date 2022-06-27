using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;

namespace Tabel.Repository
{
    interface IRepository
    {
        IEnumerable<User> GetUsers();
        int AddUser(User newUser);
        int DeleteUser(int idUser);
    }
}
