﻿using Bolt.PubSub.RabbitMq.Publishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Bolt.PubSub.RabbitMq.Tests
{
    public class IocFixture
    {
        private IServiceProvider sp;

        public IocFixture()
        {
            sp = BuildServiceProvider(null);
        }

        private IServiceProvider BuildServiceProvider(Action<IServiceCollection> modfify)
        {
            var sc = new ServiceCollection();

            var config = new ConfigurationBuilder().Build();

            sc.AddScoped(c => NSubstitute.Substitute.For<IRabbitMqSettings>());
            sc.AddScoped(c => NSubstitute.Substitute.For<IRabbitMqPublisher>());
            sc.AddScoped(c => NSubstitute.Substitute.For<IUniqueId>());

            sc.AddRabbitMqPublisher(config);

            modfify?.Invoke(sc);

            return sc.BuildServiceProvider();
        }

        public IServiceScope CreateScope(Action<IServiceCollection> modify)
            => BuildServiceProvider(modify).CreateScope();

        public IServiceScope CreateScope() => sp.CreateScope();
    }

    [CollectionDefinition(nameof(IocFixtureCollection))]
    public class IocFixtureCollection : ICollectionFixture<IocFixture>
    {

    }

    [Collection(nameof(IocFixtureCollection))]
    public abstract class TestWithIoc: IDisposable
    {
        private readonly IocFixture fixture;
        private IServiceScope scope;
        
        public TestWithIoc(IocFixture fixture)
        {
            this.fixture = fixture;
            scope = fixture.CreateScope();
        }

        protected T GetService<T>() => scope.ServiceProvider.GetRequiredService<T>();

        protected IServiceScope CreateScope(Action<IServiceCollection> setup)
            => fixture.CreateScope(setup);

        public void Dispose()
        {
            scope?.Dispose();
        }
    }
}
