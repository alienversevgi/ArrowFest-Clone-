using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDeckManager : MonoBehaviour
{
    private const int CIRCLE_SEGMENT_INCREMENT_VALUE = 3;
    private const int CIRCLE_MIN_SEGMENT_COUNT = 5;
    private const float TAU = 6.28318530718f;
    private const float DEFAULT_RADIUS = 0.05f;

    [SerializeField] private float xCurrentRadius = DEFAULT_RADIUS;
    [SerializeField] private float yCurrentRadius = DEFAULT_RADIUS;
    [SerializeField] private float xPlusRadius = DEFAULT_RADIUS;
    [SerializeField] private float yPlusRadius = DEFAULT_RADIUS;

    [SerializeField] private int circleCount = -1;
    [SerializeField] private int plusValue;
    [SerializeField] private int currentArrowCount;
    [SerializeField] private int currentCircleSegmentCount;
    [SerializeField] private int currentSegmentIndex;

    public ArrowScoreUI arrowScoreUI;

    private Dictionary<int, Stack<GameObject>> arrows;
    private Dictionary<int, Color> arrowsColor;

    public int DeckCount;

    public GameObject prefab;

    private void Awake()
    {
        arrows = new Dictionary<int, Stack<GameObject>>();
        arrowsColor = new Dictionary<int, Color>();
        PutToDeck(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            PutToDeck(DeckCount);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (KeyValuePair<int, Stack<GameObject>> item in arrows)
            {
                Debug.Log($"key = {item.Key}");
                foreach (var arrow in item.Value)
                {
                    Debug.Log(arrow.name);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeToDeck(DeckCount);
        }
    }

    private void CreateFirstArrow()
    {
        Arrow clone = CreateArrowInstance(0, 0);
        clone.name = "baseArrow";
        currentArrowCount = 1;
    }

    private Arrow CreateArrowInstance(float x, float y)
    {
        GameObject clone = Instantiate(prefab);
        clone.transform.SetParent(this.transform);
        clone.transform.localPosition = new Vector3(x, y);
        clone.transform.localEulerAngles = Vector3.zero;
        clone.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        if (!arrows.ContainsKey(circleCount))
        {
            arrows.Add(circleCount, new Stack<GameObject>());
            arrowsColor.Add(circleCount, Random.ColorHSV());
        }

        arrows[circleCount].Push(clone);

        return clone.GetComponent<Arrow>();
    }

    private char GetOperationSymbol(OperationType type)
    {
        char symbol = '$';
        switch (type)
        {
            case OperationType.Addition:
                symbol = '+';
                break;
            case OperationType.Subtraction:
                symbol = '-';
                break;
            case OperationType.Multiplication:
                symbol = 'X';
                break;
            case OperationType.Division:
                symbol = '%';
                break;
            default:
                break;
        }

        return symbol;
    }

    public void OperationBlaBla(DoorValue doorValue)
    {
        Debug.Log($"current : {currentArrowCount}, {GetOperationSymbol(doorValue.OperationType)}{doorValue.Value}");
        switch (doorValue.OperationType)
        {
            case OperationType.Addition:
                PutToDeck(doorValue.Value);
                break;
            case OperationType.Subtraction:
                TakeToDeck(doorValue.Value);
                break;
            case OperationType.Multiplication:
                doorValue.Value = (currentArrowCount * doorValue.Value) - currentArrowCount;
                PutToDeck(doorValue.Value);
                break;
            case OperationType.Division:
                if (doorValue.Value <= currentArrowCount)
                    doorValue.Value = currentArrowCount / doorValue.Value;
                TakeToDeck(doorValue.Value);
                break;
        }
        arrowScoreUI.SetValue(currentArrowCount);
        Debug.Log("Result : " + currentArrowCount);
    }

    private void PutToDeck(int value)
    {
        int counter = value;
        int remainingValue = 0;

        if (currentArrowCount == 0)
        {
            CreateFirstArrow();
            return;
        }

        if (currentCircleSegmentCount == 0)
        {
            currentCircleSegmentCount = CIRCLE_MIN_SEGMENT_COUNT + plusValue;
            circleCount++;
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

            Arrow clone = CreateArrowInstance(x, y);
            clone.name = $"{circleCount}.circle {index}.segment + {currentArrowCount}";
            clone.GetComponent<MeshRenderer>().material.color = arrowsColor[circleCount];
            currentArrowCount++;
            counter--;
        }

        currentSegmentIndex += value;

        if (currentSegmentIndex >= currentCircleSegmentCount)
        {
            currentCircleSegmentCount = 0;
            xCurrentRadius += xPlusRadius;
            yCurrentRadius += yPlusRadius;
            plusValue += CIRCLE_SEGMENT_INCREMENT_VALUE;
        }

        if (remainingValue > 0)
        {
            PutToDeck(remainingValue);
        }
    }

    private void TakeToDeck(int value)
    {
        int remainingValue = 0;

        for (int i = 0; i < value; i++)
        {
            if (arrows[circleCount].Count == 0)
            {
                SetupPreviousCircleSettings();
                remainingValue = value - remainingValue;

                if (remainingValue >= 0)
                    TakeToDeck(remainingValue);
            }
            else
            {
                remainingValue++;
                Destroy(arrows[circleCount].Peek());
                arrows[circleCount].Pop();
                currentSegmentIndex--;
                currentArrowCount--;

                if (currentSegmentIndex == 0 && circleCount != -1)
                    SetupPreviousCircleSettings();
            }
        }

        if (arrows[circleCount].Count == 0 && circleCount == -1)
        {
            Debug.LogError("Death");
            circleCount = -1;
            currentSegmentIndex = 0;
            currentCircleSegmentCount = 0;
            xCurrentRadius = xPlusRadius;
            yCurrentRadius = yPlusRadius;
        }
    }

    private void SetupPreviousCircleSettings()
    {
        currentCircleSegmentCount = arrows.ContainsKey(circleCount - 1) ? arrows[circleCount - 1].Count : 0;
        currentSegmentIndex = currentCircleSegmentCount;
        circleCount--;
        xCurrentRadius -= xPlusRadius;
        yCurrentRadius -= yPlusRadius;
        plusValue = Mathf.Clamp(plusValue - CIRCLE_SEGMENT_INCREMENT_VALUE, 0, int.MaxValue);
    }
}