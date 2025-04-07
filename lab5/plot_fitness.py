import pandas as pd
import matplotlib.pyplot as plt

def plot_fitness_evolution():
    df = pd.read_csv("./assets/fitness_data.csv")

    plt.figure(figsize=(12, 6))
    plt.plot(df['Generation'], df['BestFitness'], label='Best Fitness', color='blue',
             marker='o', linestyle='-', linewidth=2, markersize=6)
    plt.plot(df['Generation'], df['AvgFitness'], label='Average Fitness', color='red',
             marker='x', linestyle='--', linewidth=2, markersize=6)

    plt.title('Fitness Evolution over Generations', fontsize=16)
    plt.xlabel('Generation', fontsize=14)
    plt.ylabel('Fitness (Number of Attacks)', fontsize=14)

    plt.xticks(fontsize=12)
    plt.yticks(fontsize=12)
    plt.legend(fontsize=12)

    plt.grid(True, linestyle='--', linewidth=0.5)
    plt.tight_layout()
    plt.xscale('log')
    plt.savefig("./assets/fitness_evolution.png", dpi=600)

def main():
    plot_fitness_evolution()

if __name__ == "__main__":
    main()
