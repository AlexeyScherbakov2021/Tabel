﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Tabel.Infrastructure
{
    public enum SmenaKind: int { None, First, Second,  DayOff, Otpusk }
    public enum TabelKind: int { None, Work, DayOff, Komandir, Otpusk, WorkDayOff, DopOtpusk, Bolen, Obuchenie, NotWork};
    public enum UserRoles: int { Admin, Управление, Пользователь};



    public class UserRolesBinding : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public UserRolesBinding(Type enumType)
        {
            if(enumType is null || !enumType.IsEnum)
                    throw new Exception("не тот тип");

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
