using System;

namespace Bolt.PubSub
{
    public interface IMessageSerializer
    {
        byte[] Serialize<T>(T content);
        T Deserialize<T>(ReadOnlySpan<byte> content);
        bool IsApplicable(string contentType);
    }
}
