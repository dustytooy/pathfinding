using R3;
using UnityEngine;

namespace Pathfinding.Grid
{
    public class CellModel : ICell
    {
        public static readonly float size = 1f;
        public Vector2Int position { get; set; }

        public ReactiveProperty<TerrainType> type { get; set; }
        public ReactiveProperty<int> gCost { get; set; }
        public ReactiveProperty<int> hCost { get; set; }
        public int fCost => gCost.CurrentValue + hCost.CurrentValue;
        public ReactiveProperty<INode> parent { get ; set; }

        public CellModel(Vector2Int position)
        {
            this.position = position;
            type = new ReactiveProperty<TerrainType>(TerrainType.Traversable);
            gCost = new ReactiveProperty<int>(0);
            hCost = new ReactiveProperty<int>(0);
            parent = new ReactiveProperty<INode>(null);
        }
    }
}
