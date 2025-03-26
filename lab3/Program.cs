using System;
using System.Text;

class Program
{
    private enum Command
    {
        Solve,
        Experiments,
        Unknown
    }

    private class Configuration
    {
        public Command Command { get; set; } = Command.Unknown;
        public int N { get; set; } = 0;
        public int MinN { get; set; } = 4;
        public int MaxN { get; set; } = 8;
        public NQueensSolver.SolveMethod Method { get; set; } = NQueensSolver.SolveMethod.BFS;
        public NQueensSolver.PruningLevel PruningLevel { get; set; } = NQueensSolver.PruningLevel.Full;
        public NQueensSolver.SolutionMode SolutionMode { get; set; } = NQueensSolver.SolutionMode.All;
        public bool RunAllMethods { get; set; } = true;
        public bool Debug { get; set; } = false;
    }

    static int Main(string[] args)
    {
        Console.WriteLine("N-Queens Solver - BFS vs DFS vs BestFirst Comparison");
        Console.WriteLine("====================================================\n");

        if (args.Length == 0)
        {
            PrintUsage();
            return 0;
        }

        try
        {
            Configuration config = ParseArguments(args);

            switch (config.Command)
            {
                case Command.Solve:
                    ExecuteSolveCommand(config);
                    break;

                case Command.Experiments:
                    ExecuteExperimentsCommand(config);
                    break;

                default:
                    Console.WriteLine($"Unknown command: {args[0]}");
                    PrintUsage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            PrintUsage();
        }

        return 0;
    }

    private static Configuration ParseArguments(string[] args)
    {
        Configuration config = new();

        if (args[0] == "-s" || args[0] == "--solve")
        {
            config.Command = Command.Solve;
            ParseSolveArguments(args, config);
        }
        else if (args[0] == "-e" || args[0] == "--experiments")
        {
            config.Command = Command.Experiments;
            ParseExperimentsArguments(args, config);
        }

        return config;
    }

    private static void ParseSolveArguments(string[] args, Configuration config)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("Missing n value for solve mode");
        }

        config.N = int.Parse(args[1]);

        // Parse optional parameters
        for (int i = 2; i < args.Length; i += 2)
        {
            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing value for option {args[i]}");
            }

