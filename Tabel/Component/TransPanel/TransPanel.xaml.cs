using DocumentFormat.OpenXml.Drawing;
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

namespace Tabel.Component.TransPanel
{
    /// <summary>
    /// Логика взаимодействия для TransPanel.xaml
    /// </summary>
    public partial class TransPanel : ListBox
    {
        ListBoxItem StartItem;
        int StartIndex = -1;


        public static readonly RoutedEvent ContextMenuCheckEvent =
            EventManager.RegisterRoutedEvent("ContextMenuCheck", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(TransPanel));

        public event RoutedEventHandler ContextMenuCheck
        {
            add { AddHandler(ContextMenuCheckEvent, value); }
            remove { RemoveHandler(ContextMenuCheckEvent, value); }
        }

        public static readonly RoutedEvent ContextMenuUnCheckEvent =
            EventManager.RegisterRoutedEvent("ContextMenuUnCheck", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(TransPanel));

        public event RoutedEventHandler ContextMenuUnCheck
        {
            add { AddHandler(ContextMenuUnCheckEvent, value); }
            remove { RemoveHandler(ContextMenuUnCheckEvent, value); }
        }


        public TransPanel()
        {
            InitializeComponent();
        }



        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //base.OnPreviewMouseLeftButtonDown(e);

            //if (e.Source is TransPanel ov)
            //{
                Border border = e.OriginalSource as Border;

                if (border != null && border.Name == "Bd")
                {
                    ListBoxItem CurrentListBoxItem = (ListBoxItem)ContainerFromElement(border);


                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {

                        //ContentPresenter child = (ContentPresenter)border.Child;
                        int EndIndex = Items.IndexOf(CurrentListBoxItem.Content);

                        int n = StartIndex < EndIndex ? 1 : -1;

                        SelectedItems.Clear();

                        for (int i = StartIndex; i != EndIndex; i += n)
                            SelectedItems.Add(Items[i]);

                        SelectedItems.Add(Items[EndIndex]);

                        e.Handled = true;
                        return;
                    }

                    StartItem = CurrentListBoxItem;

                    if (StartItem != null)
                    {

                        //ContentPresenter child = (ContentPresenter)border.Child;
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            // добавдение выделенного элемента
                            SelectedItems.Add(StartItem.Content);
                        }
                        else
                        {
                            SelectedItems.Clear();
                            SelectedItem = StartItem.Content;
                        }

                        FocusManager.SetFocusedElement(this, StartItem);
                        e.Handled = true;
                    }

                    StartIndex = SelectedIndex;
                }
                else
                {
                    SelectedItems.Clear();
                }
            //}
        }


        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //if (e.Source is TransPanel ov)
                //{
                    ListBoxItem lbi = (ListBoxItem)ContainerFromElement((DependencyObject)e.OriginalSource);
                    if (lbi != null)
                        lbi.IsSelected = true;
                //}
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ContextMenuCheckEvent);
            args.Source = this;
            RaiseEvent(args);

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ContextMenuUnCheckEvent);
            args.Source = this;
            RaiseEvent(args);

        }
    }
}
