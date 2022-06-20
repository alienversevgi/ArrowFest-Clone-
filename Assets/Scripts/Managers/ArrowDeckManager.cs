using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class ArrowDeckManager : MonoBehaviour
{
    #region Fields

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
    [SerializeField] public List<Arrow> allArrow;

    public ArrowScoreUI arrowScoreUI;

    private Dictionary<int, Stack<Arrow>> arrows;
    private Dictionary<int, Color> arrowsColor;

    public int DeckCount;

    public GameObject GhostArrow;
    private bool isAllStepsCompleted;
    private float weight = 0;
    private float minSpaceBetweenTwoArrow = 0.02f; // 0.02f
    private float maxFirst = 16;
    private float oneArrowSize = 0.02f;
    public float size;
    private bool isTransformationCompleted;

    #endregion

    #region Unity Methods
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            //PutToDeck(DeckCount);
            OnStepReached(1.0m);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PutToDeck(10);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TakeToDeck(4);
        }
    }

    #endregion

    #region Public Methods

    public void Initialize()
    {
        arrows = new Dictionary<int, Stack<Arrow>>();
        arrowsColor = new Dictionary<int, Color>();
        PutToDeck(1);
        EventManager.Instance.OnDamageTaken.AddListener(ReduceArrow);
    }

    public void ApplyOperation(DoorValue doorValue)
    {
        //Debug.Log($"current : {currentArrowCount}, {GetOperationSymbol(doorValue.OperationType)}{doorValue.Value}");
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
        //Debug.Log("Result : " + currentArrowCount);
    }

    public void OnStepReached(decimal stepIndex)
    {
        isTransformationCompleted = true;

        switch (stepIndex)
        {
            case 1.0m:
                weight = 0.11f;
                break;
            case 1.2m:
                weight = 0.26f;
                break;
            case 1.4m:
                weight = 0.51f;
                break;
            default:
                weight = 0.71f;
                break;
        }

        RecalculateArrowPositions(weight);
    }

    #endregion

    #region Private Methods

    private void SetArrowCount(int value)
    {
        currentArrowCount = value;
        arrowScoreUI.SetValue(currentArrowCount);
    }

    private void CreateFirstArrow()
    {
        Arrow clone = CreateArrowInstance(0, 0);
        clone.name = "baseArrow";
        SetArrowCount(1);
    }

    private Arrow CreateArrowInstance(float x, float y)
    {
        Arrow instanceArrow = PoolManager.Instance.ArrowPool.Allocate();
        instanceArrow.transform.SetParent(this.transform);
        instanceArrow.SetPositionAndEnable(new Vector3(x, y));
        instanceArrow.transform.localEulerAngles = Vector3.zero;
        //instanceArrow.OnArrowHitted += InstanceArrow_OnArrowHitted;
        //clone.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        //instanceArrow.transform.localScale = prefab.transform.localScale; aktife

        if (!arrows.ContainsKey(circleCount))
        {
            arrows.Add(circleCount, new Stack<Arrow>());
            arrowsColor.Add(circleCount, Random.ColorHSV());
        }

        arrows[circleCount].Push(instanceArrow);

        return instanceArrow;
    }

    private void ReduceArrow(int hitPoint)
    {
        //Debug.Log("hp" + hitPoint);
        TakeToDeck(hitPoint);

        if(isTransformationCompleted)
            RecalculateArrowPositions(weight);
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
            SetArrowCount(++currentArrowCount);
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
            if (currentArrowCount == 0)
                break;

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
                PoolManager.Instance.ArrowPool.Release(arrows[circleCount].Peek());
                arrows[circleCount].Pop();
                currentSegmentIndex--;
                SetArrowCount(--currentArrowCount);

                if (currentSegmentIndex == 0 && circleCount != -1)
                    SetupPreviousCircleSettings();
            }
        }

        if (currentArrowCount == 0)
        {
            Debug.LogError("Death");
            isAllStepsCompleted = true;
            EventManager.Instance.OutOfArrow.Invoke();
            circleCount = -1;
            currentSegmentIndex = 0;
            currentCircleSegmentCount = 0;
            xCurrentRadius = xPlusRadius;
            yCurrentRadius = yPlusRadius;
        }
    }

    private void RecalculateArrowPositions(float weight)
    {
        float q = weight * 2 / (currentArrowCount - 2);
        float currentX = -weight;
        allArrow = new List<Arrow>();
        allArrow = arrows.SelectMany(it => it.Value).ToList();
        
        //for (int i = circleCount; i >= -1; i--)
        //{
        //    allArrow.AddRange(arrows[i]);
        //}

        if (currentArrowCount % 2 != 0)
            allArrow[currentArrowCount - 1].transform.DOLocalMove(new Vector3(0, 0), .2f);

        //allArrow = allArrow.OrderBy(arrow => arrow.transform.localPosition.x).ToList();
        List<Arrow> left = allArrow.Take(allArrow.Count / 2).ToList();
        List<Arrow> right = allArrow.Skip(allArrow.Count / 2).Take(allArrow.Count / 2).ToList();
        float space = weight / (allArrow.Count / 2);

        currentX = currentArrowCount % 2 == 0 ? -space * .5f : -space;
        for (int i = left.Count - 1; i >= 0; i--)
        {
            left[i].transform.localPosition = new Vector3(currentX, 0);
            currentX -= space;
        }

        currentX = currentArrowCount % 2 == 0 ? space * .5f : space;
        for (int i = right.Count - 1; i >= 0; i--)
        {
            right[i].transform.localPosition = new Vector3(currentX, 0);
            currentX += space;
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

    #endregion
}