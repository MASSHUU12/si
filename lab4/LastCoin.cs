using System;
using System.Collections.Generic;

internal class LastCoin(int maxTake = 3, int coins = 5)
{
    private readonly Dictionary<int, List<int>> tree = [];
    private int nodesGenerated;
    private readonly List<string> nodeProcessingOrder = [];
    private readonly List<string> alphaBetaChanges = [];
    private readonly List<string> cutOffPoints = [];

    private int AlphaBetaEvaluateMaxState(int state, int alpha = -100, int beta = 100)
    {
        nodesGenerated++;
        nodeProcessingOrder.Add($"Max State: {state}, Alpha: {alpha}, Beta: {beta}");

        if (IsTerminal(state))
        {
            return Payoff(state);
        }

        int value = int.MinValue;
        List<int> children = GenerateChildren(state);

        foreach (int child in children)
        {
            value = Math.Max(value, AlphaBetaEvaluateMinState(child, alpha, beta));
            alpha = Math.Max(alpha, value);
            alphaBetaChanges.Add($"Max State: {state}, Child: {child}, Alpha: {alpha}, Beta: {beta}");

            if (alpha >= beta)
            {
                cutOffPoints.Add($"Cut-off at Max State: {state}, Child: {child}, Alpha: {alpha}, Beta: {beta}");
                break;
            }
        }
        return value;
    }

    private int AlphaBetaEvaluateMinState(int state, int alpha = -100, int beta = 100)
    {
        nodesGenerated++;
        nodeProcessingOrder.Add($"Min State: {state}, Alpha: {alpha}, Beta: {beta}");

        if (IsTerminal(state))
        {
            return Payoff(state);
        }

        int value = int.MaxValue;
        List<int> children = GenerateChildren(state);

        foreach (int child in children)
        {
            value = Math.Min(value, AlphaBetaEvaluateMaxState(child, alpha, beta));
            beta = Math.Min(beta, value);
            alphaBetaChanges.Add($"Min State: {state}, Child: {child}, Alpha: {alpha}, Beta: {beta}");

            if (alpha >= beta)
            {
                cutOffPoints.Add($"Cut-off at Min State: {state}, Child: {child}, Alpha: {alpha}, Beta: {beta}");
                break;
            }
        }
        return value;
    }

    private static bool IsTerminal(int state)
    {
        return state <= 0;
    }

    private static int Payoff(int state)
    {
        return state == 0 ? -100 : 100;
    }

    private List<int> GenerateChildren(int state)
    {
        List<int> children = [];
        for (int i = 1; i <= maxTake; i++)
        {
            if (state - i >= 0)
            {
                children.Add(state - i);
                if (!tree.TryGetValue(state, out List<int>? value))
                {
                    value = [];
                    tree[state] = value;
                }

                value.Add(state - i);
            }
        }
        return children;
    }

    public void Play()
    {
        int bestMove = -1;
        int bestValue = int.MinValue;
        List<int> children = GenerateChildren(coins);

        foreach (int child in children)
        {
            int value = AlphaBetaEvaluateMinState(child);
            if (value > bestValue)
            {
                bestValue = value;
                bestMove = child;
            }
        }

        Console.WriteLine($"Best move for player A is to take {coins - bestMove} coins.");
        Console.WriteLine($"Nodes generated: {nodesGenerated}");

        Console.WriteLine("\nAlpha-Beta Changes:");
        foreach (string change in alphaBetaChanges)
        {
            Console.WriteLine(change);
        }

        Console.WriteLine("\nNode Processing Order:");
        foreach (string order in nodeProcessingOrder)
        {
            Console.WriteLine(order);
        }

        Console.WriteLine("\nCut-off Points:");
        foreach (string cutOff in cutOffPoints)
        {
            Console.WriteLine(cutOff);
        }
        DrawTree();
    }

    private void DrawTree()
    {
        Console.WriteLine("\nGame Tree:");
        DrawSubTree(coins, 0, "Root");
    }

    private void DrawSubTree(int state, int depth, string label)
    {
        Console.WriteLine($"{new string(' ', depth * 4)}{label}: {state}");
        if (tree.TryGetValue(state, out List<int>? value))
        {
            for (int i = 0; i < value.Count; i++)
            {
                DrawSubTree(value[i], depth + 1, $"Child {i + 1}");
            }
        }
    }
}
