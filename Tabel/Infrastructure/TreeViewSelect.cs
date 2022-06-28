using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tabel.Infrastructure
{
    internal class TreeViewSelect : TreeView
    {
        public object SelectedItemTV
        {
            get => (object)GetValue(SelectedItemTVProperty);
            set => SetValue(SelectedItemTVProperty, value);
            //set => throw new Exception("This property is read-only. To bind to it you must use 'Mode=OneWayToSource'.");
        }


        public static readonly DependencyProperty SelectedItemTVProperty =
            DependencyProperty.Register("SelectedItemTV", typeof(object), typeof(TreeViewSelect),
                    new PropertyMetadata(default(object)));


        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
            SelectedItemTV = base.SelectedItem;
            //SetValue(SelectedItemTVProperty, base.SelectedItem);
        }


    }
}
