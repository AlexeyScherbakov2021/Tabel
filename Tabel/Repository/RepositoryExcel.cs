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
        private static readonly int FirstMonthDays = 15;

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
                using (XLWorkbook wb = new XLWorkbook(@"Отчеты\Табель.xltx"))
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

                            decimal hours = day.WhiteHours * item.person.p_stavka;

                            ws.Cell(RowHours, ColNum).Value = day.typeDay.t_name;
                            if (day.WhiteHours != 0)
                            {

                                ws.Cell(RowHours + 1, ColNum).Value = hours;
                                Hours += hours;
                                if (day.td_Day <= 15)
                                    Hours1 += hours;
                                else
                                    Hours2 += hours;

                                if (hours > 8)
                                {
                                    if (hours > 10)
                                        hours20 += hours - 10;

                                    hours15 += hours > 9 ? 2 : 1;
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
                                OffHours += hours;

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




        public static async void PrintModelAsync(IEnumerable<ModPerson> ListModPerson, int SelectYear, int SelectMonth)
        {
            await Task.Run(() => PrintModel(ListModPerson, SelectYear, SelectMonth));

        }


        public static void PrintModel(IEnumerable<ModPerson> ListModPerson,  int SelectYear, int SelectMonth)
        {
            try
            {
                using (XLWorkbook wb = new XLWorkbook(@"Отчеты\Модель.xlsx"))
                {
                    var ws = wb.Worksheets.Worksheet(1);
                    var ws2 = wb.Worksheets.Worksheet(2);
                    var ws3 = wb.Worksheets.Worksheet(3);

                    int RowNum = 6;
                    int RowNum2 = 6;
                    int RowNum3 = 6;

                    ws.Cell("A2").Value = "'" + App.ListMonth[SelectMonth - 1].Name + " " + SelectYear;
                    ws2.Cell("A2").Value = "'" + App.ListMonth[SelectMonth - 1].Name + " " + SelectYear;
                    ws3.Cell("A2").Value = "'" + App.ListMonth[SelectMonth - 1].Name + " " + SelectYear;

                    foreach (var item in ListModPerson)
                    {
                        if (item.person.p_tab_number == "ГПХ")
                        {
                            ws3.Cell(RowNum3, 1).Value = item.person.p_tab_number;
                            ws3.Cell(RowNum3, 2).Value = item.person.otdel.parent?.ot_name ?? item.person.otdel.ot_name;
                            ws3.Cell(RowNum3, 3).Value = item.person.FIO;
                            ws3.Cell(RowNum3, 4).Value = item.person.p_profession;
                            ws3.Cell(RowNum3, 5).Value = item.premiaBonus.Summa;
                            ws3.Cell(RowNum3, 6).Value = item.premiaFP.Summa;
                            ws3.Cell(RowNum3, 7).Value = item.premiaOtdel.Summa;
                            ws3.Cell(RowNum3, 8).Value = item.premiStimul.Summa;
                            if (item.premiaAddWorks.Summa != 0)
                                ws3.Cell(RowNum3, 9).Value = item.premiaAddWorks.Summa;
                            ws3.Cell(RowNum3, 10).Value = item.premiaTransport.Summa;
                            ws3.Cell(RowNum3, 11).Value = item.premiaPrize.Summa;
                            ws3.Cell(RowNum3, 12).Value = item.md_bolnich;
                            ws3.Cell(RowNum3, 13).Value = item.md_compens;
                            ws3.Cell(RowNum3, 14).Value = item.PremiaItogo;
                            ws3.Cell(RowNum3, 15).Value = item.Itogo;
                            ws3.Row(RowNum3).InsertRowsBelow(1);
                            RowNum3++;
                        }

                        else if (!string.IsNullOrEmpty(item.person.p_tab_number))
                        {
                            ws.Cell(RowNum, 1).Value = item.person.p_tab_number;
                            ws.Cell(RowNum, 2).Value = item.person.otdel?.parent?.ot_name ?? item.person.otdel.ot_name;
                            ws.Cell(RowNum, 3).Value = item.person.FIO;
                            ws.Cell(RowNum, 4).Value = item.person.p_profession;
                            ws.Cell(RowNum, 5).Value = item.premiaBonus.Summa;
                            ws.Cell(RowNum, 6).Value = item.premiaFP.Summa;
                            ws.Cell(RowNum, 7).Value = item.premiaOtdel.Summa;
                            ws.Cell(RowNum, 8).Value = item.premiStimul.Summa;
                            if (item.premiaAddWorks.Summa != 0)
                                ws.Cell(RowNum, 9).Value = item.premiaAddWorks.Summa;
                            ws.Cell(RowNum, 10).Value = item.premiaTransport.Summa;
                            ws.Cell(RowNum, 11).Value = item.premiaPrize.Summa;
                            ws.Cell(RowNum, 12).Value = item.md_bolnich;
                            ws.Cell(RowNum, 13).Value = item.md_compens;
                            ws.Cell(RowNum, 14).Value = item.PremiaItogo;
                            ws.Cell(RowNum, 15).Value = item.Itogo;
                            ws.Row(RowNum).InsertRowsBelow(1);
                            RowNum++;
                        }

                        ws2.Cell(RowNum2, 1).Value = item.person.p_tab_number;
                        ws2.Cell(RowNum2, 2).Value = item.person.otdel.parent?.ot_name ?? item.person.otdel.ot_name;
                        ws2.Cell(RowNum2, 3).Value = item.person.FIO;
                        ws2.Cell(RowNum2, 4).Value = item.person.p_profession;
                        ws2.Cell(RowNum2, 5).Value = item.premiaBonus.Summa;
                        ws2.Cell(RowNum2, 6).Value = item.premiaFP.Summa;
                        ws2.Cell(RowNum2, 7).Value = item.premiaOtdel.Summa;
                        ws2.Cell(RowNum2, 8).Value = item.premiStimul.Summa;
                        if (item.premiaAddWorks.Summa != 0)
                            ws2.Cell(RowNum2, 9).Value = item.premiaAddWorks.Summa;
                        ws2.Cell(RowNum2, 10).Value = item.premiaTransport.Summa;
                        ws2.Cell(RowNum2, 11).Value = item.premiaPrize.Summa;
                        ws2.Cell(RowNum2, 12).Value = item.md_bolnich;
                        ws2.Cell(RowNum2, 13).Value = item.md_compens;
                        ws2.Cell(RowNum2, 14).Value = item.PremiaItogo;
                        ws2.Cell(RowNum2, 15).Value = item.Itogo;
                        ws2.Row(RowNum2).InsertRowsBelow(1);
                        RowNum2++;
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
    }
}
