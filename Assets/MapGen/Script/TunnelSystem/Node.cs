using MapGen.GridSystem;

namespace MapGen.TunnelSystem
{
    public class Node
    {
        public int ID { get; }
        public NodeState NodeState { get; }
        public GridElement GridElement { get; }
        
        public Node(int id, NodeState nodeState, GridElement gridElement)
        {
            ID = id;
            NodeState = nodeState;
            GridElement = gridElement;
        }
    }
}