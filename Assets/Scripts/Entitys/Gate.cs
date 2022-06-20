using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IEntity
{
    [SerializeField] private Door leftDoor;
    [SerializeField] private Door rightDoor;

    public void Initialize(DoorValue left, DoorValue right)
    {
        leftDoor.Initialize(left, OnPassed);
        rightDoor.Initialize(right, OnPassed);
    }

    private void OnPassed()
    {
        leftDoor.Disable();
        rightDoor.Disable();
    }

    public void SetPositionAndEnable(Vector3 newPosition)
    {
        this.transform.position = newPosition;
        this.gameObject.SetActive(true);
    }

    public void Reset()
    {
        this.gameObject.SetActive(false);
        this.transform.position = Vector3.zero;
    }
}
