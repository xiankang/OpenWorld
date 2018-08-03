using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Asset;
using Core.RepresentLogic;

public class Game : MonoBehaviour {
    public LifeCycle _LifeCycle { get; private set; } = null;

    private void Awake()
    {
        //Debug.Log("Game Awake()");
    }

    private void OnEnable()
    {
        //Debug.Log("Game OnEnable()");
    }

    // Use this for initialization
    void Start () {
        _LifeCycle = gameObject.AddComponent<LifeCycle>();
        _LifeCycle.Add(0, new InteractionManager());

        GameEnv._InputManager = new GameObject("_InputManager").AddComponent<InputManager>();

        StartCoroutine(DoInit());
	}

    public IEnumerator DoInit()
    {
        yield return ResourceManager.Init();

        yield return _LifeCycle.Singletons.InitCoroutine(0, RefreshProgress);

        GameEnv._InputManager = new GameObject("_InputManager").AddComponent<InputManager>();

        string mapPath = "Game/Map/CactusPack/World";
        yield return World.DoLoadMap(mapPath);

        string heroPath = "Game/Model/M2_4";
        yield return World.DoLoadHero(heroPath);
    }

    IEnumerator RefreshProgress(string moduleName, int moduleIndex, int moduleCount, string moduleSubInfo)
    {
        Debug.LogFormat("初始化{0} {1}/{2}", moduleName, moduleIndex, moduleCount);

        yield return 1;
    }

    private void FixedUpdate()
    {
        //Debug.Log("Game FixedUpdate()");
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("Game Update()");
    }

    private void LateUpdate()
    {
        //Debug.Log("Game LateUpdate()");
    }

    private void OnWillRenderObject()
    {
        //Debug.Log("Game OnWillRenderObject()");
    }

    private void OnGUI()
    {
        //Debug.Log("Game OnGUI()");
    }

    private void OnDisable()
    {
        //Debug.Log("Game OnDisable()");
    }

    private void OnDestroy()
    {
        //Debug.Log("Game OnDestroy()");
    }
}

