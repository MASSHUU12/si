import matplotlib.pyplot as plt
import pandas as pd
import sys
import os

def plot_nqueens_statistics(csv_file):
    """
    Create graphs showing statistics for BFS, DFS, and Best First Search (H1, H2, and Hdod) N-Queens algorithms.
    The graphs show:
    1. Open list count vs N
    2. Closed list count vs N
    3. Execution time vs N
    """
    try:
        data = pd.read_csv(csv_file)
        print(f"Successfully read data from {csv_file}")
        print(f"Data contains {len(data)} rows for N={data['n'].min()} to N={data['n'].max()}")
    except Exception as e:
        print(f"Error reading CSV file: {e}")
        return

    fig, axs = plt.subplots(3, 1, figsize=(12, 18))
    fig.suptitle('N-Queens Problem: BFS vs DFS vs Best First (H1, H2 & Hdod) Comparison', fontsize=16)

    colors = {'BFS': 'blue', 'DFS': 'red', 'BH1': 'green', 'BH2': 'purple', 'Hdod': 'orange'}
    markers = {'BFS': 'o', 'DFS': 's', 'BH1': 'D', 'BH2': '^', 'Hdod': 'v'}

    # Plot 1: Open List Size
    axs[0].plot(data['n'], data['BFS-Max'], color=colors['BFS'], marker=markers['BFS'], label='BFS')
    axs[0].plot(data['n'], data['DFS-Max'], color=colors['DFS'], marker=markers['DFS'], label='DFS')
    axs[0].plot(data['n'], data['BH1-Max'], color=colors['BH1'], marker=markers['BH1'], label='Best First (H1)')
    axs[0].plot(data['n'], data['BH2-Max'], color=colors['BH2'], marker=markers['BH2'], label='Best First (H2)')
    axs[0].plot(data['n'], data['Hdod-Max'], color=colors['Hdod'], marker=markers['Hdod'], label='Best First (Hdod)')
    axs[0].set_title('Open List Size vs Number of Queens')
    axs[0].set_xlabel('Number of Queens (n)')
    axs[0].set_ylabel('Open List Size')
    axs[0].grid(True)
    axs[0].legend()

    # Plot 2: Closed List Size
    axs[1].plot(data['n'], data['BFS-Cls'], color=colors['BFS'], marker=markers['BFS'], label='BFS')
    axs[1].plot(data['n'], data['DFS-Cls'], color=colors['DFS'], marker=markers['DFS'], label='DFS')
    axs[1].plot(data['n'], data['BH1-Cls'], color=colors['BH1'], marker=markers['BH1'], label='Best First (H1)')
    axs[1].plot(data['n'], data['BH2-Cls'], color=colors['BH2'], marker=markers['BH2'], label='Best First (H2)')
    axs[1].plot(data['n'], data['Hdod-Cls'], color=colors['Hdod'], marker=markers['Hdod'], label='Best First (Hdod)')
    axs[1].set_title('Closed List Size vs Number of Queens')
    axs[1].set_xlabel('Number of Queens (n)')
    axs[1].set_ylabel('Closed List Size (States Checked)')
    axs[1].grid(True)
    axs[1].legend()

    # Plot 3: Execution Time
    axs[2].plot(data['n'], data['BFS-T'], color=colors['BFS'], marker=markers['BFS'], label='BFS')
    axs[2].plot(data['n'], data['DFS-T'], color=colors['DFS'], marker=markers['DFS'], label='DFS')
    axs[2].plot(data['n'], data['BH1-T'], color=colors['BH1'], marker=markers['BH1'], label='Best First (H1)')
    axs[2].plot(data['n'], data['BH2-T'], color=colors['BH2'], marker=markers['BH2'], label='Best First (H2)')
    axs[2].plot(data['n'], data['Hdod-T'], color=colors['Hdod'], marker=markers['Hdod'], label='Best First (Hdod)')
    axs[2].set_title('Execution Time vs Number of Queens')
    axs[2].set_xlabel('Number of Queens (n)')
    axs[2].set_ylabel('Execution Time (seconds)')
    axs[2].grid(True)
    axs[2].legend()

    # Use logarithmic scale if the values span multiple orders of magnitude
    for ax in axs:
        y_min, y_max = ax.get_ylim()
        if y_max / max(y_min, 1) > 100:
            ax.set_yscale('log')

    # Adjust layout
    plt.tight_layout(rect=[0, 0, 1, 0.95])

    # Save the figure
    output_file = './assets/nqueens_statistics.png'
    plt.savefig(output_file, dpi=600)
    print(f"Plots saved to {output_file}")

def main():
    if len(sys.argv) > 1:
        csv_file = sys.argv[1]
    else:
        csv_files = [f for f in os.listdir('.') if f.endswith('.csv')]
        if not csv_files:
            print("No CSV file specified and no CSV files found in the current directory.")
            print("Usage: python plot_nqueens.py [csv_file]")
            return

        if len(csv_files) == 1:
            csv_file = csv_files[0]
            print(f"Using found CSV file: {csv_file}")
        else:
            print("Multiple CSV files found. Please choose one:")
            for i, file in enumerate(csv_files):
                print(f"{i+1}. {file}")

            try:
                choice = int(input("Enter the number of your choice: "))
                if 1 <= choice <= len(csv_files):
                    csv_file = csv_files[choice-1]
                else:
                    print("Invalid choice. Exiting.")
                    return
            except ValueError:
                print("Invalid input. Exiting.")
                return

    plot_nqueens_statistics(csv_file)

if __name__ == "__main__":
    main()
