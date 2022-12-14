using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Tabel.Infrastructure
{
    public enum SmenaKind : int { None, First, Second, DayOff, Otpusk };

    public enum KindTrans : int { None, Used };

    public enum TabelKind : int { None, Work, DayOff, Komandir, Otpusk, WorkDayOff, DopOtpusk, Bolen, Obuchenie, NotWork };



    public enum UserRoles : int { Admin, Управление, Пользователь };

    //public class UserRolesBinding : MarkupExtension
    //{
    //    public Type EnumType { get; private set; }

    //    public UserRolesBinding(Type enumType)
    //    {
    //        if (enumType is null || !enumType.IsEnum)
    //            throw new Exception("не тот тип");

    //        EnumType = enumType;
    //    }

    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        return Enum.GetValues(EnumType);
    //    }
    //}

    public enum SpecType : int { Рабочий, ИТР };


    public class EnumTypeBinding : MarkupExtension
    {
        public Type EnumType { get; private set; }

        public EnumTypeBinding(Type enumType)
        {
            if (enumType is null || !enumType.IsEnum)
                throw new Exception("не тот тип");

            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //object result = Enum.GetValues(EnumType);

            //object result2 = Enum.GetValues(EnumType)
            //    .Cast<object>()
            //    .Select(e => new { Value = (int)e, DisplayName = e.ToString() })
            //    ;

            return Enum.GetValues(EnumType)
                //.Cast<object>()
                //.Select(e => new { Value = (int)e, DisplayName = e.ToString() })
                ;
        }
    }





    public class EnumToString
    {
        public static List<string> ListSmenaKind = new List<string>() {"",  "1см", "2см", "В", "О" };
        //public static List<string> ListSpecType = new List<string>() {"Рабочий",  "ИТР" };


        public static string SmenaKindToString(SmenaKind num)
        {
            return ListSmenaKind[(int)num];
        }

        //public static string SpecTypeToString(SmenaKind num)
        //{
        //    return ListSpecType[(int)num];
        //}


    }




}
