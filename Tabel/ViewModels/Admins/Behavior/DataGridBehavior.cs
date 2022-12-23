using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tabel.Models;

namespace Tabel.ViewModels.Admins.Behavior
{
    internal class DataGridBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            //base.OnAttached();
        }
        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
        }

        private void AssociatedObject_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DataGrid dg = sender as DataGrid;
                var data = (dg.Items[dg.SelectedIndex] as Personal);

                DataObject data2 = new DataObject("Person", data, false);


                DragDropEffects ddEffect = DragDropEffects.All;
                DragDrop.DoDragDrop(dg, data2, ddEffect);
            }
        }



        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

    }
}
