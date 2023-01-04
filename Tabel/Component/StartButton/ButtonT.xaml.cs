using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StartButton
{
    /// <summary>
    /// Логика взаимодействия для ButtonT.xaml
    /// </summary>
    public partial class ButtonT : Button
    {
        public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ButtonT));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }



        public ButtonT()
        {
            InitializeComponent();
            //DataContext = this;
        }
    }
}
