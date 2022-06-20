using UnityEngine;
using DG.Tweening;
using PathCreation;

public class ArrowController : MonoBehaviour
{
    private Vector3 first;
    private Vector3 second;
    private Vector3 distancePositionBetweenTwoFrame;

    [SerializeField] private float slideSpeed = 1.0f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float stairRunSpeed = 1.3f;

    [SerializeField] private ArrowDeckManager arrowDeck;
    private float range; // 0.5f

    private VertexPath path;
    public EndOfPathInstruction end;
    private bool isPathCompleted;
    private bool isFirstInputDetected;
    private bool isStairRunActive;
    private float dstTravelled;

    public void Initialize(VertexPath followPath, float width)
    {
        path = followPath;
        range = width;
        this.transform.position = path.GetPoint(0);
        Vector3 rotation = new Vector3(path.GetRotation(0).eulerAngles.x, path.GetRotation(0).eulerAngles.y, 0);
        this.transform.eulerAngles = rotation;

        isFirstInputDetected = false;
        isPathCompleted = false;
        EventManager.Instance.OutOfArrow.AddListener(() => isStairRunActive = false);
    }

    private void Update()
    {
        InputDetection();
        FollowPath();
        Slide();
        ApplyScaleForSlide();
        MoveInfinity();
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
            isFirstInputDetected = true;
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
        if (!isFirstInputDetected || isPathCompleted)
            return;

        dstTravelled += speed * Time.deltaTime;
        transform.position = path.GetPointAtDistance(dstTravelled, end) + new Vector3(0, .4f, 0);
        Quaternion pathRotation = path.GetRotationAtDistance(dstTravelled, end);
        Vector3 rotation = new Vector3(pathRotation.eulerAngles.x, pathRotation.eulerAngles.y, 0);
        transform.eulerAngles = rotation;

        if (path.GetPoint(path.NumPoints - 1).DistanceAnAxis(transform.position, Axis.Z) < 4.0f)
        {
            Debug.LogError("Finished");
            isPathCompleted = true;
            isStairRunActive = true;
            arrowDeck.OnStepReached(1.0m);
            arrowDeck.transform.DOLocalMoveX(0, .5f);
            Camera.main.transform.DOLocalMove(new Vector3(0.0f, 3.5f, -6.5f), 1.0f);
        }
    }

    private void Slide()
    {
        Vector3 horizontal = new Vector3(distancePositionBetweenTwoFrame.x, 0, 0);

        Vector3 newPosition = arrowDeck.transform.localPosition + (slideSpeed * horizontal) * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, -range, range);

        arrowDeck.transform.localPosition = arrowDeck.transform.localPosition.SetOnlyOneAxisVector(Axis.X, newPosition.x);
    }

    private void MoveInfinity()
    {
        if (!isStairRunActive)
            return;

        this.transform.Translate(Vector3.forward * Time.deltaTime * stairRunSpeed);
    }
}