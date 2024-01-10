using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserActions {
    Forward,
    Backward,
    Left,
    Right,
    Brake
};
public class KeyBindings : MonoBehaviour
{
    private Dictionary<UserActions, HashSet<KeyCode>> _actions = new() {
        { UserActions.Forward, new HashSet<KeyCode>() { KeyCode.W } },
        { UserActions.Backward, new HashSet<KeyCode>() { KeyCode.S } },
        { UserActions.Brake, new HashSet<KeyCode>() { KeyCode.Space } }
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
