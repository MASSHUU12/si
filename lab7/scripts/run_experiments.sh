#!/usr/bin/env bash

set -e

python3 generate_data.py --m 1000 --out ../data/train.csv
python3 generate_data.py --m 10000 --out ../data/test.csv

for neurons in 5 10 20; do
  for eta in 0.001 0.01 0.1; do
    dotnet run --project ../src/MlpRegression \
      --data ../data/train.csv \
      --neurons $neurons \
      --epochs 5000 \
      --eta $eta \
      >logs/log_n${neurons}_e${eta}.txt
    python3 plot_surface.py \
      --V V.csv --W W.csv \
      --out surfaces/surf_n${neurons}_e${eta}.png
  done
done
