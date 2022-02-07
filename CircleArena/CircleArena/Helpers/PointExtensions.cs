using System;
using System.Windows;

namespace CircleArena.Helpers
{
    public static class PointExtensions
    {
        public static Point GetRandomPointInRect(Rect rect)
        {
            var random = new Random(DateTime.Now.Millisecond);

            // Tech debt: Possible int overflow. Should be fine as screen is unlikely to extend past int.max
            var x = random.Next(0, (int)rect.Width);
            var y = random.Next(0, (int)rect.Height);

            return new Point(x, y);
        }
    }
}
