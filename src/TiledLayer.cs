using System.Drawing;

namespace TiledCS;

/// <summary>
///     Represents a tile layer as well as an object layer within a tile map.
/// </summary>
public sealed class TiledLayer
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
    ///     Gets the layer's offset.
    /// </summary>
    /// <value>The offset.</value>
    public PointF Offset { get; internal set; }

    /// <summary>
    ///     Gets the layer's parallax position.
    /// </summary>
    /// <value>The parallax position.</value>
    public PointF Parallax { get; internal set; }

    /// <summary>
    ///     Gets the layer properties if set.
    /// </summary>
    public TiledProperty[] Properties { get; internal set; }

    /// <summary>
    ///     Gets the layer's tint color.
    /// </summary>
    /// <value>The tint color, or <see langword="null" /> if this value is not set.</value>
    public Color? TintColor { get; internal set; }

    /// <summary>
    ///     Gets the layer type..
    /// </summary>
    public TiledLayerType Type { get; internal set; }

    /// <summary>
    ///     Defines if the layer is visible in the editor
    /// </summary>
    public bool IsVisible { get; internal set; }

    /// <summary>
    ///     Gets the size of the layer.
    /// </summary>
    /// <value>The size of the layer, measured in tiles.</value>
    public Size Size { get; internal set; }
}
