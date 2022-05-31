namespace TiledCS;

/// <summary>
///     Represents a tile layer chunk when the map is infinite
/// </summary>
public class TiledChunk
{
    public int[] Data { get; internal set; }

    public byte[] DataRotationFlags { get; internal set; }

    public int Height { get; internal set; }

    public int Width { get; internal set; }

    public int X { get; internal set; }

    public int Y { get; internal set; }
}
