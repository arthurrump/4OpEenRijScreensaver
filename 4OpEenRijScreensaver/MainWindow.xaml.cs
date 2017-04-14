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
    enum Field
    {
        Empty,
        Zero,
        One,
        Two,
        Three
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        int _cols, _rows;
        double _r;
        Field[,] _board;

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _cols = 19; // TODO: decide based on actual screensize
            _r = MainWindowWindow.ActualWidth / _cols;
            _rows = (int)Math.Round(MainWindowWindow.ActualHeight / _r);
            _board = new Field[_cols, _rows];

            var r = new Random();

            SolidColorBrush[] players =
            {
                new SolidColorBrush(
                        Color.FromRgb((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256))
                    ),
                new SolidColorBrush(
                        Color.FromRgb((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256))
                    ),
                new SolidColorBrush(
                        Color.FromRgb((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256))
                    ),
                new SolidColorBrush(
                        Color.FromRgb((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256))
                    )
            };

            for (int i = 0; i < _cols; i++)
            {
                MainGrid.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                );

                var s = new StackPanel { VerticalAlignment = VerticalAlignment.Bottom };
                s.SetValue(Grid.ColumnProperty, i);
                MainGrid.Children.Add(s);
            }


            for (int i = 0; i < _cols * _rows; i++)
            {
                int col;
                do
                {
                    col = r.Next(_cols);
                } while (_board[col, _rows - 1] != Field.Empty);

                for (int j = 0; j < _rows; j++)
                    if (_board[col, j] == Field.Empty)
                    {
                        _board[col, j] = (Field)(i % 4 + 1);
                        break;
                    }

                var el = new Ellipse
                {
                    Fill = players[i % 4],
                    Width = _r - 8.5,
                    Height = _r - 8.5,
                    Margin = new Thickness(4)
                };

                ((Panel)MainGrid.Children[col]).Children.Insert(0, el);

                await Task.Delay(1000);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
#if !DEBUG
            App.Current.Shutdown();
#endif
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Shutdown();
        }

        Point _mousePos;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
#if !DEBUG
            if (_mousePos != default(Point) && _mousePos != e.GetPosition(MainWindowWindow))
                App.Current.Shutdown();

            _mousePos = e.GetPosition(MainWindowWindow);
#endif
        }
    }
}
