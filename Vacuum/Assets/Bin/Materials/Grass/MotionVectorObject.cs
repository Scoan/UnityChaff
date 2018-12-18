using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionVectorObject : MonoBehaviour {

    public float m_MaxVelocity = 5.0f;

    Material m_AssignedMaterial;
    Rigidbody m_RigidBody;

    Vector3 m_CurrentVelocity;
    Vector3 m_PreviousNormalizedVelocity = new Vector3(0f,0f,0f);
    Vector3 m_PreviousPosition = new Vector3(0f,0f,0f);

	// Use this for initialization
	void Start () {
        // Get handle to my material
        m_AssignedMaterial = GetComponent<Renderer>().material;
        m_RigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_CurrentVelocity = GetVelocity(m_PreviousPosition);
        m_PreviousPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update () {
        // TODO: Set Motion Vector property on my material to my movement (smoothed?)
        // TODO: Check if my material has a _MotionVector property and complain if it doesn't?
        Vector3 velocity = m_CurrentVelocity;
        float velocityMag = m_CurrentVelocity.magnitude;
        Vector3 normalizedVelocity = velocity.normalized * Mathf.Clamp((velocityMag / m_MaxVelocity), 0.0f, 1.0f);

        Vector3 smoothVelocity;
        smoothVelocity.x = Mathf.Lerp(m_PreviousNormalizedVelocity.x, normalizedVelocity.x, .3f);
        smoothVelocity.y = Mathf.Lerp(m_PreviousNormalizedVelocity.y, normalizedVelocity.y, .3f);
        smoothVelocity.z = Mathf.Lerp(m_PreviousNormalizedVelocity.z, normalizedVelocity.z, .3f);

        m_AssignedMaterial.SetVector("_MotionVector", new Vector4(smoothVelocity.x, smoothVelocity.z, 1.0f, 1.0f));

        m_PreviousNormalizedVelocity = smoothVelocity;
	}

    Vector3 GetVelocity(Vector3 previousPosition)
    {
        Vector3 currentPosition = this.transform.position;
        Vector3 velocity = (currentPosition - previousPosition) / Time.fixedDeltaTime;
        return velocity;
    }

}
