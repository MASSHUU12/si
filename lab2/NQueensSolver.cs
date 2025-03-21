using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

class NQueensSolver
{
    private int n;
    private SolveMethod method;
    private PruningLevel pruningLevel;
    private SolutionMode solutionMode;
    private Statistics statistics;

    public enum SolveMethod
    {
        BFS,
        DFS
    }

    public enum PruningLevel
    {
        None,       // No pruning at all
        Minimal,    // Only check for obvious column conflicts
        Partial,    // Check all conflicts during generation but not during processing
        Full        // Check conflicts at both generation and processing
    }

    public enum SolutionMode
    {
        First,      // Find only the first solution
        All         // Find all possible solutions
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

    public NQueensSolver(
        int n,
        SolveMethod method = SolveMethod.BFS,
        PruningLevel pruningLevel = PruningLevel.Full,
        SolutionMode solutionMode = SolutionMode.All
    )
    {
        this.n = n;
        this.method = method;
        this.pruningLevel = pruningLevel;
        this.solutionMode = solutionMode;
        this.statistics = new();
    }

    public Statistics Solve()
    {
        List<(int, int)> initialState = new();
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Open list
        Queue<List<(int, int)>>? bfsQueue = null;
        Stack<List<(int, int)>>? dfsStack = null;

        if (this.method == SolveMethod.BFS)
        {
            bfsQueue = new();
            bfsQueue.Enqueue(initialState);
        }
        else
        {
            dfsStack = new();
            dfsStack.Push(initialState);
        }

        this.statistics.MaxOpenListCount = 1;
        this.statistics.TotalStatesEnqueued = 1;

        // Closed list to track visited states
        HashSet<string> closed = new();

        while ((method == SolveMethod.BFS && bfsQueue!.Count > 0) ||
               (method == SolveMethod.DFS && dfsStack!.Count > 0))
        {
            List<(int, int)> currentState;

            if (method == SolveMethod.BFS)
            {
                currentState = bfsQueue!.Dequeue();
                statistics.MaxOpenListCount = Math.Max(statistics.MaxOpenListCount, bfsQueue.Count);
            }
            else
            {
                currentState = dfsStack!.Pop();
                statistics.MaxOpenListCount = Math.Max(statistics.MaxOpenListCount, dfsStack.Count);
            }

            // Convert state to string for closed list tracking
            string stateKey = StateToString(currentState);

            // Skip if already processed
            if (closed.Contains(stateKey))
                continue;

            closed.Add(stateKey);

            // Check if this is a final state (n queens placed with no conflicts)
            if (currentState.Count == n)
            {
                if (!HasConflicts(currentState))
                {
                    this.statistics.AllSolutions.Add(new(currentState));

                    // Stop after finding the first solution if in First mode
                    if (this.solutionMode == SolutionMode.First)
                    {
                        break;
                    }
                }
                continue; // Skip generating children as we've reached a leaf node
            }

            if (pruningLevel == PruningLevel.Full && HasConflicts(currentState))
            {
                continue;
            }

            List<List<(int, int)>> children = GenerateChildren(currentState);
            foreach (var child in children)
            {
                this.statistics.TotalStatesEnqueued++;

                if (method == SolveMethod.BFS)
                {
                    bfsQueue!.Enqueue(child);
                }
                else
                {
                    dfsStack!.Push(child);
                }
            }
        }

        stopwatch.Stop();

        this.statistics.ClosedListCount = closed.Count;
        this.statistics.ExecutionTime = stopwatch.Elapsed;

        return this.statistics;
    }

    private List<List<(int, int)>> GenerateChildren(List<(int, int)> state)
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

            bool shouldAdd = true;

            // Apply different pruning strategies
            if (pruningLevel == PruningLevel.Full || pruningLevel == PruningLevel.Partial)
            {
                // Full conflict checking for full and partial pruning
                if (HasConflicts(newState))
                {
                    shouldAdd = false;
                }
            }
            else if (pruningLevel == PruningLevel.Minimal)
            {
                // Only check for obvious column conflicts (much faster than full checking)
                if (HasColumnConflict(newState))
                {
                    shouldAdd = false;
                }
            }

            // For None pruning level, add all states
            if (shouldAdd)
            {
                children.Add(newState);
            }
        }

