using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace TiledCS
{
    /// <summary>
    ///     Represents a Tiled map
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
        ///     Returns an empty instance of TiledMap
        /// </summary>
        public TiledMap()
        {
        }

        /// <summary>
        ///     Loads a Tiled map in TMX format and parses it
        /// </summary>
        /// <param name="path">The path to the tmx file</param>
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
        ///     The amount of horizontal tiles
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     The amount of vertical tiles
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     The tile width in pixels
        /// </summary>
        public int TileWidth { get; set; }

        /// <summary>
        ///     The tile height in pixels
        /// </summary>
        public int TileHeight { get; set; }

        /// <summary>
        ///     The parallax origin x
        /// </summary>
        public float ParallaxOriginX { get; set; }

        /// <summary>
        ///     The parallax origin y
        /// </summary>
        public float ParallaxOriginY { get; set; }

        /// <summary>
        ///     Returns true if the map is configured as infinite
        /// </summary>
        public bool Infinite { get; set; }

        /// <summary>
        ///     Returns the defined map background color as a hex string
        /// </summary>
        public string BackgroundColor { get; set; }

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
                BackgroundColor = nodeMap.Attributes["backgroundcolor"]?.Value;
                Infinite = nodeMap.Attributes["infinite"].Value == "1";

                Width = int.Parse(nodeMap.Attributes["width"].Value);
                Height = int.Parse(nodeMap.Attributes["height"].Value);
                TileWidth = int.Parse(nodeMap.Attributes["tilewidth"].Value);
                TileHeight = int.Parse(nodeMap.Attributes["tileheight"].Value);

                if (nodesProperty != null) Properties = ParseProperties(nodesProperty);
                if (nodesTileset != null) Tilesets = ParseTilesets(nodesTileset);
                if (nodesLayer != null) Layers = ParseLayers(nodesLayer, nodesObjectGroup, nodesImageLayer);
                if (nodesGroup != null) Groups = ParseGroups(nodesGroup);
                if (attrParallaxOriginX != null)
                    ParallaxOriginX = float.Parse(attrParallaxOriginX.Value, CultureInfo.InvariantCulture);
                if (attrParallaxOriginY != null)
                    ParallaxOriginY = float.Parse(attrParallaxOriginY.Value, CultureInfo.InvariantCulture);
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
                property.name = node.Attributes["name"].Value;
                property.type = node.Attributes["type"]?.Value;
                property.value = node.Attributes["value"]?.Value;

                if (property.value == null && node.InnerText != null) property.value = node.InnerText;

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
                tileset.firstgid = int.Parse(node.Attributes["firstgid"].Value);
                tileset.source = node.Attributes["source"].Value;

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
                tiledGroup.id = int.Parse(node.Attributes["id"].Value);
                tiledGroup.name = node.Attributes["name"].Value;

                if (attrVisible != null) tiledGroup.visible = attrVisible.Value == "1";
                if (attrLocked != null) tiledGroup.locked = attrLocked.Value == "1";
                if (nodesProperty != null) tiledGroup.properties = ParseProperties(nodesProperty);
                if (nodesGroup != null) tiledGroup.groups = ParseGroups(nodesGroup);
                if (nodesLayer != null) tiledGroup.layers = ParseLayers(nodesLayer, nodesObjectGroup, nodesImageLayer);

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
            tiledLayer.id = int.Parse(node.Attributes["id"].Value);
            tiledLayer.name = node.Attributes["name"].Value;
            tiledLayer.height = int.Parse(node.Attributes["height"].Value);
            tiledLayer.width = int.Parse(node.Attributes["width"].Value);
            tiledLayer.type = TiledLayerType.TileLayer;
            tiledLayer.visible = true;

            if (attrVisible != null) tiledLayer.visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.tintcolor = attrTint.Value;
            if (attrOffsetX != null) tiledLayer.offsetX = float.Parse(attrOffsetX.Value);
            if (attrOffsetY != null) tiledLayer.offsetY = float.Parse(attrOffsetY.Value);
            if (attrParallaxX != null) tiledLayer.offsetX = float.Parse(attrParallaxX.Value);
            if (attrParallaxY != null) tiledLayer.offsetY = float.Parse(attrParallaxY.Value);
            if (nodesProperty != null) tiledLayer.properties = ParseProperties(nodesProperty);

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
                    chunk.x = int.Parse(nodeChunk.Attributes["x"].Value);
                    chunk.y = int.Parse(nodeChunk.Attributes["y"].Value);
                    chunk.width = int.Parse(nodeChunk.Attributes["width"].Value);
                    chunk.height = int.Parse(nodeChunk.Attributes["height"].Value);

                    if (encoding == "csv")
                        ParseTileLayerDataAsCsv(nodeChunk.InnerText, ref chunk.data, ref chunk.dataRotationFlags);
                    if (encoding == "base64")
                        ParseTileLayerDataAsBase64(nodeChunk.InnerText, compression, ref chunk.data, ref chunk.dataRotationFlags);

                    chunks.Add(chunk);
                }

                tiledLayer.chunks = chunks.ToArray();
            }
            else
            {
                if (encoding == "csv")
                    ParseTileLayerDataAsCsv(nodeData.InnerText, ref tiledLayer.data, ref tiledLayer.dataRotationFlags);
                if (encoding == "base64")
                    ParseTileLayerDataAsBase64(nodeData.InnerText, compression, ref tiledLayer.data,
                        ref tiledLayer.dataRotationFlags);
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
            tiledLayer.id = int.Parse(node.Attributes["id"].Value);
            tiledLayer.name = node.Attributes["name"].Value;
            tiledLayer.objects = ParseObjects(nodesObject);
            tiledLayer.type = TiledLayerType.ObjectLayer;
            tiledLayer.visible = true;

            if (attrVisible != null) tiledLayer.visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.tintcolor = attrTint.Value;
            if (attrOffsetX != null) tiledLayer.offsetX = int.Parse(attrOffsetX.Value);
            if (attrOffsetY != null) tiledLayer.offsetY = int.Parse(attrOffsetY.Value);
            if (nodesProperty != null) tiledLayer.properties = ParseProperties(nodesProperty);

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
            tiledLayer.id = int.Parse(node.Attributes["id"].Value);
            tiledLayer.name = node.Attributes["name"].Value;
            tiledLayer.type = TiledLayerType.ImageLayer;
            tiledLayer.visible = true;

            if (attrVisible != null) tiledLayer.visible = attrVisible.Value == "1";
            if (attrLocked != null) tiledLayer.locked = attrLocked.Value == "1";
            if (attrTint != null) tiledLayer.tintcolor = attrTint.Value;
            if (attrOffsetX != null) tiledLayer.offsetX = int.Parse(attrOffsetX.Value);
            if (attrOffsetY != null) tiledLayer.offsetY = int.Parse(attrOffsetY.Value);
            if (nodesProperty != null) tiledLayer.properties = ParseProperties(nodesProperty);
            if (nodeImage != null) tiledLayer.image = ParseImage(nodeImage);

            return tiledLayer;
        }

        private TiledImage ParseImage(XmlNode node)
        {
            var tiledImage = new TiledImage();
            tiledImage.source = node.Attributes["source"].Value;
            tiledImage.width = int.Parse(node.Attributes["width"].Value);
            tiledImage.height = int.Parse(node.Attributes["height"].Value);

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
                obj.id = int.Parse(node.Attributes["id"].Value);
                obj.name = node.Attributes["name"]?.Value;
                obj.type = node.Attributes["type"]?.Value;
                obj.gid = int.Parse(node.Attributes["gid"]?.Value ?? "0");
                obj.x = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture);
                obj.y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture);

                if (nodesProperty != null) obj.properties = ParseProperties(nodesProperty);

                if (nodePolygon != null)
                {
                    string points = nodePolygon.Attributes["points"].Value;
                    string[] vertices = points.Split(' ');

                    var polygon = new TiledPolygon();
                    polygon.points = new float[vertices.Length * 2];

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        polygon.points[i * 2 + 0] =
                            float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture);
                        polygon.points[i * 2 + 1] =
                            float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture);
                    }

                    obj.polygon = polygon;
                }

                if (nodeEllipse != null) obj.ellipse = new TiledEllipse();

                if (nodePoint != null) obj.point = new TiledPoint();

                if (node.Attributes["width"] != null)
                    obj.width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["height"] != null)
                    obj.height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["rotation"] != null)
                    obj.rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);

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
                    int gid1 = Tilesets[i + 0].firstgid;
                    int gid2 = Tilesets[i + 1].firstgid;

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
                var path = $"{srcFolder}/{mapTileset.source}";

                if (File.Exists(path))
                    tilesets.Add(mapTileset.firstgid, new TiledTileset(path));
                else
                    throw new TiledException("Cannot locate tileset '" + path +
                                             "'. Please make sure the source folder is correct and it ends with a slash.");
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
                if (tile.id == gid - mapTileset.firstgid) return tile;
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
                if (i == gid - mapTileset.firstgid) return new[] {tileHor, tileVert};

                // Update x and y position
                tileHor++;

                if (tileHor == tileset.Image.width / tileset.TileWidth)
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
        ///     An instance of the class TiledSourceRect that represents a rectangle. Returns null if the provided gid was not found
        ///     within the tileset.
        /// </returns>
        public TiledSourceRect GetSourceRect(TiledMapTileset mapTileset, TiledTileset tileset, int gid)
        {
            var tileHor = 0;
            var tileVert = 0;

            for (var i = 0; i < tileset.TileCount; i++)
            {
                if (i == gid - mapTileset.firstgid)
                {
                    var result = new TiledSourceRect();
                    result.x = tileHor * tileset.TileWidth;
                    result.y = tileVert * tileset.TileHeight;
                    result.width = tileset.TileWidth;
                    result.height = tileset.TileHeight;

                    return result;
                }

                // Update x and y position
                tileHor++;

                if (tileHor == tileset.Image.width / tileset.TileWidth)
                {
                    tileHor = 0;
                    tileVert++;
                }
            }

            return null;
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
            return IsTileFlippedHorizontal(layer, tileHor + tileVert * layer.width);
        }

        /// <summary>
        ///     Checks is a tile is flipped horizontally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped horizontally or False if not</returns>
        public bool IsTileFlippedHorizontal(TiledLayer layer, int dataIndex)
        {
            return (layer.dataRotationFlags[dataIndex] & (FlippedHorizontallyFlag >> ShiftFlipFlagToByte)) > 0;
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
            return IsTileFlippedVertical(layer, tileHor + tileVert * layer.width);
        }

        /// <summary>
        ///     Checks is a tile is flipped vertically
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped vertically or False if not</returns>
        public bool IsTileFlippedVertical(TiledLayer layer, int dataIndex)
        {
            return (layer.dataRotationFlags[dataIndex] & (FlippedVerticallyFlag >> ShiftFlipFlagToByte)) > 0;
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
            return IsTileFlippedDiagonal(layer, tileHor + tileVert * layer.width);
        }

        /// <summary>
        ///     Checks is a tile is flipped diagonally
        /// </summary>
        /// <param name="layer">An entry of the TiledMap.layers array</param>
        /// <param name="dataIndex">An index of the TiledLayer.data array</param>
        /// <returns>True if the tile was flipped diagonally or False if not</returns>
        public bool IsTileFlippedDiagonal(TiledLayer layer, int dataIndex)
        {
            return (layer.dataRotationFlags[dataIndex] & (FlippedDiagonallyFlag >> ShiftFlipFlagToByte)) > 0;
        }
    }
}
