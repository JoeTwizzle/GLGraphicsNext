namespace GLGraphicsNext;
public readonly struct GLQueryAnySamples : IDisposable, IEquatable<GLQueryAnySamples>
{
    public readonly GLQueryBase RawQuery;

    public GLQueryAnySamples()
    {
        RawQuery = new(QueryTarget.AnySamplesPassed);
    }

    public GLQueryAnySamples(GLQueryBase rawQuery)
    {
        RawQuery = rawQuery;
    }

    /// <summary>
    /// Starts measurement for this query
    /// </summary>
    public void Begin()
    {
        GL.BeginQuery(QueryTarget.AnySamplesPassed, RawQuery.Handle.Value);
    }

    /// <summary>
    /// Ends measurement for this query
    /// </summary>
    public void End()
    {
        GL.EndQuery(QueryTarget.AnySamplesPassed);
    }

    /// <summary>
    /// Gets if any sample passed occlusion testing, between <see cref="Begin"/> and <see cref="End"/>
    /// </summary>
    /// <remarks>Synchronouly waits for the result to arrive</remarks>
    /// <returns>If any sample passed</returns>
    public bool GetResult()
    {
        return GL.GetQueryObjectui64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResult) != 0;
    }

    /// <summary>
    /// Gets if any sample passed occlusion testing, between <see cref="Begin"/> and <see cref="End"/>
    /// </summary>
    /// <returns>If any sample passed or false if the result was not yet available</returns>
    public bool GetResultIfAvailable()
    {
        return GL.GetQueryObjectui64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResultNoWait) != 0;
    }

    /// <summary>
    /// If the result is available to be read using <see cref="GetResultIfAvailable()"/>
    /// </summary>
    /// <returns>Boolean indicating if a result is available</returns>
    public bool IsResultAvailable()
    {
        return GL.GetQueryObjecti64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResultAvailable) != 0;
    }

    public void Dispose()
    {
        RawQuery.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLQueryAnySamples gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawQuery.GetHashCode();
    }

    public static bool operator ==(GLQueryAnySamples left, GLQueryAnySamples right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLQueryAnySamples left, GLQueryAnySamples right)
    {
        return !(left == right);
    }

    public bool Equals(GLQueryAnySamples other)
    {
        return RawQuery.Equals(other.RawQuery);
    }
}
