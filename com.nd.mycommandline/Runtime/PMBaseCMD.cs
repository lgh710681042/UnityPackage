using UnityEngine;

namespace MyCommandLine
{
    internal class PMBaseCMD : MonoBehaviour
    {
        bool isDown;
        static bool isUnlimitedFrameSet = false;
        static int m_defaultFrameRate;
        static int m_defaultVSyncCount;
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        // Update is called once per frame
        private void Awake()
        {
        }

        private void Update()
        {

            if (Input.GetKey(KeyCode.Return) && Input.GetKey(KeyCode.LeftControl)
                && Input.GetKey(KeyCode.LeftAlt))
            {
                if (isDown)
                    return;
                isDown = true;

                ConsolePM.GetInstance().ShowView();
            }

            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftAlt)
               || Input.GetKeyUp(KeyCode.Return))
            {
                isDown = false;
            }

            if (Input.touchCount == 4)
            {
                ConsolePM.GetInstance().ShowView();
            }

        }

        [ConsoleCmd("DeleteConsoleCache", "删除缓存的命令")]
        public static void DeleteConsoleCache()
        {
            ConsolePM.GetInstance().ClearCMDCache();
        }

        [ConsoleCmd("ShowFPS", "显示帧率")]
        public static void ShowFPS()
        {
            ConsolePM.GetInstance().ShowFPS(isUnlimitedFrameSet);
        }

        [ConsoleCmd("SetUnlimitedFrame", "帧率无上限")]
        public static void SetUnlimitedFrame()
        {
            if (isUnlimitedFrameSet)
            {
#if !UNITY_IOS
                QualitySettings.vSyncCount = m_defaultVSyncCount;
#endif
                Application.targetFrameRate = m_defaultFrameRate;
            }
            else
            {
#if !UNITY_IOS
                m_defaultVSyncCount = QualitySettings.vSyncCount;
#endif
                m_defaultFrameRate = Application.targetFrameRate;
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 9999;
            }

            isUnlimitedFrameSet = !isUnlimitedFrameSet;

            GameObject fpsGo = GameObject.Find("FPSGo");
            if (fpsGo != null)
            {
                StatFPS statFPSCpt = fpsGo.GetComponent<StatFPS>();
                if (statFPSCpt != null)
                {
                    statFPSCpt.UnlimitedFrameState = isUnlimitedFrameSet;
                }
            }
        }
    }
}
