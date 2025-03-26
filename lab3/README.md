# N Queens

![Statistics](./assets/nqueens_statistics.png)

## **Porównanie algorytmów**

### **1. BFS (przeszukiwanie wszerz)**

BFS eksploruje rozwiązanie warstwowo, co powoduje ogromny wzrost wykorzystania
pamięci i liczby odwiedzonych stanów w miarę wzrostu **n**.

- Dla **n = 4**, BFS zapisał **341 stanów** i znalazł rozwiązanie w **2.41 ms**.
- Dla **n = 8**, liczba stanów wzrosła do **19 173 961**,
a czas wykonania do **16 369.58 ms** (ponad 16 sekund).
- Warto zauważyć, że liczba stanów na liście zamkniętej gwałtownie rośnie,
co pokazuje jego nieefektywność dla większych **n**.

**Wniosek**: BFS szybko eksploruje całą przestrzeń stanów,
ale jego wydajność gwałtownie spada przy większych **n** z powodu rosnącego zużycia pamięci.

---

### **2. DFS (przeszukiwanie w głąb)**

DFS eksploruje jedno możliwe rozwiązanie do końca,
co skutkuje mniejszym zapotrzebowaniem na pamięć.

- Dla **n = 4**, DFS zapisał tylko **161 stanów** i znalazł rozwiązanie w **0.30 ms**.
- Dla **n = 8**, liczba stanów wzrosła do **1 485 577**,
a czas wykonania do **1899.15 ms** (~1.9 sekundy).

**Wniosek**: DFS działa znacznie szybciej niż BFS, zużywa mniej pamięci,
ale w niektórych przypadkach może trafić na nieefektywne ścieżki prowadzące do większej liczby iteracji.

---

### **3. Best First Search (przeszukiwanie najlepsze pierwsze)**

Wykorzystuje heurystyki do prowadzenia przeszukiwania.
Trzy testowane heurystyki to:

- **H1** - preferuje układy bardziej centralne
- **H2** - ocenia układy według liczby ataków
- **Hdod** - uwzględnia odległości Manhattanowskie między hetmanami

#### **Porównanie heurystyk**

- **H1** (Heurystyka 1)
  - Dla **n = 4**, przetworzyła **29 stanów** w **1.74 ms**, czyli gorzej niż DFS.
  - Dla **n = 8**, obsłużyła **152 537 stanów** w **414.46 ms** (szybciej niż BFS, ale wolniej niż DFS).
  - Heurystyka ta działa stabilnie, ale nie jest optymalna dla większych **n**.

- **H2** (Heurystyka 2)
  - Znacznie mniej stanów w kolejce niż H1 (**n = 4: 61 stanów, n = 8: 63 113 stanów**).
  - Czas dla **n = 8** to **162.28 ms**, co oznacza **znacznie lepszą efektywność** niż H1.
  - H2 szybko konwerguje na poprawne rozwiązanie.

- **Hdod** (Heurystyka odległościowa)
  - Znacznie większa liczba stanów niż H1 i H2 (**n = 8: 19 132 473 stanów**).
  - Czas dla **n = 8**: **56 626.94 ms (~56 sekund!)**, co czyni ją **najwolniejszą heurystyką**.

**Wnioski**:

- **H2 okazała się najlepszą heurystyką** – znajduje rozwiązanie szybciej niż H1 i Hdod.
- **Hdod jest bardzo kosztowna obliczeniowo**, choć może zapewniać lepsze rozwiązania w przypadku optymalizacji pozycji.
- **Best First Search z dobrą heurystyką (np. H2) jest efektywniejszy niż BFS i DFS**.

---

## **Podsumowanie**

| Algorytm              | Zalety                                   | Wady                                            |
|-----------------------|------------------------------------------|-------------------------------------------------|
| **BFS**               | Znajduje najkrótsze rozwiązanie          | Ogromne zużycie pamięci, wolny dla dużych **n** |
| **DFS**               | Niskie zapotrzebowanie na pamięć         | Może trafić w nieefektywne ścieżki              |
| **Best First (H1)**   | Szybszy niż BFS                          | Wolniejszy od DFS dla dużych **n**              |
| **Best First (H2)**   | Najlepsza wydajność, szybka konwergencja | Może czasami utknąć w lokalnych minimach        |
| **Best First (Hdod)** | Może prowadzić do lepszych układów       | Ekstremalnie kosztowny czasowo                  |

## **Rekomendacja**

- **Dla małych wartości n (4-5):** DFS jest najlepszy.
- **Dla średnich wartości n (6-7):** DFS lub Best First (H2).
- **Dla dużych wartości n (8+):** Best First z heurystyką H2 jest najlepszym wyborem.
- BFS jest generalnie niepraktyczny dla większych **n** z powodu ogromnego zużycia pamięci.
