#!/usr/bin/env python3
import argparse
import pandas as pd
import matplotlib.pyplot as plt

def main():
    parser = argparse.ArgumentParser(
        description="Plot Time, PS, and BF from CSV files (differentiated by TS)"
    )
    parser.add_argument('csv_files', nargs='+', help='Paths to CSV files')
    args = parser.parse_args()

    fig, axes = plt.subplots(nrows=2, ncols=1, figsize=(8, 12), sharex=True)
    time_ax, bf_ax = axes

    for file in args.csv_files:
        df = pd.read_csv(file)

        ts_value = df['TS'].iloc[0]
        label = f"TS={ts_value}"

        time_ax.plot(df['N'], df['Time'], label=label, marker='o')
        bf_ax.plot(df['N'], df['BF'], label=label, marker='o')

    time_ax.set_ylabel("Time (ms)")
    time_ax.set_title("Execution Time")
    time_ax.legend()

    bf_ax.set_ylabel("Best Fitness")
    bf_ax.set_xlabel("N")
    bf_ax.set_title("Best Fitness")
    bf_ax.legend()

    plt.tight_layout()
    plt.savefig("./assets/tournament_experiment.png", dpi=600)

if __name__ == "__main__":
    main()
