using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SqlTypes;
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
using Tabel.Views.PrintForm;

namespace Tabel.ViewModels
{
    internal class TabelUCViewModel : ViewModel, IBaseUCViewModel
    {
        private RepositoryMSSQL<Personal> repoPersonal = AllRepo.GetRepoPersonal();
        private readonly RepositoryMSSQL<WorkTabel> repoTabel = AllRepo.GetRepoTabel();
        private readonly RepositoryMSSQL<typeDay> repoTypeDay = AllRepo.GetRepoTypeDay();
        private readonly RepositoryMSSQL<TabelPerson> repoTabelPerson = AllRepo.GetRepoTabelPerson();

        public WorkTabel Tabel { get; set; }
        public IEnumerable<typeDay> ListTypeDays { get; set; }
        public ObservableCollection<TabelPerson> ListTabelPerson { get; set; }

        public Otdel SelectedOtdel { get; set; }
        private int _SelectMonth;
        private int _SelectYear;

        #region Команды
        //--------------------------------------------------------------------------------
        // Команда Создать график
        //--------------------------------------------------------------------------------
        public ICommand CreateCommand => new LambdaCommand(OnCreateCommandExecuted, CanCreateCommand);
        private bool CanCreateCommand(object p) => SelectedOtdel != null && SelectedOtdel.ot_parent is null;
        private void OnCreateCommandExecuted(object p)
        {

            if (Tabel != null)
            {
                if (MessageBox.Show("Текущий табель будет удален. Подтверждаете?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    return;

                repoTabel.Remove(Tabel);
            }

            // получение данных производственного календаря
            RepositoryCalendar repo = AllRepo.GetRepoCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);


            RepositoryMSSQL<Otdel> repoOtdel = AllRepo.GetRepoAllOtdels();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == SelectedOtdel.id).Select(s => s.id).ToList();

            // получение сотрудников отдела
            repoPersonal = AllRepo.GetRepoPersonal();

            List<Personal> PersonsFromOtdel = repoPersonal.Items
                .AsNoTracking()
                .Where(it => (it.p_otdel_id == SelectedOtdel.id && it.p_delete == false) || listOtdels.Contains(it.p_otdel_id.Value))
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name)
                .ToList();

