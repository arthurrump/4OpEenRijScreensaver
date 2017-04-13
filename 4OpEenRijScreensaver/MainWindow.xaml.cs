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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _4OpEenRijScreensaver
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown();
        }

        Point _mousePos;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePos != default(Point) && _mousePos != e.GetPosition(MainWindowWindow))
                App.Current.Shutdown();

            _mousePos = e.GetPosition(MainWindowWindow);
        }
    }
}
