## 1. Wstęp

Celem ćwiczenia było zaimplementowanie od podstaw jednokryterialnego perceptronu wielowarstwowego (MLP)
z jedną warstwą ukrytą oraz algorytmem wstecznej propagacji błędów,
a następnie przeprowadzenie szeregu eksperymentów zmieniających podstawowe hiperparametry sieci:
liczbę neuronów w warstwie ukrytej $N$, współczynnik uczenia $\eta$ oraz liczbę epok treningowych.

Funkcję aproksymowaną zadano wzorem:

$$
y^* = f(x_1, x_2) = \cos(x_1x_2)\cos(2x_1), (x_1,x_2) \in [0,\pi]^2,
$$

na bazie której wygenerowano zbiór treningowy (1000 próbek) i testowy (10 000 próbek).

## 2. Metodyka

1. Generowanie danych
   - Skrypt w Pythonie tworzy losowe pary $(x_1, x_2) \sim U([0,\pi]^2)$ i oblicza odpowiadające wartości $y^*$.
   - Dane zapisywane są w formacie CSV: kolumny `x1,x2,y`.
2. Architektura MLP
   - Jedna warstwa ukryta z $N$ neuronami, funkcja aktywacji sigmoidalna.
   - Warstwa wyjściowa to pojedynczy neuron z funkcją liniową.
   - Sieć przyjmuje dodatkowe biasy: $v_{k0}$ dla warstwy ukrytej i $w_o$ dla wyjścia.
   - Metody:
     - `_forward` - oblicza wyjścia ukryte $\phi(s_k)$ i wynik sieci $y$ według wzorów.
     - `fit` - realizuje stochastyczny spadek gradientowy według reguł aktualizacji wag iteracyjnie przez określoną liczbę epok.
       
$$
v_{kj} := v_{kj} - \eta(y - y^*)w_k\phi(s_k)(1-\phi(s_k))x_j, w_k := w_k - \eta(y - y^\*)\phi(s_k) 
$$

3. Eksperymenty

- Zamodelowano pełen "grid search" po hiperparametrach:
  - $N \in \{25, 50, 100, 150\}$
  - $\eta \in \{0,05, 0,1, 0,5\}$
  - epochs $\in \{50000, 100000, 500000\}$
  - batch size = 32
- Dla każdej konfiguracji:
  - trening (`--mode train`),
  - test i obliczenie średniego błędu bezwzględnego (MAE),
  - wizualizacja powierzchni sieci (surface plot) oraz punktów uczących (scatter plot) w Pythonie.

## 3. Wyniki eksperymentów z porównaniem do sklearn

Spośród przebadanych ustawień najlepsze (MAE bliskie zeru) uzyskano przy:

![scatter_n50_e0.05_ep500000](https://raw.githubusercontent.com/MASSHUU12/si/refs/heads/master/lab7/assets/scatter_n50_e0.05_ep500000.png)

| Konfiguracja                           | Model   | MAE      | RMSE     |
| -------------------------------------- | ------- | -------- | -------- |
| $N = 50, \eta = 0.05, epoki = 500000 $ | sklearn | 0.438879 | 0.539368 |
|                                        | .NET    | 0.061969 | 0.083408 |

![scatter_n100_e0.1_ep500000](https://raw.githubusercontent.com/MASSHUU12/si/refs/heads/master/lab7/assets/scatter_n100_e0.1_ep500000.png)

| Konfiguracja                           | Model   | MAE      | RMSE     |
| -------------------------------------- | ------- | -------- | -------- |
| $N = 100, \eta = 0.1, epoki = 500000 $ | sklearn | 0.428952 | 0.526184 |
|                                        | .NET    | 0.080281 | 0.133268 |

### Podsumowanie wszystkich konfiguracji

We wszystkich testowanych konfiguracjach ręcznie napisany MLP osiągał istotnie niższy błąd niż `MLPRegressor`, co sugeruje:
- **Lepsza kalibracja** w implementacji.
- **Optymalizacja spadku gradientu** i inicjalizacji wag w sieci jest dla tego zadania korzystniejsza.

## 4. Wnioski

1. **Przewaga własnej implementacji**
   We wszystkich przypadkach własny MLP osiągał istotnie mniejsze MAE niż `sklearn` z tymi samymi parametrami.
2. **Hiperparametry**
   - **N = 25-50, $\eta$ = 0.1-0.5, epoki $\geq 5 \cdot 10^5$** to sweet spot - minimalny błąd i rezonowne czasy trenowania.
   - Dla `sklearn` nawet przy tych ustawieniach błąd pozostaje wysoki, co może wynikać z interpetacji parametrów
     (np. `max_iter` w sklearn to maksymalna liczba epok przed sprawdzeniem warunku stopu, a nie gwarantowane 500 000 update'ów).
3. **Implementacja vs biblioteka**
   - Własna implementacja umożliwia pełną kontrolę nad algorytmem (również nad kolejnością operacji, dokładnością numeryczną itp.).
   - `MLPRegressor` ułatwia szybkie prototypowanie, ale ukrywa część detali (czasem zaskakujące ustawienia domyślne, timeouty, sprawdzanie stopu).
4. **Liczba neuronów**
  Już $N = 25$ wystarcza do odwzorowania funkcji. Większe $N$ dawało niewielką poprawę, ale znacznie droższy trening.
5. **Współczynnik uczenia**
   Zbyt niski ($< 0,01$) spowalnia zbieżność, zbyt wysoki ($\geq 0,5$) powoduje niestabilności.
   Optimum w okolicach $0,1$.
6. **Liczba epok**
   Wymagane setki tysięcy epok, aby osiągnąć zadowalający błąd - algorytm online SGD jest powolny.
   Mini-batch (32) poprawia stabilność i eliminuje NaN, ale nadal potrzeba dużej liczby iteracji.
