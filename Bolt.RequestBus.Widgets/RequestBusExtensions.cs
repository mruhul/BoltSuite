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

            return BuildWidgetGroupResponse(widgetsRsp);
        }
        
        public static WidgetGroupResponse WidgetResponse<TRequest>(this IRequestBus bus, TRequest request)
        {
            var widgetsRsp = bus.Responses<TRequest, WidgetResponse>(request);

            return BuildWidgetGroupResponse(widgetsRsp);
        }

        private static WidgetGroupResponse BuildWidgetGroupResponse(ResponseCollection<WidgetResponse> rsp)
        {
            var units = new List<Response<WidgetResponse>>();

            var mainRsp = rsp.MainResponse();
            if (mainRsp != null)
            {
                if (!mainRsp.IsSucceed)
                {   
                    var convertRspMain = Convert(rsp.OtherResponses().Prepend(mainRsp));

                    return new WidgetGroupResponse
                    {
                        Errors = mainRsp.Errors,
                        StatusCode = mainRsp.StatusCode ?? 400,
                        StatusReason = mainRsp.StatusReason,
                        Widgets = convertRspMain.Widgets,
                        RedirectAction = mainRsp.Value?.RedirectAction,
                        MetaData = convertRspMain.MetaData
                    };
                }

                if (!string.IsNullOrWhiteSpace(mainRsp.Value?.RedirectAction?.Url))
                {
                    var convertRspRedirect = Convert(new[] { mainRsp });

                    return new WidgetGroupResponse
                    {
                        RedirectAction = mainRsp.Value.RedirectAction,
                        StatusCode = mainRsp.StatusCode ?? 302,
                        StatusReason = mainRsp.StatusReason,
                        Widgets = convertRspRedirect.Widgets,
                        MetaData = convertRspRedirect.MetaData
                    };
                }
                
                units.Add(mainRsp);
            }

            var otherRsp = rsp.OtherResponses();

            units.AddRange(otherRsp);

            var convertRsp = Convert(units);

            return new WidgetGroupResponse
            {
                StatusCode = mainRsp?.StatusCode ?? 200,
                Widgets = convertRsp.Widgets,
                MetaData = convertRsp.MetaData
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

        private static (IEnumerable<WidgetUnitResponse> Widgets, Dictionary<string,object> MetaData) Convert(IEnumerable<Response<WidgetResponse>> responses)
        {
            var metaData = new Dictionary<string, object>();

            if (responses == null) 
                return (Enumerable.Empty<WidgetUnitResponse>(), metaData);

            var result = new List<WidgetUnitResponse>();
            var groups = new Dictionary<string, List<WidgetUnitResponse>>();

            foreach (var response in responses)
            {
                if(response.Value == null) continue;

                if(response.Value.Widgets == null) continue;

                var index = 0;

                foreach (var singleWidgetResponseDto in response.Value.Widgets)
                {
                    if (singleWidgetResponseDto == null) continue;
                    
                    if (string.IsNullOrWhiteSpace(singleWidgetResponseDto.Group) is false)
                    {
                        if (groups.ContainsKey(singleWidgetResponseDto.Group) is false)
                        {
                            groups[singleWidgetResponseDto.Group] = new List<WidgetUnitResponse>();
                        }

                        groups[singleWidgetResponseDto.Group].Add(new WidgetUnitResponse
                        {
                            Data = singleWidgetResponseDto.Data,
                            DisplayOrder = singleWidgetResponseDto.DisplayOrder,
                            Name = singleWidgetResponseDto.Name,
                            Type = singleWidgetResponseDto.Type,
                            StatusCode = response.StatusCode,
                            Errors = index == 0 ? response.Errors : null
                        });
                    }
                    else
                    {
                        result.Add(new WidgetUnitResponse
                        {
                            Data = singleWidgetResponseDto.Data,
                            DisplayOrder = singleWidgetResponseDto.DisplayOrder,
                            Name = singleWidgetResponseDto.Name,
                            Type = singleWidgetResponseDto.Type,
                            StatusCode = response.StatusCode,
                            Errors = index == 0 ? response.Errors : null
                        });
                    }

                    AppendMetaData(metaData, singleWidgetResponseDto);

                    index++;
                }
            }

            foreach (var grp in groups)
            {
                var widgetsInGroup = grp.Value.OrderBy(x => x.DisplayOrder).ToArray();

                result.Add(new WidgetUnitResponse
                {
                    Name = grp.Key,
                    DisplayOrder = widgetsInGroup.FirstOrDefault()?.DisplayOrder ?? 0,
                    Type = "WidgetGroup",
                    Widgets = widgetsInGroup
                });
            }

            return (result.OrderBy(x => x.DisplayOrder), metaData);
        }

        private static void AppendMetaData(Dictionary<string,object> metaData, SingleWidgetResponseDto rsp)
        {
            if (rsp.MetaData == null) return;

            foreach(var item in rsp.MetaData)
            {
                metaData[item.Key] = item.Value;
            }
        }
    }
}