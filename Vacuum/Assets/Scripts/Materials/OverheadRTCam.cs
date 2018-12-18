using UnityEngine;
using System.Collections;

public class OverheadRTCam : MonoBehaviour {

    public RenderTexture m_RTCam;
    public RenderTexture m_RT1;
    public RenderTexture m_RT2;
    public Texture InitialColor;

    public Shader m_PPShader;
    Material m_PPMat;

    // Use this for initialization
    void Start () {
        m_RT1.DiscardContents();
        m_RT2.DiscardContents();

        Graphics.Blit(InitialColor, m_RT2);
        if (m_PPShader != null)
        {
            m_PPMat = new Material(m_PPShader);
            m_PPMat.SetTexture("_OutTexture", m_RT2);
        }
        
    }

    // Update is called once per frame
    void Update () {
        
    }

    void OnPostRender()
    {
        DoBlit();
    }

    public void SetTgtColor(Vector3 in_color) { SetTgtColor(new Vector4(in_color.x, in_color.y, in_color.z, 1.0f)); }
    public void SetTgtColor(Vector4 in_color)
    {
        m_PPMat.SetColor("_TgtValue", in_color);
    }

    public void DoBlit()
    {
        
        if (m_PPMat != null)
        {
            m_PPMat.SetFloat("_DeltaTime", Time.deltaTime);
            // Blit to RT1 as an intermediate step, as we can't read & write RT2 (which is used by m_PPMat) at the same time
            Graphics.Blit(m_RTCam, m_RT1, m_PPMat);
            Graphics.Blit(m_RT1, m_RT2);
        }
        else
        {
            Graphics.Blit(m_RTCam, m_RT2);
        }  
    }

}
