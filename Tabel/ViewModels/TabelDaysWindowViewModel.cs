using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class TabelDaysWindowViewModel : ViewModel
    {
        public TabelPerson TabPerson { get; set; }
        public List<typeDay> ListTypeDays { get; set; }
        private bool IsModify = false;
        private readonly RepositoryMSSQL<TabelPerson> repoTabelPerson;
        public bool IsSaved { get; set; } = false;
        public string CurrentDate { get; }

        public TabelDaysWindowViewModel()
        {

        }

        public TabelDaysWindowViewModel(RepositoryMSSQL<TabelPerson> repo,  int PersonId)
        {
            repoTabelPerson = new RepositoryMSSQL<TabelPerson>();
            TabPerson = repoTabelPerson.Items.FirstOrDefault(it => it.id == PersonId);
            RepositoryMSSQL<typeDay>  repoTypeDay = new RepositoryMSSQL<typeDay>(repoTabelPerson.GetDB());
            ListTypeDays = repoTypeDay.Items.ToList();

            // получение данных производственного календаря
            RepositoryCalendar repoCal = new RepositoryCalendar(); // AllRepo.GetRepoCalendar();
            var ListDays = repoCal.GetListDays(TabPerson.tabel.t_year, TabPerson.tabel.t_month);

            foreach (var item in TabPerson.TabelDays)
            {
                item.PropertyChanged += Person_PropertyChanged;
                item.CalendarTypeDay = ListDays[item.td_Day - 1].KindDay;
            }
            AnalizeOverWork();

            CurrentDate = App.ListMonth[TabPerson.tabel.t_month - 1].Name + " " + TabPerson.tabel.t_year;
        }


        //--------------------------------------------------------------------------------------
        // Событие изменения параметров дня
        //--------------------------------------------------------------------------------------
        private void Person_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // отслеживаем изменение часов
            if (e.PropertyName == "td_Hours")
            {
                //TabelPerson person = (sender as TabelDay).TabelPerson;
                AnalizeOverWork();
                IsModify = true;
            }
            if (e.PropertyName == "td_KindId")
            {
                IsModify = true;
            }


        }

        //--------------------------------------------------------------------------------------
        // Запись в базу данных
        //--------------------------------------------------------------------------------------
        public void SaveForm()
        {
            repoTabelPerson.Save();
            //repoTabel.Save();
            IsModify = false;
            IsSaved = true;
        }

        //--------------------------------------------------------------------------------------
        // Расчет лишних часов для всего месяца
        //--------------------------------------------------------------------------------------
        private void AnalizeOverWork()
        {
            List<TabelDay> ListDays = TabPerson.TabelDays.ToList();
            int nCntPermDays = TabPerson.PrevPermWorkCount + 1;

            decimal PrevHours;
            decimal? OverHours;

            for (int i = 0; i < ListDays.Count; i++)
            {
                if (i == 0)
                    // для первого дня берем предыдущий день из прошлого табеля
                    PrevHours = TabPerson.PrevDay is null ? 0 : (TabPerson.PrevDay.td_Hours - TabPerson.PrevDay.td_Hours2) ?? 0;
                else
                    // часы предыдущего дня
                    PrevHours = ListDays[i - 1].WhiteHours;

                OverHours = 0;

                if (ListDays[i].td_Hours == 0)
                    nCntPermDays = 0;

                if (nCntPermDays >= 7)
                {
                    // если проработано более 6 дней подряд
                    OverHours = ListDays[i].td_Hours;
                    nCntPermDays = 0;
                }
                else if (ListDays[i].CalendarTypeDay != TypeDays.Holyday)
                {
                    if (ListDays[i].td_Hours > 12)
                    {
                        OverHours = ListDays[i].td_Hours - 12;
                        //ListDays[i].td_Hours2 = ListDays[i].td_Hours - OverHours;
                        //ListDays[i].WhiteHours = 12;
                    }

                    if (PrevHours + ListDays[i].td_Hours > 20)
                        OverHours = PrevHours + ListDays[i].td_Hours - 20;

                }

                ListDays[i].td_Hours2 = OverHours;
                ListDays[i].OnPropertyChanged("WhiteHours");
                //ListDays[i].WhiteHours = (ListDays[i].td_Hours - OverHours) ?? 0;
                ListDays[i].VisibilityHours = OverHours > 0 ? Visibility.Visible : Visibility.Collapsed;
                nCntPermDays++;
            }
            TabPerson.OnPropertyChanged(nameof(TabPerson.OverWork));
        }


        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => IsModify;
        private void OnSaveCommandExecuted(object p)
        {
            SaveForm();
        }

        //--------------------------------------------------------------------------------
        // Событие Закрытие
        //--------------------------------------------------------------------------------
        public ICommand CloseCommand => new LambdaCommand(OnCloseCommandExecuted, CanCloseCommand);
        private bool CanCloseCommand(object p) => true;
        private void OnCloseCommandExecuted(object p)
        {
            if (IsModify == true)
            {
                MessageBoxResult res;
                res = MessageBox.Show("Сохранить измененные данные?", "Предупреждение",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (res == MessageBoxResult.Yes)
                    SaveForm();
                else if (res == MessageBoxResult.Cancel)
                {
                    (p as CancelEventArgs).Cancel = true;
                    return;
                }
            }

        }

        #endregion

    }
}
