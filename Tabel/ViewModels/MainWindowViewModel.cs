using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly IRepository repo;
        public User CurrentUser;

        public MainWindowViewModel()
        {
            CurrentUser = App.CurrentUser;

            repo = new RepositoryMSSQL();

        }
    }
}
