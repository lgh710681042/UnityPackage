using System;

namespace MyCommandLine
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ConsoleCmdAttribute : Attribute
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public int ShortCut 
        { 
            set{
                _shortCut = value;
            }
            get{
                return _shortCut;
            } 
        }
        public int _shortCut = 0;
        public ConsoleCmdParam[] ParamArray;
        public ConsoleCmdAttribute(string _name,string _descript)
        {
            Name = _name;
            Description = _descript;
        }
    }
    public enum ConsoleType
    {
        Both,
        Button,
        Cmd
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ConsoleCmdParam : Attribute
    {
        public string Name { set; get; }
        public object Value { set; get; }
        public string Description { set; get; }
        public ConsoleCmdParam (string _name,string _descript,object _value)
        {
            Name = _name;
            Value = _value;
            Description = _descript;
        }
    }
}
