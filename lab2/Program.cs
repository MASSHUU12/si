using System;
using System.Text;

class Program
{
    static int Main(string[] args)
    {
        Console.WriteLine("N-Queens Solver - BFS vs DFS Comparison");
        Console.WriteLine("=======================================" + Environment.NewLine);

        if (args.Length == 0)
        {
            PrintUsage();
            return 0;
        }

        try
        {
            if (args[0] == "-s" || args[0] == "--solve")
            {
                // Single solve mode
                if (args.Length < 2)
                {
                    Console.WriteLine("Error: Missing n value for solve mode");
                    PrintUsage();
                    return 1;
                }

                int n = int.Parse(args[1]);

                // Check if method is specified
                NQueensSolver.SolveMethod method = NQueensSolver.SolveMethod.BFS; // Default
                bool runBoth = true;

                if (args.Length >= 4 && (args[2] == "-m" || args[2] == "--method"))
                {
                    string methodName = args[3].ToLower();
                    runBoth = false;

                    if (methodName == "bfs")
                        method = NQueensSolver.SolveMethod.BFS;
                    else if (methodName == "dfs")
                        method = NQueensSolver.SolveMethod.DFS;
                    else
                    {
                        Console.WriteLine($"Error: Invalid method '{methodName}'. Use 'bfs' or 'dfs'");
                        return 1;
                    }
                }

                Console.WriteLine($"Solving {n}-Queens Puzzle:");

                if (runBoth || method == NQueensSolver.SolveMethod.BFS)
                {
                    var bfsStats = NQueensSolver.Solve(n, NQueensSolver.SolveMethod.BFS);
                    NQueensSolver.PrintResults(bfsStats, n, NQueensSolver.SolveMethod.BFS);
                }

                if (runBoth || method == NQueensSolver.SolveMethod.DFS)
                {
                    var dfsStats = NQueensSolver.Solve(n, NQueensSolver.SolveMethod.DFS);
                    NQueensSolver.PrintResults(dfsStats, n, NQueensSolver.SolveMethod.DFS);
                }
            }
            else if (args[0] == "-e" || args[0] == "--experiments")
            {
                // Experiments mode
                int minN = 4; // Default
                int maxN = 8; // Default

                if (args.Length >= 2)
                    minN = int.Parse(args[1]);

                if (args.Length >= 3)
                    maxN = int.Parse(args[2]);

                Console.WriteLine($"Running experiments from n={minN} to n={maxN}");
                NQueensSolver.RunExperiments(minN, maxN);
            }
            else
            {
                Console.WriteLine($"Unknown command: {args[0]}");
                PrintUsage();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            PrintUsage();
        }

        return 0;
    }

    static void PrintUsage()
    {
        StringBuilder sb = new();
        sb.AppendLine("Usage:");
        sb.AppendLine("  -s, --solve <n> [-m, --method <bfs|dfs>]");
        sb.AppendLine("      Solve n-Queens puzzle for specific n value using BFS, DFS, or both.");
        sb.AppendLine("      Example: --solve 8 --method bfs");
        sb.AppendLine();
        sb.AppendLine("  -e, --experiments [min_n] [max_n]");
        sb.AppendLine("      Run experiments from min_n to max_n (defaults: min_n=4, max_n=8).");
        sb.AppendLine("      Example: --experiments 4 12");

        Console.WriteLine(sb);
    }
}