            switch (args[i])
            {
                case "-m":
                case "--method":
                    config.RunAllMethods = false;
                    config.Method = ParseMethod(args[i + 1]);
                    break;

                case "-l":
                case "--level":
                    config.PruningLevel = ParsePruningLevel(args[i + 1]);
                    break;

                case "-o":
                case "--solution-mode":
                    config.SolutionMode = ParseSolutionMode(args[i + 1]);
                    break;

                case "--debug":
                    config.Debug = true;
                    break;

                default:
                    throw new ArgumentException($"Unknown option '{args[i]}'");
            }
        }
    }

    private static void ParseExperimentsArguments(string[] args, Configuration config)
    {
        int argIndex = 1;

        // Parse optional min and max N values
        if (argIndex < args.Length && !args[argIndex].StartsWith("-"))
        {
            config.MinN = int.Parse(args[argIndex++]);

            if (argIndex < args.Length && !args[argIndex].StartsWith("-"))
            {
                config.MaxN = int.Parse(args[argIndex++]);
            }
        }

        // Parse other options
        while (argIndex < args.Length)
        {
            switch (args[argIndex])
            {
                case "-l":
                case "--level":
                    if (argIndex + 1 >= args.Length)
                        throw new ArgumentException($"Missing value for option {args[argIndex]}");
                    config.PruningLevel = ParsePruningLevel(args[++argIndex]);
                    break;

                case "-o":
                case "--solution-mode":
                    if (argIndex + 1 >= args.Length)
                        throw new ArgumentException($"Missing value for option {args[argIndex]}");
                    config.SolutionMode = ParseSolutionMode(args[++argIndex]);
                    break;

                case "--debug":
                    config.Debug = true;
                    break;

                default:
                    throw new ArgumentException($"Unknown option '{args[argIndex]}'");
            }

            argIndex++;
        }
    }

    private static NQueensSolver.SolveMethod ParseMethod(string methodName)
    {
        return methodName.ToLower() switch
        {
            "bfs" => NQueensSolver.SolveMethod.BFS,
            "dfs" => NQueensSolver.SolveMethod.DFS,
            "bf" => NQueensSolver.SolveMethod.BestFirst,
            _ => throw new ArgumentException($"Invalid method '{methodName}'. Use 'bfs', 'dfs' or 'bf'")
        };
    }

    private static NQueensSolver.PruningLevel ParsePruningLevel(string levelName)
    {
        return levelName.ToLower() switch
        {
            "none" => NQueensSolver.PruningLevel.None,
            "minimal" => NQueensSolver.PruningLevel.Minimal,
            "partial" => NQueensSolver.PruningLevel.Partial,
            "full" => NQueensSolver.PruningLevel.Full,
            _ => throw new ArgumentException($"Invalid pruning level '{levelName}'. Use 'none', 'minimal', 'partial', or 'full'")
        };
    }

    private static NQueensSolver.SolutionMode ParseSolutionMode(string modeName)
    {
        return modeName.ToLower() switch
        {
            "first" => NQueensSolver.SolutionMode.First,
            "all" => NQueensSolver.SolutionMode.All,
            _ => throw new ArgumentException($"Invalid solution mode '{modeName}'. Use 'first' or 'all'")
        };
    }

    private static void ExecuteSolveCommand(Configuration config)
    {
        string solutionModeText = config.SolutionMode == NQueensSolver.SolutionMode.First
            ? "finding first solution only"
            : "finding all solutions";

        Console.WriteLine($"Solving {config.N}-Queens Puzzle with {config.PruningLevel} pruning, {solutionModeText}:");

        if (config.RunAllMethods || config.Method == NQueensSolver.SolveMethod.BFS)
        {
            NQueensSolver bfsSolver = new(
                config.N,
                NQueensSolver.SolveMethod.BFS,
                config.PruningLevel,
                config.SolutionMode,
                debug: config.Debug
            );
            var bfsStats = bfsSolver.Solve();
            bfsSolver.PrintResults();
        }

        if (config.RunAllMethods || config.Method == NQueensSolver.SolveMethod.DFS)
        {
            NQueensSolver dfsSolver = new(
                config.N,
                NQueensSolver.SolveMethod.DFS,
                config.PruningLevel,
                config.SolutionMode,
                debug: config.Debug
            );
            var dfsStats = dfsSolver.Solve();
            dfsSolver.PrintResults();
        }

        if (config.RunAllMethods || config.Method == NQueensSolver.SolveMethod.BestFirst)
        {
            NQueensSolver bfSolver = new(
                config.N,
                NQueensSolver.SolveMethod.BestFirst,
                config.PruningLevel,
                config.SolutionMode,
                debug: config.Debug
            );
            var dfsStats = bfSolver.Solve();
            bfSolver.PrintResults();
        }
    }

    private static void ExecuteExperimentsCommand(Configuration config)
    {
        string solutionModeText = config.SolutionMode == NQueensSolver.SolutionMode.First
            ? "finding first solution only"
            : "finding all solutions";

        Console.WriteLine(
            $"Running experiments from n={config.MinN} to n={config.MaxN} with {config.PruningLevel} pruning, {solutionModeText}"
        );
        NQueensSolver.RunExperiments(config.MinN, config.MaxN, config.PruningLevel, config.SolutionMode);
    }

    private static void PrintUsage()
    {
        StringBuilder sb = new();
        sb.AppendLine("Usage:");
        sb.AppendLine("  -s, --solve <n> [-m, --method <bfs|dfs|bf>] [-l, --level <none|minimal|partial|full>] [-o, --solution-mode <first|all>] [--debug]");
        sb.AppendLine("      Solve n-Queens puzzle for specific n value.");
        sb.AppendLine("      Options:");
        sb.AppendLine("        -m, --method: Specify search method (bfs, dfs, or both if omitted)");
        sb.AppendLine("        -l, --level: Specify pruning level:");
        sb.AppendLine("          none: No pruning");
        sb.AppendLine("          minimal: Only check for column conflicts");
        sb.AppendLine("          partial: Check conflicts during generation only");
        sb.AppendLine("          full: Check conflicts during both generation and processing (default)");
        sb.AppendLine("        -o, --solution-mode: Specify solution finding mode:");
        sb.AppendLine("          first: Find only the first solution (faster)");
        sb.AppendLine("          all: Find all possible solutions (default)");
        sb.AppendLine("        --debug: Enable debug mode to show intermediate results");
        sb.AppendLine("      Example: --solve 8 --method bfs --level minimal --solution-mode first --debug");
        sb.AppendLine();
        sb.AppendLine("  -e, --experiments [min_n] [max_n] [-l, --level <none|minimal|partial|full>] [-o, --solution-mode <first|all>] [--debug]");
        sb.AppendLine("      Run experiments from min_n to max_n (defaults: min_n=4, max_n=8).");
        sb.AppendLine("      Example: --experiments 4 12 --level partial --solution-mode first --debug");

        Console.WriteLine(sb);
    }
}
