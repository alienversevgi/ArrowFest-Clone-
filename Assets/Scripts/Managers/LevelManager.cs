using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public PathCreator PathCreator;
    [SerializeField] private RoadMeshCreator roadMeshCreator;
    [SerializeField] private StepController stepController;
    [SerializeField] private List<LevelData> levels;
    [SerializeField] private int currentLevel;
    public LevelData CurrentLevelData => levels[currentLevel];

    public void LoadLevel()
    {
        PathCreator.Initialize(CurrentLevelData.RoadData.pathCreatorData);
        roadMeshCreator.Initialize(CurrentLevelData.RoadData.roadMeshSettings);

        Quaternion stepInitialRotation = PathCreator.path.GetRotation(1, EndOfPathInstruction.Stop);
        stepController.Initialize(stepInitialRotation);

        #region Setup Gates

        foreach (GateData gateData in CurrentLevelData.Gates)
        {
            Gate cloneGate = PoolManager.Instance.GatePool.Allocate();
            cloneGate.Initialize(gateData.Left, gateData.Right);

            cloneGate.SetPositionAndEnable(GetPositionOnRoad(gateData.Position));
            cloneGate.transform.eulerAngles = GetRotationOnRoad(gateData.Position);
        }

        #endregion

        #region Setup Chibies

        foreach (ChibiData chibiData in CurrentLevelData.Chibis)
        {
            Chibi cloneChibi = PoolManager.Instance.ChibiPool.Allocate();
            cloneChibi.Initialize(chibiData.HP);

            float yAxisAngle = chibiData.RotationDirection == RotationDirectionType.Front ? 180 : 0;

            cloneChibi.SetPositionAndEnable(GetPositionOnRoad(chibiData.Position));
            cloneChibi.transform.eulerAngles = new Vector3(0, yAxisAngle, 0);
        }

        #endregion

        #region Setup ChibieGroups

        foreach (GhibiGroupData chibiGroupData in CurrentLevelData.ChibiGroups)
        {
            ChibiGroup cloneChibiGroup = PoolManager.Instance.ChibiGroupPool.Allocate();
            Vector3 position = GetPositionOnRoad(chibiGroupData.Position);
            float t = PathCreator.path.GetClosestTimeOnPath(position);

            cloneChibiGroup.SetPositionAndEnable(position);
            cloneChibiGroup.Initialize(PathCreator.path, PathCreator.transform);

            cloneChibiGroup.transform.eulerAngles = GetRotationOnRoad(chibiGroupData.Position);
        }

        #endregion
    }

    #region Private Methods

    private Vector3 GetRotationOnRoad(Vector3 position)
    {
        float timeToPoint = PathCreator.path.GetClosestTimeOnPath(position);
        Quaternion rotation = PathCreator.path.GetRotation(timeToPoint);
        Vector3 eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);

        return eulerAngles;
    }

    private Vector3 GetPositionOnRoad(Vector3 position)
    {
        Vector3 onRoadPosition = PathCreator.path.GetClosestPointOnPath(position);

        return onRoadPosition;
    }

    #endregion
}