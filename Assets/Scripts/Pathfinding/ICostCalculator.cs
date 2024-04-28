namespace Pathfinding
{
    public interface ICostCalculator
    {
        public int G(INode current, INode start);
        public int H(INode current, INode end);
    }
}
