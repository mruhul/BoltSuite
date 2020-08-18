using Microsoft.Extensions.DependencyInjection;

namespace Bolt.FluentHttpClient.Fakes
{
    public static class FakeExtensions
    {
        public static IFluentHaveFakeResponseProvider FakeHttpClient(this IServiceScope scope)
        {
            return scope.ServiceProvider.GetRequiredService<IFakeResponseProvider>().When();
        }
    }
}
