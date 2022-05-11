
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCommandLine
{
    internal class ConsolePM
    {
        private static ConsolePM m_instance = null;
        
        public static ConsolePM GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new ConsolePM();
            }
            
            return m_instance;
        }
        
        private static ConsoleManager m_consoleManager = null;

        internal ConsoleManager ConsoleMgr
        {
            set
            {
                m_consoleManager = value;
            }

            get
            {
                return m_consoleManager;
            }
        }
        
        /// <summary>
        /// 清理指令缓存
        /// </summary>
        public void ClearCMDCache()
        {
            if (m_consoleManager == null || m_consoleManager.pmView == null) return;
            m_consoleManager.pmView.DeleteCache();
        }

        public void ShowView()
        {
            if (m_consoleManager == null) return;
            m_consoleManager.ShowView();
        }

        public void HideView()
        {
            if (m_consoleManager == null) return;
            m_consoleManager.HideView();
        }

        public void ShowFPS(bool isUnlimitedFrameSet)
        {
            GameObject fpsGo = GameObject.Find("FPSGo");
            if (fpsGo == null)
            {
                fpsGo = new GameObject("FPSGo");
                StatFPS statFPSCpt = fpsGo.AddComponent<StatFPS>();
                if (statFPSCpt != null)
                {
                    statFPSCpt.UnlimitedFrameState = isUnlimitedFrameSet;
                }
            }
            else
            {
                GameObject.Destroy(fpsGo);
            }
        }  
    }
}
