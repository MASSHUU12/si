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

## 3. Wyniki eksperymentów

Spośród przebadanych ustawień najlepsze (MAE bliskie zeru) uzyskano przy:

![surf_n25_e0.1_ep500000](https://raw.githubusercontent.com/MASSHUU12/si/refs/heads/master/lab7/assets/surf_n25_e0.1_ep500000.png)

> $N = 25, \eta = 0.1, epoki = 500000, MAE = 0.077118$

![surf_n25_e0.5_ep500000](https://raw.githubusercontent.com/MASSHUU12/si/refs/heads/master/lab7/assets/surf_n25_e0.5_ep500000.png)

> $N = 25, \eta = 0.5, epoki = 500000, MAE = 0.058950$

oraz - nieco słabiej, acz wciąż dobrze - przy:
- $N$ = 25, $\eta$ = 0,05, epoki = 500 000
- $N$ = 50, $\eta$ = 0,1 lub 0,5, epoki = 500 000
- $N$ = 100, $\eta$ = 0,1, epoki = 500 000
- $N$ = 150, $\eta$ = 0,1, epoki = 500 000

Widzimy wyraźnie, że dla $N$ = 25-100 oraz bardzo dużej liczby epok ($\geq 5 \cdot 10^5$)
i umiarkowanych-wysokich wartościach $\eta$ sieć jest w stanie niemal doskonale odtworzyć zadaną funkcję.

## 4. Wnioski

1. **Liczba neuronów**
  Już $N = 25$ wystarcza do odwzorowania funkcji. Większe $N$ dawało niewielką poprawę, ale znacznie droższy trening.
2. **Współczynnik uczenia**
   Zbyt niski ($< 0,01$) spowalnia zbieżność, zbyt wysoki ($\geq 0,5$) powoduje niestabilności.
   Optimum w okolicach $0,1$.
3. **Liczba epok**
   Wymagane setki tysięcy epok, aby osiągnąć zadowalający błąd - algorytm online SGD jest powolny.
   Mini-batch (32) poprawia stabilność i eliminuje NaN, ale nadal potrzeba dużej liczby iteracji.
