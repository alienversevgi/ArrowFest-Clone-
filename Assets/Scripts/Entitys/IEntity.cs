using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity : IResettable
{
    void SetPositionAndEnable(Vector3 newPosition);
}
