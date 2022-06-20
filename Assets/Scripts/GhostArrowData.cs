using UnityEngine;

public class GhostArrowData
{
    public Vector3 Position { get; set; }
    public Transform ParentPartTransform { get; set; }
    public ContactPoint ContactPoint { get; set; }

    public GhostArrowData(Vector3 position, Transform parentPartTransform)
    {
        Position = position;
        ParentPartTransform = parentPartTransform;
    }

    public GhostArrowData(ContactPoint contactPoint) : this(contactPoint.point, contactPoint.thisCollider.transform)
    {
        ContactPoint = contactPoint;
    }
}