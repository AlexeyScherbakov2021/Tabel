using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Component.MonthPanel;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;
using Tabel.Views;
using Tabel.Views.PrintForm;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;

namespace Tabel.ViewModels
{
    internal class TabelUCViewModel : ViewModel, IBaseUCViewModel, IDisposable
    {
        private readonly BaseModel db;
        private RepositoryMSSQL<Models.Personal> repoPersonal;
        private readonly RepositoryMSSQL<WorkTabel> repoTabel;
        private readonly RepositoryMSSQL<typeDay> repoTypeDay;
        private readonly RepositoryMSSQL<TabelPerson> repoTabelPerson;
        private readonly RepositoryMSSQL<OtpuskPerson> repoOtpuskPerson;

        public WorkTabel Tabel { get; set; }
        public IEnumerable<typeDay> ListTypeDays { get; set; }
        public ObservableCollection<TabelPerson> ListTabelPerson { get; set; }
        public TabelPerson SelectedPerson { get; set; }

        public Otdel SelectedOtdel { get; set; }
        private int _SelectMonth;
        private int _SelectYear;
        private bool IsModify = false;



        //--------------------------------------------------------------------------------------------------
        // Конструктор
        //--------------------------------------------------------------------------------------------------
        public TabelUCViewModel()
        {
            repoPersonal = new RepositoryMSSQL<Models.Personal>();
            db = repoPersonal.GetDB();
            repoTabel = new RepositoryMSSQL<WorkTabel>(db);
            repoTypeDay = new RepositoryMSSQL<typeDay>(db);
            repoTabelPerson = new RepositoryMSSQL<TabelPerson>(db);
            repoOtpuskPerson = new RepositoryMSSQL<OtpuskPerson>(db);

            //repoPersonal = AllRepo.GetRepoPersonal();
            //repoTabel = AllRepo.GetRepoTabel();
            //repoTypeDay = AllRepo.GetRepoTypeDay();
            //repoTabelPerson = AllRepo.GetRepoTabelPerson();

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
            // отслеживаем изменение часов
            if(e.PropertyName == "td_Hours")
            {
                TabelPerson person = (sender as TabelDay).TabelPerson;
                AnalizeOverWork(person);
                IsModify = true;
            }
            if (e.PropertyName == "td_KindId")
            {
                IsModify = true;
            }


        }


        //--------------------------------------------------------------------------------------
        // Расчет лишних часов для всего месяца
        //--------------------------------------------------------------------------------------
        private void AnalizeOverWork(TabelPerson person)
        {
            List<TabelDay> ListDays = person.TabelDays.ToList();
            int nCntPermDays = person.PrevPermWorkCount + 1;

            decimal PrevHours;
            decimal? OverHours;

            for (int i = 0; i < ListDays.Count; i++)
            {
                if(i == 0)
                    // для первого дня берем предыдущий день из прошлого табеля
                    PrevHours = person.PrevDay is null ? 0 : (person.PrevDay.td_Hours - person.PrevDay.td_Hours2) ?? 0;
                else
                    // часы предыдущего дня
                    PrevHours = ListDays[i - 1].WhiteHours;

                OverHours = 0;

                if(ListDays[i].td_Hours == 0)
                    nCntPermDays = 0;

                if (nCntPermDays >= 7)
                {
                    // если проработано более 6 дней подряд
                    OverHours = ListDays[i].td_Hours;
                    nCntPermDays = 0;
                }
                else if(ListDays[i].CalendarTypeDay != TypeDays.Holyday)
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
            person.OnPropertyChanged(nameof(person.OverWork));
        }

        //--------------------------------------------------------------------------------------
        // расстановка типов дней по производственному календарю
        //--------------------------------------------------------------------------------------
        private void SetTypeDays()
        {
            if (Tabel is null) return;

            List<(int Day, TypeDays KindDay, decimal Hours)> ListDaysMonth = null;

            // получение данных производственного календаря
            RepositoryCalendar repo = new RepositoryCalendar(db);
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            int PrevYear = _SelectYear;
            int PrevMonth = _SelectMonth - 1;
            if(PrevMonth < 1)
            {
                PrevMonth = 1;
                PrevYear--;
            }

            var PrevTabel = repoTabel.Items
                .AsNoTracking()
                .FirstOrDefault(it => it.t_month == PrevMonth
                    && it.t_year == PrevYear
                    && it.t_otdel_id == Tabel.t_otdel_id);

            if(PrevTabel == null)
            {
                RepositoryCalendar repoCal = new RepositoryCalendar(db);
                ListDaysMonth = repoCal.GetListDays(PrevYear, PrevMonth);
            }

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

                // инициализация расчетных часов
                item.CalcHours = 0;
                // получение часов последнего дня предыдущего месяца
                item.PrevDay = null;
                // получение количестве непрерывно отработанных дней в конце предыдущего месяца
                item.PrevPermWorkCount = 0;

                if (PrevTabel != null && item.person != null)
                {
                    // получение данных предыдущего табеля
                    var PrevTabelPerson = PrevTabel.tabelPersons
                        .FirstOrDefault(it => it.person.id == item.person.id);

                    if (PrevTabelPerson != null)
                    {
                        item.PrevDay = PrevTabelPerson.TabelDays.Last();
                        var PrevListDays = PrevTabelPerson.TabelDays.ToArray();
                        int nCntDays = PrevListDays.Count() - 1;
                        for (int n = nCntDays; n > nCntDays - 6; n--)
                        {
                            if (PrevListDays[n].td_Hours /*- PrevListDays[n].td_Hours2*/ == 0)
                                break;

                            item.PrevPermWorkCount++;
                        }
                    }
                }
                else if(ListDaysMonth != null)
                {
                    // расчет при отсутствии предыдущего табеля
                    item.PrevDay = new TabelDay();
                    var LastDay = ListDaysMonth.Last();
                    item.PrevDay.td_Day = LastDay.Day;
                    item.PrevDay.CalendarTypeDay = LastDay.KindDay;
                    item.PrevDay.td_Hours = LastDay.Hours;

                    int nCntDays = ListDaysMonth.Count() - 1;
                    for (int n = nCntDays; n > nCntDays - 6; n--)
                    {
                        if (ListDaysMonth[n].Hours == 0)
                            break;

                        item.PrevPermWorkCount++;
                    }
                }
                //item.OnPropertyChanged("WhiteHours");

            }
        }

