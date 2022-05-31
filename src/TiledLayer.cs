namespace TiledCS;

/// <summary>
///     Represents a tile layer as well as an object layer within a tile map.
/// </summary>
public class TiledLayer
{
    /// <summary>
    ///     Gets the chunks of data when the map is infinite.
    /// </summary>
    public TiledChunk[] Chunks { get; internal set; }

    /// <summary>
    ///     An int array of gid numbers which define which tile is being used where. The length of the array equals the layer width * the
    ///     layer height. Is null when the layer is not a tilelayer.
    /// </summary>
    public int[] Data { get; internal set; }

    /// <summary>
    ///     A parallel array to data which stores the rotation flags of the tile.
    ///     Bit 3 is horizontal flip,
    ///     bit 2 is vertical flip, and
    ///     bit 1 is (anti) diagonal flip.
    ///     Is null when the layer is not a tilelayer.
    /// </summary>
    public byte[] DataRotationFlags { get; internal set; }

    /// <summary>
    ///     Total vertical tiles
    /// </summary>
    public int Height { get; internal set; }

    /// <summary>
    ///     Gets the layer id.
    /// </summary>
    public int Id { get; internal set; }

    /// <summary>
    ///     Gets the image the layer represents when the layer is an image layer.
    /// </summary>
    public TiledImage Image { get; internal set; }

    /// <summary>
    ///     Is true when the layer is locked
    /// </summary>
    public bool IsLocked { get; internal set; }

    /// <summary>
    ///     Gets the layer name.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     Gets the list of objects in case of an objectgroup layer. Is null when the layer has no objects..
    /// </summary>
    public TiledObject[] Objects { get; internal set; }

    /// <summary>
    ///     Gets the horizontal offset.
    /// </summary>
    public float OffsetX { get; internal set; }

    /// <summary>
    ///     Gets the vertical offset.
    /// </summary>
    public float OffsetY { get; internal set; }

    /// <summary>
    ///     Gets the parallax x position.
    /// </summary>
    public float ParallaxX { get; internal set; }

    /// <summary>
    ///     Gets the parallax y position.
    /// </summary>
    public float ParallaxY { get; internal set; }

    /// <summary>
    ///     Gets the layer properties if set.
    /// </summary>
    public TiledProperty[] Properties { get; internal set; }

    /// <summary>
    ///     Gets the tint color set by the user in hex code.
    /// </summary>
    public string TintColor { get; internal set; }

    /// <summary>
    ///     Gets the layer type..
    /// </summary>
    public TiledLayerType Type { get; internal set; }

    /// <summary>
    ///     Defines if the layer is visible in the editor
    /// </summary>
    public bool IsVisible { get; internal set; }

    /// <summary>
    ///     Total horizontal tiles
    /// </summary>
    public int Width { get; internal set; }
}
