using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using EnverPool;

namespace Game.Level
{
    public class Chibi : Entity, IDamageable<int>
    {
        #region Fields

        [SerializeField] private int _hp;
        [SerializeField] private List<Collider> _subColliders;

        private bool _isDead;
        private Animator _animator;
        private bool _isPoolable;

        #endregion

        #region Unity Methods

        private void OnCollisionEnter(Collision collision)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();

            if (arrow != null && !_isDead)
            {
                _isDead = true;
                GhostArrowData ghostArrowData = new GhostArrowData(collision.contacts[0]);
                ExecuteCreateGhostArrowSequence(HittableBodyPart.Default, ghostArrowData);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(bool isPoolable = true)
        {
            _isPoolable = isPoolable;
            _isDead = false;
            _animator = this.GetComponent<Animator>();
            _subColliders = this.transform.GetComponentsInChildren<Collider>().ToList();
            _animator.SetTrigger("Idle");
        }

        public void Move()
        {
            _animator.SetTrigger("Run");
        }

        public void Jump()
        {
            _animator.SetTrigger("Jump");
            Vector3 jumpEndPosition = this.transform.localPosition;
            this.transform.DOLocalJump(jumpEndPosition, .4f, 1, 1f);
        }

        public void ExecuteCreateGhostArrowSequence(HittableBodyPart hittableBodyPart = HittableBodyPart.Default, GhostArrowData ghostArrowData = null)
        {
            if (hittableBodyPart == HittableBodyPart.Default)
            {
                CreateGhostArrowOnBody(ghostArrowData);
                int remaingHP = _hp;

                for (int i = 0; i < remaingHP; i++)
                {
                    int randomIndex = Random.Range(0, _subColliders.Count);
                    CreateGhostArrowOnBodyPart((HittableBodyPart)randomIndex);
                }
            }
            else
            {
                int remaingHP = _hp;

                for (int i = 0; i < remaingHP; i++)
                {
                    CreateGhostArrowOnBodyPart(hittableBodyPart);
                }
            }
        }

        public void Damage(int value)
        {
            _hp -= value;
            EventManager.Instance.OnDamageTaken.Raise();
        }

        public void SetStepChibi()
        {
            _isPoolable = true;
            _animator.SetTrigger("Idle");
        }

        #endregion

        #region Private Methods

        private void CreateGhostArrowOnBodyPart(HittableBodyPart hittableBodyPart = HittableBodyPart.Default)
        {
            Collider selectedCollider = _subColliders[(int)hittableBodyPart];
            Bounds selectedColliderBound = selectedCollider.bounds;
            Vector3 randomPositionOnCollider = GetRandomPositionOnCollider(selectedColliderBound);

            #region Log

            string message = " ";
            message += " name : " + selectedCollider.name + '\n';
            message += " min : " + selectedColliderBound.min + '\n';
            message += " max : " + selectedColliderBound.max + '\n';
            message += " center : " + selectedColliderBound.center + '\n';
            message += " extends : " + selectedColliderBound.extents + '\n';
            message += " randomPositionOnCollider : " + randomPositionOnCollider + '\n';
            message += " ClosestPoint : " + selectedCollider.ClosestPoint(randomPositionOnCollider) + '\n';
            message += " ClosestPointOnBounds : " + selectedCollider.ClosestPointOnBounds(randomPositionOnCollider) + '\n';
            //Debug.Log(message);

            #endregion

            GhostArrowData ghostArrowData = new GhostArrowData(randomPositionOnCollider, selectedCollider.transform);
            CreateGhostArrowOnBody(ghostArrowData);
        }

        private void CreateGhostArrowOnBody(GhostArrowData ghostArrowData)
        {
            Entity ghostArrow = PoolManager.Instance.GhostArrowPool.Allocate();
            ghostArrow.SetPositionAndEnable(ghostArrowData.Position);
            ghostArrow.transform.SetParent(ghostArrowData.ParentPartTransform);

            Utility.Timer.Instance.StartTimer(1.0f, () => PoolManager.Instance.GhostArrowPool.Release(ghostArrow));

            if (_isPoolable)
                Utility.Timer.Instance.StartTimer(1.0f, () => PoolManager.Instance.ChibiPool.Release(this));

            if (this.transform.localEulerAngles.y > 0)
            {
                _animator.SetTrigger("FallBack");
            }
            else
            {
                _animator.SetTrigger("FallFront");
            }

            Damage(1);
        }

        private Vector3 GetRandomPositionOnCollider(Bounds selectedColliderBound)
        {
            Vector3 randomPositionOnCollider = Vector3.zero;
            bool inCenter;
            if (selectedColliderBound.min.x == selectedColliderBound.max.x
                || selectedColliderBound.min.y == selectedColliderBound.max.y
                )
            {
                inCenter = true;
            }
            else
            {
                inCenter = Random.value > 0.5f;
            }

            if (inCenter)
            {
                randomPositionOnCollider = selectedColliderBound.center;
            }
            else
            {
                randomPositionOnCollider = new Vector3
                    (Random.Range(selectedColliderBound.min.x + .005f, selectedColliderBound.max.x - .005f),
                    Random.Range(selectedColliderBound.min.y + .005f, selectedColliderBound.max.y - .005f),
                    Random.Range(selectedColliderBound.min.z + .005f, selectedColliderBound.max.z - .005f));
            }

            return randomPositionOnCollider;
        }

        #endregion
    }
}