﻿using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(new Vector3(0.1f * Time.deltaTime * Time.time, 0.2f * Time.deltaTime * Time.time, 0.3f * Time.deltaTime * Time.time) * 100);
	}
}
