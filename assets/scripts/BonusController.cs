using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusController : MonoBehaviour
{

    public float shiftSpeed=0.0f;
    public Vector3 shiftVector=Vector3.left;
    private bool isScored=false;

    void Update()
    {
		if ((GameManager.gameNew) || (GameManager.gamePause) || (GameManager.gameOver)) {
			//print ("not_isBak, Pause || gameOver");
			return;	
		}

        if (!isScored){
            Shift ();
        }
    }

    private void Shift(){
        transform.localPosition += shiftVector * (shiftSpeed - (shiftSpeed * RocketManager.speed/10)) * Time.deltaTime;
        if (transform.position.x < -10)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Player") {
			isScored = true;
		}
	}    
}
