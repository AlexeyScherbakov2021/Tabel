using ClosedXML.Excel;
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

        public string[] ListKind { get; set; } //= { "1см", "2см", "В", "О" };

        // Текщий график смен
        public Smena SmenaShedule { get; set; }
        public ObservableCollection<SmenaPerson> ListSmenaPerson { get; set; }

        //private DateTime _CurrentDate;

        public SmenaUCViewModel()
        {
            repoPersonal = new RepositoryMSSQL<Personal>();
            db = repoPersonal.GetDB();
            repoSmena = new RepositoryMSSQL<Smena>(db);
            repoSmenaPersonal = new RepositoryMSSQL<SmenaPerson>(db);
            ListKind = EnumToString.ListSmenaKind.ToArray();
        }


        public bool ClosingFrom()
        {
            return false;
        }

        public void SaveForm()
        {
            repoSmenaPersonal.Save();
            repoSmena.Save();
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
                .Where(it => (it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false) || listOtdels.Contains(it.p_otdel_id.Value))
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
        private bool CanSaveCommand(object p) => _SelectedOtdel != null && SmenaShedule != null ;
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
