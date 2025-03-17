# N Queens

![Statistics](./assets/nqueens_statistics.png)

## Wnioski

## Obserwacje

1. Wykładniczy wzrost dla BFS:
  - BFS Open list: 1 -> 73 711
  - Lista zamknięta BFS: 16 -> 4,601,179
  - Czas BFS: 2 ms -> 25 897 ms (~26 sekund)

2. Wydajność dla DFS:
  - Lista otwarta DFS: 2 -> 43 (niemal liniowy wzrost z n)
  - Lista zamknięta DFS: waha się, ale pozostaje mała (maksymalnie 262 stany)
  - Czas DFS: stale poniżej 1,2 ms nawet przy n=13

3. Luka w wydajności:
  - Przy n=13, DFS jest około 57 500 razy szybszy niż BFS
  - Różnica ta rośnie wykładniczo wraz z n

## Słabe strony implementacji

1. Zarządzanie pamięcią:
  - Brak limitu pamięci na listach otwartych/zamkniętych
  - W przypadku BFS zużycie pamięci eksploduje przy większych wartościach n.
  - Tworzenie nowych list stanów dla każdego stanu podrzędnego powoduje znaczną presję na GC

2. Nieefektywna reprezentacja stanów:
  - Konwersja stanów na ciągi znaków dla zamkniętej listy jest kosztowna.
  - Pełna reprezentacja tablicy, gdy potrzebne są tylko pozycje królowej
  - Tworzenie nowej listy List<(int,int)> dla każdego stanu powoduje narzut alokacji

3. Brak optymalizacji:
  - Brak odrzucania oczywiście nieprawidłowych ścieżek
  - Brak wykorzystania symetrii planszy (odbicia/obroty)

4. Algorytm sprawdzania konfliktów:
  - Sprawdzanie konfliktów O(n²) dla każdego stanu jest nieefektywne.
  - Można użyć przyrostowej walidacji lub tablic bitowych do szybszego sprawdzania

5. Struktura implementacji:
  - Ogólne podejście BFS/DFS nie wykorzystuje specyficznych właściwości N-Queens.
  - Rozmieszczenie wiersz po wierszu mogłoby być bardziej efektywnie zakodowane

DFS znacznie przewyższa BFS,
ponieważ może szybko znaleźć prawidłową ścieżkę do rozwiązania,
skupiając się na eksploracji wgłąb.
Ponieważ wszystkie rozwiązania znajdują się na głębokości N,
a współczynnik rozgałęzienia zmniejsza się wraz z umieszczaniem większej liczby królowych,
DFS często znajduje rozwiązanie po zbadaniu tylko niewielkiej części przestrzeni poszukiwań.

BFS musi zbadać wszystkie stany na każdym poziomie przed przejściem głębiej,
co jest niezwykle nieefektywne dla tego problemu,
ponieważ zdecydowana większość tych stanów prowadzi do ślepych zaułków.
