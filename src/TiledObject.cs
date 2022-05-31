using System.Drawing;
using TiledCS.Shapes;

namespace TiledCS;

/// <summary>
///     Represents an tiled object defined in object layers and tiles.
/// </summary>
public class TiledObject
{
    /// <summary>
    ///     If an object was set to an ellipse shape, this property will be set
    /// </summary>
    public TiledEllipse Ellipse { get; internal set; }

    /// <summary>
    ///     Gets the tileset gid when the object is linked to a tile.
    /// </summary>
    public int Gid { get; internal set; }

    /// <summary>
    ///     Gets the object id.
    /// </summary>
    public int Id { get; internal set; }

    /// <summary>
    ///     Gets the object's name.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    ///     If an object was set to a point shape, this property will be set
    /// </summary>
    public TiledPoint Point { get; internal set; }

    /// <summary>
    ///     If an object was set to a polygon shape, this property will be set and can be used to access the polygon's data
    /// </summary>
    public TiledPolygon Polygon { get; internal set; }

    /// <summary>
    ///     An array of properties. Is null if none were defined.
    /// </summary>
    public TiledProperty[] Properties { get; internal set; }

    /// <summary>
    ///     Gets the object's rotation.
    /// </summary>
    public float Rotation { get; internal set; }

    /// <summary>
    ///     Gets the object type if defined. Null if none was set..
    /// </summary>
    public string Type { get; internal set; }
    
    /// <summary>
    ///     Gets the object's size.
    /// </summary>
    /// <value>The size, measured in pixels.</value>
    public SizeF Size { get; internal set; }
    
    /// <summary>
    ///     Gets the object's position.
    /// </summary>
    /// <value>The position, measured in pixels.</value>
    public PointF Position { get; internal set; }
}
