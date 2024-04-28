using R3;

namespace Pathfinding
{
    public enum TerrainType
    {
        Obstacle,
        Traversable
    }
    public interface INode
    {
        public ReactiveProperty<TerrainType> type { get; set; }
        public ReactiveProperty<int> gCost { get; set; }
        public ReactiveProperty<int> hCost { get; set; }
        public int fCost { get; }
        public ReactiveProperty<INode> parent { get; set; }
    }
}
