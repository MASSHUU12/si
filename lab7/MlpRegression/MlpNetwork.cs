using System;
using System.Linq;

namespace MlpRegression;

public class MlpNetwork
{
    public readonly int N;      // number of hidden neurons
    public readonly double Eta;
    public readonly int Epochs;
    public readonly int BatchSize;
    public double[,] V;         // hidden weights: size N x (2 inputs + 1 bias)
    public double[] W;          // output weights: size (N hidden + 1 bias)

    private readonly Random rnd = new();

    public MlpNetwork(int neurons, double eta, int epochs, int batchSize)
    {
        N = neurons;
        Eta = eta;
        Epochs = epochs;
        BatchSize = batchSize;
        V = new double[N, 3];
        W = new double[N + 1];
        InitializeWeights();
    }

    public MlpNetwork(double[,] vLoaded, double[] wLoaded)
    {
        // infer N from loaded dimensions
        N = vLoaded.GetLength(0);
        Eta = 0; // no training
        Epochs = 0;
        BatchSize = 1;
        V = vLoaded;
        W = wLoaded;
    }

    private void InitializeWeights()
    {
        // Small random weights ~ U[-0.5,0.5]
        for (int i = 0; i < N; i++)
            for (int j = 0; j < 3; j++)
                V[i, j] = rnd.NextDouble() - 0.5;
        for (int i = 0; i < W.Length; i++)
            W[i] = rnd.NextDouble() - 0.5;
    }

    private static double Sigmoid(double z)
        => 1.0 / (1.0 + Math.Exp(-z));

    private static double Dsigmoid(double phi)
        => phi * (1 - phi);

    // Forward pass: returns (phi_hiddens[], y_out)
    private (double[] phi, double y) Forward(double[] x)
    {
        // x: length 2
        var phi = new double[N];
        for (int i = 0; i < N; i++)
        {
            // bias + inputs
            double sum = V[i, 0] + V[i, 1] * x[0] + V[i, 2] * x[1];
            phi[i] = Sigmoid(sum);
        }

        // output: bias weight W[0] plus weighted hidden
        double y = W[0];
        for (int i = 0; i < N; i++)
            y += W[i + 1] * phi[i];
        return (phi, y);
    }

    // Mini-batch training
    public void Fit(double[][] X, double[] Y)
    {
        int M = X.Length;
        var indices = Enumerable.Range(0, M).ToArray();

        for (int ep = 0; ep < Epochs; ep++)
        {
            // Sample a random mini-batch of indices
            var batchIdx = indices
                .OrderBy(_ => rnd.Next())
                .Take(BatchSize)
                .ToArray();

            // Zero accumulators
            double[] gradW = new double[N + 1];
            double[,] gradV = new double[N, 3];

            // Accumulate gradients over the batch
            foreach (int i in batchIdx)
            {
                var x = X[i];
                double y_true = Y[i];
                var (phi, y_pred) = Forward(x);
                double delta = y_pred - y_true;

                // Output-layer grads
                gradW[0] += delta; // bias
                for (int k = 0; k < N; k++)
                    gradW[k + 1] += delta * phi[k];

                // Hidden-layer grads
                for (int k = 0; k < N; k++)
                {
                    double gk = delta * W[k + 1] * Dsigmoid(phi[k]);
                    gradV[k, 0] += gk; // bias
                    gradV[k, 1] += gk * x[0];
                    gradV[k, 2] += gk * x[1];
                }
            }

            // Average and apply update
            double scale = Eta / BatchSize;
            for (int j = 0; j < W.Length; j++)
                W[j] -= scale * gradW[j];
            for (int k = 0; k < N; k++)
                for (int j = 0; j < 3; j++)
                    V[k, j] -= scale * gradV[k, j];
        }
    }

    public double[] Predict(double[][] X)
    {
        return X.Select(x => Forward(x).y).ToArray();
    }
}
