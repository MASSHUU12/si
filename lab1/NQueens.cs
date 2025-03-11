using System;
using System.Collections.Generic;

static class NQueens
{
    public static void Generate(int n)
    {
        List<Tuple<int, int>> board = [];

        bool Solve(int row)
        {
            if (row == n)
            {
                return true;
            }

            for (int col = 0; col < n; col++)
            {
                if (!IsSafe(row, col))
                {
                    continue;
                }

                board.Add(new(row, col));

                if (Solve(row + 1))
                {
                    return true;
                }

                board.RemoveAt(board.Count - 1);
            }

            return false;
        }

        bool IsSafe(int row, int col)
        {
            // Note: We don't need to check for same row conflicts because our algorithm
            // places queens row by row, ensuring only one queen per row by design.
            // The 'board' list only contains queens from previous rows (0 to row-1).

            foreach (Tuple<int, int> pos in board)
            {
                (int r, int c) = pos;

                if (c == col || Math.Abs(r - row) == Math.Abs(c - col))
                {
                    return false;
                }
            }

            return true;
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

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write($"{chessboard[i, j]} ");
                }
                Console.WriteLine();
            }

            return;
        }

        Console.WriteLine($"No solution exists for {n} queens.");
    }
}
