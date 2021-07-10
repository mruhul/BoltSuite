using System.Threading.Tasks;

namespace Bolt.PubSub
{
    public interface IMessageSubscriber
    {
        Task Start();
    }
}
