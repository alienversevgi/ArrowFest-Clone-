using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Create LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public RoadData RoadData;
    public List<GateData> Gates;
    public List<ChibiData> Chibis;
    public List<GhibiGroupData> ChibiGroups;
}