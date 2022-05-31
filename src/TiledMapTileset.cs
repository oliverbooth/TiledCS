namespace TiledCS;

/// <summary>
///     Represents an element within the Tilesets array of a TiledMap object.
/// </summary>
public sealed class TiledMapTileset
{
    /// <summary>
    ///     Gets the first GID, that is, the GID which matches the tile with source vector 0,0.
    /// </summary>
    /// <remarks>This value is used to determine which tileset belongs to which GID.</remarks>
    public int FirstGid { get; internal set; }

    /// <summary>
    ///     Gets the tsx file path as defined in the map file itself.
    /// </summary>
    public string Source { get; internal set; }
}
