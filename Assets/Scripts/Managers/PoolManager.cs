using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject gatePrefab;
    [SerializeField] private GameObject chibiPrefab;
    [SerializeField] private GameObject chibiGroupPrefab;
    [SerializeField] private GameObject ghostArrowPrefab;

    public Pool<Arrow> ArrowPool { get; private set; }
    public Pool<Gate> GatePool { get; private set; }
    public Pool<Chibi> ChibiPool { get; private set; }
    public Pool<ChibiGroup> ChibiGroupPool { get; private set; }
    public Pool<Entity> GhostArrowPool { get; private set; }

    public void Initialize()
    {
        ArrowPool = new Pool<Arrow>(new PrefabFactory<Arrow>(arrowPrefab, "Arrows"), 10);
        GatePool = new Pool<Gate>(new PrefabFactory<Gate>(gatePrefab, "Gates"), 10);
        ChibiPool = new Pool<Chibi>(new PrefabFactory<Chibi>(chibiPrefab, "Chibis"), 10);
        ChibiGroupPool = new Pool<ChibiGroup>(new PrefabFactory<ChibiGroup>(chibiGroupPrefab, "ChibiGroups"), 2);
        GhostArrowPool = new Pool<Entity>(new PrefabFactory<Entity>(ghostArrowPrefab, "GhostArrows"), 10);
    }
}
