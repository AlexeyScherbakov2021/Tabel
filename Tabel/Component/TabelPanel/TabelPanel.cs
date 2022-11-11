using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Tabel.Component.TabelPanel
{
    internal class TabelPanel : Panel
    {
        double heightT = 0;
        double heightB = 0;


        // Количество дней на панели
        public static readonly DependencyProperty CountDaysProperty =
                DependencyProperty.Register("CountDays", typeof(int), typeof(TabelPanel),
                new FrameworkPropertyMetadata(30, FrameworkPropertyMetadataOptions.AffectsArrange));

        public int CountDays
        {
            get { return (int)GetValue(CountDaysProperty); }
            set { SetValue(CountDaysProperty, value); }
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            Size childSize = new Size();
            double widthT = 0;
            double widthB = 0;
            int nCount = 0;
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    child.Measure(size);
                    childSize = child.DesiredSize;
                    if (nCount < 15)
                    {
                        widthT += childSize.Width;
                        heightT = Math.Max(heightT, childSize.Height);
                    }
                    else
                    {
                        widthB += childSize.Width;
                        heightB = Math.Max(heightB, childSize.Height);
                    }
                    nCount++;
                }
            }

            //Size PanelSize = new Size(childSize.Width * 15, childSize.Height * 2);
            Size PanelSize = new Size(Math.Max(widthT, widthB), heightT + heightB);
            //if (Children.Count > 30)
            //    PanelSize.Width += childSize.Width;

            return PanelSize;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            int CurrentIndex = 0;

            Point location = new Point(0, 0);

            foreach (UIElement child in Children)
            {
                child.Arrange(new Rect(location, child.DesiredSize));
                location.X += child.DesiredSize.Width;
                if (CurrentIndex++ == 14)
                {
                    location.Y += heightT;
                    location.X = 0;
                }
            }

            return finalSize;
        }



    }
}
