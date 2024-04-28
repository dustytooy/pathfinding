using UnityEngine;

namespace Pathfinding.Grid
{
    internal class CostCalculator : ICostCalculator
    {
        public int G(Pathfinding.INode current, Pathfinding.INode parent)
        {
            CellModel cur = current as CellModel;
            CellModel par = parent as CellModel;

            return parent.gCost.CurrentValue + Mathf.FloorToInt((par.position - cur.position).magnitude * 10f);
        }

        public int H(Pathfinding.INode current, Pathfinding.INode goal)
        {
            CellModel cur = current as CellModel;
            CellModel tar = goal as CellModel;

            float dx = Mathf.Abs(cur.position.x - tar.position.x);
            float dy = Mathf.Abs(cur.position.y - tar.position.y);

            return Mathf.FloorToInt(CellModel.size * (dx + dy + (Mathf.Sqrt(2f) - 2) * Mathf.Min(dx, dy)) * 10f);

        }
    }
}
