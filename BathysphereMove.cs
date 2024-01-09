using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BathysphereMove : MonoBehaviour
{
    public Rigidbody rb;
    const float THRESHOLD = 1e-6f;
    const float PI = 3.1415926f;
    const float RADIUS = 1.1f;
    const float MIDSECTION = PI * RADIUS * RADIUS;
    const int MAX_VELOCITY_LEVEL = 3;
    const int MIN_VELOCITY_LEVEL = -3;
    const float BASE_DRAG_FORCE = 3800f;
    float dragScaleFactor = .25f;
    public int velocityLevel = 0;
    public float _frictionForceContribution;
    Vector3 velocity => rb.velocity;
    float velocityMagnitude => velocity.magnitude;
    float Reynolds => velocityMagnitude * 2 * RADIUS * Water.DENSITY / Water.DYNAMIC_VISCOUSITY;
    bool areWeMovingForward => Vector3.Dot(velocity, Vector3.forward) > 0;


    public float _totalForce;
    void IncreaseVelocityLevel(){ChangeVelocityLevel(1); }
    void DecreaseVelocityLevel() { ChangeVelocityLevel(-1); }
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

    void ApplyForces()
    {
        if (rb == null)
            Console.WriteLine("rigid body is null!!! Add rigidbody to Bathysphere!");
        float dragForce = GetDragForce();
        float fricForce = GetResistForce();
        float totalForce = dragForce - fricForce;
        _frictionForceContribution = fricForce / dragForce * 100;
        rb.AddForce(Vector3.forward * totalForce);
        _totalForce = totalForce;
    }

    float GetResistForce()
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
        force *= 0.5f * Water.DENSITY * velocityMagnitude * velocityMagnitude * MIDSECTION;
        if (!areWeMovingForward)
            force *= -1;
        return force;
    }

    float GetDragForce()
    {
        if (velocityLevel == 0)
            return 0;
        float force = velocityLevel * BASE_DRAG_FORCE * dragScaleFactor;
        return force;
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UserInput userInput = UserInput.instance;
        userInput.MoveForward += IncreaseVelocityLevel;
        userInput.MoveBackward += DecreaseVelocityLevel;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyForces();
    }
}
