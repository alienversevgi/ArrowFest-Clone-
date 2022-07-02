using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EnverPool;

namespace Game.Level
{
    public class ChibiGroup : Entity
    {
        #region Fields

        [SerializeField] private float _speed = 1.0f;
        [SerializeField] private List<Transform> _rows;

        private Dictionary<int, List<Chibi>> _rowChibis;
        private List<RowChibiData> _rowChibiDatas;
        private VertexPath _movePath;

        private float _dstTravelled;
        private bool _isJumpActivated;

        private Coroutine _killCoroutine;

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            if (DetectJumpState())
            {
                Jump();
            }

            DetectArrowAndExecuteAction();
        }

        #endregion

        #region Public Methods

        public void Initialize(List<RowChibiData> rowChibiDatas, VertexPath followPath, Transform pathCreatorTranform)
        {
            _rowChibiDatas = rowChibiDatas;
            _rowChibis = new Dictionary<int, List<Chibi>>();

            for (int i = 0; i < _rowChibiDatas.Count; i++)
            {
                _rowChibis.Add(i, _rows[i].GetComponentsInChildren<Chibi>().ToList());

                for (int j = 0; j < _rowChibiDatas[i].Chibis.Count; j++)
                {
                    _rowChibis[i][j].Initialize(false);
                    _rowChibis[i][j].SetLocalPositionAndEnable(_rowChibiDatas[i].Chibis[j].Position);
                }
            }

            _movePath = CreateMovePath(followPath, pathCreatorTranform);

            _dstTravelled = 0;
            _isJumpActivated = false;
        }

        public override void Reset()
        {
            base.Reset();

            for (int i = 0; i < _rowChibiDatas.Count; i++)
            {
                for (int j = 0; j < _rowChibiDatas[i].Chibis.Count; j++)
                {
                    _rowChibis[i][j].transform.SetParent(_rows[i]);
                    _rowChibis[i][j].SetLocalPositionAndEnable(_rowChibiDatas[i].Chibis[j].Position);
                }
            }
        }

        #endregion

        #region Private Methods

        private VertexPath CreateMovePath(VertexPath followPath, Transform pathCreatorTranform)
        {
            List<Vector3> points = followPath.GetAllPoints;

            points.RemoveAll(it => it.z < this.transform.position.z);
            points.Insert(0, this.transform.position);

            BezierPath bezierPath = new BezierPath(points);
            VertexPath vertexPath = new VertexPath(bezierPath, pathCreatorTranform);

            return vertexPath;
        }

        private void Move()
        {
            _rowChibis.ToList().ForEach(chibis => chibis.Value.ForEach(chibi => chibi.Move()));

            _dstTravelled += _speed * Time.deltaTime;
            transform.position = _movePath.GetPointAtDistance(_dstTravelled, EndOfPathInstruction.Stop);
            Quaternion pathRotation = _movePath.GetRotationAtDistance(_dstTravelled, EndOfPathInstruction.Stop);
            Vector3 rotation = new Vector3(pathRotation.eulerAngles.x, pathRotation.eulerAngles.y, 0);
            transform.eulerAngles = rotation;
        }

        private void Jump()
        {
            _isJumpActivated = true;
            StartCoroutine(JumpRowChibis());
        }

        private void Kill()
        {
            if (_killCoroutine != null)
                return;

            _killCoroutine = StartCoroutine(KillRowChibis());
        }

        private bool DetectJumpState()
        {
            if (_isJumpActivated)
                return false;

            bool isGateReached = false;
            RaycastHit hit;
            int gateLayer = 1 << 7;

            if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, Mathf.Infinity, gateLayer))
            {
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
        private IEnumerator KillRowChibis()
        {
            for (int i = 0; i < _rowChibis.Count; i++)
            {
                for (int j = 0; j < _rowChibis[i].Count; j++)
                {
                    _rowChibis[i][j].ExecuteCreateGhostArrowSequence(_rowChibiDatas[i].Chibis[j].HittableBodyPart);
                    _rowChibis[i][j].transform.SetParent(null);
                    yield return new WaitForSecondsRealtime(.05f);
                }
            }

            Utility.Timer.Instance.StartTimer(1.0f, () => PoolManager.Instance.ChibiGroupPool.Release(this));
        }

        private IEnumerator JumpRowChibis()
        {
            foreach (List<Chibi> rowChibis in _rowChibis.Values)
            {
                rowChibis.ForEach(chibi => chibi.Jump());
                yield return new WaitForSecondsRealtime(.1f);
            }
        }

        #endregion
    }
}