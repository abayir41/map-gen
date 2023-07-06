using MapGen.Placables;

namespace MapGen.Map
{
    public struct PlacableData
    {
        public Placable Placable { get; }
        public int Rotation { get; }
        
        public PlacableData(Placable placable, int rotation)
        {
            Placable = placable;
            Rotation = rotation;
        }
    }
}