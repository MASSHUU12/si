import matplotlib.pyplot as plt
import pandas as pd
import sys
import os

def plot_nqueens_statistics(csv_file):
    """
    Create three graphs showing statistics for BFS and DFS N-Queens algorithms.
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

    fig, axs = plt.subplots(3, 1, figsize=(10, 15))
    fig.suptitle('N-Queens Problem: BFS vs DFS Comparison', fontsize=16)

    bfs_color = 'blue'
    dfs_color = 'red'
    bfs_marker = 'o'
    dfs_marker = 's'

    # Plot 1: Open List Size (now using BFS-Max and DFS-Max)
    axs[0].plot(data['n'], data['BFS-Max'], color=bfs_color, marker=bfs_marker, label='BFS')
    axs[0].plot(data['n'], data['DFS-Max'], color=dfs_color, marker=dfs_marker, label='DFS')
    axs[0].set_title('Open List Size vs Number of Queens')
    axs[0].set_xlabel('Number of Queens (n)')
    axs[0].set_ylabel('Open List Size')
    axs[0].grid(True)
    axs[0].legend()

    # Plot 2: Closed List Size
    axs[1].plot(data['n'], data['BFS-Closed'], color=bfs_color, marker=bfs_marker, label='BFS')
    axs[1].plot(data['n'], data['DFS-Closed'], color=dfs_color, marker=dfs_marker, label='DFS')
    axs[1].set_title('Closed List Size vs Number of Queens')
    axs[1].set_xlabel('Number of Queens (n)')
    axs[1].set_ylabel('Closed List Size (States Checked)')
    axs[1].grid(True)
    axs[1].legend()

    # Plot 3: Execution Time (now using BFS-Time and DFS-Time)
    axs[2].plot(data['n'], data['BFS-Time'], color=bfs_color, marker=bfs_marker, label='BFS')
    axs[2].plot(data['n'], data['DFS-Time'], color=dfs_color, marker=dfs_marker, label='DFS')
    axs[2].set_title('Execution Time vs Number of Queens')
    axs[2].set_xlabel('Number of Queens (n)')
    axs[2].set_ylabel('Execution Time (seconds)')  # Updated to seconds instead of ms
    axs[2].grid(True)
    axs[2].legend()

    # Use logarithmic scale if the values span multiple orders of magnitude
    for ax in axs:
        y_min, y_max = ax.get_ylim()
        if y_max / max(y_min, 1) > 100:  # If range spans more than 2 orders of magnitude
            ax.set_yscale('log')

    # Adjust layout
    plt.tight_layout(rect=[0, 0, 1, 0.95])  # Adjust to make room for the suptitle

    # Save the figure
    output_file = './assets/nqueens_statistics.png'
    plt.savefig(output_file, dpi=600)
    print(f"Plots saved to {output_file}")

def main():
    # Check if CSV file is provided as command-line argument
    if len(sys.argv) > 1:
        csv_file = sys.argv[1]
    else:
        # Look for CSV files in the current directory
        csv_files = [f for f in os.listdir('.') if f.endswith('.csv')]
        if not csv_files:
            print("No CSV file specified and no CSV files found in the current directory.")
            print("Usage: python plot_nqueens.py [csv_file]")
            return

        # If there's only one CSV file, use it
        if len(csv_files) == 1:
            csv_file = csv_files[0]
            print(f"Using found CSV file: {csv_file}")
        else:
            # If there are multiple CSV files, ask the user to choose
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
