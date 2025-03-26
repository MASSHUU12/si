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
    private Heuristic heuristic;

    public enum SolveMethod
    {
        BFS,
        DFS,
        BestFirst
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

    public enum Heuristic
    {
        H1,
        H2
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
        SolutionMode solutionMode = SolutionMode.All,
        Heuristic heuristic = Heuristic.H1
    )
    {
        this.n = n;
        this.method = method;
        this.pruningLevel = pruningLevel;
        this.solutionMode = solutionMode;
        this.heuristic = heuristic;
        this.statistics = new();
    }

    public Statistics Solve()
    {
        List<(int, int)> initialState = new();
        Stopwatch stopwatch = Stopwatch.StartNew();

        // Open list
        Queue<List<(int, int)>>? bfsQueue = null;
        Stack<List<(int, int)>>? dfsStack = null;
        PriorityQueue<List<(int, int)>, int>? bestFirstQueue = null;

        switch (this.method)
        {
            case SolveMethod.BFS:
                bfsQueue = new();
                bfsQueue.Enqueue(initialState);
                break;
            case SolveMethod.DFS:
                dfsStack = new();
                dfsStack.Push(initialState);
                break;
            case SolveMethod.BestFirst:
                bestFirstQueue = new();
                bestFirstQueue.Enqueue(initialState, EvaluateState(initialState));
                break;
            default:
                throw new("UNREACHABLE");
        }

        this.statistics.MaxOpenListCount = 1;
        this.statistics.TotalStatesEnqueued = 1;

        // Closed list to track visited states
        HashSet<string> closed = new();

        while ((method == SolveMethod.BFS && bfsQueue!.Count > 0) ||
               (method == SolveMethod.DFS && dfsStack!.Count > 0) ||
               (method == SolveMethod.BestFirst && bestFirstQueue!.Count > 0))
        {
            List<(int, int)> currentState;

            switch (this.method)
            {
                case SolveMethod.BFS:
                    currentState = bfsQueue!.Dequeue();
                    statistics.MaxOpenListCount = Math.Max(statistics.MaxOpenListCount, bfsQueue.Count);
                    break;
                case SolveMethod.DFS:
                    currentState = dfsStack!.Pop();
                    statistics.MaxOpenListCount = Math.Max(statistics.MaxOpenListCount, dfsStack.Count);
                    break;
                case SolveMethod.BestFirst:
                    currentState = bestFirstQueue!.Dequeue();
                    statistics.MaxOpenListCount = Math.Max(statistics.MaxOpenListCount, bestFirstQueue.Count);
                    break;
                default:
                    throw new("UNREACHABLE");
            }

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

                switch (this.method)
                {
                    case SolveMethod.BFS:
                        bfsQueue!.Enqueue(child);
                        break;
                    case SolveMethod.DFS:
                        dfsStack!.Push(child);
                        break;
                    case SolveMethod.BestFirst:
                        bestFirstQueue!.Enqueue(child, EvaluateState(child));
                        break;
                    default:
                        throw new("UNREACHABLE");
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

    private int EvaluateState(List<(int, int)> state)
    {
        return heuristic switch
        {
            Heuristic.H1 => EvaluateH1(state),
            Heuristic.H2 => EvaluateH2(state),
            _ => throw new("UNREACHABLE")
        };
    }

    private int EvaluateH1(List<(int, int)> state)
    {
        int l = state.Count;
        int sumWrow = 0;

        for (int i = 0; i < l; i++)
        {
            int row = state[i].Item1 + 1; // Convert zero-indexed to one-indexed
            int wrow = (row <= n / 2) ? (n - row + 1) : row;
            sumWrow += wrow;
        }

        return (n - l) * sumWrow;
    }

    private int EvaluateH2(List<(int, int)> state)
    {
        int l = state.Count;
        int attackCount = CountAttacks(state);

        return attackCount + (n - l);
    }

    private int CountAttacks(List<(int, int)> state)
    {
        int attackCount = 0;

        for (int i = 0; i < state.Count; i++)
        {
            var (row1, col1) = state[i];

            for (int j = i + 1; j < state.Count; j++)
            {
                var (row2, col2) = state[j];

                if (col1 == col2 ||               // Same column
                    row1 == row2 ||               // Same row
                    row1 + col1 == row2 + col2 || // Same rising diagonal
                    row1 - col1 == row2 - col2)   // Same falling diagonal
                {
                    attackCount++;
                }
            }
        }

        return attackCount;
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
        return CountAttacks(board) > 0;
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

    private static Statistics RunSingleExperiment(
        int n,
        SolveMethod method,
        PruningLevel pruningLevel,
        SolutionMode solutionMode,
        Heuristic heuristic
    )
    {
        NQueensSolver solver = new(n, method, pruningLevel, solutionMode, heuristic);
        Statistics stats = solver.Solve();
        solver.PrintResults();
        return stats;
    }

    private static void PrintSummaryHeader(StringBuilder sb)
    {
        const string format = "{0,4} | {1,8} | {2,10} | {3,10} | {4,8} | {5,8} | {6,10} | {7,10} | {8,8} | {9,8} | {10,10} | {11,10} | {12,8} | {13,8} | {14,10} | {15,10} | {16,8}";
        sb.AppendLine(
            string.Format(
                format,
                "n",
                "BFS-Max",
                "BFS-Enq",
                "BFS-Cls",
                "BFS-T",
                "DFS-Max",
                "DFS-Enq",
                "DFS-Cls",
                "DFS-T",
                "BH1-Max",
                "BH1-Enq",
                "BH1-Cls",
                "BH1-T",
                "BH2-Max",
                "BH2-Enq",
                "BH2-Cls",
                "BH2-T",
                "Sol"
            )
        );
        sb.AppendLine(new string('-', 196));
    }

    private static void PrintSummaryRow(
        StringBuilder sb,
        int n,
        Statistics bfsStats,
        Statistics dfsStats,
        Statistics bestFirstStatsH1,
        Statistics bestFirstStatsH2,
        SolutionMode solutionMode
    )
    {
        const string format = "{0,4} | {1,8} | {2,10} | {3,10} | {4,8} | {5,8} | {6,10} | {7,10} | {8,8} | {9,8} | {10,10} | {11,10} | {12,8} | {13,8} | {14,10} | {15,10} | {16,8}";
        sb.AppendLine(
            string.Format(
                format,
                n,
                bfsStats.MaxOpenListCount,
                bfsStats.TotalStatesEnqueued,
                bfsStats.ClosedListCount,
                bfsStats.ExecutionTime.TotalMilliseconds.ToString("F2"),
                dfsStats.MaxOpenListCount,
                dfsStats.TotalStatesEnqueued,
                dfsStats.ClosedListCount,
                dfsStats.ExecutionTime.TotalMilliseconds.ToString("F2"),
                bestFirstStatsH1.MaxOpenListCount,
                bestFirstStatsH1.TotalStatesEnqueued,
                bestFirstStatsH1.ClosedListCount,
                bestFirstStatsH1.ExecutionTime.TotalMilliseconds.ToString("F2"),
                bestFirstStatsH2.MaxOpenListCount,
                bestFirstStatsH2.TotalStatesEnqueued,
                bestFirstStatsH2.ClosedListCount,
                bestFirstStatsH2.ExecutionTime.TotalMilliseconds.ToString("F2"),
                solutionMode == SolutionMode.First
                    ? "1"
                    : bfsStats.AllSolutions.Count.ToString()
            )
        );
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

        StringBuilder summary = new();
        PrintSummaryHeader(summary);

        List<int> nValues = [];
        List<Statistics> bfsStatsList = [],
            dfsStatsList = [],
            bestFirstStatsH1List = [],
            bestFirstStatsH2List = [];

        for (int n = minN; n <= maxN; n++)
        {
            Console.WriteLine($"\n=== Testing n = {n} ===");

            Statistics bfsStats = RunSingleExperiment(
                n,
                SolveMethod.BFS,
                pruningLevel,
                solutionMode
            );
            Statistics dfsStats = RunSingleExperiment(
                n,
                SolveMethod.DFS,
                pruningLevel,
                solutionMode
            );
            Statistics bestFirstStatsH1 = RunSingleExperiment(
                n,
                SolveMethod.BestFirst,
                pruningLevel,
                solutionMode,
                Heuristic.H1
            );
            Statistics bestFirstStatsH2 = RunSingleExperiment(
                n,
                SolveMethod.BestFirst,
                pruningLevel,
                solutionMode,
                Heuristic.H2
            );

            nValues.Add(n);
            bfsStatsList.Add(bfsStats);
            dfsStatsList.Add(dfsStats);
            bestFirstStatsH1List.Add(bestFirstStatsH1);
            bestFirstStatsH2List.Add(bestFirstStatsH2);

            PrintSummaryRow(summary, n, bfsStats, dfsStats, bestFirstStatsH1, bestFirstStatsH2, solutionMode);
        }

        summary.AppendLine("\n--- CSV Format ---");
        summary.AppendLine("n,BFS-Max,BFS-Enq,BFS-Cls,BFS-T,DFS-Max,DFS-Enq,DFS-Cls,DFS-T,BH1-Max,BH1-Enq,BH1-Cls,BH1-T,BH2-Max,BH2-Enq,BH2-Cls,BH2-T,Sol");

        for (int i = 0; i < nValues.Count; i++)
        {
            summary.AppendLine($"{nValues[i]},{bfsStatsList[i].MaxOpenListCount},{bfsStatsList[i].TotalStatesEnqueued},{bfsStatsList[i].ClosedListCount},{bfsStatsList[i].ExecutionTime.TotalMilliseconds:F2}," +
                $"{dfsStatsList[i].MaxOpenListCount},{dfsStatsList[i].TotalStatesEnqueued},{dfsStatsList[i].ClosedListCount},{dfsStatsList[i].ExecutionTime.TotalMilliseconds:F2}," +
                $"{bestFirstStatsH1List[i].MaxOpenListCount},{bestFirstStatsH1List[i].TotalStatesEnqueued},{bestFirstStatsH1List[i].ClosedListCount},{bestFirstStatsH1List[i].ExecutionTime.TotalMilliseconds:F2}," +
                $"{bestFirstStatsH2List[i].MaxOpenListCount},{bestFirstStatsH2List[i].TotalStatesEnqueued},{bestFirstStatsH2List[i].ClosedListCount},{bestFirstStatsH2List[i].ExecutionTime.TotalMilliseconds:F2}," +
                $"{(solutionMode == SolutionMode.First ? "1" : bfsStatsList[i].AllSolutions.Count.ToString())}");
        }

        summary.AppendLine("\nExperiments completed.");
        Console.Write(summary);
    }
}
