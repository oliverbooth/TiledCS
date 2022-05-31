using System;

namespace TiledCS;

/// <summary>
///     Represents a tile animation. Tile animations are a group of tiles which act as frames for an animation.
/// </summary>
public sealed class TiledTileAnimation
{
    /// <summary>
    ///     Gets the duration of the animation.
    /// </summary>
    /// <value>A <see cref="TimeSpan" /> representing the duration.</value>
    public TimeSpan Duration { get; internal set; }

    /// <summary>
    ///     Gets the tile id within a tileset.
    /// </summary>
    public int TileId { get; internal set; }
}
