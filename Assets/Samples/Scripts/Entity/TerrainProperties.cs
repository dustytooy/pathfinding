namespace Dustytoy.Samples.Grid2D
{
    public struct TerrainProperties
    {
        public enum Type : int
        {
            None,
            Obstacle,
        }
        public Type type;
        public TerrainProperties(Type type)
        {
            this.type = type;
        }
    }
}
