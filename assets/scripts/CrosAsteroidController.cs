using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosAsteroidController : MonoBehaviour {

	public static event EventManager.AsteroidDelegate OnAsteroidCrash;
	
	public ParticleSystem destroyEffect;

	public AudioSource soundCrash;
	public AudioClip[] clipsCrash;
	public AudioClip[] clipsCrashAlone;

	private float shiftSpeed=0.0f;

	void Start () {
		shiftSpeed = transform.parent.GetComponent<Parallaxer>().shiftSpeed;
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
			OnAsteroidCrash(transform, shiftSpeed);
			Instantiate(destroyEffect, transform.position, Quaternion.identity);
			gameObject.transform.position = Vector3.one * 1000;			
			soundCrash.PlayOneShot(clipsCrashAlone[Random.Range(0, clipsCrashAlone.Length)], 0.5f);				
			//obj.transform.position = Vector3.one * 1000;
		}		
	}

}
