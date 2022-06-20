using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GateData
{
    public DoorValue Left;
    public DoorValue Right;

    public Vector3 Position;

    public override string ToString()
    {
        return $"Left {Left.ToString()}, Right {Right.ToString()} position : {Position.ToString()}";
    }
}