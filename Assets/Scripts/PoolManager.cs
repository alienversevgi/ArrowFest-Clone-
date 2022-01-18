using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;

    public Pool<Arrow> PlatformPool { get; private set; }

    public void Initialize()
    {
        PlatformPool = new Pool<Arrow>(new PrefabFactory<Arrow>(arrowPrefab, "Arrows"), 10);
    }
}
