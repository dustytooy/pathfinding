namespace Pathfinding.Grid
{
    public interface ICellView
    {
        public void UpdateGCost(int value);
        public void UpdateHCost(int value);
        public void UpdateType(Pathfinding.TerrainType type);
    }
}
