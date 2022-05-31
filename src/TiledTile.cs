namespace TiledCS;

/// <summary>
///     Represents a tile within a tileset.
/// </summary>
/// <remarks>These are not defined for all tiles within a tileset, only the ones with properties, terrains and animations.</remarks>
public class TiledTile
{
    /// <summary>
    ///     An array of tile animations. Is null if none were defined.
    /// </summary>
    public TiledTileAnimation[] Animation { get; internal set; }

    /// <summary>
    ///     Gets the tile id.
    /// </summary>
    public int Id { get; internal set; }

    /// <summary>
    ///     Gets the individual tile image.
    /// </summary>
    public TiledImage Image { get; internal set; }

    /// <summary>
    ///     An array of tile objects created using the tile collision editor
    /// </summary>
    public TiledObject[] Objects { get; internal set; }

    /// <summary>
    ///     An array of properties. Is null if none were defined.
    /// </summary>
    public TiledProperty[] Properties { get; internal set; }

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