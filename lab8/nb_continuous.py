#!/usr/bin/env python3

import numpy as np
from sklearn.base import BaseEstimator, ClassifierMixin

class NaiveBayesContinuous(BaseEstimator, ClassifierMixin):
    def __init__(self):
        pass

    def fit(self, X, y):
        X = np.asarray(X)
        y = np.asarray(y)
        self.classes_, counts = np.unique(y, return_counts=True)
        n_samples, n_features = X.shape

        self.priors_ = counts / n_samples

        # Compute per-class mean and variance for each feature
        # Store as dict: class -> (mean_vector, var_vector)
        self.params_ = {}
        for idx, c in enumerate(self.classes_):
            X_c = X[y == c]
            mu = X_c.mean(axis=0)
            var = X_c.var(axis=0)
            # Avoid zero variance
            var[var == 0] = 1e-9
            self.params_[c] = (mu, var)

        return self

    def _joint_log_likelihood(self, X):
        X = np.asarray(X)
        n_samples, n_features = X.shape
        n_classes = len(self.classes_)
        jll = np.zeros((n_samples, n_classes))

        for idx, c in enumerate(self.classes_):
            mu, var = self.params_[c]
            log_prior = np.log(self.priors_[idx])
            log_likelihood = -0.5 * (
                ((X - mu) ** 2) / var
                + np.log(2 * np.pi * var)
            )
            total_ll = log_likelihood.sum(axis=1)
            jll[:, idx] = log_prior + total_ll

        return jll

    def predict_proba(self, X):
        jll = self._joint_log_likelihood(X)
        log_prob_x = np.logaddexp.reduce(jll, axis=1, keepdims=True)
        return np.exp(jll - log_prob_x)

    def predict(self, X):
        jll = self._joint_log_likelihood(X)
        class_index = np.argmax(jll, axis=1)
        return self.classes_[class_index]
