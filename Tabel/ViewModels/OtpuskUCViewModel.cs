//using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{

    public class OtpuskDays : Observable
    {
        public DateTime StartDate { get; set; }
        private DateTime _EndDate;
        public DateTime EndDate { get => _EndDate; set { Set(ref _EndDate, value); } }
        public int CountDay => EndDate.DayOfYear - StartDate.DayOfYear + 1;
        public string ToolTipName { get; set; }

        public OtpuskDays() { }
    }



    internal class OtpuskUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        //private int _SelectMonth;
        private int _SelectYear;


        public List<OtpuskDays> ListDays { get; set; } = new List<OtpuskDays>() { 
            new OtpuskDays() { StartDate = new DateTime(2023, 1, 1), EndDate = new DateTime(2023,1,31), ToolTipName="подсказка 1" },
            new OtpuskDays() { StartDate = new DateTime(2023, 6, 1), EndDate = new DateTime(2023,6,30), ToolTipName="подсказка 2" },
            new OtpuskDays() { StartDate = new DateTime(2023, 11, 1), EndDate = new DateTime(2023,11,30), ToolTipName="подсказка 3"  },
        };


        private readonly BaseModel db;
        private readonly RepositoryMSSQL<Personal> repoPersonal;

        public TransPerson SelectedPerson { get; set; }

        private bool IsModify;

        


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

        public OtpuskUCViewModel()
        {
            repoPersonal = new RepositoryMSSQL<Personal>();
            db = repoPersonal.GetDB();
            //repoTransp = new RepositoryMSSQL<Transport>(db);
            //repoTransPerson = new RepositoryMSSQL<TransPerson>(db);
            IsModify = false;
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
            //_SelectMonth = Month;
            _SelectYear = Year;
            _SelectedOtdel = SelectOtdel;
            ListTransPerson = null;

            if (SelectOtdel is null) return;

            if (_SelectedOtdel.ot_parent is null)
            {
                //Transp = repoTransp.Items.FirstOrDefault(it => it.tr_Year == Year
                //    && it.tr_Month == Month
                //    && it.tr_OtdelId == _SelectedOtdel.id);
                //if (Transp != null)
                //    ListTransPerson = new ObservableCollection<TransPerson>(repoTransPerson.Items
                //        .Where(it => it.tp_TranspId == Transp.id)
                //        .OrderBy(o => o.person.p_lastname)
                //        .ThenBy(o => o.person.p_name)
                //        );
            }
            else
            {
                //Transp = repoTransp.Items.FirstOrDefault(it => it.tr_Year == Year
                //    && it.tr_Month == Month
                //    && it.tr_OtdelId == _SelectedOtdel.ot_parent);
                //if (Transp != null)
                //    ListTransPerson = new ObservableCollection<TransPerson>(repoTransPerson.Items
                //        .Where(it => it.tp_TranspId == Transp.id && it.person.p_otdel_id == _SelectedOtdel.id)
                //        .OrderBy(o => o.person.p_lastname)
                //        .ThenBy(o => o.person.p_name)
                //        );
            }

            //SetTypeDays();

            //OnPropertyChanged(nameof(Transp));
            //OnPropertyChanged(nameof(ListTransPerson));
        }

        public void SaveForm()
        {
            //repoTransPerson.Save();
            //repoTransp.Save();
            IsModify = false;
        }


        #region Команды

        //--------------------------------------------------------------------------------
        // Событие выбора закладки
        //--------------------------------------------------------------------------------
        public ICommand EditOtpuskCommand => new LambdaCommand(OnEditOtpuskCommandExecuted, CanEditOtpuskCommand);
        private bool CanEditOtpuskCommand(object p) => true;
        private void OnEditOtpuskCommandExecuted(object p)
        {
            OtpuskDays od = (p as RoutedEventArgs).OriginalSource as OtpuskDays;
            od.EndDate = new DateTime(2023, 12, 10);
            od.OnPropertyChanged(nameof(od.CountDay));
        }

        #endregion

    }
}
