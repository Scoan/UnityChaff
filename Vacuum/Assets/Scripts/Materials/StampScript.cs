using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StampScript : MonoBehaviour
{
    // The mover script. We'll grab max velocity from it.
    MoverScript m_moverScript;
    // The camera that composites stamps into things!
    private OverheadRTCam m_renderCam;

    float m_sleepDist = .001f;
    Vector3 m_velocity;
    float m_suction;

    Vector3 m_lastPos;
    Vector3 m_lastVelocity;
    float m_lastSuction;

    // For debugging
    List<float> m_velocitySamples = new List<float>();

    // Use this for initialization
    void Start()
    {
        m_moverScript = (MoverScript)GetComponentInParent(typeof(MoverScript));
        m_renderCam = FindObjectOfType<OverheadRTCam>();

        m_lastPos = this.transform.position;
        m_lastVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        m_lastSuction = 0.0f;

        initDebugVelocity();
    }

    // Update is called once per frame
    void Update()
    {
        // Calc change in position since last update.
        Vector3 delta_pos = this.transform.position - m_lastPos;

        // If the stamp stomp moving, maintain the previous velocity. Otherwise, point in direction of movement!
        // TODO: Should we actually sleep velocity?
        if (delta_pos.magnitude > m_sleepDist)
        {
            m_velocity = delta_pos / Time.deltaTime;
            m_velocity = m_velocity / m_moverScript.m_maxSpeed; // Normalize velocity against max speed.
        }
        else
        {
            m_velocity = m_lastVelocity;
        }
        //DebugAvgVelocity(m_velocity.magnitude);
        
        // lerp last velocity to velocity to smooth out changes a bit
        m_velocity = Vector3.Lerp(m_lastVelocity, m_velocity, 6.0f * Time.deltaTime);      // TODO: Magic number :( maybe do this in fixedUpdate?

        // lerp suction
        m_suction = Mathf.Lerp(m_lastSuction, m_moverScript.m_isPowered ? m_moverScript.m_maxSuction : 0.0f, 2.0f * Time.deltaTime);        // TODO: Magic Number :(

        // Colorize render cam with vacuum's velocity!
        // TODO: Instead of colorizing the cam, colorize *the stamp*. Like, write vertex color and multiply stencil against it in shader.
        // Maybe calc vert velocities individually in a compute shader?

        Vector3 out_color = new Vector3(m_velocity.x, m_velocity.z, m_suction);
        out_color = Vector3.ClampMagnitude(out_color, 1.0f);
        out_color.x = (out_color.x + 1.0f) * .5f;
        out_color.y = (out_color.y + 1.0f) * .5f;

        Debugger.Debug3DText("velocity: " + out_color, new Vector3(3, 3, 3));
        m_renderCam.SetTgtColor(out_color);

        // Remember prev pos and velocity for next calculation.
        m_lastPos = this.transform.position;
        m_lastVelocity = m_velocity;
        m_lastSuction = m_suction;

    }





    // Debugging

    int DEBUG_VELOCITY_SAMPLES = 20;

    private void initDebugVelocity()
    {
        for (int i = 0; i < DEBUG_VELOCITY_SAMPLES; i++)
        {
            m_velocitySamples.Add(0);
        }
    }

    private void DebugAvgVelocity(float new_velocity)
    {
        m_velocitySamples.RemoveAt(0);
        m_velocitySamples.Add(new_velocity);
        float avg_vel = 0.0f;
        for (int i = 0; i < m_velocitySamples.Count; i++)
        {
            avg_vel += m_velocitySamples[i];
        }
        avg_vel /= m_velocitySamples.Count;

        Debugger.Debug3DText(string.Format("Avg Velocity over {0} samples: {1}", DEBUG_VELOCITY_SAMPLES, avg_vel.ToString()), Vector3.zero);
    }

}
