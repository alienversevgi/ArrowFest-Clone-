using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour, IEntity
{
    public GameObject ghostPrefab;
    public event Action<int> OnArrowHitted;

    private Collider collider;

    private void Awake()
    {
        collider = this.GetComponent<Collider>();
    }

    public void Reset()
    {
        this.transform.position = Vector3.zero;
        this.transform.eulerAngles = Vector3.zero;
        this.gameObject.SetActive(false);
    }

    public void SetPositionAndEnable(Vector3 newPosition)
    {
        this.transform.localPosition = newPosition;
        this.gameObject.SetActive(true);
    }
 
    //private void OnCollisionEnter(Collision collision)
    //{
    //    var chibi = collision.gameObject.GetComponent<Chibi>();

    //    if (collision != null && collision.gameObject.CompareTag("Chibi"))
    //    {
    //        if (!chibi.IsDead)
    //        {
    //            collider.isTrigger = true;
    //        }
    //    }
    //}
  
    //private void OnCollisionExit(Collision collision)
    //{
    //    collider.isTrigger = false;
    //}
}