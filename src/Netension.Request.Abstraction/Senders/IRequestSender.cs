namespace Netension.Request.Abstraction.Senders
{
    /// <summary>
    /// Responsible for routing and sending <see cref="Requests.IRequest">IRequest</see>.
    /// </summary>
    /// <remarks>
    /// Routing works based on the <see cref="Resolvers.IRequestSenderKeyResolver">IRequestSenderKeyResolver</see>.
    /// </remarks>
    /// <inheritdoc cref="ICommandSender"/>
    /// <inheritdoc cref="IQuerySender"/>
    public interface IRequestSender : IQuerySender, ICommandSender
    {
    }
}
