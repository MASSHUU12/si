#!/usr/bin/env bash

set -euo pipefail

TRAIN_CSV="./data/train.csv"
TEST_CSV="./data/test.csv"
V_DIR="./models/V"
W_DIR="./models/W"
SURF_DIR="./surfaces"
LOG_DIR="./logs"

mkdir -p data $V_DIR $W_DIR $SURF_DIR $LOG_DIR

echo "Generating data..."
python3 ./scripts/generate_data.py --m 1000 --out $TRAIN_CSV
python3 ./scripts/generate_data.py --m 10000 --out $TEST_CSV

for neurons in 25 50 100 150; do
  for eta in 0.05 0.1 0.5; do
    for epochs in 50000 100000 500000; do

      runname="n${neurons}_e${eta}_ep${epochs}"
      V_PATH="$V_DIR/V_${runname}.csv"
      W_PATH="$W_DIR/W_${runname}.csv"
      DOTNET_LOG="$LOG_DIR/train_test_${runname}.log"
      PRED_CSV="models/pred_${runname}.csv"

      echo "=== RUN $runname ===" | tee $DOTNET_LOG

      echo "[.NET] Training MLP..." | tee -a $DOTNET_LOG
      dotnet run --project ./MlpRegression \
        --mode train \
        --data $TRAIN_CSV \
        --neurons $neurons \
        --eta $eta \
        --epochs $epochs \
        --batch 32 \
        --V $V_PATH \
        --W $W_PATH \
        >>$DOTNET_LOG 2>&1
      echo "[.NET] Training complete." | tee -a $DOTNET_LOG

      echo "[.NET] Testing MLP..." | tee -a $DOTNET_LOG
      dotnet run --project ./MlpRegression \
        --mode test \
        --test $TEST_CSV \
        --V $V_PATH \
        --W $W_PATH \
        >"$PRED_CSV"
      echo "[.NET] Predictions -> $PRED_CSV" | tee -a "$DOTNET_LOG"

      echo "[Python] Comparing models..." | tee -a "$DOTNET_LOG"
      python3 ./scripts/compare_models.py \
        --train "$TRAIN_CSV" \
        --test "$TEST_CSV" \
        --dotnet-pred "$PRED_CSV" \
        --neurons "$neurons" \
        --eta "$eta" \
        --epochs "$epochs" \
        --batch 32 \
        >>"$DOTNET_LOG"

      echo "[Plot] Generating surface plots..." | tee -a "$DOTNET_LOG"
      python3 ./scripts/plot_surface.py \
        --V "$V_PATH" \
        --W "$W_PATH" \
        --grid 80 \
        --out "$SURF_DIR/surf_${runname}.png" "$SURF_DIR/scatter_${runname}.png"
      echo "[Plot] Saved surfaces for $runname" | tee -a "$DOTNET_LOG"

      echo "=== END RUN $runname ===" | tee -a "$DOTNET_LOG"
      echo "" | tee -a "$DOTNET_LOG"
    done
  done
done

echo "All experiments completed."
