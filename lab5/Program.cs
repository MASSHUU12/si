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
        public int popSize { get; set; } = 100;
        public int tournamentSize { get; set; } = 5;
        public int genMax { get; set; } = 1000;
    }

    static int Main(string[] args)
    {
        Console.WriteLine("N-Queens Solver - Evolutionary Algorithm");
        Console.WriteLine("========================================\n");

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

        for (int i = 2; i < args.Length; i++)
        {
            string option = args[i].ToLower();
            if (option == "-p" || option == "--popsize")
            {
                if (++i >= args.Length)
                    throw new ArgumentException("Missing value for option " + option);
                config.popSize = int.Parse(args[i]);
            }
            else if (option == "-t" || option == "--tournamentsize")
            {
                if (++i >= args.Length)
                    throw new ArgumentException("Missing value for option " + option);
                config.tournamentSize = int.Parse(args[i]);
            }
            else if (option == "-g" || option == "--genmax")
            {
                if (++i >= args.Length)
                    throw new ArgumentException("Missing value for option " + option);
                config.genMax = int.Parse(args[i]);
            }
            else
            {
                throw new ArgumentException($"Unknown option '{args[i]}'");
            }
        }
    }

    private static void ParseExperimentsArguments(string[] args, Configuration config)
    {
        int argIndex = 1;
        if (argIndex < args.Length && !args[argIndex].StartsWith("-"))
        {
            config.MinN = int.Parse(args[argIndex++]);
            if (argIndex < args.Length && !args[argIndex].StartsWith("-"))
            {
                config.MaxN = int.Parse(args[argIndex++]);
            }
        }

        while (argIndex < args.Length)
        {
            string option = args[argIndex].ToLower();
            if (option == "-p" || option == "--popsize")
            {
                if (++argIndex >= args.Length)
                    throw new ArgumentException("Missing value for option " + option);
                config.popSize = int.Parse(args[argIndex]);
            }
            else if (option == "-t" || option == "--tournamentsize")
            {
                if (++argIndex >= args.Length)
                    throw new ArgumentException("Missing value for option " + option);
                config.tournamentSize = int.Parse(args[argIndex]);
            }
            else if (option == "-g" || option == "--genmax")
            {
                if (++argIndex >= args.Length)
                    throw new ArgumentException("Missing value for option " + option);
                config.genMax = int.Parse(args[argIndex]);
            }
            else
            {
                throw new ArgumentException($"Unknown option '{args[argIndex]}'");
            }
            argIndex++;
        }
    }

    private static void ExecuteSolveCommand(Configuration config)
    {
        NQueensSolver solver = new(
            config.N,
            config.popSize,
            config.tournamentSize,
            config.genMax
        );
        var stats = solver.Solve();
        solver.PrintResults();
    }

    private static void ExecuteExperimentsCommand(Configuration config)
    {
        NQueensSolver.RunExperiments(
            config.MinN,
            config.MaxN,
            config.popSize,
            config.tournamentSize,
            config.genMax
        );
    }

    private static void PrintUsage()
    {
        StringBuilder sb = new();
        sb.AppendLine("Usage:");
        sb.AppendLine("  -s, --solve <n> [options]");
        sb.AppendLine("      Solve n-Queens puzzle for a specific n value.");
        sb.AppendLine("      Options:");
        sb.AppendLine("         -p, --popSize <value>         Population size (default: 100)");
        sb.AppendLine("         -t, --tournamentSize <value>  Tournament pool size (default: 5)");
        sb.AppendLine("         -g, --genMax <value>          Maximum generations (default: 1000)");
        sb.AppendLine("      Example: --solve 8 -p 200 -t 10 -g 1500");
        sb.AppendLine();
        sb.AppendLine("  -e, --experiments [n] [times] [options]");
        sb.AppendLine("      Run experiments for n-Queens for n times (defaults: n = 4, times = 8).");
        sb.AppendLine("      Options:");
        sb.AppendLine("         -p, --popSize <value>         Population size (default: 100)");
        sb.AppendLine("         -t, --tournamentSize <value>  Tournament pool size (default: 5)");
        sb.AppendLine("         -g, --genMax <value>          Maximum generations (default: 1000)");
        sb.AppendLine("      Example: --experiments 4 12 -p 200 -t 10 -g 1500");
        Console.WriteLine(sb.ToString());
    }
}
