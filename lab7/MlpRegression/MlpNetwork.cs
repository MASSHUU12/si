using System;

namespace MlpRegression;

public class MlpNetwork
{
    public readonly int N;      // number of hidden neurons
    public readonly double Eta;
    public readonly int Epochs;
    public double[,] V;         // hidden weights: size N x (2 inputs + 1 bias)
    public double[] W;          // output weights: size (N hidden + 1 bias)

    private readonly Random rnd = new();

    public MlpNetwork(int neurons, double eta, int epochs)
    {
        N = neurons;
        Eta = eta;
        Epochs = epochs;
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
        V = vLoaded;
        W = wLoaded;
    }

    private void InitializeWeights()
    {
        // small random weights ~ U[-0.5,0.5]
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

    // forward pass: returns (phi_hiddens[], y_out)
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

    public void Fit(double[][] X, double[] Y)
    {
        int M = X.Length;
        for (int ep = 0; ep < Epochs; ep++)
        {
            // stochastic: pick one sample
            int i = rnd.Next(M);
            var xi = X[i];
            double yi_true = Y[i];

            // forward
            var (phi, yi_pred) = Forward(xi);
            double delta = yi_pred - yi_true;

            // update output weights
            // W[0] is bias
            W[0] -= Eta * delta;
            for (int k = 0; k < N; k++)
                W[k + 1] -= Eta * delta * phi[k];

            // backpropagate to V
            for (int k = 0; k < N; k++)
            {
                double grad_k = delta * W[k + 1] * Dsigmoid(phi[k]);
                V[k, 0] -= Eta * grad_k; // bias
                V[k, 1] -= Eta * grad_k * xi[0];
                V[k, 2] -= Eta * grad_k * xi[1];
            }
        }
    }

    public double[] Predict(double[][] X)
    {
        int M = X.Length;
        var preds = new double[M];
        for (int i = 0; i < M; i++)
            preds[i] = Forward(X[i]).y;
        return preds;
    }
}