            if(PersonsFromOtdel?.Count == 0)
            {
                MessageBox.Show("В вашем отделе нет сотрудников. Табель не создан.","Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Tabel = new WorkTabel();
            Tabel.t_author_id = App.CurrentUser.id;
            Tabel.t_otdel_id = SelectedOtdel.id;
            Tabel.t_month = _SelectMonth;
            Tabel.t_year = _SelectYear;
            Tabel.t_date_create = DateTime.Now;
            Tabel.tabelPersons = new ObservableCollection<TabelPerson>();


            // если есть персонал в отделе, добавляем его и формируем дни
            foreach (var item in PersonsFromOtdel)
            {
                TabelPerson tp = new TabelPerson();
                tp.tp_person_id = item.id;
                //tp.person = repoPersonal.Items.FirstOrDefault(it => it.id == item.id); 
                tp.TabelDays = new ObservableCollection<TabelDay>();

                foreach(var listItem in ListDays)
                {
                    TabelDay td = new TabelDay();
                    td.td_Day = listItem.Day;
                    td.CalendarTypeDay = listItem.KindDay;
                    switch (td.CalendarTypeDay)
                    {
                        case TypeDays.Holyday:
                            td.td_KindId = 2;
                            td.td_Hours = 0;
                            break;
                        case TypeDays.Work:
                            td.td_KindId = 1;
                            td.td_Hours = 8;
                            break;
                        case TypeDays.ShortWork:
                            td.td_KindId = 1;
                            td.td_Hours = 7;
                            break;
                    }

                    tp.TabelDays.Add(td);

                }

                Tabel.tabelPersons.Add(tp);
            }

            if (Tabel.tabelPersons.Count > 0)
                repoTabel.Add(Tabel, true);

            Tabel = repoTabel.Items
                .Where(it => it.id == Tabel.id)
                .Include(inc => inc.tabelPersons.Select(s => s.person))
                .FirstOrDefault();

            ListTabelPerson = new ObservableCollection<TabelPerson>(Tabel.tabelPersons);

            OnPropertyChanged(nameof(ListTabelPerson));
            OnPropertyChanged(nameof(Tabel));

        }

        //--------------------------------------------------------------------------------
        // Команда Загрузить из производственного календаря
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintCommand);
        private bool CanPrintCommand(object p) => Tabel != null && SelectedOtdel != null;
        private void OnPrintCommandExecuted(object p)
        {
            //TabelPrint print = new TabelPrint();
            //print.DataContext = this;
            //print.ShowDialog();


            using (XLWorkbook wb = new XLWorkbook(@"Отчеты\Табель.xlsx"))
            {
                int NumPP = 1;

                var ws = wb.Worksheets.Worksheet(2);

                // Заполение шапки
                ws.Cell("A8").Value = Tabel.otdel.ot_name;
                ws.Cell("AJ12").Value = Tabel.t_number;
                ws.Cell("AP12").Value = Tabel.t_date_create.Value.ToString("dd.MM.yyyy");

                DateTime startDate = new DateTime(_SelectYear, Tabel.t_month, 1);
                ws.Cell("AY12").Value = startDate.ToString("dd.MM.yyyy");
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                ws.Cell("BB12").Value = endDate.ToString("dd.MM.yyyy");
                //ws.Cell("AA15").Value = "Составил: " + App.CurrentUser.u_fio;


                int RowNum = 24;
                ws.Row(27).InsertRowsBelow((ListTabelPerson.Count() - 1) * 4);
                var range = ws.Range(RowNum, 1, RowNum + 3, 75);

                for (int i = 0; i < ListTabelPerson.Count() - 1; i++)
                {
                    RowNum += 4;
                    range.CopyTo(ws.Cell(RowNum, 1));
                }

                RowNum = 24;
                foreach (var item in ListTabelPerson)
                {
                    ws.Cell(RowNum, 1).Value = NumPP++;
                    ws.Cell(RowNum, 3).Value = item.person.FIO;
                    ws.Cell(RowNum, 11).Value = item.person.p_tab_number;

                    // отработано за половину месяца
                    ws.Cell(RowNum, 30).Value = item.DaysWeek1;
                    ws.Cell(RowNum + 1, 30).Value = item.HoursWeek1;
                    ws.Cell(RowNum + 2, 30).Value = item.DaysWeek2;
                    ws.Cell(RowNum + 3, 30).Value = item.HoursWeek2;

                    // отработано за месяц
                    ws.Cell(RowNum, 33).Value = item.DaysMonth;
                    ws.Cell(RowNum + 2, 33).Value = item.HoursMonth;

                    // правая часть
                    ws.Cell(RowNum, 73).Value = item.WorkedHours1;
                    ws.Cell(RowNum + 1, 73).Value = item.WorkedHours15;
                    ws.Cell(RowNum + 2, 73).Value = item.WorkedHours2;
                    ws.Cell(RowNum + 3, 73).Value = item.WorkedOffHours;

                    int ColNum = 14;
                    foreach(var day in item.TabelDays)
                    {
                        ws.Cell(RowNum, ColNum).Value = day.typeDay.t_name;
                        if(day.td_Hours.Value != 0)
                            ws.Cell(RowNum + 1, ColNum).Value =  day.td_Hours;

                        ColNum++;
                        if(ColNum > 29)
                        {
                            ColNum = 14;
                            RowNum += 2;
                        }
                    }

                    RowNum += 2;

                }

                string TempFile = FileOperation.GenerateTempFileNameWithDelete("TempTabel.xlsx");
                wb.SaveAs(TempFile);
                Process.Start(TempFile);

            }

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => SelectedOtdel != null && Tabel != null;
        private void OnSaveCommandExecuted(object p)
        {
            repoTabelPerson.Save();
            repoTabel.Save();
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => SelectedOtdel != null && Tabel != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            List<Personal> ListPersonal = repoPersonal.Items
                .AsNoTracking()
                .Where(it => it.p_otdel_id == SelectedOtdel.id)
                .ToList();

            // составляем список добавленных людей
            foreach(var item in ListTabelPerson)
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

                RepositoryCalendar repo = AllRepo.GetRepoCalendar();
                var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

                foreach (var item in ListPersonal)
                {
                    TabelPerson tp = new TabelPerson();
                    //tp.tp_person_id = item.id;
                    tp.person = repoPersonal.Items.FirstOrDefault(it => it.id == item.id);
                    tp.TabelDays = new ObservableCollection<TabelDay>();

                    foreach (var listItem in ListDays)
                    {
                        TabelDay td = new TabelDay();
                        tp.tp_tabel_id = Tabel.id;
                        td.td_Day = listItem.Day;
                        td.CalendarTypeDay = listItem.KindDay;
                        switch (td.CalendarTypeDay)
                        {
                            case TypeDays.Holyday:
                                td.td_KindId = 2;
                                td.td_Hours = 0;
                                break;
                            case TypeDays.Work:
                                td.td_KindId = 1;
                                td.td_Hours = 8;
                                break;
                            case TypeDays.ShortWork:
                                td.td_KindId = 1;
                                td.td_Hours = 7;
                                break;
                        }

                        tp.TabelDays.Add(td);

                    }

                    repoTabelPerson.Add(tp);
                    ListTabelPerson.Add(tp);
                }

                OnPropertyChanged(nameof(ListTabelPerson));
            }

        }

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------------------------
        public TabelUCViewModel()
        {
            ListTypeDays = repoTypeDay.Items.ToList();
            OnPropertyChanged(nameof(ListTypeDays));
        }

