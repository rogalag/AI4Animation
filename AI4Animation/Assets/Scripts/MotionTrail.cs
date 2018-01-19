﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class MotionTrail : MonoBehaviour {

	public int TrailLength = 10;
	public float TimeDifference = 0.1f;

	public GameObject Prototype;
	public Queue<GameObject> Instances;

	private System.DateTime Timestamp;

	void Start() {
		Instances = new Queue<GameObject>();
		Timestamp = Utility.GetTimestamp();
		Prototype = CreatePrototype();
	}

	void Update() {
		if(Utility.GetElapsedTime(Timestamp) >= TimeDifference) {
			Timestamp = Utility.GetTimestamp();
			if(Instances.Count >= TrailLength) {
				Utility.Destroy(Instances.Dequeue());
			}
			Instances.Enqueue(CreateInstance());
		}
	}

	void OnRenderObject() {
		/*
		UnityGL.Start();
		int index = 0;
		GameObject previous = null;
		foreach(GameObject instance in Instances) {
			index += 1;
			instance.GetComponent<Transparency>().SetTransparency(0.25f);
			if(index > 1) {
				//UnityGL.DrawLine(previous.transform.position, instance.transform.position, 10f, Utility.Red);
			}
			previous = instance;
		}
		UnityGL.Finish();
		*/
	}

	private GameObject CreatePrototype() {
		GameObject instance = Instantiate(gameObject);
		instance.name = "Prototype";
		instance.SetActive(false);
		instance.hideFlags = HideFlags.HideInHierarchy;
		Cleanup(instance.transform);
		instance.AddComponent<Transparency>();
		return instance;
	}

	private GameObject CreateInstance() {
		GameObject instance = Instantiate(Prototype);
		instance.name = name + " (Motion Trail)";
		instance.SetActive(true);
		Copy(gameObject.transform, instance.transform);
		return instance;
	}

	private void Cleanup(Transform t) {
		foreach(Component c in t.GetComponents<Component>()) {
			if(!(c is Transform)) {
				if(c is Renderer) {
					Renderer r = (Renderer)c;
					if(!r.material.HasProperty("_Color")) {
						Utility.Destroy(c);
					}
				} else {
					Utility.Destroy(c);
				}
			}
		}
		for(int i=0; i<t.childCount; i++) {
			Cleanup(t.GetChild(i));
		}
	}

	private void Copy(Transform original, Transform instance) {
		instance.localPosition = original.localPosition;
		instance.localRotation = original.localRotation;
		instance.localScale = original.localScale;
		for(int i=0; i<original.childCount; i++) {
			Copy(original.GetChild(i), instance.GetChild(i));
		}
	}

}