        //--------------------------------------------------------------------------------------
        // Событие закрытия формы
        //--------------------------------------------------------------------------------------
        public bool ClosingFrom()
        {
            return IsModify;
        }


        //--------------------------------------------------------------------------------------
        // Запись в базу данных
        //--------------------------------------------------------------------------------------
        public void SaveForm()
        {
            repoTabelPerson.Save();
            repoTabel.Save();
            IsModify = false;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Создать табель
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
            RepositoryCalendar repo = new RepositoryCalendar(db); // AllRepo.GetRepoCalendar();
            var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

            RepositoryMSSQL<Otdel> repoOtdel = new RepositoryMSSQL<Otdel>(db);// AllRepo.GetRepoAllOtdels();
            List<int> listOtdels = repoOtdel.Items.AsNoTracking().Where(it => it.ot_parent == SelectedOtdel.id).Select(s => s.id).ToList();

            // получение сотрудников отдела
            repoPersonal = new RepositoryMSSQL<Models.Personal>(db);// AllRepo.GetRepoPersonal();

            List<Models.Personal> PersonsFromOtdel = repoPersonal.Items
                .AsNoTracking()
                .Where(it => (it.p_otdel_id == SelectedOtdel.id || listOtdels.Contains(it.p_otdel_id.Value)) && it.p_delete == false)
                .OrderBy(o => o.p_lastname)
                .ThenBy(o => o.p_name)
                .ToList();

            if (PersonsFromOtdel?.Count == 0)
            {
                MessageBox.Show("В вашем отделе нет сотрудников. Табель не создан.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                List<OtpuskDays> OtpDays = repoOtpuskPerson
                    .Items
                    .AsNoTracking()
                    .FirstOrDefault(it => it.person.id == item.id && it.otpusk.o_year == _SelectYear)?.ListDays.ToList();

                //tp.person = repoPersonal.Items.FirstOrDefault(it => it.id == item.id); 
                tp.TabelDays = new ObservableCollection<TabelDay>();

                foreach (var listItem in ListDays)
                {
                    TabelDay td = new TabelDay();
                    td.td_Day = listItem.Day;

                    td.CalendarTypeDay = listItem.KindDay;
                    switch (td.CalendarTypeDay)
                    {
                        case TypeDays.Holyday:
                            td.td_KindId = (int)TabelKindDays.OffDay;
                            td.td_Hours = 0;
                            break;
                        case TypeDays.Work:
                            td.td_KindId = (int)TabelKindDays.Worked;
                            td.td_Hours = 8;
                            break;
                        case TypeDays.ShortWork:
                            td.td_KindId = (int)TabelKindDays.Worked;
                            td.td_Hours = 7;
                            break;
                    }

                    if (OtpDays != null && OtpuskUCViewModel.IsOtpuskDay(new DateTime(_SelectYear, _SelectMonth, td.td_Day), OtpDays))
                    {
                        td.typeDay = repoTypeDay.Items.FirstOrDefault(it => it.t_name == "ОТ");
                        td.td_Hours = 0;
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


        //private bool IsOtpuskDay(DateTime date, List<OtpuskDays> OtpDays)
        //{
        //    foreach(var item in OtpDays)
        //    {
        //        if (date >= item.od_StartDate && date <= item.od_EndDate)
        //            return true;
        //    }

        //    return false;
        //}


        //--------------------------------------------------------------------------------
        // Команда Загрузить из производственного календаря
        //--------------------------------------------------------------------------------
        public ICommand PrintCommand => new LambdaCommand(OnPrintCommandExecuted, CanPrintCommand);
        private bool CanPrintCommand(object p) => Tabel != null && SelectedOtdel != null;
        private void OnPrintCommandExecuted(object p)
        {
            bool IsAllPrint = p is null;

            IEnumerable<TabelPerson> SelectedListPerson = ListTabelPerson
                .Where(it => !string.IsNullOrEmpty(it.person.p_tab_number) && it.person.p_tab_number != "ГПХ");

            RepositoryExcel.PrintTabel(ListTabelPerson, IsAllPrint, Tabel, _SelectYear, _SelectMonth);

        }

        //--------------------------------------------------------------------------------
        // Команда Сохранить
        //--------------------------------------------------------------------------------
        public ICommand SaveCommand => new LambdaCommand(OnSaveCommandExecuted, CanSaveCommand);
        private bool CanSaveCommand(object p) => SelectedOtdel != null && Tabel != null && IsModify;
        private void OnSaveCommandExecuted(object p)
        {
            SaveForm();
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить сотрудников
        //--------------------------------------------------------------------------------
        public ICommand AddPersonCommand => new LambdaCommand(OnAddPersonCommandExecuted, CanAddPersonCommand);
        private bool CanAddPersonCommand(object p) => SelectedOtdel != null && Tabel != null;
        private void OnAddPersonCommandExecuted(object p)
        {
            List<Models.Personal> ListPersonal = repoPersonal.Items
                .AsNoTracking()
                .Where(it => it.p_otdel_id == SelectedOtdel.id && it.p_delete == false)
                .ToList();

            // составляем список добавленных людей
            foreach (var item in ListTabelPerson)
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
                RepositoryCalendar repo = new RepositoryCalendar(db);// AllRepo.GetRepoCalendar();
                var ListDays = repo.GetListDays(_SelectYear, _SelectMonth);

                foreach (var item in ListPersonal)
                {
                    List<OtpuskDays> OtpDays = repoOtpuskPerson
                        .Items
                        .AsNoTracking()
                        .FirstOrDefault(it => it.person.id == item.id && it.otpusk.o_year == _SelectYear)?.ListDays.ToList();

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
                                td.td_KindId = (int)TabelKindDays.OffDay;
                                td.td_Hours = 0;
                                break;
                            case TypeDays.Work:
                                td.td_KindId = (int)TabelKindDays.Worked;
                                td.td_Hours = 8;
                                break;
                            case TypeDays.ShortWork:
                                td.td_KindId = (int)TabelKindDays.Worked;
                                td.td_Hours = 7;
                                break;
                        }

                        if (OtpDays != null && OtpuskUCViewModel.IsOtpuskDay(new DateTime(_SelectYear, _SelectMonth, td.td_Day), OtpDays))
                        {
                            td.typeDay = repoTypeDay.Items.FirstOrDefault(it => it.id == (int)TabelKindDays.Otpusk);
                            td.td_Hours = 0;
                        }
                        tp.TabelDays.Add(td);
                    }

                    repoTabelPerson.Add(tp, true);
                    ListTabelPerson.Add(tp);
                }

                OnPropertyChanged(nameof(ListTabelPerson));
                IsModify = true;
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
                repoTabelPerson.Remove(SelectedPerson, true);
                ListTabelPerson.Remove(SelectedPerson);
                //IsModify = true;
            }
        }



        //--------------------------------------------------------------------------------
        // Команда СЗ для выходного
        //--------------------------------------------------------------------------------
        public ICommand SZOffDayCommand => new LambdaCommand(OnSZOffDayCommandExecuted, CanSZOffDayCommand);
        private bool CanSZOffDayCommand(object p) => SelectedOtdel != null && Tabel != null;
        private void OnSZOffDayCommandExecuted(object p)
        {
            List<int> listDays = new List<int>();
            List<Models.Personal> listPerson = new List<Models.Personal>();

            foreach(var pers in ListTabelPerson)
            {
                foreach(var day in pers.TabelDays)
                {
                    if(day.typeDay.t_name == "РВ" && day.CalendarTypeDay == TypeDays.Holyday )
                    {
                        listDays.Add(day.td_Day);
                    }
                }

            }

            listDays = listDays.Distinct().ToList();

            SelectDateWindow win = new SelectDateWindow();
            SelectDateWindowViewModel vm = new SelectDateWindowViewModel(listDays, _SelectMonth, _SelectYear);
            win.DataContext = vm;

            if(win.ShowDialog() == true)
            {
                foreach(var persMod in ListTabelPerson)
                {
                    foreach (var day in persMod.TabelDays)
                    {
                        if (day.typeDay.t_name == "РВ" && day.CalendarTypeDay == TypeDays.Holyday && day.td_Day == vm.SelectedDate.Day)
                        {
                            listPerson.Add(persMod.person);
                        }
                    }
                }
                RepositoryWord repoWord = new RepositoryWord(@"Отчеты\СЗ выходные дни.docx");
                repoWord.CreateWorkOffSZ(listPerson, vm.SelectedDate);
            }

        }

        #endregion

    }
}
