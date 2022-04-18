using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletController : MonoBehaviour
{
    public float buletSpeed;
    public float destroyPos;
    public Vector3 movementRoute;

    void Start()
    {
        //print("Tangens 20 = "+Math.Tan(20 * (Math.PI/180)));
        //print("Tangens 30 = "+Math.Tan(30 * (Math.PI/180)));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += movementRoute * (buletSpeed * Time.deltaTime);    
        if (transform.position.x > destroyPos){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "DamageZone") {
            Destroy(gameObject);
        }
    }
}
