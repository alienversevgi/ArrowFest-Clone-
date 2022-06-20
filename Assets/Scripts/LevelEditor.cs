using PathCreation;
using PathCreation.Examples;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(RoadDataSystem))]
public class LevelEditor : Editor
{
    RoadDataSystem roadDataSystem;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        roadDataSystem = (RoadDataSystem)target;
        if (GUILayout.Button("Save"))
        {
            roadDataSystem.Save();
        }
    }


    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S))
    //    {
    //        roadData.bezierPathPoints = pathCreator.EditorData.bezierPath.GetPoints();
    //        roadData.roadMeshSettings = roadMeshCreator.roadMeshSettings;
    //        Debug.Log("Saved");
    //    }

    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        pathCreator.Initialize(roadData.pathCreatorData, roadData.bezierPathPoints);
    //        roadMeshCreator.Initialize(roadData.roadMeshSettings);
    //        Debug.Log("Loaded");
    //    }
    //}
}
#endif