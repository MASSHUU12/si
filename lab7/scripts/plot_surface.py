#!/usr/bin/env python3

"""
Loads V,W and plots the true vs. learned surface over [0,π]^2.

Usage:
    python3 plot_surface.py --V V.csv --W W.csv [--grid N] [--out file.png] [--show]

Arguments:
    --V       Path to hidden-layer weight matrix CSV.
    --W       Path to output-layer weight vector CSV.
    --grid    Number of points per axis (default: 50).
    --out     Path where to save the figure (default: surface.png).
    --show    If given, display interactively instead of saving.
"""

import argparse
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

def load_weights(v_path, w_path):
    V = pd.read_csv(v_path, header=None).values         # shape (N, 3)
    W = pd.read_csv(w_path, header=None).values.ravel() # shape (N+1,)
    return V, W

def sigmoid(z):
    return 1.0 / (1.0 + np.exp(-z))

def mlp_predict(V, W, X):
    M = X.shape[0]
    Xb = np.hstack([np.ones((M,1)), X]) # (M,3)
    H = sigmoid(Xb.dot(V.T))            # (M,N)
    Hb = np.hstack([np.ones((M,1)), H]) # (M,N+1)
    return Hb.dot(W)                    # (M,)

def true_function(X1, X2):
    return np.cos(X1 * X2) * np.cos(2 * X1)

if __name__ == "__main__":
    p = argparse.ArgumentParser()
    p.add_argument('--V', required=True)
    p.add_argument('--W', required=True)
    p.add_argument('--grid', type=int, default=50)
    p.add_argument('--out', nargs=2, metavar=('SURF','SCAT'),
                   help="output files: surface.png scatter.png",
                   default=['surface.png','scatter.png'])
    p.add_argument('--show', action='store_true')
    args = p.parse_args()

    V, W = load_weights(args.V, args.W)

    xs = np.linspace(0, np.pi, args.grid)
    ys = np.linspace(0, np.pi, args.grid)
    X1, X2 = np.meshgrid(xs, ys)
    XY = np.column_stack([X1.ravel(), X2.ravel()])

    Z_true = true_function(X1, X2)
    Z_pred = mlp_predict(V, W, XY).reshape(args.grid, args.grid)

    # Surface plots
    fig = plt.figure(figsize=(12,6))
    ax1 = fig.add_subplot(1,2,1, projection='3d')
    ax1.plot_surface( X1, X2, Z_true, rstride=1, cstride=1, edgecolor='none' )
    ax1.set_title('True function');    ax1.set_xlabel('x1'); ax1.set_ylabel('x2'); ax1.set_zlabel('f(x1,x2)')
    ax2 = fig.add_subplot(1,2,2, projection='3d')
    ax2.plot_surface( X1, X2, Z_pred, rstride=1, cstride=1, edgecolor='none' )
    ax2.set_title('MLP prediction'); ax2.set_xlabel('x1'); ax2.set_ylabel('x2'); ax2.set_zlabel('ŷ(x1,x2)')
    fig.tight_layout()
    plt.savefig(args.out[0], dpi=150)
    print(f"Saved surface plot to '{args.out[0]}'")

    # Scatter plots
    fig2 = plt.figure(figsize=(12,6))
    ax3 = fig2.add_subplot(1,2,1, projection='3d')
    ax3.scatter( X1.ravel(), X2.ravel(), Z_true.ravel(), marker='.', alpha=0.7 )
    ax3.set_title('True function (scatter)');    ax3.set_xlabel('x1'); ax3.set_ylabel('x2'); ax3.set_zlabel('f(x1,x2)')
    ax4 = fig2.add_subplot(1,2,2, projection='3d')
    ax4.scatter( X1.ravel(), X2.ravel(), Z_pred.ravel(), marker='.', alpha=0.7 )
    ax4.set_title('MLP prediction (scatter)'); ax4.set_xlabel('x1'); ax4.set_ylabel('x2'); ax4.set_zlabel('ŷ(x1,x2)')
    fig2.tight_layout()
    plt.savefig(args.out[1], dpi=150)
    print(f"Saved scatter plot to '{args.out[1]}'")
