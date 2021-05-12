using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bolt.RequestBus.Widgets
{
    public static class RequestBusExtensions
    {
        public static async Task<WidgetGroupResponse> WidgetResponseAsync<TRequest>(this IRequestBus bus, TRequest request)
        {
            var widgetsRsp = await bus.ResponsesAsync<TRequest, WidgetResponse>(request);

            return BuildWidgetGroupResponse<TRequest>(widgetsRsp);
        }
        
        public static WidgetGroupResponse WidgetResponse<TRequest>(this IRequestBus bus, TRequest request)
        {
            var widgetsRsp = bus.Responses<TRequest, WidgetResponse>(request);

            return BuildWidgetGroupResponse<TRequest>(widgetsRsp);
        }

        private static WidgetGroupResponse BuildWidgetGroupResponse<TRequest>(ResponseCollection<WidgetResponse> rsp)
        {
            var result = new WidgetGroupResponse();
            var mainRsp = rsp.MainResponse();
            if (mainRsp != null)
            {
                if (!mainRsp.IsSucceed || !StatusCodeHelper.IsSuccessful(mainRsp.StatusCode))
                {
                    result.StatusCode = mainRsp.StatusCode ?? 400;
                    result.Errors = mainRsp.Errors;
                    return result;
                }

                if (!string.IsNullOrWhiteSpace(mainRsp.Value?.RedirectAction?.Url))
                {
                    result.StatusCode = mainRsp.StatusCode ?? 302;
                    result.RedirectAction = mainRsp.Value.RedirectAction;
                    return result;
                }

                result.StatusCode = mainRsp.StatusCode ?? 200;
                
                result.AddResponse(BuildWidgetUnitResponse(mainRsp));
            }
            else
            {
                result.StatusCode = 200;
            }

            var otherRsp = rsp.OtherResponses();
            
            foreach (var otherResponse in otherRsp)
            {
                result.AddResponse(BuildWidgetUnitResponse(otherResponse));
            }

            return result;
        }

        private static WidgetUnitResponse BuildWidgetUnitResponse(Response<WidgetResponse> rsp)
        {
            return new WidgetUnitResponse
            {
                Data = rsp.Value?.Data,
                Name = rsp.Value?.Name,
                Type = rsp.Value?.Type,
                Errors = rsp.Errors,
                StatusCode = rsp?.StatusCode ?? 200,
                DisplayOrder = rsp.Value?.DisplayOrder ?? 0
            };
        }
    }
}