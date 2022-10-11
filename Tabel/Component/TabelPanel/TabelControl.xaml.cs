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
using System.Xml.Linq;
using Tabel.Models;

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
        // проверка на выделенный диапазон
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
        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //if (ListBoxDays.SelectedItems.Count > 1)
        //    //{
        //    //    TextBox tb = sender as TextBox;

        //    //    foreach (TabelDay item in ListBoxDays.SelectedItems)
        //    //    {
        //    //        item.td_Hours = int.Parse(tb.Text);
        //    //    }
        //    //}
        //}


        //-----------------------------------------------------------------------------------------
        // событие нажатия кноаки
        //-----------------------------------------------------------------------------------------
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                TextBox tb = sender as TextBox;
                decimal hours;
                decimal.TryParse(tb.Text, out hours);
                foreach (TabelDay item in ListBoxDays.SelectedItems)
                {
                    item.td_Hours = hours;
                }
                e.Handled = true;

                MoveToNextDay(ListBoxDays.SelectedIndex + 1);

            }

            if (e.Key == Key.Tab)
            {
                e.Handled = true;

                int napr = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift 
                    ? ListBoxDays.SelectedIndex - 1 
                    : ListBoxDays.SelectedIndex + 1;

                if (napr < 0) return;

                MoveToNextDay(napr);

                //ListBoxDays.SelectedIndex = napr;

                //var res = GetTabelPanelVisual("TabelPanel");

                //ListBoxItem listBoxItem = (ListBoxItem)ProcessElement(res, "ListBoxItem");
                //var parent = VisualTreeHelper.GetParent(listBoxItem);
                
                //listBoxItem = (ListBoxItem)VisualTreeHelper.GetChild(parent, ListBoxDays.SelectedIndex);

                //FrameworkElement elem =  (FrameworkElement)ProcessElement(listBoxItem, "TextBox");

                //elem.Focus();
            }

        }


        private void MoveToNextDay(int newIndex)
        {
            ListBoxDays.SelectedIndex = newIndex;

            var res = GetTabelPanelVisual("TabelPanel");

            ListBoxItem listBoxItem = (ListBoxItem)ProcessElement(res, "ListBoxItem");
            var parent = VisualTreeHelper.GetParent(listBoxItem);

            listBoxItem = (ListBoxItem)VisualTreeHelper.GetChild(parent, ListBoxDays.SelectedIndex);

            FrameworkElement elem = (FrameworkElement)ProcessElement(listBoxItem, "TextBox");

            elem.Focus();

        }


        //-----------------------------------------------------------------------------------------
        // поиск элемента TabelPanel в графическом дереве
        //-----------------------------------------------------------------------------------------
        private DependencyObject GetTabelPanelVisual(string type)
        {
            return ProcessElement(ListBoxDays, type);
        }

        //-----------------------------------------------------------------------------------------
        // поиск элемента указанного типа в графическом дереве
        //-----------------------------------------------------------------------------------------
        public DependencyObject ProcessElement(DependencyObject element, string type)
        {
            DependencyObject elem = null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                elem = VisualTreeHelper.GetChild(element, i);
                string typeName = elem.GetType().Name;
                if (elem.GetType().Name == type)
                    return elem;

                elem = ProcessElement(elem, type);
                if (elem != null)
                    return elem;
            }
            return elem;
        }


    }
}
