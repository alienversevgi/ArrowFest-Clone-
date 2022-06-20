using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;

public class Door : MonoBehaviour
{
    private Color negativeColor = Color.red;
    private Color positiveColor = Color.cyan;/*new Color32(80,221,255,255);*/

    [SerializeField] private TextMeshPro text;
    private DoorValue doorValue;
    private Collider collider;
    private Action passed;

    public void Initialize(DoorValue doorValue, Action passedCallBack)
    {
        this.doorValue = doorValue;
        gameObject.SetActive(doorValue.IsEnable);
        if (!doorValue.IsEnable)
            return;

        collider = this.GetComponent<Collider>();
        collider.enabled = true;
        this.gameObject.SetActive(true);
        passed = passedCallBack;

        text.text = $"{GetOperationSymbol()}{doorValue.Value.ToString()}";
        Color color = GetDoorColor(doorValue);
        this.GetComponent<Renderer>().material.color = color;

        if (doorValue.HasMoveable)
        {
            HorizontalMove();
        }
    }

    private void HorizontalMove()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(this.transform.DOLocalMoveX(4.77f, 1f).SetEase(Ease.InOutSine));
        sequence.Append(this.transform.DOLocalMoveX(-4.77f, 1f).SetEase(Ease.InOutSine));
        sequence.SetLoops(-1);
    }

    public void Disable()
    {
        collider.enabled = false;
    }

    private Color GetDoorColor(DoorValue doorValue)
    {
        Color color = Color.black;

        if (doorValue.OperationType == OperationType.Addition || doorValue.OperationType == OperationType.Multiplication)
            color = positiveColor;
        else
            color = negativeColor;

        return color;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.LogError("OnTrigger " + text.text);
        if (other != null)
        {
            //Debug.Log(other.gameObject.name);
        }
        if (other.GetComponent<ArrowDeckManager>() != null)
        {
            passed();
            other.GetComponent<ArrowDeckManager>().ApplyOperation(doorValue);
            this.gameObject.SetActive(false);
        }
    }

    private char GetOperationSymbol()
    {
        char symbol = '$';
        switch (doorValue.OperationType)
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
}