using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour {

    private static int fps_ = 0;
    private static int min_fps_ = 0;
    private static int max_fps_ = 0;
    private static int avg_fps_ = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		fps_ = (int)(1.0f / Time.deltaTime);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 200), "Hello World!");
        GUI.Label(new Rect(Screen.width - 120, 10, 100, 100), "FPS: " + fps_);
    }
}
