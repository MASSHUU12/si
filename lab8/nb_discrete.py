#!/usr/bin/env python3

import numpy as np
from sklearn.base import BaseEstimator, ClassifierMixin

class NaiveBayesDiscrete(BaseEstimator, ClassifierMixin):
    def __init__(self, n_bins=5, laplace=False):
        self.n_bins = n_bins
        self.laplace = laplace

    def fit(self, X, y):
        X = np.asarray(X)
        y = np.asarray(y)
        self.classes_, counts = np.unique(y, return_counts=True)
        n_classes = len(self.classes_)
        n_samples, n_features = X.shape

        self.priors_ = counts / n_samples
        self.bin_edges_ = [
            np.histogram_bin_edges(X[:, j], bins=self.n_bins)
            for j in range(n_features)
        ]

        Xb = self._discretize(X)

        # Include an extra index 0 that will not be used, so that binned
        # feature values coming out of np.digitize can be used directly as indices
        self.cond_prob_ = [
            np.zeros((n_classes, len(edges) + 1))
            for edges in self.bin_edges_
        ]

        for j, edges in enumerate(self.bin_edges_):
            n_bins_j = len(edges)
            for c_idx, c in enumerate(self.classes_):
                Xb_c = Xb[y == c, j]
                counts = np.bincount(Xb_c, minlength=n_bins_j + 1)
                if self.laplace:
                    counts += 1  # add-one smoothing on all bins
                denom = counts.sum()
                self.cond_prob_[j][c_idx, :] = counts / denom

        return self

    def predict_proba(self, X):
        Xb = self._discretize(X)
        n_samples, n_features = Xb.shape
        n_classes = len(self.classes_)

        log_priors = np.log(self.priors_)
        log_proba = np.zeros((n_samples, n_classes))

        for c_idx in range(n_classes):
            log_p = log_priors[c_idx]
            for j in range(n_features):
                prob_j = self.cond_prob_[j][c_idx, Xb[:, j]]
                log_p += np.log(prob_j)
            log_proba[:, c_idx] = log_p

        # Convert log-probabilities to actual probabilities
        max_log = np.max(log_proba, axis=1, keepdims=True)
        exp_log = np.exp(log_proba - max_log)
        proba = exp_log / exp_log.sum(axis=1, keepdims=True)

        return proba

    def predict(self, X):
        proba = self.predict_proba(X)
        idx = np.argmax(proba, axis=1)
        return self.classes_[idx]

    def _discretize(self, X):
        X = np.asarray(X)
        n_samples, n_features = X.shape
        Xb = np.zeros_like(X, dtype=int)
        for j, edges in enumerate(self.bin_edges_):
            Xb[:, j] = np.digitize(X[:, j], edges, right=False)
        return Xb
