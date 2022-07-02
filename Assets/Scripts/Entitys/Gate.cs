using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnverPool;

namespace Game.Level
{
    public class Gate : Entity
    {
        #region Fields

        [SerializeField] private Door _leftDoor;
        [SerializeField] private Door _rightDoor;

        #endregion

        #region Public Methods

        public void Initialize(DoorValue left, DoorValue right)
        {
            _leftDoor.Initialize(left, OnPassed);
            _rightDoor.Initialize(right, OnPassed);
        }

        public override void Reset()
        {
            base.Reset();
            DisableDoors();
        }

        #endregion

        #region Private Methods

        private void OnPassed()
        {
            DisableDoors();
            Utility.Timer.Instance.StartTimer(.5f, () => PoolManager.Instance.GatePool.Release(this));
        }

        private void DisableDoors()
        {
            _leftDoor.Disable();
            _rightDoor.Disable();
        }

        #endregion
    }
}
