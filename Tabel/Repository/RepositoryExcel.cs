using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tabel.Models;

namespace Tabel.Repository
{
    internal class RepositoryExcel
    {
        private static readonly int FirstMonthDays = 20;


        public static void PrintTabel(IEnumerable<TabelPerson> ListTabelPerson, bool IsAllMonth 
            , WorkTabel Tabel, int SelectYear, int SelectMonth)
        {

            decimal Hours;
            int Days;
            decimal Hours1;
            int Days1;
            decimal Hours2;
            int Days2;
            decimal hours15;
            decimal hours20;
            decimal OffHours;

            int NumPP;

            try {
                using (XLWorkbook wb = new XLWorkbook(@"Отчеты\Табель.xlsx"))
                {
                    NumPP = 1;
                    var ws = wb.Worksheets.Worksheet(1);
                    ws.Name = "ПР " + App.ListMonth[SelectMonth - 1].Name;

                    // Заполение шапки
                    ws.Cell("A8").Value = Tabel.otdel.ot_name;
                    ws.Cell("AJ12").Value = Tabel.t_number;
                    ws.Cell("AP12").Value = Tabel.t_date_create.Value.ToString("dd.MM.yyyy");

                    DateTime startDate = new DateTime(SelectYear, Tabel.t_month, 1);
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

                        int RowHours = RowNum;
                        int ColNum = 14;
                        Hours = 0;
                        Days = 0;
                        hours15 = 0;
                        hours20 = 0;
                        OffHours = 0;
                        Days1 = 0;
                        Hours1 = 0;
                        Days2 = 0;
                        Hours2 = 0;

                        foreach (var day in item.TabelDays)
                        {
                            if (!IsAllMonth && day.td_Day > FirstMonthDays)
                                break;

                            ws.Cell(RowHours, ColNum).Value = day.typeDay.t_name;
                            if (day.WhiteHours != 0)
                            {
                                ws.Cell(RowHours + 1, ColNum).Value = day.WhiteHours;
                                Hours += day.WhiteHours;
                                if (day.td_Day <= 15)
                                    Hours1 += day.WhiteHours;
                                else
                                    Hours2 += day.WhiteHours;

                                if (day.WhiteHours > 8)
                                {
                                    if (day.WhiteHours > 10)
                                        hours20 += day.WhiteHours - 10;

                                    hours15 += day.WhiteHours > 9 ? 2 : 1;
                                }
                            }

                            if (day.typeDay.t_name == "Я")
                            {
                                Days++;
                                if (day.td_Day <= 15)
                                    Days1++;
                                else
                                    Days2++;
                            }

                            if (day.typeDay.t_name == "РВ")
                                OffHours += day.WhiteHours;

                            ColNum++;
                            if (day.td_Day == 15)
                            {
                                ColNum = 14;
                                RowHours += 2;
                            }
                        }

                        // отработано за половину месяца
                        ws.Cell(RowNum, 30).Value = Days1; //item.DaysWeek1;
                        ws.Cell(RowNum + 1, 30).Value = Hours1; //item.HoursWeek1;
                        ws.Cell(RowNum + 2, 30).Value = Days2; //item.DaysWeek2;
                        ws.Cell(RowNum + 3, 30).Value = Hours2; // item.HoursWeek2;

                        // отработано за месяц
                        ws.Cell(RowNum, 33).Value = Days;// item.DaysMonth;
                        ws.Cell(RowNum + 2, 33).Value = Hours; // item.HoursMonth;

                        // правая часть
                        ws.Cell(RowNum, 73).Value = Hours - OffHours; // item.WorkedHours1;
                        ws.Cell(RowNum + 1, 73).Value = hours15; //item.WorkedHours15;
                        ws.Cell(RowNum + 2, 73).Value = hours20; //item.WorkedHours2;
                        ws.Cell(RowNum + 3, 73).Value = OffHours; //item.WorkedOffHours;

                        RowNum = RowHours + 2;
                    }

                    string TempFile = System.IO.Path.GetTempFileName();
                    TempFile = System.IO.Path.ChangeExtension(TempFile, "xlsx");
                    wb.SaveAs(TempFile);
                    Process.Start(TempFile);
                }
            }
            catch
            {
                MessageBox.Show("Не найден шаблон табеля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

}
    }
}
