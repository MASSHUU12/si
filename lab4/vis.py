import pydot

graphs = pydot.graph_from_dot_file("./assets/gametree.dot")
graph = graphs[0]
graph.write_png("./assets/gametree.png")
