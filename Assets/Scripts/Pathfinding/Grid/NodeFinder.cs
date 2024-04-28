using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Grid
{
    internal class NodeFinder : INodeFinder
    {
        private Grid _grid;
        public NodeFinder(Grid grid)
        {
            _grid = grid;
        }

        public Pathfinding.INode GetNode(Vector2 position)
        {
            int x = (int)(position.x / CellModel.size);
            int y = (int)(position.y / CellModel.size);
            return _grid.GetCell(x, y);
        }

        public Vector2 GetPosition(Pathfinding.INode node)
        {
            CellModel cell = node as CellModel;
            return new Vector2(cell.position.x + 0.5f, cell.position.y + 0.5f) * CellModel.size;
        }

        public List<Pathfinding.INode> GetNeighbors(Pathfinding.INode node)
        {
            var neighbors = new List<Pathfinding.INode>();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    CellModel cell = node as CellModel;
                    int newX = cell.position.x + i;
                    int newY = cell.position.y + j;

                    // If this successor is a valid cell
                    if (_grid.ValidatePosition(newX, newY))
                    {
                        neighbors.Add(cell);
                    }
                }
            }

            return neighbors;
        }
    }
}
