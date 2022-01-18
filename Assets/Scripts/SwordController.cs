using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathCreation;

public class SwordController : MonoBehaviour
{
    private Vector3 first;
    private Vector3 second;
    private Vector3 distancePositionBetweenTwoFrame;

    [SerializeField] private float slideSpeed = 1.0f;
    [SerializeField] private float speed = 1.0f;

    [SerializeField] private ArrowDeckManager arrowDeck;

    private float range => levelManager.roadData.roadMeshSettings.roadWidth;

    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private LevelManager levelManager;

    public EndOfPathInstruction end;

    private float dstTravelled;

    private void Update()
    {
        InputDetection();
        FollowPath();
        Slide();
        ApplyScaleForSlide();
    }

    private void ApplyScaleForSlide()
    {
        float distance = Mathf.Abs(arrowDeck.transform.localPosition.x);
        arrowDeck.transform.DOScaleX(Mathf.Clamp(range * 2 - distance, .5f, 1), .1f);
    }

    private void InputDetection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            first = Input.mousePosition;
            second = first;
        }

        if (Input.GetMouseButtonUp(0))
        {
            distancePositionBetweenTwoFrame = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            first = Input.mousePosition;
            distancePositionBetweenTwoFrame = first - second;
        }

        second = first;
    }

    private void FollowPath()
    {
        dstTravelled += speed * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(dstTravelled, end) + new Vector3(0, .4f, 0);
        Quaternion pathRotation = pathCreator.path.GetRotationAtDistance(dstTravelled, end);
        Vector3 rotation = new Vector3(pathRotation.eulerAngles.x, pathRotation.eulerAngles.y, 0);
        transform.eulerAngles = rotation;
    }

    private void Slide()
    {
        Vector3 horizontal = new Vector3(distancePositionBetweenTwoFrame.x, 0, 0);

        Vector3 newPosition = arrowDeck.transform.localPosition + (slideSpeed * horizontal) * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -range, range);

        arrowDeck.transform.localPosition = arrowDeck.transform.localPosition.SetOnlyOneAxisVector(Axis.X, newPosition.x);
    }
}