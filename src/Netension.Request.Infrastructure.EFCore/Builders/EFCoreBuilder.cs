using LightInject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Request.Infrastructure.EFCore.Abstractions;
using Netension.Request.Infrastructure.EFCore.Managers;
using System;

namespace Netension.Request.Infrastructure.EFCore.Builders
{
    public class EFCoreBuilder
    {
        public IHostBuilder HostBuilder { get; }

        public EFCoreBuilder(IHostBuilder hostBuilder)
        {
            HostBuilder = hostBuilder;
        }

        public void AddDatabase<TContext>(Action<IServiceProvider, DbContextOptionsBuilder> configure)
            where TContext : DbContext
        {
            HostBuilder.ConfigureServices(services => services.AddDbContext<TContext>(configure));
            HostBuilder.ConfigureContainer<IServiceContainer>(container =>
            {
                container.RegisterScoped<ITransactionManager>(factory => new TransactionManager(factory.GetInstance<TContext>().Database));
            });
        }
    }
}
