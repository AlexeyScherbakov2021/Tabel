//using DocumentFormat.OpenXml.Drawing;
//using DocumentFormat.OpenXml.Spreadsheet;
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
using Tabel.Infrastructure;
using Tabel.Models;

namespace Tabel.Component.TabelPanel
{
    /// <summary>
    /// Логика взаимодействия для TabelControl.xaml
    /// </summary>
    public partial class TabelControlLight : UserControl
    {

        //int StartIndex = -1;
        //ListBoxItem StartItem;

        //public static readonly DependencyProperty SourceCollectionProperty =
        //DependencyProperty.Register("SourceCollection", typeof(IEnumerable), typeof(TabelControlLight));

        //public IEnumerable SourceCollection
        //{
        //    get { return (IEnumerable)GetValue(SourceCollectionProperty); }
        //    set { SetValue(SourceCollectionProperty, value); }
        //}

        //public static readonly DependencyProperty ItemsKindProperty =
        //DependencyProperty.Register("ItemsKind", typeof(IEnumerable), typeof(TabelControlLight));

        //public IEnumerable ItemsKind
        //{
        //    get { return (IEnumerable)GetValue(ItemsKindProperty); }
        //    set { SetValue(ItemsKindProperty, value); }
        //}


        //public static readonly DependencyProperty SelectedItemProperty =
        //DependencyProperty.Register("SelectedItem", typeof(object), typeof(TabelControlLight));

        //public object SelectedItem
        //{
        //    get { return (object)GetValue(SelectedItemProperty); }
        //    set { SetValue(SelectedItemProperty, value); }
        //}


        public TabelControlLight()
        {
            InitializeComponent();
        }

        //-----------------------------------------------------------------------------------------
        // событие получения фокуса выпадающего списка типов дней
        //-----------------------------------------------------------------------------------------
        //private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    var Parent = ((sender as ComboBox).Parent as StackPanel).Parent as StackPanel;
        //    TextBlock tb = Parent.Children[0] as TextBlock;
        //    int index = int.Parse(tb.Text.Substring(0, 2));

        //    if (!IsSelectInRange(index))
        //        ListBoxDays.SelectedIndex = index - 1;
        //}


        //-----------------------------------------------------------------------------------------
        // проверка на выделенный диапазон
        //-----------------------------------------------------------------------------------------
        //private bool IsSelectInRange(int index)
        //{
        //    if (ListBoxDays.SelectedItems is null)
        //        return false;

        //    foreach (TabelDay item in ListBoxDays.SelectedItems)
        //        if (item.td_Day == index)
        //            return true;

        //    return false;
        //}

        //-----------------------------------------------------------------------------------------
        // событие получния фокуса часам дня
        //-----------------------------------------------------------------------------------------
        //private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox tbox = sender as TextBox;
        //    if (tbox.Name != "TBHours")
        //        return;

        //    var Parent = (tbox.Parent as StackPanel).Parent as StackPanel;
        //    TextBlock tb = Parent.Children[0] as TextBlock;

        //    int index = int.Parse(tb.Text.Substring(0, 2));
        //    if (!IsSelectInRange(index))
        //        ListBoxDays.SelectedIndex = index - 1;

        //    tbox.SelectAll();
        //}

        //-----------------------------------------------------------------------------------------
        // выбор типа дня в выпадающем списке
        //-----------------------------------------------------------------------------------------
        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (ListBoxDays.SelectedItems.Count > 1)
        //    {
        //        ComboBox cb = sender as ComboBox;

        //        foreach (TabelDay item in ListBoxDays.SelectedItems)
        //        {
        //            item.td_KindId = cb.SelectedIndex;
        //        }
        //    }
        //}


        //-----------------------------------------------------------------------------------------
        // событие нажатия кнопки
        //-----------------------------------------------------------------------------------------
        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        TextBox tb = sender as TextBox;
        //        decimal hours;
        //        decimal.TryParse(tb.Text, out hours);
        //        foreach (TabelDay item in ListBoxDays.SelectedItems)
        //        {
        //            item.td_Hours = hours;
        //        }
        //        e.Handled = true;

        //        if(ListBoxDays.SelectedItems.Count <= 1 )
        //            MoveToNextDay(ListBoxDays.SelectedIndex + 1);

        //    }

        //    if (e.Key == Key.Tab)
        //    {
        //        e.Handled = true;

        //        int napr = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
        //            ? ListBoxDays.SelectedIndex - 1
        //            : ListBoxDays.SelectedIndex + 1;

        //        if (napr < 0) return;

        //        MoveToNextDay(napr);

        //    }

        //}


        //private void MoveToNextDay(int newIndex)
        //{
        //    ListBoxDays.SelectedIndex = newIndex;

