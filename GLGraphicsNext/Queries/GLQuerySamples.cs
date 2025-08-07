namespace GLGraphicsNext;
public readonly unsafe struct GLQuerySamples : IDisposable, IEquatable<GLQuerySamples>
{
    public readonly GLQueryBase RawQuery;

    public GLQuerySamples()
    {
        RawQuery = new(QueryTarget.SamplesPassed);
    }

    public GLQuerySamples(GLQueryBase rawQuery)
    {
        RawQuery = rawQuery;
    }

    /// <summary>
    /// Starts measurement for this query
    /// </summary>
    public void Begin()
    {
        GL.BeginQuery(QueryTarget.SamplesPassed, RawQuery.Handle.Value);
    }

    /// <summary>
    /// Ends measurement for this query
    /// </summary>
    public void End()
    {
        GL.EndQuery(QueryTarget.SamplesPassed);
    }

    /// <summary>
    /// Gets the samples that passed occlusion testing between <see cref="Begin"/> and <see cref="End"/>
    /// </summary>
    /// <remarks>Synchronouly waits for the result to arrive</remarks>
    /// <returns>Number of samples</returns>
    public ulong GetResult()
    {
        return GL.GetQueryObjectui64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResult);
    }

    public ulong GetResultIfAvailable()
    {
        return GL.GetQueryObjectui64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResultNoWait);
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
        return obj is GLQuerySamples gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawQuery.GetHashCode();
    }

    public static bool operator ==(GLQuerySamples left, GLQuerySamples right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLQuerySamples left, GLQuerySamples right)
    {
        return !(left == right);
    }

    public bool Equals(GLQuerySamples other)
    {
        return RawQuery.Equals(other.RawQuery);
    }
}
