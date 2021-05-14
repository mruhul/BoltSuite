using System.Collections.Generic;

namespace Bolt.RequestBus.Widgets
{
    public interface IWidgetBuilderCollectName
    {
        IWidgetBuilderHaveName WithName(string name);
    }

    public interface IWidgetBuilderHaveName : IWidgetBuilderCollectType
    {

    }

    public interface IWidgetBuilderCollectType
    {
        IWidgetBuilderHaveType WithType(string type);
    }

    public interface IWidgetBuilderHaveType : IWidgetBuilderBuild, IWidgetBuilderCollectDisplayOrder
    {

    }

    public interface IWidgetBuilderCollectDisplayOrder
    {
        IWidgetBuilderHaveDisplayOrder WithDisplayOrder(int displayOrder);
    }

    public interface IWidgetBuilderHaveDisplayOrder : IWidgetBuilderBuild
    {

    }

    public interface IWidgetBuilderBuild
    {
        WidgetResponse Build(object data);
    }

    public sealed class WidgetBuilder : IWidgetBuilderHaveName, IWidgetBuilderHaveType, IWidgetBuilderHaveDisplayOrder
    {
        private readonly string _name;
        private string _type;
        private int _displayOrder;

        private WidgetBuilder(string name) => _name = name;

        
        public static IWidgetBuilderHaveName WithName(string name)
        {
            return new WidgetBuilder(name);
        }

        public IWidgetBuilderHaveType WithType(string type)
        {
            _type = type;
            return this;
        }

        public IWidgetBuilderHaveDisplayOrder WithDisplayOrder(int displayOrder)
        {
            _displayOrder = displayOrder;
            return this;
        }

        public static WidgetResponse Redirect(RedirectAction redirectAction)
        {
            return new()
            {
                RedirectAction = redirectAction
            };
        }

        public static WidgetResponse Redirect(string url, bool? isPermanent = null)
            => Redirect(new RedirectAction
            {
                IsPermanent = isPermanent ?? false,
                Url = url
            });

        public static WidgetResponse WithWidgets(params SingleWidgetResponseDto[] widgets)
            => new()
            {
                Widgets = widgets
            };

        public static WidgetResponse WithWidgets(IEnumerable<SingleWidgetResponseDto> widgets)
            => new()
            {
                Widgets = widgets
            };

        public WidgetResponse Build(object data)
        {
            return new()
            {
                Widgets = new[] {new SingleWidgetResponseDto
                {
                    Data = data,
                    Type = _type,
                    Name = _name,
                    DisplayOrder = _displayOrder
                }}
            };
        }
    }


}