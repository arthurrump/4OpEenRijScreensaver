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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Game game = new Game(4);
            await game.Start(
                DecideBoardSize, 
                SetupUI, 
                Draw, 
                BlinkFields, 
                ResetUI
            );
        }

        private ((int cols, int rows) size, double radius) DecideBoardSize()
        {
            var random = new Random();
            double width = MainWindowWindow.ActualWidth;
            double minR = 40, maxR = 160;

            int cols = random.Next(Math.Min((int)Math.Ceiling(width / maxR), 12), (int)Math.Floor(width / minR));
            double radius = width / cols;
            int rows = (int)Math.Ceiling(MainWindowWindow.Height / radius);

            return ((cols, rows), radius);
        }

        private void SetupUI(int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                MainGrid.ColumnDefinitions.Add(
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                );

                var s = new StackPanel { VerticalAlignment = VerticalAlignment.Bottom };
                s.SetValue(Grid.ColumnProperty, i);
                MainGrid.Children.Add(s);
            }
        }

        private void Draw(SolidColorBrush color, int column, double radius)
        {
            var el = new Ellipse
            {
                Fill = color,
                Width = radius - 8.5,
                Height = radius - 8.5,
                Margin = new Thickness(4)
            };
            ((Panel)MainGrid.Children[column]).Children.Insert(0, el);
        }

        private async Task BlinkFields((int column, int row)[] fields)
        {
            // In Game (0,0) is the lower left corner, in the UI it's the highest ellipse in the left column
            for (int j = 0; j < 4; j++)
                fields[j].row = ((Panel)MainGrid.Children[fields[j].column]).Children.Count - 1 - fields[j].row;

            UIElement[] ellipses =
            {
                ((Panel)MainGrid.Children[fields[0].column]).Children[fields[0].row],
                ((Panel)MainGrid.Children[fields[1].column]).Children[fields[1].row],
                ((Panel)MainGrid.Children[fields[2].column]).Children[fields[2].row],
                ((Panel)MainGrid.Children[fields[3].column]).Children[fields[3].row]
            };

            foreach (var ell in ellipses) ell.Opacity = 1;
            await Task.Delay(500);
            foreach (var ell in ellipses) ell.Opacity = 0;
            await Task.Delay(500);
            foreach (var ell in ellipses) ell.Opacity = 1;
            await Task.Delay(250);
            foreach (var ell in ellipses) ell.Opacity = 0;
            await Task.Delay(250);
            foreach (var ell in ellipses) ell.Opacity = 1;
            await Task.Delay(100);
            foreach (var ell in ellipses) ell.Opacity = 0;
            await Task.Delay(100);
        }

        private void ResetUI()
        {
            MainGrid.Children.Clear();
            MainGrid.ColumnDefinitions.Clear();
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
