using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;

namespace MyCommandLine
{
    internal class StatFPS : MonoBehaviour
    {
        public int FontSize = 16;
       
        public bool ShowDetail = true;
        public bool UnlimitedFrameState = false;

        public float ThresholdFPS = 24f;
        private bool m_showDetailStyle;
        private int m_nBaseFontSize = 16;

        private Rect m_rectStart = new Rect(10, 20, 140, 40); // The rect the window is initially displayed at.
        private Rect m_rectLabel = new Rect(0, 0, 140, 40);
        private bool m_bAllowDrag = true; // Do you want to allow the dragging of the FPS window
        private GUIStyle m_guiStyle; // The style the text will be displayed at, based en defaultSkin.label.
        private RectOffset m_padding;

        ProfilerRecorder mainThreadTimeRecorder;
        private static float m_mainThreadFrameTime = 0.0f;
        private static float m_fps = 0.0f;

        private void Awake()
        {
            m_showDetailStyle = ShowDetail;
            DontDestroyOnLoad(transform.gameObject);
        }

        private void ResizeGUI(bool bForce = false)
        {
            if (m_showDetailStyle != ShowDetail || bForce)
            {
                m_showDetailStyle = ShowDetail;
                var ratio = 1.0f * FontSize / m_nBaseFontSize;
                
                if (ShowDetail)
                {
                    m_rectStart = new Rect(0, 0, 220 * ratio, 100 * ratio);
                }
                else
                {
                    m_rectStart = new Rect(10, 20, 100 * ratio, 40 * ratio);
                }
                
                m_rectLabel = new Rect(0, 0, m_rectStart.width, m_rectStart.height);
            }
        }
        
        void InitGUI()
        {
            ResizeGUI(true);
            
            m_padding = new RectOffset(10, 10, 10, 10);
            m_guiStyle = new GUIStyle(GUI.skin.label);
            m_guiStyle.normal.textColor = Color.white;
            m_guiStyle.alignment = TextAnchor.MiddleLeft;
            m_guiStyle.fontSize = FontSize;
            m_guiStyle.padding = m_padding;
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        }

        void OnDisable()
        {
            mainThreadTimeRecorder.Dispose();
        }

        private void Update()
        {
            ResizeGUI();
        } 

        void OnGUI()
        {
            if (m_guiStyle == null)
            {
                InitGUI();
            }

            m_mainThreadFrameTime = (float)(GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-9f));
            m_fps = 1.0f / Time.smoothDeltaTime;
            bool bLowFPS = false;
            if (m_fps < ThresholdFPS)
            {
                bLowFPS = true;
            }

            m_guiStyle.normal.textColor = bLowFPS ? Color.red : Color.white;
            m_rectStart = GUI.Window(0, m_rectStart, StatsWindow, "");
        }

        static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            var samples = new List<ProfilerRecorderSample>(samplesCount);
            recorder.CopyTo(samples);
            for (var i = 0; i < samples.Count; ++i)
                r += samples[i].Value;
            r /= samplesCount;

            return r;
        }

        void StatsWindow(int windowID)
        {
            string switchflag = UnlimitedFrameState ? "on" : "off";
            GUI.Label(m_rectLabel, string.Format("FPS: {0:F2}\nMainThread: {1:F2}ms\nUnlimitedFps: {2}",
                m_fps, m_mainThreadFrameTime * 1000, switchflag), m_guiStyle);

            if (m_bAllowDrag)
            {
                GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
            }
        }
    }
}
