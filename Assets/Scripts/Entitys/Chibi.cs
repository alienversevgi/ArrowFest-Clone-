using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Chibi : MonoBehaviour, IEntity, IDamageable<int>
{
    #region Fields

    private int hp;
    public int HP
    {
        get
        {
            return hp;
        }
        private set
        {
            hp = value;
        }
    }

    public bool IsDead;

    private Animator animator;
    [SerializeField] private List<Collider> subColliders;
    //private HittableBodyPart hittableBodyPart;

    private Vector3 firstPosition;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        IsDead = false;
        animator = this.GetComponent<Animator>();
        subColliders = this.transform.GetComponentsInChildren<Collider>().ToList();
        firstPosition = this.transform.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Arrow arrow = collision.gameObject.GetComponent<Arrow>();
        if (arrow != null && !IsDead)
        {
            IsDead = true;
            GhostArrowData ghostArrowData = new GhostArrowData(collision.contacts[0]);
            ExecuteCreateGhostArrowSequence(HittableBodyPart.Default, ghostArrowData);
        }
    }

    #endregion

    #region Public Methods

    public void Initialize(int hp)
    {
        this.hp = hp;
    }

    public void Move()
    {
        animator.SetTrigger("Run");
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
        Vector3 jumpEndPosition = this.transform.localPosition;
        this.transform.DOLocalJump(jumpEndPosition, .4f, 1, 1f);
    }

    public void ExecuteCreateGhostArrowSequence(HittableBodyPart hittableBodyPart = HittableBodyPart.Default, GhostArrowData ghostArrowData = null)
    {
        if (hittableBodyPart == HittableBodyPart.Default)
        {
            CreateGhostArrowOnBody(ghostArrowData);
            int remaingHP = hp;

            for (int i = 0; i < remaingHP; i++)
            {
                int randomIndex = Random.Range(0, subColliders.Count);
                CreateGhostArrowOnBodyPart((HittableBodyPart)randomIndex);
            }
        }
        else
        {
            int remaingHP = hp;

            for (int i = 0; i < remaingHP; i++)
            {
                CreateGhostArrowOnBodyPart(hittableBodyPart);
            }
        }
    }
    public void Damage(int value)
    {
        hp -= value;
        EventManager.Instance.OnDamageTaken.Invoke(value);
    }

    public void SetPositionAndEnable(Vector3 newPosition)
    {
        this.transform.position = newPosition;
        this.gameObject.SetActive(true);
    }

    public void Reset()
    {
        this.gameObject.SetActive(false);
        this.transform.position = Vector3.zero;
    }

    #endregion

    #region Private Methods

    private void CreateGhostArrowOnBodyPart(HittableBodyPart hittableBodyPart = HittableBodyPart.Default)
    {
        Collider selectedCollider = subColliders[(int)hittableBodyPart];
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
        //clone.AddComponent<test>().target = ghostArrowData.ParentPartTransform;
        if (this.transform.localEulerAngles.y > 0)
        {
            animator.SetTrigger("FallBack");
        }
        else
        {
            animator.SetTrigger("FallFront");
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