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
using Tabel.Models2;

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


        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(object), typeof(TabelControl));

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }


        public TabelControl()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------------------------------------
        // событие получения фокуса выпадающего списка типов дней
        //-----------------------------------------------------------------------------------------
        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var Parent = ((sender as ComboBox).Parent as StackPanel).Parent as StackPanel;
            TextBlock tb = Parent.Children[0] as TextBlock;
            int index = int.Parse(tb.Text);

            if (!IsSelectInRange(index))
                ListBoxDays.SelectedIndex = index - 1;
        }


        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        private bool IsSelectInRange(int index)
        {
            if (ListBoxDays.SelectedItems is null)
                return false;

            foreach (TabelDay item in ListBoxDays.SelectedItems)
                if (item.td_Day == index)
                    return true;

            return false;
        }

        //-----------------------------------------------------------------------------------------
        // событие получния фокуса часам дня
        //-----------------------------------------------------------------------------------------
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            var Parent = (tbox.Parent as StackPanel).Parent as StackPanel;
            TextBlock tb = Parent.Children[0] as TextBlock;
            
            int index = int.Parse(tb.Text);
            if (!IsSelectInRange(index))
                ListBoxDays.SelectedIndex = index - 1;

            tbox.SelectAll();
        }

        //-----------------------------------------------------------------------------------------
        // выбор типа дня в выпадающем списке
        //-----------------------------------------------------------------------------------------
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ListBoxDays.SelectedItems.Count > 1)
            {
                ComboBox cb = sender as ComboBox;

                foreach (TabelDay item in ListBoxDays.SelectedItems)
                {
                    item.td_KindId = cb.SelectedIndex;
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        // изменение часов
        //-----------------------------------------------------------------------------------------
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (ListBoxDays.SelectedItems.Count > 1)
            //{
            //    TextBox tb = sender as TextBox;

            //    foreach (TabelDay item in ListBoxDays.SelectedItems)
            //    {
            //        item.td_Hours = int.Parse(tb.Text);
            //    }
            //}
        }
    }
}
