namespace TiledCS
{
    /// <summary>
    ///     Used as data type for the GetSourceRect method. Represents basically a rectangle.
    /// </summary>
    public class TiledSourceRect
    {
        /// <summary>
        ///     Gets the height in pixels from the tile in the source image.
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        ///     Gets the width in pixels from the tile in the source image.
        /// </summary>
        public int Width { get; internal set; }

        /// <summary>
        ///     Gets the x position in pixels from the tile location in the source image.
        /// </summary>
        public int X { get; internal set; }

        /// <summary>
        ///     Gets the y position in pixels from the tile location in the source image.
        /// </summary>
        public int Y { get; internal set; }
    }
}
