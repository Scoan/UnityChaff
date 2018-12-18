using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using ColorPalette;


public class ProcGen : MonoBehaviour
{
    // Singleton-ish instance.
    private static ProcGen m_procGen = null;
    private static GameObject m_procGenObject = new GameObject();

    public Texture2D m_paletteTexture;

    void Start()
    {
    }

    void Awake()
    {
        Debug.Log("Beware, ProcGen LIVES");
        // Generate color palette
        //Random.InitState(1337);
        m_paletteTexture = ColorPalette.ColorPalette.GetPaletteSheet();

        // TEST: Assign texture to test plane
        GameObject testObj = GameObject.Find("testPlane");
        if (testObj != null)
            testObj.GetComponent<Renderer>().material.mainTexture = m_paletteTexture;

        // TEST: Save palette out
        byte[] bytes = m_paletteTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../ColorPalette.png", bytes);
        
    }


    [RuntimeInitializeOnLoadMethod]
    private static ProcGen RequestProcGen()
    {
        ///
        /// Creates a procGen component, or returns the existing one.
        ///
        if (m_procGen == null)
        {
            m_procGen = (ProcGen)m_procGenObject.AddComponent(typeof(ProcGen));
            m_procGenObject.name = "_ProcGenHolder";
        }
        return m_procGen;
    }


}