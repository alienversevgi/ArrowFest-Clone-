using PathCreation;
using PathCreation.Examples;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private RoadMeshCreator roadMeshCreator;

    public RoadData roadData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            roadData.pathCreatorData = pathCreator.EditorData;
            roadData.roadMeshSettings = roadMeshCreator.roadMeshSettings;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            pathCreator.Initialize(roadData.pathCreatorData);
            roadMeshCreator.Initialize(roadData.roadMeshSettings);
        }
    }
}
