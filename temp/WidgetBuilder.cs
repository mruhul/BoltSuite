using System;
using System.Collections.Generic;
using System.Text;

namespace Bolt.RequestBus.Widgets
{
    public interface IWidgetBuilderHaveStatus : IWidgetBuilderCollectWidgets, IWidgetBuilderSingleWidgetCollectName
    {
    }

    public interface IWidgetBuilderCollectRedirect
    {
        IWidgetBuilderHaveRedirect Redirect(IRedirectAction redirectAction);
    }

    public interface IWidgetBuilderHaveRedirect: IWidgetBuilderBuild
    {
    }

    public interface IWidgetBuilderCollectWidgets
    {
        IWidgetBuilderHaveWidgets WithWidget(IWidgetUnit widget);
        IWidgetBuilderHaveWidgets WithWidget(string name, string type, object data, int displayOrder = 0);
        IWidgetBuilderHaveWidgets WithWidgets(params IWidgetUnit[] widget);
    }

    public interface IWidgetBuilderHaveWidgets : IWidgetBuilderBuild
    {

    }

    public interface IWidgetBuilderBuild
    {
        IWidgetHandlerResponse Build();
    }

    public interface IWidgetBuilderSingleWidgetCollectName
    {
        IWidgetBuilderSingleWidgetHasName WithName(string name);
    }

    public interface IWidgetBuilderSingleWidgetHasName : IWidgetBuilderSingleWidgetCollectType
    {

    }

    public interface IWidgetBuilderSingleWidgetCollectType
    {
        IWidgetBuilderSingleWidgetHasType WithType(string type);
    }

    public interface IWidgetBuilderSingleWidgetHasType : IWidgetBuilderSingleWidgetCollectDisplayOrder, IWidgetBuilderSingleWidgetBuilder
    {

    }

    public interface IWidgetBuilderSingleWidgetCollectDisplayOrder
    {
        IWidgetBuilderSingleWidgetHasDisplayOrder WithDisplayOrder(int order);
    }

    public interface IWidgetBuilderSingleWidgetHasDisplayOrder : IWidgetBuilderSingleWidgetBuilder
    {

    }

    public interface IWidgetBuilderSingleWidgetBuilder
    {
        IWidgetHandlerResponse Build(object data);
    }


    public class WidgetBuilder : IWidgetBuilderHaveStatus, IWidgetBuilderHaveRedirect, IWidgetBuilderHaveWidgets, 
        IWidgetBuilderSingleWidgetHasName,
        IWidgetBuilderSingleWidgetHasType,
        IWidgetBuilderSingleWidgetHasDisplayOrder
    {
        private readonly WidgetHandlerResponse _rsp = new WidgetHandlerResponse();
        private readonly List<IWidgetUnit> _widgets = new List<IWidgetUnit>(1);
        private WidgetUnit _unit;

        private WidgetBuilder(int statusCode)
        {
            _rsp.StatusCode = statusCode;
        }

        private WidgetBuilder(IRedirectAction redirectAction)
        {
            _rsp.RedirectAction = redirectAction;
        }

        public static IWidgetBuilderHaveStatus WithStatus(int statusCode)
        {
            return new WidgetBuilder(statusCode);
        }

        public static IWidgetBuilderHaveStatus Ok()
        {
            return new WidgetBuilder(200);
        }
        
        public static IWidgetBuilderHaveStatus NotFound()
        {
            return new WidgetBuilder(404);
        }

        public static IWidgetBuilderHaveStatus Failed()
        {
            return new WidgetBuilder(500);
        }

        public static IWidgetBuilderHaveRedirect Redirect(IRedirectAction redirectAction)
        {
            return new WidgetBuilder(redirectAction);
        }


        public IWidgetBuilderHaveWidgets WithWidget(IWidgetUnit widget)
        {
            if (widget == null) return this;
            _widgets.Add(widget);
            return this;
        }

        public IWidgetBuilderHaveWidgets WithWidget(string name, string type, object data, int displayOrder)
        {
            return WithWidget(new WidgetUnit
            {
                Data = data,
                DisplayOrder = displayOrder,
                Name = name,
                Type = type
            });
        }

        public IWidgetBuilderHaveWidgets WithWidgets(params IWidgetUnit[] widgets)
        {
            if (widgets == null) return this;
            foreach (var widget in _widgets)
            {
                if(widget == null) continue;
                
                _widgets.Add(widget);
            }

            return this;
        }

        public IWidgetHandlerResponse Build()
        {
            _rsp.Widgets = _widgets;
            return _rsp;
        }

        public IWidgetBuilderSingleWidgetHasName WithName(string name)
        {
            _unit = new WidgetUnit {Name = name};
            return this;
        }

        public IWidgetBuilderSingleWidgetHasType WithType(string type)
        {
            _unit.Type = type;
            return this;
        }

        public IWidgetBuilderSingleWidgetHasDisplayOrder WithDisplayOrder(int order)
        {
            _unit.DisplayOrder = order;
            return this;
        }

        public IWidgetHandlerResponse Build(object data)
        {
            _unit.Data = data;
            _rsp.Widgets = new[] {_unit};
            return _rsp;
        }
    }
}
