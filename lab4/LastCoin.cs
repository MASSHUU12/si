using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

internal class TreeNode(int state)
{
    private static int idCounter;
    public int Id { get; } = idCounter++;
    public int State { get; set; } = state;
    public List<TreeNode> Children { get; set; } = [];
}

internal class LastCoin(int maxTake = 3, int coins = 5)
{
    private int nodesGenerated;
    private readonly List<string> cutOffPoints = [];
    private readonly List<string> alphaBetaChanges = [];
    private readonly List<string> nodeProcessingOrder = [];

    public TreeNode BuildGameTree(int state)
    {
        TreeNode node = new(state);

        if (IsTerminal(state))
        {
            return node;
        }

        for (int i = 1; i <= maxTake; i++)
        {
            if (state - i >= 0)
            {
                node.Children.Add(BuildGameTree(state - i));
            }
        }
        return node;
    }

    private int AlphaBetaEvaluateMax(
        TreeNode node,
        int alpha = -100,
        int beta = 100
    )
    {
        nodesGenerated++;
        nodeProcessingOrder.Add(
            $"Max State: {node.State}, Alpha: {alpha}, Beta: {beta}"
        );

        if (IsTerminal(node.State))
        {
            return Payoff(node.State);
        }

        int value = int.MinValue;
        foreach (TreeNode child in node.Children)
        {
            int childVal = AlphaBetaEvaluateMin(child, alpha, beta);
            value = Math.Max(value, childVal);
            alpha = Math.Max(alpha, value);
            alphaBetaChanges.Add(
                $"Max State: {node.State}, Child: {child.State}, Alpha: {alpha}, Beta: {beta}"
            );

            if (alpha >= beta)
            {
                cutOffPoints.Add(
                    $"Cut-off at Max State: {node.State}, Child: {child.State}, Alpha: {alpha}, Beta: {beta}"
                );
                break;
            }
        }
        return value;
    }

    private int AlphaBetaEvaluateMin(
        TreeNode node,
        int alpha = -100,
        int beta = 100
    )
    {
        nodesGenerated++;
        nodeProcessingOrder.Add(
            $"Min State: {node.State}, Alpha: {alpha}, Beta: {beta}"
        );

        if (IsTerminal(node.State))
        {
            return Payoff(node.State);
        }

        int value = int.MaxValue;
        foreach (TreeNode child in node.Children)
        {
            int childVal = AlphaBetaEvaluateMax(child, alpha, beta);
            value = Math.Min(value, childVal);
            beta = Math.Min(beta, value);
            alphaBetaChanges.Add(
                $"Min State: {node.State}, Child: {child.State}, Alpha: {alpha}, Beta: {beta}"
            );

            if (alpha >= beta)
            {
                cutOffPoints.Add(
                    $"Cut-off at Min State: {node.State}, Child: {child.State}, Alpha: {alpha}, Beta: {beta}"
                );
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

    public void Play()
    {
        TreeNode root = BuildGameTree(coins);

        int bestValue = int.MinValue;
        int bestMoveState = -1;
        foreach (TreeNode child in root.Children)
        {
            int value = AlphaBetaEvaluateMin(child);
            if (value > bestValue)
            {
                bestValue = value;
                bestMoveState = child.State;
            }
        }

        Console.WriteLine(
            $"Best move for player A is to take {coins - bestMoveState} coins."
        );
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

        // Console.WriteLine("\nGame Tree:");
        // PrintTree(root);
        ExportTreeAsDOT(root, "./assets/gametree.dot");
    }

    public static void PrintTree(TreeNode node, int depth = 0)
    {
        Console.WriteLine($"{new string(' ', depth * 4)}State: {node.State}");
        foreach (TreeNode child in node.Children)
        {
            PrintTree(child, depth + 1);
        }
    }

    public static void ExportTreeAsDOT(TreeNode root, string filePath)
    {
        StringBuilder sb = new();
        _ = sb.AppendLine("digraph GameTree {");
        _ = sb.AppendLine("    rankdir=TB;");
        _ = sb.AppendLine(
            "    node [shape=circle, style=filled, fillcolor=white, fontname=\"Helvetica\", fontsize=12];"
        );

        AppendDOT(root, sb);

        _ = sb.AppendLine("}");
        File.WriteAllText(filePath, sb.ToString());
        Console.WriteLine($"DOT file exported to {filePath}");
    }

    private static void AppendDOT(TreeNode node, StringBuilder sb)
    {
        _ = sb.AppendLine(
            CultureInfo.InvariantCulture,
            $"    node{node.Id} [label=\"ID: {node.Id}\\nState: {node.State}\"];"
        );

        foreach (TreeNode child in node.Children)
        {
            _ = sb.AppendLine(
                CultureInfo.InvariantCulture,
                $"    node{node.Id} -> node{child.Id};"
            );
            AppendDOT(child, sb);
        }
    }
}
