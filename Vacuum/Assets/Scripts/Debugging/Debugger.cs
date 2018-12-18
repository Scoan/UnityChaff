using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Drawing;
using _Color;


struct DebugLine
{
    public Vector3 m_start;
    public Vector3 m_end;
    public Color m_color;
    public float m_width;

    public DebugLine(Vector3 start, Vector3 end, Color color, float width)
    {
        m_start = start;
        m_end = end;
        m_color = color;
        m_width = width;
    }
}

struct DebugText
{
    public string m_text;
    public Vector3 m_pos;
  
    public DebugText(string text, Vector3 pos)
    {
        m_text = text;
        m_pos = pos; 
    }
}


public class Debugger : MonoBehaviour {
    /// <summary>
    ///  Singleton component for drawing debugging information to the screen.
    ///  Don't attach this to anything; it will create itself on start through the magic of [RuntimeInitializeOnLoadMethod]RequestDebugger()
    /// </summary>

    // Singleton-ish instance.
    private static Debugger m_debugger = null;
    private static GameObject m_debuggerObject = new GameObject();

    private Camera m_camera;
    // Queued 3D text
    private List<DebugText> m_queuedText = new List<DebugText>();
    // Queued 3D lines
    private List<DebugLine> m_queuedLines = new List<DebugLine>();

    
    void Start () {
        m_camera = Camera.main;
	}

    void Awake()
    {
        // Set debug framerate cap.
        //DebugCapFramerate(30);

    }

    void Update () {
        // Start a coroutine to clear the draw queue after the frame has rendered.
        StartCoroutine(ClearDrawQueueAfterFrame());

        /// COLOR DEBUGGING
        Color some_color = Color.green;
        //ColorSchemes.getComplementaryColor(some_color, out other_color, ColorMode.rgb);
        List<Color> colors = ColorSchemes.GetSplitComplementaryColors(some_color, _Color.ColorSpace.ryb);
        Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 3, 0), colors[0], 1.0f, false);
        Debug.DrawLine(new Vector3(0, 3, 0), new Vector3(0, 6, 0), colors[1], 1.0f, false);
        Debug.DrawLine(new Vector3(0, 6, 0), new Vector3(0, 9, 0), colors[2], 1.0f, false);
    } 

    void OnGUI()
    {
        DrawQueued();
    }

    public static void Debug3DText(string text, Vector3 position)
    {
        /// Draws text in world space.
        RequestDebugger().QueueDrawString(text, position);
    }

    public static void Debug3DLine(Vector3 start, Vector3 end) { Debug3DLine(start, end, Color.white, 1.0f); }
    public static void Debug3DLine(Vector3 start, Vector3 end, Color color) { Debug3DLine(start, end, color, 1.0f); }
    public static void Debug3DLine(Vector3 start, Vector3 end, float width) { Debug3DLine(start, end, Color.white, width); }
    public static void Debug3DLine(Vector3 start, Vector3 end, Color color, float width)
    {
        /// Draws a line in world space.
        RequestDebugger().QueueDrawLine(start, end, color, width);
    }


    public static void DebugCapFramerate(int fps)
    {
        /// Caps the framerate.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps;
    }

    //-------------------------------------------------------------------------------
    [RuntimeInitializeOnLoadMethod]
    private static Debugger RequestDebugger()
    {
        ///
        /// Creates a debugger component, or returns the existing one.
        ///
        if (m_debugger == null)
        {
            m_debugger = (Debugger)m_debuggerObject.AddComponent(typeof(Debugger));
            m_debuggerObject.name = "_DebuggerHolder";
        }
        return m_debugger;
    }

    private void QueueDrawString(string str, Vector3 pos)
    {
        /// Queues a string for drawing on the next frame.
        if (PosInView(pos)) {
            m_queuedText.Add(new DebugText(str, pos));
        }
    }

    private void QueueDrawLine(Vector3 start, Vector3 end) { QueueDrawLine(start, end, Color.white, 1.0f); }
    private void QueueDrawLine(Vector3 start, Vector3 end, Color color) { QueueDrawLine(start, end, color, 1.0f); }
    private void QueueDrawLine(Vector3 start, Vector3 end, float width) { QueueDrawLine(start, end, Color.white, width); }
    private void QueueDrawLine(Vector3 start, Vector3 end, Color color, float width)
    {
        /// Queues a line for drawing on the next frame.
        /// TODO: What if the cam only sees a point between the start and end? 
        ///     Try: Vector3.Project to find point on start-end line closest to cam, check if THAT is in view.
        if (PosInView(start) || PosInView(end))
        {
            m_queuedLines.Add(new DebugLine(start, end, color, width));
        }
    }

    private void DrawQueued()
    {
        /// Draws the queue. MUST be called from OnGUI.
        // Draw queued text
        for (var i = 0; i < m_queuedText.Count; i++)
        {
            Handles.Label(m_queuedText[i].m_pos, m_queuedText[i].m_text);
        }
        // Draw queued lines
        for (var i=0; i < m_queuedLines.Count; i++)
        {
            Drawing.Drawing.DrawLine(m_queuedLines[i].m_start, m_queuedLines[i].m_end, m_queuedLines[i].m_color, m_queuedLines[i].m_width);
        }

    }

    private IEnumerator ClearDrawQueueAfterFrame()
    {
        /// Coroutine that clears the draw queue after the current frame is rendered.
        yield return new WaitForEndOfFrame();
        m_queuedText.Clear();
        m_queuedLines.Clear();
    }

    private bool PosInView(Vector3 pos)
    {
        /// Returns whether the provided position is in view of the camera.
        /// TODO: Move to some math lib
        Rect cam_rect = new Rect(0, 0, m_camera.pixelWidth, m_camera.pixelHeight);

        return m_camera.WorldToViewportPoint(pos).z > 0 && cam_rect.Contains(m_camera.WorldToViewportPoint(pos));
    }

}
