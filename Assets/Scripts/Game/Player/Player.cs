using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Asset;

public class Player : GameEntity {
    private static string defaultHero = "Caprice";
    private static string heroPath = "Game/Heroes";
    public Hero _hero;

    public void SelectHero(string heroName)
    {
        string heroFile = string.Format("{0}/{1}/{2}", heroPath, heroName, heroName);
        GameObject hero = ResourceManager.LoadPrefab(heroFile);
        hero.transform.parent = transform;
        _hero = hero.GetComponent<Hero>();
    }
	// Use this for initialization
	protected void Start () {
        base.Start();
        //这里先选择默认英雄
        SelectHero(defaultHero);
        //设置摄相机
        SetCamera();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetCamera()
    {
        Transform cameraSlot = _hero.transform.Find("camera_slot");
        Camera.main.transform.parent = cameraSlot;
        Camera.main.transform.localPosition = new Vector3(0, -0.35f, -1);
    }
}
