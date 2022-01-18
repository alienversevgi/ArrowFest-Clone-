using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Craeta",menuName ="road", order = 0)]
public class RoadData : ScriptableObject
{
    [SerializeField] public PathCreatorData pathCreatorData;
    [SerializeField] public RoadMeshSettings roadMeshSettings;
}
