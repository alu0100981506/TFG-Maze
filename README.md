# Deep Learning en Videojuegos: Resolución de Laberintos
## Trabajo de Fin de Grado 2019-2020
### Autor: Pablo Martin Gonzalez

### Abstract

Pathfinding algorithms are indispensable today, these algorithms are used by applications such as Google Maps, GPS systems, or even, in the routing of packets through the internet. In the field of medicine, they are also used in operations, to find the shortest and safest path considering all factors, including blood flows and obstacles, to get where the surgeon wants to go. These algorithms are deterministic and always take the same number of iterations to find the optimal path. The objective of this work is to study the possibility of creating through Deep Learning a model capable of competing against current “pathfinding” algorithms. To do this, the Unity development environment, and the ML-Agents machine learning toolkit, published by the Unity development team, are used.

#### Keywords: Laberynth, Deep Learning, Ml-Agents, Pathfinding




### GIFS del comportamiento

#### PPO

Aqui se puede apreciar el comportamiento de un modelo entrenado sesetan y cinco horas bajo una red neuronal PPO.

![](Gifs/PPO.gif)


#### LSTM

Por el otro lado aqui se puede ver como un moidelo entrenado treinta horas en una red neuronal LSTM es capaz de resolver laberintos de quince por quince.

![](Gifs/ResolucionDeLaberintosComplejos.gif)

Es apreciable que los modelos se repiten, intentado acudir por el mismo camino varias veces, hasta que por fin cambian el comportamiento y alcanzan la salida

![](Gifs/ComportamientoRepititivo.gif)

Y que en escenarios pequeños son capaces de resolver el problema de manera rapida

![](Gifs/RapidezLaberintosPequeños.gif)
