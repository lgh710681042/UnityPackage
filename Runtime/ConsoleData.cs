	using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyCommandLine
{
    internal class PMMethodInfo
    {
        public MethodInfo info;
        public ConsoleCmdAttribute bute;
        public PMMethodInfo(MethodInfo _info, ConsoleCmdAttribute _bute)
        {
            info = _info;
            bute = _bute;
        }
    }
    internal class PMConsoleHead
    {
        public string key;
        public string descript;
        public MethodInfo method;
        public PMConsoleHead(string _key, string _descript, MethodInfo _method)
        {
            key = _key;
            descript = _descript;
            method = _method;
        }
    }
    internal class PMConsoleParam
    {
        public string key;
        public string descript;
        public PMConsoleParam(string _key, string _descript)
        {
            key = _key;
            descript = _descript;
        }
    }
    public enum LocalType
    {
        Non,
        FreeCamera,
        ShowFps
    }

    internal class ConsoleData
    {
        public List<PMMethodInfo> pmInfos = new List<PMMethodInfo>();
        public void InitData()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var item in assemblies)
            {
                var types = item.GetTypes();
                foreach (var temType in types)
                {
                    var methods = temType.GetMethods();
                    foreach (var method in methods)
                    {
                        foreach (var bute in method.GetCustomAttributes<ConsoleCmdAttribute>())
                        {
                            var list = new List<ConsoleCmdParam>();
                            foreach (var param in method.GetCustomAttributes<ConsoleCmdParam>())
                            {
                                list.Add(new ConsoleCmdParam(param.Name,param.Description, param.Value));
                            }
                            bute.ParamArray = list.ToArray();
                            pmInfos.Add(new PMMethodInfo(method, bute));
                        }
                    }
                }
            }
        }
		
        public PMMethodInfo GetInfoByIndex(int index)
        {
            return pmInfos[index];
        }
		
        public int Length()
        {
            return pmInfos.Count;
        }
    }
}
