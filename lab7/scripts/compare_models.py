#!/usr/bin/env python3

import argparse
import pandas as pd
from sklearn.neural_network import MLPRegressor
from sklearn.metrics import mean_absolute_error, mean_squared_error

def main():
    p = argparse.ArgumentParser(
        description="Compare .NET MLP predictions vs sklearn MLPRegressor"
    )
    p.add_argument("--train",      required=True, help="train CSV (x1,x2,y)")
    p.add_argument("--test",       required=True, help="test  CSV (x1,x2,y)")
    p.add_argument("--dotnet-pred",required=True,
                   help="CSV of .NET predictions with header 'y_pred'")
    p.add_argument("--neurons",    type=int,   required=True)
    p.add_argument("--eta",        type=float, required=True)
    p.add_argument("--epochs",     type=int,   required=True)
    p.add_argument("--batch",      type=int,   default=32)
    args = p.parse_args()

    train = pd.read_csv(args.train)
    test  = pd.read_csv(args.test)
    X_train, y_train = train[['x1','x2']], train['y']
    X_test,  y_test  = test[['x1','x2']],  test['y']

    dotnet_pred = pd.read_csv(args.dotnet_pred)['y_pred'].values

    skl = MLPRegressor(
        hidden_layer_sizes=(args.neurons,),
        activation='logistic',
        solver='sgd',
        learning_rate_init=args.eta,
        max_iter=args.epochs,
        batch_size=args.batch,
        shuffle=True,
        tol=0.0,
        random_state=0
    )
    skl.fit(X_train, y_train)
    skl_pred = skl.predict(X_test)

    for name, pred in [('sklearn', skl_pred), ('.NET', dotnet_pred)]:
        mae  = mean_absolute_error(y_test, pred)
        mse = mean_squared_error(y_test, pred)
        rmse = mse ** 0.5
        print(f"{name:8s} MAE={mae:.6f}, RMSE={rmse:.6f}")

if __name__ == "__main__":
    main()
