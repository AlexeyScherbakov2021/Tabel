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

        // Пользователи
        IEnumerable<User> GetUsers();
        int AddUser(User newUser);
        int DeleteUser(int idUser);


        // отделы
        IEnumerable<Otdel> GetOtdels();
        int AddOtdel(Otdel newOtdel, Otdel parent = null);
        int DeleteOtdel(int idOtdel);
        int UpdateOtdel(Otdel editOtdel);
    }
}
