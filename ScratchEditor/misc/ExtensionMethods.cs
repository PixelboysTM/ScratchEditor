using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ScratchEditor.misc.math;

namespace ScratchEditor.misc
{
    public static class ExtensionMethods
    {
        public static bool OverlapRect(this Rect r1, Rect r2)
        {
            return Collisions.RectRect(r1, r2);
        }
        
        public static string RemoveLastPart(this string raw, char trim, bool endsWith = true)
        {
            var s = raw.Split(trim);
            var n = "";
            for (int i = 0; i < s.Length - (endsWith ? 2 : 1); i++)
            {
                n += s[i];
            }

            if (n.Length > 0)
            {
                n += trim;
            }
            raw = n;
            return n;
        }

        public static int InRange(this int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }

            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        public static double InRange(this double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }

            else if (value > max)
            {
                value = max;
            }

            return value;
        }
    }
}