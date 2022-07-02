using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Game.Level;

namespace Game
{
    public class ArrowDeckManager : MonoBehaviour
    {
        #region Fields

        private const int CIRCLE_SEGMENT_INCREMENT_VALUE = 3;
        private const int CIRCLE_MIN_SEGMENT_COUNT = 5;
        private const float TAU = 6.28318530718f;
        private const float DEFAULT_RADIUS = 0.05f;

        [SerializeField] private ArrowScoreUI _arrowScoreUI;

        private float _xCurrentRadius = DEFAULT_RADIUS;
        private float _yCurrentRadius = DEFAULT_RADIUS;
        private float _xPlusRadius = DEFAULT_RADIUS;
        private float _yPlusRadius = DEFAULT_RADIUS;

        private int _circleCount;
        private int _plusValue;
        private int _currentArrowCount;
        private int _currentCircleSegmentCount;
        private int _currentSegmentIndex;

        private List<Arrow> _allArrow;
        private Dictionary<int, Stack<Arrow>> _arrows;

        private float _weight;
        private bool _isTransformationCompleted;
        private bool _isDeath;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            EventManager.Instance.OnDamageTaken.Register(ReduceArrow);
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            _arrows = new Dictionary<int, Stack<Arrow>>();
            _circleCount = -1;
            _currentArrowCount = 0;
            _isTransformationCompleted = false;
            _isDeath = false;

            CreateFirstArrow();
        }

        public void ApplyOperation(DoorValue doorValue)
        {
            int value = doorValue.Value;
            switch (doorValue.OperationType)
            {
                case OperationType.Addition:
                    PutToDeck(value);
                    break;
                case OperationType.Subtraction:
                    TakeToDeck(value);
                    break;
                case OperationType.Multiplication:
                    value = (_currentArrowCount * doorValue.Value) - _currentArrowCount;
                    Debug.Log(value + " " + _currentArrowCount + " " + doorValue);
                    PutToDeck(value);
                    break;
                case OperationType.Division:
                    if (doorValue.Value <= _currentArrowCount)
                        value = _currentArrowCount / doorValue.Value;
                    TakeToDeck(value);
                    break;
            }
        }

        public void OnStepReached(decimal stepIndex)
        {
            _isTransformationCompleted = true;
            _weight = GetWeight(stepIndex);

            RecalculateArrowPositions(_weight);
        }

        private float GetWeight(decimal stepIndex)
        {
            switch (stepIndex)
            {
                case -1.0m:
                    _weight = 0.0f;
                    break;
                case 1.0m:
                    _weight = 0.11f;
                    break;
                case 1.2m:
                    _weight = 0.27f;
                    break;
                case 1.4m:
                    _weight = 0.54f;
                    break;
                default:
                    _weight = 0.8f;
                    break;
            }
            return _weight;
        }

        #endregion

        #region Private Methods

        private void SetArrowCount(int value)
        {
            _currentArrowCount = value;
            _arrowScoreUI.SetValue(_currentArrowCount);
        }

        private void CreateFirstArrow()
        {
            Arrow clone = CreateArrowInstance(0, 0);
            clone.name = "BaseArrow";
            SetArrowCount(1);
        }

        private Arrow CreateArrowInstance(float x, float y)
        {
            Arrow instanceArrow = PoolManager.Instance.ArrowPool.Allocate();
            instanceArrow.transform.SetParent(this.transform);
            instanceArrow.SetLocalPositionAndEnable(new Vector3(x, y));
            instanceArrow.transform.localEulerAngles = Vector3.zero;

            if (!_arrows.ContainsKey(_circleCount))
            {
                _arrows.Add(_circleCount, new Stack<Arrow>());
            }

            _arrows[_circleCount].Push(instanceArrow);

            return instanceArrow;
        }

        private void ReduceArrow()
        {
            TakeToDeck(1);

            if (_isTransformationCompleted)
            {
                RecalculateArrowPositions(_weight);
            }
        }

