using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BathysphereMove : MonoBehaviour
{
    Water medium;
    public Rigidbody rb;
    /// <summary>
    /// threshold for Reynolds number and velocity magnitude
    /// </summary>
    const float THRESHOLD = 1e-6f;
    const float PI = 3.1415926f;
    const float RADIUS = 1.1f;
    const float MIDSECTION = PI * RADIUS * RADIUS;
    const int MAX_VELOCITY_LEVEL = 3;
    const int MIN_VELOCITY_LEVEL = -3;
    /// <summary>
    /// with 3800 Nutons sphere moves at speed around 5 m/s
    /// </summary>
    const float BASE_DRAG_FORCE = 5000f;
    float dragScaleFactor = .25f;
    /// <summary>
    /// for debugging
    /// </summary>
    public float _frictionForceContribution;
    Vector3 velocity => rb.velocity;
    float velocityMagnitude => velocity.magnitude;
    float Reynolds => velocityMagnitude * 2 * RADIUS * medium.DENSITY / medium.DYNAMIC_VISCOUSITY;
    bool areWeMovingForward => Vector3.Dot(velocity, Vector3.forward) > 0;

    bool breakForceEnabled = false;
    /// <summary>
    /// for debugging. Later i have to make this field private
    /// </summary>
    public int velocityLevel = 0;


    public float _totalForce;
    void IncreaseVelocityLevel() { ChangeVelocityLevel(1); }
    void DecreaseVelocityLevel() { ChangeVelocityLevel(-1); }
    void EnableBrakeForce() { breakForceEnabled = true; }
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
        float dragForce = GetDragForce();
        float fricForce = GetFrictionForce();
        float totalForce = dragForce - fricForce;
        if (breakForceEnabled) {
            totalForce -= GetBrakeForce();
        }
        _frictionForceContribution = fricForce / dragForce * 100;
        
        rb.AddForce(Vector3.forward * totalForce);
        _totalForce = totalForce;
    }
    /// <summary>
    /// Evaluating braking force, which slows down vehicle
    /// </summary>
    /// <returns>braking force, SI units</returns>
    float GetBrakeForce(){
        if (velocityMagnitude < THRESHOLD){
            return 0;
        }
        float force = Mathf.Sqrt(velocityMagnitude) * 3 * BASE_DRAG_FORCE;
        if (!areWeMovingForward)
            force *= -1;
        return force;
    }
    /// <summary>
    /// Evaluating the force of liquid friction
    /// </summary>
    /// <returns>force of liquid friction, SI units</returns>
    float GetFrictionForce()
    {
        if (velocityMagnitude < THRESHOLD)
            return 0;
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
        if (!areWeMovingForward)
            force *= -1;
        return force;
    }
    /// <summary>
    /// Evaluating dragging force of engines
    /// </summary>
    /// <returns>dragging force, SI units</returns>
    float GetDragForce()
    {
        if (velocityLevel == 0)
            return 0;
        float force = velocityLevel * BASE_DRAG_FORCE * dragScaleFactor;
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
