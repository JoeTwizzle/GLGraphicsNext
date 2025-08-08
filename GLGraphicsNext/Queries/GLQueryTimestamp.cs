namespace GLGraphicsNext;

/// <summary>
/// An OpenGL Query object with QueryTarget.Timestamp. It returns the gpu timestamp in nanoseconds, when the gpu has completed all commands, before <see cref="FetchCompletionTimestamp"/> was called
/// </summary>
public readonly unsafe struct GLQueryTimestamp : IDisposable, IEquatable<GLQueryTimestamp>
{
    public readonly GLQueryBase RawQuery;

    public GLQueryTimestamp()
    {
        RawQuery = new((QueryTarget)All.Timestamp);
    }

    public GLQueryTimestamp(GLQueryBase rawQuery)
    {
        RawQuery = rawQuery;
    }

    public void FetchCompletionTimestamp()
    {
        GL.QueryCounter(RawQuery.Handle.Value, QueryCounterTarget.Timestamp);
    }

    /// <summary>
    /// Gets the gpu timestamp in nanoseconds, when the gpu has completed all commands, before <see cref="FetchCompletionTimestamp"/> was called
    /// </summary>
    /// <remarks>Synchronouly waits for the result to arrive</remarks>
    /// <returns>The gpu duration in nanoseconds</returns>
    public long GetCompletionTimestampResult()
    {
        return GL.GetQueryObjecti64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResult);
    }

    /// <summary>
    /// Gets the gpu timestamp in nanoseconds, when the gpu has completed all commands, before <see cref="FetchCompletionTimestamp"/> was called
    /// </summary>
    /// <returns>The gpu duration in nanoseconds or zero if the result was not yet available</returns>
    public long GetCompletionTimestampResultIfAvailable()
    {
        return GL.GetQueryObjecti64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResultNoWait);
    }

    /// <summary>
    /// If the result is available to be read using <see cref="GetCompletionTimestampResultIfAvailable()"/>
    /// </summary>
    /// <returns>Boolean indicating if a result is available</returns>
    public bool IsCompletionTimestampResultAvailable()
    {
        return GL.GetQueryObjecti64(RawQuery.Handle.Value, QueryObjectParameterName.QueryResultAvailable) != 0;
    }

    /// <summary>
    /// Gets the gpu timestamp in nanoseconds, when the gpu has started work on all commands, before <see cref="FetchCompletionTimestamp"/> was called
    /// </summary>
    /// <remarks>Synchronouly waits for the result to arrive</remarks>
    /// <returns>The gpu duration in nanoseconds</returns>
    public long GetIssueTimestampResult()
    {
        long res;
        GL.GetInteger64v(GetPName.Timestamp, &res);
        return res;
    }

    public void Dispose()
    {
        RawQuery.Dispose();
    }

    public override bool Equals(object? obj)
    {
        return obj is GLQueryTimestamp gl && Equals(gl);
    }

    public override int GetHashCode()
    {
        return RawQuery.GetHashCode();
    }

    public static bool operator ==(GLQueryTimestamp left, GLQueryTimestamp right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(GLQueryTimestamp left, GLQueryTimestamp right)
    {
        return !(left == right);
    }

    public bool Equals(GLQueryTimestamp other)
    {
        return RawQuery.Equals(other.RawQuery);
    }
}
