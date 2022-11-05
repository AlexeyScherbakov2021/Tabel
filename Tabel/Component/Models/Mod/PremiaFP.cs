﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabel.Component.Models.Mod;
using Tabel.Models;

namespace Tabel.Component.Models
{
    public class PremiaFP : BasePremia, IDisposable
    {
        static bool lockPropertyChanged = false;

        // рассчетные часы
        private decimal? _SummaHoursFP;
        public decimal? SummaHoursFP { get => _SummaHoursFP; set { Set(ref _SummaHoursFP, value); } }
        
        // рассчетная сумма
        private decimal? _SummaPremFP;
        public decimal? SummaPremFP { get => _SummaPremFP; set { Set(ref _SummaPremFP, value); } }

        // суммарный процент группы
        private decimal? _ProcGroup;
        public decimal? ProcGroup { get => _ProcGroup; set { Set(ref _ProcGroup, value); } }


        //-------------------------------------------------------------------------------------------------------
        // Конструктор
        //-------------------------------------------------------------------------------------------------------
        public PremiaFP(ModPerson person) : base(person)
        {
            model.PropertyChanged += PremiaFPPropertyChanged;
        }

        public void Dispose()
        {
            model.PropertyChanged -= PremiaFPPropertyChanged;
        }


        //-------------------------------------------------------------------------------------------------------
        // установка расчетных часов и премии
        //-------------------------------------------------------------------------------------------------------
        public override void Calculation()
        {
            SummaHoursFP = model.md_sumFromFP * model.md_premFP / 100;
            SummaPremFP = model.TabelDays == 0
                ? 0
                : SummaHoursFP * model.md_cat_prem_tarif * (model.TabelDays - model.TabelAbsent)
                        / model.TabelDays;
        }



        //-------------------------------------------------------------------------------------------------------
        // Получение итоговой премии
        //-------------------------------------------------------------------------------------------------------
        public override decimal? GetPremia()
        {
            return SummaPremFP ?? 0;
        }

        //-------------------------------------------------------------------------------------------------------
        // установка суммарного процента по группе
        //-------------------------------------------------------------------------------------------------------
        public void CalcChangeProcent()
        {
            List<ModPerson> groupPerson = model.Mod?.ModPersons.Where(it => it.md_group == model.md_group && !String.IsNullOrEmpty(it.md_group)).ToList();

            if (groupPerson == null) return;
            decimal? SummaProc = groupPerson.Sum(it => it.md_premFP) / 100m;
            if (groupPerson != null)
            {
                foreach (var item in groupPerson)
                    item.premiaFP.ProcGroup = SummaProc;
            }

        }

        //-------------------------------------------------------------------------------------------------------
        // 
        //-------------------------------------------------------------------------------------------------------
        public void CalcChangeProcentAlls()
        {
            List<string> ListGroups = model.Mod?.ModPersons.Select(it => it.md_group).Distinct().ToList();
            if (ListGroups == null) return;
            foreach (var group in ListGroups)
            {
                List<ModPerson> groupPerson = model.Mod.ModPersons.Where(it => it.md_group == group && !String.IsNullOrEmpty(it.md_group)).ToList();
                if (groupPerson == null) return;
                decimal? SummaProc = groupPerson.Sum(it => it.md_premFP) / 100m;
                if (groupPerson != null)
                {
                    foreach (var item in groupPerson)
                        item.premiaFP.ProcGroup = SummaProc;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------
        // Событие изменения влияющих полей
        //-------------------------------------------------------------------------------------------------------
        private void PremiaFPPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "md_premFP":
                case "md_cat_prem_tarif":
                    Calculation();
                    CalcChangeProcent();
                    break;

                case "md_sumFromFP":
                    if (lockPropertyChanged) return;

                    lockPropertyChanged = true;

                    List<ModPerson> groupPerson = model.Mod?.ModPersons.Where(it => it.md_group == model.md_group && !String.IsNullOrEmpty(it.md_group)).ToList();
                    if (groupPerson != null)
                    {
                        decimal? summFP = model is null ? 0 : model.md_sumFromFP;
                        foreach (var item in groupPerson)
                        {
                            item.md_sumFromFP = summFP;
                            item.OnPropertyChanged(nameof(item.md_sumFromFP));
                            item.premiaFP.Calculation();
                        }
                    }
                    lockPropertyChanged = false;
                    break;

                case "md_group":
                    decimal? summ = model.Mod?.ModPersons
                        .Where(it => it.md_group == model.md_group && it.id != model.id)
                        .Select(s => s.md_sumFromFP).FirstOrDefault();
                    if (summ != null)
                        model.md_sumFromFP = summ;

                    Calculation();
                    CalcChangeProcentAlls();
                    break;
            }
        }

    }
}