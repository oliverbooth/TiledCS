namespace TiledCS;

/// <summary>
///     Represents a tile animation. Tile animations are a group of tiles which act as frames for an animation.
/// </summary>
public class TiledTileAnimation
{
    /// <summary>
    ///     Gets the duration in miliseconds.
    /// </summary>
    public int Duration { get; internal set; }

    /// <summary>
    ///     Gets the tile id within a tileset.
    /// </summary>
    public int TileId { get; internal set; }
}
