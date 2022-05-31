namespace TiledCS
{
    /// <summary>
    ///     Represents a polygon shape.
    /// </summary>
    public class TiledPolygon
    {
        /// <summary>
        ///     Gets the array of vertices where each two elements represent an x and y position. Like 'x,y,x,y,x,y,x,y'..
        /// </summary>
        public float[] Points { get; internal set; }
    }
}
