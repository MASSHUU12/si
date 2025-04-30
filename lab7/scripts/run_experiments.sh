#!/usr/bin/env bash
#
# 1) regenerate data
# 2) train models with different settings (mode=train)
# 3) test them on the held-out test set (mode=test)
# 4) plot the learned surface for each trained model

set -e

# Paths
TRAIN_CSV="./data/train.csv"
TEST_CSV="./data/test.csv"
V_DIR="./models/V"
W_DIR="./models/W"
SURF_DIR="./surfaces"

mkdir -p data
mkdir -p $V_DIR $W_DIR $SURF_DIR

# Generate data
python3 ./scripts/generate_data.py --m 1000 --out $TRAIN_CSV
python3 ./scripts/generate_data.py --m 10000 --out $TEST_CSV

# Sweep hyper-parameters
for neurons in 25 50 100 150; do
  for eta in 0.05 0.1 0.5; do
    for epochs in 50000 100000 500000; do

      # Names for this run
      runname="n${neurons}_e${eta}_ep${epochs}"
      V_PATH="$V_DIR/V_${runname}.csv"
      W_PATH="$W_DIR/W_${runname}.csv"
      LOG="./logs/train_${runname}.log"

      # Train
      dotnet run --project ./MlpRegression \
        --mode train \
        --data $TRAIN_CSV \
        --neurons $neurons \
        --eta $eta \
        --epochs $epochs \
        --batch 32 \
        --V $V_PATH \
        --W $W_PATH \
        >$LOG 2>&1

      echo "Trained model $runname -> saved V,W"

      # Test
      TEST_LOG="./logs/test_${runname}.log"
      dotnet run --project ./MlpRegression \
        --mode test \
        --test $TEST_CSV \
        --V $V_PATH \
        --W $W_PATH \
        >>$LOG 2>&1
      echo "  -> test results appended to $LOG"

      # Plot surface
      OUT_SURF="$SURF_DIR/surf_${runname}.png"
      OUT_SCAT="$SURF_DIR/scatter_${runname}.png"
      python3 ./scripts/plot_surface.py \
        --V $V_PATH \
        --W $W_PATH \
        --grid 80 \
        --out "$OUT_SURF" "$OUT_SCAT"

      echo "  -> surface plot saved to $OUT_SURF"
      echo "  -> scatter plot saved to $OUT_SCAT"
    done
  done
done
