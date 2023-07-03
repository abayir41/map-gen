using MapGen.Placables;

namespace MapGen.Map
{
    public class PlacableData
    {
        public Placable Placable { get; }
        public int Rotation { get; }
        public float[,] Noise { get; }
        
        public PlacableData(Placable placable, int rotation, float[,] noise)
        {
            Placable = placable;
            Rotation = rotation;
            Noise = noise;
        }
    }
}