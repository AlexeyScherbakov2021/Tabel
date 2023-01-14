﻿using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using System.Globalization;
using System.IO;
using Tabel.Component.TransPanel;
using Tabel.Component.SmenaPanel;
using System.Windows.Controls;

namespace Tabel.ViewModels
{
    internal class SmenaUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;

        private readonly BaseModel db;
        private readonly RepositoryMSSQL<Personal> repoPersonal;
        private readonly RepositoryMSSQL<Smena> repoSmena;
        private readonly RepositoryMSSQL<SmenaPerson> repoSmenaPersonal;
        private readonly RepositoryMSSQL<OtpuskPerson> repoOtpuskPerson;

        public string[] ListKind { get; set; } //= { "1см", "2см", "В", "О" };

        // Текщий график смен
        public Smena SmenaShedule { get; set; }
        private ObservableCollection<SmenaPerson> _ListSmenaPerson;
        public ObservableCollection<SmenaPerson> ListSmenaPerson 
        {
            get => _ListSmenaPerson;
            set
            {
                if (_ListSmenaPerson == value) return;

                if (_ListSmenaPerson != null)
                {
                    foreach (var item in _ListSmenaPerson)
                        foreach(var day in item.SmenaDays)
                            day.PropertyChanged -= Item_PropertyChanged;
                }

                _ListSmenaPerson = value;
                if (_ListSmenaPerson == null) return;

                foreach (var item in _ListSmenaPerson)
                    foreach (var day in item.SmenaDays)
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



        public SmenaPerson SelectedPerson { get;set;}

        private bool IsModify;


        //private DateTime _CurrentDate;

        //--------------------------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------------------------
        public SmenaUCViewModel()
        {
            repoPersonal = new RepositoryMSSQL<Personal>();
            db = repoPersonal.GetDB();
            repoOtpuskPerson = new RepositoryMSSQL<OtpuskPerson>(db);
            repoSmena = new RepositoryMSSQL<Smena>(db);
            repoSmenaPersonal = new RepositoryMSSQL<SmenaPerson>(db);
            ListKind = EnumToString.ListSmenaKind.ToArray();
            IsModify = false;
        }


        public bool ClosingFrom()
        {
            return IsModify;
        }

        public void SaveForm()
        {
            repoSmenaPersonal.Save();
            repoSmena.Save();
            IsModify = false;
        }


        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (SmenaShedule != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                repoSmena.Remove(SmenaShedule);
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

            SmenaShedule = new Smena();
            SmenaShedule.sm_UserId = App.CurrentUser.id;
            SmenaShedule.sm_OtdelId = _SelectedOtdel.id;
            SmenaShedule.sm_Month = _SelectMonth;
            SmenaShedule.sm_Year = _SelectYear;
            SmenaShedule.sm_DateCreate = DateTime.Now;
            SmenaShedule.SmenaPerson = new ObservableCollection<SmenaPerson>();

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            // количество дней в месяце
            //DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                List<OtpuskDays> OtpDays = repoOtpuskPerson
                    .Items
                    .AsNoTracking()
                    .FirstOrDefault(it => it.person.id == item.id && it.otpusk.o_year == _SelectYear)?.ListDays.ToList();

                SmenaPerson sp = new SmenaPerson();
                sp.sp_PersonId = item.id;
                sp.SmenaDays = new List<SmenaDay>();

                foreach (var listItem in ListDays)
                {
                    SmenaDay sd = new SmenaDay();
                    sd.sd_Day = listItem.Day;
                    if (listItem.KindDay == TypeDays.Holyday)
                    {
                        sd.sd_Kind = SmenaKind.DayOff;
                        sd.OffDay = true;
                    }
                    else
                    {
                        sd.sd_Kind = SmenaKind.First;
                        sd.OffDay = false;
                    }

                    if (OtpDays != null && OtpuskUCViewModel.IsOtpuskDay(new DateTime(_SelectYear, _SelectMonth, sd.sd_Day), OtpDays))
                        sd.sd_Kind = SmenaKind.Otpusk;

                    sp.SmenaDays.Add(sd);
                }

                SmenaShedule.SmenaPerson.Add(sp);
            }

            if (SmenaShedule.SmenaPerson.Count > 0)
                repoSmena.Add(SmenaShedule, true);

            SmenaShedule = repoSmena.Items
                .Where(it => it.id == SmenaShedule.id)
                .Include(inc => inc.SmenaPerson.Select(s => s.personal))
                .FirstOrDefault();

            if(SmenaShedule != null)
                ListSmenaPerson = new ObservableCollection<SmenaPerson>(SmenaShedule.SmenaPerson);

            OnPropertyChanged(nameof(ListSmenaPerson));
            OnPropertyChanged(nameof(SmenaShedule));

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => /*_SelectedOtdel != null && SmenaShedule != null */ IsModify;
        private void OnSaveCommandExecuted(object p)
        {
            SaveForm();
        }

        //--------------------------------------------------------------------------------
        // Распечатать
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintdCommand);
        private bool CanPrintdCommand(object p) => SmenaShedule != null;
        private void OnPrintCommandExecuted(object p)
        {
            using (XLWorkbook wb = new XLWorkbook(@"Отчеты\График смен.xlsx"))
            {
                var ws = wb.Worksheets.Worksheet(1);

                ws.Cell("P3").Value = SmenaShedule.otdel.ot_name;
                ws.Cell("C8").Value = SmenaShedule.sm_Number;
                ws.Cell("D8").Value = SmenaShedule.sm_DateCreate.ToString("dd.MM.yyyy");
                ws.Cell("K7").Value = "График смен - " + App.ListMonth.First(it => it.Number == SmenaShedule.sm_Month).Name;
                DateTime startDate = new DateTime(_SelectYear, _SelectMonth, 1);
                ws.Cell("AG9").Value = startDate.ToString("dd.MM.yyyy");
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                ws.Cell("AJ9").Value = endDate.ToString("dd.MM.yyyy");
                ws.Cell("AA15").Value = "Составил: " + App.CurrentUser.u_fio;

                int ColNum = 5;
                //int korr = (int)new DateTime(_SelectYear, 1, 1).DayOfWeek;
                //korr = korr <= 1 ? korr = -2 : korr - 9;
                //int ColWeek = 5;
                int NumWeek = 0;
                int OldWeek = 0;
                int StartColumn = ColNum;
                GregorianCalendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);

                while (startDate <= endDate)
                {
                    //NumWeek = (startDate.DayOfYear + korr) / 7 + 1;
                    NumWeek = cal.GetWeekOfYear(startDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                    if(startDate.Day == 1 || startDate.DayOfWeek == DayOfWeek.Monday)
                        ws.Cell(11, ColNum).Value = NumWeek;

                    if(OldWeek != 0 && NumWeek != OldWeek)
                    {
                        ws.Range(11, StartColumn, 11, ColNum - 1).Merge();
                        ws.Cell(11, StartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        StartColumn = ColNum;
                    }

                    OldWeek = NumWeek;
                    ws.Cell(12, ColNum).Value = startDate;
                    startDate = startDate.AddDays(1);
                    ColNum++;
                    //ColWeek++;
                }
                ws.Range(11, StartColumn, 11, ColNum - 1).Merge();
                ws.Cell(11, StartColumn).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                int RowNum = 13;
                ws.Row(RowNum).InsertRowsBelow(ListSmenaPerson.Count() - 1);
                foreach (var item in ListSmenaPerson)
                {
                    ws.Cell(RowNum, 3).Value = item.personal.ShortFIO;
                    ColNum = 5;
                    foreach(var day in item.SmenaDays)
                    {
                        ws.Cell(RowNum, ColNum).Value = EnumToString.SmenaKindToString(day.sd_Kind);
                        if(day.sd_Kind == SmenaKind.DayOff)
                            ws.Cell(RowNum, ColNum).Style.Fill.BackgroundColor = XLColor.LightGray;
                        ColNum++;
                    }

                    RowNum++;
                }

                string TempFile = FileOperation.GenerateTempFileNameWithDelete("TempGrSmen.xlsx");
                wb.SaveAs(TempFile);
                Process.Start(TempFile);
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => _SelectedOtdel != null && SmenaShedule != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            List<Models.Personal> ListPersonal = repoPersonal.Items
                .AsNoTracking()
                .Where(it => it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false)
                .ToList();

            // составляем список добавленных людей
            foreach (var item in ListSmenaPerson)
            {
                var pers = ListPersonal.FirstOrDefault(it => it.id == item.personal.id);
                if (pers != null)
                    ListPersonal.Remove(pers);
            }

            if (ListPersonal.Count == 0)
            {
                MessageBox.Show("Новых людей для отдела не обнаружено.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (MessageBox.Show($"Найдено людей: {ListPersonal.Count}. Добавлять?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                RepositoryCalendar repo = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
                var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

                foreach (var item in ListPersonal)
                {
                    List<OtpuskDays> OtpDays = repoOtpuskPerson
                        .Items
                        .AsNoTracking()
                        .FirstOrDefault(it => it.person.id == item.id && it.otpusk.o_year == _SelectYear)?.ListDays.ToList();

                    SmenaPerson sp = new SmenaPerson();
                    //tp.tp_person_id = item.id;
                    sp.personal = repoPersonal.Items.FirstOrDefault(it => it.id == item.id);
                    sp.SmenaDays = new ObservableCollection<SmenaDay>();

                    foreach (var listItem in ListDays)
                    {
                        SmenaDay sd = new SmenaDay();
                        sp.sp_SmenaId = SmenaShedule.id;
                        sd.sd_Day = listItem.Day;
                        sd.OffDay = listItem.KindDay == TypeDays.Holyday;
                        sd.sd_Kind = sd.OffDay ? SmenaKind.DayOff : SmenaKind.First;

                        if (OtpDays != null && OtpuskUCViewModel.IsOtpuskDay(new DateTime(_SelectYear, _SelectMonth, sd.sd_Day), OtpDays))
                            sd.sd_Kind = SmenaKind.Otpusk;

                        sp.SmenaDays.Add(sd);
                    }

                    repoSmenaPersonal.Add(sp, true);
                    ListSmenaPerson.Add(sp);
                }

                OnPropertyChanged(nameof(ListSmenaPerson));
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
            if (MessageBox.Show($"Удалить {SelectedPerson.personal.FIO}?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                repoSmenaPersonal.Remove(SelectedPerson, true);
                ListSmenaPerson.Remove(SelectedPerson);
                //IsModify = true;
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Конекстного меню выбора смены 
        //--------------------------------------------------------------------------------
        public ICommand IsSelectCommand => new LambdaCommand(OnIsSelectCommandExecuted, CanIsSelectCommand);
        private bool CanIsSelectCommand(object p) => true;
        private void OnIsSelectCommandExecuted(object p)
        {
            SmenaPanel panel = (SmenaPanel)((p as RoutedEventArgs).Source);
            SmenaKind smena = (SmenaKind)((p as RoutedEventArgs).OriginalSource as MenuItem).Tag;
            //TransPanel panel = p.Source as TransPanel;

            foreach (SmenaDay item in panel.SelectedItems)
            {
                if (item.OffDay && smena != SmenaKind.Otpusk)
                    item.sd_Kind = SmenaKind.DayOff;
                else
                    item.sd_Kind = smena;

                item.OnPropertyChanged(nameof(item.sd_Kind));
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
            ListSmenaPerson = null;


            if (SelectOtdel is null) return;


            if (_SelectedOtdel.ot_parent is null)
            {
                SmenaShedule = repoSmena.Items
                .Where(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == _SelectedOtdel.id)
                .Include(inc => inc.SmenaPerson)
                .FirstOrDefault();
                if (SmenaShedule != null)
                    ListSmenaPerson = new ObservableCollection<SmenaPerson>(repoSmenaPersonal.Items
                        .Where(it => it.sp_SmenaId == SmenaShedule.id)
                        .OrderBy(o => o.personal.p_lastname)
                        .ThenBy(o => o.personal.p_name)
                        );
            }
            else
            {
                SmenaShedule = repoSmena.Items
                .Where(it => it.sm_Year == _SelectYear
                && it.sm_Month == _SelectMonth
                && it.sm_OtdelId == _SelectedOtdel.ot_parent)
                .Include(inc => inc.SmenaPerson)
                .FirstOrDefault();
                if (SmenaShedule != null)
                    ListSmenaPerson = new ObservableCollection<SmenaPerson>(repoSmenaPersonal.Items
                        .Where(it => it.sp_SmenaId == SmenaShedule.id && it.personal.p_otdel_id == _SelectedOtdel.id)
                        .OrderBy(o => o.personal.p_lastname)
                        .ThenBy(o => o.personal.p_name)
                        );
            }

            SetTypeDays();

            OnPropertyChanged(nameof(ListSmenaPerson));
            OnPropertyChanged(nameof(SmenaShedule));

        }

        //--------------------------------------------------------------------------------------
        // расстановка топв дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (SmenaShedule is null) return;

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            //RepositoryMSSQL<WorkCalendar> repoDays = new RepositoryMSSQL<WorkCalendar>();
            //// количество дней в месяце
            //DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            //List<WorkCalendar> cal = repoDays.Items.AsNoTracking().Where(it => it.cal_date.Year == _SelectYear && it.cal_date.Month == _SelectMonth).ToList();

            foreach (var item in ListSmenaPerson)
            {
                // расставляем выходные по каледнарю


                int i = 0;
                foreach (var day in item.SmenaDays)
                    day.OffDay = ListDays[i++].KindDay == TypeDays.Holyday;
                //{
                //    DayOfWeek week = new DateTime(_SelectYear, _SelectMonth, day.sd_Day).DayOfWeek;
                //    if (week == DayOfWeek.Sunday || week == DayOfWeek.Saturday)
                //    {
                //        day.OffDay = true;
                //    }
                //}

                //// дополняем измененные дни
                //foreach (var day in cal)
                //{
                //    SmenaDay sd = item.SmenaDays.FirstOrDefault(it => it.sd_Day == day.cal_date.Day);
                //    sd.OffDay = day.cal_type == TypeDays.Holyday;
                //}

            }

        }

    }
}
