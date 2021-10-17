using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SwordController : MonoBehaviour
{
    private const float MIN_X = -1.1f;
    private const float MAX_X = 1.1f;
    private const int CIRCLE_SEGMENT_INCREMENT_VALUE = 3;
    private const int CIRCLE_MIN_SEGMENT_COUNT = 5;
    private const float TAU = 6.28318530718f;

    private Vector3 first;
    private Vector3 second;
    private Vector3 distancePositionBetweenTwoFrame;

    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float cameraFollowSpeed = 0.7f;
    [SerializeField] private Camera camera;

    [SerializeField] private float xCurrentRadius;
    [SerializeField] private float yCurrentRadius;
    [SerializeField] private float xPlusRadius;
    [SerializeField] private float yPlusRadius;

    [SerializeField] private int circleCount;
    [SerializeField] private int plusValue;
    [SerializeField] private int currentArrowCount;
    [SerializeField] private int currentCircleSegmentCount;
    [SerializeField] private int currentSegmentIndex;

    public GameObject prefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            PutToDeck(20);
        }

        InputDetection();
        Move();
        ScaleForMovement();
    }

    private void ScaleForMovement()
    {
        float distance = Mathf.Abs(this.transform.position.x); // origin is Vector3.zero
        this.transform.DOScaleX(Mathf.Clamp(1.4f - distance, .5f, 1), .1f);
    }

    private void InputDetection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            first = Input.mousePosition;
            second = first;
        }

        if (Input.GetMouseButtonUp(0))
        {
            distancePositionBetweenTwoFrame = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            first = Input.mousePosition;
            distancePositionBetweenTwoFrame = first - second;
        }

        second = first;
    }

    private void Move()
    {
        Vector3 horizontal = new Vector3(distancePositionBetweenTwoFrame.x, 0, 0);

        Vector3 newPosition = this.transform.position + (Vector3.forward * speed + horizontal) * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, MIN_X, MAX_X);

        this.transform.position = transform.position.SetOnlyOneAxisVector(Axis.X, newPosition.x);
    }

    private void PutToDeck(int value)
    {
        int counter = value;
        int remainingValue = 0;

        if (currentCircleSegmentCount == 0)
        {
            currentCircleSegmentCount = CIRCLE_MIN_SEGMENT_COUNT + plusValue;
            currentSegmentIndex = 0;
        }

        if ((currentSegmentIndex + value) > currentCircleSegmentCount)
        {
            remainingValue = (currentSegmentIndex + value) - currentCircleSegmentCount;
        }

        for (int index = currentSegmentIndex; index < currentCircleSegmentCount; index++)
        {
            if (counter <= 0)
            {
                break;
            }

            float t = index / (float)currentCircleSegmentCount;
            float radian = t * TAU;

            float x = Mathf.Sin(radian) * xCurrentRadius;
            float y = Mathf.Cos(radian) * yCurrentRadius;

            GameObject clone = Instantiate(prefab);
            clone.transform.SetParent(this.transform);
            clone.transform.localPosition = new Vector3(x, y);
            clone.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            clone.name = $"{circleCount}.circle {index}.segment + {currentArrowCount}";

            currentArrowCount++;
            counter--;
        }

        currentSegmentIndex += value;

        if (currentSegmentIndex >= currentCircleSegmentCount)
        {
            currentCircleSegmentCount = 0;
            circleCount++;
            xCurrentRadius += xPlusRadius;
            yCurrentRadius += yPlusRadius;
            plusValue += CIRCLE_SEGMENT_INCREMENT_VALUE;
        }

        if (remainingValue > 0)
        {
            PutToDeck(remainingValue);
        }
    }
}