using System.Collections.Generic;

namespace BoardGame
{
    /// <summary>
    /// Implement common function for Board game
    /// </summary>
    public static class BoardGameCommon
    {
        public static List<Basic.Vec2Int> Neibors(Basic.Vec2Int pos, Basic.Vec2Int size)
        {
            var acc = new List<Basic.Vec2Int>(8);
            var yMin = System.Math.Max(pos.y - 1, 0);
            var yMax = System.Math.Min(pos.y + 1, size.y - 1);
            var xMin = System.Math.Max(pos.x - 1, 0);
            var xMax = System.Math.Min(pos.x + 1, size.x - 1);

            for (int y = yMin; y <= yMax; y++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    if (x == pos.x && y == pos.y)
                    {
                        continue;
                    }
                    else
                    {
                        acc.Add(new Basic.Vec2Int(x, y));
                    }
                }
            }

            return acc;
        }
    }
}
