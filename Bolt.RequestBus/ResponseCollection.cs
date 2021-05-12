using System.Collections.Generic;
using System.Linq;

namespace Bolt.RequestBus
{
    public record ResponseCollection<TValue>
    {
        public ICollection<ResponseUnit<TValue>> Responses { get; init; }

        public Response<TValue> MainResponse()
            => Responses.FirstOrDefault(x => x.IsMainResponse)?.Response;

        public IEnumerable<Response<TValue>> OtherResponses()
            => Responses.Where(x => !x.IsMainResponse).Select(x => x.Response) ?? Enumerable.Empty<Response<TValue>>();

        public void AddResponse(Response<TValue> response, bool isMainResponse = false)
        {
            Responses.Add(new ResponseUnit<TValue>
            {
                IsMainResponse = isMainResponse,
                Response = response
            });
        }
    }

    public record ResponseUnit<TValue>
    {
        public Response<TValue> Response { get; init; }
        public bool IsMainResponse { get; init; }
    }
}