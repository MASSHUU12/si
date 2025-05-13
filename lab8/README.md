# Naiwny klasyfikator Bayesa (NBC)

## 1. Wstęp

Celem ćwiczenia było zaimplementowanie dwóch wariantów naiwnych
klasyfikatorów Bayesa:

- **dyskretny (Discrete NBC)** z opcjonalną poprawką Laplace'a.
- **ciągły (Gaussian NBC)** przy założeniu rozkładu normalnego.

Następnie przeprowadzono eksperymenty podstawowe (70% danych uczących,
30% testowych) oraz dodatkowe, porównując własne implementacje ze scikit-learn
(`CategoricalNB` i `GaussianNB`).

## 2. Dane i eksploracja

### 2.1 Zbiór Wine

- **Żródło**: [UCI Wine Data Set](https://archive.ics.uci.edu/dataset/109/wine)
- **Próbki**: 178
- **Atrybuty**: 13 cech chemicznych
- **Klasy**: 3 (1: 59, 2: 71, 3:48)
- **Brakujące wartości**: 0 na wszystkich kolumnach

### 2.2 Zbiór Iris

- **Żródło**: [UCI Iris Data Set](https://archive.ics.uci.edu/dataset/53/iris)
- **Próbki**: 150
- **Atrybuty**: 4 (długość i szerokość działki kielicha i płatka)
- **Klasy**: 3 po 50 próbek każda
- **Brakujące wartości**: 0 na wszystkich kolumnach

## 3. Przygotowanie danych

1. **Wczytanie i podział**
  Dane podzielono funkcją `train_test_split` w proporcji 70% (uczenie) / 30%
  (test) ze stratifikacją po klasach i `random_state=0`:
  - Wine: 124 próbek uczących, 54 testowych
  - Iris: 105 próbek uczących, 45 testowych

2. **Dyskretyzacja (tylko dla wersji dyskretnej)**
  Ilośc przedziałów (`n_bins`) traktowano jako parametr eksperymentu (2, 5, 10, 20).

## 4. Eksperymenty podstawowe

Dokładne wyniki można znaleźć w folderze [results](./results).

### 4.1 NBC dyskretny

Dla obu zbiorów przeprowadzono klasyfikację:

- bez poprawki Laplace'a
- z poprawką Laplace'a
- a także wykorzystano czystą wersję `CategoricalNB` na tych samych dyskretyzacjach.

**Wyniki (n_bins=20)**

| Zbiór | Laplace=False (własna) | Laplace=False (sklearn) | Laplace=True (własna) | Laplace=True (sklearn) |
| ----- | ---------------------- | ----------------------- | --------------------- | ---------------------- |
| Iris  | 95.56%                 | 97.78%                  | 97.78%                | 97.78%                 |
| Wine  | 94.44%                 | 98.15%                  | 98.15%                | 98.15%                 |

### 4.2 NBC ciągły

Dla obu zbiorów porównano własne `NaiveBayesContinuous` z `GaussianNB`:

| Zbiór    | Własna implementacja | scikit-learn |
| -------- | -------------------- | ------------ |
| Iris     | 97.78 %              | 97.78 %      |
| Wine     | 96.30 %              | 96.30 %      |

## 5. Eksperymenty dodatkowe

### 5.1 Wpływ liczby przedziałów

Badano dokładność wersji dyskretnej (Laplace=True) w zależności od `n_bins`:

| Zbiór    | n_bins=2 | n_bins=5 | n_bins=10 | n_bins=20 |
| -------- | -------- | -------- | --------- | --------- |
| **Iris** | 73.33 %  | 93.33 %  | 95.56 %   | 97.78 %   |
| **Wine** | 90.74 %  | 96.30 %  | 100.00 %  | 98.15 %   |

### 5.2 Wersja logarytmiczna

Dla `laplace=False` sprawdzono także klasę wyznaczaną jako `argmax`
nieustrukturyzowanego log-prawdopodobieństwa (`predict_log_proba`):

- Wyniki `argmax(log-proby)` pokrywają się z `predict` (bez softmax) -
  dowód poprawności implementacji.

### 5.3 Predict_proba i log-loss

Dla przykładu pierwszych 5 próbek oraz całego zbioru testowego zmierzono:

- Posteriorne rozkłady klas
- Log-loss (`sklearn.metrics.log_loss`)

**Przykład - Iris,** `laplace=False`

```txt
First 5 custom proba:
  [[2.04e-24, 1.36e-24, 1.00e+00], …]
Log-loss (custom): 0.0913
Log-loss (sklearn): 0.1270
```

## 6. Wnioski

1. **Poprawka Laplace'a** znacząco poprawia dokładność dla niskiego `n_bins`, a
przy `n_bins=20` stabilizuje wyniki równolegle do scikit-learn.
2. **Liczba przedziałów** ma duży wpływ: niewielka ich ilość (2-5) prowadzi do
przeuczenia lub niedouczenia, optimum w okolicy 10-20.
3. **Wersja logarytmiczna** działa poprawnie i zapobiega problemom numerycznym.
4. **Continuous NBC** osiąga wysoką dokładność zbliżoną do dyskretnej wersji z
optymalnymi parametrami.
