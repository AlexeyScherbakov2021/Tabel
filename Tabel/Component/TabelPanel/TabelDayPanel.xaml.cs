using System;
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
using Tabel.Models;

namespace Tabel.Component.TabelPanel
{
    /// <summary>
    /// Логика взаимодействия для TabelDayPanel.xaml
    /// </summary>
    public partial class TabelDayPanel : ListBoxItem
    {
        public TabelDayPanel()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------------------------------------
        // выбор типа дня в выпадающем списке
        //-----------------------------------------------------------------------------------------
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (ListBoxDays.SelectedItems.Count > 1)
            //{
            //    ComboBox cb = sender as ComboBox;

            //    foreach (TabelDay item in ListBoxDays.SelectedItems)
            //    {
            //        item.td_KindId = cb.SelectedIndex;
            //    }
            //}
        }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //ListBoxDays.InvalidateMeasure();
            //ListBoxDays.InvalidateArrange();

        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //TextBox tbox = sender as TextBox;
            //if (tbox.Name != "TBHours")
            //    return;

            //var Parent = (tbox.Parent as StackPanel).Parent as StackPanel;
            //TextBlock tb = Parent.Children[0] as TextBlock;

            //int index = int.Parse(tb.Text.Substring(0, 2));
            //if (!IsSelectInRange(index))
            //    ListBoxDays.SelectedIndex = index - 1;

            //tbox.SelectAll();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

    }
}
