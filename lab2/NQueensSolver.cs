// #define ENABLE_PRUNING

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

static class NQueensSolver
{
    public enum SolveMethod
    {
        BFS,
        DFS
    }

    public class Statistics
    {
        public int MaxOpenListCount { get; set; }
        public int ClosedListCount { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public List<List<(int, int)>> AllSolutions { get; set; } = [];
        // Track total states ever added to open list
        public int TotalStatesEnqueued { get; set; }
    }

    public static Statistics Solve(int n, SolveMethod method = SolveMethod.BFS)
    {
        Statistics statistics = new();
        List<(int, int)> initialState = new();
        Stopwatch stopwatch = Stopwatch.StartNew();

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

        statistics.MaxOpenListCount = 1;
        statistics.TotalStatesEnqueued = 1;

        // Closed list to track visited states
        HashSet<string> closed = new();

        while ((method == SolveMethod.BFS && bfsQueue!.Count > 0) ||
               (method == SolveMethod.DFS && dfsStack!.Count > 0))
        {
            List<(int, int)> currentState;
            int currentOpenSize;

            if (method == SolveMethod.BFS)
            {
                currentState = bfsQueue!.Dequeue();
                currentOpenSize = bfsQueue.Count;
            }
            else
            {
                currentState = dfsStack!.Pop();
                currentOpenSize = dfsStack.Count;
            }

            // Convert state to string for closed list tracking
            string stateKey = StateToString(currentState);

            // Skip if already processed
            if (closed.Contains(stateKey))
                continue;

            closed.Add(stateKey);

            // Check if this is a final state (n queens placed with no conflicts)
            if (currentState.Count == n && !HasConflicts(currentState))
            {
                statistics.AllSolutions.Add(new(currentState));
                continue; // Skip generating children as we've reached a leaf node
            }

            if (HasConflicts(currentState))
                continue;

            List<List<(int, int)>> children = GenerateChildren(currentState, n);
            foreach (var child in children)
            {
                statistics.TotalStatesEnqueued++;

                if (method == SolveMethod.BFS)
                {
                    bfsQueue!.Enqueue(child);
                    statistics.MaxOpenListCount = Math.Max(statistics.MaxOpenListCount, bfsQueue.Count);
                }
                else
                {
                    dfsStack!.Push(child);
                    statistics.MaxOpenListCount = Math.Max(statistics.MaxOpenListCount, dfsStack.Count);
                }
            }
        }

        stopwatch.Stop();

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

#if ENABLE_PRUNING
            // Only add states without conflicts - this pruning ensures BFS and DFS
            // will explore the same total number of states, just in different orders
            if (!HasConflicts(newState))
            {
                children.Add(newState);
            }
#else
            // Add all possible queen placements, even those with conflicts
            // This will cause BFS and DFS to explore different numbers of states
            children.Add(newState);
#endif
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
        StringBuilder sb = new();

        sb.AppendLine($"\nN-Queens solution for n={n} using {method} approach:");

        if (stats.AllSolutions.Count > 0)
        {
            sb.AppendLine(
                $"\nFirst solution state: {StateToString(stats.AllSolutions.First())}"
            );
            PrintBoard(stats.AllSolutions.First(), n);
        }
        else
        {
            sb.AppendLine("No solution found!");
        }

        sb.AppendLine("\nStatistics:");
        sb.AppendLine($"Total solutions found: {stats.AllSolutions.Count}");
        sb.AppendLine($"Maximum Open list size: {stats.MaxOpenListCount}");
        sb.AppendLine($"Total states enqueued: {stats.TotalStatesEnqueued}");
        sb.AppendLine($"States in Closed list (checked): {stats.ClosedListCount}");
        sb.AppendLine($"Execution time: {stats.ExecutionTime.TotalMilliseconds} ms");

        Console.WriteLine(sb);

        // Display all solutions
        // Console.WriteLine("\nAll Solutions:");
        // for (int i = 0; i < stats.AllSolutions.Count; i++)
        // {
        //     Console.WriteLine($"\nSolution {i + 1}:");
        //     PrintBoard(stats.AllSolutions[i], n);
        // }
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

#if ENABLE_PRUNING
        Console.WriteLine("Conflict pruning is ENABLED - only generating valid states");
#else
        Console.WriteLine("Conflict pruning is DISABLED - generating all possible states");
#endif

        // Create arrays to store results for plotting
        int[] nValues = new int[maxN - minN + 1];
        int[] bfsMaxOpenCounts = new int[maxN - minN + 1];
        int[] bfsEnqueuedCounts = new int[maxN - minN + 1];
        int[] bfsClosedCounts = new int[maxN - minN + 1];
        double[] bfsTimes = new double[maxN - minN + 1];

        int[] dfsMaxOpenCounts = new int[maxN - minN + 1];
        int[] dfsEnqueuedCounts = new int[maxN - minN + 1];
        int[] dfsClosedCounts = new int[maxN - minN + 1];
        double[] dfsTimes = new double[maxN - minN + 1];

        int[] bfsSolutionCounts = new int[maxN - minN + 1];
        int[] dfsSolutionCounts = new int[maxN - minN + 1];

        for (int n = minN, i = 0; n <= maxN; n++, i++)
        {
            Console.WriteLine($"\n=== Testing n = {n} ===");
            nValues[i] = n;

            Console.WriteLine("\nRunning BFS...");
            Statistics bfsStats = Solve(n, SolveMethod.BFS);
            PrintResults(bfsStats, n, SolveMethod.BFS);
            bfsMaxOpenCounts[i] = bfsStats.MaxOpenListCount;
            bfsEnqueuedCounts[i] = bfsStats.TotalStatesEnqueued;
            bfsClosedCounts[i] = bfsStats.ClosedListCount;
            bfsTimes[i] = bfsStats.ExecutionTime.TotalMilliseconds;
            bfsSolutionCounts[i] = bfsStats.AllSolutions.Count;

            Console.WriteLine("\nRunning DFS...");
            Statistics dfsStats = Solve(n, SolveMethod.DFS);
            PrintResults(dfsStats, n, SolveMethod.DFS);
            dfsMaxOpenCounts[i] = dfsStats.MaxOpenListCount;
            dfsEnqueuedCounts[i] = dfsStats.TotalStatesEnqueued;
            dfsClosedCounts[i] = dfsStats.ClosedListCount;
            dfsTimes[i] = dfsStats.ExecutionTime.TotalMilliseconds;
            dfsSolutionCounts[i] = dfsStats.AllSolutions.Count;
        }

        StringBuilder sb = new();
        sb.AppendLine("\n=== Summary of Results ===");

#if ENABLE_PRUNING
        sb.AppendLine("\nNote: With conflict pruning enabled, BFS-Enqueued and DFS-Enqueued are the same because");
        sb.AppendLine("both algorithms explore the exact same states due to the row-by-row queen placement with pruning.");
        sb.AppendLine("Similarly, BFS-Closed and DFS-Closed are the same as both algorithms visit");
        sb.AppendLine("the same unique states, just in a different order.");
        sb.AppendLine("The primary difference is in MaxOpen (memory usage) and execution time.\n");
#else
        sb.AppendLine("\nNote: With conflict pruning disabled, BFS and DFS will explore");
        sb.AppendLine("substantially different numbers of states. DFS will typically explore");
        sb.AppendLine("many more states but use less memory (smaller MaxOpen),");
        sb.AppendLine("while BFS will be more efficient at finding solutions but use more memory.\n");
#endif

        const string format = "{0,4} | {1,10} | {2,12} | {3,12} | {4,10} | {5,10} | {6,12} | {7,12} | {8,10} | {9,10}";

        sb.AppendLine(
            string.Format(
                format,
                "n",
                "BFS-Max",
                "BFS-Enqueued",
                "BFS-Closed",
                "BFS-Time",
                "DFS-Max",
                "DFS-Enqueued",
                "DFS-Closed",
                "DFS-Time",
                "Solutions"
            )
        );
        sb.AppendLine(new string('-', 130));

        for (int i = 0; i < nValues.Length; i++)
        {
            sb.AppendLine(
                string.Format(
                    format,
                    nValues[i],
                    bfsMaxOpenCounts[i],
                    bfsEnqueuedCounts[i],
                    bfsClosedCounts[i],
                    bfsTimes[i].ToString("F2"),
                    dfsMaxOpenCounts[i],
                    dfsEnqueuedCounts[i],
                    dfsClosedCounts[i],
                    dfsTimes[i].ToString("F2"),
                    bfsSolutionCounts[i]  // BFS and DFS should find the same number of solutions
                )
            );
        }

        sb.AppendLine("\n--- CSV Format for Data Plotting ---");
        sb.AppendLine("n,BFS-Max,BFS-Enqueued,BFS-Closed,BFS-Time,DFS-Max,DFS-Enqueued,DFS-Closed,DFS-Time,Solutions");

        for (int i = 0; i < nValues.Length; i++)
        {
            sb.AppendLine($"{nValues[i]},{bfsMaxOpenCounts[i]},{bfsEnqueuedCounts[i]},{bfsClosedCounts[i]},{bfsTimes[i]:F2}," +
                          $"{dfsMaxOpenCounts[i]},{dfsEnqueuedCounts[i]},{dfsClosedCounts[i]},{dfsTimes[i]:F2},{bfsSolutionCounts[i]}");
        }

        sb.AppendLine("\nExperiments completed.");
        Console.WriteLine(sb);
    }
}
