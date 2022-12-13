﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Component.TransPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{
    internal class TransportUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        private readonly BaseModel db;
        private readonly RepositoryMSSQL<Personal> repoPersonal;
        private readonly RepositoryMSSQL<Transport> repoTransp;
        private readonly RepositoryMSSQL<TransPerson> repoTransPerson;

        public Transport Transp { get; set; }

        private ObservableCollection<TransPerson> _ListTransPerson;
        public ObservableCollection<TransPerson> ListTransPerson 
        {
            get => _ListTransPerson;
            set
            {
                if (_ListTransPerson == value) return;

                if (_ListTransPerson != null)
                {
                    foreach (var item in _ListTransPerson)
                        foreach (var day in item.TransDays)
                            day.PropertyChanged -= Item_PropertyChanged;
                }

                _ListTransPerson = value;
                if (_ListTransPerson == null) return;

                foreach (var item in _ListTransPerson)
                    foreach (var day in item.TransDays)
                        day.PropertyChanged += Item_PropertyChanged;

            }
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "md_kvalif_tarif"
            //    || e.PropertyName == "md_prem_otdel")
            //    return;
            IsModify = true;
        }


        public TransPerson SelectedPerson { get; set; }

        private bool IsModify;

        public TransportUCViewModel()
        {
            repoPersonal = new RepositoryMSSQL<Personal>();
            db = repoPersonal.GetDB();
            repoTransp = new RepositoryMSSQL<Transport>(db);
            repoTransPerson = new RepositoryMSSQL<TransPerson>(db);
            IsModify = false;
        }

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать 
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (Transp != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                repoTransp.Remove(Transp);
            }

            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>(db);// AllRepo.GetRepoAllOtdels();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == _SelectedOtdel.id).Select(s => s.id).ToList();

            List<Personal> PersonsFromOtdel = repoPersonal.Items
                .AsNoTracking()
                .Where(it => (it.p_otdel_id == _SelectedOtdel.id || listOtdels.Contains(it.p_otdel_id.Value)) && it.p_delete == false)
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name)
                .ToList();

            if (PersonsFromOtdel?.Count == 0)
            {
                MessageBox.Show("В вашем отделе нет сотрудников. Форма не создана.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            Transp = new Transport();
            Transp.tr_UserId = App.CurrentUser.id;
            Transp.tr_OtdelId = _SelectedOtdel.id;
            Transp.tr_Month = _SelectMonth;
            Transp.tr_Year = _SelectYear;
            Transp.tr_DateCreate = DateTime.Now;
            Transp.TransportPerson = new ObservableCollection<TransPerson>();

            // количество дней в месяце
            //DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                TransPerson tp = new TransPerson();
                tp.tp_PersonId = item.id;
                //tp.person = item;
                tp.TransDays = new List<TransDay>();

                //for (DateTime IndexDate = StartDay; IndexDate.Month == _SelectMonth; IndexDate = IndexDate.AddDays(1))
                foreach (var listItem in ListDays)
                {
                    TransDay td = new TransDay();
                    td.td_Day = listItem.Day;
                    td.OffDay = listItem.KindDay == TypeDays.Holyday;

                    //if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
                    //    td.OffDay = true;
                    tp.TransDays.Add(td);
                }

                Transp.TransportPerson.Add(tp);
            }


            if (Transp.TransportPerson.Count > 0)
                repoTransp.Add(Transp, true);

            Transp = repoTransp.Items.Where(it => it.id == Transp.id)
                .Include(i => i.TransportPerson.Select(s => s.person))
                .FirstOrDefault();

            ListTransPerson = new ObservableCollection<TransPerson>(Transp.TransportPerson);
            //SetTypeDays();

            OnPropertyChanged(nameof(ListTransPerson));
            OnPropertyChanged(nameof(Transp));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => /*_SelectedOtdel != null && Transp != null*/ IsModify;
        private void OnSaveCommandExecuted(object p)
        {
            SaveForm();
        }


        //--------------------------------------------------------------------------------
        // Распечатать
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintdCommand);
        private bool CanPrintdCommand(object p) => Transp != null;
        private void OnPrintCommandExecuted(object p)
        {
            using (XLWorkbook wb = new XLWorkbook(@"Отчеты\График ЛТ.xlsx"))
            {
                var ws = wb.Worksheets.Worksheet(1);

                ws.Cell(3,8).Value = "Использование личного транспорта сотрудниками участка " + Transp.otdel.ot_name;
                ws.Cell(3, 3).Value = Transp.tr_DateCreate.ToString("dd.MM.yyyy");
                ws.Cell(3, 35).Value = (new DateTime(Transp.tr_Year, Transp.tr_Month, 1)).ToString("`MMMM yyyy");
                ws.Cell(10, 30).Value = "Составил: " + App.CurrentUser.u_fio;

                int ColNum = 4;
                int NumWeek = 0;
                int OldWeek = 0;
                int StartColumn = ColNum;
                GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);
                DateTime startDate = new DateTime(_SelectYear, _SelectMonth, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                while (startDate <= endDate)
                {
                    NumWeek = cal.GetWeekOfYear(startDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    if (startDate.Day == 1 || startDate.DayOfWeek == DayOfWeek.Monday)
                        ws.Cell(5, ColNum).Value = NumWeek;

                    if (OldWeek != 0 && NumWeek != OldWeek)
                    {
                        ws.Range(5, StartColumn, 5, ColNum - 1).Merge();
                        ws.Cell(5, StartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        StartColumn = ColNum;
                    }

                    OldWeek = NumWeek;
                    ws.Cell(6, ColNum).Value = startDate;
                    ws.Cell(7, ColNum).Value = startDate.ToString("ddd");
                    startDate = startDate.AddDays(1);
                    ColNum++;
                    //ColWeek++;
                }

                ws.Range(5, StartColumn, 5, ColNum - 1).Merge();
                ws.Cell(5, StartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                int RowNum = 8;
                ws.Row(RowNum).InsertRowsBelow(ListTransPerson.Count() - 1);
                foreach (var item in ListTransPerson)
                {
                    ws.Cell(RowNum, 2).Value = item.person.ShortFIO;
                    ColNum = 4;
                    foreach (var day in item.TransDays)
                    {
                        if (day.td_Kind == KindTrans.Used)
                            ws.Cell(RowNum, ColNum).Value = day.td_Kind;
                        if (day.OffDay)
                            ws.Cell(RowNum, ColNum).Style.Fill.BackgroundColor = XLColor.LightGray;
                        ColNum++;
                    }
                    ws.Cell(RowNum, 35).Value = item.ItogDays;
                    ws.Cell(RowNum, 36).Value = item.tp_tarif;
                    ws.Cell(RowNum, 37).Value = item.Summa;
                    ws.Cell(RowNum, 38).Value = item.tp_Kompens;
                    ws.Cell(RowNum, 39).Value = item.Itogo;

                    RowNum++;
                }

                string TempFile = FileOperation.GenerateTempFileNameWithDelete("TempTransp.xlsx");
                wb.SaveAs(TempFile);
                Process.Start(TempFile);
            }

        }

        //--------------------------------------------------------------------------------
        // 
        //--------------------------------------------------------------------------------
        public ICommand SelectKindCommand => new LambdaCommand(OnSelectKindCommandExecuted, CanSelectKindCommand);
        private bool CanSelectKindCommand(object p) => true;
        private void OnSelectKindCommandExecuted(object p)
        {

        }


        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => _SelectedOtdel != null && Transp != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            List<Models.Personal> ListPersonal = repoPersonal.Items
                .AsNoTracking()
                .Where(it => it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false)
                .ToList();

            // составляем список добавленных людей
            foreach (var item in ListTransPerson)
            {
                var pers = ListPersonal.FirstOrDefault(it => it.id == item.person.id);
                if (pers != null)
                    ListPersonal.Remove(pers);
            }

            if (ListPersonal.Count == 0)
            {
                MessageBox.Show("Новых людей для отдела не обнаружено.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (MessageBox.Show($"Найдено людей: {ListPersonal.Count}. Добавлять?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                RepositoryCalendar repo = new RepositoryCalendar(db);
                var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

                foreach (var item in ListPersonal)
                {
                    TransPerson tp = new TransPerson();
                    tp.person = repoPersonal.Items.FirstOrDefault(it => it.id == item.id);
                    tp.TransDays = new ObservableCollection<TransDay>();

                    foreach (var listItem in ListDays)
                    {
                        TransDay td = new TransDay();
                        tp.tp_TranspId = Transp.id;
                        td.td_Day = listItem.Day;
                        td.OffDay = listItem.KindDay == TypeDays.Holyday;
                        td.td_Kind = 0;
                        tp.TransDays.Add(td);
                    }

                    repoTransPerson.Add(tp, true);
                    ListTransPerson.Add(tp);
                }

                OnPropertyChanged(nameof(ListTransPerson));
                //IsModify = true;
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Удалить сотрудника
        //--------------------------------------------------------------------------------
        public ICommand DeletePersonCommand => new LambdaCommand(OnDeletePersonCommandExecuted, CanDeletePersonCommand);
        private bool CanDeletePersonCommand(object p) => SelectedPerson != null;
        private void OnDeletePersonCommandExecuted(object p)
        {
            if (MessageBox.Show($"Удалить {SelectedPerson.person.FIO}?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                repoTransPerson.Remove(SelectedPerson, true);
                ListTransPerson.Remove(SelectedPerson);
                //IsModify = true;
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Отметить использование 
        //--------------------------------------------------------------------------------
        public ICommand IsSelectCommand => new LambdaCommand(OnIsSelectCommandExecuted, CanIsSelectCommand);
        private bool CanIsSelectCommand(object p) => true;
        private void OnIsSelectCommandExecuted(object p)
        {
            TransPanel panel =(TransPanel)( (p as RoutedEventArgs).Source);
            //TransPanel panel = p.Source as TransPanel;

            foreach(TransDay item in panel.SelectedItems)
            {
                item.td_Kind = KindTrans.Used;
                item.OnPropertyChanged(nameof(item.td_Kind));
            }
            panel.SelectedItems.Clear();
        }


        //--------------------------------------------------------------------------------
        // Команда Снять отметки использование 
        //--------------------------------------------------------------------------------
        public ICommand IsUnSelectCommand => new LambdaCommand(OnIsUnSelectCommandExecuted, CanIsUnSelectCommand);
        private bool CanIsUnSelectCommand(object p) => true;
        private void OnIsUnSelectCommandExecuted(object p)
        {
            TransPanel panel =(TransPanel)( (p as RoutedEventArgs).Source);
            //TransPanel panel = p.Source as TransPanel;

            foreach(TransDay item in panel.SelectedItems)
            {
                item.td_Kind = KindTrans.None;
                item.OnPropertyChanged(nameof(item.td_Kind));
            }
            panel.SelectedItems.Clear();
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
            ListTransPerson = null;

            if (SelectOtdel is null) return;

            if (_SelectedOtdel.ot_parent is null)
            {
                Transp = repoTransp.Items.FirstOrDefault(it => it.tr_Year == Year
                    && it.tr_Month == Month
                    && it.tr_OtdelId == _SelectedOtdel.id);
                if (Transp != null)
                    ListTransPerson = new ObservableCollection<TransPerson>(repoTransPerson.Items
                        .Where(it => it.tp_TranspId == Transp.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }
            else
            {
                Transp = repoTransp.Items.FirstOrDefault(it => it.tr_Year == Year
                    && it.tr_Month == Month
                    && it.tr_OtdelId == _SelectedOtdel.ot_parent);
                if (Transp != null)
                    ListTransPerson = new ObservableCollection<TransPerson>(repoTransPerson.Items
                        .Where(it => it.tp_TranspId == Transp.id && it.person.p_otdel_id == _SelectedOtdel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }

            SetTypeDays();

            OnPropertyChanged(nameof(Transp));
            OnPropertyChanged(nameof(ListTransPerson));

        }

        //--------------------------------------------------------------------------------------
        // расстановка топв дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (Transp is null) return;

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar(db);
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            //RepositoryMSSQL<WorkCalendar> repoDays = new RepositoryMSSQL<WorkCalendar>();
            // количество дней в месяце
            //DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            //List<WorkCalendar> cal = repoDays.Items.AsNoTracking().Where(it => it.cal_date.Year == _SelectYear && it.cal_date.Month == _SelectMonth).ToList();

            foreach (var item in ListTransPerson)
            {
                // расставляем выходные по каледнарю

                int i = 0;
                foreach ( var day in item.TransDays)
                {
                    day.OffDay = ListDays[i].KindDay == TypeDays.Holyday;
                    //DayOfWeek week = new DateTime(_SelectYear, _SelectMonth, day.td_Day).DayOfWeek;
                    //if (week == DayOfWeek.Sunday || week == DayOfWeek.Saturday)
                    //{
                    //    day.OffDay = true;
                    //}
                    i++;
                }

                //// дополняем измененные дни
                //foreach(var day in cal)
                //{
                //    TransDay td = item.TransDays.FirstOrDefault(it => it.td_Day == day.cal_date.Day);
                //    td.OffDay = day.cal_type == TypeDays.Holyday;
                //}

            }

        }

        public bool ClosingFrom()
        {
            return IsModify;
        }

        public void SaveForm()
        {
            repoTransPerson.Save();
            repoTransp.Save();
            IsModify = false;
        }
    }
}
