using System.Collections.Generic;
using UnityEngine;

namespace MapGen.Utilities
{
    public static class BresenhamLineAlgorithm
    {
        public static List<Vector2Int> DrawLine(Vector2Int a, Vector2Int b)
        {
            return DrawLine(a.x, a.y, b.x, b.y);
        }
        
        private static List<Vector2Int> DrawLine(int x1, int y1, int x2, int y2)
        {
            var result = new List<Vector2Int>();
            
            int dx = Mathf.Abs(x2 - x1);
            int dy = Mathf.Abs(y2 - y1);
            int sx = (x1 < x2) ? 1 : -1;
            int sy = (y1 < y2) ? 1 : -1;

            int error = dx - dy;

            while (true)
            {
                result.Add(new Vector2Int(x1, y1));
                
                if (x1 == x2 && y1 == y2)
                    break;

                int e2 = 2 * error;

                if (e2 > -dy)
                {
                    error -= dy;
                    x1 += sx;
                }

                if (e2 < dx)
                {
                    error += dx;
                    y1 += sy;
                }
            }

            return result;
        }
    }
}