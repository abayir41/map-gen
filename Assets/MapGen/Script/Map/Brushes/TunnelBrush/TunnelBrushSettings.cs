using MapGen.TunnelSystem;
using UnityEngine;

namespace MapGen.Map.Brushes.TunnelBrush
{
    [CreateAssetMenu(fileName = "Tunnel Brush Settings", menuName = "MapGen/Brushes/Tunnel/Brush Settings", order = 0)]
    public class TunnelBrushSettings : ScriptableObject
    {
        [Header("Tunnel")] 
        [SerializeField] private float tunnelMinLength;
        [SerializeField] private float tunnelAverageMinHeight;
        [SerializeField] private float betweenTunnelMinSpace;
        [SerializeField] private TunnelPlacable _tunnelBrush;

        public TunnelPlacable TunnelBrush => _tunnelBrush;
        public float TunnelMinLength => tunnelMinLength;
        public float TunnelAverageMinHeight => tunnelAverageMinHeight;
        public float BetweenTunnelMinSpace => betweenTunnelMinSpace;
        
    }
}