using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using TiledCS.Shapes;

namespace TiledCS;

/// <summary>
///     Represents a Tiled map.
/// </summary>
public class TiledMap
{
    private const uint FlippedHorizontallyFlag = 0b10000000000000000000000000000000;
    private const uint FlippedVerticallyFlag = 0b01000000000000000000000000000000;
    private const uint FlippedDiagonallyFlag = 0b00100000000000000000000000000000;

    /// <summary>
    ///     How many times we shift the FLIPPED flags to the right in order to store it in a byte.
    ///     For example: 0b10100000000000000000000000000000 >> SHIFT_FLIP_FLAG_TO_BYTE = 0b00000101
    /// </summary>
    private const int ShiftFlipFlagToByte = 29;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TiledMap" /> class.
    /// </summary>
    public TiledMap()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TiledMap" /> class by loading the map from the specified file.
    /// </summary>
    /// <param name="path">The path to the TMX file.</param>
    /// <exception cref="TiledException">Thrown when the map could not be loaded or is not in a correct format</exception>
    public TiledMap(string path)
    {
        // Check the file
        if (!File.Exists(path)) throw new TiledException($"{path} not found");

        string content = File.ReadAllText(path);

        if (path.EndsWith(".tmx"))
            ParseXml(content);
        else
            throw new TiledException("Unsupported file format");
    }

    /// <summary>
    ///     Returns the Tiled version used to create this map
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    ///     Returns an array of properties defined in the map
    /// </summary>
    public TiledProperty[] Properties { get; set; }

    /// <summary>
    ///     Returns an array of tileset definitions in the map
    /// </summary>
    public TiledMapTileset[] Tilesets { get; set; }

    /// <summary>
    ///     Returns an array of layers or null if none were defined
    /// </summary>
    public TiledLayer[] Layers { get; set; }

    /// <summary>
    ///     Returns an array of groups or null if none were defined
    /// </summary>
    public TiledGroup[] Groups { get; set; }

    /// <summary>
    ///     Returns the defined map orientation as a string
    /// </summary>
    public string Orientation { get; set; }

    /// <summary>
    ///     Returns the render order as a string
    /// </summary>
    public string RenderOrder { get; set; }

    /// <summary>
    ///     Gets the size of the map.
    /// </summary>
    /// <value>The size, measured in tiles.</value>
    public Size Size { get; internal set; }

    /// <summary>
    ///     Gets the size of a tile in the map.
    /// </summary>
    /// <value>The tile size, measured in pixels.</value>
    public Size TileSize { get; internal set; }

    /// <summary>
    ///     Gets the parallax origin.
    /// </summary>
    /// <value>The parallax origin.</value>
    public PointF ParallaxOrigin { get; internal set; }

    /// <summary>
    ///     Returns true if the map is configured as infinite
    /// </summary>
    public bool Infinite { get; set; }

    /// <summary>
    ///     Returns the defined map background color as a hex string
    /// </summary>
    public Color? BackgroundColor { get; set; }

