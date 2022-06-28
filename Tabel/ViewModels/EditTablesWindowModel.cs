using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class EditTablesWindowModel : ViewModel
    {
        private readonly RepositoryOtdel repo;
        public ObservableCollection<Otdel> ListOtdel { get; set; }


        //private bool _IsSelected;
        //public bool IsSelected { get => _IsSelected; set { Set(ref _IsSelected, value); } }

        private Otdel _SelectedItem;
        public Otdel SelectedItem { get => _SelectedItem; set { Set(ref _SelectedItem, value); } }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Добавить отдел
        //--------------------------------------------------------------------------------
        public ICommand AddCommand => new LambdaCommand(OnAddCommandExecuted, CanAddCommand);
        private bool CanAddCommand(object p) => true;
        private void OnAddCommandExecuted(object p)
        {
            Otdel otdel = new Otdel {
                ot_name = "Новый отдел 2",
                parent = SelectedItem,
                ot_parent = SelectedItem?.id,
                subOtdels = new ObservableCollection<Otdel>()
            };

            //repo.Add(otdel);
            //Otdel parentOtdel = ListOtdel.SingleOrDefault(it => it.id == otdel.parent.id);

            if (otdel.parent is null)
                ListOtdel.Add(otdel);
            else
            {
                otdel.parent.subOtdels.Add(otdel);
            }
            //otdel.parent.subOtdels.Add(repo.Add(otdel));

            SelectedItem = otdel;
            //IsSelected = true;
        }

        //--------------------------------------------------------------------------------
        // Команда Удалить отдел
        //--------------------------------------------------------------------------------
        public ICommand DeleteCommand => new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommand);
        private bool CanDeleteCommand(object p) => true;
        private void OnDeleteCommandExecuted(object p)
        {
            //repo.DeleteOtdel(0);
        }
        #endregion



        public EditTablesWindowModel()
        {
            repo = new RepositoryOtdel();

            ListOtdel = new ObservableCollection<Otdel>(repo.Items);
            ListOtdel.CollectionChanged += ListOtdel_CollectionChanged;
        }

        private void ListOtdel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }
    }
}
