namespace MapGen.Map.Brushes.BrushAreas
{
    public interface IIncreasableBrushArea
    {
        public void IncreaseArea(int amount);
        public void DecreaseArea(int amount);
    }
}