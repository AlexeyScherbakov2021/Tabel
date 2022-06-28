using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Tabel.Views
{
    /// <summary>
    /// Логика взаимодействия для AddFormStringWindow.xaml
    /// </summary>
    public partial class AddFormStringWindow : Window
    {
        public string ResultValue;

        public AddFormStringWindow(string title, string placeHolder = "")
        {
            InitializeComponent();
            Title = title;
            //ResultValue = placeHolder;
            resultText.Text = placeHolder;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
