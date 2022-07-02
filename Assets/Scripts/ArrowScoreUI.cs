using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArrowScoreUI : MonoBehaviour
{
    #region Fields

    [SerializeField] private Transform _target;
    [SerializeField] private TextMeshPro _text;

    private Vector3 _offset = new Vector3(0.0f, -0.16f, 0.6f);

    #endregion

    #region Unity Methods

    private void Update()
    {
        this.transform.localPosition = _target.localPosition - _offset;
    }

    #endregion

    #region Public Methods

    public void SetValue(int currentArrowCount)
    {
        _text.text = currentArrowCount.ToString();
    }

    #endregion
}