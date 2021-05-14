using System.Collections.Generic;
using System.Linq;
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
            var units = new List<WidgetUnitResponse>();

            var mainRsp = rsp.MainResponse();
            if (mainRsp != null)
            {
                if (!mainRsp.IsSucceed || !StatusCodeHelper.IsSuccessful(mainRsp.StatusCode))
                {
                    return new WidgetGroupResponse
                    {
                        Errors = mainRsp.Errors,
                        StatusCode = mainRsp.StatusCode ?? 400,
                        StatusReason = mainRsp.StatusReason
                    };
                }

                if (!string.IsNullOrWhiteSpace(mainRsp.Value?.RedirectAction?.Url))
                {
                    return new WidgetGroupResponse
                    {
                        RedirectAction = mainRsp.Value.RedirectAction,
                        StatusCode = mainRsp.StatusCode ?? 302,
                        StatusReason = mainRsp.StatusReason
                    };
                }


                units.AddRange(BuildWidgetUnitResponse(mainRsp));
            }

            var otherRsp = rsp.OtherResponses();
            
            foreach (var otherResponse in otherRsp)
            {
                units.AddRange(BuildWidgetUnitResponse(otherResponse));
            }

            return new WidgetGroupResponse
            {
                StatusCode = mainRsp?.StatusCode ?? 200,
                Widgets = units
            };
        }

        private static IEnumerable<WidgetUnitResponse> BuildWidgetUnitResponse(Response<WidgetResponse> rsp)
        {
            var widgets = rsp?.Value?.Widgets;

            if(widgets == null) yield break;
            
            var index = 0;

            foreach (var widget in widgets)
            {
                if(widget == null) continue;

                yield return new WidgetUnitResponse
                {
                    Data = widget.Data,
                    DisplayOrder = widget.DisplayOrder,
                    Name = widget.Name,
                    Type = widget.Type,
                    StatusCode = rsp.StatusCode,
                    Errors = index == 0 ? rsp.Errors : null
                };

                index++;
            }
        }
    }
}