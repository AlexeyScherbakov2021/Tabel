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

namespace Tabel.Component.OtpuskPanel
{
    /// <summary>
    /// Логика взаимодействия для OtpuskPanel.xaml
    /// </summary>
    public partial class OtpuskPanel : UserControl
    {

        public static DependencyProperty ItemsSourceProperty = 
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(OtpuskPanel));

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly RoutedEvent EditOtpuskEvent =
          EventManager.RegisterRoutedEvent("EditOtpusk", RoutingStrategy.Bubble,
              typeof(RoutedEventHandler), typeof(OtpuskPanel));

        public event RoutedEventHandler EditOtpusk
        {
            add { AddHandler(EditOtpuskEvent, value); }
            remove { RemoveHandler(EditOtpuskEvent, value); }
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
            //lb.SizeChanged += Lb_SizeChanged;

        }

        private void EditOtpusk_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args;

            args = new RoutedEventArgs(EditOtpuskEvent);
            args.Source = (sender as MenuItem).Tag;
            RaiseEvent(args);
        }

        private void DeleteOtpusk_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddOtpusk_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
