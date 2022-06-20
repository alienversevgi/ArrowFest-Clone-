using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StepController : MonoBehaviour
{
    private List<Step> steps;
    [SerializeField] private List<Color> colors;

    public const int STEP_COUNT = 21;
    public const float SPACE_BETWEEN_TWO_STEP = 3;

    public void Initialize(Quaternion initialRotation)
    {
        this.transform.eulerAngles = new Vector3(initialRotation.eulerAngles.x, initialRotation.eulerAngles.y, 0);

        steps = this.GetComponentsInChildren<Step>(true).ToList();
        CreateSteps();
    }

    private void CreateSteps()
    {
        decimal counter = 1.0m;
        for (int i = 0; i < STEP_COUNT; i++)
        {
            float newZ = (SPACE_BETWEEN_TWO_STEP * i);

            steps[i].transform.localPosition = new Vector3(0, -0.9f, newZ);
            steps[i].gameObject.name = "X" + counter;
            steps[i].gameObject.SetActive(true);
            steps[i].Initialize(counter, colors[i]);
            counter += 0.2m;
        }
    }
}