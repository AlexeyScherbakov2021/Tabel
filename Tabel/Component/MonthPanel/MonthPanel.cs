using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tabel.Component.MonthPanel
{
    internal class MonthPanel : Panel
    {
        public static readonly DependencyProperty StartIndexProperty =
            DependencyProperty.Register("StartIndex", typeof(int), typeof(MonthPanel),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public int StartIndex
        {
            get { return (int)GetValue(StartIndexProperty); }
            set { SetValue(StartIndexProperty, value); }
        }

        public static readonly DependencyProperty VerticalSpaceProperty =
            DependencyProperty.Register("VerticalSpace", typeof(double), typeof(MonthPanel),
                new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double VerticalSpace
        {
            get { return (double)GetValue(VerticalSpaceProperty); }
            set { SetValue(VerticalSpaceProperty, value); }
        }

        public static readonly DependencyProperty HorizontalSpaceProperty =
            DependencyProperty.Register(" HorizontalSpace", typeof(double), typeof(MonthPanel),
                new FrameworkPropertyMetadata(24d, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double HorizontalSpace
        {
            get { return (double)GetValue(HorizontalSpaceProperty); }
            set { SetValue(HorizontalSpaceProperty, value); }
        }

        public void SetStartIndex(int ind)
        {
            StartIndex = ind;
        }



        protected override Size MeasureOverride(Size availableSize)
        {
            foreach(UIElement child in Children)
            {
                if(child != null)
                {
                    child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                }
            }

            return new Size(HorizontalSpace * 6, VerticalSpace * 7);
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            int CurrentIndex = StartIndex;

            Point location = new Point(0, StartIndex * VerticalSpace);

            foreach (UIElement child in Children)
            {
                child.Arrange( new Rect( location, child.DesiredSize));
                location.Y += VerticalSpace;
                if (CurrentIndex++ >= 6)
                {
                    location.X += HorizontalSpace;
                    location.Y = 0;
                    CurrentIndex = 0;
                }
            }

            return finalSize;
        }

    }
}