    /// <summary>
    ///     Can be used to parse the content of a TMX map manually instead of loading it using the constructor
    /// </summary>
    /// <param name="xml">The tmx file content as string</param>
    /// <exception cref="TiledException"></exception>
    public void ParseXml(string xml)
    {
        try
        {
            // Load the xml document
            var document = new XmlDocument();
            document.LoadXml(xml);

            XmlNode nodeMap = document.SelectSingleNode("map");
            XmlNodeList nodesProperty = nodeMap.SelectNodes("properties/property");
            XmlNodeList nodesLayer = nodeMap.SelectNodes("layer");
            XmlNodeList nodesImageLayer = nodeMap.SelectNodes("imagelayer");
            XmlNodeList nodesObjectGroup = nodeMap.SelectNodes("objectgroup");
            XmlNodeList nodesTileset = nodeMap.SelectNodes("tileset");
            XmlNodeList nodesGroup = nodeMap.SelectNodes("group");
            XmlAttribute attrParallaxOriginX = nodeMap.Attributes["parallaxoriginx"];
            XmlAttribute attrParallaxOriginY = nodeMap.Attributes["parallaxoriginy"];

            Version = nodeMap.Attributes["tiledversion"].Value;
            Orientation = nodeMap.Attributes["orientation"].Value;
            RenderOrder = nodeMap.Attributes["renderorder"].Value;
            if (nodeMap.Attributes["backgroundcolor"]?.Value is { } backgroundColor)
                BackgroundColor = TiledUtilities.HexToColor(backgroundColor);
            Infinite = nodeMap.Attributes["infinite"].Value == "1";

            int width = int.Parse(nodeMap.Attributes["width"].Value);
            int height = int.Parse(nodeMap.Attributes["height"].Value);
            Size = new Size(width, height);

            int tileWidth = int.Parse(nodeMap.Attributes["tilewidth"].Value);
            int tileHeight = int.Parse(nodeMap.Attributes["tileheight"].Value);
            TileSize = new Size(tileWidth, tileHeight);

            if (nodesProperty != null) Properties = ParseProperties(nodesProperty);
            if (nodesTileset != null) Tilesets = ParseTilesets(nodesTileset);
            if (nodesLayer != null) Layers = ParseLayers(nodesLayer, nodesObjectGroup, nodesImageLayer);
            if (nodesGroup != null) Groups = ParseGroups(nodesGroup);

            var parallaxOriginX = 0.0f;
            var parallaxOriginY = 0.0f;

            if (attrParallaxOriginX != null)
                parallaxOriginX = float.Parse(attrParallaxOriginX.Value, CultureInfo.InvariantCulture);
            if (attrParallaxOriginY != null)
                parallaxOriginY = float.Parse(attrParallaxOriginY.Value, CultureInfo.InvariantCulture);

            ParallaxOrigin = new PointF(parallaxOriginX, parallaxOriginY);
        }
        catch (Exception ex)
        {
            throw new TiledException("An error occurred while trying to parse the Tiled map file", ex);
        }
    }

    private TiledProperty[] ParseProperties(XmlNodeList nodeList)
    {
        var result = new List<TiledProperty>();

        foreach (XmlNode node in nodeList)
        {
            var property = new TiledProperty();
            property.Name = node.Attributes["name"].Value;
            property.Type = node.Attributes["type"]?.Value;
            property.Value = node.Attributes["value"]?.Value;

            if (property.Value == null && node.InnerText != null) property.Value = node.InnerText;

            result.Add(property);
        }

        return result.ToArray();
    }

    private TiledMapTileset[] ParseTilesets(XmlNodeList nodeList)
    {
        var result = new List<TiledMapTileset>();

        foreach (XmlNode node in nodeList)
        {
            var tileset = new TiledMapTileset();
            tileset.FirstGid = int.Parse(node.Attributes["firstgid"].Value);
            tileset.Source = node.Attributes["source"].Value;

            result.Add(tileset);
        }

        return result.ToArray();
    }

    private TiledGroup[] ParseGroups(XmlNodeList nodeListGroups)
    {
        var result = new List<TiledGroup>();

        foreach (XmlNode node in nodeListGroups)
        {
            XmlNodeList nodesProperty = node.SelectNodes("properties/property");
            XmlNodeList nodesGroup = node.SelectNodes("group");
            XmlNodeList nodesLayer = node.SelectNodes("layer");
            XmlNodeList nodesObjectGroup = node.SelectNodes("objectgroup");
            XmlNodeList nodesImageLayer = node.SelectNodes("imagelayer");
            XmlAttribute attrVisible = node.Attributes["visible"];
            XmlAttribute attrLocked = node.Attributes["locked"];

            var tiledGroup = new TiledGroup();
            tiledGroup.Id = int.Parse(node.Attributes["id"].Value);
            tiledGroup.Name = node.Attributes["name"].Value;

            if (attrVisible != null) tiledGroup.IsVisible = attrVisible.Value == "1";
            if (attrLocked != null) tiledGroup.IsLocked = attrLocked.Value == "1";
            if (nodesProperty != null) tiledGroup.Properties = ParseProperties(nodesProperty);
            if (nodesGroup != null) tiledGroup.Groups = ParseGroups(nodesGroup);
            if (nodesLayer != null) tiledGroup.Layers = ParseLayers(nodesLayer, nodesObjectGroup, nodesImageLayer);

            result.Add(tiledGroup);
        }

        return result.ToArray();
    }

