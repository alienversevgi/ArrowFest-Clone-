using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour,IResettable
{
    public void Reset()
    {
        this.transform.position = Vector3.zero;
        this.transform.eulerAngles = Vector3.zero;
        this.gameObject.SetActive(false);
    }
}