# N-Queens Solver - Evolutionary Algorithm

## Wyniki dla N = 50

Czas wykonania: 892.6828 ms
Liczba ataków: 3

Najlepsze rozwiązanie znaleziono po 1000 generacjach.

[Ustawienie na szachownicy](./assets/fitness_board.txt).

![Wykres zmienności wartości funkcji przystosowania](./assets/fitness_evolution.png)
> Wykres zmienności wartości funkcji przystosowania.

## Eksperyment dla rozmiaru turnieju

Dla każdego rozmiaru turnieju testy zostały uruchomione 100 razy dla:

- n = 50
- Rozmiar populacji = 1000
- Maksymalna liczba generacji = 1000
- Prawdopodobieństwo krzyżowania = 0.9
- Prawdopodobieństwo mutacji = 0.5

- Rozmiar 5:
  - Średnia wartość evaluate: 3.65
  - Odchylenie standardowe: 0.89
- Rozmiar 15:
  - Średnia wartość evaluate: 3.76
  - Odchylenie standardowe: 1.00
- Rozmiar 50:
  - Średnia wartość evaluate: 3.67
  - Odchylenie standardowe: 1.02
- Rozmiar 100:
  - Średnia wartość evaluate: 3.63
  - Odchylenie standardowe: 0.92

### Wnioski

Wyniki wskazują, że w obrębie testowanych rozmiarów turnieju (5, 15, 50, 100)
średnie wartości evaluate oraz ich odchylenia standardowe są dość zbliżone.

1. Drobne różnice średnich wartości evaluate (od 3.63 do 3.76) sugerują,
że w tej konfiguracji zmiana rozmiaru turnieju nie wpływa znacząco na jakość uzyskiwanych rozwiązań.

2. Najniższa średnia (3.63) uzyskana dla turnieju o rozmiarze 100 może sugerować,
że większy rozmiar turnieju – czyli wyższa presja selekcyjna – potencjalnie
pozwala na nieco lepszą selekcję osobników, jednak różnice są znikome.

3. Odchylenia standardowe (od 0.89 do 1.02) są porównywalne, co oznacza,
że zmienność wyników eksperymentów nie ulega znaczącej zmianie przy modyfikacji rozmiaru turnieju.

Wyniki wskazują na pewną stabilność algorytmu względem tego parametru – zmiana
rozmiaru turnieju w badanym zakresie nie wpływa drastycznie na końcową wartość funkcji przystosowania.
Można zatem wnioskować, że przy tych ustawieniach algorytmu rozmiar turnieju
nie jest krytycznym parametrem dla jakości rozwiązania.
