using System;

namespace TiledCS;

/// <summary>
///     Represents an exception only thrown by TiledCS
/// </summary>
public class TiledException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TiledException" /> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TiledException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TiledException" /> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">The inner exception.</param>
    public TiledException(string message, Exception inner) : base(message, inner)
    {
    }
}
