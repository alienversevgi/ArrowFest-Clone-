using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Level;
using EnverPool;

namespace Game
{
    public class PoolManager : Singleton<PoolManager>
    {
        #region Fields

        [SerializeField] private GameObject _arrowPrefab;
        [SerializeField] private GameObject _gatePrefab;
        [SerializeField] private GameObject _chibiPrefab;
        [SerializeField] private GameObject _chibiGroupPrefab;
        [SerializeField] private GameObject _ghostArrowPrefab;

        public Pool<Arrow> ArrowPool { get; private set; }
        public Pool<Gate> GatePool { get; private set; }
        public Pool<Chibi> ChibiPool { get; private set; }
        public Pool<ChibiGroup> ChibiGroupPool { get; private set; }
        public Pool<Entity> GhostArrowPool { get; private set; }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            ArrowPool = new Pool<Arrow>(new PrefabFactory<Arrow>(_arrowPrefab, "Arrows"), 300);
            GatePool = new Pool<Gate>(new PrefabFactory<Gate>(_gatePrefab, "Gates"), 10);
            ChibiPool = new Pool<Chibi>(new PrefabFactory<Chibi>(_chibiPrefab, "Chibis"), 10);
            ChibiGroupPool = new Pool<ChibiGroup>(new PrefabFactory<ChibiGroup>(_chibiGroupPrefab, "ChibiGroups"), 2);
            GhostArrowPool = new Pool<Entity>(new PrefabFactory<Entity>(_ghostArrowPrefab, "GhostArrows"), 150);
        }

        public void ResetAll()
        {
            ArrowPool.ReleaseAll();
            GatePool.ReleaseAll();
            ChibiPool.ReleaseAll();
            ChibiGroupPool.ReleaseAll();
            GhostArrowPool.ReleaseAll();
        }

        #endregion
    }
}