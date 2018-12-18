using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosToShader : MonoBehaviour {

    Material myMaterial;

    // Use this for initialization
    void Start () {
		myMaterial = GetComponent<Renderer>().material;

        Vector3 pos = this.transform.position;
        myMaterial.SetVector("_Position", new Vector4(pos.x, pos.y, pos.z, 1.0f));

        // TODO: make scaling deterministic (based on transform?)
        this.transform.localScale = this.transform.localScale * Mathf.Lerp(.8f, 1.0f, Random.value);

    }
	
	// Update is called once per frame
	void Update () {
    }
}
