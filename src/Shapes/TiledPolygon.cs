namespace TiledCS.Shapes;

/// <summary>
///     Represents a polygon.
/// </summary>
public sealed class TiledPolygon : TiledShape
{
    /// <summary>
    ///     Gets the array of vertices where each two elements represent an x and y position. Like 'x,y,x,y,x,y,x,y'..
    /// </summary>
    public float[] Points { get; internal set; }
}
