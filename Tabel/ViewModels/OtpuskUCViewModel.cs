//using DocumentFormat.OpenXml.Drawing.Charts;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;

namespace Tabel.ViewModels
{
    internal class OtpuskUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        //private int _SelectMonth;
        private int _SelectYear;

        private readonly BaseModel db;
        private readonly RepositoryMSSQL<Personal> repoPersonal;
        private readonly RepositoryMSSQL<Otpusk> repoOtpusk;
        private readonly RepositoryMSSQL<OtpuskPerson> repoOtpuskPerson;

        //public ObservableCollection<OtpuskDays> ListDays { get; set; }
        //    = new ObservableCollection<OtpuskDays>() { 
        //    new OtpuskDays() { od_StartDate = new DateTime(2023, 1, 1), od_EndDate = new DateTime(2023,1,31) },
        //    new OtpuskDays() { od_StartDate = new DateTime(2023, 6, 1), od_EndDate = new DateTime(2023,6,30) },
        //    new OtpuskDays() { od_StartDate = new DateTime(2023, 11, 1), od_EndDate = new DateTime(2023,11,30)  },
        //};

        public Otpusk otpusk { get; set; }

        public OtpuskPerson SelectedPerson { get; set; }

        private bool IsModify;

        private ObservableCollection<OtpuskPerson> _ListOtpuskPerson;
        public ObservableCollection<OtpuskPerson> ListOtpuskPerson
        {
            get => _ListOtpuskPerson;
            set
            {
                if (_ListOtpuskPerson == value) return;

                if (_ListOtpuskPerson != null)
                {
                    foreach (var item in _ListOtpuskPerson)
                        foreach (var day in item.ListDays)
                            day.PropertyChanged -= Item_PropertyChanged;
                }

                _ListOtpuskPerson = value;
                if (_ListOtpuskPerson == null) return;

                foreach (var item in _ListOtpuskPerson)
                    foreach (var day in item.ListDays)
                        day.PropertyChanged += Item_PropertyChanged;

            }
        }



