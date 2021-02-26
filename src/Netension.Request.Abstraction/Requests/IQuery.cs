namespace Netension.Request.Abstraction.Requests
{
    /// <summary>
    /// Base type of the queries.
    /// </summary>
    /// <remarks>
    /// Implement <see cref="IRequest"/>.<br/>
    /// Queries do not modify the status of the application, but they have result (Only get).
    /// </remarks>
    /// <typeparam name="TResponse"></typeparam>
    /// <example>
    /// CreateUserQuery - Get detail of a user.
    /// </example>
    public interface IQuery<TResponse> : IRequest
    {
    }
}
