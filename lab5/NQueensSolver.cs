using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

class NQueensSolver
{
    private int n;
    private int popSize;           // Population size
    private int tournamentSize;    // Tournament pool size
    private int genMax;            // Maximum generations
    private const double pc = 0.7; // Crossover probability
    private const double pm = 0.2; // Mutation probability

    private Random random = new();
    private Statistics statistics;

    public class Statistics
    {
        public int BestGeneration { get; set; }
        public int BestFitness { get; set; }
        public List<(int, int)> BestBoard { get; set; } = [];
        public TimeSpan ExecutionTime { get; set; }
    }

    public NQueensSolver(
        int n,
        int popSize,
        int tournamentSize,
        int genMax
    )
    {
        this.n = n;
        this.popSize = popSize;
        this.tournamentSize = tournamentSize;
        this.genMax = genMax;
        this.statistics = new();
    }

    public Statistics Solve()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        List<List<(int, int)>> population = [];
        for (int i = 0; i < popSize; i++)
        {
            population.Add(GenerateRandomIndividual());
        }

        List<int> fitness = EvaluatePopulation(population);
        int bestIndex = GetBestIndex(fitness);

        int generation = 0;
        // ffmax is 0 since a perfect solution has 0 attacks.
        while (generation < genMax && fitness[bestIndex] > 0)
        {
            // Tournament selection creates a new population P'
            List<List<(int, int)>> newPopulation = [];
            for (int i = 0; i < popSize; i++)
            {
                newPopulation.Add(TournamentSelection(population, fitness));
            }

            // One-point crossover on pairs with probability pc
            for (int i = 0; i < popSize - 1; i += 2)
            {
                if (random.NextDouble() > pc)
                {
                    continue;
                }

                (List<(int, int)> child1, List<(int, int)> child2) = Crossover(newPopulation[i], newPopulation[i + 1]);
                newPopulation[i] = child1;
                newPopulation[i + 1] = child2;
            }

            // For each individual, with probability pm, mutate one gene
            for (int i = 0; i < popSize; i++)
            {
                if (random.NextDouble() > pm)
                {
                    continue;
                }

                Mutate(newPopulation[i]);
            }

            population = newPopulation;
            fitness = EvaluatePopulation(population);
            bestIndex = GetBestIndex(fitness);
            generation++;
        }

        stopwatch.Stop();
        this.statistics.ExecutionTime = stopwatch.Elapsed;
        this.statistics.BestGeneration = generation;
        this.statistics.BestFitness = fitness[bestIndex];
        this.statistics.BestBoard = population[bestIndex];

