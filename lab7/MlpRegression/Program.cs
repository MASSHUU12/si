using System;
using System.Globalization;
using System.Linq;

namespace MlpRegression;

class Program
{
    static void Main(string[] args)
    {
        string? mode = null;     // "train" or "test"
        string? dataPath = null; // for train: --data; for test: --test
        int neurons = 10;
        double eta = 0.01;
        int epochs = 10000;
        int batchSize = 1;
        string modelV = "../data/V.csv"; // where to save / load V
        string modelW = "../data/W.csv"; // where to save / load W

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--mode":
                    mode = args[++i];
                    break;
                case "--data":
                    dataPath = args[++i];
                    break;
                case "--test":
                    dataPath = args[++i];
                    break;
                case "--neurons":
                    neurons = int.Parse(args[++i], CultureInfo.InvariantCulture);
                    break;
                case "--eta":
                    eta = double.Parse(args[++i], CultureInfo.InvariantCulture);
                    break;
                case "--epochs":
                    epochs = int.Parse(args[++i], CultureInfo.InvariantCulture);
                    break;
                case "--batch":
                    batchSize = int.Parse(args[++i], CultureInfo.InvariantCulture);
                    break;
                case "--V":
                    modelV = args[++i];
                    break;
                case "--W":
                    modelW = args[++i];
                    break;
                default:
                    Console.Error.WriteLine($"Unknown arg: {args[i]}");
                    break;
            }
        }

        if (mode != "train" && mode != "test")
        {
            Console.Error.WriteLine("Usage:");
            Console.Error.WriteLine("  Train: dotnet run -- --mode train --data train.csv [--neurons N] [--eta η] [--epochs E] [--batch B] [--V V.csv] [--W W.csv]");
            Console.Error.WriteLine("  Test:  dotnet run -- --mode test --test test.csv --V V.csv --W W.csv");
            return;
        }

        if (mode == "train")
        {
            if (dataPath == null)
            {
                Console.Error.WriteLine("Error: --data is required in train mode.");
                return;
            }

            var (X_train, y_train) = Utils.LoadCsv(dataPath);

            MlpNetwork mlp = new(neurons, eta, epochs, batchSize);
            mlp.Fit(X_train, y_train);

            Utils.SaveMatrix(modelV, mlp.V);
            Utils.SaveVector(modelW, mlp.W);

            Console.WriteLine($"Training complete. Weights saved to '{modelV}' and '{modelW}'.");
        }
        else // mode == "test"
        {
            if (dataPath == null)
            {
                Console.Error.WriteLine("Error: --test is required in test mode.");
                return;
            }

            var (X_test, y_test) = Utils.LoadCsv(dataPath);
            double[,] V = Utils.LoadMatrix(modelV);
            double[] W = Utils.LoadVector(modelW);
            MlpNetwork mlp = new(V, W);

            double[] y_pred = mlp.Predict(X_test);

            // Write CSV to stdout
            //    so that `dotnet run … > predictions.csv` gives:
            //      y_pred
            //      0.12345
            //      0.67890
            //      …
            Console.WriteLine("y_pred");
            foreach (var p in y_pred)
                Console.WriteLine(p.ToString("G17", CultureInfo.InvariantCulture));

            // Compute and report MAE to stderr
            double mae = y_pred
                .Zip(y_test, (pred, actual) => Math.Abs(pred - actual))
                .Average();
            Console.Error.WriteLine($"Test MAE: {mae:F6}");
        }
    }
}
