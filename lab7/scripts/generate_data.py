#!/usr/bin/env python3

import numpy as np
import pandas as pd
import argparse

def f(x1, x2):
    return np.cos(x1 * x2) * np.cos(2 * x1)

def main(m, out_csv):
    x1 = np.random.rand(m) * np.pi
    x2 = np.random.rand(m) * np.pi
    y = f(x1, x2)
    df = pd.DataFrame({"x1": x1, "x2": x2, "y": y})
    df.to_csv(out_csv, index=False)

if __name__ == "__main__":
    p = argparse.ArgumentParser()
    p.add_argument("--m", type=int, default=1000)
    p.add_argument("--out", default="../data/train.csv")
    args = p.parse_args()
    main(args.m, args.out)
