using MapGen.GridSystem;

namespace MapGen.TunnelSystem
{
    public class Node
    {
        public int ID { get; }
        public NodeState NodeState { get; }
        public GridCell GridCell { get; }
        
        public Node(int id, NodeState nodeState, GridCell gridCell)
        {
            ID = id;
            NodeState = nodeState;
            GridCell = gridCell;
        }
    }
}