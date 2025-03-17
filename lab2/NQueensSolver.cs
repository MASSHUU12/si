using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

static class NQueensSolver
{
    public enum SolveMethod
    {
        BFS,
        DFS
    }

    public class Statistics
    {
        public int OpenListCount { get; set; }
        public int ClosedListCount { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public List<(int, int)>? Solution { get; set; }
    }

    public static Statistics Solve(int n, SolveMethod method = SolveMethod.BFS)
    {
        var stopwatch = Stopwatch.StartNew();
        Statistics statistics = new();
        List<(int, int)> initialState = new();

        // Open list
        Queue<List<(int, int)>>? bfsQueue = null;
        Stack<List<(int, int)>>? dfsStack = null;

        if (method == SolveMethod.BFS)
        {
            bfsQueue = new();
            bfsQueue.Enqueue(initialState);
        }
        else
        {
            dfsStack = new();
            dfsStack.Push(initialState);
        }

        // Closed list to track visited states
        HashSet<string> closed = new();

        while ((method == SolveMethod.BFS && bfsQueue!.Count > 0) ||
               (method == SolveMethod.DFS && dfsStack!.Count > 0))
        {
            List<(int, int)> currentState;
            if (method == SolveMethod.BFS)
                currentState = bfsQueue!.Dequeue();
            else
                currentState = dfsStack!.Pop();

            // Convert state to string for closed list tracking
            string stateKey = StateToString(currentState);

            // Skip if already processed
            if (closed.Contains(stateKey))
                continue;

            closed.Add(stateKey);

            // Check if this is a final state (n queens placed with no conflicts)
            if (currentState.Count == n && !HasConflicts(currentState))
            {
                statistics.Solution = currentState;
                break;
            }

            List<List<(int, int)>> children = GenerateChildren(currentState, n);
            foreach (var child in children)
            {
                if (method == SolveMethod.BFS)
                    bfsQueue!.Enqueue(child);
                else
                    dfsStack!.Push(child);
            }
        }

        stopwatch.Stop();

        statistics.OpenListCount = method == SolveMethod.BFS
            ? bfsQueue!.Count
            : dfsStack!.Count;
        statistics.ClosedListCount = closed.Count;
        statistics.ExecutionTime = stopwatch.Elapsed;

        return statistics;
    }

    private static List<List<(int, int)>> GenerateChildren(List<(int, int)> state, int n)
    {
        List<List<(int, int)>> children = new();

        // If we have an empty board, we can place a queen in any position
        if (state.Count == 0)
        {
            for (int col = 0; col < n; col++)
            {
                List<(int, int)> newState = new() { (0, col) };
                children.Add(newState);
            }
            return children;
        }

        int nextRow = state.Count;

        // Try placing a queen in each column of the next row
        for (int col = 0; col < n; col++)
        {
            List<(int, int)> newState = new(state);
            newState.Add((nextRow, col));

            if (!HasConflicts(newState))
            {
                children.Add(newState);
            }
        }

        return children;
    }

