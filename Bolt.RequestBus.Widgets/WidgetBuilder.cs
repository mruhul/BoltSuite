using System.Collections.Generic;

namespace Bolt.RequestBus.Widgets
{
    public interface IWidgetBuilderCollectName
    {
        IWidgetBuilderHaveName WithName(string name);
    }

    public interface IWidgetBuilderHaveName : IWidgetBuilderCollectType, IWidgetBuilderCollectGroupName
    {

    }

    public interface IWidgetBuilderCollectGroupName
    {
        IWidgetBuilderHaveGroupName WithGroup(string groupName);
    }

    public interface IWidgetBuilderHaveGroupName : IWidgetBuilderCollectType
    {

    }

    public interface IWidgetBuilderCollectType
    {
        IWidgetBuilderHaveType WithType(string type);
    }

    public interface IWidgetBuilderHaveType : IWidgetBuilderBuildWithData, IWidgetBuilderCollectData, IWidgetBuilderCollectDisplayOrder
    {

    }

    public interface IWidgetBuilderCollectDisplayOrder
    {
        IWidgetBuilderHaveDisplayOrder WithDisplayOrder(int displayOrder);
    }

    public interface IWidgetBuilderHaveDisplayOrder : IWidgetBuilderBuildWithData, IWidgetBuilderCollectData
    {

    }

    public interface IWidgetBuilderCollectData
    {
        IWidgetBuilderHaveData WithData(object data);
    }

    public interface IWidgetBuilderHaveData : IWidgetBuilderBuild, IWidgetBuilderCollectWidgets
    {

    }

    public interface IWidgetBuilderBuild
    {
        WidgetResponse Build();
    }


    public interface IWidgetBuilderCollectWidgets
    {
        IWidgetBuilderHaveName AnotherWithName(string name);
    }
    

    public interface IWidgetBuilderBuildWithData
    {
        WidgetResponse Build(object data);
    }

    public sealed class WidgetBuilder : IWidgetBuilderHaveName, IWidgetBuilderHaveType, IWidgetBuilderHaveDisplayOrder, IWidgetBuilderHaveGroupName,
        IWidgetBuilderHaveData
    {
        private string _name;
        private string _type;
        private string _group;
        private int _displayOrder;
        private object _data;
        private List<SingleWidgetResponseDto> _widgets;

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

        public IWidgetBuilderHaveGroupName WithGroup(string groupName)
        {
            _group = groupName;
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
            return WithData(data).Build();
        }

        public IWidgetBuilderHaveData WithData(object data)
        {
            _data = data;
            return this;
        }

        public WidgetResponse Build()
        {
            var widget = new SingleWidgetResponseDto
            {
                Data = _data,
                Type = _type,
                Group = _group,
                Name = _name,
                DisplayOrder = _displayOrder
            };

            if (_widgets == null)
            {
                return new()
                {
                    Widgets = new[]
                    {
                        widget
                    }
                };
            }

            _widgets.Add(widget);

            return new()
            {
                Widgets = _widgets
            };
        }

        public IWidgetBuilderHaveName AnotherWithName(string name)
        {
            _widgets ??= new List<SingleWidgetResponseDto>();

            _widgets.Add(new SingleWidgetResponseDto
            {
                Data = _data,
                Type = _type,
                Group = _group,
                Name = _name,
                DisplayOrder = _displayOrder
            });

            _group = null;
            _data = null;
            _name = null;
            _type = null;
            _displayOrder = 0;

            _name = name;
            return this;
        }
    }


}