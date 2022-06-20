using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HittableBodyPart
{
    Default = -1,
    
    Hips,
    LeftUpLeg,
    LeftLeg,
    RightUpLeg,
    RightLeg,
    Body,
    LeftArm,
    LeftForeArm,
    LeftHand,
    Head,
    RightArm,
    RightForeArm,
    RightHand
}

public enum RotationDirectionType
{
    Front,
    Back
}

[System.Serializable]
public class ChibiData 
{
    public int HP;
    public Vector3 Position;
    public RotationDirectionType RotationDirection; // or can be use specific rotation vector3
    public HittableBodyPart HittableBodyPart;
}
