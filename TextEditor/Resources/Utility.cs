

namespace TextEditor.Resources
{
    public class Utility
    {
        private static string ToString(params object[] objs)
        {
            string s = string.Empty;

            foreach (object obj in objs)
            {
                s += obj.ToString();
            }

            return s;
        }

        public static string ToIEC(ulong size)
        {
            double s = size;
            double k = 1024.0;
            double m = k * k;
            double g = m * k;
            double t = g * k;

            if (s < k) return ToString(s, " B");
            if (s < m) return ToString(s / k, " KiB");
            if (s < g) return ToString(s / m, " MiB");
            if (s < t) return ToString(s / g, " GiB");
            return ToString(s / t, " TiB");
        }

        public static string ToSI(ulong size)
        {
            double s = size;

            if (s < 1000.0) return ToString(s, " B");
            if (s < 1e6) return ToString(s / 1e3, " kB");
            if (s < 1e9) return ToString(s / 1e6, " MB");
            if (s < 1e12) return ToString(s / 1e9, " GB");
            return ToString(s / 1e12, " TB");
        }
    }
}
