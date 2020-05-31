using System.Collections.Generic;

namespace Bolt.RequestBus
{
    public static class ResponseCollectionExtensions
    {
        public static IEnumerable<IResponse<TResult>> ReadAll<TResult>(this IResponseCollection<TResult> source)
        {
            if(source == null) yield break;

            if (source.MainResponse != null) yield return source.MainResponse;
            
            if(source.OtherResponses == null) yield break;

            foreach (var otherResponse in source.OtherResponses)
            {
                yield return otherResponse;
            }
        }
    }
}