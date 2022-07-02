using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;

namespace Game.Level
{
    public class Door : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TextMeshPro _valueText;

        private Color _negativeColor = Color.red;
        private Color _positiveColor = Color.cyan;

        private DoorValue _doorValue;
        private Collider _collider;
        private Action _onPassedCallBack;
        private Sequence _horizontalMoveSequence;

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ArrowDeckManager>() != null)
            {
                _onPassedCallBack();
                other.GetComponent<ArrowDeckManager>().ApplyOperation(_doorValue);
                this.gameObject.SetActive(false);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(DoorValue doorValue, Action passedCallBack)
        {
            _doorValue = doorValue;
            gameObject.SetActive(doorValue.IsEnable);

            if (!doorValue.IsEnable)
                return;

            _collider = this.GetComponent<Collider>();
            _collider.enabled = true;
            _onPassedCallBack = passedCallBack;

            SetupDoor();
        }

        #endregion

        #region Private Methods

        private void SetupDoor()
        {
            _valueText.text = $"{GetOperationSymbol()}{_doorValue.Value.ToString()}";
            Color color = GetDoorColor(_doorValue);

            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor("_Color", color);

            this.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);

            if (_doorValue.HasMoveable)
            {
                HorizontalMove();
            }
        }

        private void HorizontalMove()
        {
            _horizontalMoveSequence = DOTween.Sequence();
            _horizontalMoveSequence.Append(this.transform.DOLocalMoveX(4.77f, 1f).SetEase(Ease.InOutSine));
            _horizontalMoveSequence.Append(this.transform.DOLocalMoveX(-4.77f, 1f).SetEase(Ease.InOutSine));
            _horizontalMoveSequence.SetLoops(-1);
        }

        public void Disable()
        {
            _horizontalMoveSequence.Kill();

            if (gameObject.activeInHierarchy)
            {
                _collider.enabled = false;
            }
        }

        private Color GetDoorColor(DoorValue doorValue)
        {
            Color color = Color.black;

            if (doorValue.OperationType == OperationType.Addition || doorValue.OperationType == OperationType.Multiplication)
                color = _positiveColor;
            else
                color = _negativeColor;

            return color;
        }

        private char GetOperationSymbol()
        {
            char symbol = '$';
            switch (_doorValue.OperationType)
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

        #endregion
    }
}