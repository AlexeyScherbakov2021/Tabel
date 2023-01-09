using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Tabel.Models;

namespace Tabel.ViewModels.Admins.Behavior
{
    internal class DataGridBehavior : Behavior<DataGrid>
    {
        private bool IsDrag = false;
        private bool IsDragEnabled = true;
        private Point StartPoint;


        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            AssociatedObject.BeginningEdit += AssociatedObject_BeginningEdit;
            AssociatedObject.CellEditEnding += AssociatedObject_CellEditEnding;
            //base.OnAttached();
        }


        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;
            AssociatedObject.BeginningEdit -= AssociatedObject_BeginningEdit;
            AssociatedObject.CellEditEnding -= AssociatedObject_CellEditEnding;
        }

        private void AssociatedObject_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            IsDragEnabled = true;
        }

        private void AssociatedObject_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            IsDragEnabled = false;
        }


        private void AssociatedObject_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && !IsDrag && IsDragEnabled)
            {
                DataGrid dg = sender as DataGrid;
                Point pt = e.GetPosition(dg);

                var diff = pt - StartPoint;
                if (diff.X < 5 && diff.Y < 5)
                    return;

                var hit = VisualTreeHelper.HitTest(dg, e.GetPosition(dg));
                var typ = hit.VisualHit.GetType();
                if (typ.Name == "TextBlock" && pt.Y > 24 && dg.SelectedIndex >= 0)
                {
                    var data = (dg.Items[dg.SelectedIndex] as Personal);

                    DataObject data2 = new DataObject("Person", data, false);

                    DragDropEffects ddEffect = DragDropEffects.All;
                    IsDrag = true;
                    DragDrop.DoDragDrop(dg, data2, ddEffect);
                    IsDrag = false;
                }
            }
        }



        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StartPoint = e.GetPosition(sender as DataGrid);
        }

    }
}
