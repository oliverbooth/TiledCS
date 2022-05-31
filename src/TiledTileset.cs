using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using TiledCS.Shapes;

namespace TiledCS
{
    /// <summary>
    ///     Represents a Tiled tileset.
    /// </summary>
    public class TiledTileset
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TiledTileset" /> class.
        /// </summary>
        public TiledTileset()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TiledTileset" /> class by loading a tileset from the specified file.
        /// </summary>
        /// <param name="path">The file path of the TSX file.</param>
        /// <exception cref="TiledException">Thrown when the file could not be found or parsed</exception>
        public TiledTileset(string path)
        {
            // Check the file
            if (!File.Exists(path)) throw new TiledException($"{path} not found");

            string content = File.ReadAllText(path);

            if (path.EndsWith(".tsx"))
                ParseXml(content);
            else
                throw new TiledException("Unsupported file format");
        }

        /// <summary>
        ///     Gets the Tiled version used to create this tileset.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        ///     Gets the tileset name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets the tile width in pixels.
        /// </summary>
        public int TileWidth { get; set; }

        /// <summary>
        ///     Gets the tile height in pixels.
        /// </summary>
        public int TileHeight { get; set; }

        /// <summary>
        ///     Gets the total amount of tiles.
        /// </summary>
        public int TileCount { get; set; }

        /// <summary>
        ///     Gets the amount of horizontal tiles.
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        ///     Gets the image definition used by the tileset.
        /// </summary>
        public TiledImage Image { get; set; }

        /// <summary>
        ///     Gets the amount of spacing between the tiles in pixels.
        /// </summary>
        public int Spacing { get; set; }

        /// <summary>
        ///     Gets the amount of margin between the tiles in pixels.
        /// </summary>
        public int Margin { get; set; }

        /// <summary>
        ///     An array of tile definitions
        /// </summary>
        /// <remarks>Not all tiles within a tileset have definitions. Only those with properties, animations, terrains, ...</remarks>
        public TiledTile[] Tiles { get; set; }

        /// <summary>
        ///     An array of tileset properties
        /// </summary>
        public TiledProperty[] Properties { get; set; }

        /// <summary>
        ///     Can be used to parse the content of a TSX tileset manually instead of loading it using the constructor
        /// </summary>
        /// <param name="xml">The tmx file content as string</param>
        /// <exception cref="TiledException"></exception>
        public void ParseXml(string xml)
        {
            try
            {
                var document = new XmlDocument();
                document.LoadXml(xml);

                XmlNode nodeTileset = document.SelectSingleNode("tileset");
                XmlNode nodeImage = nodeTileset.SelectSingleNode("image");
                XmlNodeList nodesTile = nodeTileset.SelectNodes("tile");
                XmlNodeList nodesProperty = nodeTileset.SelectNodes("properties/property");

                XmlAttribute attrMargin = nodeTileset.Attributes["margin"];
                XmlAttribute attrSpacing = nodeTileset.Attributes["spacing"];

                Version = nodeTileset.Attributes["tiledversion"].Value;
                Name = nodeTileset.Attributes["name"]?.Value;
                TileWidth = int.Parse(nodeTileset.Attributes["tilewidth"].Value);
                TileHeight = int.Parse(nodeTileset.Attributes["tileheight"].Value);
                TileCount = int.Parse(nodeTileset.Attributes["tilecount"].Value);
                Columns = int.Parse(nodeTileset.Attributes["columns"].Value);

                if (attrMargin != null) Margin = int.Parse(nodeTileset.Attributes["margin"].Value);
                if (attrSpacing != null) Spacing = int.Parse(nodeTileset.Attributes["spacing"].Value);
                if (nodeImage != null) Image = ParseImage(nodeImage);

                Tiles = ParseTiles(nodesTile);
                Properties = ParseProperties(nodesProperty);
            }
            catch (Exception ex)
            {
                throw new TiledException("An error occurred while trying to parse the Tiled tileset file", ex);
            }
        }

        private TiledImage ParseImage(XmlNode node)
        {
            var tiledImage = new TiledImage();
            tiledImage.Source = node.Attributes["source"].Value;
            tiledImage.Width = int.Parse(node.Attributes["width"].Value);
            tiledImage.Height = int.Parse(node.Attributes["height"].Value);

            return tiledImage;
        }

        private TiledTileAnimation[] ParseAnimations(XmlNodeList nodeList)
        {
            var result = new List<TiledTileAnimation>();

            foreach (XmlNode node in nodeList)
            {
                var animation = new TiledTileAnimation();
                animation.TileId = int.Parse(node.Attributes["tileid"].Value);
                animation.Duration = int.Parse(node.Attributes["duration"].Value);

                result.Add(animation);
            }

            return result.ToArray();
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

        private TiledTile[] ParseTiles(XmlNodeList nodeList)
        {
            var result = new List<TiledTile>();

            foreach (XmlNode node in nodeList)
            {
                XmlNodeList nodesProperty = node.SelectNodes("properties/property");
                XmlNodeList nodesObject = node.SelectNodes("objectgroup/object");
                XmlNodeList nodesAnimation = node.SelectNodes("animation/frame");
                XmlNode nodeImage = node.SelectSingleNode("image");

                var tile = new TiledTile();
                tile.Id = int.Parse(node.Attributes["id"].Value);
                tile.Type = node.Attributes["type"]?.Value;
                tile.Terrain = node.Attributes["terrain"]?.Value.Split(',').AsIntArray();
                tile.Properties = ParseProperties(nodesProperty);
                tile.Animation = ParseAnimations(nodesAnimation);
                tile.Objects = ParseObjects(nodesObject);

                if (nodeImage != null)
                {
                    var tileImage = new TiledImage();
                    tileImage.Width = int.Parse(nodeImage.Attributes["width"].Value);
                    tileImage.Height = int.Parse(nodeImage.Attributes["height"].Value);
                    tileImage.Source = nodeImage.Attributes["source"].Value;

                    tile.Image = tileImage;
                }

                result.Add(tile);
            }

            return result.ToArray();
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
                obj.X = float.Parse(node.Attributes["x"].Value, CultureInfo.InvariantCulture);
                obj.Y = float.Parse(node.Attributes["y"].Value, CultureInfo.InvariantCulture);

                if (nodesProperty != null) obj.Properties = ParseProperties(nodesProperty);

                if (nodePolygon != null)
                {
                    string points = nodePolygon.Attributes["points"].Value;
                    string[] vertices = points.Split(' ');

                    var polygon = new TiledPolygon();
                    polygon.Points = new float[vertices.Length * 2];

                    for (var i = 0; i < vertices.Length; i++)
                    {
                        polygon.Points[i * 2 + 0] = float.Parse(vertices[i].Split(',')[0], CultureInfo.InvariantCulture);
                        polygon.Points[i * 2 + 1] = float.Parse(vertices[i].Split(',')[1], CultureInfo.InvariantCulture);
                    }

                    obj.Polygon = polygon;
                }

                if (nodeEllipse != null) obj.Ellipse = new TiledEllipse();

                if (nodePoint != null) obj.Point = new TiledPoint();

                if (node.Attributes["width"] != null)
                    obj.Width = float.Parse(node.Attributes["width"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["height"] != null)
                    obj.Height = float.Parse(node.Attributes["height"].Value, CultureInfo.InvariantCulture);

                if (node.Attributes["rotation"] != null)
                    obj.Rotation = float.Parse(node.Attributes["rotation"].Value, CultureInfo.InvariantCulture);

                result.Add(obj);
            }

            return result.ToArray();
        }
    }
}