        private void PutToDeck(int value)
        {
            int counter = value;
            int remainingValue = 0;

            if (_currentCircleSegmentCount == 0)
            {
                _currentCircleSegmentCount = CIRCLE_MIN_SEGMENT_COUNT + _plusValue;
                _circleCount++;
                _currentSegmentIndex = 0;
            }

            if ((_currentSegmentIndex + value) > _currentCircleSegmentCount)
            {
                remainingValue = (_currentSegmentIndex + value) - _currentCircleSegmentCount;
            }

            for (int index = _currentSegmentIndex; index < _currentCircleSegmentCount; index++)
            {
                if (counter <= 0)
                {
                    break;
                }

                float t = index / (float)_currentCircleSegmentCount;
                float radian = t * TAU;

                float x = Mathf.Sin(radian) * _xCurrentRadius;
                float y = Mathf.Cos(radian) * _yCurrentRadius;

                Arrow clone = CreateArrowInstance(x, y);
                clone.name = $"{_circleCount}.circle {index}.segment + {_currentArrowCount}";
                SetArrowCount(++_currentArrowCount);
                counter--;
            }

            _currentSegmentIndex += value;

            if (_currentSegmentIndex >= _currentCircleSegmentCount)
            {
                _currentCircleSegmentCount = 0;
                _xCurrentRadius += _xPlusRadius;
                _yCurrentRadius += _yPlusRadius;
                _plusValue += CIRCLE_SEGMENT_INCREMENT_VALUE;
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
                if (_currentArrowCount == 0)
                    break;

                if (_arrows[_circleCount].Count == 0)
                {
                    SetupPreviousCircleSettings();
                    remainingValue = value - remainingValue;

                    if (remainingValue >= 0)
                        TakeToDeck(remainingValue);
                }
                else
                {
                    remainingValue++;
                    PoolManager.Instance.ArrowPool.Release(_arrows[_circleCount].Peek());
                    _arrows[_circleCount].Pop();
                    _currentSegmentIndex--;
                    SetArrowCount(--_currentArrowCount);

                    if (_currentSegmentIndex == 0 && _circleCount != -1)
                        SetupPreviousCircleSettings();
                }
            }

            if (_currentArrowCount == 0 && _isDeath == false)
            {
                Debug.LogError("Death");
                _isDeath = true;
                EventManager.Instance.OnOutOfArrow.Raise();
                _circleCount = -1;
                _currentSegmentIndex = 0;
                _currentCircleSegmentCount = 0;
                _xCurrentRadius = _xPlusRadius;
                _yCurrentRadius = _yPlusRadius;
            }
        }

        private void RecalculateArrowPositions(float weight)
        {
            float currentX = -weight;
            _allArrow = _arrows.SelectMany(it => it.Value).ToList();

            if (_currentArrowCount % 2 != 0)
                _allArrow[_currentArrowCount - 1].transform.DOLocalMove(new Vector3(0, 0), .2f);

            List<Arrow> left = _allArrow.Take(_allArrow.Count / 2).ToList();
            List<Arrow> right = _allArrow.Skip(_allArrow.Count / 2).Take(_allArrow.Count / 2).ToList();
            float space = weight / (_allArrow.Count / 2);

            currentX = _currentArrowCount % 2 == 0 ? -space * .5f : -space;
            for (int i = left.Count - 1; i >= 0; i--)
            {
                left[i].transform.localPosition = new Vector3(currentX, 0);
                currentX -= space;
            }

            currentX = _currentArrowCount % 2 == 0 ? space * .5f : space;
            for (int i = right.Count - 1; i >= 0; i--)
            {
                right[i].transform.localPosition = new Vector3(currentX, 0);
                currentX += space;
            }
        }

        private void SetupPreviousCircleSettings()
        {
            _currentCircleSegmentCount = _arrows.ContainsKey(_circleCount - 1) ? _arrows[_circleCount - 1].Count : 0;
            _currentSegmentIndex = _currentCircleSegmentCount;
            _circleCount--;
            _xCurrentRadius -= _xPlusRadius;
            _yCurrentRadius -= _yPlusRadius;
            _plusValue = Mathf.Clamp(_plusValue - CIRCLE_SEGMENT_INCREMENT_VALUE, 0, int.MaxValue);
        }

        #endregion
    }
}