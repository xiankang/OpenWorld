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

    public static IEnumerator DoLoadHero(string heroPath)
    {
        GameObject hero = ResourceManager.LoadPrefab(heroPath);
        Transform cameraSlot = hero.transform.Find("camera_slot");
        Camera.main.transform.parent = cameraSlot;
        Camera.main.transform.localPosition = new Vector3(0, -0.35f, -1);

        yield return null;
    }

    private void Start()
    {

    }
    private void Update()
    {

    }
}