    private static bool HasConflicts(List<(int, int)> board)
    {
        int n = board.Count;
        for (int i = 0; i < n - 1; i++)
        {
            var (row1, col1) = board[i];

            for (int j = i + 1; j < n; j++)
            {
                var (row2, col2) = board[j];

                if (col1 == col2 ||               // Same column
                    row1 == row2 ||               // Same row
                    row1 + col1 == row2 + col2 || // Same rising diagonal
                    row1 - col1 == row2 - col2)   // Same falling diagonal
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static string StateToString(List<(int, int)> state)
    {
        if (state.Count == 0)
            return "[]";

        StringBuilder sb = new();
        sb.Append('[');

        foreach (var (row, col) in state)
        {
            sb.Append($"({row},{col}),");
        }

        sb.Length--; // Remove the last comma
        sb.Append(']');

        return sb.ToString();
    }

    public static void PrintResults(Statistics stats, int n, SolveMethod method)
    {
        Console.WriteLine($"\nN-Queens solution for n={n} using {method} approach:");

        if (stats.Solution != null)
        {
            Console.WriteLine($"\nSolution state: {StateToString(stats.Solution)}");
            PrintBoard(stats.Solution, n);
        }
        else
        {
            Console.WriteLine("No solution found!");
        }

        Console.WriteLine("\nStatistics:");
        Console.WriteLine($"States in Open list: {stats.OpenListCount}");
        Console.WriteLine($"States in Closed list (checked): {stats.ClosedListCount}");
        Console.WriteLine($"Execution time: {stats.ExecutionTime.TotalMilliseconds} ms");
    }

    private static void PrintBoard(List<(int, int)> solution, int n)
    {
        StringBuilder sb = new();
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

        Console.WriteLine(sb);
    }

    public static void RunExperiments(int minN = 4, int maxN = 12)
    {
        Console.WriteLine("Running N-Queens experiments...");

        // Create arrays to store results for plotting
        int[] nValues = new int[maxN - minN + 1];
        int[] bfsOpenCounts = new int[maxN - minN + 1];
        int[] bfsClosedCounts = new int[maxN - minN + 1];
        double[] bfsTimes = new double[maxN - minN + 1];
        int[] dfsOpenCounts = new int[maxN - minN + 1];
        int[] dfsClosedCounts = new int[maxN - minN + 1];
        double[] dfsTimes = new double[maxN - minN + 1];

        for (int n = minN, i = 0; n <= maxN; n++, i++)
        {
            Console.WriteLine($"\n=== Testing n = {n} ===");
            nValues[i] = n;

            Console.WriteLine("\nRunning BFS...");
            Statistics bfsStats = Solve(n, SolveMethod.BFS);
            PrintResults(bfsStats, n, SolveMethod.BFS);
            bfsOpenCounts[i] = bfsStats.OpenListCount;
            bfsClosedCounts[i] = bfsStats.ClosedListCount;
            bfsTimes[i] = bfsStats.ExecutionTime.TotalMilliseconds;

            Console.WriteLine("\nRunning DFS...");
            Statistics dfsStats = Solve(n, SolveMethod.DFS);
            PrintResults(dfsStats, n, SolveMethod.DFS);
            dfsOpenCounts[i] = dfsStats.OpenListCount;
            dfsClosedCounts[i] = dfsStats.ClosedListCount;
            dfsTimes[i] = dfsStats.ExecutionTime.TotalMilliseconds;
        }

        StringBuilder sb = new();
        sb.AppendLine("\n=== Summary of Results ===");

        const string format = "{0,4} | {1,12} | {2,12} | {3,14} | {4,12} | {5,12} | {6,14}";

        sb.AppendLine(
            string.Format(
                format,
                "n",
                "BFS-Open",
                "BFS-Closed",
                "BFS-Time(ms)",
                "DFS-Open",
                "DFS-Closed",
                "DFS-Time(ms)"
            )
        );
        sb.AppendLine(new string('-', 98));

        for (int i = 0; i < nValues.Length; i++)
        {
            sb.AppendLine(
                string.Format(
                    format,
                    nValues[i],
                    bfsOpenCounts[i],
                    bfsClosedCounts[i],
                    bfsTimes[i].ToString("F2"),
                    dfsOpenCounts[i],
                    dfsClosedCounts[i],
                    dfsTimes[i].ToString("F2")
                )
            );
        }

        sb.AppendLine("\n--- CSV Format for Data Plotting ---");
        sb.AppendLine("n,BFS-Open,BFS-Closed,BFS-Time(ms),DFS-Open,DFS-Closed,DFS-Time(ms)");

        for (int i = 0; i < nValues.Length; i++)
        {
            sb.AppendLine($"{nValues[i]},{bfsOpenCounts[i]},{bfsClosedCounts[i]},{bfsTimes[i]:F2},{dfsOpenCounts[i]},{dfsClosedCounts[i]},{dfsTimes[i]:F2}");
        }

        sb.AppendLine("\nExperiments completed.");
        Console.WriteLine(sb);
    }
}
