using System;
using System.Collections.Generic;
using System.Text;

static class NQueens
{
    public static void Generate(int n)
    {
        List<Tuple<int, int>> board = [];

        bool[] colsOccupied = new bool[n],
            diagDown = new bool[2 * n - 1],
            diagUp = new bool[2 * n - 1];

        bool Solve(int row)
        {
            if (row == n)
            {
                return true;
            }

            for (int col = 0; col < n; col++)
            {
                // Note: We don't need to check for same row conflicts because our algorithm
                // places queens row by row, ensuring only one queen per row by design.
                // The 'board' list only contains queens from previous rows (0 to row-1).

                int downDiag = row + col, upDiag = row - col + n - 1;

                if (colsOccupied[col] || diagDown[downDiag] || diagUp[upDiag])
                {
                    continue;
                }

                board.Add(new(row, col));
                colsOccupied[col] = true;
                diagDown[downDiag] = true;
                diagUp[upDiag] = true;

                if (Solve(row + 1))
                {
                    return true;
                }

                board.RemoveAt(board.Count - 1);
                colsOccupied[col] = false;
                diagDown[downDiag] = false;
                diagUp[upDiag] = false;
            }

            return false;
        }

        if (Solve(0))
        {
            Console.WriteLine($"Solution for {n} Queens:");

            char[,] chessboard = new char[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    chessboard[i, j] = '.';
                }
            }

            foreach (var pos in board)
            {
                chessboard[pos.Item1, pos.Item2] = 'Q';
            }

            StringBuilder sb = new();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    sb.Append($"{chessboard[i, j]} ");
                }
                sb.AppendLine();
            }

            Console.Write(sb);

            return;
        }

        Console.WriteLine($"No solution exists for {n} queens.");
    }
}
