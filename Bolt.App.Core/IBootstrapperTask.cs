using System.Threading.Tasks;

namespace Bolt.App.Core
{
    /// <summary>
    /// An interface to run any code when application starts. So instead of adding code in startup
    /// we can just bind new instances of <see cref="IBootstrapperTask"/> to run different startup tasks
    /// </summary>
    public interface IBootstrapperTask
    {
        Task RunAsync();
    }
}