using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Force
{
    Vector3 direction { get; }
    Vector3 position  { get; }
    public Vector3 Magnitude {  get; }
    public Vector3 Evaluate { get; }
}