    private TiledLayer[] ParseLayers(XmlNodeList nodesLayer, XmlNodeList nodesObjectGroup, XmlNodeList nodesImageLayer)
    {
        var result = new List<TiledLayer>();

        foreach (XmlNode node in nodesLayer) result.Add(ParseTileLayer(node));

        foreach (XmlNode node in nodesObjectGroup) result.Add(ParseObjectLayer(node));

        foreach (XmlNode node in nodesImageLayer) result.Add(ParseImageLayer(node));

        return result.ToArray();
    }

    private TiledLayer ParseTileLayer(XmlNode node)
    {
        XmlNode nodeData = node.SelectSingleNode("data");
        XmlNodeList nodesProperty = node.SelectNodes("properties/property");
        XmlAttribute attrVisible = node.Attributes["visible"];
        XmlAttribute attrLocked = node.Attributes["locked"];
        XmlAttribute attrTint = node.Attributes["tintcolor"];
        XmlAttribute attrOffsetX = node.Attributes["offsetx"];
        XmlAttribute attrOffsetY = node.Attributes["offsety"];
        XmlAttribute attrParallaxX = node.Attributes["parallaxx"];
        XmlAttribute attrParallaxY = node.Attributes["parallaxy"];

        var tiledLayer = new TiledLayer();
        tiledLayer.Id = int.Parse(node.Attributes["id"].Value);
        tiledLayer.Name = node.Attributes["name"].Value;

        int width = int.Parse(node.Attributes["width"].Value);
        int height = int.Parse(node.Attributes["height"].Value);
        tiledLayer.Size = new Size(width, height);

        tiledLayer.Type = TiledLayerType.TileLayer;
        tiledLayer.IsVisible = true;

        var offsetX = 0.0f;
        var offsetY = 0.0f;
        var parallaxX = 0.0f;
        var parallaxY = 0.0f;

        if (attrVisible != null) tiledLayer.IsVisible = attrVisible.Value == "1";
        if (attrLocked != null) tiledLayer.IsLocked = attrLocked.Value == "1";
        if (attrTint != null) tiledLayer.TintColor = TiledUtilities.HexToColor(attrTint.Value);
        if (attrOffsetX != null) offsetX = float.Parse(attrOffsetX.Value);
        if (attrOffsetY != null) offsetY = float.Parse(attrOffsetY.Value);
        if (attrParallaxX != null) parallaxX = float.Parse(attrParallaxX.Value);
        if (attrParallaxY != null) parallaxY = float.Parse(attrParallaxY.Value);
        if (nodesProperty != null) tiledLayer.Properties = ParseProperties(nodesProperty);

        tiledLayer.Offset = new PointF(offsetX, offsetY);
        tiledLayer.Parallax = new PointF(parallaxX, parallaxY);

        ParseTileLayerData(nodeData, ref tiledLayer);

        return tiledLayer;
    }

