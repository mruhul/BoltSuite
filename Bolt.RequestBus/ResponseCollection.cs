using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bolt.RequestBus
{
    public interface IResponseCollection<TResult>
    {
        IResponse<TResult> MainResponse { get; }
        IEnumerable<IResponse<TResult>> OtherResponses { get; }
        void AddResponse(IResponse<TResult> response);
    }

    internal sealed class ResponseCollection<TResult> : IResponseCollection<TResult>
    {
        private List<IResponse<TResult>> _responses;
        
        public IResponse<TResult> MainResponse { get; set; }
        public IEnumerable<IResponse<TResult>> OtherResponses => _responses ?? Enumerable.Empty<IResponse<TResult>>();

        public void AddResponse(IResponse<TResult> rsp)
        {
            if(rsp == null) return;
            
            _responses ??= new List<IResponse<TResult>>();

            _responses.Add(rsp);
        }
    }
}