using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChibiGroup : MonoBehaviour, IEntity
{
    #region Fields

    [SerializeField] private List<Transform> rows;
    private Dictionary<int, List<Chibi>> rowChibies;
    private float dstTravelled;
    private VertexPath movePath;
    public EndOfPathInstruction end;
    [SerializeField] private float speed = 1.0f;
    private bool isMoveActive;
    private bool isEnable;
    private float t;
    private bool isJumpActivated;
    private Pool<Entity> ghostPool;

    private Coroutine killCoroutine;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        isEnable = true;
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    Arrow arrowController = other.transform.GetComponent<Arrow>();

    //    if (arrowController != null)
    //    {
    //        isMoveActive = true;
    //    }
    //}

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Kill();
        }

        if (DetectJumpState())
        {
            Jump();
        }

        DetectArrowAndExecuteAction();
    }

    #endregion

    #region Public Methods

    public void Initialize(VertexPath followPath, Transform pathCreatorTranform)
    {
        rowChibies = new Dictionary<int, List<Chibi>>();

        for (int i = 0; i < rows.Count; i++)
        {
            rowChibies.Add(i, rows[i].GetComponentsInChildren<Chibi>().ToList());
        }

        movePath = CreateMovePath(followPath, pathCreatorTranform);

        foreach (List<Chibi> chibies in rowChibies.Values)
        {
            chibies.ForEach(chibi => chibi.Initialize(1));
        }
    }

    public void Reset()
    {
        this.gameObject.SetActive(false);
        this.transform.position = Vector3.zero;
    }

    public void SetPositionAndEnable(Vector3 newPosition)
    {
        this.transform.position = newPosition;
        gameObject.SetActive(true);
    }

    #endregion

    #region Private Methods

    private VertexPath CreateMovePath(VertexPath followPath, Transform pathCreatorTranform)
    {
        List<Vector3> points = followPath.GetAllPoints;

        string message = "";
        for (int i = 0; i < points.Count; i++)
        {
            message += "\n FIRST " + i + " " + points[i];
        }
        //Debug.Log(message);

        points.RemoveAll(it => it.z < this.transform.position.z);
        points.Insert(0, this.transform.position);

        message = "";
        for (int i = 0; i < points.Count; i++)
        {
            message += "\n SECOND " + i + " " + points[i];
        }
        //Debug.Log(message);

        BezierPath bezierPath = new BezierPath(points);
        VertexPath vertexPath = new VertexPath(bezierPath, pathCreatorTranform);

        return vertexPath;
    }

    private void Move()
    {
        rowChibies.ToList().ForEach(it => it.Value.ForEach(ik => ik.Move()));

        dstTravelled += speed * Time.deltaTime;
        transform.position = movePath.GetPointAtDistance(dstTravelled, end);
        Quaternion pathRotation = movePath.GetRotationAtDistance(dstTravelled, end);
        Vector3 rotation = new Vector3(pathRotation.eulerAngles.x, pathRotation.eulerAngles.y, 0);
        transform.eulerAngles = rotation;
    }

    private void Jump()
    {
        isJumpActivated = true;
        StartCoroutine(JumpRowChibies());
    }

    private void Kill()
    {
        if (killCoroutine != null)
            return;

        killCoroutine = StartCoroutine(KillRowChibies());
    }

    private bool DetectJumpState()
    {
        if (isJumpActivated)
            return false;

        bool isGateReached = false;
        RaycastHit hit;
        int gateLayer = 1 << 7;

        if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, Mathf.Infinity, gateLayer))
        {
            //Debug.Log("distance " + hit.distance + " name " + hit.collider.name);
            if (hit.distance < 1)
            {
                isGateReached = true;
            }
        }

        return isGateReached;
    }

    private void DetectArrowAndExecuteAction()
    {
        RaycastHit hit;
        int arrowControllerLayer = 1 << 8;
        if (Physics.Raycast(this.transform.position, this.transform.TransformDirection(Vector3.back), out hit, Mathf.Infinity, arrowControllerLayer))
        {
            if (hit.distance < 2.0f)
            {
                Kill();
            }

            if (hit.distance < 3.0f)
            {
                Move();
            }
        }
    }

    private IEnumerator KillRowChibies()
    {
        for (int i = 0; i < rowChibies.Count; i++)
        {
            for (int j = 0; j < rowChibies[i].Count; j++)
            {
                rowChibies[i][j].ExecuteCreateGhostArrowSequence(HittableBodyPart.Body);
                rowChibies[i][j].transform.SetParent(null);
                yield return new WaitForSecondsRealtime(.05f);
            }
        }

        isEnable = false;
        /*
        foreach (List<Chibi> rowChibies in rowChibies.Values)
        {
            rowChibies.ForEach(chibi => chibi.ExecuteCreateGhostArrowSequence(HittableBodyPart.Body));
            yield return new WaitForSecondsRealtime(.2f);
        }
        */
    }

    private IEnumerator JumpRowChibies()
    {
        foreach (List<Chibi> rowChibies in rowChibies.Values)
        {
            rowChibies.ForEach(chibi => chibi.Jump());
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    #endregion
}