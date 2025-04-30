"""
Reguła delta wg instrukcji SI lab 6
"""
import numpy as np
import matplotlib.pyplot as plt
import random

def make_samples(samples: int, dimension: int):
    """
    Generuj próbki
    :param samples: liczba próbek
    :param dimension: wymiar problemu
    :return: X - wejścia, y - etykiety
    """
    X = np.random.uniform(-20, 20, [samples,dimension])
    # print("Pierwsza próbka:", X[0])
    random_coef = np.random.uniform(-1,1,dimension+1)
    print("Losowe współczynniki prostej referencyjnej:", random_coef)
    y = np.ones(samples)
    sum = np.sum(X * random_coef[:-1], axis=1) + random_coef[-1]
    y[sum < 0] = -1
    return X, y


def visualize(inputs, output, d: int, coef=None):
    """
    W
    :param output: wektor etykiet (-1, 1)
    :param inputs: macierz wejść (samples, d)
    :param d: wymiarowość wejścia
    :param coef: wagi
    """
    if d == 2:
        plt.scatter(inputs[:, 0], inputs[:, 1], c=output)
        plt.xlabel('cecha 1')
        plt.ylabel('cecha 2')
        if (coef is not None):
            # wzór na granicę decyzyjną
            # w0 + x1*w1 + x2*w2 = 0
            # x2 = -(x1*w1 + w0)/w2
            x = np.linspace(-20, 20, 100)
            y = -(x * coef[0] + coef[2]) / coef[1]
            plt.plot(x, y, '-r')
        plt.show()
    elif d == 3:
        fig = plt.figure()
        ax = fig.add_subplot(111, projection='3d')
        ax.scatter(inputs[:, 0], inputs[:, 1], inputs[:, 2], c=output)
        ax.set_xlabel('cecha 1')
        ax.set_ylabel('cecha 2')
        ax.set_zlabel('cecha 3')
        if (coef is not None):
            # wzrór na płaszczyznę decyzyjną
            # w0 + x1*w1 + x2*w2 + x3w3 = 0
            # x3 = -(x1*w1 + x2*w2 + w0)/w3
            x = np.linspace(-25, 25, 100)
            y = np.linspace(-25, 25, 100)
            X, Y = np.meshgrid(x, y)
            Z = -(X * coef[0] + Y * coef[1] + coef[3]) / coef[2]
            ax.plot_surface(X, Y, Z, alpha=0.8)
        plt.show()
    else:
        print("Nie można zwizualizować danych o więcej niż 3D")


def neuron_output(x, w):
    """
    Oblicza wyjście neuronu
    :param x: wejście
    :param w: wagi neuronu
    :return: neuron wyjście
    """
    # Extend inputs by additional columns with 1s
    # Dodać 1 na końcu wektora x


    # Policzyć iloczyn skalarny (np.dot)

    # Zwrócić wynik funkcji aktywacji a jeżeli xw >= 0 to 1, w przeciwnym wypadku -1
    xe = np.hstack([x, np.ones((x.shape[0], 1))])
    return np.where(np.dot(xe, w) >= 0, 1, -1)


def perceptron_training(x_, y_, eta=0.01):
    """
    Uczenie perceptronu
    :param X_: próbki uczące
    :param y_: klasy
    :param eta: współczynnik uczenia
    :return: wagi/parametry modelu
    """
    # Pseudokod l.2: Wagi początkowe
    w = list()
    d = x_.shape[1]
    print("Wymiarowość problemu", d)
    w.append(np.zeros(d + 1))
    # Pseudokod l.3 k - liczba iteracji
    k = 0
    # E to zbior próbek źle sklasyfikowanych
    f_x = neuron_output(x_, w[k])
    # print("f_x", f_x)
    # print("y_", y_)
    E = np.where(y_ != f_x)[0]
    print("E - próbki niepoprawnie identyfikowane", E)
    print("Wagi początkowe", w[k])
    # Pseudokod l.4 Dopóki E nie jest puste
    while len(E) > 0:
        # Pseudokod l.5: Wybierz losowo próbkę ze zbioru E
        i = random.choice(E)
        new_x_i = np.append(x_[i], 1) # dodać 1 na końcu wektora
        # Pseudokod l.6: Zaktualizuj wagi
        w.append(w[k] + eta * y_[i] * new_x_i) # w[k+1] = w[k] + eta * y_i * x_i
        # Psedudokod l.7 Zaktualizuj k
        k += 1
        # Zaktualizuj E
        E = np.where(y_ != neuron_output(x_, w[k]))[0]
        # print("E - próbki niepoprawnie identyfikowane", E)
    return w

# Ustaw Parametry
dimension = 3
samples = 10000

X, y = make_samples(samples, dimension)
visualize(X, y ,dimension)

# 2. Perceptron learning algorithm
weights = perceptron_training(X, y)
print("Wagi: ", weights[-1])
print("Zgodność z danymi uczącymi: ", np.sum(neuron_output(X, weights[-1]) == y))
print("Epoki/iteracje: ", len(weights) - 1)

# 3. Wizualizacja granicy decyzyjnej
visualize(X, y, dimension, weights[-1])
