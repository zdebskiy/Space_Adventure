using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackPageDownController : MonoBehaviour
{

    private Image img;
    private float speed = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        img = gameObject.GetComponent <Image> ();
    }

    // Update is called once per frame
    void Update()
    {
        img.color -= new Color(0.0f, 0.0f, 0.0f, speed * Time.deltaTime);
        if (img.color.a <= 0.0f) {
            gameObject.SetActive(false);
            img.color += new Color(0.0f, 0.0f, 0.0f, 1.0f);            
        }
    }
}
