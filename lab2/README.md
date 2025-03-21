# N Queens

![Statistics](./assets/nqueens_statistics.png)

## Wnioski

BFS i DFS badają dokładnie tę samą przestrzeń stanów, ale DFS robi to bardziej
efektywnie pamięciowo i czasowo dla problemu N-Hetmanów. Dla obu algorytmów
liczba stanów obsłużonych jest identyczna, ale znacząco różni się maksymalny
rozmiar listy otwartej i wydajność czasowa.

## Obserwacje

1. Wykładniczy wzrost złożoności:
  - Liczba stanów (BFS/DFS-Enqueued i Closed): 341 -> 19,173,961 (dla n=4 do n=8)
  - Czas wykonania: kilka milisekund dla n=4, ~26 sekund dla n=8

2. Różnica w wykorzystaniu pamięci (Max Open List):
  - BFS: 255 -> 16,777,215 (wykładniczy wzrost)
  - DFS: 12 -> 56 (niemal liniowy wzrost z n)

3. Identyczna liczba wygenerowanych i przetworzonych stanów:
  - BFS-Enqueued = DFS-Enqueued dla każdego n
  - BFS-Closed = DFS-Closed dla każdego n
  - Wynika to z deterministycznego generowania stanów potomnych i identycznego zamkniętego zbioru stanów

4. Wydajność czasowa:
  - DFS jest ogólnie szybszy, szczególnie dla większych n
  - Dla n=8: DFS (~18.9s) vs BFS (~25.9s)

## Słabe strony implementacji

1. Zarządzanie pamięcią:
  - Brak limitu pamięci na listach otwartych
  - W przypadku BFS zużycie pamięci eksploduje przy większych wartościach n
  - Tworzenie nowych list stanów dla każdego stanu podrzędnego powoduje znaczną presję na GC

2. Nieefektywna reprezentacja stanów:
  - Konwersja stanów na ciągi znaków dla zamkniętej listy jest kosztowna
  - Pełna reprezentacja tablicy, gdy potrzebne są tylko pozycje królowej
  - Tworzenie nowej listy List<(int,int)> dla każdego stanu powoduje narzut alokacji

3. Brak optymalizacji:
  - Brak wykorzystania symetrii planszy (odbicia/obroty)
  - BFS i DFS generują i odwiedzają tę samą liczbę stanów

4. Algorytm sprawdzania konfliktów:
  - Sprawdzanie konfliktów O(n²) dla każdego stanu jest nieefektywne.
  - Można użyć przyrostowej walidacji lub tablic bitowych do szybszego sprawdzania

5. Struktura implementacji:
  - Ogólne podejście BFS/DFS nie wykorzystuje specyficznych właściwości N-Queens.
  - Rozmieszczenie wiersz po wierszu mogłoby być bardziej efektywnie zakodowane
