namespace TiledCS
{
    /// <summary>
    ///     Represents a layer or object group.
    /// </summary>
    public class TiledGroup
    {
        /// <summary>
        ///     Gets the group's subgroups.
        /// </summary>
        public TiledGroup[] Groups { get; internal set; }

        /// <summary>
        ///     Gets the group's id.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        ///     Gets the group's layers.
        /// </summary>
        public TiledLayer[] Layers { get; internal set; }

        /// <summary>
        ///     Gets the group's locked state.
        /// </summary>
        public bool IsLocked { get; internal set; }

        /// <summary>
        ///     Gets the group's name.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     Gets the group's objects.
        /// </summary>
        public TiledObject[] Objects { get; internal set; }

        /// <summary>
        ///     Gets the group's user properties.
        /// </summary>
        public TiledProperty[] Properties { get; internal set; }

        /// <summary>
        ///     Gets the group's visibility.
        /// </summary>
        public bool IsVisible { get; internal set; }
    }
}
