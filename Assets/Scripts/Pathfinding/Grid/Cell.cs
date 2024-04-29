using UnityEngine;

namespace Pathfinding.Grid
{
    public class Cell : INode
    {
        public static readonly float size = 1f;
        public Vector2Int position { get; set; }

        public int gCost { get; set; }
        public int hCost { get; set; }
        public int fCost => gCost + hCost;
        public INode parent { get ; set; }
        public bool isObstacle { get ; set ; }

        public Cell(Vector2Int position, bool isObstacle)
        {
            this.position = position;
            this.isObstacle = isObstacle;
            gCost = int.MaxValue;
            hCost = 0;
            parent = null;
        }

        public bool Equals(INode other)
        {
            if(other == null) { return false; }
            if(other is Cell cell)
            {
                return position == cell.position;
            }
            return false;
        }

        public int CompareTo(INode other)
        {
            if (other == null) return 1;
            if(other is Cell)
            {
                if(fCost == other.fCost)
                {
                    return hCost.CompareTo(other.hCost);
                }
                else
                {
                    return fCost.CompareTo(other.fCost);
                }
            }
            return 1;
        }
    }
}
