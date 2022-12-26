using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tabel.Commands;

namespace Tabel.Component.OtpuskPanel
{
    /// <summary>
    /// Логика взаимодействия для OtpuskPanel.xaml
    /// </summary>
    public partial class OtpuskPanel : UserControl
    {
        private object SelectedMonth;

        public static DependencyProperty ItemsSourceProperty = 
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(OtpuskPanel));

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }


        // событие редактирования отпуска
        public static readonly RoutedEvent EditOtpuskEvent =
          EventManager.RegisterRoutedEvent("EditOtpusk", RoutingStrategy.Bubble,
              typeof(RoutedEventHandler), typeof(OtpuskPanel));

        public event RoutedEventHandler EditOtpusk
        {
            add { AddHandler(EditOtpuskEvent, value); }
            remove { RemoveHandler(EditOtpuskEvent, value); }
        }

        // событие добавления отпуска
        public static readonly RoutedEvent AddOtpuskEvent =
          EventManager.RegisterRoutedEvent("AddOtpusk", RoutingStrategy.Bubble,
              typeof(RoutedEventHandler), typeof(OtpuskPanel));

        public event RoutedEventHandler AddOtpusk
        {
            add { AddHandler(AddOtpuskEvent, value); }
            remove { RemoveHandler(AddOtpuskEvent, value); }
        }

        // событие удаления отпуска
        public static readonly RoutedEvent DeleteOtpuskEvent =
          EventManager.RegisterRoutedEvent("DeleteOtpusk", RoutingStrategy.Bubble,
              typeof(RoutedEventHandler), typeof(OtpuskPanel));

        public event RoutedEventHandler DeleteOtpusk
        {
            add { AddHandler(DeleteOtpuskEvent, value); }
            remove { RemoveHandler(DeleteOtpuskEvent, value); }
        }




        //public static DependencyProperty TickDaysProperty = 
        //    DependencyProperty.Register("TickDays", typeof(double), typeof(OtpuskPanel));

        //public double TickDays
        //{
        //    get { return (double)GetValue(TickDaysProperty); }
        //    set { SetValue(TickDaysProperty, value); }
        //}



        public OtpuskPanel()
        {
            InitializeComponent();
            //DataContext = this;
            //lb.SizeChanged += Lb_SizeChanged;

        }

        //public ICommand AddOtpuskCommand => new LambdaCommand(AddOtpuskExec, CanAddOtpuskCommand);
        //bool CanAddOtpuskCommand(object p) => true;
        //void AddOtpuskExec(object p)
        //{
        //    RoutedEventArgs args;
        //    args = new RoutedEventArgs(AddOtpuskEvent);
        //    //args.Source = ;
        //    RaiseEvent(args);
        //}

        //-------------------------------------------------------------------------------------------
        // событие меню Редактировать отпуск
        //-------------------------------------------------------------------------------------------
        private void EditOtpusk_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args;
            args = new RoutedEventArgs(EditOtpuskEvent);
            args.Source = (sender as MenuItem).Tag;
            RaiseEvent(args);
        }

        //-------------------------------------------------------------------------------------------
        // событие меню Удалить отпуск
        //-------------------------------------------------------------------------------------------
        private void DeleteOtpusk_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args;
            args = new RoutedEventArgs(DeleteOtpuskEvent);
            args.Source = (sender as MenuItem).Tag;
            RaiseEvent(args);

        }

        //-------------------------------------------------------------------------------------------
        // событие меню Добавить отпуск
        //-------------------------------------------------------------------------------------------
        private void AddOtpusk_Click(object sender, RoutedEventArgs e)
        {

            RoutedEventArgs args;
            args = new RoutedEventArgs(AddOtpuskEvent);
            args.Source = SelectedMonth;
            RaiseEvent(args);
        }

        private void TextBlock_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectedMonth = sender;
        }
    }
}
