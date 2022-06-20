using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class RoadDataSystem : MonoBehaviour
{
    public PathCreator PathCreator;
    public RoadMeshCreator roadMeshCreator;
    public int LevelIndex = -1;

    public void Save()
    {
        RoadData roadData = new RoadData();
        roadData.pathCreatorData = PathCreator.EditorData;
        roadData.roadMeshSettings = roadMeshCreator.roadMeshSettings;

        if (LevelIndex == -1)
        {
            Debug.LogError("Please input fileName!");
            return;
        }

        string path = $"Assets/Resources/RoadData/Level{LevelIndex}_RoadData.asset";
        LevelIndex = -1;
        AssetDatabase.CreateAsset(roadData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Saved to " + path);
    }
}
#endif