        return children;
    }

    private bool HasColumnConflict(List<(int, int)> board)
    {
        int n = board.Count;
        var (newRow, newCol) = board[n - 1]; // Get the last added queen

        for (int i = 0; i < n - 1; i++)
        {
            var (_, col) = board[i];
            if (newCol == col)
                return true;
        }

        return false;
    }

    private bool HasConflicts(List<(int, int)> board)
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

    private string StateToString(List<(int, int)> state)
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

    public void PrintResults()
    {
        StringBuilder sb = new();

        string modeText = this.solutionMode == SolutionMode.First
            ? "finding first solution"
            : "finding all solutions";
        sb.AppendLine(
            $"\nN-Queens solution for n={this.n} using {this.method} with {this.pruningLevel} pruning, {modeText}:"
        );

        if (this.statistics.AllSolutions.Count > 0)
        {
            sb.AppendLine(
                $"\nFirst solution state: {StateToString(this.statistics.AllSolutions.First())}"
            );
            PrintBoard(this.statistics.AllSolutions.First());
        }
        else
        {
            sb.AppendLine("No solution found!");
        }

        sb.AppendLine("\nStatistics:");
        sb.AppendLine($"Total solutions found: {this.statistics.AllSolutions.Count}");
        sb.AppendLine($"Maximum Open list size: {this.statistics.MaxOpenListCount}");
        sb.AppendLine($"Total states enqueued: {this.statistics.TotalStatesEnqueued}");
        sb.AppendLine($"States in Closed list (checked): {this.statistics.ClosedListCount}");
        sb.AppendLine($"Execution time: {this.statistics.ExecutionTime.TotalMilliseconds} ms");

        Console.WriteLine(sb);
    }

    private void PrintBoard(List<(int, int)> solution)
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

    public static void RunExperiments(
        int minN = 4,
        int maxN = 12,
        PruningLevel pruningLevel = PruningLevel.Full,
        SolutionMode solutionMode = SolutionMode.All
    )
    {
        Console.WriteLine("Running N-Queens experiments...");
        string modeText = solutionMode == SolutionMode.First
            ? "finding first solution only"
            : "finding all solutions";
        Console.WriteLine($"Solution mode: {modeText}");

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
            NQueensSolver bfsSolver = new(n, SolveMethod.BFS, pruningLevel);
            Statistics bfsStats = bfsSolver.Solve();
            bfsSolver.PrintResults();
            bfsMaxOpenCounts[i] = bfsStats.MaxOpenListCount;
            bfsEnqueuedCounts[i] = bfsStats.TotalStatesEnqueued;
            bfsClosedCounts[i] = bfsStats.ClosedListCount;
            bfsTimes[i] = bfsStats.ExecutionTime.TotalMilliseconds;
            bfsSolutionCounts[i] = bfsStats.AllSolutions.Count;

            Console.WriteLine("\nRunning DFS...");
            NQueensSolver dfsSolver = new(n, SolveMethod.DFS, pruningLevel);
            Statistics dfsStats = dfsSolver.Solve();
            dfsSolver.PrintResults();
            dfsMaxOpenCounts[i] = dfsStats.MaxOpenListCount;
            dfsEnqueuedCounts[i] = dfsStats.TotalStatesEnqueued;
            dfsClosedCounts[i] = dfsStats.ClosedListCount;
            dfsTimes[i] = dfsStats.ExecutionTime.TotalMilliseconds;
            dfsSolutionCounts[i] = dfsStats.AllSolutions.Count;
        }

        StringBuilder sb = new();
        sb.AppendLine("\n=== Summary of Results ===");

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
                    solutionMode == SolutionMode.First ? "1" : bfsSolutionCounts[i].ToString()
                )
            );
        }

        sb.AppendLine("\n--- CSV Format for Data Plotting ---");
        sb.AppendLine("n,BFS-Max,BFS-Enqueued,BFS-Closed,BFS-Time,DFS-Max,DFS-Enqueued,DFS-Closed,DFS-Time,Solutions");

        for (int i = 0; i < nValues.Length; i++)
        {
            sb.AppendLine($"{nValues[i]},{bfsMaxOpenCounts[i]},{bfsEnqueuedCounts[i]},{bfsClosedCounts[i]},{bfsTimes[i]:F2}," +
                          $"{dfsMaxOpenCounts[i]},{dfsEnqueuedCounts[i]},{dfsClosedCounts[i]},{dfsTimes[i]:F2}," +
                          $"{(solutionMode == SolutionMode.First ? "1" : bfsSolutionCounts[i].ToString())}");
        }

        sb.AppendLine("\nExperiments completed.");
        Console.WriteLine(sb);
    }
}
