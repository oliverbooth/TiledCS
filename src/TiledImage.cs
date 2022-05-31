namespace TiledCS
{
    /// <summary>
    ///     Represents an image.
    /// </summary>
    public class TiledImage
    {
        /// <summary>
        ///     Gets the image height.
        /// </summary>
        public int Height { get; internal set; }

        /// <summary>
        ///     Gets the image source path.
        /// </summary>
        public string Source { get; internal set; }

        /// <summary>
        ///     Gets the image width.
        /// </summary>
        public int Width { get; internal set; }
    }
}