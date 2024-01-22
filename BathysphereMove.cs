using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BathysphereMove : MonoBehaviour
{
    const float MIDSECTION = PI * RADIUS * RADIUS;
    Water medium;
    Rigidbody rb;
    /// <summary>
    /// threshold for Reynolds number and velocity magnitude
    /// </summary>
    const float THRESHOLD = 1e-4f;
    const float PI = 3.1415926f;
    const float RADIUS = 1.1f;
    const int MAX_VELOCITY_LEVEL =  5;
    const int MIN_VELOCITY_LEVEL = -5;
    Vector3 forwardDirection => new Vector3(-Mathf.Sin(-transform.rotation.eulerAngles.y * Mathf.Deg2Rad), 0f, Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
    /// <summary>
    /// with 3800 Nutons sphere moves at speed around 5 m/s
    /// </summary>
    const float BASE_DRAG_FORCE = 10000f;
    float dragScaleFactor = .5f;
    /// <summary>
    /// for debugging
    /// </summary>
    Vector3 velocity => rb.velocity;
    Vector3 forwardVelocityDirection => velocity.normalized;
    Vector3 forwardVelocity => forwardDirection * Vector3.Dot(forwardDirection, forwardVelocityDirection);
    float velocityMagnitude => velocity.magnitude;
    float Reynolds => velocityMagnitude * 2 * RADIUS * medium.DENSITY / medium.DYNAMIC_VISCOUSITY;
    bool areWeMovingForward => Vector3.Dot(velocity, forwardDirection) > 0;
    [SerializeField] int torqueDirection = 0;
    [SerializeField] float _w_frictionForceContribution;
    public float BASE_TORQUE_FORCE = .1f;

    bool breakForceEnabled = false;
    /// <summary>
    /// for debugging. Later i have to make this field private
    /// </summary>
    [SerializeField] int velocityLevel = 0;


    [SerializeField] float _w_totalForce;
    void IncreaseVelocityLevel() { ChangeVelocityLevel(1); }
    void DecreaseVelocityLevel() { ChangeVelocityLevel(-1); }
    void EnableBrakeForce() { breakForceEnabled = true; }
    void EnableTorqueRight() { torqueDirection = 1; }
    void EnableTorqueLeft() { torqueDirection = -1; }
    void DisableTorque() { torqueDirection = 0; }
    void DisableBrakeForce() { breakForceEnabled = false; }
    /// <summary>
    /// Change velocityLevel by a given level (this.velocityLevel += velocityLevel)
    /// 
    /// <para>If new level is out of bounds, velocity level is not changed</para>
    /// </summary>
    /// <param name="velocityLevelOffset"></param>
    /// <returns>true, if velocityLevel is changed. false, otherwise</returns>
    public bool ChangeVelocityLevel(int velocityLevelOffset)
    {
        int newLevel = velocityLevelOffset + velocityLevel;
        if (newLevel >= MIN_VELOCITY_LEVEL && newLevel <= MAX_VELOCITY_LEVEL)
        {
            velocityLevel = newLevel;
            return true;
        }
        else
            return false;

    }
    /// <summary>
    /// Set velocityLevel to the given level
    /// </summary>
    /// <param name="velocityLevel"></param>
    /// <returns></returns>
    public bool SetVelocityLevel(int velocityLevel)
    {
        if (velocityLevel < MIN_VELOCITY_LEVEL)
        {
            this.velocityLevel = MIN_VELOCITY_LEVEL;
            return false;
        }
        else if (velocityLevel > MAX_VELOCITY_LEVEL) {
            this.velocityLevel = MAX_VELOCITY_LEVEL;
            return false;
        }
        else
            this.velocityLevel = velocityLevel;
        return true;
    }
    /// <summary>
    /// Applying all acting forces to rigid body
    /// </summary>
    void ApplyForces()
    {
        if (rb == null)
            Console.WriteLine("rigid body is null!!! Add rigidbody to Bathysphere!");
        Vector3 dragForce = GetDragForce();
        Vector3 fricForce = GetForwardFrictionForce();
        Vector3 totalForce = dragForce + fricForce;
        totalForce += GetBrakeForce();
        totalForce += GetLateralFrictionForce();
        _w_frictionForceContribution = fricForce.magnitude / (dragForce.magnitude - fricForce.magnitude) * 100;
        
        rb.AddForce(totalForce);
        _w_totalForce = totalForce.magnitude;
        rb.AddTorque(Vector3.up * BASE_TORQUE_FORCE * torqueDirection);
    }
    /// <summary>
    /// Evaluating braking force, which slows down vehicle
    /// </summary>
    /// <returns>braking force, SI units</returns>
    Vector3 GetBrakeForce(){
        if (velocityMagnitude < THRESHOLD || !breakForceEnabled){
            return Vector3.zero;
        }
        Vector3 force = - Mathf.Sqrt(velocityMagnitude) * 3 * BASE_DRAG_FORCE * forwardVelocityDirection;
        return force;
    }
    float StocksSphereFriction()
    {
        if (velocityMagnitude < THRESHOLD)
            return 0f;
        float force = 0;
        float _Re = Reynolds;
        if (_Re < THRESHOLD)
            force = 0;
        else if (_Re <= 1)
            force = 24f / _Re;
        else if (1 < _Re && _Re < 700)
            force = 24 / _Re + 4 / MathF.Pow(_Re, 1f / 3f);
        else
            force = .44f;
        force *= 0.5f * medium.DENSITY * velocityMagnitude * velocityMagnitude * MIDSECTION;
        return force;
    }
    /// <summary>
    /// Evaluating the force of liquid friction
    /// </summary>
    /// <returns>force of liquid friction, SI units</returns>
    Vector3 GetForwardFrictionForce()
    {
        Vector3 force = -StocksSphereFriction() * forwardVelocityDirection;
        Debug.DrawLine(transform.position, transform.position + force / BASE_DRAG_FORCE, Color.white);
        return force;
    }
    Vector3 GetLateralFrictionForce()
    {
        Vector3 lateralVelocityDirection = velocity - velocity.magnitude * forwardVelocityDirection;

        var force = -lateralVelocityDirection.normalized * 50f * medium.DENSITY * Mathf.Sqrt(velocityMagnitude) * velocityMagnitude * MIDSECTION;
        Debug.DrawLine(transform.position, transform.position + force / BASE_DRAG_FORCE, Color.green);
        return force;
    }
    /// <summary>
    /// Evaluating dragging force of engines
    /// </summary>
    /// <returns>dragging force, SI units</returns>
    Vector3 GetDragForce()
    {
        if (velocityLevel == 0)
            return Vector3.zero;
        Vector3 force = velocityLevel * BASE_DRAG_FORCE * dragScaleFactor * forwardDirection;
        Debug.DrawLine(transform.position, transform.position+force / BASE_DRAG_FORCE, Color.red);
        return force;
    }

    private void Awake()
    {
        if (!(medium is Medium))
        {
            throw new Exception($"Bathysphere field 'medium' does not impliment 'Medium' interface! Fix it!");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UserInput userInput = UserInput.instance;
        if (userInput == null)
            Debug.LogError("UserInput script was not instantiated! Attach the script to a game object!");
        userInput._actions[UserActions.Forward ].Add(new DelegateKeyAction(delegate () { IncreaseVelocityLevel(); }, null) );
        userInput._actions[UserActions.Backward].Add(new DelegateKeyAction(delegate () { DecreaseVelocityLevel(); }, null));
        userInput._actions[UserActions.Brake   ].Add(new DelegateKeyAction(delegate () { EnableBrakeForce(); }, delegate () { DisableBrakeForce(); }));
        userInput._actions[UserActions.Left].Add(new DelegateKeyAction(delegate () { EnableTorqueLeft(); }, delegate () { DisableTorque(); }));
        userInput._actions[UserActions.Right].Add(new DelegateKeyAction(delegate () { EnableTorqueRight(); }, delegate () { DisableTorque(); }));
        /*        userInput.MoveForward  += IncreaseVelocityLevel;
                userInput.MoveBackward += DecreaseVelocityLevel;
                userInput.EnableBrake  += EnableBrakeForce;
                userInput.DisableBrake += DisableBrakeForce;*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForces();
    }
}
