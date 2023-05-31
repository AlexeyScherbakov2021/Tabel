using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Commands;
using Tabel.Infrastructure;
using Tabel.Models;
using Tabel.Repository;
using Tabel.ViewModels.Base;

namespace Tabel.ViewModels.ModViewModel
{
    public class PremiaOtdelViewModel : ModViewModel
    {
        public ObservableCollection<ModPerson> ListModPerson { get; set; }

        public decimal? SetProcPrem { get; set; }
        public decimal? SetProcFull { get; set; }

        public Mod SelectMod { get; set; }

        public AttachFile SelectedFile { get; set; }

        public PremiaOtdelViewModel(BaseModel db) : base(db)
        {
        }

        //-------------------------------------------------------------------------------------
        // Изменение списка сотрудников
        //-------------------------------------------------------------------------------------
        public override void ChangeListPerson(ICollection<ModPerson> listPerson, int Year, int Month, Otdel Otdel)
        {
            _SelectMonth = Month;
            _SelectYear = Year;
            SelectMod = listPerson?.FirstOrDefault()?.Mod;
            OnPropertyChanged("SelectMod");

            ListModPerson = new ObservableCollection<ModPerson> (listPerson?.Where(it => it.person.p_type_id == SpecType.N2));
            LoadFromCategory(listPerson);
            OnPropertyChanged(nameof(ListModPerson));
        }


        //-------------------------------------------------------------------------------------
        // Добавление сотрудников
        //-------------------------------------------------------------------------------------
        public override void AddPersons(ICollection<ModPerson> listNewPerson)
        {
            LoadFromCategory(listNewPerson);
            foreach (var person in listNewPerson)
            {
                if(person.person.p_type_id == SpecType.N2)
                    ListModPerson.Add(person);
            }
            OnPropertyChanged(nameof(ListModPerson));
        }

        //-------------------------------------------------------------------------------------
        // Загрузка категорий сотрудников
        //-------------------------------------------------------------------------------------
        private void LoadFromCategory(ICollection<ModPerson> listPerson)
        {
            if (listPerson is null || listPerson.FirstOrDefault()?.Mod?.m_IsClosed == true)
                return;

            foreach (var item in listPerson)
            {
                item.md_kvalif_tarif = item.md_workHours * item.person.p_premTarif;
                item.PlanTarifOtdel = 162m * item.person.p_premTarif;
                if (item.md_kvalif_tarif > item.PlanTarifOtdel)
                    item.md_kvalif_tarif = item.PlanTarifOtdel;
            }
        }




        #region Команды

        //--------------------------------------------------------------------------------
        // Команда Применить % премии к выбранным
        //--------------------------------------------------------------------------------
        //public ICommand SetProcPremCommand => new LambdaCommand(OnSetProcPremCommandExecuted, CanSetProcPremCommand);
        //private bool CanSetProcPremCommand(object p) => true;
        //private void OnSetProcPremCommandExecuted(object p)
        //{
        //    if (p is DataGrid dg)
        //    {
        //        foreach (ModPerson item in dg.SelectedItems)
        //        {
        //            item.md_kvalif_prem = SetProcPrem;
        //            item.OnPropertyChanged(nameof(item.md_kvalif_prem));
        //        }
        //    }

        //}

        //--------------------------------------------------------------------------------
        // Команда Применить % от полной к выбранным
        //--------------------------------------------------------------------------------
        public ICommand SetProcFullCommand => new LambdaCommand(OnSetProcFullCommandExecuted, CanSetProcFullCommand);
        private bool CanSetProcFullCommand(object p) => true;
        private void OnSetProcFullCommandExecuted(object p)
        {
            if (p is DataGrid dg)
            {
                foreach (ModPerson item in dg.SelectedItems)
                {
                    item.md_kvalif_proc = SetProcFull;
                    item.OnPropertyChanged(nameof(item.md_kvalif_proc));
                }
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Добавить файл
        //--------------------------------------------------------------------------------
        public ICommand AttachFileCommand => new LambdaCommand(OnAttachFileCommandExecuted, CanAttachFileCommand);
        private bool CanAttachFileCommand(object p) => SelectMod != null && SelectMod?.m_IsClosed != true;
        private void OnAttachFileCommandExecuted(object p)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Multiselect = true;

            if (dlgOpen.ShowDialog() == true)
            {
                LoadFiles(dlgOpen.FileNames);
            }

        }

        //--------------------------------------------------------------------------------
        // Команда Drop
        //--------------------------------------------------------------------------------
        public void ItemsControl_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                LoadFiles(files);
            }
        }


        //--------------------------------------------------------------------------------
        // Добавление файлов
        //--------------------------------------------------------------------------------
        private void LoadFiles(string[] files)
        {
            if (SelectMod == null || SelectMod.m_IsClosed == true)
                return;

            RepositoryFiles RepFile = new RepositoryFiles();
            RepositoryMSSQL<AttachFile> repoAttach = new RepositoryMSSQL<AttachFile>(db);
            foreach (var file in files)
            {
                AttachFile af = new AttachFile() { FullName = file, mod_id = SelectMod.id };
                SelectMod.ListAttachFiles.Add(af);
                repoAttach.Add(af);
            }
            RepFile.AddFiles(SelectMod.ListAttachFiles, SelectMod.m_year);
            repoAttach.Save();

        }


        //--------------------------------------------------------------------------------
        // Команда Удалить файл
        //--------------------------------------------------------------------------------
        public ICommand DetachFileCommand => new LambdaCommand(OnDetachFileCommandExecuted, CanDetachFileCommand);
        private bool CanDetachFileCommand(object p) => SelectedFile != null && SelectMod?.m_IsClosed != true;
        private void OnDetachFileCommandExecuted(object p)
        {
            RepositoryFiles RepFile = new RepositoryFiles();
            RepFile.DeleteFiles(SelectedFile, SelectMod.m_year);
            RepositoryMSSQL<AttachFile> repoAttach = new RepositoryMSSQL<AttachFile>(db);
            repoAttach.Remove(SelectedFile);
            SelectMod.ListAttachFiles.Remove(SelectedFile);
            repoAttach.Save();
        }

        //--------------------------------------------------------------------------------
        // Команда Открыть файл
        //--------------------------------------------------------------------------------
        public ICommand StartFileCommand => new LambdaCommand(OnStartFileCommandExecuted, CanStartFileCommand);
        private bool CanStartFileCommand(object p) => SelectMod != null;
        private void OnStartFileCommandExecuted(object p)
        {
            RepositoryFiles RepFile = new RepositoryFiles();
            AttachFile af = new AttachFile() { FileName = SelectedFile.FileName, mod_id = SelectMod.id };
            string FileName = RepFile.GetFile(af, SelectMod.m_year);
            Process.Start(FileName);

        }



        #endregion

    }
}
