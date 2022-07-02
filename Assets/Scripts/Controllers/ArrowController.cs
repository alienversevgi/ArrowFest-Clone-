using UnityEngine;
using DG.Tweening;
using PathCreation;
using System;
using Utility;

namespace Game
{
    public class ArrowController : MonoBehaviour
    {
        #region Fields

        private const float RANGE_OFFSET = 0.1f;

        [SerializeField] private float _slideSpeed = 0.05f;
        [SerializeField] private float _speed = 3.0f;
        [SerializeField] private float _stairRunSpeed = 6.0f;

        [SerializeField] private ArrowDeckManager _arrowDeck;

        private Vector3 _first;
        private Vector3 _second;
        private Vector3 _distancePositionBetweenTwoFrame;

        private Vector3 _cameraDefaultPosition = new Vector3(0.0f, 1.4f, -2.9f);
        private Vector3 _cameraZoomOutPosition = new Vector3(0.0f, 3.5f, -6.5f);

        private VertexPath _path;

        private float _dstTravelled;
        private float _range;

        private bool _isActivated;
        private bool _isPathCompleted;
        private bool _isFirstInputDetected;
        private bool _isStairRunActive;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (!_isActivated)
                return;

            InputDetection();
            FollowPath();
            Slide();
            ApplyScaleForSlide();
            MoveInfinity();
        }

        private void OnEnable()
        {
            EventManager.Instance.OnOutOfArrow.Register(ArrowDeck_OnOutOfArrow);
            EventManager.Instance.OnLevelLoaded.Register(Activate);
        }

        #endregion

        #region Public Methods

        public void Initialize(VertexPath followPath, float width)
        {
            _path = followPath;
            _range = width - RANGE_OFFSET;

            this.transform.position = _path.GetPoint(0);
            Vector3 rotation = new Vector3(_path.GetRotation(0).eulerAngles.x, _path.GetRotation(0).eulerAngles.y, 0);
            this.transform.eulerAngles = rotation;

            Camera.main.transform.localPosition = _cameraDefaultPosition;

            _dstTravelled = 0;
            _isActivated = false;
            _isFirstInputDetected = false;
            _isPathCompleted = false;
            _isStairRunActive = false;
        }

        #endregion

        #region Private Methods

        private void Activate()
        {
            _isActivated = true;
        }

        private void ArrowDeck_OnOutOfArrow()
        {
            Action action = null;
            if (_isStairRunActive)
            {
                _isStairRunActive = false;
                action = () => EventManager.Instance.OnLevelCompleted.Raise();
            }
            else
            {
                _isPathCompleted = true;
                action = () => EventManager.Instance.OnLevelFailed.Raise();
            }

            Timer.Instance.StartTimer(.5f, action);
        }

        private void ApplyScaleForSlide()
        {
            float distance = Mathf.Abs(_arrowDeck.transform.localPosition.x);
            _arrowDeck.transform.DOScaleX(Mathf.Clamp(_range * 2 - distance, .5f, 1), .1f);
        }

        private void InputDetection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _first = Input.mousePosition;
                _second = _first;
                _isFirstInputDetected = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _distancePositionBetweenTwoFrame = Vector3.zero;
            }

            if (Input.GetMouseButton(0))
            {
                _first = Input.mousePosition;
                _distancePositionBetweenTwoFrame = _first - _second;
            }

            _second = _first;
        }

        private void FollowPath()
        {
            if (!_isFirstInputDetected || _isPathCompleted)
                return;

            _dstTravelled += _speed * Time.deltaTime;
            this.transform.position = _path.GetPointAtDistance(_dstTravelled, EndOfPathInstruction.Stop) + new Vector3(0, .4f, 0);
            Quaternion pathRotation = _path.GetRotationAtDistance(_dstTravelled, EndOfPathInstruction.Stop);
            Vector3 rotation = new Vector3(pathRotation.eulerAngles.x, pathRotation.eulerAngles.y, 0);
            this.transform.eulerAngles = rotation;

            if (_path.GetPoint(_path.NumPoints - 1).DistanceAnAxis(transform.position, Axis.Z) < 4.0f)
            {
                Debug.LogError("Finished");
                _isPathCompleted = true;
                _isStairRunActive = true;
                _arrowDeck.OnStepReached(1.0m);
                _arrowDeck.transform.DOLocalMoveX(0, .5f);
                Camera.main.transform.DOLocalMove(_cameraZoomOutPosition, 1.0f);
            }
        }

        private void Slide()
        {
            if (_isStairRunActive)
                return;

            Vector3 horizontal = new Vector3(_distancePositionBetweenTwoFrame.x, 0, 0);

            Vector3 newPosition = _arrowDeck.transform.localPosition + (_slideSpeed * horizontal) * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, -_range, _range);

            _arrowDeck.transform.localPosition = _arrowDeck.transform.localPosition.SetOnlyOneAxisVector(Axis.X, newPosition.x);
        }

        private void MoveInfinity()
        {
            if (!_isStairRunActive)
                return;

            this.transform.Translate(Vector3.forward * Time.deltaTime * _stairRunSpeed);
        }

        #endregion
    }
}