using System;
using System.Collections.Generic;
using System.Linq;

namespace TiledCS;

/// <summary>
///     Represents a tile within a tileset.
/// </summary>
/// <remarks>These are not defined for all tiles within a tileset, only the ones with properties, terrains and animations.</remarks>
public sealed class TiledTile
{
    private TiledTileAnimation[] _animation = Array.Empty<TiledTileAnimation>();
    private TiledObject[] _objects = Array.Empty<TiledObject>();
    private TiledProperty[] _properties = Array.Empty<TiledProperty>();

    /// <summary>
    ///     Gets a read-only view of the tile animations.
    /// </summary>
    /// <value>A read-only view of the tile animations.</value>
    public IReadOnlyList<TiledTileAnimation> Animation
    {
        get => _animation[..];
        internal set => _animation = value.ToArray();
    }

    /// <summary>
    ///     Gets the tile ID.
    /// </summary>
    /// <value>The tile ID.</value>
    public int Id { get; internal set; }

    /// <summary>
    ///     Gets the individual tile image.
    /// </summary>
    /// <value>The tile image.</value>
    public TiledImage Image { get; internal set; }

    /// <summary>
    ///     Gets a read-only view of the objects.
    /// </summary>
    /// <value>A read-only view of the objects.</value>
    public IReadOnlyCollection<TiledObject> Objects
    {
        get => _objects[..];
        internal set => _objects = value.ToArray();
    }

    /// <summary>
    ///     Gets a read-only view of the properties.
    /// </summary>
    /// <value>A read-only view of the properties.</value>
    public IReadOnlyCollection<TiledProperty> Properties
    {
        get => _properties[..];
        internal set => _properties = value.ToArray();
    }

    /// <summary>
    ///     Gets the terrain definitions as int array. These are indices indicating what part of a terrain and which terrain this tile.
    ///     represents.
    /// </summary>
    /// <remarks>
    ///     In the map file empty space is used to indicate null or no value. However, since it is an int array I needed something
    ///     so I decided to replace empty values with -1.
    /// </remarks>
    public int[] Terrain { get; internal set; }

    /// <summary>
    ///     Gets the custom tile type, set by the user.
    /// </summary>
    public string Type { get; internal set; }
}