    private void ParseTileLayerData(XmlNode nodeData, ref TiledLayer tiledLayer)
    {
        string encoding = nodeData.Attributes["encoding"].Value;
        string compression = nodeData.Attributes["compression"]?.Value;

        if (encoding != "csv" && encoding != "base64")
            throw new TiledException("Only CSV and Base64 encodings are currently supported");

        if (Infinite)
        {
            XmlNodeList nodesChunk = nodeData.SelectNodes("chunk");
            var chunks = new List<TiledChunk>();

            foreach (XmlNode nodeChunk in nodesChunk)
            {
                var chunk = new TiledChunk();
                chunk.X = int.Parse(nodeChunk.Attributes["x"].Value);
                chunk.Y = int.Parse(nodeChunk.Attributes["y"].Value);
                chunk.Width = int.Parse(nodeChunk.Attributes["width"].Value);
                chunk.Height = int.Parse(nodeChunk.Attributes["height"].Value);

                if (encoding == "csv")
                {
                    int[] data = chunk.Data;
                    byte[] rotationFlags = chunk.DataRotationFlags;
                    ParseTileLayerDataAsCsv(nodeChunk.InnerText, ref data, ref rotationFlags);
                    chunk.Data = data;
                    chunk.DataRotationFlags = rotationFlags;
                }

                if (encoding == "base64")
                {
                    int[] data = chunk.Data;
                    byte[] rotationFlags = chunk.DataRotationFlags;
                    ParseTileLayerDataAsBase64(nodeChunk.InnerText, compression, ref data, ref rotationFlags);
                    chunk.Data = data;
                    chunk.DataRotationFlags = rotationFlags;
                }

                chunks.Add(chunk);
            }

            tiledLayer.Chunks = chunks.ToArray();
        }
        else
        {
            if (encoding == "csv")
            {
                int[] data = tiledLayer.Data;
                byte[] rotationFlags = tiledLayer.DataRotationFlags;
                ParseTileLayerDataAsCsv(nodeData.InnerText, ref data, ref rotationFlags);
                tiledLayer.Data = data;
                tiledLayer.DataRotationFlags = rotationFlags;
            }

            if (encoding == "base64")
            {
                int[] data = tiledLayer.Data;
                byte[] rotationFlags = tiledLayer.DataRotationFlags;
                ParseTileLayerDataAsBase64(nodeData.InnerText, compression, ref data, ref rotationFlags);
                tiledLayer.Data = data;
                tiledLayer.DataRotationFlags = rotationFlags;
            }
        }
    }

    private void ParseTileLayerDataAsBase64(string input, string compression, ref int[] data, ref byte[] dataRotationFlags)
    {
        using (var base64DataStream = new MemoryStream(Convert.FromBase64String(input)))
        {
            if (compression == null)
            {
                // Parse the decoded bytes and update the inner data as well as the data rotation flags
                var rawBytes = new byte[4];
                data = new int[base64DataStream.Length];
                dataRotationFlags = new byte[base64DataStream.Length];

                for (var i = 0; i < base64DataStream.Length; i++)
                {
                    base64DataStream.Read(rawBytes, 0, rawBytes.Length);
                    var rawId = BitConverter.ToUInt32(rawBytes, 0);
                    uint hor = rawId & FlippedHorizontallyFlag;
                    uint ver = rawId & FlippedVerticallyFlag;
                    uint dia = rawId & FlippedDiagonallyFlag;
                    dataRotationFlags[i] = (byte) ((hor | ver | dia) >> ShiftFlipFlagToByte);

                    // assign data to rawID with the rotation flags cleared
                    data[i] = (int) (rawId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag));
                }
            }
            else if (compression == "zlib")
            {
                // .NET doesn't play well with the headered zlib data that Tiled produces,
                // so we have to manually skip the 2-byte header to get what DeflateStream's looking for
                // Should an external library be used instead of this hack?
                base64DataStream.ReadByte();
                base64DataStream.ReadByte();

                using (var decompressionStream = new DeflateStream(base64DataStream, CompressionMode.Decompress))
                {
                    // Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
                    var decompressedDataBuffer = new byte[4]; // size of each tile
                    var dataRotationFlagsList = new List<byte>();
                    var layerDataList = new List<int>();

                    while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) ==
                           decompressedDataBuffer.Length)
                    {
                        var rawId = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                        uint hor = rawId & FlippedHorizontallyFlag;
                        uint ver = rawId & FlippedVerticallyFlag;
                        uint dia = rawId & FlippedDiagonallyFlag;
                        dataRotationFlagsList.Add((byte) ((hor | ver | dia) >> ShiftFlipFlagToByte));

                        // assign data to rawID with the rotation flags cleared
                        layerDataList.Add((int) (rawId &
                                                 ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag)));
                    }

