#!/usr/bin/env python3

import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score

from nb_discrete import NaiveBayesDiscrete
from nb_continuous import NaiveBayesContinuous
from sklearn.naive_bayes import CategoricalNB, GaussianNB
import argparse

def load_data(path, delimiter=","):
    data = np.genfromtxt(path, delimiter=delimiter)
    if data.ndim == 1:  # single sample?
        data = data[np.newaxis, :]
    y = data[:, 0]
    X = data[:, 1:]
    return X, y

def explore_data(X, y):
    print("=== Data exploration ===")
    print(f"Total samples: {X.shape[0]}")
    print(f"Number of features: {X.shape[1]}")

    classes, counts = np.unique(y, return_counts=True)

    print("Class distribution:")
    for c, cnt in zip(classes, counts):
        if np.isnan(c):
            class_str = "NaN"
        else:
            class_str = int(c)
        print(f"  Class {class_str}: {cnt} samples")

    print("Missing values per feature:", np.sum(np.isnan(X), axis=0))
    print()

def run_discrete_nb(X_train, X_test, y_train, y_test, n_bins=10, laplace=True):
    print("=== Discrete Naive Bayes ===")
    print(f"Parameters: n_bins={n_bins}, laplace={laplace}")

    nbd = NaiveBayesDiscrete(n_bins=n_bins, laplace=laplace)
    nbd.fit(X_train, y_train)
    y_pred = nbd.predict(X_test)
    acc_custom = accuracy_score(y_test, y_pred)
    print(f"Custom NaiveBayesDiscrete accuracy: {acc_custom:.4f}")

    Xb_train = nbd._discretize(X_train)
    Xb_test  = nbd._discretize(X_test)
    sk_nbd = CategoricalNB()
    sk_nbd.fit(Xb_train, y_train)
    y_pred_sk = sk_nbd.predict(Xb_test)
    acc_sklearn = accuracy_score(y_test, y_pred_sk)
    print(f"sklearn CategoricalNB accuracy:     {acc_sklearn:.4f}")
    print()

def run_continuous_nb(X_train, X_test, y_train, y_test):
    print("=== Continuous (Gaussian) Naive Bayes ===")

    nbc = NaiveBayesContinuous()
    nbc.fit(X_train, y_train)
    y_pred = nbc.predict(X_test)
    acc_custom = accuracy_score(y_test, y_pred)
    print(f"Custom NaiveBayesContinuous accuracy: {acc_custom:.4f}")

    gnb = GaussianNB()
    gnb.fit(X_train, y_train)
    y_pred_sk = gnb.predict(X_test)
    acc_sklearn = accuracy_score(y_test, y_pred_sk)
    print(f"sklearn GaussianNB accuracy:          {acc_sklearn:.4f}")
    print()

def bin_sensitivity(X_train, X_test, y_train, y_test, bins_list, laplace):
    print("=== Bin count sensitivity ===")
    for b in bins_list:
        nbd = NaiveBayesDiscrete(n_bins=b, laplace=laplace).fit(X_train, y_train)
        acc = accuracy_score(y_test, nbd.predict(X_test))
        print(f"n_bins = {b:2d} -> accuracy = {acc:.4f}")
    print()

def main():
    p = argparse.ArgumentParser(
        description="Run Naive Bayes experiments on a UCI-style .data file"
    )
    p.add_argument("data_path", help="Path to .data file (first column=label)")
    p.add_argument(
        "--delimiter", "-d", default=",",
        help="Delimiter used in the file (default ',')"
    )
    p.add_argument(
        "--test-size", "-t", type=float, default=0.3,
        help="Proportion of data to reserve for testing (default 0.3)"
    )
    p.add_argument(
        "--bins", "-b", type=int, nargs="+", default=[2,5,10,20],
        help="List of bin counts for sensitivity (default 2 5 10 20)"
    )
    p.add_argument(
        "--laplace",  action="store_true",
        help="Apply Laplace smoothing in discrete NB"
    )
    args = p.parse_args()

    X, y = load_data(args.data_path, delimiter=args.delimiter)
    explore_data(X, y)

    X_train, X_test, y_train, y_test = train_test_split(
        X, y,
        test_size=args.test_size,
        random_state=0,
        stratify=y
    )

    run_discrete_nb(X_train, X_test, y_train, y_test, n_bins=max(args.bins), laplace=args.laplace)
    run_continuous_nb(X_train, X_test, y_train, y_test)
    bin_sensitivity(X_train, X_test, y_train, y_test, args.bins, args.laplace)

if __name__ == "__main__":
    main()
