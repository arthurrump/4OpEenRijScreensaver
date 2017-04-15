using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _4OpEenRijScreensaver
{
    public class Game
    {
        const int EMPTY = 0;

        int _cols, _rows, _playerCount;

        public Game(int columns, int rows, int playerCount)
        {
            _cols = columns;
            _rows = rows;
            _playerCount = playerCount;
        }

        public async Task Start(Action<SolidColorBrush, int> draw, Func<(int column, int row)[], Task> blink, Action reset)
        {
            while (true)
            {
                await Play(draw, blink);
                reset();
            }
        }

        private async Task Play(Action<SolidColorBrush, int> draw, Func<(int column, int row)[], Task> blink)
        {
            SolidColorBrush[] playerColors = GetPlayerColors(_playerCount);
            int[,] board = new int[_cols, _rows];

            for (int i = 0; i < _cols * _rows; i++)
            {
                (int num, SolidColorBrush color) currentPlayer = (i % _playerCount + 1, playerColors[i % _playerCount]);

                (int col, int row) = Next(ref board, currentPlayer.num);

                draw(currentPlayer.color, col);

                await Task.Delay(250);

                var winningMove = IsWinningMove(board, col, row, currentPlayer.num);
                if (winningMove.win)
                {
                    await blink(winningMove.fields);
                    break;
                }

                await Task.Delay(250);
            }
        }

        private (int column, int row) Next(ref int[,] board, int player)
        {
            int col, row = -1;
            var random = new Random();

            do
            {
                col = random.Next(_cols);
            } while (board[col, _rows - 1] != EMPTY);

            for (int r = 0; r < _rows; r++)
            {
                if (board[col, r] == EMPTY)
                {
                    board[col, r] = player;
                    row = r;
                    break;
                }
            }

            return (col, row);
        }

        public (bool win, (int column, int row)[] fields) IsWinningMove(int[,] board, int column, int row, int player)
        {
            // Horizontal
            for (int c = Math.Max(column - 3, 0); c <= column && c + 3 < _cols; c++)
                if (board[c, row] == player && board[c + 1, row] == player &&
                    board[c + 2, row] == player && board[c + 3, row] == player)
                    return (true, new(int, int)[] { (c, row), (c + 1, row), (c + 2, row), (c + 3, row) });

            // Vertical
            for (int r = Math.Max(row - 3, 0); r <= row && r + 3 < _rows; r++)
                if (board[column, r] == player && board[column, r + 1] == player &&
                    board[column, r + 2] == player && board[column, r + 3] == player)
                    return (true, new(int, int)[] { (column, r), (column, r + 1), (column, r + 2), (column, r + 3) });

            // Diagonal Bottom-up
            for (int d = Math.Min(column - 3, row - 3) < 0 ? -3 - Math.Min(column - 3, row - 3) : -3;
                d <= 0 && column + d + 3 < _cols && row + d + 3 < _rows; d++)
                if (board[column + d, row + d] == player && board[column + d + 1, row + d + 1] == player &&
                    board[column + d + 2, row + d + 2] == player && board[column + d + 3, row + d + 3] == player)
                    return (true, new(int, int)[] {
                            (column + d, row + d), (column + d + 1, row + d + 1), (column + d + 2, row + d + 2), (column + d + 3, row + d + 3)
                        });

            // Diagonal Top-down
            for (int d = Math.Min(column - 3, _rows - 1 - (row + 3)) < 0 ? -3 - Math.Min(column - 3, _rows - 1 - (row + 3)) : -3;
                d <= 0 && column + d + 3 < _cols && row - d - 3 >= 0; d++)
                if (board[column + d, row - d] == player && board[column + d + 1, row - d - 1] == player &&
                    board[column + d + 2, row - d - 2] == player && board[column + d + 3, row - d - 3] == player)
                    return (true, new(int, int)[] {
                            (column + d, row - d), (column + d + 1, row - d - 1), (column + d + 2, row - d - 2), (column + d + 3, row - d - 3)
                        });

            return (false, null);
        }

        private SolidColorBrush[] GetPlayerColors(int count)
        {
            var r = new Random();
            var result = new SolidColorBrush[count];

            for (int i = 0; i < count; i++)
                result[i] = new SolidColorBrush(Color.FromRgb((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256)));

            return result;
        }
    }
}