        //------------------------------------------------------------------------------------------------------------
        //  Конструктор
        //------------------------------------------------------------------------------------------------------------
        public OtpuskUCViewModel()
        {
            repoOtpusk = new RepositoryMSSQL<Otpusk>();
            db = repoOtpusk.GetDB();
            repoOtpuskPerson = new RepositoryMSSQL<OtpuskPerson>(db);
            repoPersonal = new RepositoryMSSQL<Personal>(db);
            IsModify = false;
        }


        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "md_kvalif_tarif"
            //    || e.PropertyName == "md_prem_otdel")
            //    return;
            //SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.AllDays));
            IsModify = true;
        }


        public bool ClosingFrom()
        {
            return IsModify;
        }


        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------
        public void OtdelChanged(Otdel SelectOtdel, int Year, int Month)
        {
            if (_SelectYear == Year && _SelectedOtdel == SelectOtdel)
                return;

            //_SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;
            ListOtpuskPerson  = null;

            if (SelectOtdel is null) return;

            if (_SelectedOtdel.ot_parent is null)
            {
                otpusk = repoOtpusk.Items.FirstOrDefault(it => it.o_year == Year
                    && it.o_otdelId == _SelectedOtdel.id);
                if (otpusk != null)
                    ListOtpuskPerson = new ObservableCollection<OtpuskPerson>(repoOtpuskPerson.Items
                        .Where(it => it.op_otpuskId == otpusk.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }
            else
            {
                otpusk = repoOtpusk.Items.FirstOrDefault(it => it.o_year == Year
                    && it.o_otdelId == _SelectedOtdel.ot_parent);
                if (otpusk != null)
                    ListOtpuskPerson = new ObservableCollection<OtpuskPerson>(repoOtpuskPerson.Items
                        .Where(it => it.op_otpuskId == otpusk.id && it.person.p_otdel_id == _SelectedOtdel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }

            //SetTypeDays();

            OnPropertyChanged(nameof(otpusk));
            OnPropertyChanged(nameof(ListOtpuskPerson));
        }

        public void SaveForm()
        {
            repoOtpuskPerson.Save();
            repoOtpusk.Save();
            IsModify = false;
        }


        #region Команды

        //--------------------------------------------------------------------------------
        // Событие редактирования отпуска
        //--------------------------------------------------------------------------------
        public ICommand EditOtpuskCommand => new LambdaCommand(OnEditOtpuskCommandExecuted, CanEditOtpuskCommand);
        private bool CanEditOtpuskCommand(object p) => true;
        private void OnEditOtpuskCommandExecuted(object p)
        {
            OtpuskDays od = (p as RoutedEventArgs).OriginalSource as OtpuskDays;

            SelectOtpuskWindow WinSelect = new SelectOtpuskWindow();

            FrameworkElement elem = ((p as RoutedEventArgs).Source) as FrameworkElement;
            Point pt = Mouse.GetPosition(elem);

            var trans = PresentationSource.FromVisual(elem).CompositionTarget.TransformFromDevice;
            Point pt2 = trans.Transform(elem.PointToScreen(pt));
            //Point pt2 = elem.PointToScreen(pt);

            WinSelect.Top = pt2.Y - WinSelect.Height / 2;
            WinSelect.Left = pt2.X - WinSelect.Width / 2;

            WinSelect.cal.DisplayDate = new DateTime(_SelectYear, od.od_StartDate.Month, 1);
            WinSelect.cal.SelectedDates.AddRange(od.od_StartDate, od.od_EndDate);

            WinSelect.cal.DisplayDateStart = new DateTime(_SelectYear, 1, 1);
            WinSelect.cal.DisplayDateEnd = new DateTime(_SelectYear + 1, 1, 31);

            if (WinSelect.ShowDialog() == true)
            {
                var SelectedDates = WinSelect.cal.SelectedDates.OrderBy(it => it);

                DateTime StartDate = SelectedDates.First();
                if (StartDate.Year != _SelectYear)
                {
                    MessageBox.Show($"Начальная дата должна быть в {_SelectYear} году", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                od.od_StartDate = StartDate;
                od.od_EndDate = SelectedDates.Last();
                od.OnPropertyChanged(nameof(od.CountDay));
                IsModify = true;
                SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.AllDays));

            }

        }

        //--------------------------------------------------------------------------------
        // Событие удаления отпуска
        //--------------------------------------------------------------------------------
        public ICommand DeleteOtpuskCommand => new LambdaCommand(OnDeleteOtpuskCommandExecuted, CanDeleteOtpuskCommand);
        private bool CanDeleteOtpuskCommand(object p) => true;
        private void OnDeleteOtpuskCommandExecuted(object p)
        {
            OtpuskDays od = (p as RoutedEventArgs).OriginalSource as OtpuskDays;
            SelectedPerson.ListDays.Remove(od);

            RepositoryMSSQL<OtpuskDays> repoDays = new RepositoryMSSQL<OtpuskDays>(db);
            repoDays.Delete(od);
            IsModify = true;
            SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.AllDays));
            //OnPropertyChanged(nameof(ListDays));
        }

        //--------------------------------------------------------------------------------
        // Событие добавление отпуска
        //--------------------------------------------------------------------------------
        public ICommand AddOtpuskCommand => new LambdaCommand(OnAddOtpuskCommandExecuted, CanAddOtpuskCommand);
        private bool CanAddOtpuskCommand(object p) => true;
        private void OnAddOtpuskCommandExecuted(object p)
        {
            SelectOtpuskWindow WinSelect = new SelectOtpuskWindow();
            FrameworkElement elem = ((p as RoutedEventArgs).OriginalSource) as FrameworkElement;
            Point pt = Mouse.GetPosition(elem);
            var trans = PresentationSource.FromVisual(elem).CompositionTarget.TransformFromDevice;
            Point pt2 = trans.Transform(elem.PointToScreen(pt));

            //pt = elem.PointToScreen(pt);
            WinSelect.Top = pt2.Y - WinSelect.Height / 2;
            WinSelect.Left = pt2.X - WinSelect.Width / 2;

            int month = int.Parse(elem.Tag.ToString());
            if (month > 12) month--;
            WinSelect.cal.DisplayDate = new DateTime(_SelectYear, month, 1);

            WinSelect.cal.DisplayDateStart = new DateTime(_SelectYear, 1, 1);
            WinSelect.cal.DisplayDateEnd = new DateTime(_SelectYear + 1, 1, 31);

            if (WinSelect.ShowDialog() == true)
            {
                var SelectedDates = WinSelect.cal.SelectedDates.OrderBy(it => it);
                DateTime StartDate = SelectedDates.First();
                if (StartDate.Year != _SelectYear )
                {
                    MessageBox.Show($"Начальная дата должна быть в {_SelectYear} году", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop) ;
                    return;
                }

                var NewOtpusk = new OtpuskDays()
                {
                    od_StartDate = StartDate,
                    od_EndDate = SelectedDates.Last()
                };
                IsModify = true;
                SelectedPerson.ListDays.Add(NewOtpusk);
                SelectedPerson.OnPropertyChanged(nameof(SelectedPerson.AllDays));

            }
        }


        //--------------------------------------------------------------------------------
        // Команда Создать 
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => _SelectedOtdel != null && _SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {
            if (otpusk != null)
            {
                if (MessageBox.Show("Текущая форма будет удалена. Продолжить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;
                repoOtpusk.Remove(otpusk);
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


            otpusk = new Otpusk();
            //otpusk.tr_UserId = App.CurrentUser.id;
            otpusk.o_otdelId = _SelectedOtdel.id;
            otpusk.o_year = _SelectYear;
            otpusk.o_DateCreate = DateTime.Now;
            otpusk.ListOtpuskPerson = new ObservableCollection<OtpuskPerson>();

            // количество дней в месяце
            //DateTime StartDay = new DateTime(_SelectYear, _SelectMonth, 1);

            // получение данных производственного календаря
            //RepositoryCalendar repo = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
            //var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                OtpuskPerson op = new OtpuskPerson();
                op.op_personId = item.id;
                //tp.person = item;
                op.ListDays = new ObservableCollection<OtpuskDays>();

                //for (DateTime IndexDate = StartDay; IndexDate.Month == _SelectMonth; IndexDate = IndexDate.AddDays(1))
                //foreach (var listItem in ListDays)
                //{
                //    OtpuskDays od = new OtpuskDays();
                //    //od.td_Day = listItem.Day;
                //    //od.OffDay = listItem.KindDay == TypeDays.Holyday;

                //    //if (IndexDate.DayOfWeek == DayOfWeek.Sunday || IndexDate.DayOfWeek == DayOfWeek.Saturday)
                //    //    td.OffDay = true;
                //    op.ListDays.Add(td);
                //}

                otpusk.ListOtpuskPerson.Add(op);
            }


            if (otpusk.ListOtpuskPerson.Count > 0)
                repoOtpusk.Add(otpusk, true);

            otpusk = repoOtpusk.Items.Where(it => it.id == otpusk.id)
                .Include(i => i.ListOtpuskPerson.Select(s => s.person))
                .FirstOrDefault();

            ListOtpuskPerson = new ObservableCollection<OtpuskPerson>(otpusk.ListOtpuskPerson);
            //SetTypeDays();

            OnPropertyChanged(nameof(ListOtpuskPerson));
            OnPropertyChanged(nameof(otpusk));

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
        // Команда Добавить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => _SelectedOtdel != null && otpusk != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            List<Models.Personal> ListPersonal = repoPersonal.Items
                .AsNoTracking()
                .Where(it => it.p_otdel_id == _SelectedOtdel.id && it.p_delete == false)
                .ToList();

            // составляем список добавленных людей
            foreach (var item in ListOtpuskPerson)
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
                //RepositoryCalendar repo = new RepositoryCalendar(db);
                //var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

                foreach (var item in ListPersonal)
                {
                    OtpuskPerson op = new OtpuskPerson();
                    op.op_otpuskId = otpusk.id;
                    op.person = repoPersonal.Items.FirstOrDefault(it => it.id == item.id);
                    op.ListDays = new ObservableCollection<OtpuskDays>();

                    //foreach (var listItem in ListDays)
                    //{
                    //    TransDay td = new TransDay();
                    //    tp.tp_TranspId = Transp.id;
                    //    td.td_Day = listItem.Day;
                    //    td.OffDay = listItem.KindDay == TypeDays.Holyday;
                    //    td.td_Kind = 0;
                    //    tp.TransDays.Add(td);
                    //}

                    repoOtpuskPerson.Add(op, true);
                    ListOtpuskPerson.Add(op);
                }

                OnPropertyChanged(nameof(ListOtpuskPerson));
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
                repoOtpuskPerson.Remove(SelectedPerson, true);
                ListOtpuskPerson.Remove(SelectedPerson);
                //IsModify = true;
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Распечатать
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintCommand);
        private bool CanPrintCommand(object p) => ListOtpuskPerson?.Count > 0;
        private void OnPrintCommandExecuted(object p)
        {
            try
            {
                using (XLWorkbook wb = new XLWorkbook(@"Отчеты\Отпуск.xlsx"))
                {

                    var ws = wb.Worksheets.Worksheet(1);

                    // Заполение шапки
                    ws.Cell("C3").Value = otpusk.o_number;
                    ws.Cell("D3").Value = otpusk.o_DateCreate;
                    ws.Cell("E3").Value = _SelectYear;

                    int curRow = 7;

                    foreach (var pers in ListOtpuskPerson)
                    {
                        foreach (var dates in pers.ListDays)
                        {
                            ws.Cell(curRow, 1).Value = pers.person.otdel.ot_name;
                            ws.Cell(curRow, 2).Value = pers.person.p_profession;
                            ws.Cell(curRow, 3).Value = pers.person.FIO;
                            ws.Cell(curRow, 5).Value = pers.person.p_tab_number;
                            ws.Cell(curRow, 6).Value = dates.CountDay;
                            ws.Cell(curRow, 7).Value = dates.od_StartDate;
                            ws.Row(curRow).InsertRowsBelow(1);
                            curRow++;
                        }
                    }

                    string TempFile = System.IO.Path.GetTempFileName();
                    TempFile = System.IO.Path.ChangeExtension(TempFile, "xlsx");
                    wb.SaveAs(TempFile);
                    Process.Start(TempFile);
                }

            }
            catch
            {
                MessageBox.Show("Не найден шаблон модели", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }






        #endregion


        public static bool IsOtpuskDay(DateTime date, List<OtpuskDays> OtpDays)
        {

            if (OtpDays != null)
            {
                foreach (var item in OtpDays)
                {
                    if (date >= item.od_StartDate && date <= item.od_EndDate)
                        return true;
                }
            }
            return false;
        }

    }
}
