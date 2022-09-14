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

namespace Tabel.Component.TabelPanel
{
    /// <summary>
    /// Логика взаимодействия для TabelControl.xaml
    /// </summary>
    public partial class TabelControl : UserControl
    {

        public static readonly DependencyProperty SourceCollectionProperty =
        DependencyProperty.Register("SourceCollection", typeof(IEnumerable), typeof(TabelControl));

        public IEnumerable SourceCollection
        {
            get { return (IEnumerable)GetValue(SourceCollectionProperty); }
            set { SetValue(SourceCollectionProperty, value); }
        }

        public static readonly DependencyProperty ItemsKindProperty =
        DependencyProperty.Register("ItemsKind", typeof(IEnumerable), typeof(TabelControl));

        public IEnumerable ItemsKind
        {
            get { return (IEnumerable)GetValue(ItemsKindProperty); }
            set { SetValue(ItemsKindProperty, value); }
        }



        public TabelControl()
        {
            InitializeComponent();
        }
    }
}
