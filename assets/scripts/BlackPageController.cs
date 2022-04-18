using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackPageController : MonoBehaviour
{

    private Image img;
    private float speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        img = gameObject.GetComponent <Image> ();
    }

    // Update is called once per frame
    void Update()
    {
        img.color -= new Color(0.0f, 0.0f, 0.0f, speed * Time.deltaTime);
        if (img.color.a <= 0.3f) {
            img.color += new Color(0.0f, 0.0f, 0.0f, 1.0f);
            gameObject.SetActive(false);
        }
    }
}
