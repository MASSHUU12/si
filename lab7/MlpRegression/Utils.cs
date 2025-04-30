using System.IO;
using System.Linq;
using System.Globalization;

namespace MlpRegression;

public static class Utils
{
    // Loads a CSV of columns x1,x2,y (with header) into X[M][2] and y[M]
    public static (double[][] X, double[] y) LoadCsv(string path)
    {
        string[]? lines = File.ReadAllLines(path)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .Skip(1)  // skip header
                        .ToArray();

        int M = lines.Length;
        var X = new double[M][];
        var y = new double[M];

        for (int i = 0; i < M; i++)
        {
            var parts = lines[i].Split(',');
            X[i] = new double[2];
            X[i][0] = double.Parse(parts[0], CultureInfo.InvariantCulture);
            X[i][1] = double.Parse(parts[1], CultureInfo.InvariantCulture);
            y[i] = double.Parse(parts[2], CultureInfo.InvariantCulture);
        }

        return (X, y);
    }

    // Save a 2D array to CSV (no header)
    public static void SaveMatrix(string path, double[,] A)
    {
        int rows = A.GetLength(0), cols = A.GetLength(1);
        using StreamWriter w = new(path);
        for (int i = 0; i < rows; i++)
        {
            var line = string.Join(",", Enumerable.Range(0, cols)
                .Select(j => A[i, j]
                .ToString("G17", CultureInfo.InvariantCulture)));
            w.WriteLine(line);
        }
    }

    // Save a 1D array to CSV (one column, no header)
    public static void SaveVector(string path, double[] v)
    {
        using StreamWriter w = new(path);
        foreach (var val in v)
            w.WriteLine(val.ToString("G17", CultureInfo.InvariantCulture));
    }


    public static double[,] LoadMatrix(string path)
    {
        string[]? lines = File.ReadAllLines(path)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToArray();
        int rows = lines.Length;
        var firstCols = lines[0].Split(',');
        int cols = firstCols.Length;
        var M = new double[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            var parts = lines[i].Split(',');
            for (int j = 0; j < cols; j++)
                M[i, j] = double.Parse(parts[j], CultureInfo.InvariantCulture);
        }
        return M;
    }

    public static double[] LoadVector(string path)
    {
        var lines = File.ReadAllLines(path)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToArray();
        return lines
            .Select(l => double.Parse(l, CultureInfo.InvariantCulture))
            .ToArray();
    }
}
