namespace TiledCS
{
    /// <summary>
    ///     Represents an element within the Tilesets array of a TiledMap object.
    /// </summary>
    public class TiledMapTileset
    {
        /// <summary>
        ///     Gets the first GID, that is, the GID which matches the tile with source vector 0,0.
        /// </summary>
        /// <remarks>This value is used to determine which tileset belongs to which GID.</remarks>
        public int FirstGid { get; internal set; }

        /// <summary>
        ///     Gets the tsx file path as defined in the map file itself.
        /// </summary>
        public string Source { get; internal set; }
    }

    /// <summary>
    ///     Represents a property object in both tilesets, maps, layers and objects. Values are all in string but you can use the 'type'
    ///     property for conversions.
    /// </summary>
    public class TiledProperty
    {
        /// <summary>
        ///     Gets the property name or key in string format.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     Gets the property type as used in Tiled. Can be bool, number, string, ....
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        ///     Gets the value in string format.
        /// </summary>
        public string Value { get; internal set; }
    }

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
        ///     Gets the object's height in pixels.
        /// </summary>
        public float Height { get; internal set; }

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
        ///     Gets the object's width in pixels.
        /// </summary>
        public float Width { get; internal set; }

        /// <summary>
        ///     Gets the object's x position in pixels.
        /// </summary>
        public float X { get; internal set; }

        /// <summary>
        ///     Gets the object's y position in pixels.
        /// </summary>
        public float Y { get; internal set; }
    }

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

    /// <summary>
    ///     Represents a point shape.
    /// </summary>
    public class TiledPoint
    {
    }

    /// <summary>
    ///     Represents an ellipse shape.
    /// </summary>
    public class TiledEllipse
    {
    }

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

    /// <summary>
    ///     Represents a tile animation. Tile animations are a group of tiles which act as frames for an animation.
    /// </summary>
    public class TiledTileAnimation
    {
        /// <summary>
        ///     Gets the duration in miliseconds.
        /// </summary>
        public int Duration { get; internal set; }

        /// <summary>
        ///     Gets the tile id within a tileset.
        /// </summary>
        public int TileId { get; internal set; }
    }

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

    /// <summary>
    ///     Represents a tile layer chunk when the map is infinite
    /// </summary>
    public class TiledChunk
    {
        public int[] Data { get; internal set; }

        public byte[] DataRotationFlags { get; internal set; }

        public int Height { get; internal set; }

        public int Width { get; internal set; }

        public int X { get; internal set; }

        public int Y { get; internal set; }
    }
}
