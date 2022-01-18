using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private RoadMeshCreator roadMeshCreator;

    public RoadData roadData;

    public GameObject v;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(v, pathCreator.path.GetPoint(Random.Range(0, pathCreator.path.NumPoints)), Quaternion.identity);
        }

        /*
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
        */
    }
}
