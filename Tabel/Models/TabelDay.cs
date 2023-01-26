namespace Tabel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Windows;
    using Tabel.Component.MonthPanel;
    using Tabel.Infrastructure;

    [Table("TabelDay")]
    public partial class TabelDay : Observable, IEntity
    {
        [Key]
        [Column("td_Id")]
        public int id { get; set; }

        public int td_TabelPersonId { get; set; }

        public int td_Day { get; set; }

        //public int td_KindId { get; set; }
        private int _td_KindId;

        public int td_KindId
        {
            get => _td_KindId;
            set
            {
                if (Set(ref _td_KindId, value))
                    TabelPerson?.UpdateUI();
            }
        }


        //public decimal? td_Hours { get; set; }
        private decimal? _td_Hours;
        [Column(TypeName = "numeric")]
        public decimal? td_Hours
        {
            get => _td_Hours;
            set
            {
                if (Set(ref _td_Hours, value))
                    TabelPerson?.UpdateUI();
            }
        }

        public decimal? _td_Hours2 = 0;
        public decimal? td_Hours2 { get => _td_Hours2; set { Set(ref _td_Hours2, value); } }

        public virtual TabelPerson TabelPerson { get; set; }

        public virtual typeDay typeDay { get; set; }


        private Visibility _VisibilityHours = Visibility.Collapsed;
        [NotMapped]
        public Visibility VisibilityHours { get => _VisibilityHours; set { Set(ref _VisibilityHours, value); } }

        [NotMapped]
        public TypeDays CalendarTypeDay { get; set; }

        [NotMapped]
        public decimal WhiteHours => ((td_Hours ?? 0) - (td_Hours2 ?? 0));

        public string DayString
        {
            get
            {
                DateTime dt = new DateTime(TabelPerson.tabel.t_year, TabelPerson.tabel.t_month, td_Day);
                return dt.ToString("d ddd").ToLower();
            }
        }


    }
}
