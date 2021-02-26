namespace Netension.Request.Abstraction.Requests
{
    /// <summary>
    /// Base type of commands.
    /// </summary>
    /// <remarks>
    /// Implement <see cref="IRequest"/>.<br/>
    /// Commands modify the status of the application, but they do not have result value (Only set).
    /// </remarks>
    /// <example>
    /// CreateUserCommand - Create a new user.
    /// </example>
    public interface ICommand : IRequest
    {
    }
}
