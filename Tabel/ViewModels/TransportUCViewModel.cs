﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models2;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class TransportUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        private readonly RepositoryMSSQL<Personal> repoPersonal = new RepositoryMSSQL<Personal>();
        private readonly RepositoryMSSQL<Transport> repoTransp = new RepositoryMSSQL<Transport>();
        private readonly RepositoryMSSQL<TransPerson> repoTrnaspPersonal = new RepositoryMSSQL<TransPerson>();

        public Transport Transp { get; set; }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать 
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null;
        private void OnCreateCommandExecuted(object p)
        {
            if (Transp != null)
            {
                repoTransp.Remove(Transp);
            }

            List<Personal> PersonsFromOtdel = repoPersonal.Items.AsNoTracking().Where(it => it.p_otdel_id == _SelectedOtdel.id).ToList();
    

            Transp = new Transport();
            Transp.tr_UserId = App.CurrentUser.id;
            Transp.tr_OtdelId = _SelectedOtdel.id;
            Transp.tr_Month = _SelectMonth;
            Transp.tr_Year = _SelectYear;
            Transp.tr_DateCreate = DateTime.Now;
            Transp.TransportPerson = new ObservableCollection<TransPerson>();

            // количество дней в месяце
            DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                TransPerson tp = new TransPerson();
                tp.tp_PersonId = item.id;
                //tp.person = item;
                tp.TransDays = new List<TransDay>();

                for (DateTime IndexDate = StartDay; IndexDate.Month == _SelectMonth; IndexDate = IndexDate.AddDays(1))
                {
                    TransDay td = new TransDay();
                    td.td_Day = IndexDate.Day;
                    //if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
                    //    td.OffDay = true;
                    tp.TransDays.Add(td);
                }

                Transp.TransportPerson.Add(tp);
            }

            SetTypeDays();

            if (Transp.TransportPerson.Count > 0)
                repoTransp.Add(Transp, true);

            Transp = repoTransp.Items.Where(it => it.id == Transp.id)
                .Include(i => i.TransportPerson.Select(s => s.person))
                .FirstOrDefault();

            OnPropertyChanged(nameof(Transp));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => true;
        private void OnSaveCommandExecuted(object p)
        {
            repoTransp.Save();
        }

        //--------------------------------------------------------------------------------
        // 
        //--------------------------------------------------------------------------------
        public ICommand SelectKindCommand => new LambdaCommand(OnSelectKindCommandExecuted, CanSelectKindCommand);
        private bool CanSelectKindCommand(object p) => true;
        private void OnSelectKindCommandExecuted(object p)
        {

        }
        #endregion

        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------

        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;

            if (SelectOtdel is null) return;

            Transp = repoTransp.Items
                .Where(it => it.tr_Year == _SelectYear
                && it.tr_Month == _SelectMonth
                && it.tr_OtdelId == _SelectedOtdel.id)
                .Include(inc => inc.TransportPerson)
                .FirstOrDefault();
            
            //List<Personal> PersonsFromOtdel = repoPersonal.Items.Where(it => it.p_otdel_id == _SelectedOtdel.id).ToList();

            SetTypeDays();
            OnPropertyChanged(nameof(Transp));

        }

        //--------------------------------------------------------------------------------------
        // расстановка топв дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (Transp is null) return;

            RepositoryMSSQL<WorkCalendar> repoDays = new RepositoryMSSQL<WorkCalendar>();
            // количество дней в месяце
            DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            List<WorkCalendar> cal = repoDays.Items.AsNoTracking().Where(it => it.cal_date.Year == _SelectYear && it.cal_date.Month == _SelectMonth).ToList();

            foreach (var item in Transp.TransportPerson)
            {
                // расставляем выходные по каледнарю
                foreach ( var day in item.TransDays)
                {
                    DayOfWeek week = new DateTime(_SelectYear, _SelectMonth, day.td_Day).DayOfWeek;
                    if (week == DayOfWeek.Sunday || week == DayOfWeek.Saturday)
                    {
                        day.OffDay = true;
                    }
                }

                // дополняем измененные дни
                foreach(var day in cal)
                {
                    TransDay td = item.TransDays.FirstOrDefault(it => it.td_Day == day.cal_date.Day);
                    td.OffDay = day.cal_type == TypeDays.Holyday;
                }

            }

        }


    }
}