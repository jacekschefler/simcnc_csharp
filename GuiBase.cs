using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static sim.ResultA;
namespace sim;

public class GuiBase
{
    public enum WidgetType
    {
        PushButton = 0,
        ToolButton = 1,
        ProgressBar = 2,
        LineEdit = 3,
        Dial = 4,
        Slider = 5,
        CheckBox = 6,
        Label = 7,
        OpenFileButton = 8,
        ToolButtonWithLed = 9,
        ToolButtonWithProgress = 10,
        VerticalSlider = 11,
        HorizontalSlider = 12,
        DigitalIOControl = 13,
        AnalogIOControl = 14,
        CurrentGcodesWidget = 15,
        MdiLineWidget = 16,
        PythonConsole = 17,
        GCodeList = 18,
        SimGLWidget = 19,
        OffsetTable = 20,
        GroupBox = 21,
        Frame = 22,
        TabWidget = 23,
        ScrollArea = 24,
        Splitter = 25
    }
    protected readonly Comm _comm;
    internal GuiBase(Comm comm)
    {
        _comm = comm;
    }

    protected IEnumerable<string> GetIds()
    {
        return _comm.SendAndWait("gui_getWidgetsId:").Split(';');
    }

    public class Widget
    {
        protected readonly Comm _comm;
        protected readonly string _name;
        public Widget(string name, Comm comm)
        {
            _name = name;
            _comm = comm;
        }

        public virtual void SetAttribute(string attribute, object value)
        {
            var _value = value switch
            {
                string _ => "str",
                bool _ => "bool",
                int _ => "int",
                uint _ => "int",
                float _ => "float",
                double _ => "float",
                _ => throw new NotImplementedException(),
            };
            _comm.SendAndWait($"gui_setAttribute:{_name}:{attribute}:{_value}:{value}");
        }
        public virtual ResultA GetAttribute(string attribute)
        {
            var result = _comm.SendAndWait($"gui_getAttribute:{_name}:{attribute}");
            var pos = result.IndexOf(':');
            if (pos < 0) throw new InvalidOperationException("Can not find char `:` in return value from sim cnc");
            var tt = result[..pos];
            var val = result[(pos + 1)..];
            ResultA return_value = tt switch
            {
                "int" => new IntResult(int.Parse(val)),
                "float" => new DoubleResult(double.Parse(val)),
                "bool" => new BoolResult(bool.Parse(val)),
                "str" => new StringResult(val.ToString()),
                _ => throw new NotImplementedException(),
            };
            return return_value;
        }
    }

    public class Label : Widget
    {

        public Label(string name, Comm comm) : base(name, comm)
        {
        }
        public static IEnumerable<IDictionary<string, string>> Attributes => new List<Dictionary<string, string>>()
        {
        new() { { "name", "Text" }, { "type", "str" } },
        };
        public string Text
        {
            get
            {
                var result = _comm.SendAndWait($"gui_getAttribute:{base._name}:Text");
                return result.Substring(result.IndexOf(':') + 1);
            }

            set => _comm.SendAndWait($"gui_setAttribute:{base._name}:Text:str:{value}");
        }
        public static WidgetType Type => WidgetType.Label;
    }

}





