using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Tabel.Component.ListBoxEasySelect
{
    internal class ListBoxEasySelect : ListBox
    {
        public static readonly DependencyProperty SelectedItems2Property = DependencyProperty.RegisterAttached(
            nameof(SelectedItems2),
            typeof(IList),
            typeof(ListBoxEasySelect),
            new PropertyMetadata(default(IList))
            );

        public IList SelectedItems2
        {
            get => (IList)GetValue(SelectedItems2Property);
            set => throw new Exception("IsReadOnly");
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            SetValue(SelectedItems2Property, base.SelectedItems);
        }

        private object _OldItem = null;

        //public ListBoxEasySelect() : base() { }

        private object GetPresentation(Object obj)
        {
            var obj2 = ContainerFromElement((DependencyObject)obj);

            if (obj2 is null)
                return null;

            var obj3 = VisualTreeHelper.GetChild(obj2, 0);
            var sender = VisualTreeHelper.GetChild(obj3, 0);
            object selectItem = (sender as ContentPresenter).Content;
            return selectItem;
        }



        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            //e.Handled = true;

            var selectItem = GetPresentation(e.OriginalSource);

            if (selectItem is null)
                return;

            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                //object selectITem = (sender as ContentPresenter).Content;

                SelectedItems.Clear();
                int index1 = Items.IndexOf(_OldItem);
                int index2 = Items.IndexOf(selectItem);

                for (int i = Math.Min(index1, index2); i <= Math.Max(index1, index2); i++)
                {
                    SelectedItems.Add(Items[i]);
                }

                return;
            }

            _OldItem = selectItem;

            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                SelectedItems.Clear();

            SelectedItems.Add(_OldItem);

        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {

                var item = GetPresentation(e.OriginalSource);
                if (item is null)
                    return;

                //object item = (sender as ContentPresenter).Content;
                SelectedItems.Add(item);
                e.Handled = true;
            }

            //base.OnPreviewMouseMove(e);
        }

    }
}
