using UnityEngine;
using System.Collections;
using MyInput;

public class MoverScript : MonoBehaviour {

    Rigidbody m_rb;

    public float m_maxSpeed = 4;
    public float m_maxTurnSpeed = 1.5f;
    public float m_maxSuction = 1.0f;      // How strong is suction?
    // Is the vacuum on?
    public bool m_isPowered = false;
    // Is the vacuum lifted up?
    public float m_liftStrength;

    

    Vector2 m_input;

    Plane m_groundPlane;

    // Use this for initialization
    void Start () {
        m_rb = this.GetComponent<Rigidbody>();

        m_input = new Vector2(0.0f, 0.0f);
        m_groundPlane = new Plane(Vector3.up, Vector3.zero);
        m_liftStrength = 0.0f;
    }
	
    void FixedUpdate()
    {

        // Handle linear velocity

        //float tgt_speed = m_maxSpeed * m_input.y;
        float tgt_speed = m_maxSpeed * m_input.magnitude * Mathf.Clamp(Vector3.Dot(this.transform.forward, new Vector3(m_input.x, 0, m_input.y)) * 2.0f, -1.0f, 1.0f);



        //aim tgt speed along forward vector!
        Vector3 tgt_velocity = tgt_speed * this.transform.forward;
        // And smooth the transition out a bit.
        Vector3 eff_tgt_velocity = Vector3.Lerp(m_rb.velocity, tgt_velocity, .4f);                        //TODO: Magic number
        // Apply a velocity change such that we achieve the desired speed.
        Vector3 delta_vel = eff_tgt_velocity - m_rb.velocity;
        m_rb.AddForce(delta_vel, ForceMode.VelocityChange);


        // We limit rotational velocity by forward velocity (can't turn a vacuum while it's standing still)
        float forward_vel = this.transform.InverseTransformDirection(m_rb.velocity).x;


        // TODO: Bias the "flip point"? Right now will flip from going forward to backward when input is perpendicular to forward.
        // Add some control so it doesn't flip if the previous input was in a direction, until the user lets off the joystick.
        // Also add control over the flip point; instead of flipping at perpendicular, maybe when it's +75% off forward?

        // Figure out how to rotate to aim at the input.
        Vector3 tgtDir = new Vector3(m_input.x, 0, m_input.y);
        Vector3 torqueVector = getTorqueVectorToAimAtDir(this.transform.forward * Mathf.Sign(tgt_speed), tgtDir);
        // Debug draw axes
        Debugger.Debug3DLine(this.transform.position, this.transform.position + tgtDir.normalized, Color.blue, 2);
        Debugger.Debug3DLine(this.transform.position, this.transform.position + this.transform.forward * Mathf.Sign(tgt_speed), Color.green, 2);
        Debugger.Debug3DLine(this.transform.position, this.transform.position + (torqueVector.normalized * .5f), Color.red, 2);

        // Handle rotational velocity
        //float tgt_rot_speed = m_maxTurnSpeed * m_input.x;
        float tgt_rot_speed = m_maxTurnSpeed * torqueVector.magnitude * Mathf.Sign(torqueVector.y) * Mathf.Sign(tgt_speed);

        Vector3 tgt_ang_velocity = new Vector3(0.0f, tgt_rot_speed, 0.0f) * Vector3.Dot(m_rb.velocity, this.transform.forward / m_maxSpeed);
        Vector3 eff_tgt_ang_velocity = Vector3.Lerp(m_rb.angularVelocity, tgt_ang_velocity, .9f);     //TODO: Magic number
        Vector3 delta_ang_velocity = eff_tgt_ang_velocity - m_rb.angularVelocity;

        m_rb.AddTorque(delta_ang_velocity, ForceMode.VelocityChange);

    }

	// Update is called once per frame
	void Update () {
        ManageInputs();

    }

    private void ManageInputs()
    {
        // Gather inputs
        m_input = MyInput.MyInput.ProcessInputAxes();
        // Map the input to the ground plane.
        m_input = MyInput.MyInput.CoordsOrientedAboveCamera(m_input);
        
        if (Input.GetButtonDown("Right Bumper"))
        {
            ToggleSuction();
        }

        m_liftStrength = Input.GetAxisRaw("Right Trigger");
    }


    private Vector3 getTorqueVectorToAimAtDir(Vector3 curDir, Vector3 tgtDir)
    {
        /// Returns the normalized torque vector necessary to rotate inDir to face tgtDir.
        /// The idea is to figure out whether we need to turn left or right, and (if we're close to facing the target dir) whether to slow down our turn rate.
        /// If we need to turn far, the result will be a torque vector of magnitude 1. If we're within the backOffAngle, the magnitude will drop.
        
        float minAngle = 0.5f;      // The angle at which we'll apply 0 torque. The "close enough" angle.
        float backOffAngle = 45.0f; // The angle at which we'll begin to lessen the torque applied.

        float angleBetween = Vector3.Angle(curDir, tgtDir);
        float torqueMagnitude = Mathf.Clamp((angleBetween - minAngle)/(backOffAngle - minAngle), 0.0f, 1.0f);

        Vector3 torqueVector = Vector3.Cross(curDir, tgtDir);

        // Handle degenerate case (dirs are opposite; cross.magnitude ~= 0, dot ~= -1).
        if ((torqueVector.magnitude < Mathf.Epsilon) && (Vector3.Dot(curDir, tgtDir) + 1 <= Mathf.Epsilon))
        {
            torqueVector = Vector3.up;
        }
        torqueVector = torqueVector.normalized * torqueMagnitude;

        return torqueVector;
    }



    private void ToggleSuction()
    {
        m_isPowered = !m_isPowered;
    }

    private void ToggleLifted()
    {
        // TODO: If the player lifts the vacuum...
        //      - Vacuum no longer imprints on ground
        //      - Vacuum is physically lifted up (dangles like a pendulum a bit?)
        //      - Player can move and rotate the vacuum independently
        //      - Player leaves footprints in the carpet as they move around
    }


}
