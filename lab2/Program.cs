using System;
using System.Diagnostics;

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: <size> <method>.");
    return 1;
}

if (!int.TryParse(args[0], out int n))
{
    Console.Error.WriteLine("Expected size of the board to be a number.");
    return 1;
}

if (n <= 0 || n >= 128)
{
    Console.Error.WriteLine("Size of the board should be in range [1, 127].");
    return 1;
}

if (!Enum.TryParse(args[1], true, out NQueens.SolveMethod m))
{
    Console.Error.WriteLine("Invalid method provided. Valid values are: BFS, DFS.");
    return 1;
}

Stopwatch watch = Stopwatch.StartNew();
NQueens.Solve(n, m);
watch.Stop();

double elapsedTimeInSeconds = watch.ElapsedMilliseconds / 1000.0;
Console.WriteLine("Process took {0:0.0000}s.", elapsedTimeInSeconds);

return 0;
