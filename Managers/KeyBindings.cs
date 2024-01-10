using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserActions {
    Forward,
    Backward,
    Left,
    Right,
    Up,
    Down,
    Brake
};
public class KeyBindings : MonoBehaviour
{
    private Dictionary<UserActions, HashSet<KeyCode>> _actions = new() {
        { UserActions.Forward , new HashSet<KeyCode>() { KeyCode.W } },
        { UserActions.Backward, new HashSet<KeyCode>() { KeyCode.S } },
        { UserActions.Left    , new HashSet<KeyCode>() { KeyCode.A } },
        { UserActions.Right   , new HashSet<KeyCode>() { KeyCode.D } },
        { UserActions.Up      , new HashSet<KeyCode>() { KeyCode.Q } },
        { UserActions.Down    , new HashSet<KeyCode>() { KeyCode.E } },
        { UserActions.Brake   , new HashSet<KeyCode>() { KeyCode.Space } }
    };
    static public KeyBindings instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    public HashSet<KeyCode> GetBindings(UserActions action){return _actions[action];}
}