        //--------------------------------------------------------------------------------------------------
        // получение списка персонала из отдела
        //--------------------------------------------------------------------------------------------------
        public void OtdelChanged(Otdel otdel, int Year, int Month)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            SelectedOtdel = otdel;
            ListTabelPerson = null;

            if (otdel is null)  return;


            if (ListTabelPerson != null)
            {
                foreach (var item in ListTabelPerson)
                {
                    foreach (var day in item.TabelDays)
                        day.PropertyChanged -= ListPerson_PropertyChanged;
                }
            }

            if (SelectedOtdel.ot_parent is null)
            {
                Tabel = repoTabel.Items.FirstOrDefault(it => it.t_year == Year
                    && it.t_month == Month
                    && it.t_otdel_id == otdel.id);
                if(Tabel != null)
                    ListTabelPerson = new ObservableCollection<TabelPerson>(repoTabelPerson.Items
                        .Where(it => it.tp_tabel_id == Tabel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }
            else 
            {
                Tabel = repoTabel.Items.FirstOrDefault(it => it.t_year == Year
                    && it.t_month == Month
                    && it.t_otdel_id == otdel.ot_parent);
                if (Tabel != null)
                    ListTabelPerson = new ObservableCollection<TabelPerson> (repoTabelPerson.Items
                        .Where(it => it.tp_tabel_id == Tabel.id && it.person.p_otdel_id == otdel.id)
                        .OrderBy(o => o.person.p_lastname)
                        .ThenBy(o => o.person.p_name)
                        );
            }

            SetTypeDays();

            if (ListTabelPerson != null)
            {
                foreach (var item in ListTabelPerson)
                {
                    foreach (var day in item.TabelDays)
                        day.PropertyChanged += ListPerson_PropertyChanged;

                }
            }

            OnPropertyChanged(nameof(ListTabelPerson));
            OnPropertyChanged(nameof(Tabel));
        }

        //--------------------------------------------------------------------------------------
        // Событие изменения параметров дня
        //--------------------------------------------------------------------------------------
        private void ListPerson_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "td_Hours")
            {
                TabelPerson person = (sender as TabelDay).TabelPerson;

                List<TabelDay> ListDays = person.TabelDays.ToList();

                //person.OverWork = 0;
                for (int i = 0; i < ListDays.Count - 1; i++)
                {
                    if(ListDays[i].td_Hours - (ListDays[i].td_Hours2 ?? 0) + ListDays[i + 1].td_Hours > 20)
                    {
                        ListDays[i + 1].td_Hours2 = (ListDays[i].td_Hours + ListDays[i + 1].td_Hours - (ListDays[i].td_Hours2 ?? 0)) - 20;
                        //person.OverWork += ListDays[i + 1].td_Hours2;
                        ListDays[i + 1].VisibilityHours = Visibility.Visible;
                            
                    }
                    else
                    {
                        ListDays[i + 1].VisibilityHours = Visibility.Collapsed;
                        ListDays[i + 1].td_Hours2 = 0;

                    }
                }

                person.OnPropertyChanged(nameof(person.OverWork));
            }
        }

        //--------------------------------------------------------------------------------------
        // расстановка типов дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (Tabel is null) return;

            // получение данных производственного календаря
            RepositoryCalendar repo = AllRepo.GetRepoCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            foreach (var item in ListTabelPerson)
            {
                // расставляем выходные по каледнарю
                int i = 0;
                foreach (var day in item.TabelDays)
                {
                    day.CalendarTypeDay = ListDays[i].KindDay;
                    if(day.td_Hours2 > 0)
                        day.VisibilityHours = Visibility.Visible;
                    i++;
                }

            }


        }


    }
}
