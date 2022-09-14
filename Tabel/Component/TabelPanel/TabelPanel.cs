using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tabel.Component.TabelPanel
{
    internal class TabelPanel : Panel
    {
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
            Size size = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    child.Measure(size);
                    childSize = child.DesiredSize;

                }
            }

            Size PanelSize = new Size(childSize.Width * 15, childSize.Height * 2);
            if (Children.Count > 30)
                PanelSize.Width += childSize.Width;

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
                    location.Y += child.DesiredSize.Height;
                    location.X = 0;
                }
            }

            return finalSize;
        }



    }
}
