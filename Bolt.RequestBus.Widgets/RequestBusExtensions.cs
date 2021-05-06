using System.Threading.Tasks;

namespace Bolt.RequestBus.Widgets
{
    public static class RequestBusExtensions
    {
        public static async Task<IWidgetGroupResponse> WidgetResponseAsync<TRequest>(this IRequestBus bus, TRequest request)
        {
            var widgetsRsp = await bus.ResponsesAsync<TRequest, IWidgetResponse>(request);

            return BuildWidgetGroupResponse<TRequest>(widgetsRsp);
        }
        
        public static IWidgetGroupResponse WidgetResponse<TRequest>(this IRequestBus bus, TRequest request)
        {
            var widgetsRsp = bus.Responses<TRequest, IWidgetResponse>(request);

            return BuildWidgetGroupResponse<TRequest>(widgetsRsp);
        }

        private static IWidgetGroupResponse BuildWidgetGroupResponse<TRequest>(IResponseCollection<IWidgetResponse> rsp)
        {
            var result = new WidgetGroupResponse();

            if (rsp.MainResponse != null)
            {
                if (!rsp.MainResponse.IsSucceed || !StatusCodeHelper.IsSuccessful(rsp.MainResponse.Value?.StatusCode))
                {
                    result.StatusCode = rsp.MainResponse.Value?.StatusCode ?? 400;
                    result.Errors = rsp.MainResponse.Errors;
                    return result;
                }

                if (!string.IsNullOrWhiteSpace(rsp.MainResponse.Value?.RedirectAction?.Url))
                {
                    result.StatusCode = rsp.MainResponse.Value.StatusCode;
                    result.RedirectAction = rsp.MainResponse.Value.RedirectAction;
                    return result;
                }

                result.StatusCode = rsp.MainResponse.Value?.StatusCode ?? 200;
                
                result.AddResponse(BuildWidgetUnitResponse(rsp.MainResponse));
            }
            else
            {
                result.StatusCode = 200;
            }

            if (rsp.OtherResponses == null) return result;

            foreach (var otherResponse in rsp.OtherResponses)
            {
                result.AddResponse(BuildWidgetUnitResponse(otherResponse));
            }

            return result;
        }

        private static IWidgetUnitResponse BuildWidgetUnitResponse(IResponse<IWidgetResponse> rsp)
        {
            return new WidgetUnitResponse
            {
                Data = rsp.Value?.Data,
                Name = rsp.Value?.Name,
                Type = rsp.Value?.Type,
                Errors = rsp.Errors,
                StatusCode = rsp.Value?.StatusCode ?? 200,
                DisplayOrder = rsp.Value?.DisplayOrder ?? 0
            };
        }
    }
}