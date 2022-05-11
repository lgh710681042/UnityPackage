using System.Text.RegularExpressions;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace MyCommandLine
{
    internal class ConsoleManager : MonoBehaviour
    {
        
        public ConsoleView pmView = null;
        private ConsoleData m_consoleData = new ConsoleData();
        
        private void Awake()
        {
            if (ConsolePM.GetInstance().ConsoleMgr != null)
            {
                Debug.LogError("ConsoleCmdModule：ConsolePM Duplicated!");
                return;
            }
            ConsolePM.GetInstance().ConsoleMgr = this;
            m_consoleData.InitData();
            
        }
        
        private void Start()
        {
            var rec = GetComponent<RectTransform>();
            rec.offsetMax = Vector2.zero;
            rec.offsetMin = Vector2.zero;
        }

        private void OnDestroy()
        {
            if (ConsolePM.GetInstance().ConsoleMgr == this)
            {
                ConsolePM.GetInstance().ConsoleMgr = null;
            }
        }

        public ConsoleData ConsoleData()
        {
            return m_consoleData;
        }
        
        public void TriggerPm(string cmd)
        {
            string head = "";
            var index = cmd.IndexOf(' ');
            if (index > 0)
            {
                head = cmd.Substring(0, index);
            }
            else
            {
                head = cmd;
            }
            foreach (var item in m_consoleData.pmInfos)
            {
                if (item.bute.Name.Equals(head, StringComparison.OrdinalIgnoreCase))
                {
                    //var reg = new Regex(@"\s-\w+\s(\w+)", RegexOptions.IgnoreCase);
                    var reg = new Regex(@"\s(\S+)", RegexOptions.IgnoreCase);
                    var maths = reg.Matches(cmd);
                    var methodParams = item.info.GetParameters();
                    var objs = maths.Count > 0 ? new object[maths.Count] : null;
                    for (var i = 0; i < maths.Count; i++)
                    {
                        var paramType = methodParams[i].ParameterType;
                        var value = maths[i].Groups[1].Value;
                        objs[i] = Convert.ChangeType(value, paramType);
                    }
                    item.info.Invoke(null, objs);
                }
            }
            
            HideView();
        }
        
        public void HideView()
        {
            pmView.ShowOrHideView(false);
        }

        public void ShowView()
        {
            pmView.ShowOrHideView(true);
        }
       
    }
}
