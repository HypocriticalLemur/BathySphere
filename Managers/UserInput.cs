using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UserInput;

public struct DelegateKeyAction
{
    public BathysphereMoveDelegate enable;// = null;
    public BathysphereMoveDelegate disable;// = null;
    public DelegateKeyAction(BathysphereMoveDelegate _enable, BathysphereMoveDelegate _disable)
    {
        enable = _enable;
        disable = _disable;
    }
}
public class UserInput : MonoBehaviour
{
    public class MyEventManager
    {

        public event BathysphereMoveDelegate MyEvent;
        public void CallMyEvent() { MyEvent.Invoke(); }
    }
    KeyBindings bindings => KeyBindings.instance;
    public delegate void BathysphereMoveDelegate();
    public event BathysphereMoveDelegate MoveForward;
    public event BathysphereMoveDelegate MoveBackward;
    public event BathysphereMoveDelegate MoveUp;
    public event BathysphereMoveDelegate MoveDown;
    public event BathysphereMoveDelegate EnableBrake;
    public event BathysphereMoveDelegate DisableBrake;
    public event BathysphereMoveDelegate TurnLeft;
    public event BathysphereMoveDelegate TurnRight;

    /*    Dictionary<UserActions, DelegateKeyAction> _actions = new() {
            { UserActions.Forward , new DelegateKeyAction(MoveForward, null) },
            { UserActions.Backward, new DelegateKeyAction(MoveBackward, null) },
            { UserActions.Left    , new DelegateKeyAction(TurnLeft, null) },
            { UserActions.Right   , new DelegateKeyAction(TurnRight, null) },
            { UserActions.Up      , new DelegateKeyAction(MoveUp, null) },
            { UserActions.Down    , new DelegateKeyAction(MoveDown, null) },
            { UserActions.Brake   , new DelegateKeyAction(EnableBrake, DisableBrake) }
        };*/
    public Dictionary<UserActions, HashSet<DelegateKeyAction>> _actions = new(){
            { UserActions.Forward , new HashSet<DelegateKeyAction>()},
            { UserActions.Backward, new HashSet<DelegateKeyAction>()},
            { UserActions.Left    , new HashSet<DelegateKeyAction>()},
            { UserActions.Right   , new HashSet<DelegateKeyAction>()},
            { UserActions.Up      , new HashSet<DelegateKeyAction>()},
            { UserActions.Down    , new HashSet<DelegateKeyAction>()},
            { UserActions.Brake   , new HashSet<DelegateKeyAction>()}
        };

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
        NewUpdateFunc();
    }
    void NewUpdateFunc(){
        foreach (KeyValuePair<UserActions, HashSet<DelegateKeyAction>> item in _actions){
            var action = item.Key;
            var legates = item.Value;
            bool keyPressed = false;
            bool keyReleased = false;
            HashSet<KeyCode> keys = bindings.GetBindings(action);
            foreach (var key in keys)
            {
                if (Input.GetKeyDown(key))
                {
                    keyPressed = true;
                    Debug.Log($"{action} key {key} pressed!");
                }
                else if (Input.GetKeyUp(key))
                {
                    keyReleased = true;
                    Debug.Log($"{action} key {key} released!");
                }
            }
            if (keyPressed)
                foreach(var legate in legates)
                    if (legate.enable != null)
                        legate.enable();
            if (keyReleased)
                foreach (var legate in legates)
                    if (legate.disable != null)
                        legate.disable();

        }
    }
    void OldUpdateFunc(){

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
