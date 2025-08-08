namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Query object with QueryTarget.TimeElapsed. It returns the gpu duration in nanoseconds for commands executed during this query.
/// </summary>
public readonly struct GLQueryTimeElapsed : IDisposable, IEquatable<GLQueryTimeElapsed>
{
    public readonly GLQueryBase RawQuery;

    public GLQueryTimeElapsed()
    {
        RawQuery = new(QueryTarget.TimeElapsed);
    }

    public GLQueryTimeElapsed(GLQueryBase rawQuery)
    {
        RawQuery = rawQuery;
    }

    /// <summary>
    /// Starts measurement for this query
    /// </summary>
    public void Begin()
    {
        GL.BeginQuery(QueryTarget.TimeElapsed, RawQuery.Handle.Value);
    }

    /// <summary>
    /// Ends measurement for this query
    /// </summary>
    public void End()
    {
        GL.EndQuery(QueryTarget.TimeElapsed);
    }

    /// <summary>
    /// Gets the gpu duration in nanoseconds, for commands between <see cref="Begin"/> and <see cref="End"/> to finish execution
    /// </summary>
    /// <remarks>Synchronouly waits for the result to arrive</remarks>
    /// <returns>The gpu duration in nanoseconds</returns>
    public long GetResult()
    {
        return GL.GetQueryObjecti64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResult);
    }

    /// <summary>
    /// Gets the gpu duration in nanoseconds, for commands between <see cref="Begin"/> and <see cref="End"/> to finish execution
    /// </summary>
    /// <returns>The gpu duration in nanoseconds or zero if the result was not yet available</returns>
    public long GetResultIfAvailable()
    {
        return GL.GetQueryObjecti64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResultNoWait);
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
        return obj is GLQueryTimeElapsed gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawQuery.GetHashCode();
    }

    public static bool operator ==(GLQueryTimeElapsed left, GLQueryTimeElapsed right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLQueryTimeElapsed left, GLQueryTimeElapsed right)
    {
        return !(left == right);
    }

    public bool Equals(GLQueryTimeElapsed other)
    {
        return RawQuery.Equals(other.RawQuery);
    }
}
