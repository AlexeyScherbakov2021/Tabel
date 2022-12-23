using System;
using System.Collections;
using System.Collections.Generic;
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



        public OtpuskPanel()
        {
            InitializeComponent();
        }
    }
}