                    data = layerDataList.ToArray();
                    dataRotationFlags = dataRotationFlagsList.ToArray();
                }
            }
            else if (compression == "gzip")
            {
                using (var decompressionStream =
                       new GZipStream(base64DataStream, CompressionMode.Decompress))
                {
                    // Parse the raw decompressed bytes and update the inner data as well as the data rotation flags
                    var decompressedDataBuffer = new byte[4]; // size of each tile
                    var dataRotationFlagsList = new List<byte>();
                    var layerDataList = new List<int>();

                    while (decompressionStream.Read(decompressedDataBuffer, 0, decompressedDataBuffer.Length) ==
                           decompressedDataBuffer.Length)
                    {
                        var rawId = BitConverter.ToUInt32(decompressedDataBuffer, 0);
                        uint hor = rawId & FlippedHorizontallyFlag;
                        uint ver = rawId & FlippedVerticallyFlag;
                        uint dia = rawId & FlippedDiagonallyFlag;

                        dataRotationFlagsList.Add((byte) ((hor | ver | dia) >> ShiftFlipFlagToByte));

                        // assign data to rawID with the rotation flags cleared
                        layerDataList.Add((int) (rawId &
                                                 ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag)));
                    }

                    data = layerDataList.ToArray();
                    dataRotationFlags = dataRotationFlagsList.ToArray();
                }
            }
            else
                throw new TiledException("Zstandard compression is currently not supported");
        }
    }

    private void ParseTileLayerDataAsCsv(string input, ref int[] data, ref byte[] dataRotationFlags)
    {
        string[] csvs = input.Split(',');

        data = new int[csvs.Length];
        dataRotationFlags = new byte[csvs.Length];

        // Parse the comma separated csv string and update the inner data as well as the data rotation flags
        for (var i = 0; i < csvs.Length; i++)
        {
            uint rawId = uint.Parse(csvs[i]);
            uint hor = rawId & FlippedHorizontallyFlag;
            uint ver = rawId & FlippedVerticallyFlag;
            uint dia = rawId & FlippedDiagonallyFlag;
            dataRotationFlags[i] = (byte) ((hor | ver | dia) >> ShiftFlipFlagToByte);

            // assign data to rawID with the rotation flags cleared
            data[i] = (int) (rawId & ~(FlippedHorizontallyFlag | FlippedVerticallyFlag | FlippedDiagonallyFlag));
        }
    }

    private TiledLayer ParseObjectLayer(XmlNode node)
    {
        XmlNodeList nodesProperty = node.SelectNodes("properties/property");
        XmlNodeList nodesObject = node.SelectNodes("object");
        XmlAttribute attrVisible = node.Attributes["visible"];
        XmlAttribute attrLocked = node.Attributes["locked"];
        XmlAttribute attrTint = node.Attributes["tintcolor"];
        XmlAttribute attrOffsetX = node.Attributes["offsetx"];
        XmlAttribute attrOffsetY = node.Attributes["offsety"];

        var tiledLayer = new TiledLayer();
        tiledLayer.Id = int.Parse(node.Attributes["id"].Value);
        tiledLayer.Name = node.Attributes["name"].Value;
        tiledLayer.Objects = ParseObjects(nodesObject);
        tiledLayer.Type = TiledLayerType.ObjectLayer;
        tiledLayer.IsVisible = true;

        var offsetX = 0.0f;
        var offsetY = 0.0f;

        if (attrVisible != null) tiledLayer.IsVisible = attrVisible.Value == "1";
        if (attrLocked != null) tiledLayer.IsLocked = attrLocked.Value == "1";
        if (attrTint != null) tiledLayer.TintColor = TiledUtilities.HexToColor(attrTint.Value);
        if (attrOffsetX != null) offsetX = int.Parse(attrOffsetX.Value);
        if (attrOffsetY != null) offsetY = int.Parse(attrOffsetY.Value);
        if (nodesProperty != null) tiledLayer.Properties = ParseProperties(nodesProperty);

        tiledLayer.Offset = new PointF(offsetX, offsetY);

        return tiledLayer;
    }

    private TiledLayer ParseImageLayer(XmlNode node)
    {
        XmlNodeList nodesProperty = node.SelectNodes("properties/property");
        XmlNode nodeImage = node.SelectSingleNode("image");
        XmlAttribute attrVisible = node.Attributes["visible"];
        XmlAttribute attrLocked = node.Attributes["locked"];
        XmlAttribute attrTint = node.Attributes["tintcolor"];
        XmlAttribute attrOffsetX = node.Attributes["offsetx"];
        XmlAttribute attrOffsetY = node.Attributes["offsety"];

        var tiledLayer = new TiledLayer();
        tiledLayer.Id = int.Parse(node.Attributes["id"].Value);
        tiledLayer.Name = node.Attributes["name"].Value;
        tiledLayer.Type = TiledLayerType.ImageLayer;
        tiledLayer.IsVisible = true;

        var offsetX = 0.0f;
        var offsetY = 0.0f;

        if (attrVisible != null) tiledLayer.IsVisible = attrVisible.Value == "1";
        if (attrLocked != null) tiledLayer.IsLocked = attrLocked.Value == "1";
        if (attrTint != null) tiledLayer.TintColor = TiledUtilities.HexToColor(attrTint.Value);
        if (attrOffsetX != null) offsetX = int.Parse(attrOffsetX.Value);
        if (attrOffsetY != null) offsetY = int.Parse(attrOffsetY.Value);
        if (nodesProperty != null) tiledLayer.Properties = ParseProperties(nodesProperty);
        if (nodeImage != null) tiledLayer.Image = ParseImage(nodeImage);

        tiledLayer.Offset = new PointF(offsetX, offsetY);
        return tiledLayer;
    }

    private TiledImage ParseImage(XmlNode node)
    {
        var tiledImage = new TiledImage();
        tiledImage.Source = node.Attributes["source"].Value;
        tiledImage.Width = int.Parse(node.Attributes["width"].Value);
        tiledImage.Height = int.Parse(node.Attributes["height"].Value);

        return tiledImage;
    }

    private TiledObject[] ParseObjects(XmlNodeList nodeList)
    {
        var result = new List<TiledObject>();

        foreach (XmlNode node in nodeList)
        {
            XmlNodeList nodesProperty = node.SelectNodes("properties/property");
            XmlNode nodePolygon = node.SelectSingleNode("polygon");
            XmlNode nodePoint = node.SelectSingleNode("point");
            XmlNode nodeEllipse = node.SelectSingleNode("ellipse");

            var obj = new TiledObject();
            obj.Id = int.Parse(node.Attributes["id"].Value);
            obj.Name = node.Attributes["name"]?.Value;
            obj.Type = node.Attributes["type"]?.Value;
            obj.Gid = int.Parse(node.Attributes["gid"]?.Value ?? "0");

            float x = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture);
            float y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture);
            obj.Position = new PointF(x, y);

            if (nodesProperty != null) obj.Properties = ParseProperties(nodesProperty);

            if (nodePolygon != null)
            {
                string points = nodePolygon.Attributes["points"].Value;
                string[] vertices = points.Split(' ');

                var polygon = new TiledPolygon();
                polygon.Points = new float[vertices.Length * 2];

                for (var i = 0; i < vertices.Length; i++)
                {
                    polygon.Points[i * 2 + 0] =
                        float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture);
                    polygon.Points[i * 2 + 1] =
                        float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture);
                }

                obj.Polygon = polygon;
            }

            if (nodeEllipse != null) obj.Ellipse = new TiledEllipse();

            if (nodePoint != null) obj.Point = new TiledPoint();

            var width = 0.0f;
            var height = 0.0f;

            if (node.Attributes["width"] != null)
                width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);

            if (node.Attributes["height"] != null)
                height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);

            obj.Size = new SizeF(width, height);

            if (node.Attributes["rotation"] != null)
                obj.Rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);

            result.Add(obj);
        }

        return result.ToArray();
    }

    /* HELPER METHODS */
    /// <summary>
    ///     Locates the right TiledMapTileset object for you within the Tilesets array
    /// </summary>
    /// <param name="gid">A value from the TiledLayer.data array</param>
    /// <returns>An element within the Tilesets array or null if no match was found</returns>
    public TiledMapTileset GetTiledMapTileset(int gid)
    {
        if (Tilesets == null) return null;

        for (var i = 0; i < Tilesets.Length; i++)
        {
            if (i < Tilesets.Length - 1)
            {
                int gid1 = Tilesets[i + 0].FirstGid;
                int gid2 = Tilesets[i + 1].FirstGid;

                if (gid >= gid1 && gid < gid2) return Tilesets[i];
            }
            else
                return Tilesets[i];
        }

        return new TiledMapTileset();
    }

    /// <summary>
    ///     Loads external tilesets and matches them to firstGids from elements within the Tilesets array
    /// </summary>
    /// <param name="src">The folder where the TiledMap file is located</param>
    /// <returns>
    ///     A dictionary where the key represents the firstGid of the associated TiledMapTileset and the value the TiledTileset
    ///     object
    /// </returns>
    public Dictionary<int, TiledTileset> GetTiledTilesets(string src)
    {
        var tilesets = new Dictionary<int, TiledTileset>();
        var info = new FileInfo(src);
        DirectoryInfo srcFolder = info.Directory;

        if (Tilesets == null) return tilesets;

        foreach (TiledMapTileset mapTileset in Tilesets)
        {
            var path = $"{srcFolder}/{mapTileset.Source}";

            if (File.Exists(path))
                tilesets.Add(mapTileset.FirstGid, new TiledTileset(path));
            else
            {
                throw new TiledException("Cannot locate tileset '" + path +
                                         "'. Please make sure the source folder is correct and it ends with a slash.");
            }
        }

        return tilesets;
    }

    /// <summary>
    ///     Locates a specific TiledTile object
    /// </summary>
    /// <param name="mapTileset">An element within the Tilesets array</param>
    /// <param name="tileset">An instance of the TiledTileset class</param>
    /// <param name="gid">An element from within a TiledLayer.data array</param>
    /// <returns>An entry of the TiledTileset.tiles array or null if none of the tile id's matches the gid</returns>
    /// <remarks>
    ///     Tip: Use the GetTiledMapTileset and GetTiledTilesets methods for retrieving the correct TiledMapTileset and TiledTileset
    ///     objects
    /// </remarks>
    public TiledTile GetTiledTile(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
    {
        foreach (TiledTile tile in tileset.Tiles)
        {
            if (tile.Id == gid - mapTileset.FirstGid)
                return tile;
        }

        return null;
    }

    /// <summary>
    ///     This method can be used to figure out the x and y position on a Tileset image for rendering tiles.
    /// </summary>
    /// <param name="mapTileset">An element of the Tilesets array</param>
    /// <param name="tileset">An instance of the TiledTileset class</param>
    /// <param name="gid">An element within a TiledLayer.data array</param>
    /// <returns>
    ///     An int array of length 2 containing the x and y position of the source rect of the tileset image. Multiply the values by
    ///     the tile width and height in pixels to get the actual x and y position. Returns null if the gid was not found
    /// </returns>
    /// <remarks>This method currently doesn't take margin into account</remarks>
    [Obsolete(
        "Please use GetSourceRect instead because with future versions of Tiled this method may no longer be sufficient")]
    public int[] GetSourceVector(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
    {
        var tileHor = 0;
        var tileVert = 0;

        for (var i = 0; i < tileset.TileCount; i++)
        {
            if (i == gid - mapTileset.FirstGid) return new[] {tileHor, tileVert};

            // Update x and y position
            tileHor++;

            if (tileHor == tileset.Image.Width / tileset.TileWidth)
            {
                tileHor = 0;
                tileVert++;
            }
        }

        return null;
    }

    /// <summary>
    ///     This method can be used to figure out the source rect on a Tileset image for rendering tiles.
    /// </summary>
    /// <param name="mapTileset"></param>
    /// <param name="tileset"></param>
    /// <param name="gid"></param>
    /// <returns>
    ///     The source rectangle that encapsulates the tile at the specified GID, or <see cref="Rectangle.Empty" /> if the
    ///     GID was not found.
    /// </returns>
    public Rectangle GetSourceRect(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
    {
        var tileHor = 0;
        var tileVert = 0;

        for (var i = 0; i < tileset.TileCount; i++)
        {
            if (i == gid - mapTileset.FirstGid)
            {
                int x = tileHor * tileset.TileWidth;
                int y = tileVert * tileset.TileHeight;

                return new Rectangle(x, y, tileset.TileWidth, tileset.TileHeight);
            }

            // Update x and y position
            tileHor++;

            if (tileHor == tileset.Image.Width / tileset.TileWidth)
            {
                tileHor = 0;
                tileVert++;
            }
        }

        return Rectangle.Empty;
    }

    /// <summary>
    ///     Checks is a tile is flipped horizontally
    /// </summary>
    /// <param name="layer">An entry of the TiledMap.layers array</param>
    /// <param name="tileHor">The tile's horizontal position</param>
    /// <param name="tileVert">The tile's vertical position</param>
    /// <returns>True if the tile was flipped horizontally or False if not</returns>
    public bool IsTileFlippedHorizontal(TiledLayer layer, int tileHor, int tileVert)
    {
        return IsTileFlippedHorizontal(layer, tileHor + tileVert * layer.Size.Width);
    }

    /// <summary>
    ///     Checks is a tile is flipped horizontally
    /// </summary>
    /// <param name="layer">An entry of the TiledMap.layers array</param>
    /// <param name="dataIndex">An index of the TiledLayer.data array</param>
    /// <returns>True if the tile was flipped horizontally or False if not</returns>
    public bool IsTileFlippedHorizontal(TiledLayer layer, int dataIndex)
    {
        return (layer.DataRotationFlags[dataIndex] & (FlippedHorizontallyFlag >> ShiftFlipFlagToByte)) > 0;
    }

    /// <summary>
    ///     Checks is a tile is flipped vertically
    /// </summary>
    /// <param name="layer">An entry of the TiledMap.layers array</param>
    /// <param name="tileHor">The tile's horizontal position</param>
    /// <param name="tileVert">The tile's vertical position</param>
    /// <returns>True if the tile was flipped vertically or False if not</returns>
    public bool IsTileFlippedVertical(TiledLayer layer, int tileHor, int tileVert)
    {
        return IsTileFlippedVertical(layer, tileHor + tileVert * layer.Size.Width);
    }

    /// <summary>
    ///     Checks is a tile is flipped vertically
    /// </summary>
    /// <param name="layer">An entry of the TiledMap.layers array</param>
    /// <param name="dataIndex">An index of the TiledLayer.data array</param>
    /// <returns>True if the tile was flipped vertically or False if not</returns>
    public bool IsTileFlippedVertical(TiledLayer layer, int dataIndex)
    {
        return (layer.DataRotationFlags[dataIndex] & (FlippedVerticallyFlag >> ShiftFlipFlagToByte)) > 0;
    }

    /// <summary>
    ///     Checks is a tile is flipped diagonally
    /// </summary>
    /// <param name="layer">An entry of the TiledMap.layers array</param>
    /// <param name="tileHor">The tile's horizontal position</param>
    /// <param name="tileVert">The tile's vertical position</param>
    /// <returns>True if the tile was flipped diagonally or False if not</returns>
    public bool IsTileFlippedDiagonal(TiledLayer layer, int tileHor, int tileVert)
    {
        return IsTileFlippedDiagonal(layer, tileHor + tileVert * layer.Size.Width);
    }

    /// <summary>
    ///     Checks is a tile is flipped diagonally
    /// </summary>
    /// <param name="layer">An entry of the TiledMap.layers array</param>
    /// <param name="dataIndex">An index of the TiledLayer.data array</param>
    /// <returns>True if the tile was flipped diagonally or False if not</returns>
    public bool IsTileFlippedDiagonal(TiledLayer layer, int dataIndex)
    {
        return (layer.DataRotationFlags[dataIndex] & (FlippedDiagonallyFlag >> ShiftFlipFlagToByte)) > 0;
    }
}
