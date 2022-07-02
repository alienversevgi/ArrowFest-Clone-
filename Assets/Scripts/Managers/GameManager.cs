using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private ArrowController _arrowController;
        [SerializeField] private ArrowDeckManager _arrowDeckManager;
        [SerializeField] private LevelManager _levelManager;

        #endregion

        #region Unity Methods

        private void Start()
        {
            Initialize();
            StartGame();
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            _levelManager.Initialize();

            PoolManager.Instance.Initialize();
            EventManager.Instance.OnLevelCompleted.Register(NextLevel);
            EventManager.Instance.OnLevelFailed.Register(RestartLevel);
        }

        private void StartGame()
        {
            _levelManager.LoadLevel();
            _arrowController.Initialize(_levelManager.LevelRoadPath, _levelManager.RoadWith);
            _arrowDeckManager.Initialize();
        }

        private void NextLevel()
        {
            PoolManager.Instance.ResetAll();
            _levelManager.IncrementLevel();

            StartGame();
        }

        private void RestartLevel()
        {
            PoolManager.Instance.ResetAll();
            StartGame();
        }

        #endregion
    }
}