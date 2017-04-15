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

            var rand = new Random();

            SolidColorBrush[] players =
            {
                new SolidColorBrush(
                        Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                    ),
                new SolidColorBrush(
                        Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                    ),
                new SolidColorBrush(
                        Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                    ),
                new SolidColorBrush(
                        Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
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
                int col, row = -1;
                Field player = (Field)(i % 4 + 1);

                do
                {
                    col = rand.Next(_cols);
                } while (_board[col, _rows - 1] != Field.Empty);

                for (int r = 0; r < _rows; r++)
                    if (_board[col, r] == Field.Empty)
                    {
                        _board[col, r] = player;
                        row = r;
                        break;
                    }

                var el = new Ellipse
                {
                    Fill = players[(int)player - 1],
                    Width = _r - 8.5,
                    Height = _r - 8.5,
                    Margin = new Thickness(4)
                };
                ((Panel)MainGrid.Children[col]).Children.Insert(0, el);

                await Task.Delay(250);

                // Check for 4-in-a-row around new filled field
                // Horizontal
                for (int c = Math.Max(col - 3, 0); c <= col && c + 3 < _cols; c++)
                    if (_board[c, row] == player && _board[c + 1, row] == player &&
                        _board[c + 2, row] == player && _board[c + 3, row] == player)
                        await Winner(new(int, int)[] { (c, row), (c + 1, row), (c + 2, row), (c + 3, row) });

                // Vertical
                for (int r = Math.Max(row - 3, 0); r <= row && r + 3 < _rows; r++)
                    if (_board[col, r] == player && _board[col, r + 1] == player &&
                        _board[col, r + 2] == player && _board[col, r + 3] == player)
                        await Winner(new(int, int)[] { (col, r), (col, r + 1), (col, r + 2), (col, r + 3) });

                // Diagonal Bottom-up
                for (int d = Math.Min(col - 3, row - 3) < 0 ? -3 - Math.Min(col - 3, row - 3) : -3;
                    d <= 0 && col + d + 3 < _cols && row + d + 3 < _rows; d++)
                    if (_board[col + d, row + d] == player && _board[col + d + 1, row + d + 1] == player &&
                        _board[col + d + 2, row + d + 2] == player && _board[col + d + 3, row + d + 3] == player)
                        await Winner(new(int, int)[] {
                            (col + d, row + d), (col + d + 1, row + d + 1), (col + d + 2, row + d + 2), (col + d + 3, row + d + 3)
                        });

                // Diagonal Top-down
                for (int d = Math.Min(col - 3, _rows - 1 - (row + 3)) < 0 ? -3 - Math.Min(col - 3, _rows - 1 - (row + 3)) : -3;
                    d <= 0 && col + d + 3 < _cols && row - d - 3 >= 0; d++)
                    if (_board[col + d, row - d] == player && _board[col + d + 1, row - d - 1] == player &&
                        _board[col + d + 2, row - d - 2] == player && _board[col + d + 3, row - d - 3] == player)
                        await Winner(new(int, int)[] {
                            (col + d, row - d), (col + d + 1, row - d - 1), (col + d + 2, row - d - 2), (col + d + 3, row - d - 3)
                        });

                await Task.Delay(250);

                async Task Winner((int c, int r)[] win)
                {
                    // In _board (0,0) is the lower left corner, in the UI it's the highest ellipse in the left column
                    for (int j = 0; j < 4; j++)
                        win[j].r = ((Panel)MainGrid.Children[win[j].c]).Children.Count - 1 - win[j].r;

                    UIElement[] ellipses =
                    {
                        ((Panel)MainGrid.Children[win[0].c]).Children[win[0].r],
                        ((Panel)MainGrid.Children[win[1].c]).Children[win[1].r],
                        ((Panel)MainGrid.Children[win[2].c]).Children[win[2].r],
                        ((Panel)MainGrid.Children[win[3].c]).Children[win[3].r]
                    };

                    foreach (var ell in ellipses) ell.Opacity = 1;
                    await Task.Delay(500);
                    foreach (var ell in ellipses) ell.Opacity = 0;
                    await Task.Delay(400);
                    foreach (var ell in ellipses) ell.Opacity = 1;
                    await Task.Delay(300);
                    foreach (var ell in ellipses) ell.Opacity = 0;
                    await Task.Delay(200);
                    foreach (var ell in ellipses) ell.Opacity = 1;
                    await Task.Delay(100);
                    foreach (var ell in ellipses) ell.Opacity = 0;
                    await Task.Delay(500);

                    _board = new Field[_cols, _rows];
                    i = 0;

                    foreach (Panel p in MainGrid.Children)
                        p.Children.Clear();

                    players = new SolidColorBrush[]
                    {
                        new SolidColorBrush(
                                Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                            ),
                        new SolidColorBrush(
                                Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                            ),
                        new SolidColorBrush(
                                Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                            ),
                        new SolidColorBrush(
                                Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                            )
                    };
                }
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
