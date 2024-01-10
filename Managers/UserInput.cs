using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    KeyBindings bindings => KeyBindings.instance;
    public delegate void BathysphereMoveDelegate();
    public event BathysphereMoveDelegate MoveForward;
    public event BathysphereMoveDelegate MoveBackward;
    public event BathysphereMoveDelegate EnableBrake;
    public event BathysphereMoveDelegate DisableBrake;
    public static UserInput instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = false;
        HashSet<KeyCode> keys = bindings.GetBindings(UserActions.Forward);
        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                forwardPressed = true;
                Debug.Log($"forward key {key} pressed!");
            }
        }
        if (forwardPressed)
            MoveForward?.Invoke();

        bool backwardPressed = false;
        keys = bindings.GetBindings(UserActions.Backward);
        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                backwardPressed = true;
                Debug.Log($"backward key {key} pressed!");
            }
        }
        if (backwardPressed)
            MoveBackward?.Invoke();

        bool brakePressed = false;
        bool brakeReleased = false;
        keys = bindings.GetBindings(UserActions.Backward);
        foreach (var key in keys)
        {
            if (Input.GetKeyDown(key))
            {
                brakePressed = true;
                Debug.Log($"brake key {key} pressed!");
            }
            if (Input.GetKeyUp(key))
            {
                brakeReleased = true;
                Debug.Log($"brake key {key} released!");
            }
        }
        if (brakePressed)
            EnableBrake?.Invoke();
        else if (brakeReleased)
            DisableBrake?.Invoke();
    }
}
