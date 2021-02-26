using Netension.Request.Abstraction.Requests;
using System;

namespace Netension.Request.Abstraction.Resolvers
{
    /// <summary>
    /// Responsible for registrate <see cref="Senders.ICommandSender">IRequestSender</see>'s and <see cref="Senders.IQuerySender">IQuerySender</see>'s predicate for their key.
    /// </summary>
    /// <remarks>
    /// These registrations will be used for resolve the proper key of <see cref="Senders.ICommandSender">ICommandSender</see> or  <see cref="Senders.IQuerySender">IQuerySender</see> according to the <see cref="IRequest"/> to be send.<br/>
    /// </remarks>
    public interface IRequestSenderKeyRegister
    {
        /// <summary>
        /// Registrate key and predicate pair.
        /// </summary>
        /// <param name="key">Key of the <see cref="Senders.ICommandSender">ICommandSender</see> or  <see cref="Senders.IQuerySender">IQuerySender</see></param>
        /// <param name="predicate">Predicate of the key. If it is true the <see cref="IRequest"/> will be sent via the <see cref="Senders.ICommandSender">ICommandSender</see> or  <see cref="Senders.IQuerySender">IQuerySender</see></param>
        void Registrate(string key, Func<IRequest, bool> predicate);
    }
}
