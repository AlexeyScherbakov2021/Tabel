using Microsoft.Xaml.Behaviors;
using StartButton;
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
        //private bool IsDrag = false;
        //private bool IsDragEnabled = true;
        private Point StartPoint;

        //public static readonly DependencyProperty TBProperty =
        //DependencyProperty.Register("TB", typeof(TextBlock), typeof(DataGridBehavior));

        //public TextBlock TB
        //{
        //    get { return (TextBlock)GetValue(TBProperty); }
        //    set { SetValue(TBProperty, value); }
        //}

        public static readonly DependencyProperty DragGridProperty =
        DependencyProperty.Register("DragGrid", typeof(Grid), typeof(DataGridBehavior));

        public Grid DragGrid
        {
            get { return (Grid)GetValue(DragGridProperty); }
            set { SetValue(DragGridProperty, value); }
        }


        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            //AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += AssociatedObject_PreviewMouseMove;
            //AssociatedObject.BeginningEdit += AssociatedObject_BeginningEdit;
            //AssociatedObject.CellEditEnding += AssociatedObject_CellEditEnding;
            //AssociatedObject.QueryContinueDrag += AssociatedObject_QueryContinueDrag; 
            //base.OnAttached();
        }

        //private void AssociatedObject_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        //{
        //    Point pt = Mouse.GetPosition(AssociatedObject);

        //}

        //private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (IsDrag)
        //    {
        //        IsDrag = false;
        //        DragDropEffects ddEffect = DragDropEffects.Move;
        //        var data = (AssociatedObject.Items[AssociatedObject.SelectedIndex] as Personal);
        //        DataObject data2 = new DataObject("Person", data, false);
        //        //DragDrop.DoDragDrop(AssociatedObject, data2, ddEffect);

        //        AssociatedObject.ReleaseMouseCapture();
        //        //TB.Visibility = Visibility.Hidden;
        //    }
        //}

        //private void AssociatedObject_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        //{
        //    Point pt = Mouse.GetPosition(AssociatedObject);
        //    //pt = new Point(pt.X + 750, pt.Y + 240);

        //    TB.Margin = new Thickness(pt.X + 760, pt.Y + 240, 0, 0);
        //}

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseMove -= AssociatedObject_PreviewMouseMove;
            //AssociatedObject.BeginningEdit -= AssociatedObject_BeginningEdit;
            //AssociatedObject.CellEditEnding -= AssociatedObject_CellEditEnding;
        }

        //private void AssociatedObject_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    IsDragEnabled = true;
        //}

        //private void AssociatedObject_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        //{
        //    IsDragEnabled = false;
        //}

        private void AssociatedObject_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point pt = e.GetPosition(AssociatedObject);
            var diff = StartPoint - pt;

            if (e.LeftButton == MouseButtonState.Pressed && (diff.X > 5 || diff.Y > 5))
            {
                DragDropEffects ddEffect = DragDropEffects.Move;
                var data = (AssociatedObject.Items[AssociatedObject.SelectedIndex] as Personal);
                DataObject data2 = new DataObject("Person", data, false);
                DragDrop.DoDragDrop(AssociatedObject, data2, ddEffect);
            }

            //if (IsDrag)
            //{
            //    Point pt = e.GetPosition(AssociatedObject);
            //    pt = AssociatedObject.PointToScreen(pt);
            //    pt = DragGrid.PointFromScreen(pt);
            //    //TB.Margin = new Thickness(pt.X, pt.Y, 0, 0);
            //    e.Handled= true;
            //}

            //if (e.LeftButton == MouseButtonState.Pressed && !IsDrag && IsDragEnabled)
            //{
            //    //DataGrid dg = sender as DataGrid;
            //    Point pt = e.GetPosition(AssociatedObject);

            //    var diff = pt - StartPoint;
            //    if (diff.X < 5 && diff.Y < 5)
            //        return;

            //    var hit = VisualTreeHelper.HitTest(AssociatedObject, e.GetPosition(AssociatedObject));
            //    var typ = hit?.VisualHit.GetType();
            //    if (typ.Name == "TextBlock" && pt.Y > 24 && AssociatedObject.SelectedIndex >= 0)
            //    {
            //        var data = (AssociatedObject.Items[AssociatedObject.SelectedIndex] as Personal);
            //        DataObject data2 = new DataObject("Person", data, false);

            //        TB.Visibility = Visibility.Visible;
            //        TB.Text = data.FIO;
                    
            //        IsDrag = true;
            //        AssociatedObject.CaptureMouse();
            //        //IsDrag = false;
            //        //TB.Visibility = Visibility.Hidden;

            //    }
            //}
        }


        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StartPoint = e.GetPosition(AssociatedObject);
        }

    }
}
