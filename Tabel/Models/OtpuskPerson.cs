﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Infrastructure;

namespace Tabel.Models
{
    [Table("OtpuskPerson")]
    public class OtpuskPerson : Observable, IEntity
    {
        public OtpuskPerson()
        {
            ListDays = new ObservableCollection<OtpuskDays>();
        }

        public int id { get; set; }
        public int? op_otpuskId { get; set; }
        public int op_personId { get; set; }

        public virtual Otpusk otpusk { get; set; }

        public virtual Personal person { get; set; }

        public virtual ObservableCollection<OtpuskDays> ListDays { get; set; }

        [NotMapped]
        public int AllDays => ListDays.Sum(s => s.CountDay);

    }
}
