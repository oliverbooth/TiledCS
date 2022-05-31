using System.Drawing;
using System.Globalization;

namespace TiledCS;

internal static class TiledUtilities
{
    public static Color HexToColor(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return Color.Transparent;

        if (hex[0] == '#') hex = hex[1..];

        byte r = byte.Parse(hex[..2], NumberStyles.HexNumber);
        byte g = byte.Parse(hex[2..4], NumberStyles.HexNumber);
        byte b = byte.Parse(hex[4..6], NumberStyles.HexNumber);

        return Color.FromArgb(r, g, b);
    }
}
