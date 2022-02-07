using System;
using System.Windows.Media;

namespace CircleArena.Helpers
{
    public static class ColorExtensions
    {
        public static Color GetRandomColor()
        {
            // Kind of terrible way to seed a random number
            var random = new Random(DateTime.Now.Millisecond);

            var bytes = new byte[3];
            random.NextBytes(bytes);

            return Color.FromArgb(byte.MaxValue, bytes[0], bytes[1], bytes[2]);
        }

        public static Color GetTransluecientColorFromColor(Color c)
        {
            return Color.FromArgb(195, c.R, c.G, c.B);
        }
    }
}
