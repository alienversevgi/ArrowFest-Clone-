using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float t;

    [SerializeField] private Vector3 offset;
    private void Start()
    {
      //  offset = this.transform.position - target.position;
    }

    void FixedUpdate()
    {
        Vector3 newPos = target.position + offset;
        this.transform.position = Vector3.Slerp(this.transform.position, newPos, t);
        this.transform.DOLookAt(target.position, .1F, AxisConstraint.X);
    }
}
