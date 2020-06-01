using System;
using Bolt.IocScanner.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Shouldly;

namespace Bolt.IocScanner.Tests
{
    public class IocScannerTests
    {
        private readonly IServiceProvider _sp;

        public IocScannerTests()
        {
            var sc = new ServiceCollection();
            sc.Scan<IocScannerTests>(new IocScannerOptions
            {
                SkipWhenAutoBindMissing = true
            });
            
            _sp = sc.BuildServiceProvider();
        }
        
        [Fact]
        public void AutoBind_Should_Bind_Implementation()
        {
            Should.NotThrow(() => _sp.GetRequiredService<IHelloWorld>());
        }
        
        [Fact]
        public void AutoBind_Should_Bind_As_Transient_Lifecycle()
        {
            _ = _sp.GetRequiredService<IHelloWorld>();
            _ = _sp.GetRequiredService<IHelloWorld>();
            
            HelloWorldImpl.InstanceCount.ShouldBe(2);
        }
        
        [Fact]
        public void AutoBind_Scoped_Should_Bind_As_Scoped_Lifecycle()
        {
            using var scope = _sp.CreateScope();
            
            _ = scope.ServiceProvider.GetRequiredService<IScopedHelloWorld>();
            _ = scope.ServiceProvider.GetRequiredService<IScopedHelloWorld>();
            _ = scope.ServiceProvider.GetRequiredService<IAnotherInterface>();
            
            ScopedHelloWorldImpl.InstanceCount.ShouldBe(1);
        }
        
        [Fact]
        public void AutoBind_Singleton_Should_Bind_As_Singleton_Lifecycle()
        {
            using var scope = _sp.CreateScope();
            
            _ = scope.ServiceProvider.GetRequiredService<ISingletonHelloWorld>();
            _ = scope.ServiceProvider.GetRequiredService<ISingletonHelloWorld>();
            _ = scope.ServiceProvider.GetRequiredService<IAnotherInterface>();
            
            using var scope2 = _sp.CreateScope();
            
            _ = scope2.ServiceProvider.GetRequiredService<ISingletonHelloWorld>();
            _ = scope2.ServiceProvider.GetRequiredService<ISingletonHelloWorld>();
            _ = scope2.ServiceProvider.GetRequiredService<IAnotherInterface>();

            
            SingletonHelloWorldImpl.InstanceCount.ShouldBe(1);
        }

        [Fact]
        public void Should_Execute_Impl_Of_ServiceRegistry()
        {
            Should.NotThrow(() => _sp.GetRequiredService<IForLoadViaServiceRegistry>());
            
        }
    }

    public class ServiceRegistryImpl : IServiceRegistry
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IForLoadViaServiceRegistry, LoadViaServiceRegistry>();
        }
    }
    
    public interface IForLoadViaServiceRegistry
    {
        
    }
    
    public class LoadViaServiceRegistry : IForLoadViaServiceRegistry
    {}
    
    public interface IHelloWorld{}

    [AutoBind]
    public class HelloWorldImpl : IHelloWorld
    {
        public static int InstanceCount = 0;

        public HelloWorldImpl()
        {
            InstanceCount++;
        }
    }
    
    public interface IScopedHelloWorld{}

    [AutoBind(LifeCycle.Scoped)]
    public class ScopedHelloWorldImpl : IScopedHelloWorld, IAnotherInterface
    {
        public static int InstanceCount = 0;

        public ScopedHelloWorldImpl()
        {
            InstanceCount++;
        }
    }
    
    public interface ISingletonHelloWorld{}
    public interface IAnotherInterface {}

    [AutoBind(LifeCycle.Singleton)]
    public class SingletonHelloWorldImpl : ISingletonHelloWorld, IAnotherInterface
    {
        public static int InstanceCount = 0;

        public SingletonHelloWorldImpl()
        {
            InstanceCount++;
        }
    }
}