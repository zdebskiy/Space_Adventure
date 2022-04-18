using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour {
	
	public static event EventManager.AsteroidDelegate OnAsteroidCrash;

	public Vector3 axis = Vector3.forward;
	public ParticleSystem destroyEffect;

	[System.Serializable]
	public struct RotationAngleRange {
		public float min;
		public float max;
	}

	public RotationAngleRange rotationAngleRange;

	public AudioClip[] clipsCrash;
	public AudioClip[] clipsCrashAlone;
	public AudioSource soundCrash;

	private float rotationAngle;
	private float shiftSpeed=0.0f;

	// Use this for initialization
	void Start () {
		//Transform t = transform;
		rotationAngle = Random.Range(rotationAngleRange.min, rotationAngleRange.max);
		shiftSpeed = transform.parent.GetComponent<Parallaxer>().shiftSpeed;
	}

	// Update is called once per frame
	void Update () {

		if ((GameManager.gamePause) || (GameManager.gameOver)) return;
		//t.rotation.z(rotationSpeed + Time.deltaTime);		

		transform.RotateAround(transform.position, axis, rotationAngle*Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D col) {

		if (col.gameObject.tag == "Player") {
			Instantiate(destroyEffect, transform.position, Quaternion.identity);
			gameObject.transform.position = Vector3.one * 1000;		
			//obj.transform.position = Vector3.one * 1000;			
			if (GameManager.curHealth != 1){
				soundCrash.PlayOneShot(clipsCrash[Random.Range(0, clipsCrash.Length)], 0.5f);	
			}			
		}
		if (col.gameObject.tag == "Bullet") {
			OnAsteroidCrash(transform, shiftSpeed); // 
			Instantiate(destroyEffect, transform.position, Quaternion.identity);
			gameObject.transform.position = Vector3.one * 1000;		
			soundCrash.PlayOneShot(clipsCrashAlone[Random.Range(0, clipsCrashAlone.Length)], 0.5f);	
			//obj.transform.position = Vector3.one * 1000;						
		}

	}
}