        return this.statistics;
    }

    private List<(int, int)> GenerateRandomIndividual()
    {
        List<(int, int)> individual = [];

        for (int i = 0; i < n; i++)
        {
            int row = this.random.Next(n);
            int col = this.random.Next(n);
            individual.Add((row, col));
        }

        return individual;
    }

    private List<int> EvaluatePopulation(List<List<(int, int)>> population)
    {
        List<int> fitness = [];

        foreach (var individual in population)
        {
            fitness.Add(CountAttacks(individual));
        }

        return fitness;
    }

    private int GetBestIndex(List<int> fitness)
    {
        int bestIndex = 0;
        int bestFitness = fitness[0];

        for (int i = 1; i < fitness.Count; i++)
        {
            if (fitness[i] < bestFitness)
            {
                bestFitness = fitness[i];
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    private List<(int, int)> TournamentSelection(
        List<List<(int, int)>> population,
        List<int> fitness
    )
    {
        int bestIndex = -1;
        int bestFitness = int.MaxValue;

        for (int i = 0; i < tournamentSize; i++)
        {
            int idx = random.Next(population.Count);
            if (fitness[idx] < bestFitness)
            {
                bestFitness = fitness[idx];
                bestIndex = idx;
            }
        }

        // Return a copy of the selected individual
        return new List<(int, int)>(population[bestIndex]);
    }

    private (List<(int, int)>, List<(int, int)>) Crossover(
        List<(int, int)> parent1,
        List<(int, int)> parent2
    )
    {
        int point = random.Next(1, n);
        List<(int, int)> child1 = [];
        List<(int, int)> child2 = [];

        child1.AddRange(parent1.GetRange(0, point));
        child1.AddRange(parent2.GetRange(point, n - point));

        child2.AddRange(parent2.GetRange(0, point));
        child2.AddRange(parent1.GetRange(point, n - point));

        return (child1, child2);
    }

    private void Mutate(List<(int, int)> individual)
    {
        int index = random.Next(n);
        int newValue = random.Next(n); // new value in [0, n-1]

        var queen = individual[index];
        // Decide whether to mutate the row or the column
        if (random.NextDouble() < 0.5)
            individual[index] = (newValue, queen.Item2);
        else
            individual[index] = (queen.Item1, newValue);
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

    public void PrintResults(bool drawBoard = true)
    {
        StringBuilder sb = new();

        sb.Append($"N-Queens solution for n={this.n}, popSize={this.popSize}, ");
        sb.AppendLine($"tournamentSize={this.tournamentSize}, genMax={this.genMax}:");
        if (drawBoard)
        {
            sb.AppendLine(BoardToString(this.statistics.BestBoard));
        }
        sb.AppendLine("\nStatistics:");
        sb.AppendLine($"Execution time: {this.statistics.ExecutionTime.TotalMilliseconds} ms");
        sb.AppendLine($"Best solution found in generation {this.statistics.BestGeneration}");
        sb.AppendLine($"Number of attacks (fitness): {this.statistics.BestFitness}");

        Console.WriteLine(sb);
    }

    private string BoardToString(List<(int, int)> solution)
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

        return sb.ToString();
    }

    private static Statistics RunSingleExperiment(
        int n,
        int popSize,
        int tournamentSize,
        int genMax
    )
    {
        NQueensSolver solver = new(n, popSize, tournamentSize, genMax);
        Statistics stats = solver.Solve();
        solver.PrintResults(false);
        return stats;
    }

    private static void PrintSummaryHeader(StringBuilder sb)
    {
        const string format = "{0,4} | {1,10} | {2, 4} | {3, 4} | {4, 8} | {5, 8} | {6, 4}";
        sb.AppendLine(string.Format(format, "N", "Time", "PS", "TS", "GM", "BG", "BF"));
        sb.AppendLine(new string('-', 64));
    }

    private static void PrintSummaryRow(
        StringBuilder sb,
        int n,
        int popSize,
        int tournamentSize,
        int genMax,
        Statistics stats
    )
    {
        const string format = "{0,4} | {1,10} | {2, 4} | {3, 4} | {4, 8} | {5, 8} | {6, 4}";
        sb.AppendLine(
            string.Format(
                format,
                n,
                stats.ExecutionTime.TotalMilliseconds,
                popSize,
                tournamentSize,
                genMax,
                stats.BestGeneration,
                stats.BestFitness
            )
        );
    }

    public static void RunExperiments(
        int n,
        int times,
        int popSize,
        int tournamentSize,
        int genMax
    )
    {
        StringBuilder sb = new();
        sb.Append(
            $"=== Running experiments for n-Queens (Evolutionary Algorithm) {times} times for n={n}, "
        );
        sb.AppendLine($"tournamentSize={tournamentSize}, genMax={genMax} ===\n");

        PrintSummaryHeader(sb);

        List<Statistics> stats = [];

        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"=== Test {i} ===");

            Statistics r = RunSingleExperiment(n, popSize, tournamentSize, genMax);

            stats.Add(r);

            PrintSummaryRow(sb, i, popSize, tournamentSize, genMax, r);
        }

        sb.AppendLine("\n--- CSV Format ---");
        sb.AppendLine("N,Time");

        for (int i = 0; i < n; i++)
        {
            sb.AppendFormat(
                "{0},{1},{2},{3},{4},{5},{6}",
                i,
                stats[i].ExecutionTime.TotalMilliseconds,
                popSize,
                tournamentSize,
                genMax,
                stats[i].BestGeneration,
                stats[i].BestFitness
            );
            sb.AppendLine($"{i},{stats[i].ExecutionTime.TotalMilliseconds}");
        }

        sb.AppendLine("\nExperiments completed.");
        Console.Write(sb);
    }
}
