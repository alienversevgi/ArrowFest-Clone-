using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    [CreateAssetMenu(fileName = "Level", menuName = "Create RoadData", order = 1)]
    public class RoadData : ScriptableObject
    {
        [SerializeField] public PathCreatorData pathCreatorData;
        [SerializeField] public RoadMeshSettings roadMeshSettings;
    }
}