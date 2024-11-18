using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

public class profiler : MonoBehaviour
{
    GUIStyle m_Style;
    private float currentFps;
    private float currentFtime;
    private float smoothedFps;
    private float smoothedFtime;
    private float smoothingFactor = 1f; // Adjust this to control how much weight is given to recent FPS
    private int targetFPS = 60;
    readonly FrameTiming[] m_FrameTimings = new FrameTiming[1];

    private IEnumerator Start()
    {
        m_Style = new GUIStyle();
        m_Style.fontSize = 15;
        m_Style.normal.textColor = Color.white;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;

        GUI.depth = 2;
        while (true)
        {
            currentFps = 1f / Time.unscaledDeltaTime;
            currentFtime = (float)m_FrameTimings[0].cpuFrameTime;

            //Apply exponential smoothing to calculate the weighted average
            smoothedFps = (smoothingFactor * currentFps) + (1f - smoothingFactor) * smoothedFps;
            smoothedFtime = (smoothingFactor * currentFtime) + (1f - smoothingFactor) * smoothedFtime;


            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnGUI()
    {
        CaptureTimings();

        // Display the smoothed FPS
        var reportMsg =
        $"\nFPS: {Mathf.Round(smoothedFps)}" +
        $"\nFrameTime (CPU): {smoothedFtime:0.0}ms";

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
