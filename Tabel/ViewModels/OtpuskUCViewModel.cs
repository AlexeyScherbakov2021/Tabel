//using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels
{

    public class OtpuskDays
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CountDays { get; set; }

        public Thickness start { get; set; }

        public int width { get; set; }

        public OtpuskDays() { }
    }



    internal class OtpuskUCViewModel : ViewModel, IBaseUCViewModel
    {
        private Otdel _SelectedOtdel;
        private int _SelectMonth;
        private int _SelectYear;




        public List<OtpuskDays> ListDays { get; set; } = new List<OtpuskDays>() { 
            new OtpuskDays() { start = new Thickness(10,0,0,0), width= 80 },
            new OtpuskDays() { start = new Thickness(200,0,0,0), width= 80 },
            new OtpuskDays() { start = new Thickness(300,0,0,0), width= 80 },
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
            _SelectMonth = Month;
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
    }
}