        //    var res = GetTabelPanelVisual("TabelPanel");

        //    ListBoxItem listBoxItem = (ListBoxItem)ProcessElement(res, "ListBoxItem");
        //    var parent = VisualTreeHelper.GetParent(listBoxItem);

        //    listBoxItem = (ListBoxItem)VisualTreeHelper.GetChild(parent, ListBoxDays.SelectedIndex);

        //    FrameworkElement elem = (FrameworkElement)ProcessElement(listBoxItem, "TextBox");

        //    elem.Focus();

        //}


        //-----------------------------------------------------------------------------------------
        // поиск элемента TabelPanel в графическом дереве
        //-----------------------------------------------------------------------------------------
        //private DependencyObject GetTabelPanelVisual(string type)
        //{
        //    return ProcessElement(ListBoxDays, type);
        //}

        //-----------------------------------------------------------------------------------------
        // поиск элемента указанного типа в графическом дереве
        //-----------------------------------------------------------------------------------------
        //public DependencyObject ProcessElement(DependencyObject element, string type)
        //{
        //    DependencyObject elem = null;

        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        //    {
        //        elem = VisualTreeHelper.GetChild(element, i);
        //        string typeName = elem.GetType().Name;
        //        if (elem.GetType().Name == type)
        //            return elem;

        //        elem = ProcessElement(elem, type);
        //        if (elem != null)
        //            return elem;
        //    }
        //    return elem;
        //}

        //private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    ListBoxDays.InvalidateMeasure();
        //    ListBoxDays.InvalidateArrange();

        //}

        //private void ListBoxDays_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ListBox lb = sender as ListBox;

        //    string TypeName = e.OriginalSource.GetType().Name;


        //    //if(TypeName == "Border")
        //    //{
        //    //    ListBoxItem CurrentListBoxItem2 = (ListBoxItem)lb.ContainerFromElement(e.OriginalSource as DependencyObject);
        //    //    StartItem = CurrentListBoxItem2;
        //    //    StartIndex = lb.SelectedIndex;
        //    //    if (lb.SelectedItems.Count < 2)
        //    //    {
        //    //        lb.SelectedItems.Clear();
        //    //        SelectedItem = StartItem.Content;
        //    //    }
        //    //    return;
        //    //}


        //    if (TypeName == "TextBoxView"
        //        //|| TypeName == "Border"
        //        || (e.OriginalSource as FrameworkElement).Tag?.ToString() == "NoSelect")
        //        return;


        //    //Border border = e.OriginalSource as Border;

        //    //if (border != null && border.Name == "Bd")
        //    //{
        //        ListBoxItem CurrentListBoxItem = (ListBoxItem)lb.ContainerFromElement(e.OriginalSource as DependencyObject);

        //        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
        //        {

        //            //ContentPresenter child = (ContentPresenter)border.Child;
        //            int EndIndex = lb.Items.IndexOf(CurrentListBoxItem.Content);

        //            int n = StartIndex < EndIndex ? 1 : -1;

        //            lb.SelectedItems.Clear();

        //            for (int i = StartIndex; i != EndIndex; i += n)
        //                lb.SelectedItems.Add(lb.Items[i]);

        //            lb.SelectedItems.Add(lb.Items[EndIndex]);

        //            e.Handled = true;
        //            return;
        //        }

        //        StartItem = CurrentListBoxItem;

        //        if (StartItem != null)
        //        {
        //            //ContentPresenter child = (ContentPresenter)border.Child;
        //            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        //            {
        //                // добавление выделенного элемента
        //                lb.SelectedItems.Add(StartItem.Content);
        //            }
        //            else if(TypeName == "Border" || TypeName == "ComboBox")
        //            {
        //                FocusManager.SetFocusedElement(this, StartItem);
        //                if(lb.SelectedItems.Count < 2)
        //                    lb.SelectedItems.Clear();
        //                return;
        //            }
        //            else
        //            {
        //                lb.SelectedItems.Clear();
        //                SelectedItem = StartItem.Content;
        //            }

        //            FocusManager.SetFocusedElement(this, StartItem);
        //            e.Handled = true;
        //        }

        //        StartIndex = lb.SelectedIndex;
        //    //}
        //    //else
        //    //{
        //    //    lb.SelectedItems.Clear();
        //    //}

        //}

        //private void ListBoxDays_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    ListBox lb = sender as ListBox;

        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        ListBoxItem lbi = (ListBoxItem)lb.ContainerFromElement((DependencyObject)e.OriginalSource);
        //        if (lbi != null)
        //            lbi.IsSelected = true;
        //    }

        //}
    }
}
