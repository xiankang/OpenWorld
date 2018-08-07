using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Asset;

public class World : MonoBehaviour
{
    //
    //private static GameObject hero;
    public static IEnumerator DoLoadMap(string mapPath)
    {
        ResourceManager.LoadPrefab(mapPath);
        yield return null;
    }

    private void Start()
    {

    }
    private void Update()
    {

    }
}
