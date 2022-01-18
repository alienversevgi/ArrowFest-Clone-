using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Door leftDoor;
    [SerializeField] private Door rightDoor;

    private void Start()
    {
        DoorValue left = new DoorValue(Random.Range(1, 10), OperationType.Multiplication);
        DoorValue right = new DoorValue(Random.Range(1, 10), OperationType.Division);
        Initialize(left, right);
    }

    public void Initialize(DoorValue leftDoorValue, DoorValue rightDoorValue)
    {
        leftDoor.Initialize(leftDoorValue, OnPassed);
        rightDoor.Initialize(rightDoorValue, OnPassed);
    }

    private void OnPassed()
    {
        leftDoor.Disable();
        rightDoor.Disable();
    }
}
