using System;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Color negativeColor = Color.red;
    private Color positiveColor = Color.green;

    private TextMeshPro text;
    private DoorValue doorValue;
    private Collider collider;
    private Action passed;

    private void Awake()
    {
        collider = this.GetComponent<Collider>();
    }

    public void Initialize(DoorValue doorValue, Action passedCallBack)
    {
        this.doorValue = doorValue;
        collider.enabled = true;
        this.gameObject.SetActive(true);
        passed = passedCallBack;

        text = this.transform.GetChild(0).GetComponent<TextMeshPro>();
        text.text = $"{GetOperationSymbol()} {doorValue.Value.ToString()}";
        Color color = GetDoorColor(doorValue);

        this.GetComponent<Renderer>().material.color = color;
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
        Debug.LogError("OnTrigger " + text.text);
        if (other != null)
        {
            Debug.Log(other.gameObject.name);
        }
        if (other.GetComponent<ArrowDeckManager>() != null)
        {
            passed();
            other.GetComponent<ArrowDeckManager>().OperationBlaBla(doorValue);
            this.gameObject.SetActive(false);
        }
    }

    private char GetOperationSymbol()
    {
        char symbol = '$';
        switch (doorValue.OperationType)
        {
            case OperationType.Addition: symbol = '+';
                break;
            case OperationType.Subtraction: symbol = '-';
                break;
            case OperationType.Multiplication: symbol = 'X';
                break;
            case OperationType.Division: symbol = '%';
                break;
            default:
                break;
        }

        return symbol;
    }
}