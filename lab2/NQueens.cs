using System;
using System.Collections.Generic;
using System.Text;

static class NQueens
{
    public enum SolveMethod : byte
    {
        BFS,
        DFS
    }

    public static void Solve(int n, SolveMethod method = SolveMethod.BFS)
    {
        List<List<(int, int)>> solutions;

        switch (method)
        {
            case SolveMethod.DFS:
                solutions = SolveDFS(n);
                break;
            case SolveMethod.BFS:
            default:
                solutions = SolveBFS(n);
                break;
        }

        PrintSolutions(solutions, n, method);
    }

    private static List<List<(int, int)>> SolveBFS(int n)
    {
        // Initialize the queue with boards having a queen in the first row at different columns
        Queue<List<(int, int)>> queens = new();
        for (int i = 0; i < n; i++)
        {
            queens.Enqueue(new() { (0, i) });
        }

        List<List<(int, int)>> answers = new();

        while (queens.Count > 0)
        {
            var board = queens.Dequeue();

            // If we have n queens, we have a solution
            if (board.Count == n)
            {
                answers.Add(board);
                continue;
            }

            // Current row to place the next queen
            int row = board.Count;

            // Try each column in the current row
            for (int i = 0; i < n; i++)
            {
                if (!IsUnderAttack(board, row, i))
                {
                    List<(int, int)> newBoard = new(board);
                    newBoard.Add((row, i));
                    queens.Enqueue(newBoard);
                }
            }
        }

        return answers;
    }

    private static List<List<(int, int)>> SolveDFS(int n)
    {
        List<List<(int, int)>> solutions = new();
        List<(int, int)> currentBoard = new();

        // Start the DFS from row 0
        DFSHelper(n, 0, currentBoard, solutions);

        return solutions;
    }

    private static void DFSHelper(
        int n,
        int row,
        List<(int, int)> currentBoard,
        List<List<(int, int)>> solutions
    )
    {
        // Base case: If we've placed queens in all rows, we have a solution
        if (row == n)
        {
            solutions.Add(new(currentBoard));
            return;
        }

        // Try placing a queen in each column of the current row
        for (int col = 0; col < n; col++)
        {
            if (!IsUnderAttack(currentBoard, row, col))
            {
                // Place the queen and move to the next row
                currentBoard.Add((row, col));
                DFSHelper(n, row + 1, currentBoard, solutions);

                // Backtrack by removing the queen to try other positions
                currentBoard.RemoveAt(currentBoard.Count - 1);
            }
        }
    }

    private static bool IsUnderAttack(List<(int, int)> board, int row, int col)
    {
        foreach (var (queenRow, queenCol) in board)
        {
            if (queenCol == col || // Same column
                queenRow + queenCol == row + col || // Same rising diagonal
                queenRow - queenCol == row - col)   // Same falling diagonal
            {
                return true;
            }
        }
        return false;
    }

    private static void PrintSolutions(
        List<List<(int, int)>> solutions,
        int n,
        SolveMethod method
    )
    {
        Console.WriteLine(
            $"Found {solutions.Count} solutions for {n}-queens puzzle using {method} approach:"
        );

        int solutionCount = 1;
        StringBuilder sb = new();

        foreach (var solution in solutions)
        {
            sb.Clear();
            sb.AppendLine($"Solution {solutionCount++}:");

            char[,] board = new char[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    board[i, j] = '.';

            foreach (var (row, col) in solution)
                board[row, col] = 'Q';

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    sb.Append(board[i, j]);
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            sb.AppendLine();

            Console.Write(sb.ToString());
        }
    }
}
