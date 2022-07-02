using PathCreation;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Utility;
using Game.Level;

using Random = UnityEngine.Random;
using System.Linq;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private PathCreator _pathCreator;
        [SerializeField] private RoadMeshCreator _roadMeshCreator;
        [SerializeField] private StepController _stepController;
        [SerializeField] private GameObject _levelLoadingObject;
        [SerializeField] private TextMeshProUGUI _levelText;

        public VertexPath LevelRoadPath => _pathCreator.path;
        public float RoadWith => _currentLevelData.RoadData.roadMeshSettings.roadWidth;

        public int CurrentLevel
        {
            get
            {
                return PlayerPrefs.GetInt("level", 0);
            }
            private set
            {

                PlayerPrefs.SetInt("level", value);
            }
        }

        private List<LevelData> _levels;
        private LevelData _currentLevelData => _levels[CurrentLevel];
        private bool _isFirstTimeLevelLoading;

        #endregion

        #region Public Methods

        public void Initialize()
        {
            _levels = Resources.LoadAll<LevelData>("Levels").ToList();
            _isFirstTimeLevelLoading = true;
        }

        public void LoadLevel()
        {
            _levelText.text = $"Level-{CurrentLevel}";
            _pathCreator.Initialize(_currentLevelData.RoadData.pathCreatorData);
            _roadMeshCreator.Initialize(_currentLevelData.RoadData.roadMeshSettings);

            Quaternion stepInitialRotation = _pathCreator.path.GetRotation(1, EndOfPathInstruction.Stop);
            _stepController.Initialize(stepInitialRotation);

            if (_isFirstTimeLevelLoading)
            {
                _levelLoadingObject.SetActive(true);

                Utility.Timer.Instance.StartTimer(3.0f, () =>
                {
                    EventManager.Instance.OnLevelLoaded.Raise();
                    _levelLoadingObject.SetActive(false);
                });
            }
            else
            {
                EventManager.Instance.OnLevelLoaded.Raise();
            }

            #region Setup Gates

            foreach (GateData gateData in _currentLevelData.Gates)
            {
                Gate cloneGate = PoolManager.Instance.GatePool.Allocate();
                cloneGate.Initialize(gateData.Left, gateData.Right);

                cloneGate.SetPositionAndEnable(GetPositionOnRoad(gateData.Position));
                cloneGate.transform.eulerAngles = GetRotationOnRoad(gateData.Position);
            }

            #endregion

            #region Setup Chibis

            foreach (ChibiData chibiData in _currentLevelData.Chibis)
            {
                Chibi cloneChibi = PoolManager.Instance.ChibiPool.Allocate();
                cloneChibi.Initialize();
                float yAxisAngle = chibiData.RotationDirection == RotationDirectionType.Front ? 180 : 0;

                Vector3 position = chibiData.Position.SetOnlyOneAxisVector(Axis.Y, GetPositionOnRoad(chibiData.Position).y);
                cloneChibi.SetPositionAndEnable(position);
                cloneChibi.transform.eulerAngles = new Vector3(0, yAxisAngle, 0);
            }

            #endregion

            #region Setup ChibieGroups

            foreach (GhibiGroupData chibiGroupData in _currentLevelData.ChibiGroups)
            {
                ChibiGroup cloneChibiGroup = PoolManager.Instance.ChibiGroupPool.Allocate();
                Vector3 position = GetPositionOnRoad(chibiGroupData.Position);
                float t = _pathCreator.path.GetClosestTimeOnPath(position);

                cloneChibiGroup.SetPositionAndEnable(position);
                cloneChibiGroup.Initialize(chibiGroupData.rowChibiDatas, _pathCreator.path, _pathCreator.transform);

                cloneChibiGroup.transform.eulerAngles = GetRotationOnRoad(chibiGroupData.Position);
            }

            #endregion
        }

        public void IncrementLevel()
        {
            CurrentLevel++;
            if (CurrentLevel > _levels.Count - 1)
            {
                CurrentLevel = Random.Range(0, _levels.Count);
            }
        }

        #endregion

        #region Private Methods

        private Vector3 GetRotationOnRoad(Vector3 position)
        {
            float timeToPoint = _pathCreator.path.GetClosestTimeOnPath(position);
            Quaternion rotation = _pathCreator.path.GetRotation(timeToPoint);
            Vector3 eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);

            return eulerAngles;
        }

        private Vector3 GetPositionOnRoad(Vector3 position)
        {
            Vector3 onRoadPosition = _pathCreator.path.GetClosestPointOnPath(position);

            return onRoadPosition;
        }

        #endregion
    }
}