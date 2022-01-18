using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArrowScoreUI : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private TextMeshPro text;

    private void Awake()
    {
        text = this.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        this.transform.localPosition = target.localPosition - offset;
    }

    public void SetValue(int currentArrowCount)
    {
        text.text = currentArrowCount.ToString();
    }
}
