using System;
using UnityEngine;
using Unity.Profiling;

public class FrameTimingsHUDDisplay : MonoBehaviour
{
    GUIStyle m_Style;
    private int lasFrameIndex;
    private float[] frameDeltaTimeArray;
    readonly FrameTiming[] m_FrameTimings = new FrameTiming[50];

    private void Awake()
    {
        m_Style = new GUIStyle();
        m_Style.fontSize = 15;
        m_Style.normal.textColor = Color.white;
        frameDeltaTimeArray = new float[50];

    }

    void OnGUI()
    {
        CaptureTimings();

        var reportMsg =
        $"\nFPS: {1f/Time.unscaledDeltaTime:00}"+
        $"\nCPU: {m_FrameTimings[0].cpuFrameTime:00.0}" +
        $"\nMain Thread: {m_FrameTimings[0].cpuMainThreadFrameTime:00.0}" +
        $"\nRender Thread: {m_FrameTimings[0].cpuRenderThreadFrameTime:00.0}" +
        $"\nGPU: {m_FrameTimings[0].gpuFrameTime:00.0}";

        var oldColor = GUI.color;
        GUI.color = new Color(1, 1, 1, 1);
        float w = 300, h = 210;

        GUILayout.BeginArea(new Rect(32, 50, w, h), "Frame Stats", GUI.skin.window);
        GUILayout.Label(reportMsg, m_Style);
        GUILayout.EndArea();

        GUI.color = oldColor;
    }

    private void CaptureTimings()
    {
        FrameTimingManager.CaptureFrameTimings();
        FrameTimingManager.GetLatestTimings((uint)m_FrameTimings.Length, m_FrameTimings);
    }
}
