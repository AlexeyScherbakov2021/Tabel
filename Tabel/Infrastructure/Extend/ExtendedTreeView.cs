using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using System.Windows.Input;

namespace Tabel.Infrastructure.Extend
{
    public class ExtendedTreeView : TreeView
    {
        public ExtendedTreeView()
            : base()
        {
            this.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(___ICH);
        }

        void ___ICH(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedItem != null)
            {
                SetValue(SelectedItem_Property, SelectedItem);
            }
        }

        public object SelectedItem_
        {
            get { return (object)GetValue(SelectedItem_Property); }
            set { SetValue(SelectedItem_Property, value); }
        }

        public static readonly DependencyProperty SelectedItem_Property = 
            DependencyProperty.Register("SelectedItem_", typeof(object), typeof(ExtendedTreeView), new UIPropertyMetadata(null));



        //private static readonly PropertyInfo IsSelectionChangeActiveProperty = typeof(TreeView).GetProperty
        //    (
        //      "IsSelectionChangeActive",
        //      BindingFlags.NonPublic | BindingFlags.Instance
        //    );


        //public static void AllowMultiSelection(TreeView treeView)
        //{
        //    if (IsSelectionChangeActiveProperty == null) return;

        //    var selectedItems = new List<TreeViewItem>();
        //    treeView.SelectedItemChanged += (a, b) =>
        //    {
        //        var treeViewItem = treeView.SelectedItem as TreeViewItem;
        //        if (treeViewItem == null) return;

        //        // allow multiple selection
        //        // when control key is pressed
        //        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        //        {
        //            // suppress selection change notification
        //            // select all selected items
        //            // then restore selection change notifications
        //            var isSelectionChangeActive =
        //              IsSelectionChangeActiveProperty.GetValue(treeView, null);

        //            IsSelectionChangeActiveProperty.SetValue(treeView, true, null);
        //            selectedItems.ForEach(item => item.IsSelected = true);

        //            IsSelectionChangeActiveProperty.SetValue
        //            (
        //              treeView,
        //              isSelectionChangeActive,
        //              null
        //            );
        //        }
        //        else
        //        {
        //            // deselect all selected items except the current one
        //            selectedItems.ForEach(item => item.IsSelected = (item == treeViewItem));
        //            selectedItems.Clear();
        //        }

        //        if (!selectedItems.Contains(treeViewItem))
        //        {
        //            selectedItems.Add(treeViewItem);
        //        }
        //        else
        //        {
        //            // deselect if already selected
        //            treeViewItem.IsSelected = false;
        //            selectedItems.Remove(treeViewItem);
        //        }
        //    };

        //}

    }

}
