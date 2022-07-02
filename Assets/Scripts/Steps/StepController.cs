using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class StepController : MonoBehaviour
    {
        #region Fields

        public const int STEP_COUNT = 21;
        public const float SPACE_BETWEEN_TWO_STEP = 3;

        [SerializeField] private List<Color> _colors;
        private List<Step> _steps;

        #endregion

        #region Public Methods

        public void Initialize(Quaternion initialRotation)
        {
            this.transform.eulerAngles = new Vector3(initialRotation.eulerAngles.x, initialRotation.eulerAngles.y, 0);

            _steps = this.GetComponentsInChildren<Step>(true).ToList();
            CreateSteps();
        }

        #endregion

        #region Private Methods

        private void CreateSteps()
        {
            decimal counter = 1.0m;
            for (int i = 0; i < STEP_COUNT; i++)
            {
                float newZ = (SPACE_BETWEEN_TWO_STEP * i);

                _steps[i].transform.localPosition = new Vector3(0, -0.9f, newZ);
                _steps[i].gameObject.name = "X" + counter;
                _steps[i].gameObject.SetActive(true);
                _steps[i].Initialize(counter, _colors[i]);
                counter += 0.2m;
            }
        }

        #endregion
    }
}