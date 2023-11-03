using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Tabel.Commands;
using Tabel.Models;
using Tabel.ViewModels.Base;
using Tabel.Views;
using Microsoft.Win32;
using Tabel.Repository;
using DocumentFormat.OpenXml.Bibliography;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tabel.ViewModels
{
    internal class TasksPersonWindowViewModel : ViewModel
    {
        public string Title { get; set; } 

        private bool IsReadOnly = false;
        private Stopwatch _stopwatch = new Stopwatch();

        private readonly int Year;
        private readonly int IDModPerson;
        public ObservableCollection<TargetTask> ListTarget { get; set; }
        public TargetTask SelectedTarget { get; set; }

        private decimal? _proc100;
        public decimal? proc100 { get => _proc100; set { Set(ref _proc100, value); } }

        private decimal? _proc100fact;
        public decimal? proc100fact { get => _proc100fact; set { Set(ref _proc100fact, value); } }

        private decimal? _sum_fact;
        public decimal? sum_fact { get => _sum_fact; set { Set(ref _sum_fact, value); } }

        readonly private decimal? premBase;

        public TasksPersonWindowViewModel() { }

        public TasksPersonWindowViewModel(ModPerson person/*, RepositoryMSSQL<ModPerson> repo*/)
        {
            _stopwatch.Start();
            Title = "Выполненные задания для сотрудника " + person.person.FIO;

            //premBase = person.md_prem_otdel;
            premBase = person.md_Oklad;
            //repoTask = repo;

            IsReadOnly = person.Mod.m_IsClosed == true;
            Year = person.Mod.m_year;
            IDModPerson = person.id;
            ListTarget = new ObservableCollection<TargetTask>( person.ListTargetTask);
            foreach (TargetTask item in ListTarget)
            {
                item.PropertyChanged += Item_PropertyChanged;
                item.tt_sum_fact = premBase * item.tt_proc_fact / 100;
            }
            ListTarget.CollectionChanged += ListTarget_CollectionChanged;
            proc100 = ListTarget.Sum(it => it.tt_proc_task);
            proc100fact = ListTarget.Sum(it => it.tt_proc_fact);
            sum_fact = ListTarget.Sum(it => it.tt_sum_fact);
        }

        private void ListTarget_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems?.Count > 0)
            {
                foreach (TargetTask item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
            }

            if (e.OldItems?.Count > 0)
            {
                foreach (TargetTask item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
                proc100 = ListTarget.Sum(it => it.tt_proc_task);
                proc100fact = ListTarget.Sum(it => it.tt_proc_fact);
                sum_fact = ListTarget.Sum(it => it.tt_sum_fact);
            }
        }


        //static int isSkip = 0;
        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            proc100 = ListTarget.Sum(it => it.tt_proc_task);
            proc100fact = ListTarget.Sum(it => it.tt_proc_fact);
            sum_fact = ListTarget.Sum(it => it.tt_sum_fact);

            if (/*isSkip == 0 &&*/ sender is TargetTask tt)
            {
                if(e.PropertyName == "tt_sum_fact"/* && isSkip != 2*/)
                {
                    if (_stopwatch.ElapsedMilliseconds > 20)
                    {
                        //isSkip = 1;
                        tt.tt_proc_fact = tt.tt_sum_fact * 100 / premBase;
                        _stopwatch.Restart();
                    }
                }
                if (e.PropertyName == "tt_proc_fact"/* && isSkip != 1*/)
                {
                    if (_stopwatch.ElapsedMilliseconds > 20)
                    {
                        //isSkip = 2;
                        tt.tt_sum_fact = premBase * tt.tt_proc_fact / 100;
                        _stopwatch.Restart();
                    }                
                }
            }
            //else
                //isSkip = 0;

        }

        #region Команды =================================

        //--------------------------------------------------------------------------------
        // Команда Кнопка ОК
        //--------------------------------------------------------------------------------
        public ICommand OkCommand => new LambdaCommand(OnOkCommandExecuted, CanOkCommand);
        private bool CanOkCommand(object p) => !IsReadOnly;
        private void OnOkCommandExecuted(object p)
        {
            TasksPersonWindow win = App.Current.Windows.OfType<TasksPersonWindow>().FirstOrDefault();
            win.DialogResult = true;
            win.Close();

        }
        //--------------------------------------------------------------------------------
        // Команда Кнопка Отменить
        //--------------------------------------------------------------------------------
        public ICommand CancelCommand => new LambdaCommand(OnCancelCommandExecuted, CanCancelCommand);
        private bool CanCancelCommand(object p) => true;
        private void OnCancelCommandExecuted(object p)
        {
            TasksPersonWindow win = App.Current.Windows.OfType<TasksPersonWindow>().FirstOrDefault();
            win.Close();
        }

        //--------------------------------------------------------------------------------
        // Команда Добавить файл
        //--------------------------------------------------------------------------------
        public ICommand AttachFileCommand => new LambdaCommand(OnAttachFileCommandExecuted, CanAttachFileCommand);
        private bool CanAttachFileCommand(object p) => string.IsNullOrEmpty(SelectedTarget.tt_AttachFile);
        private void OnAttachFileCommandExecuted(object p)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            Random rnd = new Random();
            //dlgOpen.Multiselect = true;

            if (dlgOpen.ShowDialog() == true)
            {
                RepositoryFiles RepFile = new RepositoryFiles();
                AttachFile af = new AttachFile() { FullName = dlgOpen.FileName, task_id = rnd.Next() };
                RepFile.AddFiles(af, Year);

                SelectedTarget.tt_AttachFile = af.FileName;
                SelectedTarget.tt_idFile = af.task_id;
            }

        }
        //--------------------------------------------------------------------------------
        // Команда Удалить файл
        //--------------------------------------------------------------------------------
        public ICommand DetachFileCommand => new LambdaCommand(OnDetachFileCommandExecuted, CanDetachFileCommand);
        private bool CanDetachFileCommand(object p) => !string.IsNullOrEmpty(SelectedTarget.tt_AttachFile);
        private void OnDetachFileCommandExecuted(object p)
        {
            RepositoryFiles RepFile = new RepositoryFiles();
            if (!string.IsNullOrEmpty(SelectedTarget.tt_AttachFile))
            {
                AttachFile af = new AttachFile() { FileName = SelectedTarget.tt_AttachFile, task_id = SelectedTarget.tt_idFile.Value };
                RepFile.DeleteFiles(af, Year);
                SelectedTarget.tt_AttachFile = null;
            }
        }

        //--------------------------------------------------------------------------------
        // Команда Открыть файл
        //--------------------------------------------------------------------------------
        public ICommand StartFileCommand => new LambdaCommand(OnStartFileCommandExecuted, CanStartFileCommand);
        private bool CanStartFileCommand(object p) => !string.IsNullOrEmpty(SelectedTarget.tt_AttachFile);
        private void OnStartFileCommandExecuted(object p)
        {
            RepositoryFiles RepFile = new RepositoryFiles();
            AttachFile af = new AttachFile() { FileName = SelectedTarget.tt_AttachFile, task_id = SelectedTarget.tt_idFile.Value };
            string FileName = RepFile.GetFile(af, Year);
            Process.Start(FileName);

        }

        //--------------------------------------------------------------------------------
        // Команда удалить строку
        //--------------------------------------------------------------------------------
        public ICommand DeleteLineCommand => new LambdaCommand(OnDeleteLineCommandExecuted, CanDeleteLineCommand);
        private bool CanDeleteLineCommand(object p) => true;
        private void OnDeleteLineCommandExecuted(object p)
        {
            if(p is TargetTask tt)
            {
                if(MessageBox.Show($"Удалить строку ­«{tt.tt_name}»", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    ListTarget.Remove(tt);
                }

            }

        }

        #endregion

    }
}
