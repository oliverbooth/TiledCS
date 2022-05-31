using System.Collections.Generic;

namespace TiledCS;

/// <summary>
///     Represents a property object in both tilesets, maps, layers and objects. Values are all in string but you can use the 'type'
///     property for conversions.
/// </summary>
public sealed class TiledProperty
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

    /// <summary>
    ///     Implicitly converts this property a <see cref="KeyValuePair{TKey,TValue}" /> with <see cref="string" /> key and value.
    /// </summary>
    /// <param name="property">The property to convert.</param>
    /// <returns>The converted <see cref="KeyValuePair{TKey,TValue}" />.</returns>
    public static implicit operator KeyValuePair<string, string>(TiledProperty property)
    {
        return new KeyValuePair<string, string>(property.Name, property.Value);
    }
}
