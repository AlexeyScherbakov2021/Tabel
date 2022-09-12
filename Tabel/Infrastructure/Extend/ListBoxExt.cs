using System;
using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tabel.Infrastructure.Extend
{
    internal class ListBoxExt : ListBox
    {

        public static readonly DependencyProperty SelectedItems2Property = DependencyProperty.RegisterAttached(
            nameof(SelectedItems2),
            typeof(IList),
            typeof(ListBoxExt),
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

    }
}
