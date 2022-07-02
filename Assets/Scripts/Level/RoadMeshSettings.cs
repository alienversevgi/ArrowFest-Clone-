using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    [System.Serializable]
    public class RoadMeshSettings
    {
        [Header("Road settings")]
        public float roadWidth = .5f;
        [Range(0, .5f)]
        public float thickness = .15f;
        public bool flattenSurface;

        [Header("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public float textureTiling = 1;
    }
}