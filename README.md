# pathfinding
## Description
This project is a demonstration on how A* algorithm work step by step. The algorithm logic is separated from the actual implementation of node use cases. The main implementations that are dependent on use case are specified in INode.cs:
1. GetNeighbors()
2. Traceback()
3. CalculateGCost()
4. CalculateHCost()
Current use case is in a 2D grid setting where each node is a cell with 2D coordinates and a boolean to determine obstacle or not. Future use case can be vertices in a Navmesh.

Additional features include:
1. ObjectPooling (for optimizing memory and GC garbage, namely from openlist, closedlist, pathfinding requests)
2. MinHeap (for optimizing search and sort time in openlist and closedlist)

For this project, the following links were used as a base. They are all good resources.
https://www.geeksforgeeks.org/a-search-algorithm/
https://www.youtube.com/watch?v=-L-WgKMFuhE&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=3

## Framework
UniRx

## Demo URL
https://dustytooy.github.io/pathfinding/
